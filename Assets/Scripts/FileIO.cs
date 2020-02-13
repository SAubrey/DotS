using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class FileIO {
    static string path = Application.persistentDataPath + "/";
    static string path_cap = ".dots";

    public static void save_game(GameData data, string class_name) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = 
            new FileStream(concat_path(class_name), FileMode.Create);
        //GameData data = new GameData(c);
        Debug.Log("saving to file at " + concat_path(class_name));
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static GameData load_game(string class_name) {
        string full_path = concat_path(class_name);
        if (File.Exists(full_path)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = 
                new FileStream(full_path, FileMode.Open);
            
            GameData data = null;
            data = bf.Deserialize(stream) as GameData;
            /*
            if (class_name == Controller.CONTROLLER) {
                data = bf.Deserialize(stream) as ControllerData;
            } else if (class_name == Controller.ASTRA || 
                    class_name == Controller.ENDURA ||
                    class_name == Controller.MARTIAL)
                data = bf.Deserialize(stream) as DisciplineData;
            else if (class_name == Controller.CITY)
                data = bf.Deserialize(stream) as CityData;
            else if (class_name == Controller.TILE_MAPPER)
                data = bf.Deserialize(stream) as MapData;*/

            stream.Close();
            return data;
        } else {
            Debug.LogError("No save file at " + full_path);
            return null;
        }
    }

    public static bool load_file_exists() {
        return File.Exists(concat_path(Controller.CONTROLLER));
    }

    private static string concat_path(string class_name) {
        return path + class_name + path_cap;
    }
}
