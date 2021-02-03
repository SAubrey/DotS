using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSFXPlayer : AudioPlayer {
    public AudioSource delay_source;
    public AudioSource loop_source;
    public AudioClip menu;
    public AudioClip map_wind;
    public AudioClip map_rumble1;
    public AudioClip map_rumble2;
    public AudioClip battle_wind;

    private bool delaying = false;
    private float delay_counter = 0;
    private float delay = 0;
    private float delay_counter_min = 30f;
    private float delay_counter_max = 120f;


    protected override void Start() {
        //base.Start();
        delay_source = gameObject.AddComponent<AudioSource>();
        delay_source.loop = false;
        loop_source = gameObject.AddComponent<AudioSource>();
        loop_source.loop = true;
    }

    void Update() {
        if (!delaying)
            return;
        
        delay_counter += Time.deltaTime;
        if (delay_counter >= delay) {
            delay_source.clip = get_random_map_sfx();
            delay_source.Play();
            delay = get_random_delay(delay_counter_min, delay_counter_max);
            delay_counter = 0;
        }
    }

    public void activate_screen(int screen) {
        if (screen == CamSwitcher.MENU) {
            play(loop_source, menu);
            //source.clip = menu;
            delaying = false;
        } else if (screen == CamSwitcher.MAP) {
            play(loop_source, map_wind);
            delaying = true;
        } else if (screen == CamSwitcher.BATTLE) {
            play(loop_source, battle_wind);
            delaying = false;
        }
    }
    
    public AudioClip get_random_map_sfx() {
        return Random.Range(0, 2) == 0 ? map_rumble1 : map_rumble2;
    }
}
