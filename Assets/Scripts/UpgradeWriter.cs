using System.Collections.Generic;
using TMPro;

public static class UpgradeWriter {
    public static string UNIT_UPGRADE = "Unlocks access to more unit types and other research tree upgrades.";
    private static Dictionary<int, string> descriptions 
        = new Dictionary<int, string>() {
        {CityUI.TEMPLE, UNIT_UPGRADE},
        {CityUI.TEMPLE2, UNIT_UPGRADE},
        {CityUI.CITADEL, UNIT_UPGRADE},
        {CityUI.SHARED_WISDOM, "Increases the max unity of each battalion by 10."},
        {CityUI.MEDITATION, ""},
        {CityUI.SANCTUARY, ""},
        {CityUI.TEMPLE3, UNIT_UPGRADE},
        {CityUI.HALLOFADEPT, "Allows the Astra Discipline player to recruit unique Astra-only units to their battalion."},
        {CityUI.FAITHFUL, "Unlocks the heart of the Astra practice, Unity.\nIncreases the max unity of each battalion by 10.\n" +
            "Enemies encountered with the Terror attribute will increase unity by 3."}, // Unity decreases after?
        {CityUI.RUNE_PORT, "Rebuilds the ancient rune port allowing transport to any discovered rune gates."},
        {CityUI.CITADEL2, UNIT_UPGRADE},

        {CityUI.CRAFT_SHOP, UNIT_UPGRADE},
        {CityUI.CRAFT_SHOP2, UNIT_UPGRADE},
        {CityUI.STOREHOUSE, "Increases the capacity of the city inventory to 108."},
        {CityUI.REFINED_STARDUST, "The light of each battalion can hold out 5 turns before using a star crystal instead of 4."},
        {CityUI.ENCAMPMENTS, ""},
        {CityUI.STABLE, "Allows the city to store 10 equimares."},
        {CityUI.CRAFT_SHOP3, UNIT_UPGRADE},
        {CityUI.MASTERS_GUILD, "Allows the Endura Discipline to recruit unique Endura-only units to their battalion."},
        {CityUI.RESILIENT, ""},
        {CityUI.RESTORE_GREAT_TORCH, "Restores the great Torch of Ayetzu to its former glory." + 
            "The light of each battalion can hold out 5 turns before using a star crystal instead of 4."},
        {CityUI.STOREHOUSE2, "Increases the capacity of the city inventory to 144."},

        {CityUI.FORGE, UNIT_UPGRADE},
        {CityUI.FORGE2, UNIT_UPGRADE},
        {CityUI.BARRACKS, UNIT_UPGRADE},
        {CityUI.MARTIAL_ORDER, ""},
        {CityUI.STEADY_MARCH, ""},
        {CityUI.GARRISON, ""},
        {CityUI.FORGE3, UNIT_UPGRADE},
        {CityUI.DOJO_CHOSEN, "Allows the Martial Discipline to recruit unique Martial-only units to their battalion."},
        {CityUI.REFINED, "Unlocks the heart of the Endura practice, Resilience. Resilience of all units is increased by 2."},
        {CityUI.BOW_ILUHATAR, ""},
        {CityUI.BARRACKS2, UNIT_UPGRADE},  
        };

    public static void write_attribute_text(TextMeshProUGUI text, int upgrade_ID) {
        text.text += get_description(upgrade_ID) + " \n";
    }

    public static string get_description(int upgrade_ID) {
        string s;
        descriptions.TryGetValue(upgrade_ID, out s);
        return s != null ? s : "";
    }
}
