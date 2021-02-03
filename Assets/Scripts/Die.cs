using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Die : MonoBehaviour {
    public TextMeshProUGUI read_outT;
    private bool animating = false;
    private int anim_sides = 0;
    private float num_side_changes = 10;
    private int current_num_side_changes = 0;
    private const float MAX_SIDE_CHANGE_TIME = .1f;
    private float current_max_side_change_time = MAX_SIDE_CHANGE_TIME;
    private float side_change_time = 0;
    private int current_side;
    public TravelCardManager tcm;

    // Rolls at a constant period for a random number of sides.
    void Update() {
        if (!animating)
            return;

        side_change_time += Time.deltaTime;

        if (side_change_time > current_max_side_change_time) {
            if (current_num_side_changes < num_side_changes) {
                display_side(get_rand_side(current_side, anim_sides));
            } else {
                finish_roll();
            }

            current_max_side_change_time *= 1.07f; // Deaccelerate.
            side_change_time = 0;
        }
    }

    // Immediately returns the last side, animates, 
    // then lets TCM knows it's done.
    public void roll(int sides) {
        num_side_changes = Random.Range(9, 15);
        animate_roll(sides);
    }

    private int get_rand_side(int current_side, int sides) {
        int roll;
        roll = Random.Range(1, sides + 1);
        if (roll == current_side)
            roll = Random.Range(1, sides + 1);
        return roll;
    }

    private void display_side(int roll) {
        current_side = roll;
        read_outT.text = roll.ToString();
        current_num_side_changes++;
    }

    private void animate_roll(int sides) {
        anim_sides = sides;
        animating = true;
    }

    private void finish_roll() {
        animating = false;
        side_change_time = 0;
        current_num_side_changes = 0;
        current_max_side_change_time = MAX_SIDE_CHANGE_TIME;
        tcm.finish_roll(current_side);
    }
}