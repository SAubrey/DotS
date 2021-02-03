using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EquipmentUI : MonoBehaviour {
    public static EquipmentUI I { get; private set; }
    public TMP_Dropdown dd0, dd1, dd2, dd3;
    public GameObject descriptionP;
    public TMP_Text descriptionT;
    public Dictionary<TMP_Dropdown, int> dropdowns = new Dictionary<TMP_Dropdown, int>();
    //public Dictionary<TMP_Dropdown, string> selected_equipment = new Dictionary<TMP_Dropdown, string>();
    
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        dropdowns.Add(dd0, 0);
        dropdowns.Add(dd1, 1);
        dropdowns.Add(dd2, 2);
        dropdowns.Add(dd3, 3);
        foreach (TMP_Dropdown d in dropdowns.Keys) {
            d.onValueChanged.AddListener(delegate { equipment_selected(d); });
            //d.OnPointerEnter();//AddListener(delegate {load_equipment_description(d);} );
            
            
        }
        TurnPhaser.I.on_disc_change += load_discipline;
        foreach (Discipline d in Controller.I.discs.Values) {
            d.on_resource_change += register_resource_change;
        }
    }

    public void init(Discipline d) {
        load_discipline(d);
    }

    public void register_resource_change(int ID, string field, int amount, int c, int cap) {
        if (ID != TurnPhaser.I.active_disc_ID)
            return;
        if (field == Discipline.EXPERIENCE) {
            unlock_slots(Controller.I.get_disc(ID).equipment_inventory);
        }
    }

    public bool selecting = false;
    public void equipment_selected(TMP_Dropdown d) {
        selecting = true;
        string e = d.captionText.text;
        
        EquipmentInventory ei = TurnPhaser.I.active_disc.equipment_inventory;
        // Undo the selection, the same equipment cannot be worn more than once,
        // and only one of a type can be worn at once.
        if (ei.has_equipped(e) || ei.get_equipment_amount(e) > 1) {
            if (ei.equipment_slots[dropdowns[d]].full) {
                string name = ei.equipment_slots[dropdowns[d]].equipment.name;
                d.value = d.options.FindIndex(option => option.text == name);
            } else {
                d.value = d.options.FindIndex(option => option.text == "Empty");
            }
            return;
        }
        // Unequip existing worn item.
        if (ei.equipment_slots[dropdowns[d]].full) {
            ei.unequip(dropdowns[d]);
        }
        hide_equipment_descriptionP();
        ei.equip(e, dropdowns[d]);
    }

    public void load_discipline(Discipline disc) {
        EquipmentInventory ei = disc.equipment_inventory;
        unlock_slots(ei);
        fill_dropdowns(ei);
    }

    public void unlock_slots(EquipmentInventory ei) {
        if (ei == null) 
            return;
        int highest_unlocked_slot = ei.get_highest_unlocked_slot();

        foreach (TMP_Dropdown d in dropdowns.Keys) {
            activate_equipment_slot(d, dropdowns[d] <= highest_unlocked_slot);
        }
    }

    public void fill_dropdowns(EquipmentInventory ei) {
        foreach (TMP_Dropdown d in dropdowns.Keys) {
            fill_dropdown(d, ei);
            select_equipped(ei);
        }
    }

    private void fill_dropdown(TMP_Dropdown dropdown, EquipmentInventory ei) {
        clear_dropdown_options(dropdown);
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (string e in ei.equipment.Keys) {
            if (ei.equipment[e].Count > 1) {
                options.Add(new TMP_Dropdown.OptionData(ei.equipment[e].Count.ToString() + " " + ei.equipment[e][0].name));
            } else {
                options.Add(new TMP_Dropdown.OptionData(ei.equipment[e][0].name));
            }                
        }
        dropdown.AddOptions(options);
    }

    private void select_equipped(EquipmentInventory ei) {
        foreach (TMP_Dropdown d in dropdowns.Keys) {
            EquipmentSlot es = ei.equipment_slots[dropdowns[d]];
            if (!es.full)
                continue;
            d.captionText.text = es.equipment.name;
        }
    }

    private void clear_dropdown_options(TMP_Dropdown dropdown) {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("Empty") });
    }

    public void load_equipment_description(EquipmentInventory ei, string e) {
        if (!ei.has_equipped(e))
            return;
        descriptionP.SetActive(true);
        descriptionT.text = ei.equipment[e][0].description;
    }

    public void load_equipment_description(string e) {
        if (e == "Empty")
            return;
        EquipmentInventory ei = TurnPhaser.I.active_disc.equipment_inventory;
        if (!ei.has(e))
            return;
        descriptionP.SetActive(true);
        descriptionT.text = ei.equipment[e][0].description;
    }

    public void hide_equipment_descriptionP() {
        descriptionP.SetActive(false);
    }

    private void activate_equipment_slot(TMP_Dropdown d, bool state) {
        d.interactable = state;
    }
}
