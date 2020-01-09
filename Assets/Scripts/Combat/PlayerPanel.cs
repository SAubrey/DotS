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
    private bool attB_pressed = false;
    private bool defB_pressed = false;
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
        depress_moveB();
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
    
        if (punit.attack_set)
            press_attackB(); // show cancel attack
        else
            depress_attackB(); // show that an attack has not been selected

        if (punit.defending)
            press_defB();
        else
            depress_defB();
    }

    public void attack() {
        if (!bp.targeting)
            return;
        reset_buttons(1);

        toggle_attackB();
        if (slot.get_punit().attack_set) {
            aq.get_player_queue().remove_attack(slot.get_punit(), c.line_drawer);
            selector.selecting_target = false;
        } else {
            selector.selecting_target = !selector.selecting_target;
        }
    }

    public void defend() {
        if (!bp.targeting)
            return;
        reset_buttons(2);

        toggle_defB();
        slot.get_punit().defend();
    }

    public void move() {
        reset_buttons(3);
        selector.selecting_move = !selector.selecting_move;
        if (selector.selecting_move) {
            press_moveB();
        } else
            depress_moveB();
    }

    public override void close() {
        base.close();
        disable_returnB();
    }

    private void reset_buttons(int except) {
        if (except != 1 && attB_pressed) 
            attack();
        if (except != 2 && defB_pressed)
            defend();
        if (except != 3 && selector.selecting_move)
            move();
    }

    private void toggle_attackB() {
        attB_pressed = !attB_pressed;
        if (attB_pressed) {
            press_attackB();
        } else {
            depress_attackB();
        }
    }

    private void toggle_defB() {
        defB_pressed = !defB_pressed;
        if (defB_pressed) {
            press_defB();
        } else {
            depress_defB();
        }
    }

    private void update_text(PlayerUnit punit) {
        set_name(punit.get_name());
        ResT.text = punit.resilience.ToString();
        AttT.text = punit.attack_dmg.ToString();
        DefT.text = punit.defense.ToString();
    }

    public void press_attackB() {
        attackB.image.color = Controller.GREY;
        attB_pressed = true;
    }

    public void depress_attackB() {
        attackB.image.color = Color.white;
        attB_pressed = false;
    }

     public void press_defB() {
        defB.image.color = Controller.GREY;
        defB_pressed = true;
    }

    public void depress_defB() {
        defB.image.color = Color.white;
        defB_pressed = false;
    }

    public void press_moveB() {
        moveB.image.color = Controller.GREY;

    }

    public void depress_moveB() {
        moveB.image.color = Color.white;
    }

    // TOGGLES
    public void enable_rotateB() {
        upB.interactable = true;
        downB.interactable = true;
        leftB.interactable = true;
        rightB.interactable = true;
    }

    public void disable_rotateB() {
        upB.interactable = false;
        downB.interactable = false;
        leftB.interactable = false;
        rightB.interactable = false;
    }
    public void enable_returnB() {
        returnB.interactable = true;
    }

    public void disable_returnB() {
        returnB.interactable = false;
    }

    public void enable_attackB() {
        attackB.interactable = true;
    }

    public void disable_attackB() {
        attackB.interactable = false;
    }

    public void enable_defB() {
        defB.interactable = true;
    }

    public void disable_defB() {
        defB.interactable = false;
    }

    public void enable_moveB() {
        moveB.interactable = true;
    }

    public void disable_moveB() {
        moveB.interactable = false;
    }
}