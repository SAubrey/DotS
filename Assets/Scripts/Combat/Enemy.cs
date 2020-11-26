using UnityEngine;

public class Enemy : Unit {
    public const int COMMON = 0, UNCOMMON = 1, RARE = 2;

    // Enemy names
    public const int GALTSA = 0, GREM = 1, ENDU = 2,
        KOROTE = 3, MOLNER = 4, ETUENA = 5,
        CLYPTE = 6, GOLIATH = 7, KVERM = 8,
        LATU = 9, EKE_TU = 10, OETEM = 11,
        EKE_FU = 12, EKE_SHI_AMI = 13, EKE_LORD = 14,
        KETEMCOL = 15, MAHUKIN = 16, DRONGO = 17,
        MAHEKET = 18, CALUTE = 19, ETALKET = 20,
        MUATEM = 21, DRAK = 22, ZERRKU = 23,
        GOKIN = 24, TAJAQAR = 25, TAJAERO = 26,
        TERRA_QUAL = 27, DUALE = 28,
        MELD_WARRIOR = 29, MELD_SPEARMAN = 30;

    public int xp;
    public bool xp_taken = false;

    // player unit to be moved towards and attacked
    private Slot _target;
    public Slot target {
        get {
            if (_target != null) {
                if (_target.has_punit) 
                    return _target;
            } else 
                _target = null;
            return null;
        }
        set { _target = value; }
    } 

    
    public Enemy(string name, int ID, int att, int def, int hp, int xp, int style,
            int atr1=0, int atr2=0, int atr3=0) : 
            base(name, ID, att, def, hp, style, atr1, atr2, atr3) {
        type = ENEMY;
        this.xp = xp;
        defending = true;
    }

    public static Enemy create_enemy(int ID) {
        Enemy e = null;
        if (ID == GALTSA) e = new Galtsa();
        else if (ID == GREM) e = new Grem();
        else if (ID == ENDU) e = new Endu();
        else if (ID == KOROTE) e = new Korote();
        else if (ID == MOLNER) e = new Molner();
        else if (ID == ETUENA) e = new Etuena();
        else if (ID == CLYPTE) e = new Clypte();
        else if (ID == GOLIATH) e = new Goliath();
        else if (ID == KVERM) e = new Kverm();
        else if (ID == LATU) e = new Latu();
        else if (ID == EKE_TU) e = new Eke_tu();
        else if (ID == OETEM) e = new Oetem();
        else if (ID == EKE_FU) e = new Eke_fu();
        else if (ID == EKE_SHI_AMI) e = new Eke_shi_ami();
        else if (ID == EKE_LORD) e = new Eke_Lord();
        else if (ID == KETEMCOL) e = new Ketemcol();
        else if (ID == MAHUKIN) e = new Mahukin();
        else if (ID == DRONGO) e = new Drongo();
        else if (ID == MAHEKET) e = new Maheket();
        else if (ID == CALUTE) e = new Calute();
        else if (ID == ETALKET) e = new Etalket();
        else if (ID == MUATEM) e = new Muatem();
        else if (ID == DRAK) e = new Drak();
        else if (ID == ZERRKU) e = new Zerrku();
        else if (ID == GOKIN) e = new Gokin();
        else if (ID == TAJAQAR) e = new Tajaqar();
        else if (ID == TAJAERO) e = new Tajaero();
        else if (ID == TERRA_QUAL) e = new Terra_Qual();
        else if (ID == DUALE) e = new Duale();
        return e;
    }

    public bool attempt_move(Slot dest) {
        if (!can_move(dest))
            return false;
        move(dest);
        get_slot().get_group().rotate_towards_target(target.get_group());
        return true;
    }

    public bool can_target(Slot punit) {
        if (!punit.has_punit) 
            return false;

        bool melee_vs_flying = combat_style == Unit.MELEE && 
                    punit.get_unit().has_attribute(Unit.FLYING);
        return melee_vs_flying ? false : true;
    }

    public override int take_damage(int dmg) {
        health = Mathf.Max(0, health - dmg);
        slot.update_healthbar();

        if (health <= 0) {
            dead = true;
            slot.show_dead();
        }
        return health <= 0 ? DEAD : INJURED;
    }

    public override int calc_dmg_taken(int dmg, bool piercing=false) {
        // adjust for defensive attributes?
        if (!piercing)
            dmg -= defense;
        return dmg > 0 ? dmg : 0;
    }

    public override int get_post_dmg_state(int dmg) {
        return health - dmg <= 0 ? DEAD : INJURED;
    }

    public void clear_target() {
        target = null;
    }

    public int take_xp_from_death() {
        return xp_taken ? -1 : xp;
    }
}

// Plains
public class Galtsa : Enemy {
    public Galtsa() : base("Galtsa", GALTSA, 2, 0, 2, 2, MELEE, CHARGE, GROUPING_2) {
    }
}
public class Grem : Enemy {
    public Grem() : base("Grem", GREM, 1, 0, 1, 1, MELEE) {
    }
}
public class Endu : Enemy {
    public Endu() : base("Endu", ENDU, 4, 0, 2, 3, MELEE, CHARGE) {
    }
}
public class Korote : Enemy {
    public Korote() : base("Korote", KOROTE, 1, 0, 2, 2, MELEE, FLANKING, GROUPING_2) {
    }
}
public class Molner : Enemy {
    public Molner() : base("Molner", MOLNER, 2, 0, 2, 2, MELEE, FLANKING, CHARGE, GROUPING_2) {
    }
}
public class Etuena : Enemy {
    public Etuena() : base("Etuena", ETUENA, 2, 0, 3, 3, MELEE, FLYING, CHARGE, GROUPING_2) {
    }
}
public class Clypte : Enemy {
    public Clypte() : base("Clypte", CLYPTE, 3, 0, 5, 3, RANGE, TARGET_RANGE) {
    }
}
public class Goliath : Enemy {
    public Goliath() : base("Goliath", GOLIATH, 12, 0, 8, 6, MELEE, TERROR_3, CHARGE) {
    }
}

// Forest
public class Kverm : Enemy {
    public Kverm() : base("Kverm", KVERM, 2, 0, 1, 1, MELEE, STALK) {
    }
}
public class Latu : Enemy {
    public Latu() : base("Latu", LATU, 3, 0, 3, 3, MELEE, STALK, AGGRESSIVE) {
        max_num_actions = 3;
        num_actions = max_num_actions;
    }
}
public class Eke_tu : Enemy {
    public Eke_tu() : base("Eke Tu", EKE_TU, 1, 0, 2, 2, MELEE, TARGET_RANGE, AGGRESSIVE) {
        max_num_actions = 3;
        num_actions = max_num_actions;
    }
}
public class Oetem : Enemy {
    public Oetem() : base("Oetem", OETEM, 4, 0, 3, 3, MELEE, GROUPING_2) {
    }
}
public class Eke_fu : Enemy {
    public Eke_fu() : base("Eke Fu", EKE_FU, 3, 0, 2, 2, MELEE, GROUPING_2, FLANKING) {
    }
}
public class Eke_shi_ami : Enemy {
    public Eke_shi_ami() : base("Eke Shi Ami", EKE_SHI_AMI, 3, 0, 5, 4, RANGE, PIERCING, STUN, TARGET_HEAVY) {
    }
}
public class Eke_Lord : Enemy {
    public Eke_Lord() : base("Eke Lord", EKE_LORD, 6, 0, 12, 12, MELEE, ARCING_STRIKE, STUN, TARGET_HEAVY) {
    }
}
public class Ketemcol : Enemy {
    public Ketemcol() : base("Ketemcol", KETEMCOL, 2, 1, 8, 6, MELEE, ARCING_STRIKE, STUN) {
    }
}

// Titrum
public class Mahukin : Enemy {
    public Mahukin() : base("Mahukin", MAHUKIN, 2, 2, 3, 4, MELEE, GROUPING_2) {
    }
}
public class Drongo : Enemy {
    public Drongo() : base("Drongo", DRONGO, 3, 3, 6, 6, MELEE) {
    }
}
public class Maheket : Enemy {
    public Maheket() : base("Maheket", MAHEKET, 3, 2, 3, 5, MELEE, GROUPING_2) {
    }
}
public class Calute : Enemy {
    public Calute() : base("Calute", CALUTE, 6, 0, 6, 5, MELEE, STALK, AGGRESSIVE) {
        max_num_actions = 3;
        num_actions = max_num_actions;
    }
}
public class Etalket : Enemy {
    public Etalket() : base("Etalket", ETALKET, 2, 0, 5, 4, MELEE, STALK, TERROR_3) {
    }
}
public class Muatem : Enemy {
    public Muatem() : base("Muatem", MUATEM, 7, 5, 4, 12, MELEE, CRUSHING_BLOW) {
    }
}

// Mountain/Cliff
public class Drak : Enemy {
    public Drak() : base("Drak", DRAK, 3, 0, 5, 3, MELEE, TERROR_2, FLYING) {
    }
}
public class Zerrku : Enemy {
    public Zerrku() : base("Zerrku",ZERRKU, 3, 0, 3, 4, RANGE, GROUPING_2) {
    }
}
public class Gokin : Enemy {
    public Gokin() : base("Gokin", GOKIN, 2, 0, 2, 2, MELEE, FLANKING, GROUPING_2) {
    }
}

// Cave
public class Tajaqar : Enemy {
    public Tajaqar() : base("Tajaqar", TAJAQAR, 3, 1, 3, 5, MELEE, FLANKING, GROUPING_2) {
    }
}
public class Tajaero : Enemy {
    public Tajaero() : base("Tajaero", TAJAERO, 3, 0, 2, 4, RANGE, FLYING) {
    }
}
public class Terra_Qual : Enemy {
    public Terra_Qual() : base("Terra Qual", TERRA_QUAL, 5, 2, 10, 12, MELEE, ARCING_STRIKE) {
    }
}
public class Duale : Enemy {
    public Duale() : base("Duale", DUALE, 2, 0, 6, 5, RANGE, AGGRESSIVE, FLANKING) {
        max_num_actions = 3;
        num_actions = max_num_actions;
    }
}

public class Meld_Warrior : Enemy {
    public Meld_Warrior() : base("Meld Warrior", MELD_WARRIOR, 1, 0, 1, 1, MELEE, CHARGE, GROUPING_1) {
    }
}

public class Meld_Spearman : Enemy {
    public Meld_Spearman() : base("Meld Spearman", MELD_SPEARMAN, 1, 0, 1, 2, MELEE, CHARGE) {
    }
}