using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapCellUI : MonoBehaviour {
    public Text cell_typeT, enemy_countT, star_crystalsT;
    public Button scoutB, teleportB, moveB;
    private Map map;
    private MapCell cell;
    public GameObject parent;


    void init(string cell_name, int enemy_count, bool can_teleport_to, bool discovered) {
        cell_typeT.text = cell_name;
        enemy_countT.text = build_enemy_countT(enemy_count, discovered);
        enable_button(teleportB, can_teleport_to);
    }

    public void init(Map map, MapCell cell) {
        this.map = map;
        this.cell = cell;
        cell_typeT.text = cell.discovered ? cell.name : "Unknown";
        enemy_countT.text = build_enemy_countT(cell.get_enemies().Count, cell.discovered);
        star_crystalsT.text = map.c.get_disc().get_var(Storeable.STAR_CRYSTALS).ToString();

        Vector3 pos = new Vector3(cell.pos.x, cell.pos.y, 0);
        //transform.position =
            //map.c.cam_switcher.mapCam.WorldToScreenPoint(new Vector3(pos.x + 0.5f, pos.y - 2.5f, 0));
            transform.position = new Vector3(pos.x + 0.5f, pos.y - 2.5f, 0); // camera mode, not overlay

        enable_button(moveB, map.can_move(pos));
        enable_button(scoutB, map.can_scout(pos));
        enable_button(teleportB, map.can_teleport(pos));
    }

    private string build_enemy_countT(int enemy_count, bool discovered) {
        if (enemy_count > 0 && discovered) {
            return enemy_count + " enemies have been revealed sulking in the darkness.";
        } else if (discovered) {
            return "This land is free from darkness.";
        } else {
            return "We know not what waits in the darkness.";
        }
    }

    public void move() {
        float randx = Random.Range(0.4f, 0.6f); // simulate human placement, prevent perfect overlap
        float randy = Random.Range(0.4f, 0.6f);
        map.move_player(new Vector3(cell.pos.x + randx, cell.pos.y + randy, 0), true);
        close();
    }

    public void close() {
        map.open_cell_UI_script = null;
        Destroy(gameObject);
    }

    public void scout() {
        map.scout(new Vector3(cell.pos.x, cell.pos.y, 0));
        enable_button(scoutB, false);
        close();
    }

    public void teleport() {
        map.move_player(new Vector3(cell.pos.x, cell.pos.y, 0), true);
        close();
    }

    private void enable_button(Button b, bool state) {
        b.interactable = state;
    }
}
