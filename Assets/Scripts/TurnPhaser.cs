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

        map_ui.disable_mineB();
        map_ui.disable_scoutB();
        map_ui.set_active_rune_gateB(false);
        stage = MOVEMENT;
    }

    public void advance_stage() {
        stage++;
    }

        // Stage adjustment happens internally only. 
    // advance_stage is the only externally accessible way to advance.
    private int _stage = 0;
    private int stage {
        get { return _stage; }
        set {
            _stage = value;

            if (_stage == MOVEMENT) {
                map_ui.set_next_stageB_text("Movement");
                // Don't move if resuming battle.
                if (c.get_active_bat().in_battle) {
                    stage = ACTION; // Bypasses travel card and action.
                } else
                    moving = true;
            }
            else if (_stage == TRAVEL_CARD) {
                Debug.Log("travel card stage");
                travel_card();
            }
            else if (_stage == ACTION) {
                Debug.Log("action stage");
                action();
            }
            else if (_stage > ACTION) {
                advance_player();
                stage = MOVEMENT;
            }
        }
    }

    private void advance_player() {
        c.active_disc_ID = (c.active_disc_ID + 1) % 3;
        if (c.active_disc_ID == 0)
            c.advance_turn();
        
        reset();
        cs.set_active(CamSwitcher.MAP, true);
    }

    public void reset() { // New game, new player
        tc = null;
        cell = c.tile_mapper.get_cell(c.get_disc().pos);
        map_ui.set_active_next_stageB(false);
        map_ui.set_active_rune_gateB(false);
        map_ui.set_active_ask_to_enterP(false);
        map_ui.set_active_game_lossP(false);
        map_ui.set_active_mineB(check_mineable(cell));
        map_ui.set_active_scoutB(c.get_active_bat().get_unit(PlayerUnit.SCOUT) != null);
        map_ui.update_cell_text(cell.name);
        stage = MOVEMENT;
    }

    // Don't pull a travel card on a discovered cell.
    public void travel_card() {
        moving = false;
        _stage = TRAVEL_CARD;
        map_ui.set_next_stageB_text("Travel Card");
        cell = c.tile_mapper.get_cell(c.get_disc().pos);

        // Load forced travel card from a previous save.
        if (c.get_disc().restart_battle_from_drawn_card) {
            td.display_card(c.get_disc().get_travelcard());
            c.get_disc().restart_battle_from_drawn_card = false;
            Debug.Log("resuming prev battle");
            return;
        }

        if (!cell.discovered) {
            cell.discover();
            if (cell.creates_travelcard) { // Cell can have cards drawn.
                
                // DRAW CARD
                tc = td.draw_card(cell.tier, cell.ID);
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
    // 
    public void action() {
        _stage = ACTION;
        map_ui.set_next_stageB_text("End Turn");

        if (tc != null) {
            handle_travelcard(tc);
        } else if (cell.has_enemies) { // Someone retreated from here.
            c.get_active_bat().in_battle = true;
            if (cell.get_enemies().Count > 0) { // either mini retreat or 
                enemy_loader.load_existing_enemies(cell.get_enemies());

            } else { // Resume battle exactly as it was.
                c.formation.load_board(c.active_disc_ID);
            }
            cs.set_active(CamSwitcher.BATTLE, true);
            return;
        } 

        Debug.Log("no travelcard, no enemies in tile. Can mine?");
        if (check_mineable(cell)) {
            map_ui.enable_mineB();
        } 
        c.map_ui.set_active_next_stageB(true);
        //if () check then enable build button

        // Check for settlement and adjust UI accordingly.
    }

    private void handle_travelcard(TravelCard tc) {
        if (tc.follow_rule(TravelCard.ENTER_COMBAT)) { // If a combat travel card was pulled.
            begin_new_combat(tc);
        } else if (tc.follow_rule(TravelCard.AFFECT_RESOURCES)) {
            if (tc.requires_seeker) {
                if (c.get_active_bat().has_seeker) {
                    StartCoroutine(c.get_disc().adjust_resources_visibly(tc.consequence));
                    c.get_disc().complete_travelcard();
                }    
            } else {
                StartCoroutine(c.get_disc().adjust_resources_visibly(tc.consequence));
                c.get_disc().complete_travelcard();
            }
        } 
    }

    private void begin_new_combat(TravelCard tc) {
        Debug.Log("setting in_battle to " + c.get_active_bat().in_battle + 
        "for " + c.active_disc_ID + " at " + cell.pos.x + ", " + cell.pos.y);
        c.get_active_bat().in_battle = true; 
        enemy_loader.load(cell.ID, cell.tier, tc.enemies);
        cs.set_active(CamSwitcher.BATTLE, true);
    }

    public void mine() {
        map_ui.disable_mineB();
        int mine_qty = c.get_active_bat().mine_qty;
        if (cell.ID == MapCell.TITRUM_ID || cell.ID == MapCell.MOUNTAIN_ID) {
            if (cell.minerals >= mine_qty) {
                c.get_disc().change_var(Storeable.MINERALS, mine_qty, true);
            } else {
                c.get_disc().change_var(Storeable.MINERALS, cell.minerals, true);
            }
        } else if (cell.ID == MapCell.STAR_ID) {
            if (cell.star_crystals >= mine_qty) {
                c.get_disc().change_var(Storeable.STAR_CRYSTALS, mine_qty, true);
            } else {
                c.get_disc().change_var(Storeable.STAR_CRYSTALS, cell.star_crystals, true);
            }
        }
        c.map_ui.set_active_next_stageB(true);
    }

    private bool check_mineable(MapCell cell) {
        return (c.get_active_bat().has_miner && 
            (cell.minerals > 0 || cell.star_crystals > 0));
    }
}

public class Turn {
    int stage;
}
