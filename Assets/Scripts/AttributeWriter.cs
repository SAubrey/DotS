using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class AttributeWriter {

    public static string GROUPING_DESC = 
    "Active. Unit's attack or defense increases by 1 for each unit in the same group up to the attribute level.";
    public static string TERROR_DESC = "";
    public static string HEAL_DESC = "Active. Select an injured unit from the selection bar for placement in a green reserve group.";

    private static Dictionary<int, string> descriptions 
        = new Dictionary<int, string>() {
            {Unit.FLANKING, "Flanking\nUnit spawns to the side of your battalion."},
            {Unit.FLYING, "Flying\nUnit cannot be hit by melee units that do not have Reach."},
            {Unit.GROUPING_1, "Grouping 1\n" + GROUPING_DESC},
            {Unit.GROUPING_2, "Grouping 2\n" + GROUPING_DESC},
            {Unit.STALK, "Stalk\nUnit spawns behind your battalion."},
            {Unit.PIERCING, "Piercing\nPassive. Attacks ignore defense."},
            {Unit.ARCING_STRIKE, "Arcing Strike\nUnit’s attack sweeps over multiple groups."},
            {Unit.AGGRESSIVE, "Aggressive\nUnit has an additional action per combat phase."},
            {Unit.TERROR_1, "Terror 1\n" + TERROR_DESC},
            {Unit.TERROR_2, "Terror 2\n" + TERROR_DESC},
            {Unit.TERROR_3, "Terror 3\n" + TERROR_DESC},
            {Unit.TARGET_RANGE, "Target Range\n"},
            {Unit.TARGET_HEAVY, "Target Heavy\n"},
            {Unit.STUN, "Stun\nRemoves one action from the target while doing half damage."},
            {Unit.WEAKNESS_POLEARM, "Polearm Weakness\n"},
            {Unit.CRUSHING_BLOW, "Crushing Blow\nDamage overflows from a killed unit to the next unit in the group."},
            {Unit.REACH, "Reach\nPassive. Unit can attack diagonally."},
            {Unit.CHARGE, "Charge\nUnit can attack immediately after battle begins."},
            {Unit.INSPIRE, "Inspire\nActive. Increases resistance by one for any units in the three horizontal groups in front of this unit."},
            {Unit.HARVEST, "Harvest\nThis unit can mine resources from certain biomes."},
            {Unit.COUNTER_CHARGE, "Counter Charge\nUnit inflicts double damage against charging units."},
            {Unit.BOLSTER, "Bolster\nActive. "},
            {Unit.TRUE_SIGHT, "True Sight\nUnit will reveal or unlock travel card features."},
            {Unit.HEAL_1, "Heal 1\n" + HEAL_DESC},
            {Unit.COMBINED_EFFORT, "Combined Effort\n"},
        };

    public static void write_attribute_text(TextMeshProUGUI text, Unit u) {
        text.text = get_description(u.attribute1) + "\n" +
                    get_description(u.attribute2) + "\n" +
                    get_description(u.attribute3);
    }

    public static string get_description(int attribute) {
        string s;
        descriptions.TryGetValue(attribute, out s);
        return s != null ? s : "";
    }
}
