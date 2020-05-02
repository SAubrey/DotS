using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactPlayer : AudioPlayer {
    
    public AudioClip impact_medium_dry;
    public AudioClip impact_medium_dry1;
    public AudioClip impact_fat_wet;
    private AudioClip[] impact_mediums;
    public AudioClip blocked;
    public AudioClip dead_sound;

    protected override void Start() {
        base.Start();
        impact_mediums = new AudioClip[2] {impact_medium_dry, impact_medium_dry1};
    }
     public void play_hit_from_damage(int dmg, bool dead) {
        
        if (dmg <= 0) {
            play(blocked);
        } else if (dmg <= 2) {
            play(choose_random_clip(impact_mediums));
        } else {
            play(impact_fat_wet);
        } 
        
        if (dead) {
            //play(dead_sound);
        }
    }
}
