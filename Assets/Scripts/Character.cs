using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;

public class Character : MonoBehaviourPun {

    [Header("Basic")]
    public float health = 100f;
    public float MaxHealth { get { return stats.vitality / 99f * 2000f; } }

    public float stamina = 100f;
    public float maxStamina = 100f;

    public float poise = 100f;

    public Stats stats;

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
    public bool isWeaponDamaging = false;
    public bool isPhysicsEnabled = true;

    [Header("Status")]
    public Interactable selectedInteractable;
    public Ladder currentLadder;

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
    private float timeSinceLastDamage = 60f;

    private Quaternion? directionCorrectionRotation;

    // Graphics
    GameObject weaponObject, shieldObject, weaponBackObject, shieldBackObject;

    void Awake() {
        motor = GetComponent<CharacterMotor>();
        capsule = GetComponent<CapsuleCollider>();
        audioSource = gameObject.AddComponent<AudioSource>();

        health = MaxHealth;

        motor.OnLand += delegate () {
            if (photonView.IsMine) {

                if (motor.velocity.y < -5f) {

                    photonView.RPC("PlayState", RpcTarget.All, "Land", 0.1f);
                    photonView.RPC("PlaySound", RpcTarget.All, SoundClips.LAND);

                    if (motor.velocity.y < -15f) {
                        Damage(30f, -transform.forward);
                    }
                }
            }
        };

        RefreshEquipmentGraphics();
    }

    // Update is called once per frame
    void Update() {
        if (photonView.IsMine) {

            // Manage ladder climbing
            motor.enabled = currentLadder == null && isPhysicsEnabled;
            motor.animator.SetBool("Climbing", currentLadder != null);
            
            if(currentLadder != null) {
                float distanceFromLadder = 0.4f;
                transform.position = currentLadder.transform.position + Vector3.Project(transform.position - currentLadder.transform.position, currentLadder.transform.up) - currentLadder.transform.forward * distanceFromLadder;
                transform.forward = currentLadder.transform.forward;

                float dot = Vector3.Dot(transform.position - currentLadder.transform.position, currentLadder.transform.up);
                if(dot > currentLadder.height - 1.50f) {
                    currentLadder = null;
                    photonView.RPC("PlayState", RpcTarget.All, "Climb Up", 0.2f);
                } else if (dot < 0f) {
                    currentLadder = null;
                }
            }

            motor.canWalk = !isBusy && !isAttacking && !isEvading;
            motor.canJump = !isBusy && !isAttacking && !isEvading;
            motor.canTurn = !isBusy && !isAttacking && !isEvading;

            // Turn torwards target
            if(target != null && motor.canTurn) {
                motor.TurnTowards((target.transform.position - transform.position), CharacterMotor.TurnBehaviour.Normal);
            }

            // Manage poise
            if (poise <= 0f) {
                Flinch();
                poise = 100f;
            }

            motor.animator.SetBool("Blocking", isBlocking);

            if (!isAttacking) {
                lastAttackTimer -= Time.deltaTime;
            }

            // Manage timers
            timeSinceLastDamage += Time.deltaTime;
        }

    }

    void LateUpdate() {

        // Check around for interactables
        selectedInteractable = null;
        Interactable[] interactables = FindObjectsOfType<Interactable>();
        foreach (Interactable i in interactables) {
            if (Vector3.Distance(transform.position, i.transform.position) < 2f && !(i is Ladder)) {
                selectedInteractable = i;
                break;
            } else if (i is Ladder && Vector3.Distance(transform.position, i.transform.position + Vector3.Project(transform.position - i.transform.position, i.transform.up)) < 2f) {
                //Debug.Log(Vector3.Distance(transform.position, i.transform.position + Vector3.Project(transform.position - i.transform.position, i.transform.up)));
                selectedInteractable = i;
                break;
            }
        }

        if (photonView.IsMine) {
            if (!isAttacking) {
                timeSinceStaminaConsume += Time.deltaTime;
                timeSinceStaminaRunout += Time.deltaTime;
            }

            // Regenerate stamina
            if (timeSinceStaminaConsume > 0.7f && timeSinceStaminaRunout > 1.5f && !isAttacking && !isBlocking && !isEvading) {
                stamina = Mathf.Clamp(stamina + 30f * Time.deltaTime, 0f, maxStamina);
            }

            // Prevent blocking if evading or blocked
            if (isBusy || isEvading) {
                isBlocking = false;
            }

            // Manage combat-mode
            if (target == null && !isAttacking) {
                timeSinceNoTarget += Time.deltaTime;
            } else {
                timeSinceNoTarget = 0f;
            }

            // Manage direction correction
            if (directionCorrectionRotation != null) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, directionCorrectionRotation.Value, 400f * Time.deltaTime);
                if(transform.rotation.AlmostEquals(directionCorrectionRotation.Value, 0.1f) || !isAttacking) {
                    directionCorrectionRotation = null;
                }

                /*transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, -motor.GetGravity());
                directionCorrectionRotation = null;*/
            }

            // Manage weapon Damage
            /*if (!isWeaponDamaging || !isAttacking) {
                alreadyDamaged.Clear();
            }*/

            inCombat = target != null || timeSinceNoTarget < 4f;

            motor.animator.SetBool("Combat", inCombat);
        }

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

    public void Attack(AttackMove move) {

        if (photonView.IsMine && !isBusy && !isAttacking && stamina > 0f) {
            isBlocking = false;
            isEvading = false;

            ToggleCombat();

            lastAttackTimer = 0.5f;
            lastAttackMove = move;

            photonView.RPC("PlayState", RpcTarget.All, move.stateName, 0.2f);

        }

    }

    [PunRPC]
    public void Stagger() {
        if (photonView.IsMine) {
            photonView.RPC("PlayState", RpcTarget.All, "Stagger", 0.2f);
        }
    }

    [PunRPC]
    public void Flinch() {
        if (photonView.IsMine) {
            photonView.RPC("PlayState", RpcTarget.All, "Flinch", 0.2f);
        }
    }

    [PunRPC]
    public void Flinch(Vector3 knockback) {
        if (photonView.IsMine) {
            Flinch();
            motor.velocity = knockback;
        }
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

    public void Interact() {
        if(selectedInteractable != null && selectedInteractable.IsInteractable) {
            if(selectedInteractable is Ladder) {
                currentLadder = (Ladder) selectedInteractable;
                float dot = Vector3.Dot(transform.position - currentLadder.transform.position, currentLadder.transform.up);
                dot = Mathf.Clamp(dot, 0f, currentLadder.height - 1.5f - 0.1f);
                transform.position = currentLadder.transform.position + currentLadder.transform.up * dot;
                return;
            } else {
                selectedInteractable.Interact(this);
            }
        }
    }

    public void Climb(float magnitude) {
        transform.position = transform.position + currentLadder.transform.up * magnitude * Time.deltaTime;
        motor.animator.SetFloat("Climbing Magnitude", magnitude);
    }

    [PunRPC]
    public void Damage(float q, Vector3 direction) {
        if (photonView.IsMine) {
            if (health > 0f) {
                health -= q;

                timeSinceLastDamage = 0f;

                motor.TurnTowards(-direction, CharacterMotor.TurnBehaviour.Instant);

                if (health <= 0f) {
                    Kill();
                } else {
                    //Stagger();
                    
                }
            }
        }
    }

    [PunRPC]
    public void AttemptDamage(float q, Vector3 direction, int attackerViewId) {
        if (photonView.IsMine) {

            Character attacker = PhotonView.Find(attackerViewId).GetComponent<Character>();

            if (!isEvading && timeSinceLastDamage > 0.5f) {

                Debug.Log(attacker.gameObject.name + " attempted damage on " + gameObject.name);

                if (isBlocking && Vector3.Angle(transform.forward, attacker.transform.forward) > 90f) {

                    // This character blocked the incoming attack
                    photonView.RPC("PlayState", RpcTarget.All, "Block Hit", 0.2f);

                    string[] clips = { SoundClips.BLOCK_01, SoundClips.BLOCK_02 };
                    photonView.RPC("PlaySound", RpcTarget.All, clips[Random.Range(0, clips.Length)]);

                    ConsumeStamina(q);

                    timeSinceLastDamage = 0f;

                    // Knockback for blocking
                    Debug.Log("Knockbacking for " + (direction.normalized * 10f).magnitude);
                    motor.velocity = direction.normalized * 10f;
                } else {

                    // The incoming attack must succesfully damage this character
                    Damage(q, direction);

                    if(health > 0f) {
                        //Flinch(direction.normalized * 8f);
                        poise -= q;
                    }

                    // Play the blood effect
                    EffectManager.instance.PlayBlood(weaponObject.transform.position, attacker.transform.forward);

                    string[] clips = { SoundClips.DAMAGE_01, SoundClips.DAMAGE_02, SoundClips.DAMAGE_03 };
                    photonView.RPC("PlaySound", RpcTarget.All, clips[Random.Range(0, clips.Length)]);
                }
            }
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
        motor.animator.SetBool("Character/Dead", true);
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
                        //character.photonView.RPC("AttemptDamage", RpcTarget.All, 35f, character.transform.position - transform.position, photonView.ViewID);

                        /*string[] clips = { SoundClips.DAMAGE_01, SoundClips.DAMAGE_02, SoundClips.DAMAGE_03 };
                        character.photonView.RPC("PlaySound", RpcTarget.All, clips[Random.Range(0, clips.Length)]);*/
                    }
                }
            } else if (evt.Equals("cancelBlockMovement")) {
                //isBusy = false;
            } else if (evt.Equals("drink")) {
                health = Mathf.Clamp(health + 120f, 0f, MaxHealth);
                GetComponent<Inventory>().items.RemoveAt(0);
            } else if (evt.Equals("startWeaponDamage")) {
                isWeaponDamaging = true;
            } else if (evt.Equals("stopWeaponDamage")) {
                isWeaponDamaging = false;
            } else if (evt.Equals("correctDirection")) {
                if(target != null) {
                    directionCorrectionRotation = Quaternion.LookRotation(target.transform.position - transform.position, -motor.GetGravity());
                }
            }
        }

        if (evt.Equals("step") && motor.Grounded) {
            string[] clips = { SoundClips.STEP_STONE_01, SoundClips.STEP_STONE_02, SoundClips.STEP_STONE_03 };
            PlaySound(clips[Random.Range(0, clips.Length)]);
        }

        if (evt.Equals("swing") && isAttacking) {

            // The animation reached the point in which stamina must be consumed
            ConsumeStamina(30f * lastAttackMove.damageMultiplier);

            string[] clips = { SoundClips.SWING_01, SoundClips.SWING_02, SoundClips.SWING_03, SoundClips.SWING_04 };
            PlaySound(clips[Random.Range(0, clips.Length)]);
        }
    }

    public void OnWeaponHit(Character characterHit) {
        //if (isWeaponDamaging && isAttacking && !alreadyDamaged.Contains(characterHit)) {
        if (characterHit != this && motor.animator.GetFloat("Curve/Damage") > 0.95f && photonView.IsMine) {
            characterHit.photonView.RPC("AttemptDamage", RpcTarget.All, (35f + 10f * stats.strength) * lastAttackMove.damageMultiplier, characterHit.transform.position - transform.position, photonView.ViewID);
        }
    }

    public void Reaccomodate(EquipmentAccomodation accomodation) {
        Transform handleR = transform.FindDeepChild("Handle.R");
        Transform handleL = transform.FindDeepChild("Handle.L");

        switch (accomodation) {
            case EquipmentAccomodation.Kubold:
                if (weaponObject != null) {
                    weaponObject.transform.localRotation = Quaternion.Euler(40f, 0f, 90f);
                }
                if(shieldObject != null) {
                    shieldObject.transform.localRotation = Quaternion.Euler(2.3f, -270f, -90f);
                }
                break;
            case EquipmentAccomodation.Frank:
                if (weaponObject != null) {
                    weaponObject.transform.localRotation = Quaternion.Euler(90f, 0f, 90f);
                }
                if (shieldObject != null) {
                    shieldObject.transform.localRotation = Quaternion.Euler(2.3f, -270f, -90f);
                }
                break;
            case EquipmentAccomodation.Own:
                break;
            default:
                break;
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
            weaponObject.transform.localScale = Vector3.one * transform.localScale.x;
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
            shieldObject.transform.localScale = Vector3.one * transform.localScale.x; ;
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
        /*CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Gizmos.DrawSphere(transform.position + transform.forward * attackRadius * 2f + transform.up * 1.3f, attackRadius);*/
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

    public AttackMove() {
        
    }

    public AttackMove(string stateName, AttackType type, float damageMultiplier) {
        this.stateName = stateName;
        this.type = type;
        this.damageMultiplier = damageMultiplier;
    }

    public AttackMove(string stateName, float damageMultiplier) {
        this.stateName = stateName;
        this.damageMultiplier = damageMultiplier;
    }
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

[System.Serializable]
public class Stats {
    public int vitality = 5;
    public int endurance = 5;
    public int strength = 5;
}