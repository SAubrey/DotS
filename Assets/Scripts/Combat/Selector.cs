using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// For selecting unit slots on the field for unit placement or selection.
public class Selector : MonoBehaviour {
    private Controller c;
    private CamSwitcher cs;
    private BatLoader bat_loader;
    private BattlePhaser bp;
    private UnitPanelManager unit_panel_man;
    private PlayerPanel player_panel;
    private bool _selecting_target;
    public bool selecting_target {
        get { return _selecting_target; }
        set {
            _selecting_target = value;
            if (_selecting_target) {
                c.line_drawer.preview_line.draw(selected_slot.transform.position);
            } else {
                c.line_drawer.preview_line.erase();
            }
        }
    }
    public bool selecting_move = false;
    public Slot selected_slot;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        cs = c.cam_switcher;
        bat_loader = c.bat_loader;
        bp = c.battle_phaser;
        unit_panel_man = c.unit_panel_man;
        player_panel = unit_panel_man.player_panel;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (bp.adv_stageB.interactable)
                bp.advance_stage();
        }
        if (selected_slot == null)
            return;
            
        if (!Input.anyKeyDown)
            return;

        if (Input.GetKeyDown(KeyCode.X)) {
            unit_panel_man.close();
        }
    }

    // Called only by slot buttons. 
    // A slot will either attempt to be filled if it is empty and a unit type is selected,
    // otherwise it will select if not selected, or deselect if selected.
    public void handle_slot(Slot slot) {
        if (cs.current_cam != CamSwitcher.BATTLE)
            return;
        
        // Attempting to attack or move. Do not select a new slot upon failure.
        bool selection_taking_action = selected_slot != null && 
                (selecting_target || selecting_move);
        if (selection_taking_action) {
            attempt_action(slot);
            return;
        }

        if (verify_placement(slot)) {
            place_punit(slot);
        } else if (!slot.is_empty) { // Select unit already present in slot
            set_selected(slot);
        }
    }

    private void place_punit(Slot slot) {
        // The highest order slot must be filled first.
        Slot highest_slot = slot.get_group().get_highest_empty_slot();
        if (highest_slot.fill(c.get_active_bat().get_placeable_unit(
                bat_loader.get_selected_unit_ID()))) {
            if (bp.init_placement_stage)
                bp.check_all_units_placed();

            if (bat_loader.selecting_for_heal)
                bat_loader.complete_heal();
            // Update inventory text
            bat_loader.load_text(c.get_active_bat(), 
                    bat_loader.get_selected_unit_ID()); 
            deselect();
        }
    }

    private void attempt_action(Slot slot) {
        if (selecting_target) { // ---Attack---
            selecting_target = false;
            c.unit_panel_man.player_panel.attB_pressed = false;

            // Range units can attack units not in the front slot.
            Slot highest_enemy_slot;
            if (selected_slot.get_punit().is_range)
                highest_enemy_slot = slot;
            else
                highest_enemy_slot = slot.get_group().get_highest_enemy_slot();

            if (highest_enemy_slot != null) {
                bool successful_att = 
                    selected_slot.get_unit().attempt_set_up_attack(highest_enemy_slot);
            }
        } else if (selecting_move) { // ---Move---
            selecting_move = false;
            c.unit_panel_man.player_panel.moveB_pressed = false;

            // Units are either moved up automatically or can be swapped. 
            if (slot.is_empty) {
                Slot highest_es = slot.get_group().get_highest_empty_slot();
                slot = highest_es;
                // Prevent pointless movement within same group.
                if (selected_slot.get_group() == highest_es.get_group()) {
                    Debug.Log("same group, no move.");
                    return;
                }
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
        // Clear selection if pressed twice
        } else if (selected_slot == slot || slot == null) { 
            deselect();
        } else {  // Clear previous selection
            deselect();
            select(slot);
        }
    }

    private void select(Slot slot) {
        selected_slot = slot;
        selected_slot.show_selection(true);
        unit_panel_man.show(slot);
        c.bat_loader.clear_placement_selection();
    }

    public void deselect() {
        if (selected_slot == null)
            return;
        selected_slot.show_selection(false);
        unit_panel_man.close();
        selected_slot = null;
        selecting_target = false;
        selecting_move = false;
    }

    private bool verify_placement(Slot dest) {
        bool valid_init_place = 
            c.battle_phaser.init_placement_stage && 
             dest.is_type(Group.PLAYER);
        bool valid_post_init_place = 
            (dest.is_type(Group.PERIPHERY)) &&
            !c.battle_phaser.init_placement_stage;

        if ((c.bat_loader.selecting_for_heal || bp.placement_stage) && 
            dest.is_empty &&
            (valid_init_place || valid_post_init_place))
            return true;
        return false;
    }

    // Called from player unit panel return button
    public void return_unit() {
        if (!selected_slot.has_punit)
            return;
        Slot s = selected_slot;
        int removed_unit_ID = s.get_unit().get_ID();
        deselect();
        s.empty();
        bat_loader.load_text(c.get_active_bat(), removed_unit_ID);
        bp.check_all_units_placed(); // Can we move past placement?
    }
}
