using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewLine : MonoBehaviour{
    // Preview line is only for player attack selection.
    public LineRenderer lr = new LineRenderer();
    private Vector3 start_pos;
    private bool drawing = false;
    public Camera battle_cam;

    public void init() {
        lr.sortingLayerName = "Top";
        lr.positionCount = 2;
        lr.startWidth = 20f;
        lr.endWidth = 1f;
        lr.startColor = new Color(1, 1, 1, 0);
        lr.endColor = Color.white;
        lr.useWorldSpace = true;
    }

    void Update() {
        if (!drawing)
            return;
        Vector3 pos = battle_cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, Mathf.Abs(battle_cam.transform.position.z)));
        lr.SetPosition(1, pos);
    }
    
    public void draw(Vector3 start_pos) {
        this.start_pos = start_pos;
        lr.endColor = Color.white;
        lr.SetPosition(0, start_pos);
        drawing = true;
    }

    public void erase() {
        drawing = false;
        lr.startColor = new Color(1, 1, 1, 0);
        lr.endColor = new Color(1, 1, 1, 0);
    }
}
