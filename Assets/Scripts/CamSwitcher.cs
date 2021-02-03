using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitcher : MonoBehaviour {
    public static CamSwitcher I { get; private set; }
    public const int MENU = 1, MAP = 2, BATTLE= 3;
    public Camera menu_cam, map_cam, battle_cam;
    public GameObject menu_canvas;
    public GameObject battle_canvas;
    public GameObject battleUI_canvas;
    public GameObject map_canvas;
    public GameObject mapUI_canvas;
    public SoundManager sound_manager;
    public GameObject pause_panel, battle_pause_panel;

    private bool paused = false;
    public int current_cam = MENU;
    public int previous_cam = MAP;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    void Start() {
        sound_manager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        pause_panel.SetActive(false);
        battle_pause_panel.SetActive(false);
        set_active(MENU, true);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            toggle_paused();
        }
        if (Input.GetKeyDown(KeyCode.C)) 
            flip_menu_map();
        
    }

    void cycle() {
        if (current_cam == MAP) {
            set_active(BATTLE, true);
        } else if (current_cam == BATTLE) {
            set_active(MAP, true);
        }
    }

    public void toggle_paused() {
        paused = !paused;
        if (current_cam == MAP) {
            pause_panel.SetActive(paused);
        } else if (current_cam == BATTLE) {
            battle_pause_panel.SetActive(paused);
        }
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

    public void flip_menu_battle() {
        if (current_cam == MENU) {
            set_active(BATTLE, true);
        } else if (current_cam == BATTLE) {
            set_active(MENU, true);
        }
    }

    public void return_previous_cam() {
        set_active(previous_cam, true);
    }

    public void set_active(int screen, bool active) {
        if (screen == MENU) {  
            menu_canvas.SetActive(active);
            menu_cam.enabled = active;
            if (active) {
                set_active(MAP, false);
                set_active(BATTLE, false);
                Controller.I.check_button_states();
            }
        } else if (screen == MAP) {
            map_canvas.SetActive(active);
            map_cam.enabled = active;
            mapUI_canvas.SetActive(active);
            if (active) {
                // Set the camera on the active player.
                Vector3 p = new Vector3(TurnPhaser.I.active_disc.pos.x, TurnPhaser.I.active_disc.pos.y, -14);
                map_cam.transform.SetPositionAndRotation(p, Quaternion.identity);
                set_active(BATTLE, false);
                set_active(MENU, false);
            }
        } else if (screen == BATTLE) {
            battle_canvas.SetActive(active);
            battle_cam.enabled = active;
            battleUI_canvas.SetActive(active);
            if (active) {
                BackgroundLoader.I.load(Map.I.get_current_cell().ID);
                set_active(MAP, false);
                set_active(MENU, false);
            } else {
                UnitPanelManager.I.close();
            }
        }

        if (active) { // If not turning this screen off.
            previous_cam = current_cam;
            current_cam = screen;
            if (previous_cam != current_cam)
                sound_manager.background_SFX_player.activate_screen(screen);
        }
    }
}
