using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelCardManager : MonoBehaviour {
    public static TravelCardManager I { get; private set; }
    public Die die;
    private int num_sides;
    private TravelCard tc;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Don't pull a travel card on a discovered cell.
    public void restart_battle_from_drawn_card(MapCell cell) {
        // Load forced travel card from a previous save.
        if (TurnPhaser.I.active_disc.restart_battle_from_drawn_card) {
            MapUI.I.display_travelcard(TurnPhaser.I.active_disc.get_travelcard());
            TurnPhaser.I.active_disc.restart_battle_from_drawn_card = false;
            Debug.Log("resuming loaded battle");
            return;
        }
    }


    // Activated when 'Continue' is pressed on travel card or 'Yes' on warning to enter.
    public void continue_travel_card(bool show_warning=true) {
        MapCell cell = MapUI.I.last_open_cell;
        // Preempt entrance with warning?
        if (show_warning && 
            ((cell.ID == MapCell.CAVE_ID || 
            cell.ID == MapCell.RUINS_ID) && 
            cell.travelcard.rules.enter_combat)) {
            MapUI.I.set_active_ask_to_enterP(true);
            return;
        } 
        activate_travelcard(cell);
    }

    // Called by the continue button of the travel card. 
    public void activate_travelcard(MapCell cell) {
        if (cell == null)
            return;
        if (!cell.creates_travelcard || cell.travelcard_complete)
            return;
        if (cell.travelcard == null) {
            throw new System.ArgumentException("Cell must have a travelcard");
        }

        cell.travelcard.on_continue(TravelCardManager.I);
        if (cell.travelcard.rules.enter_combat) { // If a combat travel card was pulled.
            BattlePhaser.I.begin_new_battle(cell);
        } else if (cell.travelcard.rules.affect_resources && !cell.locked) {
            TurnPhaser.I.active_disc.receive_travelcard_consequence();
        } 
    }


    // Called by the roll button of the travel card. 
    public void roll() {
        die.roll(num_sides);
        tc.rolled = true;
    }

    public void finish_roll(int result) {
        MapUI.I.set_active_travelcard_continueB(true);
        MapUI.I.travelcard_rollB.interactable = false;
        tc.use_roll_result(result);
        tc = null;
    }

    public void set_up_roll(TravelCard tc, int num_sides) {
        this.tc = tc;
        this.num_sides = num_sides;
        MapUI.I.set_active_travelcard_continueB(false);
        MapUI.I.set_active_travelcard_rollB(true);
    }
}
