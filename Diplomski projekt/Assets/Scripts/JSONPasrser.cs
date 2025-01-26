
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
 
public class JSONPasrser : MonoBehaviour
{
    [Header("House JSON info")]
    [SerializeField]
    private HouseInfo houseInfo;

    public string JSONToParse;

    private string json;
 
    public Action<HouseInfo> JSONParseAction;

    public string url = "https://diplomskiserver.onrender.com/download/";



    private void Start() 
    {
        //dummy json za testiranje
        //StartCoroutine(GetJSON("dummyWithGps.json"));

        
    }

    public void GetJSONFunc(string filename)
    {
        StartCoroutine(GetJSON(filename));
    }

    IEnumerator GetJSON(string filename)
    {
        // Create a UnityWebRequest
        UnityWebRequest request = UnityWebRequest.Get(url + filename);

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Handle the successful response
            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);

            // Process the response data (e.g., parse JSON)
            // ...
            ParseJson(responseText);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
        }

        // Dispose of the request
        request.Dispose();
    }


    /// <summary>
    /// Fixes the number format in the JSON file
    /// </summary>
    /// <param name="houseInfo"></param>
    private void NumberFixer(HouseInfo houseInfo)
    {  
        houseInfo.Floor.Position.X /= 1000;
        houseInfo.Floor.Position.Y /= 1000;
        houseInfo.Floor.Position.Z /= 1000;
        houseInfo.Floor.Dimension.X /= 1000;
        houseInfo.Floor.Dimension.Y /= 1000;
        houseInfo.Floor.Dimension.Z /= 1000;
 
        for (int i = 0; i < houseInfo.Walls.Count; i++)
        {
            houseInfo.Walls[i].Position.X /= 1000;
            houseInfo.Walls[i].Position.Y /= 1000;
            houseInfo.Walls[i].Position.Z /= 1000;
            houseInfo.Walls[i].Dimension.X /= 1000;
            houseInfo.Walls[i].Dimension.Y /= 1000;
            houseInfo.Walls[i].Dimension.Z /= 1000;
 
            if (houseInfo.Walls[i].Doors.Count > 0)
            {
                for (int j = 0; j < houseInfo.Walls[i].Doors.Count; j++)
                {
                    houseInfo.Walls[i].Doors[j].Position.X /= 1000;
                    houseInfo.Walls[i].Doors[j].Position.Y /= 1000;
                    houseInfo.Walls[i].Doors[j].Position.Z /= 1000;
                    houseInfo.Walls[i].Doors[j].Dimension.X /= 1000;
                    houseInfo.Walls[i].Doors[j].Dimension.Y /= 1000;
                    houseInfo.Walls[i].Doors[j].Dimension.Z /= 1000;
                }
            }
 
            if (houseInfo.Walls[i].Windows.Count > 0)
            {
                for (int j = 0; j < houseInfo.Walls[i].Windows.Count; j++)
                {
                    houseInfo.Walls[i].Windows[j].Position.X /= 1000;
                    houseInfo.Walls[i].Windows[j].Position.Y /= 1000;
                    houseInfo.Walls[i].Windows[j].Position.Z /= 1000;
                    houseInfo.Walls[i].Windows[j].Dimension.X /= 1000;
                    houseInfo.Walls[i].Windows[j].Dimension.Y /= 1000;
                    houseInfo.Walls[i].Windows[j].Dimension.Z /= 1000;
                }
            }
        }
 
        houseInfo.Attic.Floor.Position.X /= 1000;
        houseInfo.Attic.Floor.Position.Y /= 1000;
        houseInfo.Attic.Floor.Position.Z /= 1000;
        houseInfo.Attic.Floor.Dimension.X /= 1000;
        houseInfo.Attic.Floor.Dimension.Y /= 1000;
        houseInfo.Attic.Floor.Dimension.Z /= 1000;
 
        houseInfo.Attic.Roof.Position.X /= 1000;
        houseInfo.Attic.Roof.Position.Y /= 1000;
        houseInfo.Attic.Roof.Position.Z /= 1000;
        houseInfo.Attic.Roof.Dimension.X /= 1000;
        houseInfo.Attic.Roof.Dimension.Y /= 1000;
        houseInfo.Attic.Roof.Dimension.Z /= 1000;
 
        for (int i = 0; i < houseInfo.Attic.AtticSegments.Count; i++)
        {
            houseInfo.Attic.AtticSegments[i].Position.X /= 1000;
            houseInfo.Attic.AtticSegments[i].Position.Y /= 1000;
            houseInfo.Attic.AtticSegments[i].Position.Z /= 1000;
            houseInfo.Attic.AtticSegments[i].Dimension.X /= 1000;
            houseInfo.Attic.AtticSegments[i].Dimension.Y /= 1000;
            houseInfo.Attic.AtticSegments[i].Dimension.Z /= 1000;
        }

        houseInfo.Items.ForEach(item =>
        {
            item.Position.X /= 1000;
            item.Position.Y /= 1000;
            item.Position.Z /= 1000;
            //item.Orientation.X /= 1000;
            //item.Orientation.Y /= 1000;
            //item.Orientation.Z /= 1000;
            //item.Orientation.W /= 1000;
        });

        CheckIfDuplicate(houseInfo);
        JSONParseAction?.Invoke(houseInfo); //JSON parsing is done - send houseInfo to the script that subscribes to this action 
    }

    /// <summary>
    /// Check if there are duplicate doors or windows (in same place - remove if true)
    /// </summary>
    /// <param name="houseInfo"></param>
    private void CheckIfDuplicate(HouseInfo houseInfo)
    {

        foreach (var wall in houseInfo.Walls)
        {
            if (wall.Doors.Count > 0)
            {
                for (int j = 0; j < wall.Doors.Count; j++)
                {
                    for (int k = j + 1; k < wall.Doors.Count; k++)
                    {
                        if (wall.Doors[j].Position.X == wall.Doors[k].Position.X && wall.Doors[j].Position.Y == wall.Doors[k].Position.Y && wall.Doors[j].Position.Z == wall.Doors[k].Position.Z)
                            wall.Doors.RemoveAt(k);
                    }
                }
            }

            if (wall.Windows.Count > 0)
            {
                for (int j = 0; j < wall.Windows.Count; j++)
                {
                    for (int k = j + 1; k < wall.Windows.Count; k++)
                    {
                        if (wall.Windows[j].Position.X == wall.Windows[k].Position.X && wall.Windows[j].Position.Y == wall.Windows[k].Position.Y && wall.Windows[j].Position.Z == wall.Windows[k].Position.Z)
                            wall.Windows.RemoveAt(k);
                    }
                }
            }
        }
    }

    /// <summary>
    /// JSON parser - for parsing JSON string into HouseInfo object (House data)
    /// </summary>
    /// <param name="JSON">JSON string that needs to be parsed</param>
    public void ParseJson(string JSON)
    {
        json = JSON.Replace("−", "-");

        int index = json.IndexOf("Items");
        string jsonSubstring = "";
        if (index != -1)
        {
            jsonSubstring = json.Substring(index);
            string[] substrings = jsonSubstring.Split(',');
            int subIndex = index - 1;
            for (int i = 0; i < substrings.Length; i++)
            {
                if (int.TryParse(substrings[i], out int n))
                {
                    int indexNumber = substrings[i].IndexOfAny("0123456789".ToCharArray());
                    json = json.Remove(subIndex, 1).Insert(subIndex, ".");
                }
                else if (substrings[i].Contains('}'))
                {
                    string subsubstring = substrings[i].Substring(0, substrings[i].IndexOf("}"));
                    if (int.TryParse(subsubstring, out int m))
                    {
                        int indexNumber = subsubstring.IndexOfAny("0123456789".ToCharArray());
                        json = json.Remove(subIndex, 1).Insert(subIndex, ".");
                    }
                }
                subIndex += substrings[i].Length + 1;
            }
        }
        //get geographic coordinates of house

        houseInfo = HouseInfo.CreateFromJSON(json);

        NumberFixer(houseInfo);
    }

    
}