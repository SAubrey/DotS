using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayer : AudioPlayer {
    public static UIPlayer I { get; private set; }
    public const int CLICK = 0;
    public const int INV_IN = 10, INV_OUT = 11;
    public Dictionary<int, AudioClip> clips = new Dictionary<int, AudioClip>();
    public AudioClip click;
    public AudioClip inv_in, inv_out;

    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    protected override void Start() {
        clips.Add(CLICK, click);
        clips.Add(INV_IN, inv_in);
        clips.Add(INV_OUT, inv_out);
    }

    public void play(int type) {
        base.play(clips[type], 0.5f);
    }
}
