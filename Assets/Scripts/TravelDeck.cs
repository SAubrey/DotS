using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TravelDeck : MonoBehaviour {

    public const int ATT1_1 = 1;
    public const int ATT2_1 = 2;
    public const int ATT3_1 = 3;
    public const int ATT4_1 = 4;
    public const int ATT5_1 = 5;
    public const int ATT6_1 = 6;
    public const int ATT7_1 = 7;

    public Sprite attacked1_1;
    public Sprite attacked2_1;
    public Sprite attacked3_1;
    public Sprite attacked4_1;
    public Sprite attacked5_1;
    public Sprite attacked6_1;
    public Sprite attacked7_1;
    public Sprite blessing1_1;

    private Dictionary<int, TravelCard> cards = new Dictionary<int, TravelCard>() {};

    private Dictionary<int, Dictionary<int, int>> card_counters 
        = new Dictionary<int, Dictionary<int, int>>() {};
    private Dictionary<int, int> t1_card_counts = new Dictionary<int, int>() {
        {ATT1_1, 1},
        {ATT2_1, 1},
        {ATT3_1, 1},
        {ATT4_1, 1},
        {ATT5_1, 1},
        {ATT6_1, 1},
        {ATT7_1, 1},
    };
    private Dictionary<int, int> t2_card_counts = new Dictionary<int, int>() {
        {ATT1_1, 1},
        {ATT2_1, 1},
        {ATT3_1, 1},
        {ATT4_1, 1},
        {ATT5_1, 1},
        {ATT6_1, 1},
        {ATT7_1, 1},
    };
    private Dictionary<int, int> t3_card_counts = new Dictionary<int, int>() {
    };

    // List allows fair random choice for draws from decks with more than 1 of a card type. 
    // Elements are removed as they are drawn.
    private Dictionary<int, List<int>> decks = new Dictionary<int, List<int>>();
    private List<int> t1deck = new List<int>();
    private List<int> t2deck = new List<int>();
    private List<int> t3deck = new List<int>();

    public GameObject travel_card_panel;
    public Image tc_img;
    //public Sprite tc_sprite;
    //public SpriteRenderer tc_sprite;
    private bool displaying = true;

    private System.Random rand;

    void Start() {
        rand = new System.Random();
        tc_img = travel_card_panel.GetComponent<Image>();
        card_counters.Add(1, t1_card_counts);
        card_counters.Add(2, t2_card_counts);
        card_counters.Add(3, t3_card_counts);

        decks.Add(1, t1deck);
        decks.Add(2, t2deck);
        decks.Add(3, t3deck);

        cards.Add(ATT1_1, new Att1_1(ATT1_1, attacked1_1));
        cards.Add(ATT2_1, new Att2_1(ATT2_1, attacked2_1));
        cards.Add(ATT3_1, new Att3_1(ATT3_1, attacked3_1));
        cards.Add(ATT4_1, new Att4_1(ATT4_1, attacked4_1));
        cards.Add(ATT5_1, new Att5_1(ATT5_1, attacked5_1));
        cards.Add(ATT6_1, new Att6_1(ATT6_1, attacked6_1));
        cards.Add(ATT7_1, new Att7_1(ATT7_1, attacked7_1));

        toggle_card_panel();
        populate_decks();
    }

    public void display_card(TravelCard tc) {
        if (!displaying) {
            toggle_card_panel();
            if (displaying)
                tc_img.sprite = tc.sprite;
                //tc_sprite.sprite = tc.sprite;
        }
    }

    public void toggle_card_panel() {
        displaying = !displaying;
        travel_card_panel.SetActive(displaying);
    }

    public TravelCard draw_card(int tier) {
        if (decks[tier].Count > 0) {
            // Draw from deck
            int index = rand.Next(0, decks[tier].Count);
            int card_id = decks[tier][index];
            // Remove card
            decks[tier].RemoveAt(index);
            card_counters[tier][card_id]--;
            return cards[card_id];
        }
        return null;
    }

    // Called once.
    private void populate_decks() {
        
        for (int tier = 1; tier <= 3; tier++) { // For each deck
            //for (int j = 0; j <= card_counters[tier].Count; j++) {
            foreach (int card_id in card_counters[tier].Keys) {
                decks[tier].Add(card_id);
            }
        }
    }

}
