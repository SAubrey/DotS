using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TravelDeck : MonoBehaviour {
    public static TravelDeck I { get; private set; }

    // <TYPE><TIER>_<NUM>
    public const int ATT1_1 = 1, ATT1_2 = 2, ATT1_3 = 3, ATT1_4 = 4, ATT1_5 = 5, ATT1_6 = 6;
    public const int CHANCE1_1 = 8, CHANCE1_2 = 9, CHANCE1_3 = 10;
    public const int BLESSING1_1 = 11;
    public const int CAVE1_1 = 12, CAVE1_2 = 13;
    public const int EVENT1_1 = 14, EVENT1_2 = 15, EVENT1_3 = 16, EVENT1_4 = 17, EVENT1_5 = 18, EVENT1_6 = 19;
    public const int EVENT2_1 = 101, EVENT2_2 = 102, EVENT2_3 = 103, 
        EVENT2_4 = 104, EVENT2_5 = 105, EVENT2_6 = 106;
    public const int EVENT3_1 = 201;
    public const int RUINS1_1 = 20, RUINS1_2 = 21, RUINS1_3 = 22, RUINS1_4 = 23;
    public const int LOCATION1_1 = 24, LOCATION1_2 = 25, LOCATION1_3 = 26;
    public const int LOCATION2_1 = 107;

    int [] tier1_cards = new int[] {
        ATT1_1, ATT1_2, ATT1_3, ATT1_3, ATT1_5, ATT1_6,
        CHANCE1_1, CHANCE1_2, CHANCE1_3,
        //BLESSING1_1,
        CAVE1_1, CAVE1_2,
        EVENT1_1, EVENT1_2, EVENT1_3, EVENT1_4, EVENT1_5,
        RUINS1_1, RUINS1_2, RUINS1_3, RUINS1_4,
        LOCATION1_2, LOCATION1_3};
    
    int [] tier2_cards = new int[] {
        ATT1_1, ATT1_2, ATT1_3, ATT1_3, ATT1_5, ATT1_6,
        CHANCE1_1, CHANCE1_2, CHANCE1_3,
        //BLESSING1_1,
        CAVE1_1, CAVE1_2,
        EVENT2_1, EVENT2_2, EVENT2_3, EVENT2_4, EVENT2_5, EVENT2_6,
        RUINS1_1, RUINS1_2, RUINS1_3, RUINS1_4,
        LOCATION2_1
    };
    int [] tier3_cards = new int[] {
        ATT1_1, ATT1_2, ATT1_3, ATT1_3, ATT1_5, ATT1_6,
        CHANCE1_1, CHANCE1_2, CHANCE1_3,
        //BLESSING1_1,
        CAVE1_1, CAVE1_2,
        EVENT2_1, EVENT2_2, EVENT2_3, EVENT2_4, EVENT2_5, EVENT2_6,
        RUINS1_1, RUINS1_2, RUINS1_3, RUINS1_4,
        LOCATION2_1
    };
    int [][] cards;

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

    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        cards = new int[][] {tier1_cards, tier2_cards, tier3_cards};

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
        allowed_cards[MapCell.CAVE_ID].Add(TravelCard.CAVE);
        allowed_cards[MapCell.RUINS_ID].Add(TravelCard.RUINS);
        // no cards for star, lush. Settlement = quest card?
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
        } else if (biome_ID == MapCell.GUARDIAN_PASS_ID) {
            if (tier == 1) {
                return make_card(EVENT1_6);
            } else if (tier == 2) {
                return make_card(EVENT2_6);
            } else if (tier == 3) {
                return make_card(EVENT3_1);
            }
        } 
        else if (combat_cards_only) {
            return make_card(ATT1_1); // debug only
        }

        List<int> drawable_cards = aggregate_drawable_cards(tier, biome_ID);
        int rand_index = Random.Range(0, drawable_cards.Count);
        return make_card(drawable_cards[rand_index]);
    }


    // Returns cards that can be drawn at the current tile biome.
    private List<int> aggregate_drawable_cards(int tier, int biome_ID) {
        List<int> valid_card_ids = new List<int>();
        foreach (int card_id in cards[tier - 1]) {
            if (check_if_card_in_biome(biome_ID, card_id)) {
                valid_card_ids.Add(card_id);
            }
        }
        return valid_card_ids;
    }

    private bool check_if_card_in_biome(int biome_ID, int card_ID) {
        if (!allowed_cards.ContainsKey(biome_ID))
            return false;
        return allowed_cards[biome_ID].Contains(make_card(card_ID).type);
    }

    public TravelCard make_card(int ID) {
        if (ID == ATT1_1) return new Att1_1();
        if (ID == ATT1_2) return new Att1_2();
        if (ID == ATT1_3) return new Att1_3();
        if (ID == ATT1_4) return new Att1_4();
        if (ID == ATT1_5) return new Att1_5();
        if (ID == ATT1_6) return new Att1_6();
        if (ID == CHANCE1_1) return new Chance1_1();
        if (ID == CHANCE1_2) return new Chance1_2();
        if (ID == CHANCE1_3) return new Chance1_3();
        if (ID == BLESSING1_1) return new Blessing1_1();
        if (ID == CAVE1_1) return new Cave1_1();
        if (ID == CAVE1_2) return new Cave1_2();
        if (ID == RUINS1_1) return new Ruins1_1();
        if (ID == RUINS1_2) return new Ruins1_2();
        if (ID == RUINS1_3) return new Ruins1_3();
        if (ID == RUINS1_4) return new Ruins1_4();
        if (ID == LOCATION1_1) return new Location1_1();
        if (ID == LOCATION1_2) return new Location1_2();
        if (ID == LOCATION1_3) return new Location1_3();
        if (ID == EVENT1_1) return new Event1_1();
        if (ID == EVENT1_2) return new Event1_2();
        if (ID == EVENT1_3) return new Event1_3();
        if (ID == EVENT1_4) return new Event1_4();
        if (ID == EVENT1_5) return new Event1_5();

        if (ID == LOCATION2_1) return new Location2_1();
        if (ID == EVENT2_1) return new Event2_1();
        if (ID == EVENT2_2) return new Event2_2();
        if (ID == EVENT2_3) return new Event2_3();
        if (ID == EVENT2_4) return new Event2_4();
        if (ID == EVENT2_5) return new Event2_5();
        if (ID == EVENT2_6) return new Event2_6();
        return null;
    }
}
