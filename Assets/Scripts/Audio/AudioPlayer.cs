using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioPlayer : MonoBehaviour {
    public List<AudioSource> sources = new List<AudioSource>();

    protected virtual void Start() {
    }

    // Play from non-looping, full volume, dynamically sized list of sources.
    public virtual void play(AudioClip clip, float volume=1f) {
        foreach (AudioSource source in sources) {
            if (!source.isPlaying) {
                play(source, clip, volume);
                return;
            }
        }
        // Add free source dynamically
        AudioSource new_source = gameObject.AddComponent<AudioSource>();
        sources.Add(new_source);
        play(new_source, clip, volume);
    }

    public void play(AudioSource source, AudioClip clip, float volume=1f) {
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    public AudioClip choose_random_clip(AudioClip[] clips) {
        int clip_index = Random.Range(0, clips.Length);
        return clips[clip_index];
    }

    public float get_random_delay(float min, float max) {
        return Random.Range(min, max);
    }

    public float get_random_pitch() {
        return Random.Range(.95f, 1.05f);
    }
}