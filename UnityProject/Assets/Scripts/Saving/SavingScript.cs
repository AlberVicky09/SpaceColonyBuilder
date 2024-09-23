using System.IO;
using UnityEngine;

public class SavingScript : MonoBehaviour
{
    string path;
    private SavingData saveData;

    void Start()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
        //persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";

    }

    public void LoadGame() {
        var reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        
        saveData = JsonUtility.FromJson<SavingData>(json);
    }

    private void GenerateSaveObject() {
        saveData = new SavingData();
    }

    public void SaveGame() {
        GenerateSaveObject();
        var json = JsonUtility.ToJson(saveData);

        var writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();
    }
}
