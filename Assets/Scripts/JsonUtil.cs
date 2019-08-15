using System;
using System.Collections;
using Boomlagoon.JSON;
using Hammerplay.TinyArmy;
using Pathfinding.Serialization.JsonFx;
using UnityEngine;

public class JsonUtil
{
    public static string CollectionToJsonString<T>(T arr, string jsonkey) where T : IList
    {
        JSONObject jObject = new JSONObject();
        JSONArray jArray = new JSONArray();
        for (int i = 0; i < arr.Count; i++)
            jArray.Add(new JSONValue(arr[i].ToString()));

        jObject.Add(jsonkey, jArray);
        return jObject.ToString();
    }

    public static T[] JsonStringToArray<T>(string jsonString, string jsonKey, Func<string, T> parse){
        JSONObject jobject = JSONObject.Parse(jsonString);
        JSONArray jArray = jobject.GetArray(jsonKey);

        T[] convertedArray = new T[jArray.Length];
        for (int i = 0; i < jArray.Length; i++)
            convertedArray[i] = parse(jArray[i].Str.ToString());

        return convertedArray;
    }

    public static string DataToString<T>(T data) {

        string jsongString = JsonWriter.Serialize(data);
        return jsongString;

    }

    public static PlayerData StringToPlayerData(string jsonString) {
        if(jsonString == string.Empty) {
            return new PlayerData();
        }
        PlayerData data = JsonReader.Deserialize<PlayerData>(jsonString);
        return data;
    }
}
