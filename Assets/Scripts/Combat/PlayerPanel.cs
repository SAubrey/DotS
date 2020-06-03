using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : UnitPanel {
    Controller c;
    AttackQueuer aq;
    public Button returnB;
    public Button moveB;
    public Button attackB;
    public Button defB;
    public Button attributeB;
    public Button upB, downB, leftB, rightB;

    public Text DefT;
    public Text ResT;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        aq = c.attack_queuer;
    }

    void Update() {
        if (!Input.anyKeyDown)
            return;
        
        // Unit actions requiring a selected unit.
        if (Input.GetKeyDown(KeyCode.A)) {
            if (attackB.interactable)
                attack();
        } else if (Input.GetKeyDown(KeyCode.D)) {
            if (defB.interactable)
                defend();
        } else if (Input.GetKeyDown(KeyCode.R)) {
            if (returnB.interactable)
                c.selector.return_unit();
        } else if (Input.GetKeyDown(KeyCode.S)) {
            if (moveB.interactable)
                move();
        }
        
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
        disable_attributeB();

        bool ranging = bp.range_stage && punit.is_range;
        bool combat_staging = (bp.range_stage && punit.is_range) || 
                                (bp.combat_stage);

        if (bp.movement_stage)
            enable_rotateB();
        if (!punit.out_of_actions && !punit.has_acted_in_stage && 
                (bp.movement_stage || ranging)) {
            enable_moveB();
        } else if (bp.init_placement_stage) {
            enable_returnB();
        } 

        bool is_first_slot_in_group = slot.get_group().get_highest_player_slot() == slot;
        if (!punit.has_acted_in_stage && !punit.out_of_actions && 
                combat_staging && is_first_slot_in_group) {
            if (punit.get_raw_attack_dmg() > 0)
                attackB.interactable = true;
            if (punit.get_raw_defense() > 0)                
                defB.interactable = true;
            if (punit.can_activate_attribute()) {
                attributeB.interactable = true;
            }
        }

        // Determine pressed buttons
        set_press_attackB(punit.attack_set);
        _attB_pressed = punit.attack_set;
        set_press_defB(punit.defending);
        _defB_pressed = punit.defending;
        //if (punit.attribute_requires_action && (attB_pressed || defB_pressed)) {
        set_press_attributeB(punit.is_attribute_active);
        _attributeB_pressed = punit.is_attribute_active;
        //} 
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

    // If successful, the button will stay pressed.
    public void attribute() {
        attributeB_pressed = !attributeB_pressed;
    }

    // If any one of the following buttons are pressed,
    // the others will then not be.
    private bool _attB_pressed = false;
    public bool attB_pressed {
        get { return _attB_pressed; }
        set {
            if (_attB_pressed == value) // Ignore if same state.
                return;
            _attB_pressed = value;
            set_press_attackB(value);
            if (_attB_pressed) {
                defB_pressed = false;
                moveB_pressed = false;
                if (punit.attribute_requires_action)
                    attributeB_pressed = false;
            } else {
                if (punit.attack_set) {
                    aq.get_player_queue().remove_attack(punit.attack_id, c.line_drawer);
                    //slot.show_attacking(false);
                } 
                if (attributeB_pressed && punit.attribute_requires_action) {
                    attributeB_pressed = false;
                }
            }
            // Disable the attack no matter the pressed state.
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
                if (punit.attribute_requires_action)
                    attributeB_pressed = false;
            } else if (attributeB_pressed && punit.attribute_requires_action) {
                attributeB_pressed = false;
            }
            punit.defending = defB_pressed;
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

    private bool _attributeB_pressed = false;
    public bool attributeB_pressed {
        get { return _attributeB_pressed; }
        set {
            if (punit.attribute_requires_action
                    && (!attB_pressed && !defB_pressed)) {
                _attributeB_pressed = false;
                set_press_attributeB(false);
                punit.set_attribute_active(false);
                return;
            }
            // Stay unpressed if the attribute cannot be activated.
            if (!punit.can_activate_attribute()) {
                _attributeB_pressed = false;
                set_press_attributeB(false);
                return;
            } 

            _attributeB_pressed = value;
            set_press_attributeB(value);
            punit.set_attribute_active(value);
        }
    }

    public void update_text(PlayerUnit punit) {
        set_name(punit.get_name());
        ResT.text = punit.get_slot().build_health_string();
        AttT.text = punit.get_slot().build_att_string();
        DefT.text = punit.get_slot().build_def_string();
    }

    private void set_press_attackB(bool pressed) {
        attackB.image.color = pressed ? Controller.GREY : Color.white;
    }

    private void set_press_defB(bool pressed) {
        defB.image.color = pressed ? Controller.GREY : Color.white;
    }

    private void set_press_moveB(bool pressed) {
        moveB.image.color = pressed ? Controller.GREY : Color.white;
    }

    private void set_press_attributeB(bool pressed) {
        attributeB.image.color = pressed ? Controller.GREY : Color.white;
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

    private void enable_attributeB() {
        attributeB.interactable = true;
    }

    private void disable_attributeB() {
        attributeB.interactable = false;
    }
}