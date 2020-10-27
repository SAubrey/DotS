using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MapCellUI : MonoBehaviour {
    public TextMeshProUGUI cell_typeT, enemy_countT, star_crystalsT, battleT;
    public Button scoutB, teleportB, moveB, unlockB, battleB, mineB;
    private Map map;
    private MapCell cell;
    public GameObject parent;

    public void init(Map map, MapCell cell) {
        this.map = map;
        this.cell = cell;
        cell_typeT.text = build_titleT();
        enemy_countT.text = build_enemy_countT(cell.get_enemies().Count, cell.discovered);
        star_crystalsT.text = cell.discovered ? cell.star_crystals.ToString() : "?";
        update_star_crystal_text();
        Vector3 pos = new Vector3(cell.pos.x, cell.pos.y, 0);
        //transform.position =
            //map.c.cam_switcher.mapCam.WorldToScreenPoint(new Vector3(pos.x + 0.5f, pos.y - 2.5f, 0));
            transform.position = new Vector3(pos.x + 0.5f, pos.y - 1.5f, 0); // camera mode, not overlay

        enable_button(moveB, map.can_move(pos));
        enable_button(scoutB, map.can_scout(pos));
        enable_button(teleportB, map.can_teleport(pos));
        enable_button(unlockB, can_unlock());
        enable_button(battleB, cell.can_setup_group_battle());
        enable_button(mineB, cell.can_mine(map.c.get_disc().bat));
        if (cell.can_setup_group_battle()) {
            battleT.text = build_group_battleB_T();
        }
    }

    public void update_star_crystal_text() {
        star_crystalsT.text = cell.discovered ? cell.star_crystals.ToString() : "?";
    }

    private string build_titleT() {
        string text = "";
        if (cell.has_rune_gate) {
            text = cell.restored_rune_gate ? "Active Rune Gate" : "Inactive Rune Gate";
        } else {
            text = cell.discovered ? cell.name : "Unknown";
        }
        return text;
    }

    private string build_enemy_countT(int enemy_count, bool discovered) {
        if (enemy_count > 0 && discovered) {
            return enemy_count + " enemies have been revealed lurking in the darkness.";
        } else if (discovered) {
            return "This land is free from darkness.";
        } else {
            return "We know not what waits in the darkness.";
        }
    }

    public string build_group_battleB_T() {
        string text = "";
        if (cell.battle == null) 
            return "Form Group Battle";
        else if (cell.battle.leader_is_active_on_map) {
            if (cell.battle.group_pending) {
                text = cell.battle.can_begin_group ? "Begin Group Battle" : "Disband Group Battle";
            }
        } else {
            if (cell.battle.active) {
                text = "Reinforce";
            } else if (cell.battle.group_pending) {
                // be able to leave in same turn
                if (cell.battle.includes_disc(map.c.get_disc()))
                    text = "Leave Group Battle";
                else
                    text = "Join Group Battle";
            } //else {
                //text = "Form Group Battle";
            //}
        }
        return text;
    }

    /*
    - Pending groups must be able to be disbanded if the leader moves/clicks disband.
        Each battalion must store a reference to the map cell that it's grouping.
        When move, null that group.
    */
    public void group_battle() {
        if (cell.battle == null) {
            // Form group
            cell.assign_group_leader();
            

        } else if (cell.battle.leader_is_active_on_map) {
            // If it's the leader's turn then there is a pending group battle.
            if (cell.battle.can_begin_group) {
                // enter battle
                cell.battle.begin();
            } else {
                cell.clear_battle();
            }
        } else {
            if (cell.battle.active) {
                // Reinforce - discipline's troops become available for placement
                // in the outskirts of the battlefield once the turn reaches the 
                // leader again.
                cell.battle.add_participant(map.c.get_disc());
            }
            else if (cell.battle.group_pending) {
                if (cell.battle.includes_disc(map.c.get_disc()))
                    cell.battle.remove_participant(map.c.get_disc());
                else
                    cell.battle.add_participant(map.c.get_disc());
            }
        }
        battleT.text = build_group_battleB_T();
    }

    public void move() {
        map.move_player(new Vector3(cell.pos.x, cell.pos.y, 0), true);
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

    public void mine() {
        map.c.get_disc().mine(cell);
    }

    // To determine if the unlock button can be pressed, including that the 
    // requirements can be met if it is an unlockable cell.
    private bool can_unlock() {
        if (cell.has_rune_gate && map.c.get_disc().get_var(Storeable.STAR_CRYSTALS) >= 10) {
            return true;
        }
        if (cell.requires_unlock) {
            if (cell.get_unlockable().requires_seeker && map.c.get_disc().bat.has_seeker) {
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
