using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {
    private Unit unit;
    public Image img;
    public Controller c;
    public Selector selector;
    private Formation f;
    public GameObject unit_panel;
    private Camera cam;
    private Text name_txt;

    public int col; // LEFT, MID, RIGHT
    public int row; // FRONT, MID, REAR
    public int num; // Hierarchy in group. 0, 1, 2
    private Group group;
    
    void Awake() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        cam = GameObject.Find("BattleCamera").GetComponent<Camera>();
        name_txt = GetComponentInChildren<Text>();
        name_txt.transform.LookAt(cam.transform);
        name_txt.transform.forward = name_txt.transform.forward * -1;
    }
    void Start() {
        f = c.formation;
        selector = c.selector;
        group = f.get_group(row, col);
        f.add_slot_to_group(this);
        
    }

    public void click() {
        selector.handle_slot(this);
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
        set_name_txt("");
        show_no_selection();
        group.validate_unit_order();
    }

    public void empty_without_validation() {
        if (unit != null) {
            unit.set_slot(null);
            unit = null;
        }
        set_sprite(PlayerUnit.EMPTY);
        set_name_txt("");
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
        set_name_txt(unit.get_name());
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

    private void set_name_txt(string txt) {
        name_txt.text = txt;
    }

    public Sprite get_sprite() { return img.sprite; }

    public Group get_group() {
        return group;
    }
}
