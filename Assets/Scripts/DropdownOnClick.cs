using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownOnClick : MonoBehaviour, IPointerUpHandler {
    public void OnPointerUp(PointerEventData data) {
        EquipmentUI.I.selecting = true;
    }

}
