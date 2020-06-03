using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementDragger : EventTrigger {
    private bool dragging;

    public void Update() {
        if (dragging) {
            Vector3 pos = (Input.mousePosition);
            transform.position = new Vector3(pos.x, pos.y, 0);
        }
    }

    public override void OnPointerDown(PointerEventData eventData) {
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData) {
        dragging = false;
    }
}
