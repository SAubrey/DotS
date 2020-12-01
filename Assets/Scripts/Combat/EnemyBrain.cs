using System.Collections.Generic;
using UnityEngine;


public class EnemyBrain : MonoBehaviour {
    public static EnemyBrain I { get; private set; }

    private List<Enemy> enemies {
        get { return Map.I.get_enemies_here(); }
    }
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
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
        if (successful) {
            target.update_healthbar();
        }
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
                enemy.has_acted_in_stage = false;
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
        List<Slot> punits = Formation.I.get_highest_full_slots(Unit.PLAYER);

        Slot nearest_punit_slot = null;
        int nearest_distance = 100;
        foreach (Slot slot in punits) {
            if (!enemy.can_target(slot)) // Control for melee vs flying
                continue;
            
            int distance = Statics.calc_distance(enemy.get_slot(), slot);
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
        // Clamp movement to one tile.
        if (dx != 0) dx = dx < 0 ? -1 : 1;
        if (dy != 0) dy = dy < 0 ? -1 : 1;

        Slot sx = null, sy = null;
        if (dx != 0)
            sx = get_slot(start, dx, 0);
        if (dy != 0)
            sy = get_slot(start, 0, dy);

        if (sx != null && sy != null) {
            // Both valid slots are productive, make arbitrary choice.
            int flip = Random.Range(0, 2);
            return flip == 0 ? sx : sy;
        } else if (sx == null && sy != null) {
            return sy;
        } else if (sy == null && sx != null) {
            return sx;
        }
        return null;
    }

    private Slot get_slot(Slot start, int col=0, int row=0) {
        Group dest = Formation.I.get_group(start.col + col, start.row + row);
        if (dest == null) return null;
        if (!dest.is_empty) return null;
        return dest.get_highest_empty_slot();
    }

    private bool check_adjacent(Slot start, Slot end) {
        return Statics.calc_distance(start, end) == 1 ? true : false;
    }

    private bool in_attack_range(Slot start, Slot end) {
        int range = start.get_enemy().attack_range;
        //Debug.Log("range: " + range + " distance: " + calc_distance(start, end));
        return Statics.calc_distance(start, end) <= range ? true : false;
    }
}