using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour {
    private Unit unit;
    public Image img;
    public Controller c;
    private Formation f;
    public GameObject unit_panel;
    private Camera cam;
    public Text namefg_T;
    public Text namebg_T;

    public int col; // LEFT, MID, RIGHT
    public int row; // FRONT, MID, REAR
    public int num; // Hierarchy in group. 0, 1, 2
    private Group group;
    
    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();

        namebg_T.transform.LookAt(cam.transform);
        namebg_T.transform.forward = namefg_T.transform.forward * -1;

        f = c.formation;
        //selector = c.selector;
    }

    public void click() {
        c.selector.handle_slot(this);
    }

    public bool fill(Unit u) {
        if (u == null)
            return false;
        set_unit(u);
        set_sprite(u.get_ID());
        return true;
    }

    // Full slots below the removed slot will be moved up. 
    public void empty() {
        if (unit != null) {
            unit.set_slot(null);
            unit = null;
        }
        set_sprite(PlayerUnit.EMPTY);
        set_namefg_T("");
        show_no_selection();
        group.validate_unit_order();
    }

    public void empty_without_validation() {
        if (unit != null) {
            unit.set_slot(null);
            unit = null;
        }
        set_sprite(PlayerUnit.EMPTY);
        set_namefg_T("");
        show_no_selection();
    }

    private void set_unit(Unit u) {
        if (u == null) {
            Debug.Log("not setting null unit");
            return;
        }
        if (u.is_playerunit()) {
            unit = u as PlayerUnit;
        } else if (u.is_enemy()) {
            unit = u as Enemy;
        }
        set_namefg_T(unit.get_name());
        unit.set_slot(this);
    }

    public Unit get_unit() {
        if (has_enemy()) {
            return get_enemy();
        } else if (has_punit()) {
            return get_punit();
        } 
        return null;
    }

    public void set_sprite(int image_ID) { 
        if (has_punit()) {
            img.sprite = f.images[image_ID]; 
        } else if (has_enemy()) {
            img.sprite = c.enemy_loader.images[image_ID];
        } else {
            img.sprite = f.images[image_ID];
        }
    }

    public void show_selection() {
        if (img != null)
            img.color = Color.gray;
    }

    public void show_no_selection() {
        if (img != null)
            img.color =  Color.white;
    }

    public void show_dead() {
        if (img != null)
            img.color = Color.red;
    }

    public void show_injured() {
        if (img != null)
            img.color = Color.yellow;
    }

    public void show_offensive() {
        if (img != null)
            img.color = new Color(1, 0.2f, 0.2f, 1);
    }

    public void show_defensive() {
        if (img != null)
            img.color = Color.blue;
    }

    public bool has_punit() {
        if (unit == null) return false;
        if (unit.is_playerunit()) return true;
        return false;
    }

    public bool has_enemy() {
        if (unit == null) return false;
        if (unit.is_enemy()) return true;
        return false;
    }

    public bool has_unit() {
        return unit != null ? true : false;
    }

    public bool is_empty() {
        return unit == null ? true : false;
    }

    public PlayerUnit get_punit() {
        return has_punit() ? unit as PlayerUnit : null;
    }

    public Enemy get_enemy() {
        return has_enemy() ? unit as Enemy : null;
    }

    private void set_namefg_T(string txt) {
        namefg_T.text = txt;
        namebg_T.text = txt;
    }

    public Sprite get_sprite() { return img.sprite; }

    public Group get_group() {
        return group;
    }

    public void set_group(Group g) {
        group = g;
        g.add_slot(this);
    }
}
