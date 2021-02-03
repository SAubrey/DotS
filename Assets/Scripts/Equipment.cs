using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Equipment {
    public const string SHARPENED_BLADES = "Sharpened Blades",
    BOLSTERED_POLEARM = "Bolstered Polearm",
    PADDED_ARMOR = "Padded Armor",
    CHAINMAIL_GARB = "Chainmail Garb",
    BOLSTERED_SHIELDS = "Bolstered Shields",
    HARDENED_ARROW_TIPS = "Hardened Arrow Tips",
    EMANATOR = "Emanator",
    PULL_CART = "Pull Cart";
    public const string SPIRIT_BANNER = "Spirit Banner",
    CRYSTAL_CRESTED_MEDALLION = "Crystal Crested Medallion",
    PLATED_ARMOR = "Plated Armor",
    EMBEDDED_CRYSTAL_GARB = "Embedded Crystal Garb",
    WAGON = "Wagon",
    REFINED_MINERAL_ARMOR = "Refined Mineral Armor",
    TITRUM_REINFORCED_ARROW_TIPS = "Titrum Reinforced Arrow Tips";
    
    public static readonly string[] t1_equipment = {
        SHARPENED_BLADES, BOLSTERED_POLEARM, PADDED_ARMOR, CHAINMAIL_GARB,
        BOLSTERED_SHIELDS, HARDENED_ARROW_TIPS, EMANATOR, PULL_CART
    };
    public static readonly string[] t2_equipment = {
        //SPIRIT_BANNER,
         //CRYSTAL_CRESTED_MEDALLION,
          PLATED_ARMOR, EMBEDDED_CRYSTAL_GARB,
        WAGON, REFINED_MINERAL_ARMOR, TITRUM_REINFORCED_ARROW_TIPS
    };
    public static readonly string[] t3_equipment = {
        
    };
    public static readonly string[][] equipment = {
        t1_equipment, t2_equipment, t3_equipment
    };

    public List<int> affected_unit_types = new List<int>();
    public List<int> affected_stats = new List<int>();
    public int affect_amount;
    public string name;
    public string description;
    public bool equipped = false;

    public static Equipment make_equipment(string name) {
        if (name == SHARPENED_BLADES) {
            return new SharpenedBlades();
        } else if (name == BOLSTERED_POLEARM) {
            return new BolsteredPolearm();
        } else if (name == PADDED_ARMOR) {
            return new BolsteredPolearm();
        } else if (name == CHAINMAIL_GARB) {
            return new ChainmailGarb();
        } else if (name == BOLSTERED_SHIELDS) {
            return new BolsteredShields();
        } else if (name == HARDENED_ARROW_TIPS) {
            return new HardenedArrowTips();
        } else if (name == EMANATOR) {
            return new Emanator();
        } else if (name == PULL_CART) {
            return new PullCart();
        } else if (name == SPIRIT_BANNER) {
            return new SpiritBanner();
        } else if (name == CRYSTAL_CRESTED_MEDALLION) { 
            return new CrystalCrestedMedallion();
        } else if (name == PLATED_ARMOR) {
            return new PlatedArmor();
        } else if (name == EMBEDDED_CRYSTAL_GARB) {
            return new EmbeddedCrystalGarb();
        } else if (name == WAGON) {
            return new Wagon();
        } else if (name == REFINED_MINERAL_ARMOR) {
            return new RefinedMineralArmor();
        }
        return null;
    }

    public Equipment(string name) {
        this.name = name;
    }

    public virtual void activate() { }   

    public virtual void deactivate() { }
}

public class SharpenedBlades : Equipment {
    public SharpenedBlades() : base(SHARPENED_BLADES) {
        description = "+2 damage for warriors";
        affected_unit_types.Add(PlayerUnit.WARRIOR);
        affected_stats.Add(Unit.ATTACK);
        affect_amount = 2;
    }
}

public class BolsteredPolearm : Equipment {
    public BolsteredPolearm() : base(BOLSTERED_POLEARM) {
        description = "+1 damage for spearman";
        affected_unit_types.Add(PlayerUnit.SPEARMAN);
        affected_stats.Add(Unit.ATTACK);
        affect_amount = 1;
    }
}

public class PaddedArmor : Equipment {
    public PaddedArmor() : base(PADDED_ARMOR) {
        description = "+1 resilience for archers and inspirators";
        affected_unit_types.Add(PlayerUnit.ARCHER);
        affected_unit_types.Add(PlayerUnit.INSPIRATOR);
        affected_stats.Add(Unit.HEALTH);
        affect_amount = 1;
    }
}

public class ChainmailGarb : Equipment {
    public ChainmailGarb() : base(CHAINMAIL_GARB) {
        description = "+1 resilience for archers and inspirators";
        affected_unit_types.Add(PlayerUnit.WARRIOR);
        affected_unit_types.Add(PlayerUnit.MINER);
        affected_stats.Add(Unit.HEALTH);
        affect_amount = 1;
    }
}

public class BolsteredShields : Equipment {
    public BolsteredShields() : base(BOLSTERED_SHIELDS) {
        description = "+2 def for spearman";
        affected_unit_types.Add(PlayerUnit.SPEARMAN);
        affected_stats.Add(Unit.DEFENSE);
        affect_amount = 2;
    }
}

public class HardenedArrowTips : Equipment {
    public HardenedArrowTips() : base(HARDENED_ARROW_TIPS) {
        description = "+1 damage for archers";
        affected_unit_types.Add(PlayerUnit.ARCHER);
        affected_stats.Add(Unit.ATTACK);
        affect_amount = 1;
    }
}

public class Emanator : Equipment {
    public Emanator() : base(EMANATOR) {
        description = "Unity cannot fall below 1";
    }
    public override void activate() {
    }
}

public class PullCart : Equipment {
    public PullCart() : base(PULL_CART) {
        description = "Increases battalion inventory by 6";
    }
    public override void activate() {
        TurnPhaser.I.active_disc.capacity += 6;
    }
    public override void deactivate() {
        TurnPhaser.I.active_disc.capacity -= 6;
    }
}

public class SpiritBanner : Equipment {
    public SpiritBanner() : base(SPIRIT_BANNER) {
        description = "Whenever a spell is cast increase unity by 1";
    }
}


public class CrystalCrestedMedallion : Equipment {
    public CrystalCrestedMedallion() : base(CRYSTAL_CRESTED_MEDALLION) {
        description = "A seeker can use this once during a battle to revive up to 3 units";
    }
}

public class PlatedArmor : Equipment{
    public PlatedArmor() : base(PLATED_ARMOR) {
        description = "+2 resilience to warriors, spearman, guardians, paladins, and vanguard";
        affected_unit_types.Add(PlayerUnit.WARRIOR);
        affected_unit_types.Add(PlayerUnit.SPEARMAN);
        affected_unit_types.Add(PlayerUnit.GUARDIAN);
        affected_unit_types.Add(PlayerUnit.PALADIN);
        //affected_unit_types.Add(PlayerUnit.VANGUARD);
        affected_stats.Add(Unit.HEALTH);
        affect_amount = 2;
    }
}

public class EmbeddedCrystalGarb : Equipment{
    public EmbeddedCrystalGarb() : base(EMBEDDED_CRYSTAL_GARB) {
        description = "+1 resilience to archers, miners, inspirators, menders, and seekers";
        affected_unit_types.Add(PlayerUnit.ARCHER);
        affected_unit_types.Add(PlayerUnit.MINER);
        affected_unit_types.Add(PlayerUnit.INSPIRATOR);
        affected_unit_types.Add(PlayerUnit.MENDER);
        affected_unit_types.Add(PlayerUnit.SEEKER);
        affected_stats.Add(Unit.HEALTH);
        affect_amount = 1;
    }
}

public class Wagon : Equipment {
    public Wagon() : base(WAGON) {
        description = "Increases battalion inventory by 12";
    }
    public override void activate() {
        TurnPhaser.I.active_disc.capacity += 12;
    }
    public override void deactivate() {
        TurnPhaser.I.active_disc.capacity -= 12;
    }
}

public class RefinedMineralArmor : Equipment {
    public RefinedMineralArmor() : base(REFINED_MINERAL_ARMOR) {
        description = "+3 resilience to vanguard, paladin";
        //affected_unit_types.Add(PlayerUnit.VANGUARD);
        affected_unit_types.Add(PlayerUnit.PALADIN);
        affected_stats.Add(Unit.HEALTH);
        affect_amount = 3;
    }
}

public class TitrumReinforcedArrowTips : Equipment {
    public TitrumReinforcedArrowTips() : base(TITRUM_REINFORCED_ARROW_TIPS) {
        description = "+2 damage for arhcers and arbalest.\nAdds Piercing to archers and scouts";
        affected_unit_types.Add(PlayerUnit.SCOUT);
        affected_unit_types.Add(PlayerUnit.ARCHER);
        affected_stats.Add(Unit.DAMAGE);
        affect_amount = 3;
    }
}
