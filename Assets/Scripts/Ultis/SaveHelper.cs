using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

public static class SaveHelper
{
    //public static GameplayObserverSO GameplayObserverSO;
    private static string _private_key = "ngvnktkkgfaIQCasd";
    //public static string fileName = "processdata.txt";

    public static void Save(object obj, string fileName)
    {
        string json = JsonUtility.ToJson(obj);
        // if (PlayerPrefsManager.canEncypt)
        // {
        //     json = DESEncryption.Encrypt(json, _private_key);
        // }
        PlayerPrefs.SetString(fileName, json);
        // WriteToFile(fileName, json);
        // Debug.Log($"Hai save SO: {json} {PlayerPrefsManager.canEncypt}");
    }

    public static void Save<TKey, TValue>(Dictionary<TKey, TValue> obj, string fileName)
    {
        string json = JsonConvert.SerializeObject(obj);
        PlayerPrefs.SetString(fileName, json);
    }
    
    public static void Save<T>(T[] obj, string fileName)
    {
        string json = JsonHelper.ToJson(obj);
        // if (PlayerPrefsManager.canEncypt)
        // {
        //     json = DESEncryption.Encrypt(json, _private_key);
        // }
        PlayerPrefs.SetString(fileName, json);
        // WriteToFile(fileName, json);
        // Debug.Log($"Hai save SO: {json} {PlayerPrefsManager.canEncypt}");
    }

    // public static void SaveDict<T>(T dict, string fileName, bool savePref = false)
    // {
    //     string json = JsonConvert.SerializeObject(dict);
    //     if (savePref)
    //     {
    //         PlayerPrefs.SetString(fileName, json);
    //         return;
    //     }
    //
    //     var encrypt = DESEncryption.Encrypt(json, _private_key);
    //     WriteToFile(fileName, encrypt);
    //     Debug.Log($"Hai save T SO: {json}");
    // }
    //
    public static void Load<T>(ref T obj, string fileName)
    {
        // string path = GetFilePath(fileName);
        // if (!File.Exists(path))
        // {
        //     Save(obj, fileName);
        // }
        //
        // string json = ReadFromFile(fileName);
        // if (PlayerPrefsManager.canEncypt)
        // {
        //     var descrypt = DESEncryption.TryDecrypt(json, _private_key, out json);
        // }
        string jsonP = PlayerPrefs.GetString(fileName);
        // obj = JsonConvert.DeserializeObject<T>(jsonP);
        // Debug.Log($"Hai: Load T SO {jsonP}");
        JsonUtility.FromJsonOverwrite(jsonP, obj);
        // Debug.Log($"Hai: Load SO {json} {PlayerPrefsManager.canEncypt}");
    }
    
    public static List<T> LoadJson<T>(string fileName)
    {
        string jsonP = PlayerPrefs.GetString(fileName);
        return JsonHelper.FromJson<T>(jsonP).ToList();
    }

    public static T Load<T>(string fileName)
    {
        string jsonP = PlayerPrefs.GetString(fileName);
        return JsonConvert.DeserializeObject<T>(jsonP);
    }

    // public static void LoadDict<T>(ref T obj, string fileName, bool savePref = false)
    // {
    //     if (savePref)
    //     {
    //         string jsonP = PlayerPrefs.GetString(fileName);
    //         obj = JsonConvert.DeserializeObject<T>(jsonP);
    //         Debug.Log($"Hai: Load T SO {jsonP}");
    //         return;
    //     }
    //
    //     string path = GetFilePath(fileName);
    //     if (!File.Exists(path))
    //     {
    //         SaveDict(obj, fileName);
    //     }
    //
    //     string json = ReadFromFile(fileName);
    //     var descrypt = DESEncryption.TryDecrypt(json, _private_key, out json);
    //     obj = JsonConvert.DeserializeObject<T>(json);
    //     Debug.Log($"Hai: Load SO {json}");
    // }

    private static void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream stream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write(json);
        }
    }

    private static string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else
        {
            Debug.LogWarning("Hai: File not fount");
            return "";
        }
    }

    private static string GetFilePath(string fileName)
    {
        // Debug.Log(Application.persistentDataPath + "/" + fileName);
        return Application.persistentDataPath + "/" + fileName;
    }
}