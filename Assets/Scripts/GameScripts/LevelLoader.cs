using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LevelLoader : MonoBehaviour
{
    string levelText;
    bool loading;


    public string LevelText
    {
        get { return levelText; }
    }

    public bool Loading
    {
        get { return loading; }
    }


    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        levelText = " ";
        loading = false;
    }

    public void LoadLevel(int levelId, int worldId)
    {
        Debug.Log($"Level: {levelId}, World: {worldId}");
        GameState.OnlineLevel = (worldId == 4);
        if (GameState.OnlineLevel){
            StartCoroutine(GetLevelTextFromBackend(levelId, worldId));
        }
        else {
            StartCoroutine(GetLevelTextFromFile(levelId, worldId));
        }
    }
    
    public void SendLevelSolution()
    {
        StartCoroutine(SendLevelSolutionHandler());
    }


    IEnumerator GetLevelTextFromFile(int levelId, int worldId)
    {
        loading = true;
        string fileName = LevelUtils.LevelName(levelId,worldId);
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            //Replace WWW with UnityWebRequest, www with webRequest and www.text with webRequest.downloadHandler.text ?
            // see https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Get.html
            WWW www = new WWW(filePath);
            yield return www;
            levelText = www.text;
        }
        else
        {
            levelText = System.IO.File.ReadAllText(filePath);
        }
        Debug.Log("Level source text:");
        Debug.Log(levelText);
        loading = false;
    }


    public string ParseJSONLevelText(string jsonLevelText){
        ProblemBackendStyle problem = JsonUtility.FromJson<ProblemBackendStyle>(jsonLevelText);
        Debug.Log(problem.network1.nodes);
        string levelText = "";
        // TODO to place the nodes, we must either write an automatic layout thing, or we must add it to the backend.
        // To have the most control, it may be best to do it in the backend.
        return leveltext;
    }

    IEnumerator GetLevelTextFromBackend(int levelId, int worldId)
    {
        loading = true;
        string fileName = LevelUtils.LevelName(levelId, worldId);
        string filePath = $"http://phylofun.remiejanssen.nl/phylofun/rearrangementproblems/{fileName}";
        //Replace WWW with UnityWebRequest, www with webRequest and www.text with webRequest.downloadHandler.text ?
        // see https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Get.html
        WWW www = new WWW(filePath);
        yield return www;
        levelText = www.text;

        Debug.Log("Level source text:");
        Debug.Log(levelText);
        string parsed_level_text = ParseJSONLevelText(levelText);
        Debug.Log(parsed_level_text);
        loading = false;
    }


    IEnumerator SendLevelSolutionHandler()
    {
        string fileName = LevelUtils.LevelName(GameState.CurrentLevel, GameState.CurrentWorld);
        string url = $"http://phylofun.remiejanssen.nl/phylofun/rearrangementproblems/{fileName}";
        
        string json_sequence = "[";
        foreach (RearrangementMove m in GameState.MovesUsedSequence){
            json_sequence += m.ToJson()+",";
        }
        json_sequence = json_sequence.TrimEnd(',') + "]";
        
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_sequence);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }    
    }


}
