using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip damage;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void PlaySound(Vector3 pos) {
        GameObject go = new GameObject();
        go.transform.position = pos;

        AudioSource source = go.AddComponent<AudioSource>();

        source.clip = damage;

        source.Play();
    }
}
