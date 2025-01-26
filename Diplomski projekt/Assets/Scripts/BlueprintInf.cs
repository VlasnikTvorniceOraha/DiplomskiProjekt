using System;
using UnityEngine;

// contains only content of one blueprint
[Serializable]
public class BlueprintInf
{
    public Data data;

    //method for creating object from JSON string
    public static BlueprintInf CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BlueprintInf>(jsonString);
    }

    [Serializable]
    public class Blueprint
    {
        //Content of blueprint - JSON string
        public string contentVR;
        public string contentAR;
        public string content;
        public int id;
        public string name;
    }

    //JSON structure - has to be here for JSONUtility to work
    [Serializable]
    public class Data
    {
        public Blueprint blueprint;
    }

    //JSON structure - has to be here for JSONUtility to work
    [Serializable]
    public class Root
    {
        public Data data;
    }
}
