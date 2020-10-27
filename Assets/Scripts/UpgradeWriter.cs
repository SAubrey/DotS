using System.Collections.Generic;
using TMPro;

public static class UpgradeWriter {
    public static string UNIT_UPGRADE = "Unlocks access to more unit types and other research tree upgrades.";
    private static Dictionary<int, string> descriptions 
        = new Dictionary<int, string>() {
        {CityUIManager.TEMPLE, UNIT_UPGRADE},
        {CityUIManager.TEMPLE2, UNIT_UPGRADE},
        {CityUIManager.CITADEL, UNIT_UPGRADE},
        {CityUIManager.SHARED_WISDOM, "Increases the max unity of each battalion by 10."},
        {CityUIManager.MEDITATION, ""},
        {CityUIManager.SANCTUARY, ""},
        {CityUIManager.TEMPLE3, UNIT_UPGRADE},
        {CityUIManager.HALLOFADEPT, "Allows the Astra Discipline player to recruit unique Astra only units to their battalion."},
        {CityUIManager.FAITHFUL, "Unlocks the heart of the Astra practice, Unity. Increases the max unity of each battalion by 10." +
            "Enemies encountered with the Terror attribute will increase unity by 3."}, // Unity decreases after?
        {CityUIManager.RUNE_PORT, "Rebuilds the ancient rune port allowing transport to any discovered rune gates."},
        {CityUIManager.CITADEL2, UNIT_UPGRADE},

        {CityUIManager.CRAFT_SHOP, UNIT_UPGRADE},
        {CityUIManager.CRAFT_SHOP2, UNIT_UPGRADE},
        {CityUIManager.STOREHOUSE, "Increases the capacity of the city inventory to 108."},
        {CityUIManager.REFINED_STARDUST, "The light of each battalion can hold out 5 turns before using a star crystal instead of 4."},
        {CityUIManager.ENCAMPMENTS, ""},
        {CityUIManager.STABLE, "Allows the city to store 10 equimares."},
        {CityUIManager.CRAFT_SHOP3, UNIT_UPGRADE},
        {CityUIManager.MASTERS_GUILD, "Allows the Astra Discipline to recruit unique Endura only units to their battalion."},
        {CityUIManager.RESILIENT, ""},
        {CityUIManager.RESTORE_GREAT_TORCH, "Restores the great Torch of Ayetzu to its former glory." + 
            "The light of each battalion can hold out 5 turns before using a star crystal instead of 4."},
        {CityUIManager.STOREHOUSE2, "Increases the capacity of the city inventory to 144."},

        {CityUIManager.FORGE, UNIT_UPGRADE},
        {CityUIManager.FORGE2, UNIT_UPGRADE},
        {CityUIManager.BARRACKS, UNIT_UPGRADE},
        {CityUIManager.MARTIAL_ORDER, ""},
        {CityUIManager.STEADY_MARCH, ""},
        {CityUIManager.GARRISON, ""},
        {CityUIManager.FORGE3, UNIT_UPGRADE},
        {CityUIManager.DOJO_CHOSEN, ""},
        {CityUIManager.REFINED, "Unlocks the heart of the Endura practice, Resilience. Resilience of all units is increased by 2."},
        {CityUIManager.BOW_ILUHATAR, ""},
        {CityUIManager.BARRACKS2, UNIT_UPGRADE},  
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
