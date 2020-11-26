using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
This class operates in solo and group battles,
and will  dynamically adapt to additional participants.

A battle exists immediately before a solo battle or while a group battle is pending,
and for the duration of the battle until retreat/defeat/victory.
*/
public class Battle {
    public bool active = false;
    public bool group_pending = true;
    public int init_turn = -1;

    // Listed in order of join.
    public List<Discipline> participants = new List<Discipline>(3);
    // For saving a board between turns.
    public Dictionary<Location, Unit> board = 
        new Dictionary<Location, Unit>();
    
    public Discipline leader;
    private Map map; 
    public MapCell cell { get; private set; }
    public int active_bat_ID = -1;
    private int _turn = 1;
    public int turn {
        get => _turn;
        private set {
            _turn = value;
            /* The active battalion ID is indexed with the remainder of the turn number
            divided by the number of participants. */
            active_bat_ID = participants[(_turn - 1) % participants.Count].ID;
        }
    }

    public Battle(Map map, MapCell cell, Discipline leader, bool grouping) {
        this.map = map;
        this.cell = cell;
        this.leader = leader;
        init_turn = TurnPhaser.I.turn;
        add_participant(leader);
        active_bat_ID = leader.ID;
        leader.bat.pending_group_battle_cell = cell;
        group_pending = grouping;
    }

    public void add_participant(Discipline participant) {
        participants.Add(participant);
        if (group_pending)
            participant.bat.pending_group_battle_cell = cell;
    }

    public void remove_participant(Discipline participant) {
        participants.Remove(participant);
        participant.bat.pending_group_battle_cell = null;
    }

    /*
    A group battle begins from MapCellUI. A solo battle begins from travel card UI.
    */
    public void begin() {
        active = true;
        group_pending = false;
        foreach (Discipline d in participants) {
            d.bat.in_battle = true;
            if (is_group)
                d.bat.pending_group_battle_cell = null;
        }
        // Move player triggers turn phase advancement, triggering 
        if (is_group) {
            move_players();
            BattlePhaser.I.begin_new_battle(cell);
        }
        // Do not move single battalion in a solo battle since their battle
        // is generated as the result of movement.
    }

    public void advance_turn() {
        //active_bat_ID++;
        turn++;
    }
    
    private void move_players() {
        foreach (Discipline d in participants) {
            d.move(cell);
        }
    }

    public void retreat() {
        Debug.Log("attempting retreat. Participants: " + participants.Count);
        cell.post_phase();
        //cell.map.c.line_drawer.clear();
        AttackQueuer.I.get_enemy_queue().reset();
        AttackQueuer.I.get_player_queue().reset();
        
        foreach (Discipline d in participants) {
            d.move(d.previous_cell);
            // Penalize
            d.change_var(Storeable.UNITY, -1, true);
            d.bat.post_phase();
        }
        cell.set_tile_color();
        end();
    }

    public void end() {
        foreach (Discipline d in participants) {
            d.bat.in_battle = false;
        }
        group_pending = false;
        cell.clear_battle();
    }

    public void post_phase() {
        foreach (Discipline d in participants) {
            d.bat.post_phase();
        }
        cell.post_phase();
    }

    public void post_battle() {
        foreach (Discipline d in participants) {
            d.bat.post_battle();
        }
        if (check_all_dead()) {
            end();
        }
        cell.post_battle();
    }

    private bool check_all_dead() {
        foreach (Discipline d in participants) {
            if (!d.dead)
                return false;
        }
        return true;
    }

    public List<Battalion> get_dead_battalions() {
        List<Battalion> dead = new List<Battalion>();
        foreach (Discipline d in participants) {
            if (d.bat.count_healthy() <= 0) {
                dead.Add(d.bat);
            }
        }
        return dead;
    }

    public void clear_stage_actions() {
        foreach (Discipline d in participants) {
            d.bat.reset_all_stage_actions();
        }
    }

    public int count_all_placed_units() {
        int sum = 0;
        foreach(Discipline d in participants) {
            sum += d.bat.get_all_placed_units().Count;
        }
        return sum;
    }

    public int count_all_units_in_reserve() {
        int sum = 0;
        foreach(Discipline d in participants)
            sum += d.bat.count_placeable();
        return sum;
    }

    public bool is_group { get => participants.Count > 1; }

    public bool can_begin_group {
        get { return TurnPhaser.I.turn > init_turn && participants.Count > 1; } 
    }

    public bool leader_is_active_on_map {
        get { return Controller.I.get_disc() == leader; }
    }

    public bool leaders_turn {
        get => active_bat_ID == leader.ID;
    }
    
    public bool last_battalions_turn { get =>
        (turn - 1) % participants.Count == participants.Count - 1;
    }

    public bool includes_disc(Discipline d) {
        return participants.Contains(d);
    }
}
