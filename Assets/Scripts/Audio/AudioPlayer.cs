using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioPlayer : MonoBehaviour {
    public List<AudioSource> sources = new List<AudioSource>();
    public AudioSource source, source1, source2; 
    //protected fading

    protected virtual void Start() {
        sources.Add(source);
        sources.Add(source1);
        sources.Add(source2);
    }

    void Update() {

    }

    public AudioClip choose_random_clip(AudioClip[] clips) {
        int clip_index = Random.Range(0, clips.Length);
        return clips[clip_index];
    }

    public void play(AudioClip clip) {
        foreach (AudioSource source in sources) {
            if (!source.isPlaying) {
                source.clip = clip;
                source.Play();
                return;
            }
        }
    }

    public float get_random_delay(float min, float max) {
        return Random.Range(min, max);
    }

    public float get_random_pitch() {
        return Random.Range(.95f, 1.05f);
    }


}
