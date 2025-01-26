using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//contains all data about the token

[Serializable]
public class TokenInfo 
{
    public Data data;

    //method for creating object from JSON string
    public static TokenInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TokenInfo>(jsonString);
    }

    //JSON structure - has to be here for JSONUtility to work
    [Serializable]
    public class Data
    {
        public Login login;
    }
    [Serializable]
    public class Login
    {
        //token data
        public string token;
    }
 
}
