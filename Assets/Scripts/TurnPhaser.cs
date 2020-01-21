﻿using System.Collections;
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

    public const int MINE_QTY = 3;

    public bool moving = false;
    private bool pulled_combat_card = false;

    private Controller c;
    private TravelDeck td;
    private TileMapper tm;
    private TravelCardManager tcm;
    private CamSwitcher cs;
    private Selector selector;
    private EnemyLoader enemy_loader;

    public Button next_turnB;
    public Button mineB;

    private Vector3 pos;
    private MapCell cell;
    private TravelCard tc;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        selector = c.selector;
        cs = c.cam_switcher;
        enemy_loader = c.enemy_loader;
        tm = c.tile_mapper;
        td = c.travel_deck;
        tcm = c.travel_card_manager;

        advance_stage();
        pre_phase();
    }

    public void advance_stage() {
        stage++;
    }

    private void pre_phase() {
        // Disable next turn button
        disable_mineB();
        cell = tm.get_cell(c.get_disc().pos);
        if (check_mineable(cell)) 
            enable_mineB();
    }

    private void movement() {
        moving = true;
    }

    private void travel_card() {
        cell = tm.get_cell(c.get_disc().pos);

        if (cell.discovered) {
            Debug.Log("Cell " + cell + " was already discovered.");
            advance_stage(); // No travel card, enter action.
        } else {
            cell.discover();
            if (!cell.no_travel_card) {

                // DRAW CARD
                tc = td.draw_card(cell.tier, cell.ID);
                tcm.set_card(c.get_disc_name(), tc);
                td.display_card(tc);

                if (tc.type == TravelCard.COMBAT) {
                    handle_combat_card(tc);
                } 
            } else {
                // Activate voluntary end turn button.
                c.map_ui.activate_next_stageB(true);
            }
        }
        // Show resources available on cell / buildings
    }

    // Called by the close button of the travel card. 
    private void action() {
        if (tc.follow_rule(TravelCard.ENTER_COMBAT)) { // If a combat travel card was pulled.
            //enemy_loader.load(cell.tier, tc.type, tc.enemies);
            enemy_loader.load(cell.ID, cell.tier, tc.enemies);
            cs.set_active(CamSwitcher.BATTLE, true);
            
        } else if (cell.has_enemies) { // Someone retreated from here.
            if (cell.get_enemies().Count > 0) { // either mini retreat or 
                enemy_loader.load_existing_enemies(cell.get_enemies());

            } else { // Resume battle exactly as it was.
                c.formation.load_board(c.active_disc);
            }
            cs.set_active(CamSwitcher.BATTLE, true);

        } else {
            Debug.Log("doing nothing");
            // other actions

            if (check_mineable(cell)) {
                enable_mineB();
            } 
            c.map_ui.activate_next_stageB(true);
        } 
            //if () check then enable build button
    }

    private void finish_phase() {
        pulled_combat_card = false;
        cs.set_active(CamSwitcher.MAP, true);
        player++;
        pre_phase();
    }

    private void handle_combat_card(TravelCard tc) {
        pulled_combat_card = true;
        c.get_active_bat().in_battle = true; 
    }

    // Stage and player adjustment happen internally only. 
    // advance_stage is the only externally accessible way to advance.
    private int _stage = 0;
    private int stage {
        get { return _stage; }
        set {
            _stage = value;
            if (_stage == MOVEMENT) {

                // Don't move if resuming battle.
                Debug.Log(c.get_active_bat().in_battle);
                if (c.get_active_bat().in_battle) {
                    stage = ACTION; // Bypasses travel card and action.
                } else
                    movement();
            }
            else if (_stage == TRAVEL_CARD) {
                moving = false;
                Debug.Log("travel card stage");
                travel_card();
            }
            else if (_stage == ACTION) {
                Debug.Log("action stage");
                action();
            }
            else if (_stage > ACTION) {
                finish_phase();
                stage = MOVEMENT;
            }
        }
    }

    private int _player = 1;
    private int player {
        get { return _player; }
        set {
            _player = value;
            c.rotate_disc();
            if (_player > c.num_discs) {
                _player = 1;
                c.advance_turn();
            }
        }
    }

    public void mine() {
        disable_mineB();
        int mine_qty = c.get_active_bat().mine_qty;
        if (cell.name == MapCell.TITRUM || cell.name == MapCell.MOUNTAIN) {
            if (cell.minerals >= mine_qty) {
                c.get_disc().change_var(Storeable.MINERALS, mine_qty);
            } else {
                c.get_disc().change_var(Storeable.MINERALS, cell.minerals);
            }
        }
    }

    private bool check_mineable(MapCell cell) {
        if (!c.get_active_bat().has_miner()) {
            return false;
        }
        if (cell.minerals <= 0 || cell.star_crystals <= 0) {
            return false;
        }
        if (cell.name == MapCell.TITRUM || 
            cell.name == MapCell.MOUNTAIN || 
            cell.name == MapCell.STAR) {
            return true;
        }
        return false;
    }

    private void enable_mineB() {
        mineB.interactable = true;
    }

    public void disable_mineB() {
        mineB.interactable = false;
    }
}
