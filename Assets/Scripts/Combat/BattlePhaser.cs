using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls the game logic of the battle phases
public class BattlePhaser : MonoBehaviour {
    public const int PLACEMENT = 0;
    public const int INTUITIVE = 1;
    public const int RANGE = 2;
    public const int ACTION1 = 3;
    public const int MOVEMENT = 4;
    public const int ACTION2 = 5;
    public const int ASSESSMENT = 6;

    private Controller c;
    private TurnPhaser tp;
    private Selector selector;
    private AttackQueuer aq;
    private EnemyBrain enemy_brain;

    public Button adv_stageB;
    public Text stageT; 

    public bool placement_stage = true;
    public bool intuitive_stage = false;
    public bool range_stage = false;
    public bool action1_stage = false;
    public bool movement_stage = false;
    public bool action2_stage = false;
    public bool assessment_stage = false;
    public bool targeting = false;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        tp = c.turn_phaser;
        selector = c.selector;
        aq = c.attack_queuer;
        enemy_brain = c.enemy_brain;

        reset();
    }

    public void reset() {
        stage = PLACEMENT - 1;
        can_skip = false;

        placement_stage = false;
        intuitive_stage = false;
        range_stage = false;
        action1_stage = false;
        movement_stage = false;
        action2_stage = false;
        assessment_stage = false;
        targeting = false;
        advance_stage();
    }

    public void advance_stage() {
        selector.deselect();
        stage = _stage + 1;
        update_advB_text();
    }

    // ---STAGES---
    private void placement() {
        if (_stage == PLACEMENT) // Require unit placement in 1st phase.
            can_skip = false;
        placement_stage = true;
    }

    private void intuitive() {
        placement_stage = false;
        can_skip = true;
        selector.deselect();
        c.get_active_bat().clear_selected_unit_type();
        //advance_stage();
        // Scan through placed characters and the enemy to determine initial effects.
    }

    private void range() {
        intuitive_stage = false;
        range_stage = true;
        can_skip = true;
        targeting = true;
        // Ranged units attack (enemy and player after player chooses who to attack)
        // (unless someone has first strike!)
        // range units can shoot anywhere, riflemen can only shoot cardinally

        enemy_brain.stage_range_attacks();
    }

    private void action1() {
        range_stage = false;
        battle(); // range attacks

        action1_stage = true;
        targeting = true;
    // pikemen special ability can hit diagonally
    }
    private void post_action1() {
        enemy_brain.stage_attacks();
    }

    private void movement() {
        action1_stage = false;
        targeting = false;
        battle(); // will clear attacks

        movement_stage = true;
    }

    private void action2() {
        movement_stage = false;
        enemy_brain.move_units();

        action2_stage = true;
        targeting = true;
        enemy_brain.stage_attacks();
    }

    private void assessment() {
        battle();
        action2_stage = false;
        targeting = false;
    }

    private void post_assessment() {
        c.get_active_bat().reset_all_actions(); // reset movement/attacks
    }

    private void battle() {
        can_skip = false;
        StartCoroutine(aq.battle());
    }

    // Only called by AttackQueuer after battle animations have finished.
    public void post_battle() {
        if (stage == ACTION1) post_action1();
       // else if (stage == MOVEMENT) post_movement();
        else if (stage == ASSESSMENT) post_assessment();
        can_skip = true;
        check_end_conditions();
    }

    /*
    Outwardly, the stage number loops 0 - num_stages once for each of a turn's 3 battle phases.
    Inwardly, it increments to 3x the num_stages to know when to end the phase.
    */
    int num_stages = 7;
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
            else if (phase_stage == ACTION1) action1();
            else if (phase_stage == MOVEMENT) movement();
            else if (phase_stage == ACTION2) action2();
            else if (phase_stage == ASSESSMENT) assessment();

            c.get_active_bat().reset_all_stage_actions();
            // After 3 phases, battle yields until the next turn.
            //if (_stage > 2) {
            if (_stage > num_stages * 3) {
                if (!check_end_conditions()) {
                    c.formation.save_board(c.get_player());
                    reset();
                    tp.advance_stage();
                }
            }
        }
    }

    private bool check_end_conditions() {
        if (check_player_won()) {
            c.get_player_obj().change_var(Storeable.EXPERIENCE, 1);
        } else if (check_enemy_won()) {
            Debug.Log("Game over for " + c.get_player()); 
        } else {
            return false;
        }
        // If someone won and battle is ending...
        c.get_active_bat().in_battle = false;
        reset();
        tp.advance_stage();
        return true;
    }

    private bool check_player_won() {
        List<Slot> enemies = c.formation.get_all_full_slots(Unit.ENEMY);
        return enemies.Count <= 0 ? true : false;
    }

    private bool check_enemy_won() {
        List<Slot> punits = c.formation.get_all_full_slots(Unit.PLAYER);
        return punits.Count <= 0 ? true : false;
    }

    private bool _can_skip = false;
    public bool can_skip {
        get { return _can_skip; }
        set { 
            _can_skip = value;
            adv_stageB.enabled = _can_skip;
            }
    }

    private void update_advB_text() {
        Text txt = adv_stageB.GetComponentInChildren<Text>();
        string basestr = "Advance to ";
        if (stage == PLACEMENT) {
            txt.text = basestr + "intuitive stage";
            stageT.text = "Placement";
        }
        else if (stage == INTUITIVE) {
            txt.text = basestr + "range stage";
            stageT.text = "Intuitive";
        }
        else if (stage == RANGE){
            txt.text = basestr + "1st action stage";
            stageT.text = "Range";
        }
        else if (stage == ACTION1){
            txt.text = basestr + "movement stage";
            stageT.text = "Action 1";
        }
        else if (stage == MOVEMENT){
            txt.text = basestr + "2nd action stage";
            stageT.text = "Movement";
        }
        else if (stage == ACTION2){
            txt.text = basestr + "assessment stage";
            stageT.text = "Action 2";
        }
        else if (stage == ASSESSMENT){
            txt.text = basestr + "placement stage";
            stageT.text = "Assessment";
        }
    }
}
