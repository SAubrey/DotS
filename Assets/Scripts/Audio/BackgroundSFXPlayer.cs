using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSFXPlayer : AudioPlayer {
    public AudioSource delay_source;
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

    
    public void set_random_map_sfx() {
        delay_source.clip = Random.Range(0, 2) == 0 ? map_rumble1 : map_rumble2;
    }
    protected override void Start() {
        //base.Start();
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
}
