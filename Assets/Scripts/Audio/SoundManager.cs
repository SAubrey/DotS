using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager I { get; private set; }

    public BackgroundSFXPlayer background_SFX_player;
    public ImpactPlayer impact_player;
    
    void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}

