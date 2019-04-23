using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviourPun {

    public static SoundManager instance;

    public AudioClip damage;

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    
}

[System.Serializable]
public class SoundClips {

    public static string prefix = "Sounds/";

    public static string STEP_STONE_01 = prefix + "Stepping/step_stone_01";
    public static string STEP_STONE_02 = prefix + "Stepping/step_stone_02";
    public static string STEP_STONE_03 = prefix + "Stepping/step_stone_03";

    public static string DAMAGE_01 = prefix + "damage_01";
    public static string DAMAGE_02 = prefix + "damage_02";
    public static string DAMAGE_03 = prefix + "damage_03";

    public static string SWING_01 = prefix + "swing_01";
    public static string SWING_02 = prefix + "swing_02";
    public static string SWING_03 = prefix + "swing_03";
    public static string SWING_04 = prefix + "swing_04";

    public static string LAND = prefix + "land";

    public static string DRINK_ESTUS = prefix + "drink_estus";

    public static AudioClip Get(string name) {
        return Resources.Load<AudioClip>(name);
    }
}
