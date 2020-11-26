using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelCardManager : MonoBehaviour {
    public static TravelCardManager I { get; private set; }
    public Die die;
    private int num_sides;
    private TravelCard tc;
    MapCell cell;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Don't pull a travel card on a discovered cell.
    public void draw_and_display_travel_card(MapCell cell) {
        //cell = Map.I.get_current_cell();
        this.cell = cell;

        // Load forced travel card from a previous save.
        if (Controller.I.get_disc().restart_battle_from_drawn_card) {
            MapUI.I.display_travelcard(Controller.I.get_disc().get_travelcard());
            Controller.I.get_disc().restart_battle_from_drawn_card = false;
            Debug.Log("resuming loaded battle");
            return;
        }

        if (!cell.creates_travelcard)
            return;

        if (cell.travelcard == null) {
            // Draw card
            cell.travelcard = TravelDeck.I.draw_card(cell.tier, cell.biome_ID);
        }

        if (cell.travelcard != null && !cell.travelcard_complete) {
            cell.travelcard.action(TravelCardManager.I);
            MapUI.I.display_travelcard(cell.travelcard);
        }
    }

    // Activated when 'Continue' is pressed on travel card.
    public void continue_travel_card() {
        // Preempt entrance with warning.
        if ((cell.biome_ID == MapCell.CAVE_ID || cell.biome_ID == MapCell.RUINS_ID) 
                && cell.has_enemies) {
            MapUI.I.set_active_ask_to_enterP(true);
        } else {
            handle_travelcard(cell);
        }
    }

    // Called by the continue button of the travel card. 
    public void handle_travelcard(MapCell cell) {
        if (cell == null)
            return;
        if (!cell.creates_travelcard || cell.travelcard_complete)
            return;
            
        if (cell.travelcard.follow_rule(TravelCard.ENTER_COMBAT)) { // If a combat travel card was pulled.
            BattlePhaser.I.begin_new_battle(cell);
        } else if (cell.travelcard.follow_rule(TravelCard.AFFECT_RESOURCES)) {
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
    }

    public void set_up_roll(TravelCard tc, int num_sides) {
        this.tc = tc;
        this.num_sides = num_sides;
        MapUI.I.set_active_travelcard_continueB(false);
        MapUI.I.set_active_travelcard_rollB(true);
    }
}
