using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Die : MonoBehaviour {
    public Text read_outT;
    private Random rand;
    private bool animating = false;
    private int anim_sides = 0;
    private float num_side_changes = 10;
    private int current_num_side_changes = 0;
    private const float MAX_SIDE_CHANGE_TIME = .1f;
    private float current_max_side_change_time = MAX_SIDE_CHANGE_TIME;
    private float side_change_time = 0;
    private int last_side;
    public TravelCardManager tcm;

    void Start() {
    }

    // Rolls at a constant period for a random number of sides.
    void Update() {
        if (animating) {
            side_change_time += Time.deltaTime;

            if (side_change_time > current_max_side_change_time) {
                if (current_num_side_changes < num_side_changes - 1) {
                    display_side(get_rand_side(anim_sides));
                } else {
                    display_side(last_side);
                    finish_roll();
                }

                current_max_side_change_time *= 1.05f; // Deaccelerate.
                side_change_time = 0;
            }
        }
    }

    // Immediately returns the last side, animates, 
    // then lets TCM knows it's done.
    public int roll(int sides) {
        num_side_changes = Random.Range(5, 15);
        last_side = get_rand_side(sides);
        animate_roll(sides);
        return last_side;
    }

    private int get_rand_side(int sides) {
        return (int)Random.Range(1, sides + 1);
    }

    private void display_side(int roll) {
        read_outT.text = roll.ToString();
        current_num_side_changes++;
    }

    private void animate_roll(int sides) {
        anim_sides = sides;
        animating = true;
    }

    private void finish_roll() {
        animating = false;
        current_num_side_changes = 0;
        current_max_side_change_time = MAX_SIDE_CHANGE_TIME;
        tcm.finish_roll(last_side);

    }
}