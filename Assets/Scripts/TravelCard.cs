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
    public bool complete;
    public int enemy_count;
    public string type_text, description, subtext, consequence_text;
    //public Dictionary<string, int> rewards = new Dictionary<string, int>();
    // Can be positive or negative.
    public Dictionary<string, int> consequence = new Dictionary<string, int>();
    public int equipment_reward_amount = 0;

    public int die_num_sides;
    public bool rolled = false;

    public Rules rules = new Rules();
    public TravelCardUnlockable unlockable;

    public virtual void on_open(TravelCardManager tcm) { }
    public virtual void on_continue(TravelCardManager tcm) { }
    public virtual void use_roll_result(int result) { }

    // Only accessed if ruins.
    public int enemy_biome_ID = MapCell.TITRUM_ID; 

    public TravelCard(int id, int type) {
        ID = id;
        this.type = type;
        foreach (string field in Controller.I.astra.resources.Keys) {
            consequence.Add(field, 0);
        }
    }
}

public class Rules {
    public bool enter_combat, requires_roll, charge, ambush, off_guard, 
        blessing1, affect_resources, fog = false;
}

public class TravelCardUnlockable {
    public string resource_type;
    public int resource_cost;
    public bool requires_seeker { get => required_unit_type == PlayerUnit.SEEKER; }
    public int required_unit_type;
    public TravelCardUnlockable(string type, int cost) {
        resource_type = type;
        resource_cost = cost;
    }
    public TravelCardUnlockable(int required_unit_type) {
        this.required_unit_type = required_unit_type;
    }
}


public class Att : TravelCard {
    public Att(int ID) : base(ID, COMBAT) {
        this.enemy_count = 7;
        rules.enter_combat = true;
        type_text = "Combat";
    }
}

public class Att1_1 : Att {
    public Att1_1() : base(TravelDeck.ATT1_1) {
        this.enemy_count = 5;
        description = "As we enter this new territory we have stumbled upon a few sorry beasts of the shadow.\nThis battle should be quick.";
        //subtext = "Two units start in reserve. 1 ranged and 1 melee if possible.";
        consequence_text = "Draw 5 enemies";
    }
}

public class Att1_2 : Att {
    public Att1_2()  : base(TravelDeck.ATT1_2) {
        this.enemy_count = 8;
        description = "This area is dangerous - there are creatures vigilant in pursuing our demise!";
        consequence_text = "Draw 8 enemies";
    }
}

public class Att1_3 : Att {
    public Att1_3() : base(TravelDeck.ATT1_3) {
        this.enemy_count = 7;
        description = "We have been caught off guard, form up men!";
        //subtext = "Two units start in reserve. 1 ranged and 1 melee if possible.";
        consequence_text = "Draw 7 enemies";
    }
}

public class Att1_4 : Att {
    public Att1_4() : base(TravelDeck.ATT1_4) {
        this.enemy_count = 7;
        rules.ambush = true;
        description = "Brace, men, it's an ambush!";
        subtext = "Skip player range phase.";
        consequence_text = "Draw 7 enemies";
    }
}

public class Att1_5 : Att {
    public Att1_5() : base(TravelDeck.ATT1_5) {
        this.enemy_count = 5;
        description = "On my signal... wait for it... Charge!";
        subtext = "Bonus attack phase. Preemptive enemy attributes like aggression do not trigger.";
        consequence_text = "Draw 5 enemies";
    }
}

public class Att1_6 : Att {
    public Att1_6() : base(TravelDeck.ATT1_6) {
        this.enemy_count = 6;
        description = "We were unable to sneak around these beats, lay on for light and glory!";
        consequence_text = "Draw 6 enemies";
    }
}


public class Chance : TravelCard {
    public Chance(int ID) : base(ID, CHANCE) {
        type_text = "Chance";
        rules.requires_roll = true;
    }

    public override void on_open(TravelCardManager tcm) {
        tcm.set_up_roll(this, die_num_sides);
    }
}

public class Chance1_1 : Chance {
    public Chance1_1() : base(TravelDeck.CHANCE1_1) {
        this.enemy_count = 6;
        die_num_sides = 6;
        description = "A thick fog has come over the land, we cannot see " + 
        "more than a few meters ahead even with our light. Hopefully we are not attacked.";
        subtext = "Roll D6\nIf even...";
        consequence_text = "Draw 6 enemies";
    }

    public override void use_roll_result(int result) {
        if (result % 2 == 0) {
            rules.enter_combat = true;
        }
    }
}

public class Chance1_2 : Chance {
    public Chance1_2() : base(TravelDeck.CHANCE1_2) {
        die_num_sides = 20;
        description = "It was around here we had anticipated ruins of our ancestors, " + 
        "but the last battalion never returned.\nMaybe we can at least find their remains.";
        subtext = "Roll D20";
        consequence_text = "13 or more - Some remaining resources are found\n3 star crystals, 1 mineral" + 
        "\n12 or less - Your men are demoralized\n-2 unity";
    }

    public override void use_roll_result(int result) {
        rules.affect_resources = true;
        if (result >= 13) {
            consequence[Storeable.MINERALS] = 1;
            consequence[Storeable.STAR_CRYSTALS] = 3;
        } else {
            consequence[Storeable.UNITY] = -2;
        }
        //TurnPhaser.I.active_disc.show_adjustments(consequence);
    }
}

public class Chance1_3 : Chance {
    public Chance1_3() : base(TravelDeck.CHANCE1_3) {
        this.enemy_count = 10;
        die_num_sides = 6;
        description = "If we move quietly we may be able to avoid these beasts.";
        subtext = "Roll D6 \nIf even...";
        consequence_text = "Draw 10 enemies";
    }

    public override void use_roll_result(int result) {
        if (result % 2 == 0) {
            rules.enter_combat = true;
        }
    }
}

public class CaveCard : TravelCard {
    public CaveCard(int ID) : base(ID, CAVE) {
        rules.enter_combat = true;
        type_text = "Cave";
    }
}
public class Cave1_1 : CaveCard {
    public Cave1_1() : base(TravelDeck.CAVE1_1) {
        rules.affect_resources = true;
        enemy_count = 5;
        consequence[Storeable.MINERALS] = 4;
        consequence[Storeable.STAR_CRYSTALS] = 4;
        description = "In hopes of finding links to our past we are instead met with great resistance. " +
        "The way is guarded - battle is our only way out now...";
        subtext = "Draw 5 cave enemies";
        consequence_text = "Victory\n4 star crystals\n4 minerals";
    }
}

public class Cave1_2 : CaveCard {
    public Cave1_2() : base(TravelDeck.CAVE1_2) {
        rules.affect_resources = true;
        equipment_reward_amount = 1;
        enemy_count = 7;
        consequence[Storeable.STAR_CRYSTALS] = 2;
        description = "The cave glows with runes of our ancestors, but an animosity dwells within. " + 
        "We are drawn by the light of our forebearers, we must defeat whatever lies within and see " +
        "what our people left behind.";
        subtext = "Draw 7 cave enemies";
        consequence_text = "Victory\n1 equipment\n2 star crystals";
    }
}


public class Blessing : TravelCard {
    public Blessing(int ID) : base(ID, BLESSING) {
        type_text = "Blessing";
    }
}

public class Blessing1_1 : Blessing {
    public Blessing1_1() : base(TravelDeck.BLESSING1_1) {
        description = "The stars shine bright - the creatues of the dark cower.";
        subtext = "Save Card: Can be discarded to avoid a future fight. (Except for ruins and caves)";
        consequence_text = "";
    }
}


public class RuinsCard : TravelCard {
    public RuinsCard(int ID) : base(ID, RUINS) {
        enemy_biome_ID = MapCell.TITRUM_ID;
        type_text = "Ruins";
    }
}

public class Ruins1_1 : RuinsCard {
    public Ruins1_1() : base(TravelDeck.RUINS1_1) {
        rules.affect_resources = true;
        consequence[Storeable.ARELICS] = 3;
        description = "These ruins at first seemed minor in scale, but with further inspection a panel " +
        "revealed a stairwell that descended into an old library. Within a seeker was found with his collected wisdom.";
        subtext = "";
        consequence_text = "1 Seeker unit\n3 Astra relics";
    }

    public override void on_continue(TravelCardManager tcm) {
        TurnPhaser.I.active_disc.bat.add_units(PlayerUnit.SEEKER, 1, true);
    }
}

public class Ruins1_2 : RuinsCard {
    public Ruins1_2() : base(TravelDeck.RUINS1_2) {
        rules.affect_resources = true;
        equipment_reward_amount = 1;
        description = "Luckily these ruins are empty of any beasts or lost souls, we are instead " +
        "blessed with abandoned treasures and knowledge.";
        subtext = "";
        consequence_text = "1 equipment";
    }
}

public class Ruins1_3 : RuinsCard {
    public Ruins1_3() : base(TravelDeck.RUINS1_3) {
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
        consequence_text = "2 Endura relics\n2 Martial relics\n3 star crystals";
    }
}

public class Ruins1_4 : RuinsCard {
    public Ruins1_4() : base(TravelDeck.RUINS1_4) {
        rules.enter_combat = true;
        rules.affect_resources = true;
        equipment_reward_amount = 1;
        enemy_biome_ID = MapCell.TITRUM_ID;
        enemy_count = 7;
        description = "These ruins have been overrun by the creatures of a nearby titrum forest.\n " +
        "To find our past we must slay these lingering creatures.";
        subtext = "Draw 7 titrum enemies";
        consequence_text = "1 equipment";
    }
}


public class LocationCard : TravelCard {
    public LocationCard(int ID) : base(ID, LOCATION) {
        type_text = "Location";
    }
}

public class Location1_1 : LocationCard {
    public Location1_1() : base(TravelDeck.LOCATION1_1) {
        // Aesthetic. Prompts rune gate image. 
        // rune gate activated with 10 SC.
        //unlockable = new TravelCardUnlockable(Storeable.STAR_CRYSTALS, 10);
        description = "Rune Gate";
        subtext = "Activate with 10 star crystals";
        consequence_text = "Allows travel to other activated gates.";
    }
}

public class Location1_2 : LocationCard {
    public Location1_2() : base(TravelDeck.LOCATION1_2) {
        unlockable = new TravelCardUnlockable(Storeable.STAR_CRYSTALS, -5);
        rules.affect_resources = true;
        equipment_reward_amount = 1;
        description = "You find a sealed ancestral safe keep, there are 5 star crystal slots " +
        "that must be filled to open it.";
        subtext = "";
        consequence_text = "1 equipment";
    }
}

public class Location1_3 : LocationCard {
    public Location1_3() : base(TravelDeck.LOCATION1_3) {
        rules.affect_resources = true;
        consequence[Storeable.ARELICS] = 1;
        consequence[Storeable.ERELICS] = 1;
        consequence[Storeable.MRELICS] = 1;
        unlockable = new TravelCardUnlockable(PlayerUnit.SEEKER);
        description = "A large stone door built into the side of a stone mound bears the symbol " +
        "of refuge. How might it open?";
        subtext = "Requires Seeker";
        consequence_text = "1 Astra relic\n1 Endura relic\n1 Martial relic";
    }
}


public class Location2_1 : LocationCard {
    public Location2_1() : base(TravelDeck.LOCATION2_1) {
        rules.affect_resources = true;
        equipment_reward_amount = 2;
        consequence[Storeable.STAR_CRYSTALS] = 6;
        consequence[Storeable.ARELICS] = 1;
        consequence[Storeable.MRELICS] = 1;
        unlockable = new TravelCardUnlockable(PlayerUnit.SEEKER);
        description = "The land descends here into a stone altar. Down some steps a beautiful courtyard" +
        "was revealed, the power of Astra glowing along the ley lines of our ancestors, pulsing with life." +
        "Towards the back of the main area stands a deep emerald green obelisk from which all the ley lines converge.\n" +
        "You see a key hole.";
        subtext = "Requires 1 Imbued Key";
        consequence_text = "2 tier 2 items\n6 star crystals\n1 Astra relic\n1 Martial relic";
    }
}



public class Event : TravelCard {
    public Event(int ID) : base(ID, EVENT) { 
        type_text = "Event";
    }
}

public class Event1_1 : Event {
    public Event1_1( ) : base(TravelDeck.EVENT1_1) {
        rules.affect_resources = true;
        consequence[Storeable.ARELICS] = 1;
        description = "This is an old shrine to the conscious sphere of knowledge, what a " + 
        "tragic wreck this place is now...\nWe were fortunate to recover a book containing our ancestors " +
        "wisdom on the light.";
        subtext = "";
        consequence_text = "1 Astra relic";
    }
}

public class Event1_2 : Event {
    public Event1_2( ) : base(TravelDeck.EVENT1_2) {
        rules.affect_resources = true;
        consequence[Storeable.STAR_CRYSTALS] = 2;
        description = "Your battalion comes upon a decayed pillar with symbols of the old empire " +
        "inscribed upon its faces.\nAt its base lies a small stash of star crystals.";
        subtext = "";
        consequence_text = "2 star crystals";
    }
}

public class Event1_3 : Event {
    public Event1_3( ) : base(TravelDeck.EVENT1_3) {
        description = "A horde of creatures have gathered.\nWe have no choice but to turn back.";
        subtext = "";
        consequence_text = "Return to the space from which you came.";
    }
    public override void on_continue(TravelCardManager tcm) {
        //tcm.c.map.move_player(tcm.c.get_disc().prev_pos);
        TurnPhaser.I.active_disc.move(TurnPhaser.I.active_disc.previous_cell);
    }
}
public class Event1_4 : Event {
    public Event1_4( ) : base(TravelDeck.EVENT1_4) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = -1;
        description = "A single skeleton is found leaning against a rock. There are no ruins in sight." +
        "The ribs are shattered and the head is cocked back, its mouth agape.\n\"What a horrendous sight\"...";
        subtext = "";
        consequence_text = "-1 unity";
    }
}

public class Event1_5 : Event {
    public Event1_5( ) : base(TravelDeck.EVENT1_5) {
        rules.affect_resources = true;
        consequence[Storeable.MINERALS] = 2;
        consequence[Storeable.UNITY] = -2;
        description = "As you come into an opening you see several buildings with doors and windows smashed in. " +
        "There is a stench of decay and blood stains the walls, but there are no bodies.";
        subtext = "";
        consequence_text = "-2 unity\n2 minerals";
    }
}

public class Event1_6 : Event {
    public Event1_6() : base(TravelDeck.EVENT1_6) {
        rules.enter_combat = true;
        description = "Duot Reach is an old, decrepit bridge. They say it is so wide the chariots of all ten lords could sit " +
        "sideways front to back and you could still walk around! The bridge has not been crossed by any Leohatar in an age.\n" +
        "Now a demon lingers there - waiting.";
        subtext = "";
        consequence_text = "";
    }
}

public class Event2_1 : Event {
    public Event2_1( ) : base(TravelDeck.EVENT2_1) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = -1;
        description = "Intense storms force your men to find shelter.";
        subtext = "";
        consequence_text = "No action next turn\n-1 unity";
    }
}


public class Event2_2 : Event {
    public Event2_2( ) : base(TravelDeck.EVENT2_2) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = -2;
        description = "The land moans and rumbles - a curdled scream can be heard echoing out from the reaches of the darkness.";
        subtext = "";
        consequence_text = "-2 unity";
    }
}

public class Event2_3 : Event {
    public Event2_3( ) : base(TravelDeck.EVENT2_3) {
        description = "Old traps and pitfalls from forgotten feuds can lay waste to unwary travelers..";
        subtext = "If a scout is present in your battalion, avoid the trap.\nOtherwise..";
        consequence_text = "Two random units are killed.";
    }
    public override void on_continue(TravelCardManager tcm) {
        // Kill 2 random units
        if (!TurnPhaser.I.active_disc.bat.has_scout) {
            TurnPhaser.I.active_disc.bat.lose_random_unit("Caught in a trap");
            TurnPhaser.I.active_disc.bat.lose_random_unit("Caught in a trap");
        }
    }
}
// Card tale of travelers skipped


public class Event2_4 : Event {
    public Event2_4() : base(TravelDeck.EVENT2_4) {
        rules.affect_resources = true;
        consequence[Storeable.UNITY] = 3;
        description = "\"The stars are shining brighter... and after such a long, dark night.\nI can feel the path forward.\"";
        subtext = "";
        consequence_text = "3 unity";
    }
}


public class Event2_5 : Event {
    public Event2_5() : base(TravelDeck.EVENT2_5) { 
        rules.affect_resources = true;
        equipment_reward_amount = 1;
        this.enemy_count = 5;
        die_num_sides = 6;
        description = "Harsh rains batter over the region - This could bear ill tidings.";
        subtext = "Roll D6\n If even, draw 5 enemies.\nRanged units range is limited to 2 tiles.";
        consequence_text = "2 Martial relics\n1 equipment";
    }

    public override void use_roll_result(int result) {
        if (result % 2 == 0) {
            rules.enter_combat = true;
            rules.affect_resources = true;
        }
    }
}


public class Event2_6 : Event {
    public Event2_6() : base(TravelDeck.EVENT2_6) {
        rules.enter_combat = true;
        description = "Duot Reach 2 is an old, decrepit bridge. They say it is so wide the chariots of all ten lords could sit " +
        "sideways front to back and you could still walk around! The bridge has not been crossed by any Leohatar in an age.\n" +
        "Now a demon lingers there - waiting.";
        subtext = "";
        consequence_text = "";
    }
}

public class Event3_1 : Event {
    public Event3_1() : base(TravelDeck.EVENT3_1) {
        rules.enter_combat = true;
        description = "Duot Reach 3 is an old, decrepit bridge. They say it is so wide the chariots of all ten lords could sit " +
        "sideways front to back and you could still walk around! The bridge has not been crossed by any Leohatar in an age.\n" +
        "Now a demon lingers there - waiting.";
        subtext = "";
        consequence_text = "";
    }
}

