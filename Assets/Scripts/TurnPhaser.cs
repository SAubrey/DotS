using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

// Manages intra-turn progression. Stages are within phases which are within a game turn.
// There are 3 stages for each phase, and a phase for each player.
public class TurnPhaser : MonoBehaviour {
    public const int MOVEMENT = 1;
    public const int TRAVEL_CARD = 2;
    public const int ACTION = 3; // mine, build

    private Controller c;
    private TravelDeck td;
    private CamSwitcher cs;
    private MapUI map_ui;
    private EnemyLoader enemy_loader;

    private MapCell cell;
    private TravelCard tc;
    public bool moving = false;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        cs = c.cam_switcher;
        enemy_loader = c.enemy_loader;
        td = c.travel_deck;
        map_ui = c.map_ui;

        stage = MOVEMENT;
    }

    public void advance_stage() {
        stage++;
    }

    // Activatedw hen 'Continue' is pressed on travel card.
    public void continue_travel_card() {
        //if (tc.follow_rule(TravelCard.))
        if ((cell.biome_ID == MapCell.CAVE_ID || cell.biome_ID == MapCell.RUINS_ID) 
                && !cell.travelcard_complete) {
            c.map_ui.set_active_ask_to_enterP(true);
        } else {
            advance_stage();
        }
    }

        // Stage adjustment happens internally only. 
    // advance_stage is the only externally accessible way to advance.
    private int _stage = 0;
    private int stage {
        get { return _stage; }
        set {
            _stage = value;

            if (_stage == MOVEMENT) {
                //map_ui.set_next_stageB_text("Movement");
                moving = true;
            }
            else if (_stage == TRAVEL_CARD) {
                //Debug.Log("travel card stage");
                travel_card();
            }
            else if (_stage == ACTION) {
                //Debug.Log("action stage");
                action();
            }
            else if (_stage > ACTION) {
                end_disciplines_turn();
            }
        }
    }

    
    public void end_disciplines_turn() {
        c.map.close_cell_UI();
        advance_player();
    }

    private void advance_player() {
        c.active_disc_ID++;
        if (c.active_disc_ID == 0)
            c.advance_turn();
        
        // If leader of a battle, bypass stages and enter battle.
        Battle b = c.map.get_current_cell().battle;
        if (b != null) {
            if (c.get_disc() == b.leader) {
                c.battle_phaser.resume_battle(c.map.get_current_cell());
            // If in battle but not leader, skip turn. (Turn used in group battle)
            } else {
                advance_player();
                return;
            }
        }
        reset();
        cs.set_active(CamSwitcher.MAP, true);
    }

    public void reset() { // New game, new player
        tc = null;
        cell = c.map.get_cell(c.get_disc().pos);
        //map_ui.set_active_next_stageB(false);
        map_ui.set_active_ask_to_enterP(false);
        map_ui.set_active_game_lossP(false);
        map_ui.update_cell_text(cell.name);
        stage = MOVEMENT;
    }

    // Don't pull a travel card on a discovered cell.
    public void travel_card() {
        moving = false;
        _stage = TRAVEL_CARD;
        //map_ui.set_next_stageB_text("Travel Card");
        cell = c.map.get_cell(c.get_disc().pos);

        // Load forced travel card from a previous save.
        if (c.get_disc().restart_battle_from_drawn_card) {
            td.display_card(c.get_disc().get_travelcard());
            c.get_disc().restart_battle_from_drawn_card = false;
            Debug.Log("resuming loaded battle");
            return;
        }

        if (!cell.discovered) {
            cell.discover();

            if (cell.creates_travelcard) { // Cell can have cards drawn.
                
                // DRAW CARD
                tc = td.draw_card(cell.tier, cell.biome_ID);
                c.get_disc().set_travelcard(tc);
                td.display_card(tc);
                tc.action(c.travel_card_manager);

                if (tc == null)
                    advance_stage();
            } else 
                advance_stage();
        } else {
            advance_stage(); // Been here, enter action.
        }
    }

    // Called by the continue button of the travel card. 
    public void action() {
        _stage = ACTION;
        //map_ui.set_next_stageB_text("End Turn");

        if (tc != null) {
            handle_travelcard(tc);
        }
        //c.map_ui.set_active_next_stageB(true);
        //if () check then enable build button

        // Check for settlement and adjust UI accordingly.
    }

    private void handle_travelcard(TravelCard tc) {
        if (tc.follow_rule(TravelCard.ENTER_COMBAT)) { // If a combat travel card was pulled.
            c.battle_phaser.begin_new_battle(tc, cell);
        } else if (tc.follow_rule(TravelCard.AFFECT_RESOURCES)) {
            StartCoroutine(c.get_disc().adjust_resources_visibly(tc.consequence));
            c.get_disc().complete_travelcard();
        } 
    }
}

public class Turn {
    int stage;
}
