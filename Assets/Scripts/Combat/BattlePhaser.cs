using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls the game logic of the battle phases
public class BattlePhaser : MonoBehaviour {
    public const int PLACEMENT = 0;
    public const int INTUITIVE = 1;
    public const int RANGE = 2;
    public const int MOVEMENT1 = 3;
    public const int COMBAT1 = 4;
    public const int MOVEMENT2 = 5;
    public const int COMBAT2 = 6;
    public const int ASSESSMENT = 7;

    private Controller c;
    private TurnPhaser tp;
    private Selector selector;
    private AttackQueuer aq;
    private EnemyBrain enemy_brain;

    public Button adv_stageB;
    public Text stageT; 
    public Text phaseT; 

    public bool placement_stage = true;
    public bool init_placement_stage = true;
    public bool range_stage = false;
    public bool movement_stage = false;
    public bool combat_stage = false;
    public bool targeting { get { return combat_stage || range_stage; } }

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        tp = c.turn_phaser;
        selector = c.selector;
        aq = c.attack_queuer;
        enemy_brain = c.enemy_brain;

        reset();
    }

    // Called at the end of 3 phases.
    public void reset() {
        phase = 1;
        can_skip = false;

        placement_stage = false;
        init_placement_stage = true;
        range_stage = false;
        movement_stage = false;
        combat_stage = false;
        c.formation.reset_groups_dir();
        c.formation.clear_battlefield();
        stage = PLACEMENT;
    }

    public void advance_stage() {
        selector.deselect();
        stage = _stage + 1;
        update_advB_text();
    }

    /*
    Outwardly, the stage number loops 0 -> num_stages once for each of a turn's 3 battle phases.
    Inwardly, it increments without wrapping.
    */
    int num_stages = 8;
    // Begin one stage less so that stage increment initializes placement.
    private int _stage = PLACEMENT - 1; 
    private int stage {
        get { return _stage % num_stages; }
        set {
            _stage = value;
            int phase_stage = stage;

            if (phase_stage == PLACEMENT) placement();
            else if (phase_stage == INTUITIVE) intuitive();
            else if (phase_stage == RANGE) range();
            else if (phase_stage == MOVEMENT1) movement1();
            else if (phase_stage == COMBAT1) combat1();
            else if (phase_stage == MOVEMENT2) movement2();
            else if (phase_stage == COMBAT2) combat2();
            else if (phase_stage == ASSESSMENT) assessment();

            c.get_active_bat().reset_all_stage_actions();
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
        selector.deselect();
        c.bat_loader.clear_placement_selection();
        advance_stage();
        // Scan through placed characters and the enemy to determine initial effects.
    }

    private void range() {
        range_stage = true;
        can_skip = true;
        // Ranged units attack (enemy and player after player chooses who to attack)
        // (unless someone has first strike!)
        // range units can shoot anywhere, riflemen can only shoot cardinally

        enemy_brain.stage_range_attacks();
    }

    private void movement1() {
        range_stage = false;
        battle(); // range attacks

        movement_stage = true;
    }

    private void combat1() {
        movement_stage = false;
        enemy_brain.move_units();
        enemy_brain.stage_attacks();

        combat_stage = true;
    }

    private void movement2() {
        combat_stage = false;
        battle(); // will clear attacks

        movement_stage = true;
    }

    private void combat2() {
        movement_stage = false;
        enemy_brain.move_units();

        combat_stage = true;
        enemy_brain.stage_attacks();
    }

    private void assessment() {
        battle();
        combat_stage = false;
    }
    // ---END FORMAL STAGES---

    private void battle() {
        can_skip = false;
        StartCoroutine(aq.battle());
    }

    // Only called by AttackQueuer after battle animations have finished.
    public void post_battle() {
        if (stage == ASSESSMENT) advance_phase();
        can_skip = true;
        Debug.Log(c.get_active_bat().get_all_placed_units().Count);
        check_finished();
    }

    private void advance_phase() {
        c.formation.rotate_actioned_player_groups();
        c.get_active_bat().post_phase(); // reset movement/attacks
        enemy_brain.post_phase(); 
        phase++;
    }

    private void post_phases() {
        if (battle_finished) {
            Debug.Log("battle finished");

            if (player_won) {
                Debug.Log("Player won the battle.");
                c.get_disc().complete_travelcard();
            } else if (enemy_won) {
                Debug.Log("Enemy won the battle.");

            }
            c.cam_switcher.flip_map_battle();
        } else {
            if (mini_retreating) { // Fall back and regroup.
                Debug.Log("Mini retreating.");
                c.get_active_bat().mini_retreating = true;
                save_enemies_to_map();
            } else // Save the board exactly as is to resume next turn.
                c.formation.save_board(c.active_disc_ID);
        }
        reset();
        tp.advance_stage();
    }

    private void save_enemies_to_map() {
        // Save enemies on battle field into map cell storage.
        MapCell mc = c.tile_mapper.get_cell(c.get_disc().pos);
        mc.save_enemies(c.formation.get_all_full_slots(Unit.ENEMY));
    }

    // Called by Unity button. 
    public void retreat() {
        if (!player_units_on_field || player_won)
            return;

        save_enemies_to_map();
        // Penalize retreat.
        c.get_disc().change_var(Storeable.UNITY, -1, true);
        c.get_disc().set_travelcard(null);
        // Move unit back to previous space
        c.tile_mapper.move_player(c.get_disc().prev_pos);
        reset();
        tp.advance_stage();
    }

    private void check_finished() {
        if (battle_finished)
            post_phases();
    }

    private bool units_in_reserve {
        get { return c.get_active_bat().count_placeable() > 0; }
    }

    private bool player_units_on_field { 
        get { return c.get_active_bat().get_all_placed_units().Count > 0; }
    } 

    private bool player_won { 
        get { return c.formation.get_all_full_slots(Unit.ENEMY).Count <= 0; }
    } 

    private bool enemy_won {
        get { return !player_units_on_field && !units_in_reserve; }
    }

    private bool mini_retreating {
        get { return !player_units_on_field && units_in_reserve; }
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

        // EDIT FOR TESTING---------------------
        can_skip = true;
        //can_skip = units_in_reserve ? false : true; // use 
    }

    private void update_advB_text() {
        Text txt = adv_stageB.GetComponentInChildren<Text>();
        string basestr = "Advance to ";
        if (stage == PLACEMENT) {
            txt.text = basestr + "range stage";
            stageT.text = "Placement";
        }
        else if (stage == RANGE){
            txt.text = basestr + "1st movement stage";
            stageT.text = "Range";
        }
        else if (stage == MOVEMENT1){
            txt.text = basestr + "1st combat stage";
            stageT.text = "Movement 1";
        }
        else if (stage == COMBAT1){
            txt.text = basestr + "2nd movement stage";
            stageT.text = "Combat 1";
        }
        else if (stage == MOVEMENT2){
            txt.text = basestr + "2nd combat stage";
            stageT.text = "Movement 2";
        }
        else if (stage == COMBAT2){
            txt.text = basestr + "assessment stage";
            stageT.text = "Combat 2";
        }
        else if (stage == ASSESSMENT){
            txt.text = basestr + "placement stage";
            stageT.text = "Assessment";
        }
    }
}
