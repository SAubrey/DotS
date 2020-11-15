using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

// Manages intra-turn progression. Stages are within phases which are within a game turn.
// There are 3 stages for each phase, and a phase for each player.
public class TurnPhaser : MonoBehaviour, ISaveLoad {
    public static TurnPhaser I { get; private set; }
    
    public const int MOVEMENT = 1;
    public const int TRAVEL_CARD = 2;
    public const int ACTION = 3; // mine, build

    private int _turn = 1;
    public int turn {
        get { return _turn; }
        set {
            _turn = value;
            if (MapUI.I != null)
                MapUI.I.turn_number_t.text = turn.ToString();
        }
    }
    
    private int _active_disc_ID;
    public int active_disc_ID {
        get { return _active_disc_ID; }
        set {
            _active_disc_ID = value % 3;
            MapUI.I.register_turn();
        }
    } // disciplines are like sub factions.

    private MapCell cell;
    void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    void Start() {
        _active_disc_ID = Discipline.ASTRA;
    }
    
    public void advance_turn() {
        TurnPhaser.I.turn++;

        foreach (Discipline disc in Controller.I.discs.Values) {
            disc.register_turn();
        }
        Controller.I.city.register_turn();
    }  
    
    public void end_disciplines_turn() {
        Map.I.close_cell_UI();
        advance_player();
    }

    private void advance_player() {
        active_disc_ID++;
        if (active_disc_ID == 0)
            advance_turn();
        
        // If leader of a battle, bypass stages and enter battle.
        Battle b = Map.I.get_current_cell().battle;
        if (b != null) {
            if (Controller.I.get_disc() == b.leader) {
                BattlePhaser.I.resume_battle(Map.I.get_current_cell());
            // If in battle but not leader, skip turn. (Turn used in group battle)
            } else {
                advance_player();
                return;
            }
        }
        reset();
        CamSwitcher.I.set_active(CamSwitcher.MAP, true);
    }

    public void reset() { // New game, new player
        cell = Controller.I.get_disc().cell;
        MapUI.I.set_active_ask_to_enterP(false);
        MapUI.I.update_cell_text(cell.name);
    }

    public GameData save() {
        TurnPhaserData data = new TurnPhaserData();
        data.name = "TurnPhaser";
        data.turn = turn;
        data.active_disc_ID = active_disc_ID;
        return data;
    }

    public void load(GameData generic) {
        TurnPhaserData data = generic as TurnPhaserData;
        active_disc_ID = data.active_disc_ID;
        turn = data.turn;
    }
}