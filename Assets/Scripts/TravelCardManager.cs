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
        if (Controller.I.get_disc().restart_battle_from_drawn_card) {
            MapUI.I.display_travelcard(Controller.I.get_disc().get_travelcard());
            Controller.I.get_disc().restart_battle_from_drawn_card = false;
            Debug.Log("resuming loaded battle");
            return;
        }
    }


    // Activated when 'Continue' is pressed on travel card or 'Yes' on warning to enter.
    public void continue_travel_card(bool show_warning=true) {
        MapCell cell = MapUI.I.last_open_cell;
        // Preempt entrance with warning.
        // cell does not have enemies at this point
        if (show_warning && 
            ((cell.biome_ID == MapCell.CAVE_ID || 
            cell.biome_ID == MapCell.RUINS_ID) && 
            cell.travelcard.rules.enter_combat)) {
            MapUI.I.set_active_ask_to_enterP(true);
        } else {
            handle_travelcard(cell);
        }
    }

    // Called by the continue button of the travel card. 
    public void handle_travelcard(MapCell cell) {
        Debug.Log("handling travelcard");
        if (cell == null)
            return;
        if (!cell.creates_travelcard || cell.travelcard_complete)
            return;
            
        if (cell.travelcard.rules.enter_combat) { // If a combat travel card was pulled.
        Debug.Log("beginning battle");
            BattlePhaser.I.begin_new_battle(cell);
        } else if (cell.travelcard.rules.affect_resources) {
            Controller.I.get_disc().adjust_resources_visibly(cell.get_travelcard_consequence());
            cell.complete_travelcard();
        } 
    }


    // Called by the roll button of the travel card. 
    public void roll() {
        die.roll(num_sides);
    }

    public void finish_roll(int result) {
        MapUI.I.set_active_travelcard_continueB(true);
        //set_rollB(false);
        tc.use_roll_result(result, Controller.I);
        tc = null;
    }

    public void set_up_roll(TravelCard tc, int num_sides) {
        this.tc = tc;
        this.num_sides = num_sides;
        MapUI.I.set_active_travelcard_continueB(false);
        MapUI.I.set_active_travelcard_rollB(true);
    }
}
