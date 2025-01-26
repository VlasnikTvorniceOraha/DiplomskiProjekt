using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private JSONPasrser jsonParser;

    //objects for storing data from server, all are SerializeField so that values can be seen in editor
    [Header("Data from server")]

    //object for storing token
    private TokenInfo tokenInfo;

    //object for storing list of blueprints - 
    [SerializeField] private BlueprintsInfo blueprintsInfo;
    //object for storing specific blueprint information - one blueprint that needs to be loaded
    [SerializeField] private BlueprintInf blueprintInf;

    //actions for login UI
    public Action onLoginFailed; //action for login failed - it sends information that the login failed to the script that subscribes to this action
    public Action onLoginSuccess; // action for login success - it sends information that the login was successful to the script that subscribes to this action
    public Action<BlueprintsInfo> onListBlueprintsReady; // action for list of blueprints ready - it sends information that the list of blueprints is ready to the script that subscribes to this action

    //token for authorization - get from web after login
    private string token;

    public Action<string> SaveToken;
    public Action<BlueprintInf> BlueprintInformation;




    /// <summary>
    /// Method that calls coroutine for login into user account.
    /// Intended to be connected to a LogIn button in UI.
    /// </summary>
    /// <param name="username">username</param>
    /// <param name="password">password</param>
    public void LogIn(string username, string password)
    {
        StartCoroutine(LogInCoroutine(username, password));
    }

    /// <summary>
    /// Method sends web request for login.
    /// As result it gets token for authorization as JSON file, which is then parsed into tokenInfo object.
    /// The token is saved in token variable for further use.
    /// </summary>
    /// <param name="username">username</param>
    /// <param name="pass">password</param>
    /// <returns> waits for response from server</returns>
    IEnumerator LogInCoroutine(string username, string pass)
    {        
        //Don't change this string, it is a query for login
        string postCont = "{\"query\": \"mutation Login($model: AccountLoginDtoInput!) { login(model: $model) { token } }\", \"variables\": { \"model\": { \"username\": \"" + username + "\", \"password\": \"" + pass + "\" } } }";

        using (UnityWebRequest queryLogin = new UnityWebRequest("http://161.53.19.97:5000/graphql", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(postCont);
            queryLogin.uploadHandler = new UploadHandlerRaw(jsonToSend);
            queryLogin.SetRequestHeader("Content-Type", "application/json");
            queryLogin.downloadHandler = new DownloadHandlerBuffer();

            yield return queryLogin.SendWebRequest();

            if (queryLogin.result != UnityWebRequest.Result.Success)
            {
                //if login failed
                //onLoginFailed?.Invoke();
                Debug.LogError("Error: " + queryLogin.error);
                Debug.LogError("Response Code: " + queryLogin.responseCode);
                Debug.LogError("Response Text: " + queryLogin.downloadHandler.text);
            }
            else
            {
                //if login success
                onLoginSuccess?.Invoke();
                tokenInfo = TokenInfo.CreateFromJSON(queryLogin.downloadHandler.text); // parses JSON file into tokenInfo object
                token = tokenInfo.data.login.token; //gets token from tokenInfo object
                SaveToken?.Invoke(token); //sends token to the script that subscribes to this action 
                GetBlueprintsList(); // calls next step - getting list of blueprints
            }
        }
    }

    /// <summary>
    /// Gets list of blueprints from server to populate the UI list of blueprints.
    /// Finally, onListBlueprintsReady action is called to inform everyone that list of blueprints is ready
    /// </summary>
    public void GetBlueprintsList()
    {
        StartCoroutine(GetJsonList());
    }

    /// <summary>
    /// Method sends web request for getting list of blueprints.
    /// As result it gets list of blueprints as JSON file, which is then parsed into blueprintsInfo object.
    /// Finally calls onListBlueprintsReady action to inform everyone that list of blueprints is ready.
    /// </summary>
    /// <returns>waits for response from server</returns>
    IEnumerator GetJsonList()
    {
        //Don't change this string, it is a query for getting list of blueprints
        string postCont = "{\"query\": \"query Blueprints { blueprints { totalCount items { id userId name}}}\",\"operationName\": \"Blueprints\",\"variables\": { }}";
        
        using (UnityWebRequest queryBlueprints = UnityWebRequest.PostWwwForm("http://161.53.19.97:5000/graphql", postCont))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(postCont);
            queryBlueprints.uploadHandler = new UploadHandlerRaw(jsonToSend);
            queryBlueprints.SetRequestHeader("Content-Type", "application/json");
            //adding token to header for authorization - withouth this it would not be possible to get data from server
            queryBlueprints.SetRequestHeader("Authorization", "Bearer " + token);
            queryBlueprints.downloadHandler = new DownloadHandlerBuffer();

            yield return queryBlueprints.SendWebRequest();

            if (queryBlueprints.result != UnityWebRequest.Result.Success)
            {
                //if getting list of blueprints failed
                Debug.LogError(queryBlueprints.error);
            }
            else
            {
                //if getting list of blueprints success
                blueprintsInfo = BlueprintsInfo.CreateFromJSON(queryBlueprints.downloadHandler.text); //parses JSON file into blueprintsInfo object
                onListBlueprintsReady?.Invoke(blueprintsInfo); //send list of blueprints to the script that subscribes to this action 
            }
        }
    }

    /// <summary>
    /// Calls coroutine for getting specific blueprint
    /// </summary>
    /// <param name="id"> blueprint id</param>
    /// <param name="name">blueprint name</param>
    public void GetBlueprint(string id, string name)
    {
        StartCoroutine(GetJSONFile(id));
    }

    /// <summary>
    /// Method sends web request for getting a specific blueprint, search by id.
    /// As result it gets specific blueprint as JSON file, which is then parsed into blueprintInf object.
    /// Blueprint content is in contentVR field of blueprintInf object.
    /// Finally calls jsonParser to parse blueprint content for building the house model.
    /// </summary>
    /// <param name="id">blueprint id</param>
    /// <returns>waits for respond from server</returns>
    IEnumerator GetJSONFile(string id)
    {
        //Don't change this string, it is a query for getting specific blueprint
        string postCont = "{\"query\": \"query getBlueprint($id: Int!) { blueprint(where: { id: { eq: $id } }) {contentVR contentAR content id name } }\",\"operationName\": \"getBlueprint\",  \"variables\": { \"id\": " + id + " }}";
        
        using (UnityWebRequest queryBlueprint = UnityWebRequest.PostWwwForm("http://161.53.19.97:5000/graphql", postCont))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(postCont);
            queryBlueprint.uploadHandler = new UploadHandlerRaw(jsonToSend);
            queryBlueprint.SetRequestHeader("Content-Type", "application/json");
            //adding token to header for authorization - withouth this it would not be possible to get data from server
            queryBlueprint.SetRequestHeader("Authorization", "Bearer " + token);
            queryBlueprint.downloadHandler = new DownloadHandlerBuffer();

            yield return queryBlueprint.SendWebRequest();

            if (queryBlueprint.result != UnityWebRequest.Result.Success)
            {
                //if getting specific blueprint failed
                Debug.LogError(queryBlueprint.error);
            }
            else
            {
                //if getting specific blueprint success
                blueprintInf = BlueprintInf.CreateFromJSON(queryBlueprint.downloadHandler.text); //parses JSON file into blueprintInf object
                yield return blueprintInf; // wait for blueprintInf to be ready
                jsonParser.ParseJson(blueprintInf.data.blueprint.contentVR); //call JSONParser.cs to parse blueprint content for building the house model
            }
        }
    }
}
