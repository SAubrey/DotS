using System.Collections.Generic;
using UnityEngine;


public class EnemyBrain : MonoBehaviour{
    private Controller c;
    private Formation f;
    private AttackQueuer aq;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        f = c.formation;
        aq = c.attack_queuer;
    }

    public void attack(Slot slot) {
        Slot target = slot.get_enemy().get_target();
        if (target == null)
            return;
        slot.get_group().rotate_towards_target(target.get_group());
        aq.attempt_attack(slot, target);
    }

    public void move_units() {
        retarget();
        List<Slot> enemies = f.get_highest_full_slots(Unit.ENEMY);
        foreach (Slot slot in enemies) {
            if (in_range(slot, slot.get_enemy().get_target()))
                continue;

            Slot destination = find_closest_adjacent_move(slot, slot.get_enemy().get_target());
            if (destination != null) {
                slot.get_enemy().attempt_move(destination);
            }
        }
    }

    public void stage_attacks() {
        retarget();
        List<Slot> enemies = f.get_highest_full_slots(Unit.ENEMY);
        foreach (Slot slot in enemies) {
            if (in_range(slot, slot.get_enemy().get_target()))
                attack(slot);
        }
    }

    public void stage_range_attacks() {
        retarget();
        List<Slot> enemies = f.get_highest_full_slots(Unit.ENEMY);
        foreach (Slot slot in enemies) {
            if (slot.get_enemy().is_range() && in_range(slot, slot.get_enemy().get_target())) {
                attack(slot);
            }
        }
    }

    /**
    Targets are maintained unless the target is off the board. 
    */
    public void retarget() {
        // control for melee vs flying, 
        List<Slot> enemies = f.get_highest_full_slots(Unit.ENEMY);
        foreach (Slot slot in enemies) {
            Slot target = slot.get_enemy().get_target();
            if (target != null) {
                if (target.has_punit)
                    continue;
            } else 
                find_nearest_target(slot);
        }
    }

    private void find_nearest_target(Slot enemy_slot) {
        List<Slot> punits = f.get_highest_full_slots(Unit.PLAYER);
        Slot nearest_punit_slot = null;
        int nearest_distance = 100;
        foreach (Slot slot in punits) {
            if (!enemy_slot.get_enemy().can_target(slot)) continue;
            
            int distance = calc_distance(enemy_slot, slot);
            if (distance < nearest_distance) {
                nearest_distance = distance;
                nearest_punit_slot = slot;
            }
        }
        enemy_slot.get_enemy().set_target(nearest_punit_slot);
    }

    public Slot find_closest_adjacent_move(Slot start, Slot end) {
        int dx = end.col - start.col;
        int dy = end.row - start.row;
        if (dx != 0) dx = dx < 0 ? -1 : 1;
        if (dy != 0) dy = dy < 0 ? -1 : 1;

        // Does not account for being able to move into groups owned by enemies.
        Group xmove = f.get_group(start.col + dx, start.row);
        //Debug.Log("xmove: " + (start.col + dx) + "y: " + (start.row) + " - " + xmove);
        if (xmove != null) {
            if (xmove.is_empty)
                return xmove.get_highest_empty_slot();
        } 
        Group ymove = f.get_group(start.col, start.row + dy);
        //Debug.Log("ymove: " + (start.col) + "y: " + (start.row + dy) + " - " + ymove);
        if (ymove != null) {
            if (ymove.is_empty)
                return ymove.get_highest_empty_slot();
        }
        return null;
    }

    private bool check_adjacent(Slot start, Slot end) {
        return calc_distance(start, end) == 1 ? true : false;
    }

    private bool in_range(Slot start, Slot end) {
        int range = start.get_enemy().attack_range;
        //Debug.Log("range: " + range + " distance: " + calc_distance(start, end));
        return calc_distance(start, end) <= range ? true : false;
    }

    public static int calc_distance(Slot start, Slot end) {
        int dx = Mathf.Abs(start.col - end.col);
        int dy = Mathf.Abs(start.row - end.row);
        return dx + dy;
    }

    public void post_phase() {
        List<Slot> enemies = f.get_all_full_slots(Unit.ENEMY);
        foreach (Slot s in enemies) {
            s.get_unit().post_phase();
        }
    }

    public void clear_dead_enemies() {
        List<Slot> enemy_slots = f.get_all_full_slots(Unit.ENEMY);
        foreach (Slot s in enemy_slots) {
            if (s.get_enemy().is_dead()) {
                s.empty(false);
            }
        }
        validate_all_enemies();
    }

    private void validate_all_enemies() {
        List<Group> enemy_groups = f.get_all_nonempty_groups(Unit.ENEMY);
        foreach (Group g in enemy_groups) {
            g.validate_unit_order();
        }
    }
}