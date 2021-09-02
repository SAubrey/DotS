using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// For selecting unit slots on the field for unit placement or selection.
public class Selector : MonoBehaviour {
    public static Selector I { get; private set; }
    private PlayerPanel player_panel;
    public Slot hovered_slot; // Only assigned when selecting a target.
    private bool _selecting_target;
    public bool selecting_target {
        get { return _selecting_target; }
        set {
            _selecting_target = value;
            if (_selecting_target) {
                LineDrawer.I.preview_line.draw(selected_slot.transform.position);
            } else {
                LineDrawer.I.preview_line.erase();
            }
        }
    }
    public bool selecting_move = false;
    private Slot _selected_slot;
    public Slot selected_slot { 
        get { return _selected_slot; } 
        private set {
            last_selected_slot = _selected_slot;
            _selected_slot = value;
        } 
    }
    public Slot last_selected_slot;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        player_panel = UnitPanelManager.I.player_panel;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (BattlePhaser.I.adv_stageB.interactable)
                BattlePhaser.I.advance();
        }
        if (selected_slot == null)
            return;
            
        if (!Input.anyKeyDown)
            return;
        if (Input.GetKeyDown(KeyCode.X)) {
            UnitPanelManager.I.close();
        }
    }

    // Called only by slot buttons. 
    // A slot will either attempt to be filled if it is empty and a unit type is selected,
    // otherwise it will select if not selected, or deselect if selected.
    public void handle_slot(Slot slot) {
        if (CamSwitcher.I.current_cam != CamSwitcher.BATTLE)
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
        Unit u = BattlePhaser.I.active_bat.get_placeable_unit(
                    BatLoader.I.get_selected_unit_ID());
        if (highest_slot.fill(u)) {

            // Verify if all units are placed and can continue
            if (BattlePhaser.I.init_placement_stage)
                BattlePhaser.I.check_all_units_placed();

            if (BatLoader.I.selecting_for_heal)
                BatLoader.I.complete_heal();
            // Update inventory text
            BatLoader.I.load_unit_text(BattlePhaser.I.active_bat, 
                    BatLoader.I.get_selected_unit_ID()); 
            deselect();
        }
    }

    private void attempt_action(Slot slot) {
        if (selecting_target) {
            if (attempt_attack(slot)) {
                //int dmg = Attack.calc_final_dmg_taken(selected_slot.get_unit(), slot.get_unit());
                slot.update_healthbar();
                slot.update_defensebar();
                deselect();
            }
        } else if (selecting_move) {
            if (attempt_move(slot)) {
                deselect();
            }
        }
    }

    private bool attempt_attack(Slot target) {
        selecting_target = false;
        UnitPanelManager.I.player_panel.attB_pressed = false;

        // Range units can attack units not in the front slot.
        Slot highest_enemy_slot = selected_slot.get_punit().is_range ? 
            target : target.get_group().get_highest_enemy_slot();

        if (highest_enemy_slot == null)
            return false;

        return selected_slot.get_unit().attempt_set_up_attack(highest_enemy_slot);
    }

    private bool attempt_move(Slot target) {
        selecting_move = false;
        UnitPanelManager.I.player_panel.moveB_pressed = false;

        // Units are either moved up automatically or can be swapped. 
        if (!target.is_empty)
            return false;

        Slot highest_es = target.get_group().get_highest_empty_slot();
        target = highest_es;
        // Prevent pointless movement within same group.
        if (selected_slot.get_group() == highest_es.get_group()) {
            return false;
        }
        return selected_slot.get_punit().attempt_move(target);
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
        //selected_slot.set_color();
        UnitPanelManager.I.show(slot);
        BatLoader.I.clear_placement_selection();
    }

    public void deselect() {
        if (selected_slot == null)
            return;
        //selected_slot.show_selection(false);
        //selected_slot.set_color();
        selected_slot.show_selection(false);
        UnitPanelManager.I.close();
        selected_slot = null;
        selecting_target = false;
        selecting_move = false;
    }

    private bool verify_placement(Slot dest) {
        bool valid_init_place = 
            BattlePhaser.I.init_placement_stage && 
             dest.is_type(Group.PLAYER);
        bool valid_post_init_place = 
            (dest.is_type(Group.PERIPHERY)) &&
            !BattlePhaser.I.init_placement_stage;

        if ((BatLoader.I.selecting_for_heal || BattlePhaser.I.placement_stage) && 
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
        BatLoader.I.load_unit_text(BattlePhaser.I.active_bat, removed_unit_ID);
        BattlePhaser.I.check_all_units_placed(); // Can we move past placement?
    }
}
