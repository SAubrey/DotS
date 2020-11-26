using System.Collections.Generic;
using TMPro;

public static class AttributeWriter {

    public static string GROUPING_DESC = 
    "Active. Unit's attack or defense increases by 1 for each unit in the same group up to the attribute level.";
    public static string TERROR_DESC = "";
    public static string HEAL_DESC = "Active. Select an injured unit from the selection bar for placement in a green reserve group.";

    private static Dictionary<int, string> descriptions 
        = new Dictionary<int, string>() {
            {Unit.FLANKING, "Flanking - Unit spawns to the side of your battalion."},
            {Unit.FLYING, "Flying - Unit cannot be hit by melee units that do not have Reach."},
            {Unit.GROUPING_1, "Grouping 1 - " + GROUPING_DESC},
            {Unit.GROUPING_2, "Grouping 2 - " + GROUPING_DESC},
            {Unit.STALK, "Stalk - Unit spawns behind your battalion."},
            {Unit.PIERCING, "Piercing - Passive. Attacks ignore defense."},
            {Unit.ARCING_STRIKE, "Arcing Strike - Unit’s attack sweeps over multiple groups."},
            {Unit.AGGRESSIVE, "Aggressive - Unit has an additional action per combat phase."},
            {Unit.TERROR_1, "Terror 1 - " + TERROR_DESC},
            {Unit.TERROR_2, "Terror 2 - " + TERROR_DESC},
            {Unit.TERROR_3, "Terror 3 - " + TERROR_DESC},
            {Unit.TARGET_RANGE, "Target Range - "},
            {Unit.TARGET_HEAVY, "Target Heavy - "},
            {Unit.STUN, "Stun - Removes one action from the target while doing half damage."},
            {Unit.WEAKNESS_POLEARM, "Polearm Weakness - "},
            {Unit.CRUSHING_BLOW, "Crushing Blow - Damage overflows from a killed unit to the next unit in the group."},
            {Unit.REACH, "Reach - Passive. Unit can attack diagonally."},
            {Unit.CHARGE, "Charge - Unit can attack immediately after battle begins."},
            {Unit.INSPIRE, "Inspire - Active. Increases resistance by one for any units in the three horizontal groups in front of this unit."},
            {Unit.HARVEST, "Harvest - This unit can mine resources from certain biomes."},
            {Unit.COUNTER_CHARGE, "Counter Charge - Unit inflicts double damage against charging units."},
            {Unit.BOLSTER, "Bolster - Active. "},
            {Unit.TRUE_SIGHT, "True Sight - Unit will reveal or unlock travel card features."},
            {Unit.HEAL_1, "Heal 1 - " + HEAL_DESC},
            {Unit.COMBINED_EFFORT, "Combined Effort - "},
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
