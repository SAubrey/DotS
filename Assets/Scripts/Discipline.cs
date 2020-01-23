using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discipline : Storeable {
    public GameObject piece;
    public Battalion bat;
    public TravelCard travel_card;
    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        bat = new Battalion();
        bat.c = c;
        map_ui = c.map_ui;
        city_ui = c.city_ui;

        _light = 4;
        _unity = 10;
        pos = new Vector3(10.5f, 10.5f);
    }

    public override void register_turn() {
        base.register_turn();
        check_insanity();
    }

    private void check_insanity() {
        // 5 == "wavering"
        // 4 == unable to build
        if (unity == 3) {
            roll_for_insanity(1, 20);
        } else if (unity == 2) {
            roll_for_insanity(2, 50);
        } else if (unity < 2) {
            roll_for_insanity(2, 80);
        }
    }

    private void roll_for_insanity(int quantity, int chance) {
        int roll = Random.Range(1, 100);
        if (roll <= chance) {
            // Units flee into the darkness.
            for (int i = 0; i < quantity; i++) {
                bat.lose_random_unit();
            }
        }
    }

    public Pos get_Pos() {
        return new Pos((int)pos.x, (int)pos.y);
    }
    
    private Vector3 _pos;// = new Vector3(0f, 0f, 0);
    public Vector3 pos {
        get { return _pos; }
        set {
            prev_pos = _pos;
            _pos = value;
            piece.transform.position = new Vector3(value.x, value.y, 0);
        }
    }
    public Vector3 prev_pos;
}
