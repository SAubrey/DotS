using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour {

    //private bool rmbHeld = false;
    public CamSwitcher cs;

    void Start() {
        cs = GameObject.Find("CamSwitcher").GetComponent<CamSwitcher>();
    }

    void Update() {
        if (cs.current_cam == CamSwitcher.MAP  || cs.current_cam == CamSwitcher.BATTLE) {
            if (Input.GetMouseButton(1)) {
                float h = -1 * Input.GetAxis("Mouse X") / 2;
                float v = -1 * Input.GetAxis("Mouse Y") / 2;
                transform.Translate(h, v, 0);
            }
        
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                transform.Translate(0, 1, 0);
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                transform.Translate(0, -1, 0);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                transform.Translate(-1, 0, 0);
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                transform.Translate(1, 0, 0);
            }
        }
    }
}
