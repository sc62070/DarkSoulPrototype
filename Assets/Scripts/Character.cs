using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unimotion;
using Photon.Pun;

public class Character : MonoBehaviourPun {

    [Header("Basic")]
    public float health = 100f;
    public float maxHealth = 100f;

    public float stamina = 100f;
    public float maxStamina = 100f;

    [Header("Equipment")]
    public Weapon weapon;
    public Shield shield;

    private float attackRadius = 0.6f;

    [Header("Other")]
    public bool inCombat = false;
    public bool isAttacking = false;
    public bool isBlocking = false;
    public bool isEvading = false;
    public bool isBusy = false;
    public bool isEquipped = false;

    public AttackMove lastAttackMove = null;
    public float lastAttackTimer = 0f;

    public Target target;

    CharacterMotor motor;
    CapsuleCollider capsule;
    AudioSource audioSource;

    public event Action OnDie;

    private float timeSinceStaminaConsume = 0f;
    private float timeSinceStaminaRunout = 0f;
    private float timeSinceNoTarget = 60f;

    // Graphics
    GameObject weaponObject, shieldObject, weaponBackObject, shieldBackObject;

    void Awake() {
        motor = GetComponent<CharacterMotor>();
        capsule = GetComponent<CapsuleCollider>();
        audioSource = gameObject.AddComponent<AudioSource>();

        motor.OnLand += delegate () {
            if (motor.velocity.y < -15f) {
                Damage(30f, -transform.forward);
            }

            photonView.RPC("PlayState", RpcTarget.All, "Land", 0.1f);
            photonView.RPC("PlaySound", RpcTarget.All, SoundClips.LAND);
        };

        RefreshEquipmentGraphics();
    }

    // Update is called once per frame
    void Update() {

        motor.canWalk = !isBusy && !isAttacking && !isEvading;
        motor.canJump = !isBusy && !isAttacking && !isEvading;
        motor.canTurn = !isBusy && !isAttacking && !isEvading;

        if (photonView.IsMine) {
            motor.animator.SetBool("Blocking", isBlocking);

            if (!isAttacking) {
                lastAttackTimer -= Time.deltaTime;
            }
        }

    }

    void LateUpdate() {

        timeSinceStaminaConsume += Time.deltaTime;
        timeSinceStaminaRunout += Time.deltaTime;

        // Regenerate stamina
        if (timeSinceStaminaConsume > 1.5f && timeSinceStaminaRunout > 2.5f && !isAttacking && !isBlocking && !isEvading) {
            stamina = Mathf.Clamp(stamina + 30f * Time.deltaTime, 0f, maxStamina);
        }

        // Prevent blocking if evading or blocked
        if (isBusy || isEvading) {
            isBlocking = false;
        }

        // Manage combat-mode
        if(target == null && !isAttacking) {
            timeSinceNoTarget += Time.deltaTime;
        } else {
            timeSinceNoTarget = 0f;
        }

        inCombat = target != null || timeSinceNoTarget < 4f;

        motor.animator.SetBool("Combat", inCombat);

        weaponObject.SetActive(isEquipped);
        weaponBackObject.SetActive(!isEquipped);
        shieldObject.SetActive(isEquipped);
        shieldBackObject.SetActive(!isEquipped);

    }

    public void Attack(AttackType type) {

        if (photonView.IsMine && !isBusy && !isAttacking && stamina > 0f) {
            isBlocking = false;
            isEvading = false;

            ToggleCombat();

            AttackMove move = type == AttackType.Light ? AttackMoves.slashR : AttackMoves.slashHeavy;

            if (lastAttackMove != null && lastAttackMove.nextAttack != null && lastAttackTimer > 0f) {
                move = lastAttackMove.nextAttack;
            }

            lastAttackTimer = 0.5f;
            lastAttackMove = move;

            photonView.RPC("PlayState", RpcTarget.All, move.stateName, 0.2f);

        }

    }

    [PunRPC]
    public void Stagger() {
        photonView.RPC("PlayState", RpcTarget.All, "Stagger", 0.2f);
    }

    [PunRPC]
    public void Flinch() {
        photonView.RPC("PlayState", RpcTarget.All, "Flinch", 0.2f);
    }

    public void Evade(Direction direction) {

        isBlocking = false;

        if (ConsumeStamina(30f)) {
            switch (direction) {
                case Direction.Left:
                    photonView.RPC("PlayState", RpcTarget.All, "Dodge Left", 0.2f);
                    break;
                case Direction.Right:
                    photonView.RPC("PlayState", RpcTarget.All, "Dodge Right", 0.2f);
                    break;
                case Direction.Up:
                    photonView.RPC("PlayState", RpcTarget.All, "Dodge Front", 0.2f);
                    break;
                case Direction.Down:
                case Direction.None:
                    photonView.RPC("PlayState", RpcTarget.All, "Dodge Back", 0.2f);
                    break;
            }
        }
    }

    public void Block(bool flag) {
        bool tmp = isBlocking;
        isBlocking = false;
        if (flag == true && !isBusy && !isAttacking && !isEvading && motor.Grounded && stamina > 0f) {
            isBlocking = true;
        }
    }

    [PunRPC]
    public void Damage(float q, Vector3 direction) {
        if (health > 0f) {
            health -= q;

            motor.TurnTowards(-direction, CharacterMotor.TurnBehaviour.Instant);
            if (health <= 0f) {
                Kill();
            } else {
                //Stagger();
                Flinch();
            }
        }
    }

    [PunRPC]
    public void AttemptDamage(float q, Vector3 direction, int attackerViewId) {

        Character attacker = PhotonView.Find(attackerViewId).GetComponent<Character>();

        if (isBlocking && Vector3.Angle(transform.forward, attacker.transform.forward) > 90f) {
            attacker.photonView.RPC("Stagger", RpcTarget.All);
            ConsumeStamina(q);
        } else {
            Damage(q, direction);
        }
    }

    public void Heal() {
        if (GetComponent<Inventory>().items.Count > 0) {
            photonView.RPC("PlayState", RpcTarget.All, "Heal", 0.2f);
            photonView.RPC("PlaySound", RpcTarget.All, SoundClips.DRINK_ESTUS);
        }
    }

    [PunRPC]
    public void PlayState(string name, float fade) {
        motor.animator.CrossFadeInFixedTime(name, fade);
    }

    [PunRPC]
    public void PlayState(string name, float fade, int layer) {
        motor.animator.CrossFadeInFixedTime(name, fade, layer);
    }

    [PunRPC]
    public void PlaySound(string soundName) {
        AudioSource source = GetComponent<AudioSource>() != null ? GetComponent<AudioSource>() : gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.PlayOneShot(SoundClips.Get(soundName));
    }

    public void Kill() {
        motor.animator.CrossFadeInFixedTime("Die", 0.2f);
        Destroy(GetComponent<Target>());
        OnDie?.Invoke();
    }

    public void ToggleCombat() {
        //GetComponent<Animator>().runtimeAnimatorController = FindObjectOfType<GameManager>().animations.combatAnimator;
    }

    public bool ConsumeStamina(float q) {
        if (stamina > 0f) {
            stamina = Mathf.Clamp(stamina - q, 0f, maxStamina);

            timeSinceStaminaConsume = 0f;

            if (stamina <= 0f) {
                timeSinceStaminaRunout = 0f;
            }

            return true;
        }
        return false;
    }

    public AudioClip clip;

    public void SendEvent(string evt) {
        if (photonView.IsMine) {
            if (evt.Equals("strike") && isAttacking) {

                // The animation reached the point in which stamina must be consumed
                ConsumeStamina(30f * lastAttackMove.damageMultiplier);

                Collider[] cols = Physics.OverlapSphere(transform.position + transform.forward * attackRadius * 2f + transform.up * 1.3f, attackRadius, LayerMask.GetMask(new string[] { "Characters" }), QueryTriggerInteraction.Ignore);
                foreach (Collider c in cols) {
                    Character character = c.GetComponent<Character>();
                    if (character != null && character != this && !character.isEvading) {
                        character.photonView.RPC("AttemptDamage", RpcTarget.All, 35f, character.transform.position - transform.position, photonView.ViewID);

                        string[] clips = { SoundClips.DAMAGE_01, SoundClips.DAMAGE_02, SoundClips.DAMAGE_03 };
                        character.photonView.RPC("PlaySound", RpcTarget.All, clips[Random.Range(0, clips.Length)]);
                    }
                }
            } else if (evt.Equals("cancelBlockMovement")) {
                //isBusy = false;
            } else if (evt.Equals("drink")) {
                health = Mathf.Clamp(health + 60f, 0f, maxHealth);
                GetComponent<Inventory>().items.RemoveAt(0);
            }
        }

        if (evt.Equals("step") && motor.Grounded) {
            string[] clips = { SoundClips.STEP_STONE_01, SoundClips.STEP_STONE_02, SoundClips.STEP_STONE_03 };
            PlaySound(clips[Random.Range(0, clips.Length)]);
        }

        if (evt.Equals("swing") && isAttacking) {
            string[] clips = { SoundClips.SWING_01, SoundClips.SWING_02, SoundClips.SWING_03, SoundClips.SWING_04 };
            PlaySound(clips[Random.Range(0, clips.Length)]);
        }
    }

    private void RefreshEquipmentGraphics() {
        Transform handleR = transform.FindDeepChild("Handle.R");
        Transform handleL = transform.FindDeepChild("Handle.L");
        Transform weaponHandleBack = transform.FindDeepChild("Socket.Weapon.Back");
        Transform shieldHandleBack = transform.FindDeepChild("Socket.Shield.Back");

        foreach (Transform child in handleR) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in weaponHandleBack) {
            Destroy(child.gameObject);
        }

        if (weapon != null) {
            weaponObject = Instantiate(weapon.prefab);
            weaponObject.transform.localScale = Vector3.one;
            weaponObject.transform.parent = handleR;
            weaponObject.transform.localPosition = Vector3.zero;
            weaponObject.transform.localRotation = Quaternion.Euler(40f, 0f, 90f);

            weaponBackObject = Instantiate(weapon.prefab);
            weaponBackObject.transform.localScale = Vector3.one;
            weaponBackObject.transform.parent = weaponHandleBack;
            weaponBackObject.transform.localPosition = Vector3.zero;
            weaponBackObject.transform.localRotation = Quaternion.identity;
        }

        if (shield != null) {
            shieldObject = Instantiate(shield.prefab);
            shieldObject.transform.localScale = Vector3.one;
            shieldObject.transform.parent = handleL;
            shieldObject.transform.localPosition = Vector3.zero;
            shieldObject.transform.localRotation = Quaternion.Euler(2.3f, -270f, -90f);

            shieldBackObject = Instantiate(shield.prefab);
            shieldBackObject.transform.localScale = Vector3.one;
            shieldBackObject.transform.parent = shieldHandleBack;
            shieldBackObject.transform.localPosition = Vector3.zero;
            shieldBackObject.transform.localRotation = Quaternion.identity;
        }

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Gizmos.DrawSphere(transform.position + transform.forward * attackRadius * 2f + transform.up * 1.3f, attackRadius);
    }

}

public enum AttackType {
    Light, Heavy
}

[System.Serializable]
public class AttackMove {
    public string stateName;
    public AttackType type;
    public float damageMultiplier = 1f;
    public AttackMove nextAttack;
}

public class AttackMoves {

    // Heavy slashes
    public static AttackMove slashHeavy = new AttackMove() {
        stateName = "Slash Heavy",
        type = AttackType.Heavy,
        damageMultiplier = 2f,
        nextAttack = null
    };

    // Light slashes
    public static AttackMove slashRL = new AttackMove() {
        stateName = "Slash RL",
        type = AttackType.Light,
        damageMultiplier = 1f,
        nextAttack = null
    };

    public static AttackMove slashR = new AttackMove() {
        stateName = "Slash R",
        type = AttackType.Light,
        damageMultiplier = 1f,
        nextAttack = slashRL
    };
}