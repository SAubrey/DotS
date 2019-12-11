using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Die : MonoBehaviour {
    public Text d6;
    public Text d8;
    public Text d20;
    private Random rand;
    private IDictionary<int, Text> die = new Dictionary<int, Text>();
    private bool animating = false;
    private int anim_sides = 0;
    private float time = 0;
    private float animation_time_max = .6f;
    private float side_change_time_max = .1f;
    private float side_change_time = 0;

    void Start() {
        die.Add(6, d6);
        die.Add(8, d8);
        die.Add(20, d20);
    }

    void Update() {
        if (animating) {
            time += Time.deltaTime;
            side_change_time += Time.deltaTime;

            if (side_change_time > side_change_time_max) {
                die[anim_sides].text = get_rand_side(anim_sides).ToString();
                side_change_time = 0;
            }
            if (time > animation_time_max) {
                time = 0;
                animating = false;
            }
        }
    }

    public void roll_die(int sides) {
        animate_roll(sides);
        //int roll = get_rand_side(sides);
        //display_roll(roll, sides);
    }

    public int get_rand_side(int sides) {
        return (int)Random.Range(1, sides + 1);
    }

    private void display_roll(int roll, int sides) {
        if (sides == 6) {
            d6.text = roll.ToString();
        } else if (sides == 8) {
            d8.text = roll.ToString();
        } else if (sides == 20) {
            d20.text = roll.ToString();
        }
    }

    private void animate_roll(int sides) {
        anim_sides = sides;
        time = 0;
        animating = true;
    }
}
