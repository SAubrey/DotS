using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour {

    public CamSwitcher cs;
    private const float ARROW_TRANS = 0.5f;
    private const float MAP_MIN_X = 0;
    private const float MAP_MAX_X = 20f;
    private const float MAP_MIN_Y = 0;
    private const float MAP_MAX_Y = 20f;
    private const float BATTLE_MIN_X = -2000f;
    private const float BATTLE_MAX_X = 2100f;
    private const float BATTLE_MIN_Y = -1600f;
    private const float BATTLE_MAX_Y = 1500f;


    void Start() {
        cs = GameObject.Find("CamSwitcher").GetComponent<CamSwitcher>();
    }

    void Update() {
        if (cs.current_cam == CamSwitcher.MAP) 
            check_input(-0.5f);
        else if (cs.current_cam == CamSwitcher.BATTLE) 
            check_input(-30f);
    }

    private void check_input(float scale) {
        if (Input.GetMouseButton(1)) {
            float h = scale * Input.GetAxis("Mouse X");
            float v = scale * Input.GetAxis("Mouse Y");
            verify_movement(h, v);
        }
        
        if (Input.GetKey(KeyCode.UpArrow))
            verify_movement(0, scale * -ARROW_TRANS);
         else if (Input.GetKey(KeyCode.DownArrow)) 
            verify_movement(0, scale * ARROW_TRANS);
         else if (Input.GetKey(KeyCode.LeftArrow)) 
            verify_movement(scale * ARROW_TRANS, 0);
         else if (Input.GetKey(KeyCode.RightArrow)) 
            verify_movement(scale * -ARROW_TRANS, 0);
    }

    private void verify_movement(float h, float v) {
        float x = transform.localPosition.x;
        float y = transform.localPosition.y;
        if (cs.current_cam == CamSwitcher.MAP) {
            if (x + h > MAP_MAX_X || x + h < MAP_MIN_X)
                h = 0;
            if (y + v > MAP_MAX_Y || y + v < MAP_MIN_Y)
                v = 0;
        }
        else if (cs.current_cam == CamSwitcher.BATTLE) {
            if (x + h > BATTLE_MAX_X || x + h < BATTLE_MIN_X)
                h = 0;
            if (y + v > BATTLE_MAX_Y || y + v < BATTLE_MIN_Y)
                v = 0;
        }
        transform.Translate(h, v, 0);
    }
}
