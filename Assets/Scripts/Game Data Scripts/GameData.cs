using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;




[Serializable]
public class SaveData
{
    public bool[] isActives;
    public int[] highScores;


}


// Keep in mind that the gamedata is only saved in RUNTIME. Any other changes will not be store.
public class GameData : MonoBehaviour
{

    // You can click the GameData object in the editor and uncheck ON RUNTIME any level to test wheter levels are correct.
    public static GameData gameData;

    // needs to be public to be serialized by formatter
    public SaveData saveData;






    // Start is called before the first frame update
    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        Load();

        if (saveData.isActives.Length != 25)
        {
            saveData.isActives = new bool[25];
            saveData.highScores = new int[25];
            saveData.isActives[0] = true;
        }

    }


    public void Save()
    {
        // Binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter();

        // Create a route from the program to file
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);


        // Create a copy save data
        SaveData data = new SaveData();
        data = saveData;

        // Save the data
        formatter.Serialize(file,data);

        // Close the Filestream
        file.Close();

        Debug.Log("Saved");


    }

    private void OnDisable()
    {
        Save();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            // Create a binary formatter
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);

            saveData = formatter.Deserialize(file) as SaveData;

            file.Close();

            Debug.Log("Loaded");
        }
    }
}
