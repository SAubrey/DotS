using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioSource source;
    public AudioSource delay_source;
    public AudioClip menu;
    public AudioClip map_wind;
    public AudioClip map_rumble1;
    public AudioClip map_rumble2;
    public AudioClip battle_wind;

    private float sample_rate = 44100;

    private bool delaying = false;
    private float delay_counter = 0;
    private float delay = 0;
    private float delay_counter_min = 30f;
    private float delay_counter_max = 120f;

    void Start() {
        delay_source.loop = false;
    }

    void Update() {
        if (!delaying)
            return;
        
        delay_counter += Time.deltaTime;
        if (delay_counter >= delay) {
            set_random_map_sfx();
            delay_source.Play();
            delay = get_random_delay(delay_counter_min, delay_counter_max);
            Debug.Log(delay);
            delay_counter = 0;
        }
    }

    public void activate_screen(int screen) {
        if (screen == CamSwitcher.MENU) {  
            source.clip = menu;
            delaying = false;
        } else if (screen == CamSwitcher.MAP) {
            source.clip = map_wind;
            delaying = true;
        } else if (screen == CamSwitcher.BATTLE) {
            delaying = false;
            source.clip = battle_wind;
        }
        source.Play();
    }

    public float get_random_delay(float min, float max) {
        return Random.Range(min, max);
    }

    public void set_random_map_sfx() {
        delay_source.clip = Random.Range(0, 2) == 0 ? map_rumble1 : map_rumble2;
    }
}
