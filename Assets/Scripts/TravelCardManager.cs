using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TravelCardManager : MonoBehaviour {
    public GameObject tc_panel;
    public Die die;
    public Button continueB;
    public Button rollB;
    private int num_sides;
    private TravelCard tc;
    public Controller c;

    public Dictionary<string, TravelCard> discipline_cards =
         new Dictionary<string, TravelCard>(); 

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        set_continueB(true);
        set_rollB(false);
    }

    public void set_card(string disc_name, TravelCard tc) {
        if (!discipline_cards.ContainsKey(disc_name)) {
            discipline_cards.Add(disc_name, tc);
        } else {
            discipline_cards[disc_name] = tc;
        }
    }

    // Called by button only.
    public void roll() {
        die.roll(num_sides);
    }

    public void finish_roll(int result) {
        set_continueB(true);
        set_rollB(false);
        tc.use_roll_result(result);
    }

    public void set_up_roll(TravelCard tc, int num_sides) {
        this.tc = tc;
        this.num_sides = num_sides;
        set_continueB(false);
        set_rollB(true);

    }

    private void set_rollB(bool state) {
        rollB.interactable = state;
        Text t = rollB.GetComponentInChildren<Text>();
        if (state) {
            t.text = "Roll";
        } else {
            t.text = "";
        }
    }
    
    private void set_continueB(bool state) {
        continueB.interactable = state;
    }
}
