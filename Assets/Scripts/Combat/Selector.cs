using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// For selecting unit slots on the field for unit placement or selection.
public class Selector : MonoBehaviour {
    private Controller c;
    private Formation f;
    private CamSwitcher cs;
    private BatLoader bat_loader;
    private BattlePhaser bp;
    private AttackQueuer aq;
    private LineDrawer line_drawer;
    private UnitPanelManager unit_panel_man;

    public bool selecting_target = false;
    private bool _selecting_move = false;
    public bool selecting_move {
        get { return _selecting_move; }
        set { 
            _selecting_move = value;
            if (!value)
                c.unit_panel_man.player_panel.depress_moveB();
            else // do you want this?
                c.unit_panel_man.player_panel.press_moveB();
        }
    }
    public Slot selected_slot;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        f = c.formation;
        cs = c.cam_switcher;
        bat_loader = c.bat_loader;
        bp = c.battle_phaser;
        aq = c.attack_queuer;
        line_drawer = c.line_drawer;
        unit_panel_man = c.unit_panel_man;
    }

    // Called only by slot buttons. 
    // A slot will either attempt to be filled if it is empty and a unit type is selected,
    // otherwise it will select if not selected, or deselect if selected.
    public void handle_slot(Slot slot) {
        if (cs.current_cam != CamSwitcher.BATTLE)
            return;
            
        // Attempting to attack or move. Do not select a new slot upon failure.
        bool selection_taking_action = selected_slot != null && (selecting_target || selecting_move);
        if (selection_taking_action) {
            attempt_action(slot);
            return;
        }

        bool punit_selected_for_placement = bp.placement_stage && slot.is_empty;
        if (punit_selected_for_placement) {
            place_punit(slot);
        } else if (!slot.is_empty) { // Select unit already present in slot
            set_selected(slot);
        }
    }

    private void place_punit(Slot slot) {
        // The highest order slot must be filled first.
        Slot highest_slot = slot.get_group().get_highest_empty_slot();
        if (highest_slot.fill(c.get_active_bat().get_selected_unit())) {
            bp.can_skip = true;
            bat_loader.load_text(c.get_active_bat()); // update inventory text
            deselect();
        }
    }

    private void attempt_action(Slot slot) {
        // ---Attack---
        if (selecting_target) { 
            selecting_target = false;
            Slot highest_enemy_slot = slot.get_group().get_highest_enemy_slot();
            if (highest_enemy_slot != null) {
                bool successful_att = aq.attempt_attack(selected_slot, highest_enemy_slot);
                if (successful_att) return;
            }
            c.unit_panel_man.player_panel.depress_attackB();

        // ---Move---
        } else if (selecting_move) { 
            selecting_move = false;

            // Units are either moved up automatically or can be swapped. 
            if (slot.is_empty) {
                Slot highest_es = slot.get_group().get_highest_empty_slot();
                slot = highest_es;
                // Prevent pointless movement within same group.
                if (selected_slot.get_group() == highest_es.get_group()) {
                    Debug.Log("same group, no move.");
                    return;
                }
                //else if (selected_slot.get_punit().attempt_move(highest_es))
                    //deselect();
            }
            if (selected_slot.get_punit().attempt_move(slot)) {
                deselect();
            }
        }
    }

    // The passed slot is the most recently clicked slot.
    // Selected slot is the previously clicked slot. 
    private void set_selected(Slot slot) {
        if (selected_slot == null) {
            select(slot);
        } else if (selected_slot == slot || slot == null) {  // Clear selection if pressed twice
            deselect();
        } else {  // Clear previous selection
            deselect();
            select(slot);
        }
    }

    private void select(Slot slot) {
        selected_slot = slot;
        selected_slot.show_selection();
        unit_panel_man.show(slot);
        f.clear_placement_selection();
    }

    public void deselect() {
        if (selected_slot != null) {
            selected_slot.show_no_selection();
            unit_panel_man.close(Unit.PLAYER);
            selected_slot = null;
            selecting_target = false;
            selecting_move = false;
        }
    }

    // Called from player unit panel return button
    public void return_unit() {
        if (selected_slot.get_punit() == null)
            return;
        Slot s = selected_slot;
        deselect();
        s.empty();
        bat_loader.load_text(c.get_active_bat());
    }
}
