using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : UnitPanel {
    Controller c;
    AttackQueuer aq;
        // Player exclusive
    public Button returnB;
    public Button moveB;
    public Button attackB;
    private bool attB_pressed = false;
    public Text DefT;
    public Text ResT;

    void Start() {
        c = GameObject.Find("Controller").GetComponent<Controller>();
        aq = c.attack_queuer;
    }

    // Limit what a player can do with a unit based on game logic.
    /* A player can only ever move OR attack in action phase. 
    */
    public override void update_panel(Slot slot) {
        if (slot.get_enemy() != null)
            return;

        PlayerUnit punit = slot.get_punit();
        update_text(punit);
        disable_attackB();
        disable_moveB();
        disable_returnB();
        depress_moveB();
        //Debug.Log(" attack set? " + punit.attack_set);
        //Debug.Log("has attacked? " + punit.has_attacked);

        bool actionable_staging = (bp.range_stage && punit.is_range()) || 
                                (bp.action1_stage || bp.action2_stage);

        if (!punit.has_moved && !punit.has_acted_in_stage && 
            (bp.movement_stage || actionable_staging)) {
            enable_moveB();
        } else if (bp.placement_stage) {
            enable_returnB();
        } 

        bool is_first_slot_in_group = slot.get_group().get_highest_player_slot() == slot;
        if (bp.targeting && !punit.has_acted_in_stage && !punit.has_attacked && 
            actionable_staging && is_first_slot_in_group) {
            enable_attackB();
        }
    
        if (punit.attack_set)
            press_attackB(); // show cancel attack
        else
            depress_attackB(); // show that an attack has not been selected
        /*if (punit.has_moved)
            press_moveB();
        else
            depress_moveB();*/
    }

    public void attack() {
        if (!bp.targeting)
            return;
        toggle_attackB();
        if (slot.get_punit().attack_set) {
            aq.get_player_queue().remove_attack(slot.get_punit(), c.line_drawer);
            selector.selecting_target = false;
        } else {
            selector.selecting_target = !selector.selecting_target;
        }
    }

    private void update_text(PlayerUnit punit) {
        set_name(punit.get_name());
        ResT.text = punit.resilience.ToString();
        AttT.text = punit.attack.ToString();
        DefT.text = punit.defense.ToString();
    }

    public void move() {
        selector.selecting_move = !selector.selecting_move;
        if (selector.selecting_move) {
            press_moveB();
        } else
            depress_moveB();
    }

    private void toggle_attackB() {
        attB_pressed = !attB_pressed;
        if (attB_pressed) {
            press_attackB();
        } else {
            depress_attackB();
        }
    }

    public void press_attackB() {
        attackB.image.color = Controller.GREY;
        attackB.GetComponentInChildren<Text>().text = "Cancel Attack";
        attB_pressed = true;
    }

    public void depress_attackB() {
        attackB.image.color = Color.white;
        attackB.GetComponentInChildren<Text>().text = "Attack";
        attB_pressed = false;
    }

    public void press_moveB() {
        moveB.image.color = Controller.GREY;
        moveB.GetComponentInChildren<Text>().text = "Cancel Move";
    }

    public void depress_moveB() {
        moveB.image.color = Color.white;
        moveB.GetComponentInChildren<Text>().text = "Move";
    }

    // TOGGLES
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

    public void enable_moveB() {
        moveB.interactable = true;
    }

    public void disable_moveB() {
        moveB.interactable = false;
    }
}