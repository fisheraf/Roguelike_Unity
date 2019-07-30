using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad
{
    

    public static void Save(SaveFile saveFile)
    {
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        FileStream file = File.Create(Application.persistentDataPath + "/gameSave.dat"); //you can call it anything you want


        //get player and game info
        //SaveFile saveFile = new SaveFile(saveFile)
        

        bf.Serialize(file, saveFile);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/gameSave.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameSave.dat", FileMode.Open);
            //saveFile = (SaveFile)bf.Deserialize(file);
            file.Close();

            //set player and game info
        }
    }
}
