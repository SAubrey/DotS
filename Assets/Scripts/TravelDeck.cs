using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TravelDeck : MonoBehaviour, ISaveLoad {

    // <TYPE><NUM>_<TIER>
    public const int ATT1_1 = 1;
    public const int ATT2_1 = 2;
    public const int ATT3_1 = 3;
    public const int ATT4_1 = 4;
    public const int ATT5_1 = 5;
    public const int ATT6_1 = 6;
    public const int ATT7_1 = 7;
    public const int CHANCE1_1 = 8;
    public const int CHANCE2_1 = 9;
    public const int CHANCE3_1 = 10;
    public const int BLESSING1_1 = 11;
    public const int CAVE1_1 = 12;
    public const int CAVE2_1 = 13;
    public const int EVENT1_1 = 14;
    public const int EVENT2_1 = 15;
    public const int EVENT3_1 = 16;
    public const int EVENT4_1 = 17;
    public const int EVENT5_1 = 18;
    public const int RUINS1_1 = 19;
    public const int RUINS2_1 = 20;
    public const int RUINS3_1 = 21;
    public const int RUINS4_1 = 22;
    public const int LOCATION1_1 = 23;
    public const int LOCATION2_1 = 24;
    public const int LOCATION3_1 = 25;

    public Sprite attacked1_1, attacked2_1, attacked3_1, attacked4_1, 
            attacked5_1, attacked6_1, attacked7_1;
    public Sprite chance1_1, chance2_1, chance3_1;
    public Sprite blessing1_1;
    public Sprite cave1_1, cave2_1;
    public Sprite event1_1, event2_1, event3_1, event4_1, event5_1;
    public Sprite location1_1, location2_1, location3_1;
    public Sprite ruins1_1, ruins2_1, ruins3_1, ruins4_1;

    // One of each actual card object for reference. Read only. <ID, card>
    private Dictionary<int, TravelCard> cards = new Dictionary<int, TravelCard>() {};

    // <TIER, CARD COUNTS> This is what the deck is generated from.
    private Dictionary<int, Dictionary<int, int>> card_counters 
        = new Dictionary<int, Dictionary<int, int>>();
    private Dictionary<int, int> t1_card_counts = new Dictionary<int, int>() {
        {ATT1_1, 1}, {ATT2_1, 1}, {ATT3_1, 1}, {ATT4_1, 1}, {ATT5_1, 1}, {ATT6_1, 1}, {ATT7_1, 1},
        {CHANCE1_1, 1}, {CHANCE2_1, 1}, {CHANCE3_1, 1},
        {CAVE1_1, 1}, {CAVE2_1, 1},
        {RUINS1_1, 1}, {RUINS2_1, 1}, {RUINS3_1, 1}, {RUINS4_1, 1},
        {LOCATION2_1, 1}, {LOCATION3_1, 1},
        {EVENT1_1, 1}, {EVENT2_1, 1}, {EVENT3_1, 1}, {EVENT4_1, 1}, {EVENT5_1, 1},
    };
    private Dictionary<int, int> t2_card_counts = new Dictionary<int, int>() {
        {ATT1_1, 1}, {ATT2_1, 1}, {ATT3_1, 1}, {ATT4_1, 1}, {ATT5_1, 1}, {ATT6_1, 1}, {ATT7_1, 1},
        {CHANCE1_1, 1}, {CHANCE2_1, 1}, {CHANCE3_1, 1},
        {CAVE1_1, 1}, {CAVE2_1, 1},
        {RUINS1_1, 1}, {RUINS2_1, 1}, {RUINS3_1, 1}, {RUINS4_1, 1},
        {LOCATION2_1, 1}, {LOCATION3_1, 1},
        {EVENT1_1, 1}, {EVENT2_1, 1}, {EVENT3_1, 1}, {EVENT4_1, 1}, {EVENT5_1, 1},
    };
    private Dictionary<int, int> t3_card_counts = new Dictionary<int, int>() {
    };

    // List allows fair random choice for draws from decks with more than 1 of a card type. 
    // This ensures grab-bag probability, or without replacement.
    // Elements are removed as they are drawn. <TIER, List<cards>>
    public Dictionary<int, List<int>> decks = new Dictionary<int, List<int>>();

    // Inclusion dictionary limiting which card types are allowed
    // in which biomes. <MapCell.ID, List<TravelCard.type>>
    // Uses these mappings to aggregate relevant cards to draw from. 
    // For each list entry, join onto a new list the cards of that type.
    private Dictionary<int, List<int>> allowed_cards = new Dictionary<int, List<int>>() {
        {MapCell.PLAINS_ID, new List<int>() },
        {MapCell.FOREST_ID, new List<int>() },
        {MapCell.RUINS_ID, new List<int>() },
        {MapCell.CLIFF_ID, new List<int>() },
        {MapCell.CAVE_ID, new List<int>() },
        {MapCell.STAR_ID, new List<int>() },
        {MapCell.TITRUM_ID, new List<int>() },
        {MapCell.LUSH_LAND_ID, new List<int>() },
        {MapCell.MIRE_ID, new List<int>() },
        {MapCell.MOUNTAIN_ID, new List<int>() },
        {MapCell.SETTLEMENT_ID, new List<int>() },
        {MapCell.RUNE_GATE_ID, new List<int>() },
    };

    public GameObject travel_card_panel;
    public TravelCardManager tcm;
    public Image tc_img;
    private bool displaying = true;
    private System.Random rand;

    void Start() {
        rand = new System.Random();
        tc_img = travel_card_panel.GetComponent<Image>();
        tcm = travel_card_panel.GetComponent<TravelCardManager>();

        card_counters.Add(1, t1_card_counts);
        card_counters.Add(2, t2_card_counts);
        card_counters.Add(3, t3_card_counts);

        decks.Add(1, new List<int>() );
        decks.Add(2, new List<int>() );
        decks.Add(3, new List<int>() );

        cards.Add(ATT1_1, new Att1_1(attacked1_1));
        cards.Add(ATT2_1, new Att2_1(attacked2_1));
        cards.Add(ATT3_1, new Att3_1(attacked3_1));
        cards.Add(ATT4_1, new Att4_1(attacked4_1));
        cards.Add(ATT5_1, new Att5_1(attacked5_1));
        cards.Add(ATT6_1, new Att6_1(attacked6_1));
        cards.Add(ATT7_1, new Att7_1(attacked7_1));
        cards.Add(CHANCE1_1, new Chance1_1(chance1_1));
        cards.Add(CHANCE2_1, new Chance2_1(chance2_1));
        cards.Add(CHANCE3_1, new Chance3_1(chance3_1));
        cards.Add(CAVE1_1, new Cave1_1(cave1_1));
        cards.Add(CAVE2_1, new Cave2_1(cave2_1));
        cards.Add(RUINS1_1, new Ruins1_1(ruins1_1));
        cards.Add(RUINS2_1, new Ruins2_1(ruins2_1));
        cards.Add(RUINS3_1, new Ruins3_1(ruins3_1));
        cards.Add(RUINS4_1, new Ruins4_1(ruins4_1));
        cards.Add(LOCATION1_1, new Location1_1(location1_1));
        cards.Add(LOCATION2_1, new Location2_1(location2_1));
        cards.Add(LOCATION3_1, new Location3_1(location3_1));
        cards.Add(EVENT1_1, new Event1_1(event1_1));
        cards.Add(EVENT2_1, new Event2_1(event2_1));
        cards.Add(EVENT3_1, new Event3_1(event3_1));
        cards.Add(EVENT4_1, new Event4_1(event4_1));
        cards.Add(EVENT5_1, new Event5_1(event5_1));

        allowed_cards[MapCell.PLAINS_ID].AddRange(new int[] {
            TravelCard.COMBAT, TravelCard.BLESSING, TravelCard.CHANCE, 
            TravelCard.EVENT, TravelCard.LOCATION} );
        allowed_cards[MapCell.FOREST_ID].AddRange(new int[] {
            TravelCard.COMBAT, TravelCard.BLESSING, TravelCard.CHANCE, 
            TravelCard.EVENT, TravelCard.LOCATION} );
        allowed_cards[MapCell.CLIFF_ID].AddRange(new int[] {
            TravelCard.COMBAT, TravelCard.BLESSING, TravelCard.CHANCE, 
            TravelCard.EVENT, TravelCard.LOCATION} );
        allowed_cards[MapCell.MOUNTAIN_ID].AddRange(new int[] {
            TravelCard.COMBAT, TravelCard.BLESSING, TravelCard.CHANCE, 
            TravelCard.EVENT, TravelCard.LOCATION} );

        allowed_cards[MapCell.TITRUM_ID].AddRange(new int[] {
            TravelCard.COMBAT, TravelCard.CHANCE, 
            TravelCard.EVENT, TravelCard.LOCATION} );
        allowed_cards[MapCell.MIRE_ID].AddRange(new int[] {
             TravelCard.BLESSING, TravelCard.EVENT } );
        allowed_cards[MapCell.RUNE_GATE_ID].Add(TravelCard.LOCATION);
        allowed_cards[MapCell.CAVE_ID].Add(TravelCard.CAVE);
        allowed_cards[MapCell.RUINS_ID].Add(TravelCard.RUINS);
        // no cards for star, lush. Settlement = quest card?

        toggle_card_panel();
    }

    public void init(bool from_save) {
        if (!from_save) 
            new_game();
    }

        // Called once.
    private void populate_decks() {
        for (int tier = 1; tier <= 3; tier++) { // For each deck
            foreach (int card_id in card_counters[tier].Keys) {
                decks[tier].Add(card_id);
            }
        }
    }

    public GameData save() {
        TravelDeckData data = new TravelDeckData(this, Controller.TRAVEL_DECK);
        return data;
    }

    private void new_game() {
        clear_data();
        populate_decks();
    }

    private void clear_data() {
        foreach (List<int> deck in decks.Values) {
            deck.Clear();
        }
    }

    public void load(GameData generic) {
        TravelDeckData td = generic as TravelDeckData;
        clear_data();

        foreach (int card_type in td.t1_deck) 
            decks[1].Add(card_type);
        foreach (int card_type in td.t2_deck) 
            decks[2].Add(card_type);
        foreach (int card_type in td.t3_deck) 
            decks[3].Add(card_type);
    }

    public Button combat_cards_onlyB; // DEV ONLY
    public bool combat_cards_only { get; set; } = false;

    // Cards are drawn without replacement. Cards that are allowed
    // in the map cell biome are pulled from the deck then chosen from randomly.
    public TravelCard draw_card(int tier, int biome_ID) {
        if (biome_ID == MapCell.LUSH_LAND_ID || biome_ID == MapCell.STAR_ID) {
            // don't draw a card.
            return null;
        } else if (biome_ID == MapCell.RUNE_GATE_ID) {
            return cards[LOCATION1_1];
        } else if (combat_cards_only) {
            return cards[ATT1_1];
        }

        if (decks[tier].Count > 0) {
            // Draw from deck
            List<int> drawable_cards = aggregate_drawable_cards(tier, biome_ID);
            if (drawable_cards.Count <= 0) {
                // out of cards for this biome (unlikely but possible)
                // just draw any applicable one.
                return get_random_matching_card(biome_ID);
            }
            int index = rand.Next(0, drawable_cards.Count);
            int card_id = drawable_cards[index];

            // -- check if the card can be played in the biome of the tile.
            // could organize actual cards by class but draw from them in a single list. 

            // Remove card
            decks[tier].Remove(card_id);
            return cards[card_id];
        }
        return null;
    }

    // Returns cards that can be drawn at the current tile biome.
    private List<int> aggregate_drawable_cards(int tier, int biome_ID) {
        List<int> valid_card_ids = new List<int>();
        foreach (int card_id in decks[tier]) {
            if (check_if_card_in_biome(biome_ID, card_id)) {
                valid_card_ids.Add(card_id);
            }
        }
        return valid_card_ids;
    }

    private bool check_if_card_in_biome(int biome_ID, int card_id) {
        if (!allowed_cards.ContainsKey(biome_ID))
            return false;
        return allowed_cards[biome_ID].Contains(cards[card_id].type);
    }

    private TravelCard get_random_matching_card(int biome_ID) {
        foreach (int card_id in cards.Keys) {
            if (check_if_card_in_biome(biome_ID, card_id)) {
                return cards[card_id];
            }
        }
        return cards[0];
    }

    public void display_card(TravelCard tc) {
        if (tc == null)
            return;
        if (!displaying) {
            toggle_card_panel();
            if (displaying) {
                tc_img.sprite = tc.sprite;
                tc.action(tcm);
            }
        }
    }

    public void toggle_card_panel() {
        displaying = !displaying;
        travel_card_panel.SetActive(displaying);
    }
    
    private void remove_card(int tier, int card_id) {
        decks[tier].Remove(card_id);
    }
}
