using System.Collections.Generic;
using UnityEngine;


public class EnemyBrain : MonoBehaviour {
    private Controller c;
    private Formation f;

    private List<Enemy> enemies {
        get { return c.map.get_enemies_here(); }
    }

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        f = c.formation;
    }

    public void attack(Enemy enemy) {
        Slot target = enemy.target;
        if (!target)
            return;
        // Ignore enemies not in slot 0 of group.
        if (enemy.get_slot().get_group().slots[0] != enemy.get_slot())
            return;
        enemy.get_slot().get_group().rotate_towards_target(target.get_group());
        bool successful = enemy.attempt_set_up_attack(target);
    }

    public void move_units() {
        retarget();
        foreach (Enemy enemy in enemies) {
            // Don't move if within attacking distance.
            if (in_attack_range(enemy.get_slot(), enemy.target))
                continue;

            Slot destination = 
                find_closest_adjacent_move(enemy.get_slot(), enemy.target);
            if (destination) {
                enemy.attempt_move(destination);
            }
        }
    }

    public void stage_attacks() {
        retarget();
        foreach (Enemy enemy in enemies) {
            if (in_attack_range(enemy.get_slot(), enemy.target))
                attack(enemy);
        }
    }

    public void stage_range_attacks() {
        retarget();
        foreach (Enemy enemy in enemies) {
            if (enemy.is_range && 
                    in_attack_range(enemy.get_slot(), enemy.target)) {
                attack(enemy);
            }
        }
    }

    /**
    Targets are maintained unless the target is off the board. 
    */
    public void retarget() {
        foreach (Enemy enemy in enemies) {
            if (enemy.target == null)
                find_nearest_target(enemy);
        }
    }

    private void find_nearest_target(Enemy enemy) {
        List<Slot> punits = f.get_highest_full_slots(Unit.PLAYER);
        Debug.Log("punit count: " + punits.Count);

        Slot nearest_punit_slot = null;
        int nearest_distance = 100;
        foreach (Slot slot in punits) {
            if (!enemy.can_target(slot)) // Control for melee vs flying
                continue;
            
            int distance = calc_distance(enemy.get_slot(), slot);
            if (distance < nearest_distance) {
                nearest_distance = distance;
                nearest_punit_slot = slot;
            }
        }
        //Debug.Log("setting target: " + nearest_punit_slot.get_punit());
        enemy.target = nearest_punit_slot;
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

    private bool in_attack_range(Slot start, Slot end) {
        int range = start.get_enemy().attack_range;
        //Debug.Log("range: " + range + " distance: " + calc_distance(start, end));
        return calc_distance(start, end) <= range ? true : false;
    }

    public static int calc_distance(Slot start, Slot end) {
        int dx = Mathf.Abs(start.col - end.col);
        int dy = Mathf.Abs(start.row - end.row);
        return dx + dy;
    }
}