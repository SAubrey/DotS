using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class AttributeWriter {

    public static string GROUPING_DESC = 
    "Active. Unit's attack or defense increases by 1 for each unit in the same group up to the attribute level.";
    public static string TERROR_DESC = "";
    public static string HEAL_DESC = "";

    private static Dictionary<int, string> descriptions 
        = new Dictionary<int, string>() {
            {Unit.FLANKING, "Flanking - "},
            {Unit.FLYING, "Flying - "},
            {Unit.GROUPING_1, "Grouping 1 - " + GROUPING_DESC},
            {Unit.GROUPING_2, "Grouping 2 - " + GROUPING_DESC},
            {Unit.STALK, "Stalk - "},
            {Unit.PIERCING, "Piercing - Passive. Attacks ignore defense."},
            {Unit.ARCING_STRIKE, "Arcing Strike - "},
            {Unit.AGGRESSIVE, "Aggressive - "},
            {Unit.TERROR_1, "Terror 1 - " + TERROR_DESC},
            {Unit.TERROR_2, "Terror 2 - " + TERROR_DESC},
            {Unit.TERROR_3, "Terror 3 - " + TERROR_DESC},
            {Unit.TARGET_RANGE, "Target Range - "},
            {Unit.TARGET_HEAVY, "Target Heavy - "},
            {Unit.STUN, "Stun - "},
            {Unit.WEAKNESS_POLEARM, "Polearm Weakness - "},
            {Unit.CRUSHING_BLOW, "Crushing Blow - "},
            {Unit.TARGET_CENTERFOLD, "Target Centerfold - "},
            {Unit.REACH, "Reach - "},
            {Unit.CHARGE, "Charge - Unit can attack immediately after battle begins."},
            {Unit.INSPIRE, "Inspire - Active. Increases resistance by one for any units in the three horizontal groups in front of this unit."},
            {Unit.HARVEST, "Harvest - This unit can mine resources from certain biomes."},
            {Unit.COUNTER_CHARGE, "Counter Charge - Unit inflicts double damage against charging units."},
            {Unit.BOLSTER, "Bolster - Active. "},
            {Unit.TRUE_SIGHT, "True Sight -"},
            {Unit.HEAL_1, "Heal 1 - " + HEAL_DESC},
            {Unit.COMBINED_EFFORT, "Combined Effort - "},
        };

    public static void write_attribute_text(Text text, Unit u) {
        text.text = get_description(u.attribute1) + "\n" +
                    get_description(u.attribute2) + "\n" +
                    get_description(u.attribute3);
    }

    public static string get_description(int attribute) {
        string s;
        descriptions.TryGetValue(attribute, out s);
        Debug.Log("tryget return:" + s);
        return s != null ? s : "";
    }
}
