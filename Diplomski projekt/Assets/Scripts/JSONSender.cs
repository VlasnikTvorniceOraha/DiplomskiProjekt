using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSONSender : MonoBehaviour
{
    public string url = "https://diplomskiserver.onrender.com/upload";

    public string jsonDummy = "{\"GPS\":{\"X\":45.813171,\"Z\":15.955117},\"Attic\":{\"Roof\":{\"Pitch\":0,\"Position\":{\"R\":0,\"X\":0,\"Y\":0,\"Z\":3200},\"Dimension\":{\"X\":10548,\"Y\":7216,\"Z\":240}},\"Floor\":null,\"AtticSegments\":[]},\"Floor\":{\"Position\":{\"R\":0,\"X\":0,\"Y\":0,\"Z\":-240},\"Dimension\":{\"X\":10548,\"Y\":7216,\"Z\":240}},\"Items\":[],\"Walls\":[{\"Doors\":[],\"Windows\":[{\"Position\":{\"R\":0,\"X\":1512,\"Y\":0,\"Z\":1000},\"Dimension\":{\"X\":1200,\"Y\":0,\"Z\":1000}}],\"Position\":{\"R\":90,\"X\":0,\"Y\":0,\"Z\":0},\"Dimension\":{\"X\":7016,\"Y\":200,\"Z\":3200}},{\"Doors\":[],\"Windows\":[{\"Position\":{\"R\":0,\"X\":4520,\"Y\":0,\"Z\":1000},\"Dimension\":{\"X\":1200,\"Y\":0,\"Z\":1000}},{\"Position\":{\"R\":0,\"X\":6099,\"Y\":0,\"Z\":1000},\"Dimension\":{\"X\":1200,\"Y\":0,\"Z\":1000}}],\"Position\":{\"R\":0,\"X\":0,\"Y\":7216,\"Z\":0},\"Dimension\":{\"X\":10348,\"Y\":200,\"Z\":3200}},{\"Doors\":[],\"Windows\":[{\"Position\":{\"R\":0,\"X\":4642,\"Y\":0,\"Z\":1000},\"Dimension\":{\"X\":1200,\"Y\":0,\"Z\":1000}}],\"Position\":{\"R\":-90,\"X\":10548,\"Y\":7216,\"Z\":0},\"Dimension\":{\"X\":7016,\"Y\":200,\"Z\":3200}},{\"Doors\":[{\"Position\":{\"R\":0,\"X\":8951,\"Y\":0,\"Z\":60},\"Dimension\":{\"X\":850,\"Y\":0,\"Z\":2200}}],\"Windows\":[],\"Position\":{\"R\":180,\"X\":10548,\"Y\":0,\"Z\":0},\"Dimension\":{\"X\":10348,\"Y\":200,\"Z\":3200}},{\"Doors\":[{\"Position\":{\"R\":0,\"X\":3509,\"Y\":0,\"Z\":60},\"Dimension\":{\"X\":850,\"Y\":0,\"Z\":2200}}],\"Windows\":[],\"Position\":{\"R\":0,\"X\":199,\"Y\":3940,\"Z\":0},\"Dimension\":{\"X\":10148,\"Y\":80,\"Z\":3200}},{\"Doors\":[{\"Position\":{\"R\":0,\"X\":1667,\"Y\":0,\"Z\":60},\"Dimension\":{\"X\":850,\"Y\":0,\"Z\":2200}}],\"Windows\":[],\"Position\":{\"R\":90,\"X\":8536,\"Y\":3940,\"Z\":0},\"Dimension\":{\"X\":3076,\"Y\":80,\"Z\":3200}},{\"Doors\":[{\"Position\":{\"R\":0,\"X\":505,\"Y\":0,\"Z\":60},\"Dimension\":{\"X\":850,\"Y\":0,\"Z\":2200}}],\"Windows\":[],\"Position\":{\"R\":90,\"X\":3389,\"Y\":3940,\"Z\":0},\"Dimension\":{\"X\":3076,\"Y\":80,\"Z\":3200}}]}";


    // Start is called before the first frame update
    void Start()
    {
        SendJSON("upload.json", jsonDummy);
    }

    public void SendJSON(string filename, string json)
    {
        StartCoroutine(SendJsonData(filename, json));
    }

    IEnumerator SendJsonData(string filename, string json)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        // Create a WWWForm object
        WWWForm form = new WWWForm();

        // Add the file to the form
        form.AddBinaryData("file", bodyRaw, filename, "application/json"); 

        // Create a UnityWebRequest
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        
    

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Handle the successful response
            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);

        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
        }

        // Dispose of the request
        request.Dispose();
    }
}
