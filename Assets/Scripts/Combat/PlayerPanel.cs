using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : UnitPanel {
    Controller c;
    AttackQueuer aq;
        // Player exclusive
    public Button returnB;
    public Button moveB;
    public Button attackB;
    public Button defB;
    public Button upB, downB, leftB, rightB;

    public Text DefT;
    public Text ResT;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        aq = c.attack_queuer;
    }

    /* 
    Limit what a player can do with a unit based on game logic.
    */
    public override void update_panel(Slot slot) {
        if (slot.get_enemy() != null)
            return;

        PlayerUnit punit = slot.get_punit();
        update_text(punit);
        disable_attackB();
        disable_defB();
        disable_moveB();
        disable_returnB();
        disable_rotateB();

        bool ranging = bp.range_stage && punit.is_range();
        bool combat_staging = (bp.range_stage && punit.is_range()) || 
                                (bp.combat_stage);


        if (!punit.has_moved && !punit.has_acted_in_stage && 
            (bp.movement_stage || ranging)) {
            enable_moveB();
            enable_rotateB();
        } else if (bp.init_placement_stage) {
            enable_returnB();
        } 

        bool is_first_slot_in_group = slot.get_group().get_highest_player_slot() == slot;
        if (!punit.has_acted_in_stage && !punit.has_acted && 
            combat_staging && is_first_slot_in_group) {
            attackB.interactable = true;
            defB.interactable = true;
        }
    
        attB_pressed = punit.attack_set;
        defB_pressed = punit.defending;
        moveB_pressed = false;
    }

    public void attack() {
        if (!bp.targeting)
            return;
        attB_pressed = !attB_pressed;
    }

    public void defend() {
        if (!bp.targeting)
            return;
        defB_pressed = !defB_pressed;
    }

    public void move() {
        moveB_pressed = !moveB_pressed;
    }

    // If any one of the following buttons are pressed,
    // the others will then not be.
    private bool _attB_pressed = false;
    public bool attB_pressed {
        get { return _attB_pressed; }
        set {
            if (_attB_pressed == value)
                return;
            _attB_pressed = value;
            set_press_attackB(value);
            if (_attB_pressed) {
                defB_pressed = false;
                moveB_pressed = false;
            } else {
                if (slot.get_punit().attack_set) {
                    aq.get_player_queue().remove_attack(slot.get_punit().attack_id, c.line_drawer);
                } 
            }
            // Disable the attack no matter the presssed state.
            selector.selecting_target = _attB_pressed;
        }
    }
    private bool _defB_pressed = false;
    public bool defB_pressed {
        get { return _defB_pressed; }
        set {
            _defB_pressed = value;
            set_press_defB(value);
            if (_defB_pressed) {
                attB_pressed = false;
                moveB_pressed = false;
            } 
            slot.get_punit().defending = defB_pressed;
        }
    }
    private bool _moveB_pressed = false;
    public bool moveB_pressed {
        get { return _moveB_pressed; }
        set {
            _moveB_pressed = value; 
            set_press_moveB(value);
            if (_moveB_pressed) {
                attB_pressed = false;
                defB_pressed = false;
            } 
            selector.selecting_move = moveB_pressed;
        }
    }

    private void update_text(PlayerUnit punit) {
        set_name(punit.get_name());
        ResT.text = punit.resilience.ToString();
        AttT.text = punit.attack_dmg.ToString();
        DefT.text = punit.defense.ToString();
    }

    private void set_press_attackB(bool pressed) {
        if (pressed)
            attackB.image.color = Controller.GREY;
        else
            attackB.image.color = Color.white;
    }

    private void set_press_defB(bool pressed) {
        if (pressed)
            defB.image.color = Controller.GREY;
        else
            defB.image.color = Color.white;
    }

    private void set_press_moveB(bool pressed) {
        if (pressed)
            moveB.image.color = Controller.GREY;
        else
            moveB.image.color = Color.white;
    }

    public override void close() {
        base.close();
        disable_returnB();
    }

    // TOGGLES
    private void enable_rotateB() {
        upB.interactable = true;
        downB.interactable = true;
        leftB.interactable = true;
        rightB.interactable = true;
    }

    private void disable_rotateB() {
        upB.interactable = false;
        downB.interactable = false;
        leftB.interactable = false;
        rightB.interactable = false;
    }
    private void enable_returnB() {
        returnB.interactable = true;
    }

    private void disable_returnB() {
        returnB.interactable = false;
    }

    private void enable_attackB() {
        attackB.interactable = true;
    }

    private void disable_attackB() {
        attackB.interactable = false;
    }

    private void enable_defB() {
        defB.interactable = true;
    }

    private void disable_defB() {
        defB.interactable = false;
    }

    private void enable_moveB() {
        moveB.interactable = true;
    }

    private void disable_moveB() {
        moveB.interactable = false;
    }
}