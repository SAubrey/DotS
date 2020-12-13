using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TravelDeck : MonoBehaviour, ISaveLoad {
    public static TravelDeck I { get; private set; }

    // <TYPE><NUM>_<TIER>
    public const int ATT1_1 = 1, ATT2_1 = 2, ATT3_1 = 3, ATT4_1 = 4, ATT5_1 = 5, ATT6_1 = 6;
    public const int CHANCE1_1 = 8, CHANCE2_1 = 9, CHANCE3_1 = 10;
    public const int BLESSING1_1 = 11;
    public const int CAVE1_1 = 12, CAVE2_1 = 13;
    public const int EVENT1_1 = 14, EVENT2_1 = 15, EVENT3_1 = 16, EVENT4_1 = 17, EVENT5_1 = 18;
    public const int EVENT1_2 = 19, EVENT2_2 = 20, EVENT3_2 = 21, 
        EVENT4_2 = 22, EVENT5_2 = 23, EVENT6_2 = 24;
    public const int RUINS1_1 = 25, RUINS2_1 = 26, RUINS3_1 = 27, RUINS4_1 = 28;
    public const int LOCATION1_1 = 29, LOCATION2_1 = 30, LOCATION3_1 = 31;
    public const int LOCATION1_2 = 32;

    public Sprite attacked1_1, attacked2_1, attacked3_1, attacked4_1, 
            attacked5_1, attacked6_1, attacked7_1;
    public Sprite chance1_1, chance2_1, chance3_1;
    public Sprite blessing1_1;
    public Sprite cave1_1, cave2_1;
    public Sprite event1_1, event2_1, event3_1, event4_1, event5_1;
    public Sprite location1_1, location2_1, location3_1;
    public Sprite ruins1_1, ruins2_1, ruins3_1, ruins4_1;

    // One of each actual card object for reference. <ID, card>
    //private readonly Dictionary<int, TravelCard> cards = new Dictionary<int, TravelCard>() {};

    // <TIER, CARD COUNTS> This is what the deck is generated from.
    private Dictionary<int, Dictionary<int, int>> card_counters 
        = new Dictionary<int, Dictionary<int, int>>();
    private Dictionary<int, int> t1_card_counts = new Dictionary<int, int>() {
        {ATT1_1, 1}, {ATT2_1, 1}, {ATT3_1, 1}, {ATT4_1, 1}, {ATT5_1, 1}, {ATT6_1, 1},
        {CHANCE1_1, 1}, {CHANCE2_1, 1}, {CHANCE3_1, 1},
        {CAVE1_1, 1}, {CAVE2_1, 1},
        {RUINS1_1, 1}, {RUINS2_1, 1}, {RUINS3_1, 1}, {RUINS4_1, 0},
        {LOCATION2_1, 0}, {LOCATION3_1, 1},
        {EVENT1_1, 1}, {EVENT2_1, 1}, {EVENT3_1, 1}, {EVENT4_1, 1}, {EVENT5_1, 1},
    };
    private Dictionary<int, int> t2_card_counts = new Dictionary<int, int>() {
        {ATT1_1, 1}, {ATT2_1, 1}, {ATT3_1, 1}, {ATT4_1, 1}, {ATT5_1, 1}, {ATT6_1, 1},
        {CHANCE1_1, 1}, {CHANCE2_1, 1}, {CHANCE3_1, 1},
        {CAVE1_1, 1}, {CAVE2_1, 1},
        {RUINS1_1, 1}, {RUINS2_1, 1}, {RUINS3_1, 1}, {RUINS4_1, 0},
        {LOCATION2_1, 0}, {LOCATION3_1, 1},
        {EVENT1_1, 1}, {EVENT2_1, 1}, {EVENT3_1, 1}, {EVENT4_1, 1}, {EVENT5_1, 1},
    };
    private Dictionary<int, int> t3_card_counts = new Dictionary<int, int>() {
        {ATT1_1, 1}, {ATT2_1, 1}, {ATT3_1, 1}, {ATT4_1, 1}, {ATT5_1, 1}, {ATT6_1, 1},
        {CHANCE1_1, 1}, {CHANCE2_1, 1}, {CHANCE3_1, 1},
        {CAVE1_1, 1}, {CAVE2_1, 1},
        {RUINS1_1, 1}, {RUINS2_1, 1}, {RUINS3_1, 1}, {RUINS4_1, 0},
        {LOCATION2_1, 0}, {LOCATION3_1, 1},
        {EVENT1_1, 1}, {EVENT2_1, 1}, {EVENT3_1, 1}, {EVENT4_1, 1}, {EVENT5_1, 1},
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
        //{MapCell.MIRE_ID, new List<int>() },
        {MapCell.MOUNTAIN_ID, new List<int>() },
        {MapCell.SETTLEMENT_ID, new List<int>() },
        {MapCell.RUNE_GATE_ID, new List<int>() },
    };

    private System.Random rand;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        rand = new System.Random();

        card_counters.Add(1, t1_card_counts);
        card_counters.Add(2, t2_card_counts);
        card_counters.Add(3, t3_card_counts);

        decks.Add(1, new List<int>() );
        decks.Add(2, new List<int>() );
        decks.Add(3, new List<int>() );
/*
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
        cards.Add(EVENT5_1, new Event5_1(event5_1));*/

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
        //allowed_cards[MapCell.MIRE_ID].AddRange(new int[] {
             //TravelCard.BLESSING, TravelCard.EVENT } );
        allowed_cards[MapCell.RUNE_GATE_ID].Add(TravelCard.LOCATION);
        allowed_cards[MapCell.CAVE_ID].Add(TravelCard.CAVE);
        allowed_cards[MapCell.RUINS_ID].Add(TravelCard.RUINS);
        // no cards for star, lush. Settlement = quest card?
    }

    public void init(bool from_save) {
        if (!from_save) 
            new_game();
    }

    private void new_game() {
        MapUI.I.close_travelcardP();
        clear_data();
        populate_decks();
    }

    // Called once.
    private void populate_decks() {
        for (int tier = 1; tier <= 3; tier++) { // For each deck
            foreach (int card_id in card_counters[tier].Keys) {
                decks[tier].Add(card_id);
            }
        }
    }

    private void clear_data() {
        foreach (List<int> deck in decks.Values) {
            deck.Clear();
        }
    }


    public GameData save() {
        return new TravelDeckData(this, Controller.TRAVEL_DECK);
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
        // Negate or bypass random draw.
        if (biome_ID == MapCell.LUSH_LAND_ID || biome_ID == MapCell.STAR_ID) {
            return null;
        } else if (biome_ID == MapCell.RUNE_GATE_ID) {
            return make_card(LOCATION1_1);
        } else if (combat_cards_only) {
            return make_card(ATT1_1); // debug only
        }

        if (decks[tier].Count <= 0)
            return null;
            
        // Draw from deck
        List<int> drawable_cards = aggregate_drawable_cards(tier, biome_ID);
        if (drawable_cards.Count <= 0) {
            // Out of cards for this biome (unlikely but possible).
            // Just draw any applicable one.
            return draw_random_card(biome_ID);
        }

        // Draw
        int index = rand.Next(0, drawable_cards.Count);
        int card_id = drawable_cards[index];

        // Remove drawn card
        decks[tier].Remove(card_id);
        return make_card(card_id);
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
        return allowed_cards[biome_ID].Contains(make_card(card_id).type);
    }

    private TravelCard draw_random_card(int biome_ID) {
        return make_card(ATT1_1);
        /*foreach (int card_id in cards.Keys) {
            if (check_if_card_in_biome(biome_ID, card_id)) {
                return make_card(card_id);
            }
        }
        return make_card(0);*/
    }

    public TravelCard make_card(int ID) {
        if (ID == ATT1_1) return new Att1_1(attacked1_1);
        if (ID == ATT2_1) return new Att2_1(attacked2_1);
        if (ID == ATT3_1) return new Att3_1(attacked3_1);
        if (ID == ATT4_1) return new Att4_1(attacked4_1);
        if (ID == ATT5_1) return new Att5_1(attacked5_1);
        if (ID == ATT6_1) return new Att6_1(attacked6_1);
        if (ID == CHANCE1_1) return new Chance1_1(chance1_1);
        if (ID == CHANCE2_1) return new Chance2_1(chance2_1);
        if (ID == CHANCE3_1) return new Chance3_1(chance3_1);
        if (ID == CAVE1_1) return new Cave1_1(cave1_1);
        if (ID == CAVE2_1) return new Cave2_1(cave2_1);
        if (ID == RUINS1_1) return new Ruins1_1(ruins1_1);
        if (ID == RUINS2_1) return new Ruins2_1(ruins2_1);
        if (ID == RUINS3_1) return new Ruins3_1(ruins3_1);
        if (ID == RUINS4_1) return new Ruins4_1(ruins4_1);
        if (ID == LOCATION1_1) return new Location1_1(location1_1);
        if (ID == LOCATION2_1) return new Location2_1(location2_1);
        if (ID == LOCATION3_1) return new Location3_1(location3_1);
        if (ID == EVENT1_1) return new Event1_1(event1_1);
        if (ID == EVENT2_1) return new Event2_1(event2_1);
        if (ID == EVENT3_1) return new Event3_1(event3_1);
        if (ID == EVENT4_1) return new Event4_1(event4_1);
        if (ID == EVENT5_1) return new Event5_1(event5_1);
        return null;
    }
}
