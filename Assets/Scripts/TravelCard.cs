using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TravelCard {
    // ---TYPES---
    public const int COMBAT = 1;
    public const int BLESSING = 2;
    public const int CHANCE = 3;
    public const int CAVE = 4;
    public const int EVENT = 5;
    public const int LOCATION = 6;
    public const int RUINS = 7;
    public const int QUEST = 8;

    public int ID;
    public int type;
    public Sprite sprite;
    public bool complete;
    public int enemy_count;
    public string type_text, description, subtext, consequence_text;
    //public Dictionary<string, int> rewards = new Dictionary<string, int>();
    // Can be positive or negative.
    public Dictionary<string, int> consequence = new Dictionary<string, int>();

    // each value is the % out of 100 to roll over.
    // The index maps to the consequence.
    public int[] odds;
    public bool reward = false;
    public int die_num_sides;

    public Rules rules = new Rules();
    public bool requires_seeker = false;
    public TravelCardUnlockable unlockable;

    public virtual void action(TravelCardManager tcm) { }
    public virtual void use_roll_result(int result, Controller c) { }

    // Only accessed if ruins.
    public int enemy_biome_ID = MapCell.TITRUM_ID; 

    public TravelCard(int id, int type, Sprite sprite) {
        ID = id;
        this.type = type;
        this.sprite = sprite;
        foreach (string field in Storeable.FIELDS) {
            consequence.Add(field, 0);
        }
    }
}

public class Rules {
    public bool enter_combat, charge, ambush, off_guard, 
        blessing1, affect_resources, fog = false;
}

public class TravelCardUnlockable {
    public string resource_type;
    public bool unlocked = false;
    public int resource_cost;
    public bool requires_seeker = false;
    public TravelCardUnlockable(string type, int cost) {
        resource_type = type;
        resource_cost = cost;
    }
    public TravelCardUnlockable(bool requires_seeker) {
        this.requires_seeker = requires_seeker;
    }
}


public class Att : TravelCard {
    public Att(int ID, Sprite sprite) : base(ID, COMBAT, sprite) {
        this.enemy_count = 7;
        rules.enter_combat = true;
        type_text = "Combat";
    }
}

public class Att1_1 : Att {
    public Att1_1(Sprite sprite) : base(TravelDeck.ATT1_1, sprite) {
        this.enemy_count = 5;
        description = "As we enter this new territory we have stumbled upon a few sorry beasts of the shadow.\nThis battle should be quick.";
        //subtext = "Two units start in reserve. 1 ranged and 1 melee if possible.";
        consequence_text = "Draw 5 enemies";
    }
}

public class Att2_1 : Att {
    public Att2_1(Sprite sprite)  : base(TravelDeck.ATT2_1, sprite) {
        this.enemy_count = 8;
        description = "This area is dangerous - there are creatures vigilant in pursuing our demise!";
        consequence_text = "Draw 8 enemies";
    }
}

public class Att3_1 : Att {
    public Att3_1(Sprite sprite) : base(TravelDeck.ATT3_1, sprite) {
        this.enemy_count = 7;
        description = "We have been caught offguard, form up men!";
        //subtext = "Two units start in reserve. 1 ranged and 1 melee if possible.";
        consequence_text = "Draw 7 enemies";
    }
}

public class Att4_1 : Att {
    public Att4_1( Sprite sprite) : base(TravelDeck.ATT4_1, sprite) {
        this.enemy_count = 7;
        rules.ambush = true;
        description = "Brace, men, it's an ambush!";
        subtext = "Skip player range phase.";
        consequence_text = "Draw 7 enemies";
    }
}

public class Att5_1 : Att {
    public Att5_1( Sprite sprite) : base(TravelDeck.ATT5_1, sprite) {
        this.enemy_count = 5;
        description = "On my signal... wait for it... Charge!";
        subtext = "Bonus attack phase. Preemptive enemy attributes like aggression do not trigger.";
        consequence_text = "Draw 5 enemies";
    }
}

public class Att6_1 : Att {
    public Att6_1( Sprite sprite) : base(TravelDeck.ATT6_1, sprite) {
        this.enemy_count = 6;
        description = "We were unable to sneak around these beats, lay on for light and glory!";
        consequence_text = "Draw 6 enemies";
    }
}


public class Chance : TravelCard {
    public Chance(int ID, Sprite sprite) : base(ID, CHANCE, sprite) {
        type_text = "Chance";
    }

    public override void action(TravelCardManager tcm) {
        tcm.set_up_roll(this, die_num_sides);
    }
}

public class Chance1_1 : Chance {
    public Chance1_1(Sprite sprite) : base(TravelDeck.CHANCE1_1, sprite) {
        this.enemy_count = 6;
        die_num_sides = 6;
        description = "A thick fog has come over the land, we cannot see " + 
        "more than a few meters ahead even with our light. Hopefully we are not attacked.";
        subtext = "Roll D6 \nIf even...";
        consequence_text = "Draw 6 enemies";
    }

    public override void use_roll_result(int result, Controller c) {
        if (result % 2 == 0) {
            rules.enter_combat = true;
        }
    }
}

public class Chance2_1 : Chance {
    public Chance2_1(Sprite sprite) : base(TravelDeck.CHANCE2_1, sprite) {
        this.enemy_count = 0;
        die_num_sides = 6;
        description = "It was around here we had anticipated ruins of our ancestors, " + 
        "but the last battalion never returned, maybe we can at least find their remains.";
        subtext = "Roll D20";
        consequence_text = "13 or more - Some remaining resources are found.\n +3 star crystals, +1 mineral." + 
        "12 or less - Your men are demoralized.\n -2 unity";
    }

    public override void use_roll_result(int result, Controller c) {
        rules.affect_resources = true;
        if (result >= 13) {
            consequence[Storeable.MINERALS] = 1;
            consequence[Storeable.STAR_CRYSTALS] = 3;
        } else {
            // lose 2 unity
            consequence[Storeable.UNITY] = -2;
        }
        c.get_disc().adjust_resources_visibly(consequence);
    }
}

public class Chance3_1 : Chance {
    public Chance3_1(Sprite sprite) : base(TravelDeck.CHANCE3_1, sprite) {
        this.enemy_count = 10;
        die_num_sides = 6;
        description = "If we move quietly we may be able to avoid these beasts.";
        subtext = "Roll D6 \nIf even...";
        consequence_text = "Draw 10 enemies";
    }

    public override void use_roll_result(int result, Controller c) {
        if (result % 2 == 1) {
            rules.enter_combat = true;
        }
    }
}

public class CaveCard : TravelCard {
    public CaveCard(int ID, Sprite sprite) : base(ID, CAVE, sprite) {
        rules.enter_combat = true;
        type_text = "Cave";
    }
}
public class Cave1_1 : CaveCard {
    public Cave1_1(Sprite sprite) : base(TravelDeck.CAVE1_1, sprite) {
        rules.affect_resources = true;
        enemy_count = 5;
        consequence[Storeable.MINERALS] = 4;
        consequence[Storeable.STAR_CRYSTALS] = 4;
        description = "In hopes of finding links to our past we are instead met with great resistance." +
        "The way is guarded, battle is our only way out now...";
        subtext = "Draw 5 cave enemies";
        consequence_text = "Victory\n4 star crystals\n4 minerals";
    }
}

public class Cave2_1 : CaveCard {
    public Cave2_1(Sprite sprite) : base(TravelDeck.CAVE2_1, sprite) {
        rules.affect_resources = true;
        enemy_count = 7;
        consequence[Storeable.STAR_CRYSTALS] = 2;
        // 1 equipment
        description = "The cave glows with runes of our ancestors, but an animosity dwells within. " + 
        "We are drawn by the light of our forebearers, we must defeat whatever lies within and see " +
        "what our people left behind.";
        subtext = "Draw 7 cave enemies";
        consequence_text = "Victory\n1 equipment\n2 star crystals";
    }
}


public class Blessing : TravelCard {
    public Blessing(int ID, Sprite sprite) : base(ID, BLESSING, sprite) {
        type_text = "Blessing";
    }
}

public class Blessing1_1 : Blessing {
    public Blessing1_1(Sprite sprite) : base(TravelDeck.BLESSING1_1, sprite) {
        description = "The stars shine bright - the creatues of the dark cower.";
        subtext = "Save Card: Can be discarded to avoid a future fight. (Except for ruins and caves)";
        consequence_text = "";
    }
}


public class RuinsCard : TravelCard {
    public RuinsCard(int ID, Sprite sprite) : base(ID, RUINS, sprite) {
        enemy_biome_ID = MapCell.TITRUM_ID;
        type_text = "Ruins";
    }
}

public class Ruins1_1 : RuinsCard {
    public Ruins1_1(Sprite sprite) : base(TravelDeck.RUINS1_1, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.ARELICS] = 3;
        description = "These ruins at first seemed minor in scale, but with further inspection a panel " +
        "revealed a stairwell that descended into an old library. Within a seeker was found with his collected wisdom.";
        subtext = "";
        consequence_text = "+1 Seeker unit\n+3 Astra relics";
    }

    public override void action(TravelCardManager tcm) {
        // +1 seeker unit 
        Controller.I.get_disc().bat.add_units(PlayerUnit.SEEKER, 1);
    }
}

public class Ruins2_1 : RuinsCard {
    public Ruins2_1(Sprite sprite) : base(TravelDeck.RUINS2_1, sprite) {
        // +1 EQUIPMENT
        description = "Luckily these ruins are empty of any beasts or lost souls, we are instead " +
        "blessed with abandoned treasures and knowledge.";
        subtext = "";
        consequence_text = "+1 equipment";
    }
}

public class Ruins3_1 : RuinsCard {
    public Ruins3_1(Sprite sprite) : base(TravelDeck.RUINS3_1, sprite) {
        rules.enter_combat = true;
        rules.affect_resources = true;
        enemy_biome_ID = MapCell.MELD_ID;
        enemy_count = 5;
        consequence[Storeable.ERELICS] = 2;
        consequence[Storeable.MRELICS] = 2;
        consequence[Storeable.STAR_CRYSTALS] = 3;
        description = "There are old tales of darkness formed in being likened to us, the Leohatar. " +
        "Indeed, they look like us but there is no light in their eyes, driven by fear and madness, " +
        "seeking to snuff out the light.";
        subtext = "Draw 5 meld enemies";
        consequence_text = "+2 Endura relics\n+2 Martial relics\n+3 star crystals";
    }
}

public class Ruins4_1 : RuinsCard {
    public Ruins4_1(Sprite sprite) : base(TravelDeck.RUINS4_1, sprite) {
        rules.enter_combat = true;
        enemy_count = 7;
        // +1 EQUIPMENT
        description = "Luckily these ruins are empty of any beasts or lost souls, we are instead " +
        "blessed with abandoned treasures and knowledge.";
        subtext = "";
        consequence_text = "+1 equipment";
    }
}


public class LocationCard : TravelCard {
    public LocationCard(int ID, Sprite sprite) : base(ID, LOCATION, sprite) {
        type_text = "Location";
    }
}

public class Location1_1 : LocationCard {
    public Location1_1(Sprite sprite) : base(TravelDeck.LOCATION1_1, sprite) {
        // Aesthetic. Prompts rune gate image. 
        // rune gate activated with 10 SC.
        //unlockable = new TravelCardUnlockable(Storeable.STAR_CRYSTALS, 10);
        description = "Rune Gate";
        subtext = "Activate with 10 star crystals";
        consequence_text = "Allows travel to other activated gates.";
    }
}

public class Location2_1 : LocationCard { // NOT OPERABLE
    public Location2_1(Sprite sprite) : base(TravelDeck.LOCATION2_1, sprite) {
        unlockable = new TravelCardUnlockable(Storeable.STAR_CRYSTALS, 5);
        //+ 1 equipment
        description = "You find a sealed ancestral safe keep, there are 5 star crystal slots " +
        "that must be filled to open it.";
        subtext = "";
        consequence_text = "+1 equipment";
    }
}

public class Location3_1 : LocationCard {
    public Location3_1(Sprite sprite) : base(TravelDeck.LOCATION3_1, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.ARELICS] = 1;
        consequence[Storeable.ERELICS] = 1;
        consequence[Storeable.MRELICS] = 1;
        unlockable = new TravelCardUnlockable(true);
        description = "A large stone door built into the side of a stone mound bears the symbol " +
        "of refuge. How might it open?";
        subtext = "Requires Seeker";
        consequence_text = "+1 Astra relic\n+1 Endura relic\n+1 Martial relic";
    }
}


public class Location1_2 : LocationCard { // NOT OPERABLE
    public Location1_2(Sprite sprite) : base(TravelDeck.LOCATION1_2, sprite) {
        rules.affect_resources = true;
        // +2 equipment
        consequence[Storeable.STAR_CRYSTALS] = 6;
        consequence[Storeable.ARELICS] = 1;
        consequence[Storeable.MRELICS] = 1;
        unlockable = new TravelCardUnlockable(true);
        description = "The land descends here into a stone altar. Down some steps a beautiful courtyard" +
        "was revealed, the power of Astra glowing along the ley lines of our ancestors, pulsing with life." +
        "Towards the back of the main area stands a deep emerald green obelisk from which all the ley lines converge.\n" +
        "You see a key hole.";
        subtext = "Requires 1 Imbued Key";
        consequence_text = "2 tier 2 items\n+6 star crystals\n+1 Astra relic\n+1 Martial relic";
    }
}



public class Event : TravelCard {
    public Event(int ID, Sprite sprite) : base(ID, EVENT, sprite) { 
        type_text = "Event";
    }
}

public class Event1_1 : Event {
    public Event1_1(Sprite sprite) : base(TravelDeck.EVENT1_1, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.ARELICS] = 1;
        description = "This is an old shrine to the conscious sphere of knowledge, what a " + 
        "tragic wreck this place is now...\nWe were fortunate to recover a book containing our ancestors " +
        "wisdom on the light.";
        subtext = "";
        consequence_text = "+1 Astra relic";
    }
}

public class Event2_1 : Event {
    public Event2_1(Sprite sprite) : base(TravelDeck.EVENT2_1, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.STAR_CRYSTALS] = 2;
        description = "Your battalion comes upon a decayed pillar with symbols of the old empire " +
        "inscribed upon its faces.\nAt its base lies a small stash of star crystals.";
        subtext = "";
        consequence_text = "+2 star crystals";
    }
}

public class Event3_1 : Event {
    public Event3_1(Sprite sprite) : base(TravelDeck.EVENT3_1, sprite) {
        description = "A horde of creatures have gathered.\nWe have no choice but to turn back.";
        subtext = "";
        consequence_text = "Return to the space from which you came.";
    }
    public override void action(TravelCardManager tcm) {
        //tcm.c.map.move_player(tcm.c.get_disc().prev_pos);
        Controller.I.get_disc().move(Controller.I.get_disc().previous_cell);
    }
}
public class Event4_1 : Event {
    public Event4_1(Sprite sprite) : base(TravelDeck.EVENT4_1, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = -1;
        description = "A single skeleton is found leaning against a rock. There are no ruins in sight." +
        "The ribs are shattered and the head is cocked back, its mouth agape.\n\"What a horrendous sight\"...";
        subtext = "";
        consequence_text = "-1 unity";
    }
}

public class Event5_1 : Event {
    public Event5_1(Sprite sprite) : base(TravelDeck.EVENT5_1, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.MINERALS] = 2;
        consequence[Storeable.UNITY] = -2;
        description = "As you come into an opening you see several buildings with doors and windows smashed in. " +
        "There is a stench of decay and blood stains the walls, but there are no bodies.";
        subtext = "";
        consequence_text = "-2 unity\n+2 minerals";
    }
}


public class Event1_2 : Event {
    public Event1_2(Sprite sprite) : base(TravelDeck.EVENT1_2, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = -1;
        description = "Intense storms force your men to find shelter.";
        subtext = "";
        consequence_text = "No action next turn \n-1 unity";
    }
}


public class Event2_2 : Event {
    public Event2_2(Sprite sprite) : base(TravelDeck.EVENT2_2, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = -2;
        description = "The land moans and rumbles - a curdled scream can be heard echoing out from the reaches of the darkness.";
        subtext = "";
        consequence_text = "-2 unity";
    }
}

public class Event3_2 : Event {
    public Event3_2(Sprite sprite) : base(TravelDeck.EVENT3_2, sprite) {
        description = "Old traps and pitfalls from forgotten feuds can lay waste to unwary travelers..";
        subtext = "If a scout is present in your battalion, avoid the trap.\nOtherwise..";
        consequence_text = "Two random units are killed.";
    }
    public override void action(TravelCardManager tcm) {
        // Kill 2 random units
        if (!Controller.I.get_disc().bat.has_scout) {
            Controller.I.get_disc().bat.lose_random_unit("Caught in a trap");
            Controller.I.get_disc().bat.lose_random_unit("Caught in a trap");
        }
    }
}
// Card tale of travelers skipped


public class Event4_2 : Event {
    public Event4_2(Sprite sprite) : base(TravelDeck.EVENT4_2, sprite) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = 3;
        description = "\"The stars are shining brighter... and after such a long, dark night.\nI can feel the path forward.\"";
        subtext = "";
        consequence_text = "+3 unity";
    }
}

public class Event5_2 : Event {
    public Event5_2(Sprite sprite) : base(TravelDeck.EVENT5_2, sprite) {
        description = "Duot Reach is an old, decrepit bridge. They say it is so wide the chariots of all ten lords could sit " +
        "sideways front to back and you could still walk around! The bridge has not been crossed by any Leohatar in an age.\n" +
        "Now a demon lingers there - waiting.";
        subtext = "";
        consequence_text = "";
    }
}

public class Event6_2 : Event {
    public Event6_2(Sprite sprite) : base(TravelDeck.EVENT6_2, sprite) {
        this.enemy_count = 5;
        die_num_sides = 6;
        rules.affect_resources = true;
        description = "Harsh rains batter over the region - This could bear ill tidings.";
        subtext = "Roll D6\n If even, draw 5 enemies.\nRanged units range is limited to 2 tiles.";
        consequence_text = "+2 Martial relics\n+1 equipment";
    }

    public override void use_roll_result(int result, Controller c) {
        if (result % 2 == 0) {
            rules.enter_combat = true;
            rules.affect_resources = true;
        }
    }
}