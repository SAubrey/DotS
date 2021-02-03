using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class DropdownItemOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public void OnPointerEnter(PointerEventData data) {
        GameObject go = data.pointerEnter;
        EquipmentUI.I.load_equipment_description(go.GetComponent<TMP_Text>().text);
    }

    public void OnPointerExit(PointerEventData data) {
        EquipmentUI.I.hide_equipment_descriptionP();
    }
}
