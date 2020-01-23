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
    private TileMapper tm;
    private TravelCardManager tcm;
    private CamSwitcher cs;
    private Selector selector;
    private EnemyLoader enemy_loader;

    public Button next_turnB;
    public Button mineB;

    private MapCell cell;
    private TravelCard tc;
    public bool moving = false;

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

    // Don't pull a travel card on a discovered cell.
    private void travel_card() {
        cell = tm.get_cell(c.get_disc().pos);

        if (!cell.discovered) {
            cell.discover();
            if (cell.has_travel_card) {

                // DRAW CARD
                tc = td.draw_card(cell.tier, cell.ID);
                c.get_disc().travel_card = tc;
                td.display_card(tc);

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
    private void action() {
        if (tc != null) {
            handle_travelcard(tc);
        } else if (cell.has_enemies) { // Someone retreated from here.
            if (cell.get_enemies().Count > 0) { // either mini retreat or 
                enemy_loader.load_existing_enemies(cell.get_enemies());

            } else { // Resume battle exactly as it was.
                c.formation.load_board(c.active_disc);
            }
            cs.set_active(CamSwitcher.BATTLE, true);
            return;
        } 

        // Other MapUI actions?

        Debug.Log("no travelcard, no enemies in tile. Can mine?");
        if (check_mineable(cell)) {
            enable_mineB();
        } 
        c.map_ui.activate_next_stageB(true);
        //if () check then enable build button

        // Check for settlement and adjust UI accordingly.
    }

    private void handle_travelcard(TravelCard tc) {
        if (tc.follow_rule(TravelCard.ENTER_COMBAT)) { // If a combat travel card was pulled.
            begin_new_combat(tc);
        } else if (tc.follow_rule(TravelCard.AFFECT_RESOURCES)) {
            StartCoroutine(tc.adjust_resources(c));
        } 
    }

    private void begin_new_combat(TravelCard tc) {
        c.get_active_bat().in_battle = true; 
        enemy_loader.load(cell.ID, cell.tier, tc.enemies);
        cs.set_active(CamSwitcher.BATTLE, true);
    }

    private void finish_phase() {
        cs.set_active(CamSwitcher.MAP, true);
        player++;
        pre_phase();
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
                Debug.Log("in battle? " + c.get_active_bat().in_battle);
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
            tc = null;
            cell = null;
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
    }

    private bool check_mineable(MapCell cell) {
        if (!c.get_active_bat().has_miner()) {
            return false;
        }
        if (cell.minerals > 0 || cell.star_crystals > 0) {
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
