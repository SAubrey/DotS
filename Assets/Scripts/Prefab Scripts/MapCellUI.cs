using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapCellUI : MonoBehaviour {
    public Text cell_typeT, enemy_countT, star_crystalsT;
    public Button scoutB, teleportB, moveB, unlockB;
    private Map map;
    private MapCell cell;
    public GameObject parent;

    public void init(Map map, MapCell cell) {
        this.map = map;
        this.cell = cell;
        cell_typeT.text = build_titleT();
        enemy_countT.text = build_enemy_countT(cell.get_enemies().Count, cell.discovered);
        star_crystalsT.text = map.c.get_disc().get_var(Storeable.STAR_CRYSTALS).ToString();

        Vector3 pos = new Vector3(cell.pos.x, cell.pos.y, 0);
        //transform.position =
            //map.c.cam_switcher.mapCam.WorldToScreenPoint(new Vector3(pos.x + 0.5f, pos.y - 2.5f, 0));
            transform.position = new Vector3(pos.x + 0.5f, pos.y - 2.5f, 0); // camera mode, not overlay

        enable_button(moveB, map.can_move(pos));
        enable_button(scoutB, map.can_scout(pos));
        enable_button(teleportB, map.can_teleport(pos));
        enable_button(unlockB, can_unlock());
    }

    private string build_titleT() {
        string text = "";
        if (cell.has_rune_gate) {
            text += cell.restored_rune_gate ? "Active Rune Gate" : "Inactive Rune Gate";
        } else {
            text += cell.discovered ? cell.name : "Unknown";
        }
        return text;
    }

    private string build_enemy_countT(int enemy_count, bool discovered) {
        if (enemy_count > 0 && discovered) {
            return enemy_count + " enemies have been revealed sulking in the darkness.";
        } else if (discovered) {
            return "This land is free from darkness.";
        } else {
            return "We know not what waits in the darkness.";
        }
    }

    public void move() {
        float randx = Random.Range(0.4f, 0.6f); // simulate human placement, prevent perfect overlap
        float randy = Random.Range(0.4f, 0.6f);
        map.move_player(new Vector3(cell.pos.x + randx, cell.pos.y + randy, 0), true);
        close();
    }

    public void close() {
        map.open_cell_UI_script = null;
        Destroy(gameObject);
    }

    public void scout() {
        map.scout(new Vector3(cell.pos.x, cell.pos.y, 0));
        enable_button(scoutB, false);
        close();
    }

    public void teleport() {
        map.move_player(new Vector3(cell.pos.x, cell.pos.y, 0), true);
        close();
    }

    // To determine if the unlock button can be pressed, including that the 
    // requirements can be met if it is an unlockable cell.
    private bool can_unlock() {
        if (cell.has_rune_gate && map.c.get_disc().get_var(Storeable.STAR_CRYSTALS) >= 10) {
            return true;
        }
        if (cell.requires_unlock) {
            if (cell.get_unlockable().requires_seeker && map.c.get_active_bat().has_seeker) {
                return true;
            }
            // Must be a resource requirement.
            else if (map.c.get_disc().get_var(cell.get_unlockable().resource_type) >= 
                cell.get_unlockable().resource_cost) {
                    return true;
            }
        }   
        return false;         
    }

    public void unlock() {
        if (cell.has_rune_gate) {
            map.c.get_disc().change_var(Storeable.STAR_CRYSTALS, -10, true);
            cell.restored_rune_gate = true;
        } else if (cell.has_travelcard) {
            if (cell.get_unlockable().requires_seeker) {
                map.c.get_disc().adjust_resources_visibly(cell.get_travelcard_consequence());
                map.c.get_disc().complete_travelcard();
            }
            else {
                map.c.get_disc().change_var(cell.get_unlock_type(), cell.get_unlock_cost(), true);
            }
        }
    }
    private void enable_button(Button b, bool state) {
        b.interactable = state;
    }
}
