using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitcher : MonoBehaviour {
    public const int MENU = 1;
    public const int MAP = 2;
    public const int BATTLE = 3;

    public Camera menuCam;
    public GameObject menu_canvas;
    public Camera battleCam;
    public GameObject battle_canvas;
    public GameObject battleUI_canvas;
    public Camera mapCam;
    public GameObject map_canvas;
    public GameObject mapUI_canvas;

    public GameObject pause_panel;
    
    private Controller c;
    private BatLoader bat_loader;

    private bool paused = false;
    public int current_cam = MAP;
    
    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        bat_loader = c.bat_loader;
        pause_panel.SetActive(false);
        set_active(MENU, true);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            cycle();
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            if (current_cam == MAP) {
                toggle_paused();
            }
        }
    }

    void cycle() {
        if (current_cam == MAP) {
            set_active(BATTLE, true);
            bat_loader.load_text(c.get_active_bat());
        } else if (current_cam == BATTLE) {
            set_active(MAP, true);
        }
    }

    public void toggle_paused() {
        paused = !paused;
        pause_panel.SetActive(paused);
    }

    // Called by buttons
    public void flip_menu_map() {
        if (current_cam == MENU) {
            set_active(MAP, true);
        } else if (current_cam == MAP) {
            set_active(MENU, true);
        }
    }

    public void flip_map_battle() {
        if (current_cam == MAP) {
            set_active(BATTLE, true);
        } else if (current_cam == BATTLE) {
            set_active(MAP, true);
        }
    }

    public void set_active(int screen, bool active) {
        if (screen == MENU) {  
            menu_canvas.SetActive(active);
            menuCam.enabled = active;
            if (active) {
                set_active(MAP, false);
                set_active(BATTLE, false);
                c.check_button_states();
            }
        } else if (screen == MAP) {
            map_canvas.SetActive(active);
            mapCam.enabled = active;
            mapUI_canvas.SetActive(active);
            if (active) {
                bat_loader.load_text(c.get_active_bat());
                // Set the camera in the middle of the map.
                mapCam.transform.SetPositionAndRotation(new Vector3(10, 10, -14), Quaternion.identity);
                set_active(BATTLE, false);
                set_active(MENU, false);
            }
        } else if (screen == BATTLE) {
            battle_canvas.SetActive(active);
            battleCam.enabled = active;
            battleUI_canvas.SetActive(active);
            if (active) {
                set_active(MAP, false);
                set_active(MENU, false);
                bat_loader.load_text(c.get_active_bat());
            }
        }

        if (active) 
            current_cam = screen;
    }
}
