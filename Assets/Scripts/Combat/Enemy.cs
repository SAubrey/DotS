
public class Enemy : Unit {
    public const int COMMON = 0;
    public const int UNCOMMON = 1;
    public const int RARE = 2;

    // Enemy names
    public const int GALTSA = 0;
    public const int GREM = 1;
    public const int ENDU = 2;
    public const int KOROTE = 3;
    public const int MOLNER = 4;
    public const int ETUENA = 5;
    public const int CLYPTE = 6;
    public const int GOLIATH = 7;
    public const int KVERM = 8;
    public const int LATU = 9;
    public const int EKE_TU = 10;
    public const int OETEM = 11;
    public const int EKE_FU = 12;
    public const int EKE_SHI_AMI = 13;
    public const int EKE_LORD = 14;
    public const int KETEMCOL = 15;
    public const int MAHUKIN = 16;
    public const int DRONGO = 17;
    public const int MAHEKET = 18;
    public const int CALUTE = 19;
    public const int ETALKET = 20;
    public const int MUATEM = 21;
    public const int DRAK = 22;
    public const int ZERRKU = 23;
    public const int GOKIN = 24;
    public const int TAJAQAR = 25;
    public const int TAJAERO = 26;
    public const int TERRA_QUAL = 27;
    public const int DUALE = 28;

    public int xp;
    public bool xp_taken = false;
    //public bool placed = false;
    public int max_health;
    public int health;

    private Slot target = null; // player unit to be moved towards and attacked

    protected void init(string name, int att, int hp, int xp, int style, int atr1, int atr2, int atr3) {
        create_attribute_list(num_attributes);
        type = ENEMY;
        this.name = name;
        attack_dmg = att;
        max_health = hp;
        health = hp;
        this.xp = xp;
        combat_style = style;
        attack_range = style == MELEE ? 1 : 9;


        if (atr1 >= 0)
            attributes[atr1] = true;
        if (atr2 >= 0)
            attributes[atr2] = true;
        if (atr3 >= 0)
            attributes[atr3] = true;
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

    public bool attempt_move(Slot end) {
        if (!in_range(1, slot.col, slot.row, end.col, end.row))
            return false;
        move(end);
        return true;
    }

    public bool can_target(Slot punit) {
        if (punit.get_unit() == null) 
            return false;

        bool melee_vs_flying = combat_style == Unit.MELEE && 
                                punit.get_unit().attributes[Unit.FLYING];
        if (melee_vs_flying) 
            return false; 
        return true;
    }

    public override int take_damage(int dmg) {
        health -= dmg;
        slot.update_healthbar(health);

        if (health <= 0) {
            dead = true;
            slot.show_dead();
        }
        return health <= 0 ? DEAD : INJURED;
    }

    public override int calc_dmg_taken(int dmg) {
        // adjust for defensive attributes?
        int final_dmg = dmg;
        return final_dmg > 0 ? final_dmg : 0;
    }

    public override float calc_hp_remaining(int dmg) {
        float damaged_hp = health - dmg;
        return damaged_hp;
    }

    public override int get_post_dmg_state(int dmg) {
        if (health - dmg <= 0)
            return DEAD;
        else 
            return INJURED;
    }

    public void clear_target() {
        target = null;
    }

    public void set_target(Slot punit) {
        target = punit;
    }
    public Slot get_target() {
        return target;
    }

    public int take_xp_from_death() {
        return xp_taken ? -1 : xp;
    }
}


public class Galtsa : Enemy {
    public Galtsa() {
        ID = GALTSA;
        init("Galtsa", 2, 2, 2, MELEE, CHARGE, GROUPING_3, 0);
    }
}
public class Grem : Enemy {
    public Grem() {
        ID = GREM;
        init("Grem", 1, 1, 1, MELEE, 0, 0, 0);
    }
}
public class Endu : Enemy {
    public Endu() {
        ID = ENDU;
        init("Endu", 4, 2, 3, MELEE, CHARGE, 0, 0);
    }
}
public class Korote : Enemy {
    public Korote() {
        ID = KOROTE;
        init("Korote", 1, 2, 2, MELEE, FLANKING, GROUPING_2, 0);
    }
}
public class Molner : Enemy {
    public Molner() {
        ID = MOLNER;
        init("Molner", 2, 2, 2, MELEE, FLANKING, GROUPING_4, CHARGE);
    }
}
public class Etuena : Enemy {
    public Etuena() {
        ID = ETUENA;
        init("Etuena", 2, 3, 3, MELEE, FLYING, CHARGE, GROUPING_2);
    }
}
public class Clypte : Enemy {
    public Clypte() {
        ID = CLYPTE;
        init("Clypte", 3, 5, 3, MELEE, TARGET_RANGE, 0, 0);
    }
}
public class Goliath : Enemy {
    public Goliath() {
        ID = GOLIATH;
        init("Goliath", 12, 8, 6, MELEE, TERROR_3, CHARGE, 0);
    }
}
public class Kverm : Enemy {
    public Kverm() {
        ID = KVERM;
        init("Kverm", 2, 1, 1, MELEE, STALK, 0, 0);
    }
}
public class Latu : Enemy {
    public Latu() {
        ID = LATU;
        init("Latu", 3, 3, 3, MELEE, STALK, AGGRESSIVE, 0);
    }
}
public class Eke_tu : Enemy {
    public Eke_tu() {
        ID = EKE_TU;
        init("Eke Tu", 1, 2, 2, MELEE, TARGET_RANGE, AGGRESSIVE, 0);
    }
}
public class Oetem : Enemy {
    public Oetem() {
        ID = OETEM;
        init("Oetem", 4, 3, 3, MELEE, GROUPING_3, 0, 0);
    }
}
public class Eke_fu : Enemy {
    public Eke_fu() {
        ID = EKE_FU;
        init("Eke Fu", 3, 2, 2, MELEE, GROUPING_4, FLANKING, 0);
    }
}
public class Eke_shi_ami : Enemy {
    public Eke_shi_ami() {
        ID = EKE_SHI_AMI;
        init("Eke Shi Ami", 3, 5, 4, MELEE, PIERCING_BOLT, STUN, TARGET_HEAVY);
    }
}
public class Eke_Lord : Enemy {
    public Eke_Lord() {
        ID = EKE_LORD;
        init("Eke Lord", 6, 12, 12, MELEE, ARCING_STRIKE, STUN, TARGET_HEAVY);
    }
}
public class Ketemcol : Enemy {
    public Ketemcol() {
        ID = KETEMCOL;
        init("Ketemcol", 2, 8, 6, MELEE, ARCING_STRIKE, STUN, ARMOR_1);
    }
}
public class Mahukin : Enemy {
    public Mahukin() {
        ID = MAHUKIN;
        init("Mahukin", 2, 3, 4, MELEE, ARMOR_2, GROUPING_2, 0);
    }
}
public class Drongo : Enemy {
    public Drongo() {
        ID = DRONGO;
        init("Drongo", 3, 6, 6, MELEE, ARMOR_3, 0, 0);
    }
}
public class Maheket : Enemy {
    public Maheket() {
        ID = MAHEKET;
        init("Maheket", 3, 3, 5, MELEE, ARMOR_2, GROUPING_3, 0);
    }
}
public class Calute : Enemy {
    public Calute() {
        ID = CALUTE;
        init("Calute", 6, 6, 5, MELEE, STALK, AGGRESSIVE, 0);
    }
}
public class Etalket : Enemy {
    public Etalket() {
        ID = ETALKET;
        init("Etalket", 2, 5, 4, MELEE, STALK, TERROR_3, 0);
    }
}
public class Muatem : Enemy {
    public Muatem() {
        ID = MUATEM;
        init("Muatem", 7, 4, 12, MELEE, ARMOR_5, DEVASTATING_BLOW, 0);
    }
}
public class Drak : Enemy {
    public Drak() {
        ID = DRAK;
        init("Drak", 3, 5, 3, MELEE, TARGET_CENTERFOLD, TERROR_2, FLYING);
    }
}
public class Zerrku : Enemy {
    public Zerrku() {
        ID = ZERRKU;
        init("Zerrku", 3, 3, 4, MELEE, GROUPING_2, 0, 0);
    }
}
public class Gokin : Enemy {
    public Gokin() {
        ID = GOKIN;
        init("Gokin", 2, 2, 2, MELEE, FLANKING, GROUPING_3, 0);
    }
}
public class Tajaqar : Enemy {
    public Tajaqar() {
        ID = TAJAQAR;
        init("Tajaqar", 3, 3, 5, MELEE, FLANKING, ARMOR_1, GROUPING_2);
    }
}
public class Tajaero : Enemy {
    public Tajaero() {
        ID = TAJAERO;
        init("Tajaero", 3, 2, 4, MELEE, FLYING, 0, 0);
    }
}
public class Terra_Qual : Enemy {
    public Terra_Qual() {
        ID = TERRA_QUAL;
        init("Terra Qual", 5, 10, 12, MELEE, 0, ARCING_STRIKE, ARMOR_2);
    }
}
public class Duale : Enemy {
    public Duale() {
        ID = DUALE;
        init("Duale", 2, 6, 5, MELEE, AGGRESSIVE, FLANKING, 0);
    }
}