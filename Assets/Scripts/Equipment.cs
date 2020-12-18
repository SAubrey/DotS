using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Equipment {
    public const int SHARPENED_BLADES = 0,
    BOLSTERED_POLEARM = 1,
    PADDED_ARMOR = 2,
    CHAINMAIL_GARB = 3,
    BOLSTERED_SHIELDS = 4,
    HARDENED_ARROW_TIPS = 5,
    EMANATOR = 6,
    PULL_CART = 7;
    
    public static readonly int[] t1_equipment = {
        SHARPENED_BLADES, BOLSTERED_POLEARM, PADDED_ARMOR, CHAINMAIL_GARB,
        BOLSTERED_SHIELDS, HARDENED_ARROW_TIPS, EMANATOR, PULL_CART
    };
    public static readonly int[] t2_equipment = {

    };
    public static readonly int[] t3_equipment = {
        
    };
    public static readonly int[][] equipment = {
        t1_equipment, t2_equipment, t3_equipment
    };

    public List<int> affected_unit_types = new List<int>();
    public List<int> affected_stats = new List<int>();
    public int affect_amount;
    public string name;

    public static Equipment make_equipment(int ID) {
        if (ID == SHARPENED_BLADES) {
            return new SharpenedBlades();
        } else if (ID == BOLSTERED_POLEARM) {
            return new BolsteredPolearm();
        } else if (ID == PADDED_ARMOR) {
            return new BolsteredPolearm();
        } else if (ID == CHAINMAIL_GARB) {
            return new ChainmailGarb();
        } else if (ID == BOLSTERED_SHIELDS) {
            return new BolsteredShields();
        } else if (ID == HARDENED_ARROW_TIPS) {
            return new HardenedArrowTips();
        } else if (ID == EMANATOR) {
            return new Emanator();
        } else if (ID == PULL_CART) {
            return new PullCart();
        }
        return null;
    }


    public Equipment(string name) {
        this.name = name;
    }

    public virtual void activate() {
    }   
}

public class SharpenedBlades : Equipment {
    public SharpenedBlades() : base("Sharpened Blades") {
        affected_unit_types.Add(PlayerUnit.WARRIOR);
        affected_stats.Add(Unit.ATTACK_BOOST);
        affect_amount = 2;
    }
}

public class BolsteredPolearm : Equipment {
    public BolsteredPolearm() : base("Bolstered Polearm") {
        affected_unit_types.Add(PlayerUnit.SPEARMAN);
        affected_stats.Add(Unit.ATTACK_BOOST);
        affect_amount = 1;
    }
}

public class PaddedArmor : Equipment {
    public PaddedArmor() : base("Padded Armor") {
        affected_unit_types.Add(PlayerUnit.ARCHER);
        affected_unit_types.Add(PlayerUnit.INSPIRATOR);
        affected_stats.Add(Unit.HEALTH_BOOST);
        affect_amount = 1;
    }
}

public class ChainmailGarb : Equipment {
    public ChainmailGarb() : base("Chainmail Garb") {
        affected_unit_types.Add(PlayerUnit.WARRIOR);
        affected_unit_types.Add(PlayerUnit.MINER);
        affected_stats.Add(Unit.HEALTH_BOOST);
        affect_amount = 1;
    }
}

public class BolsteredShields : Equipment {
    public BolsteredShields() : base("Bolstered Shields") {
        affected_unit_types.Add(PlayerUnit.SPEARMAN);
        affected_stats.Add(Unit.DEFENSE_BOOST);
        affect_amount = 2;
    }
}

public class HardenedArrowTips : Equipment {
    public HardenedArrowTips() : base("Hardened Arrow Tips") {
        affected_unit_types.Add(PlayerUnit.ARCHER);
        affected_stats.Add(Unit.ATTACK_BOOST);
        affect_amount = 1;
    }
}

public class Emanator : Equipment {
    public Emanator() : base("Emanator") {
    }
    public override void activate() {
        Controller.I.get_disc().change_var(Storeable.UNITY, 1, true);
    }
}

public class PullCart : Equipment {
    public PullCart() : base("Pull Cart") {
    }
    public override void activate() {
        Controller.I.get_disc().capacity += 6;
    }
}