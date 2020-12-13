using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MapCellUI : MonoBehaviour {
    public TextMeshProUGUI cell_typeT, enemy_countT, mineableT, battleT;
    public Button scoutB, teleportB, moveB, unlockB, battleB, mineB, show_travelcardB;
    public MapCell cell { get; private set; }

    public void init(MapCell cell) {
        this.cell = cell;
        cell_typeT.text = build_titleT();
        enemy_countT.text = build_enemy_countT(cell.get_enemies().Count, cell.discovered);
        update_star_crystal_text();
        Vector3 pos = new Vector3(cell.pos.x, cell.pos.y, 0);
        //transform.position =
            //map.c.cam_switcher.mapCam.WorldToScreenPoint(new Vector3(pos.x + 0.5f, pos.y - 2.5f, 0));
        transform.position = new Vector3(pos.x + 0.5f, pos.y - 1.5f, 0); // camera mode, not overlay
        gameObject.transform.SetAsFirstSibling();

        enable_button(moveB, Map.I.can_move(pos));
        enable_button(scoutB, Map.I.can_scout(pos));
        enable_button(teleportB, Map.I.can_teleport(pos));
        enable_button(unlockB, can_unlock());
        enable_button(battleB, cell.can_setup_group_battle());
        enable_button(mineB, cell.can_mine(Controller.I.get_disc().bat));
        enable_button(show_travelcardB, can_show_travelcard());
        if (cell.can_setup_group_battle()) {
            battleT.text = build_group_battleB_T();
        }
    }

    public void update_star_crystal_text() {
        if (!cell.discovered) {
            mineableT.text = "?";
            return;
        }
        if (cell.star_crystals > 0)
            mineableT.text = cell.star_crystals.ToString() + " Star Crystals";
        else if (cell.minerals > 0)
            mineableT.text = cell.minerals.ToString() + " Minerals";
        else
            mineableT.text = "No mineable resources.";
    }

    private string build_titleT() {
        if (cell.has_rune_gate) {
            return cell.restored_rune_gate ? "Active Rune Gate" : "Inactive Rune Gate";
        } else {
            return cell.discovered ? cell.name : "Unknown";
        }
    }

    private string build_enemy_countT(int enemy_count, bool discovered) {
        if (!discovered)
            return "We know not what waits in the darkness.";
        if (enemy_count > 0) {
            return enemy_count + " enemies are lurking in the darkness.";
        } 
        return "This land is free from darkness.";
    }

    public string build_group_battleB_T() {
        bool adjacent = Map.check_adjacent(Controller.I.get_disc().pos, cell.pos.to_vec3);
        if (!adjacent) {
            return "Group Battle";
        }

        if (!cell.has_battle) 
            return "Form Group Battle";
        if (cell.battle.leader_is_active_on_map) {
            if (cell.battle.group_pending) {
                return cell.battle.can_begin_group ? "Begin Group Battle" : "Disband Group Battle";
            }
        } else {
            if (cell.battle.active) {
                return "Reinforce";
            } else if (cell.battle.group_pending) {
                // Be able to leave in same turn
                if (cell.battle.includes_disc(Controller.I.get_disc()))
                    return "Leave Group Battle";
                else
                    return "Join Group Battle";
            }
        }
        return "";
    }

    /*
    - Pending groups must be able to be disbanded if the leader moves/clicks disband.
        Each battalion must store a reference to the map cell that it's grouping.
        When move, null that group.
    */
    public void group_battle() {
        if (!cell.has_battle) {
            // Form group
            cell.assign_group_leader();
        } else if (cell.battle.leader_is_active_on_map) {
            // If it's the leader's turn then there is a pending group battle.
            if (cell.battle.can_begin_group) {
                cell.battle.begin();
            } else {
                cell.battle.end();
            }
        } else {
            if (cell.battle.active) {
                // Reinforce - discipline's troops become available for placement
                // in the outskirts of the battlefield once the turn reaches the 
                // leader again.
                cell.battle.add_participant(Controller.I.get_disc());
            }
            else if (cell.battle.group_pending) {
                if (cell.battle.includes_disc(Controller.I.get_disc()))
                    cell.battle.remove_participant(Controller.I.get_disc());
                else
                    cell.battle.add_participant(Controller.I.get_disc());
            }
        }
        battleT.text = build_group_battleB_T();
    }

    public void move() {
        Controller.I.get_disc().move(cell);
        close();
    }

    public void close() {
        MapUI.I.open_cell_UI_script = null;
        Destroy(gameObject);
    }

    public void scout() {
        Map.I.scout(new Vector3(cell.pos.x, cell.pos.y, 0));
        enable_button(scoutB, false);
        close();
    }

    public void teleport() {
        Controller.I.get_disc().move(cell);
        close();
    }

    public void mine() {
        Controller.I.get_disc().mine(cell);
    }

    public bool can_show_travelcard() {
        return cell.creates_travelcard && cell.discovered;
    }

    public void show_travelcard() {
        Debug.Log("Travelcard: " + cell.travelcard);
        MapUI.I.toggle_travelcard(cell.travelcard);
    }

    // To determine if the unlock button can be pressed, including that the 
    // requirements can be met if it is an unlockable cell.
    private bool can_unlock() {
        if (!cell.locked)
            return false;
        if (cell.has_rune_gate && !cell.restored_rune_gate && 
            Controller.I.get_disc().get_var(Storeable.STAR_CRYSTALS) >= 10) {
            return true;
        }

        TravelCardUnlockable u = cell.get_unlockable();
        if (u.requires_seeker && Controller.I.get_disc().bat.has_seeker) {
            return true;
        }
        // Must be a resource requirement.
        if (Controller.I.get_disc().get_var(u.resource_type) >= 
            u.resource_cost) {
            return true;
        }
        return false;         
    }

    public void unlock() {
        if (cell.has_rune_gate) {
            Controller.I.get_disc().change_var(Storeable.STAR_CRYSTALS, -10, true);
            cell.restored_rune_gate = true;
        } else if (cell.has_travelcard) {
            if (cell.get_unlockable().requires_seeker) {
                Controller.I.get_disc().adjust_resources_visibly(cell.get_travelcard_consequence());
            }
            else {
                Controller.I.get_disc().change_var(cell.get_unlock_type(), cell.get_unlock_cost(), true);
            }
            cell.complete_travelcard();
        }
        cell.locked = false;
    }

    private void enable_button(Button b, bool state) {
        b.interactable = state;
    }
}
