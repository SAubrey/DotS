using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void init(bool from_save) {
        // Clear fields not overwritten by possible load.
        Formation.I.reset();
        BattlePhaser.I.reset(from_save);
        
        Map.I.init(from_save);
        TurnPhaser.I.reset();
        TravelDeck.I.init(from_save);

        game_has_begun = true;
    }

    public void game_over() {
        CamSwitcher.I.set_active(CamSwitcher.MENU, true);
        init(false);
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
            { TravelDeck.I.save() },
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
        TravelDeck.I.load(FileIO.load_game(TRAVEL_DECK));

        init(true);
        CamSwitcher.I.flip_menu_map();
    }

    public void new_game() {
        foreach (Discipline disc in discs.Values) {
            disc.new_game();
        }
        astra.pos = new Vector3(10.5f, 10.9f, 0);
        martial.pos = new Vector3(10.1f, 10.1f, 0);
        endura.pos = new Vector3(10.9f, 10.1f, 0);
        city.new_game();
        MapUI.I.update_storeable_resource_UI(city);
        MapUI.I.update_storeable_resource_UI(astra);
        MapUI.I.load_battalion_count(get_disc().bat);
        MapUI.I.highlight_discipline(MapUI.I.discT, TurnPhaser.I.active_disc_ID);
        init(false);
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

    public Discipline get_disc(int ID=-1) {
        return ID > -1 ? discs[ID] : discs[TurnPhaser.I.active_disc_ID];
    }

    public Battalion get_bat_from_ID(int ID) {
        return discs[ID].bat;
    }

    // BUTTON HANDLES
    public void inc_stat(string field) {
        get_disc().change_var(field, 1);
    }

    public void dec_stat(string field) {
        get_disc().change_var(field, -1);
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