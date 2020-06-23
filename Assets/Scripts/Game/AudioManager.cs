using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    [Serializable]
    public struct AudioData {
        public string name;
        public AudioSource audioSource;
    }
    public AudioData[] audios;
    AudioSource audioData;

    private void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy (gameObject);
    }
    public void PlaySound (string audioName) {

        for (int i = 0; i < audios.Length; i++) {
            if (audios[i].name == audioName) {
                audioData = audios[i].audioSource;
                audioData.Play ();
            }
        }

    }
}