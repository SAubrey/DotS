using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGlow : MonoBehaviour {
    public UnityEngine.Experimental.Rendering.Universal.Light2D light2d;
    public float min_intensity, max_intensity;
    private float rand_phase_offset;
    public bool active = true;
    void Start() {
        rand_phase_offset = Random.Range(0f, max_intensity * 3);
    }

    void Update() {
        if (!active)
            return;

        // Sine wave between min and max
        float vert_shift = (min_intensity + max_intensity) / 2f;
        float amp = Mathf.Abs((max_intensity - min_intensity) / 2f);
        float i = (amp * Mathf.Sin(rand_phase_offset + Time.time)) + vert_shift;
        light2d.intensity = i;
    }
}
