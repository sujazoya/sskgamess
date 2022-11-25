using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
public class SerializationManger 
{
    public static bool Save(string saveName, object saveDta)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }
        string path = Application.persistentDataPath + "/saves/" + saveName + "";

        FileStream file = File.Create(path);
        formatter.Serialize(file, saveDta);
        file.Close();
        return true;
    }
    public static bool SavePlayer(string saveName, object saveDta)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }
        string path = Application.persistentDataPath + "/saves/" + saveName + "";

        FileStream file = File.Create(path);
        formatter.Serialize(file, saveDta);
        file.Close();
        return true;
    }
    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed To Load File At {0}" , path);
            file.Close();
            return null;
        }

    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        //SurrogateSelector selector = new SurrogateSelector();

        //Vector3SerializationSarrogate vector3Serialization = new Vector3SerializationSarrogate();
        //QuaternionSerializationSarrogate quaternionSerialization = new QuaternionSerializationSarrogate();
        //selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Serialization);

        //selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSerialization);

        //formatter.SurrogateSelector = selector;


        return formatter;
    }

}
