using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour, ISaveLoad {
    public const string MAP = "Map";
    public const string CONTROLLER = "Controller";
    public const string TRAVEL_DECK = "TravelDeck";


    // Inspector gameobjects
    public Formation formation;
    public TurnPhaser turn_phaser;
    public MapUI map_ui;
    public Selector selector;
    public AttackQueuer attack_queuer;
    public UnitPanelManager unit_panel_man;
    public BattlePhaser battle_phaser;
    public CamSwitcher cam_switcher;
    public BatLoader bat_loader;
    public TravelDeck travel_deck;
    public Map map;
    public EnemyLoader enemy_loader;
    public LineDrawer line_drawer;
    public EnemyBrain enemy_brain;
    public CityUIManager city_ui;
    public TravelCardManager travel_card_manager;
    public SoundManager sound_manager;
    public BackgroundLoader background_loader;

    public Button loadB, saveB, resumeB;
    public GameObject save_warningP, new_game_warningP, load_warningP;
    public Discipline astra, martial, endura;
    public City city;
    private bool game_has_begun = false;
    private int _turn_number = 1;
    private int turn_number {
        get { return _turn_number; }
        set {
            _turn_number = value;
            if (map_ui != null)
                map_ui.turn_number_t.text = turn_number.ToString();
        }
    }
    public int get_turn_num() { return turn_number; }

    private int _active_disc_ID;
    public int active_disc_ID {
        get { return _active_disc_ID; }
        set {
            _active_disc_ID = value % 3;
            map_ui.load_discipline_UI(get_disc());
            map_ui.load_discipline_UI(city);
        }
    } // disciplines are like sub factions.

    public IDictionary<int, Discipline> discs = new Dictionary<int, Discipline>();

    void Awake() {
        astra.ID = Discipline.ASTRA;
        martial.ID = Discipline.MARTIAL;
        endura.ID = Discipline.ENDURA;
        city.ID = City.CITY;
        discs.Add(Discipline.ASTRA, astra);
        discs.Add(Discipline.MARTIAL, martial);
        discs.Add(Discipline.ENDURA, endura);

        // Public classes that all other classes will grab from initially.
        formation = GameObject.Find("Formation").GetComponent<Formation>();
        turn_phaser = GameObject.Find("TurnPhaser").GetComponent<TurnPhaser>();
        selector = GameObject.Find("Selector").GetComponent<Selector>();
        attack_queuer = GameObject.Find("AttackQueuer").GetComponent<AttackQueuer>();
        unit_panel_man = GameObject.Find("UnitPanelManager").GetComponent<UnitPanelManager>();
        battle_phaser = GameObject.Find("BattlePhaser").GetComponent<BattlePhaser>();
        cam_switcher = GameObject.Find("CamSwitcher").GetComponent<CamSwitcher>();
        bat_loader = GameObject.Find("BatLoader").GetComponent<BatLoader>();
        travel_deck = GameObject.Find(TRAVEL_DECK).GetComponent<TravelDeck>();
        map = GameObject.Find(MAP).GetComponent<Map>();
        enemy_loader = GameObject.Find("EnemyLoader").GetComponent<EnemyLoader>();
        map_ui = GameObject.Find("MapUI").GetComponent<MapUI>();
        unit_panel_man = GameObject.Find("UnitPanelManager").GetComponent<UnitPanelManager>();
        line_drawer = GameObject.Find("LineDrawer").GetComponent<LineDrawer>();
        enemy_brain = GameObject.Find("EnemyBrain").GetComponent<EnemyBrain>();
        travel_card_manager = GameObject.Find("TravelCardPanel").GetComponent<TravelCardManager>();
        city_ui = GameObject.Find("CityUIManager").GetComponent<CityUIManager>();
        sound_manager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        background_loader = GameObject.Find("BackgroundLoader").GetComponent<BackgroundLoader>();
    }

    void Start() {
        active_disc_ID = Discipline.ASTRA;
        save_warningP.SetActive(false);
        new_game_warningP.SetActive(false);
        load_warningP.SetActive(false);
    }

    public void init(bool from_save) {
        game_has_begun = true;
        map.init(from_save);
        travel_deck.init(from_save);

        // Clear fields not overwritten by possible load.
        formation.reset();
        battle_phaser.reset(from_save);
        turn_phaser.reset();
        
        cam_switcher.flip_menu_map();
    }

    // Called by save button
    public void save_game() {
        // Double check the user wants to overwrite their save.
        
        save_warningP.SetActive(false);
        List<GameData> serializables = new List<GameData>() {
            { map.save() },
            { save() },
            { astra.save() },
            { martial.save() },
            { endura.save() },
            { city.save() },
            { travel_deck.save() },
        };
        
        foreach (var s in serializables) {
            FileIO.save_game(s, s.name);
        }
    }

    public void load_game() {
        ControllerData cdata = FileIO.load_game(CONTROLLER) as ControllerData;
        if (cdata == null)
            return;

        map.load(FileIO.load_game(MAP));
        astra.load(FileIO.load_game("astra"));
        martial.load(FileIO.load_game("martial"));
        endura.load(FileIO.load_game("endura"));
        city.load(FileIO.load_game("city"));
        travel_deck.load(FileIO.load_game(TRAVEL_DECK));
        load(cdata);

        init(true);
    }

    public void new_game() {
        foreach (Discipline disc in discs.Values) {
            disc.new_game();
        }
        city.new_game();
        init(false);
    }

    public GameData save() {
        ControllerData data = new ControllerData();
        data.name = CONTROLLER;
        data.turn_number = turn_number;
        data.active_disc = active_disc_ID;
        return data;
    }

    public void load(GameData generic) {
        ControllerData data = generic as ControllerData;
        active_disc_ID = data.active_disc;
        turn_number = data.turn_number;
    }

    public void advance_turn() {
        turn_number++;

        foreach (Discipline disc in discs.Values) {
            disc.register_turn();
        }
        city.register_turn();
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

    public void end_game(bool player_won) {
        if (player_won) {
            cam_switcher.set_active(CamSwitcher.MAP, true);

        } else {
            cam_switcher.set_active(CamSwitcher.MAP, true);
            map_ui.set_active_game_lossP(true);
        }
    }

    public void show_warning_panel(bool active) {
        save_warningP.SetActive(active);
    }

    public Discipline get_disc(int ID=-1) {
        return ID > -1 ? discs[ID] : discs[active_disc_ID];
    }

    public Battalion get_active_battalion() {
        return battle_phaser.active_bat;
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