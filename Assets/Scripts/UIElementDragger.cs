using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementDragger : EventTrigger {

    private bool dragging;
    private Camera cam;

    void Start() {
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
    }
    public void Update() {
        if (dragging) {
            Vector3 pos = cam.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            transform.position = new Vector2(pos.x, pos.y);
        }
    }

    public override void OnPointerDown(PointerEventData eventData) {
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData) {
        dragging = false;
    }
}
