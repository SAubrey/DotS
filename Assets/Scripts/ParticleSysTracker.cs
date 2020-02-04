using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSysTracker : MonoBehaviour {

    public void init(Vector3 pos) {
        gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z -10);
        Destroy(gameObject, 5f);
        Destroy(this, 7f);
    }
}
