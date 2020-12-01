using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controls the game logic of the battle phases
public class BattlePhaser : MonoBehaviour {
    public static BattlePhaser I { get; private set; }
    public const int PLACEMENT = 0;
    public const int INTUITIVE = 1;
    public const int RANGE = 2;
    public const int MOVEMENT1 = 3;
    public const int COMBAT1 = 4;
    public const int MOVEMENT2 = 5;
    public const int COMBAT2 = 6;
    public const int ASSESSMENT = 7;


    public Button adv_stageB;
    public TextMeshProUGUI stageT, phaseT; 

    private List<Image> disc_icons = new List<Image>();
    public Image disc_icon1, disc_icon2, disc_icon3;
    // <discipline ID, disc Sprite>
    private Dictionary<int, Sprite> disc_sprites = new Dictionary<int, Sprite>();
    public Sprite astra_icon, martial_icon, endura_icon;
    public Sprite empty_sprite;
    public Color inactive_color;
    public Image biome_icon;
    // Lookup by biome, tier
    private Dictionary<int, Sprite> biome_sprites  = new Dictionary<int, Sprite>();
    public Sprite plains1, forest1, ruins1, cliff1, cave1, titrum1;
    public Sprite plains2, forest2, ruins2, cliff2, cave2, titrum2, mountain2, mire2;
    public bool placement_stage = true;
    public bool init_placement_stage = true;
    public bool range_stage = false;
    public bool movement_stage = false;
    public bool combat_stage = false;
    public bool targeting { get { return combat_stage || range_stage; } }
    private Battle _battle;
    private Battle battle {
        get { return _battle; }
        set {
            _battle = value;
        }
    }
    public Battalion active_bat { 
        get { 
            if (battle == null)
                return null;
            return Controller.I.get_bat_from_ID(battle.active_bat_ID); 
        }
    }
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        disc_icons.Add(disc_icon1);
        disc_icons.Add(disc_icon2);
        disc_icons.Add(disc_icon3);
        disc_sprites.Add(Discipline.ASTRA, astra_icon);
        disc_sprites.Add(Discipline.MARTIAL, martial_icon);
        disc_sprites.Add(Discipline.ENDURA, endura_icon);

        biome_sprites.Add(Map.PLAINS_1, plains1);
        biome_sprites.Add(Map.PLAINS_2, plains2);
        biome_sprites.Add(Map.FOREST_1, forest1);
        biome_sprites.Add(Map.RUINS_1, ruins1);
        biome_sprites.Add(Map.RUINS_2, ruins2);
        biome_sprites.Add(Map.CLIFF_1, cliff1);
        biome_sprites.Add(Map.CLIFF_2, cliff2);
        biome_sprites.Add(Map.CAVE_1, cave1);
        biome_sprites.Add(Map.CAVE_2, cave2);
        biome_sprites.Add(Map.TITRUM_1, titrum1);
        biome_sprites.Add(Map.TITRUM_2, titrum2);
        biome_sprites.Add(Map.MOUNTAIN_2, mountain2);
        
    }

    // Called at the end of 3 phases.
    public void reset(bool reset_battlefield=true) {
        phase = 1;
        can_skip = false;

        placement_stage = false;
        init_placement_stage = true;
        range_stage = false;
        movement_stage = false;
        combat_stage = false;
        if (reset_battlefield) {
            battle = null;
            Formation.I.reset_groups_dir();
            Formation.I.clear_battlefield();
        }
        stage = PLACEMENT;
        update_phase_text();
    }

    public void begin_new_battle(MapCell cell) {
        battle = cell.battle;
        // Must be solo battle if null, because group battles will
        // have been created at least one turn in advance of battle initiation.
        if (battle == null) { 
            battle = new Battle(Map.I, cell, Controller.I.get_disc(), false);
            cell.battle = battle;
            battle.begin();
        } 

        if (!cell.has_seen_combat) {
            Debug.Log("generating enemies");
            EnemyLoader.I.generate_new_enemies(cell, cell.travelcard.enemy_count);
        }
        Debug.Log("loading enemies");
        EnemyLoader.I.load_enemies(cell.get_enemies());
        cell.has_seen_combat = true;

        setup_participant_UI(cell);
        MapUI.I.close_cell_UI();
        CamSwitcher.I.set_active(CamSwitcher.BATTLE, true);
    }

    // First determine if a battle is grouped or not, then either resume or reload.
    /*
    Resumed battles are battles that did not finish in a single turn.
    */
    public void resume_battle(MapCell cell) {
        battle = cell.battle;
        if (battle == null)
            return;

        Formation.I.load_board(battle);
        setup_participant_UI(cell);
        CamSwitcher.I.set_active(CamSwitcher.BATTLE, true);
    }

    // At battle initialization.
    private void setup_participant_UI(MapCell cell) {
        battle = cell.battle;
        if (battle == null) {
            // only activate one image
            disc_icon1.sprite = disc_sprites[TurnPhaser.I.active_disc_ID];
            Debug.Log("this should never happen");
        }
        else {
            // Order discipline images by group join time.
            for (int i = 0; i < 3; i++) {
                if (battle.participants.Count - 1 >= i) {
                    disc_icons[i].sprite = disc_sprites[battle.participants[i].ID];
                } else {
                    disc_icons[i].sprite = empty_sprite;
                }
            }
        }
        biome_icon.sprite = biome_sprites[Map.I.tile_to_tileID[battle.cell.tile]];
        BatLoader.I.load_bat(active_bat);
        update_participant_UI();
    }

    private void update_participant_UI() {
        BatLoader.I.load_bat(active_bat);
        update_advance_stageB(battle.active_bat_ID);
        
        if (!battle.is_group) {
            update_phase_text();

            // Update icons
            disc_icon1.color = Color.white;
            disc_icon2.color = Color.clear;
            disc_icon3.color = Color.clear;
        } else {
            // Update text
            if (battle.leaders_turn)
                update_phase_text();

            // Update icons
            /* If in battle and active, color the icon white, 
            if in battle and not active, color half transparent,
            if not in battle color transparent. */
            for (int i = 0; i < 3; i++) {
                if (battle.participants.Count - 1 >= i) {
                    // Active
                    if (battle.active_bat_ID == battle.participants[i].ID)
                        disc_icons[i].color = Color.white;
                    else // Inactive
                        disc_icons[i].color = inactive_color;
                } else {
                    disc_icons[i].color = Color.clear;
                }
            }
        }
    }

    public void advance() {
        battle.advance_turn();
        if (battle.leaders_turn)
            advance_stage();
        update_participant_UI();
    }

    private void advance_stage() {
        Selector.I.deselect();
        stage = _stage + 1;
    }

    /*
    Outwardly, the stage number loops 0 -> num_stages once for each of a turn's 3 battle phases.
    Inwardly, it increments without wrapping.
    */
    private int num_stages = 8;
    // Begin one stage less so that stage increment initializes placement.
    private int _stage = PLACEMENT - 1; 
    private int stage {
        get { return _stage % num_stages; }
        set {
            _stage = value;
            int phase_stage = stage;

            if (phase_stage == PLACEMENT) placement();
            else if (phase_stage == INTUITIVE) intuitive();
            else if (phase_stage == RANGE)range();
            else if (phase_stage == MOVEMENT1) {
                range_stage = false;
                movement();
            } else if (phase_stage == COMBAT1) combat();
            else if (phase_stage == MOVEMENT2) {
                combat_stage = false;
                movement();
            } else if (phase_stage == COMBAT2) combat();
            else if (phase_stage == ASSESSMENT) assessment();

            if (battle != null)
                battle.clear_stage_actions();
        }
    } 

    private int _phase = 1;
    // After 3 phases, battle yields until the next turn.
    private int phase {
        get { return _phase; }
        set {
            _phase = value; 
            phaseT.text = "Phase " + phase;

            if (_phase > 3) {
                post_phases();
            }
        }
    }

    // ---STAGES---
    private void placement() {
        if (_stage == PLACEMENT) // Require unit placement in 1st phase.
            can_skip = false;
        placement_stage = true;
    }

    private void intuitive() {
        placement_stage = false;
        init_placement_stage = false;
        can_skip = true;
        Selector.I.deselect();
        BatLoader.I.clear_placement_selection();
        advance_stage();
        // Scan through placed characters and the enemy to determine initial effects.
    }

    private void range() {
        if (!battle.cell.travelcard.follow_rule(TravelCard.AMBUSH)) {
            range_stage = true;
        }
        can_skip = true;
        // Ranged units attack (enemy and player after player chooses who to attack)
        // (unless someone has first strike!)
        // range units can shoot anywhere, riflemen can only shoot cardinally
        EnemyBrain.I.stage_range_attacks();
    }

    private void movement() {
        enter_battle(); // range attacks

        movement_stage = true;
    }

    private void combat() {
        movement_stage = false;
        EnemyBrain.I.move_units();
        EnemyBrain.I.stage_attacks();

        combat_stage = true;
    }

    private void assessment() {
        enter_battle();
        combat_stage = false;
    }
    // ---END FORMAL STAGES---

    private void enter_battle() {
        can_skip = false;
        StartCoroutine(AttackQueuer.I.battle());
    }

    // Only called by AttackQueuer after battle animations have finished.
    public void post_battle() {
        battle.post_battle();
        if (stage == ASSESSMENT) advance_phase();
        can_skip = true;

        check_finished();
    }

    private void advance_phase() {
        Formation.I.rotate_actioned_player_groups();
        battle.post_phase(); // reset movement/attacks
        phase++;
    }

    /* Battlion deaths trigger their respawn.
    The game is only over when the city runs out of health or
    the player quits.
    */
    private void post_phases() {
        if (!battle_finished) {
            // If the battle is not finished, save the board exactly as it is.
            Formation.I.save_board(battle);
            return;
        }

        if (player_won) {
            Debug.Log("Player won the battle.");
            battle.leader.adjust_resources_visibly(battle.cell.get_travelcard_consequence());
            battle.cell.complete_travelcard();
            battle.end();
        } else if (enemy_won) {
            if (!Controller.I.get_disc().dead) {
                battle.retreat();
            }
            // Forced retreat.
            Debug.Log("Enemy won the battle. This will only be called if the player has no units on the field, but some in reserve.");
        }
        reset();
        CamSwitcher.I.set_active(CamSwitcher.MAP, true);
    }

    // Called by Unity button. 
    public void retreat() {
        if (!player_units_on_field || player_won)
            return;
        
        battle.retreat();
        CamSwitcher.I.set_active(CamSwitcher.MAP, true);
        reset();
    }

    private void check_finished() {
        if (battle_finished)
            post_phases();
    }

    private bool units_in_reserve {
        get { return battle.count_all_units_in_reserve() > 0; }
    }

    private bool player_units_on_field { 
        //get { return Controller.I.get_active_bat().get_all_placed_units().Count > 0; }
        // Ignores if an individual battalion should be retreated and ignored int eh turn cycle.
        get { return battle.count_all_placed_units() > 0; }
    } 

    // check each combat stage end
    // battalion_dead implies enemy_won, enemy_won does not imply battalion_dead.
    private bool battalion_dead {
        get => active_bat.count_healthy() <= 0;
    }

    private bool player_won { 
        //get { return Formation.I.get_all_full_slots(Unit.ENEMY).Count <= 0; }
        get { return battle.cell.get_enemies().Count <= 0; }
    } 

    private bool enemy_won {
        get { return !player_units_on_field; } //&& !units_in_reserve; }
    }

    private bool battle_finished { get { return player_won || enemy_won; } }

    private bool _can_skip = false;
    public bool can_skip {
        get { return _can_skip; }
        set { 
            _can_skip = value;
            adv_stageB.interactable = _can_skip;
        }
    }

    public void check_all_units_placed() {
        if (!init_placement_stage)
            return;

        // EDIT FOR TESTING------------------------------------------------------------------------------------?
        can_skip = true;
        //can_skip = units_in_reserve ? false : true; // use 
    }

    private void update_phase_text() {
        if (stage == PLACEMENT) {
            stageT.text = "Placement";
        } else if (stage == RANGE) {
            stageT.text = "Range";
        } else if (stage == MOVEMENT1) {
            stageT.text = "Movement 1";
        } else if (stage == COMBAT1) {
            stageT.text = "Combat 1";
        } else if (stage == MOVEMENT2) {
            stageT.text = "Movement 2";
        } else if (stage == COMBAT2) {
            stageT.text = "Combat 2";
        } else if (stage == ASSESSMENT) {
            stageT.text = "Assessment";
        }
    }

    private void update_advance_stageB(int bat_ID) {
        TextMeshProUGUI txt = adv_stageB.GetComponentInChildren<TextMeshProUGUI>();

        if (battle.last_battalions_turn) { // Always the last bat's turn in solo battles.
            string basestr = "Advance to ";
            if (stage == PLACEMENT) {
                txt.text = basestr + "range stage";
            } else if (stage == RANGE) {
                txt.text = basestr + "1st movement stage";
            } else if (stage == MOVEMENT1) {
                txt.text = basestr + "1st combat stage";
            } else if (stage == COMBAT1) {
                txt.text = basestr + "2nd movement stage";
            } else if (stage == MOVEMENT2) {
                txt.text = basestr + "2nd combat stage";
            } else if (stage == COMBAT2) {
                txt.text = basestr + "assessment stage";
            } else if (stage == ASSESSMENT) {
                txt.text = basestr + "placement stage";
            }
        } else {
            txt.text = "End " + Controller.I.get_disc(bat_ID).name;
        }
    }
}
