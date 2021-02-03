using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

// Manages intra-turn progression. Stages are within phases which are within a game turn.
// There are 3 stages for each phase, and a phase for each player.
public class TurnPhaser : MonoBehaviour, ISaveLoad {
    public static TurnPhaser I { get; private set; }
    
    public const int MOVEMENT = 1;
    public const int TRAVEL_CARD = 2;
    public const int ACTION = 3; // mine, build

    public event Action<Discipline> on_disc_change;
    public event Action on_turn_change;

    public int turn { get; private set; }
    
    private int _active_disc_ID;
    public int active_disc_ID {
        get { return _active_disc_ID; }
        set {
            _active_disc_ID = value % 3;
            active_disc = Controller.I.get_disc(active_disc_ID);
            active_disc.active = true;
            Controller.I.get_disc((_active_disc_ID + 1) % 3).active = false;
            Controller.I.get_disc((_active_disc_ID + 2) % 3).active = false;
        }
    } // disciplines are like sub factions.
    public Discipline active_disc { get; private set; }

    private MapCell cell;
    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
    }
    void Start() {
        _active_disc_ID = Discipline.ASTRA;
        active_disc = Controller.I.discs[_active_disc_ID];
    }
    
    public void advance_turn() {
        turn++;
        on_turn_change();
       /* foreach (Discipline disc in Controller.I.discs.Values) {
            disc.register_turn();
        }
        Controller.I.city.register_turn();*/
    }  
    
    public void end_disciplines_turn() {
        MapUI.I.close_cell_UI();
        advance_player();
    }

    private void advance_player() {
        active_disc_ID++;
        if (on_disc_change != null)
            on_disc_change(active_disc);
        if (active_disc_ID == 0)
            advance_turn();

        if (active_disc.dead) {
            active_disc.respawn();
        }

        // If leader of a battle, bypass stages and enter battle.
        Battle b = Map.I.get_current_cell().battle;
        if (b != null) {
            if (active_disc == b.leader) {
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
        cell = active_disc.cell;
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