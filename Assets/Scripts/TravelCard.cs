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
    // ---RULE CODES---

    public const int ENTER_COMBAT = -1;
    public const int CHARGE = 0; // bonus attack phase before p1.
    public const int AMBUSH = 1; // skip init range stage
    public const int OFF_GUARD = 2; // att3, 2 units in reserve.
    public const int BLESSING1 = 3;

    public const int AFFECT_RESOURCES = 4;

    public const int FOG = 10;

    public int enemy_count;
    //public Dictionary<string, int> rewards = new Dictionary<string, int>();
    // Can be positive or negative.
    public Dictionary<string, int> consequence = new Dictionary<string, int>();

    // each value is the % out of 100 to roll over.
    // The index maps to the consequence.
    public int[] odds;
    public int die_num_sides;
    public int ID;
    public Sprite sprite;
    public bool reward = false;
    public int type;
    private Dictionary<int, bool> rules = new Dictionary<int, bool>();
    private static int[] RULE_FIELDS = { ENTER_COMBAT, CHARGE, AMBUSH, OFF_GUARD,
        BLESSING1, AFFECT_RESOURCES, FOG, };
    public bool requires_seeker = false;

    public virtual void action(TravelCardManager tcm) { }
    public virtual void use_roll_result(int result, Controller c) { }

    public TravelCard(int id, int type, Sprite sprite) {
        ID = id;
        this.type = type;
        this.sprite = sprite;
        foreach (string field in Storeable.FIELDS) {
            consequence.Add(field, 0);
        }
        foreach (int field in RULE_FIELDS) {
            rules.Add(field, false);
        }
    }

    public bool follow_rule(int rule) {
        return rules[rule];
    }

    protected void set_rule(int rule, bool state) {
        rules[rule] = state;
    }
}


public class Att : TravelCard {
    public Att(int ID, Sprite sprite) : base(ID, COMBAT, sprite) {
        this.enemy_count = 7;
        set_rule(ENTER_COMBAT, true);
    }
}

public class Att1_1 : Att {
    public Att1_1(Sprite sprite) : base(TravelDeck.ATT1_1, sprite) {
        this.enemy_count = 7;
    }
}

public class Att2_1 : Att {
    public Att2_1(Sprite sprite)  : base(TravelDeck.ATT2_1, sprite) {
        this.enemy_count = 8;
    }
}

public class Att3_1 : Att {
    public Att3_1(Sprite sprite) : base(TravelDeck.ATT3_1, sprite) {
        this.enemy_count = 7;
    }
}

public class Att4_1 : Att {
    public Att4_1( Sprite sprite) : base(TravelDeck.ATT4_1, sprite) {
        this.enemy_count = 7;
    }
}

public class Att5_1 : Att {
    public Att5_1( Sprite sprite) : base(TravelDeck.ATT5_1, sprite) {
        this.enemy_count = 5;
    }
}

public class Att6_1 : Att {
    public Att6_1( Sprite sprite) : base(TravelDeck.ATT6_1, sprite) {
        this.enemy_count = 6;
    }
}

public class Att7_1 : Att {
    public Att7_1(Sprite sprite) : base(TravelDeck.ATT7_1, sprite) {
        this.enemy_count = 5;
    }
}


public class Chance : TravelCard {
    public Chance(int ID, Sprite sprite) : base(ID, CHANCE, sprite) {
    }

    public override void action(TravelCardManager tcm) {
        tcm.set_up_roll(this, die_num_sides);
    }
}

public class Chance1_1 : Chance {
    public Chance1_1(Sprite sprite) : base(TravelDeck.CHANCE1_1, sprite) {
        this.enemy_count = 6;
        die_num_sides = 6;
    }

    public override void use_roll_result(int result, Controller c) {
        if (result % 2 == 0) {
            set_rule(ENTER_COMBAT, true);
        }
    }
}

public class Chance2_1 : Chance {
    public Chance2_1(Sprite sprite) : base(TravelDeck.CHANCE2_1, sprite) {
        this.enemy_count = 0;
    }

    public override void use_roll_result(int result, Controller c) {
        set_rule(AFFECT_RESOURCES, true);
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
    }

    public override void use_roll_result(int result, Controller c) {
        if (result % 2 == 1) {
            set_rule(ENTER_COMBAT, true);
        }
    }
}

public class CaveCard : TravelCard {
    public CaveCard(int ID, Sprite sprite) : base(ID, CAVE, sprite) {
        set_rule(ENTER_COMBAT, true);
    }
}
public class Cave1_1 : CaveCard {
    public Cave1_1(Sprite sprite) : base(TravelDeck.CAVE1_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        enemy_count = 5;
        consequence[Storeable.MINERALS] = 4;
        consequence[Storeable.STAR_CRYSTALS] = 4;
    }
}

public class Cave2_1 : CaveCard {
    public Cave2_1(Sprite sprite) : base(TravelDeck.CAVE2_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        enemy_count = 7;
        consequence[Storeable.STAR_CRYSTALS] = 2;
        // 1 equipment
    }
}


public class Blessing : TravelCard {
    public Blessing(int ID, Sprite sprite) : base(ID, BLESSING, sprite) {
    }
}

public class Blessing1_1 : Blessing {
    public Blessing1_1(Sprite sprite) : base(TravelDeck.BLESSING1_1, sprite) {
    }
}


public class RuinsCard : TravelCard {
    public RuinsCard(int ID, Sprite sprite) : base(ID, RUINS, sprite) {
    }
}

public class Ruins1_1 : RuinsCard {
    public Ruins1_1(Sprite sprite) : base(TravelDeck.RUINS1_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        consequence[Storeable.ARELICS] = 3;
        // +1 seeker unit 
    }

    public override void action(TravelCardManager tcm) {
        tcm.c.get_active_bat().add_units(PlayerUnit.SEEKER, 1);
    }
}

public class Ruins2_1 : RuinsCard {
    public Ruins2_1(Sprite sprite) : base(TravelDeck.RUINS2_1, sprite) {
        set_rule(ENTER_COMBAT, true);
        // +1 EQUIPMENT
    }
}

public class Ruins3_1 : RuinsCard {
    public Ruins3_1(Sprite sprite) : base(TravelDeck.RUINS3_1, sprite) {
        set_rule(ENTER_COMBAT, true);
        set_rule(AFFECT_RESOURCES, true);
        enemy_count = 5;
        consequence[Storeable.ERELICS] = 2;
        consequence[Storeable.MRELICS] = 2;
        consequence[Storeable.STAR_CRYSTALS] = 3;
    }
}

public class Ruins4_1 : RuinsCard {
    public Ruins4_1(Sprite sprite) : base(TravelDeck.RUINS4_1, sprite) {
        set_rule(ENTER_COMBAT, true);
        enemy_count = 7;
        // +1 EQUIPMENT
    }
}


public class LocationCard : TravelCard {
    public LocationCard(int ID, Sprite sprite) : base(ID, LOCATION, sprite) {
    }
}

public class Location1_1 : LocationCard {
    public Location1_1(Sprite sprite) : base(TravelDeck.LOCATION1_1, sprite) {
        // rune gate activated with 10 SC.
    }

    public override void action(TravelCardManager tcm) {
        tcm.c.map.build_rune_gate(tcm.c.get_disc().get_Pos());
    }
}

public class Location2_1 : LocationCard {
    public Location2_1(Sprite sprite) : base(TravelDeck.LOCATION2_1, sprite) {
        //consequence[]
        //  if activated, -5 sc
        //+ 1 equipment
    }
}

public class Location3_1 : LocationCard {
    public Location3_1(Sprite sprite) : base(TravelDeck.LOCATION3_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        consequence[Storeable.ARELICS] = 1;
        consequence[Storeable.ERELICS] = 1;
        consequence[Storeable.MRELICS] = 1;
        requires_seeker = true;
    }
}


public class Event : TravelCard {
    public Event(int ID, Sprite sprite) : base(ID, EVENT, sprite) { }
}

public class Event1_1 : Event {
    public Event1_1(Sprite sprite) : base(TravelDeck.EVENT1_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        consequence[Storeable.ARELICS] = 1;
    }
}

public class Event2_1 : Event {
    public Event2_1(Sprite sprite) : base(TravelDeck.EVENT2_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        consequence[Storeable.STAR_CRYSTALS] = 2;
    }
}

public class Event3_1 : Event {
    public Event3_1(Sprite sprite) : base(TravelDeck.EVENT3_1, sprite) {
    }
    public override void action(TravelCardManager tcm) {
        tcm.c.map.move_player(tcm.c.get_disc().prev_pos, true);
    }
}
public class Event4_1 : Event {
    public Event4_1(Sprite sprite) : base(TravelDeck.EVENT4_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        consequence[Storeable.UNITY] = -1;
    }
}

public class Event5_1 : Event {
    public Event5_1(Sprite sprite) : base(TravelDeck.EVENT5_1, sprite) {
        set_rule(AFFECT_RESOURCES, true);
        consequence[Storeable.MINERALS] = 2;
        consequence[Storeable.UNITY] = -2;
    }
}