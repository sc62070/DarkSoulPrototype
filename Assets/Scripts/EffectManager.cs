using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    public static EffectManager instance;

    public EffectAssetCollection assets;

    public ParticleSystem[] bloodParticleSystems;

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }

        bloodParticleSystems = new ParticleSystem[4];
        for(int i = 0; i < bloodParticleSystems.Length; i++) {
            bloodParticleSystems[i] = Instantiate(EffectManager.instance.assets.bloodEffect, transform).GetComponent<ParticleSystem>();
            //DontDestroyOnLoad(bloodEffect.gameObject);
            bloodParticleSystems[i].Stop();
        }
        
    }


    public void PlayBlood(Vector3 position, Vector3 direction) {
        foreach(ParticleSystem ps in bloodParticleSystems) {
            if (!ps.isPlaying) {
                ps.transform.position = position;
                ps.transform.forward = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                ps.Play();
                return;
            }
        }

        bloodParticleSystems[0].transform.position = position;
        bloodParticleSystems[0].transform.forward = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        bloodParticleSystems[0].Play();
    }

    void Update() {

    }
}

[System.Serializable]
public class EffectAssetCollection {
    public GameObject bloodEffect;
}
