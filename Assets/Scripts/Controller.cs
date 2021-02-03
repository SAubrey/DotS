using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Controller : MonoBehaviour {
    public static Controller I { get; private set; }

    public const string MAP = "Map";
    public const string CONTROLLER = "Controller";
    public const string TRAVEL_DECK = "TravelDeck";


    public Button loadB, saveB, resumeB;
    public GameObject save_warningP, new_game_warningP, load_warningP;
    public Discipline astra, martial, endura;
    public City city;
    public bool game_has_begun { get; private set; } = false;

    public IDictionary<int, Discipline> discs = new Dictionary<int, Discipline>();
    public event Action<bool> init;

    void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(gameObject);
        }
        astra.ID = Discipline.ASTRA;
        martial.ID = Discipline.MARTIAL;
        endura.ID = Discipline.ENDURA;
        city.ID = City.CITY;
        discs.Add(Discipline.ASTRA, astra);
        discs.Add(Discipline.MARTIAL, martial);
        discs.Add(Discipline.ENDURA, endura);
    }

    void Start() {
        save_warningP.SetActive(false);
        new_game_warningP.SetActive(false);
        load_warningP.SetActive(false);
    }

    public void initialize(bool from_save) {
        init(from_save);
        if (from_save) {

        } else {
            astra.pos = new Vector3(12.5f, 12.9f, 0);
            martial.pos = new Vector3(12.1f, 12.1f, 0);
            endura.pos = new Vector3(12.9f, 12.1f, 0);
            TurnPhaser.I.active_disc_ID = 0;
        }
        // Clear fields not overwritten by possible load.
        Formation.I.reset();
        BattlePhaser.I.reset(from_save);
        
        // Order matters
        Map.I.init(from_save);
        TurnPhaser.I.reset();
        EquipmentUI.I.init(TurnPhaser.I.active_disc);

        MapUI.I.register_disc_change(TurnPhaser.I.active_disc);

        game_has_begun = true;
    }

    public void game_over() {
        CamSwitcher.I.set_active(CamSwitcher.MENU, true);
        initialize(false);
    }

    // Called by save button
    public void save_game() {
        // Double check the user wants to overwrite their save.
        save_warningP.SetActive(false);
        
        List<GameData> serializables = new List<GameData>() {
            { Map.I.save() },
            { astra.save() },
            { martial.save() },
            { endura.save() },
            { city.save() },
        };
        
        foreach (var s in serializables) {
            FileIO.save_game(s, s.name);
        }
    }

    public void load_game() {
        TurnPhaserData data = FileIO.load_game("TurnPhaser") as TurnPhaserData;
        if (data == null)
            return;

        TurnPhaser.I.load(data);
        Map.I.load(FileIO.load_game(MAP));
        astra.load(FileIO.load_game("astra"));
        martial.load(FileIO.load_game("martial"));
        endura.load(FileIO.load_game("endura"));
        city.load(FileIO.load_game("city"));

        initialize(true);
        CamSwitcher.I.flip_menu_map();
    }

    public void new_game() {
        initialize(false);
        CamSwitcher.I.flip_menu_map();
    }

    public void check_button_states() {
        loadB.interactable = FileIO.load_file_exists();
        saveB.interactable = game_has_begun;
        resumeB.interactable = game_has_begun;
    } 

    public void save_button() {
        //if (FileIO.load_file_exists()) {
        save_warningP.SetActive(true);    
        //return;
        //}
        //save_game();
    }

    public void load_button() {
        if (game_has_begun) {
            load_warningP.SetActive(true);
            return;
        }
        load_game();
    }

    public void new_game_button() {
        if (game_has_begun) {
            new_game_warningP.SetActive(true);
            return;
        }
        new_game();
    }

    public void show_warning_panel(bool active) {
        save_warningP.SetActive(active);
    }

    public Discipline get_disc(int ID) {
        return discs[ID];
    }

    // BUTTON HANDLES
    public void inc_stat(string field) {
        TurnPhaser.I.active_disc.change_var(field, 1);
    }

    public void dec_stat(string field) {
        TurnPhaser.I.active_disc.change_var(field, -1);
    }

    public void inc_city_stat(string field) {
        city.change_var(field, 1);
    }

    public void dec_city_stat(string field) {
        city.change_var(field, -1);
    }

    public void quit_game() {
        Application.Quit();
    }
}

public struct Pos {
    public int x, y;
    public Pos(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Vector3 to_vec3 { get { return new Vector3(x, y, 0); } }
}