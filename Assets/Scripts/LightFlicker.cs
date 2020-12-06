using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {
    /*
    Randomly walks light intensity proportionally to radius.
    */
    public UnityEngine.Experimental.Rendering.Universal.Light2D light2d;
    public float minIntensity, maxIntensity;
    public bool active = true;
    void Update() {
        if (!active)
            return;
        flicker();
    }

    private void flicker() {
        float variance = Random.Range(.98f, 1.02f);
        if (within_boundaries(variance * light2d.intensity, minIntensity, maxIntensity)) {
            light2d.intensity *= variance;
            light2d.pointLightOuterRadius *= variance;
        }
    }

    private bool within_boundaries(float value, float min, float max) {
        return value < max && value > min;
    }
}
