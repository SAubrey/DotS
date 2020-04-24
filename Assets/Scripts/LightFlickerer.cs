using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerer : MonoBehaviour {
    /*
    Randomly walks light intensity proportionally to radius.
    */
    public UnityEngine.Experimental.Rendering.LWRP.Light2D light2d;
    private float minIntensity = 1.6f;
    private float maxIntensity = 1.8f;
    //private float intensity

    void Update() {
        Flicker();
    }

    private void Flicker() {
        float variance = Random.Range(.98f, 1.02f);
        if (WithinBoundaries(variance * light2d.intensity, minIntensity, maxIntensity)) {
            light2d.intensity *= variance;
            light2d.pointLightOuterRadius *= variance;
        }
    }

    private bool WithinBoundaries(float value, float min, float max) {
        return value < max && value > min;
    }
}
