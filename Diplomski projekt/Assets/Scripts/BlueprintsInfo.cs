using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlueprintsInfo
{
    public Data data;

    /// <summary>
    /// method for creating object from JSON string
    /// </summary>
    public static BlueprintsInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BlueprintsInfo>(jsonString);
    }

    [Serializable]
    public class Blueprints
    {
        public int totalCount; //total number of blueprints
        public List<Item> items; //list of items (where item is data about one blueprint)
    }

    //JSON structure - has to be here for JSONUtility to work
    [Serializable]
    public class Data
    {
        public Blueprints blueprints;
    }

    //data about one blueprint
    [Serializable]
    public class Item
    {
        public int id;  //blueprint id
        public string userId; //user id
        public string name; //blueprint name
    }

    //JSON structure - has to be here for JSONUtility to work
    [Serializable]
    public class Root
    {
        public Data data;
    }
}
