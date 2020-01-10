using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public const string ASTRA = "astra"; // must be strings
    public const string ENDURA = "endura";
    public const string MARTIAL = "martial";

    // UI CONSTANTS
    public static Color GREY = new Color(.78125f, .78125f, .78125f, 1);

    // Singleton gameobjects
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
    public TileMapper tile_mapper;
    public EnemyLoader enemy_loader;
    public LineDrawer line_drawer;
    public EnemyBrain enemy_brain;

    public Discipline astra;
    public Discipline martial;
    public Discipline endura;
    public Storeable city;
    private int turn_number = 1;
    public int num_discs = 3;

    public string active_disc; // disciplines are like sub factions.

    public IDictionary<string, Discipline> discs = new Dictionary<string, Discipline>();

    void Awake() {
        // Public classes that all other classes will grab from initially.
        formation = GameObject.Find("Formation").GetComponent<Formation>();
        turn_phaser = GameObject.Find("TurnPhaser").GetComponent<TurnPhaser>();
        selector = GameObject.Find("Selector").GetComponent<Selector>();
        attack_queuer = GameObject.Find("AttackQueuer").GetComponent<AttackQueuer>();
        unit_panel_man = GameObject.Find("UnitPanelManager").GetComponent<UnitPanelManager>();
        battle_phaser = GameObject.Find("BattlePhaser").GetComponent<BattlePhaser>();
        cam_switcher = GameObject.Find("CamSwitcher").GetComponent<CamSwitcher>();
        bat_loader = GameObject.Find("BatLoader").GetComponent<BatLoader>();
        travel_deck = GameObject.Find("TravelDeck").GetComponent<TravelDeck>();
        tile_mapper = GameObject.Find("TileMapper").GetComponent<TileMapper>();
        enemy_loader = GameObject.Find("EnemyLoader").GetComponent<EnemyLoader>();
        map_ui = GameObject.Find("MapUI").GetComponent<MapUI>();
        unit_panel_man = GameObject.Find("UnitPanelManager").GetComponent<UnitPanelManager>();
        line_drawer = GameObject.Find("LineDrawer").GetComponent<LineDrawer>();
        enemy_brain = GameObject.Find("EnemyBrain").GetComponent<EnemyBrain>();

        discs.Add(ASTRA, astra);
        discs.Add(MARTIAL, martial);
        discs.Add(ENDURA, endura);
    }

    void Start() {
        set_disc(ASTRA);
    }

    public void advance_turn() {
        turn_number++;

        foreach (Discipline disc in discs.Values) {
            disc.register_turn();
        }
        city.register_turn();

        map_ui.turn_number_t.text = turn_number.ToString();
        map_ui.load_stats(active_disc);
    }
     
    public void set_disc(string type) {
        active_disc = type;
        map_ui.load_stats(active_disc);

        if (get_active_bat().in_battle) {
            if (get_active_bat().mini_retreating) {
                enemy_loader.load_existing_enemies(tile_mapper.get_enemies(get_disc().pos));
            } else {
                Debug.Log("Loading " + active_disc);
                formation.load_board(active_disc);
            }
        }
    }

    public void rotate_disc() {
        if (get_disc_name() == ASTRA) set_disc(ENDURA);
        else if (get_disc_name() == ENDURA) set_disc(MARTIAL);
        else if (get_disc_name() == MARTIAL) set_disc(ASTRA);
        Debug.Log("Rotated disc to " + get_disc_name());
    }       

    public string get_disc_name() {
        return active_disc;
    }

    public Discipline get_disc() {
        return discs[get_disc_name()];
    }

    public Battalion get_active_bat() {
        return get_disc().bat;
    }

    // BUTTON HANDLES
    public void inc_stat(string field) {
        discs[active_disc].change_var(field, 1);
    }

    public void dec_stat(string field) {
        discs[active_disc].change_var(field, -1);
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
}