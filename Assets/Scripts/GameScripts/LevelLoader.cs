using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class LevelLoader : MonoBehaviour
{
    string levelText;
    bool loading;
    static float DEFAULT_SEPARATION = 0.05f;


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
        if (GameState.OnlineLevel){
            StartCoroutine(GetLevelTextFromBackend(levelId));
        }
        else {
            Debug.Log($"Level: {levelId}, World: {worldId}");
            StartCoroutine(GetLevelTextFromFile(levelId, worldId));
        }
    }
    
    public void SendLevelSolution(List<List<int>> isomorphism)
    {
        StartCoroutine(SendLevelSolutionHandler(isomorphism));
    }


    IEnumerator GetLevelTextFromFile(int levelId, int worldId)
    {
        loading = true;
        string fileName = LevelUtils.LevelName(levelId,worldId);
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest webRequest = new UnityWebRequest(filePath);
            yield return webRequest;
            levelText = webRequest.downloadHandler.text;
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
        ProblemBackendStyle problem = JsonConvert.DeserializeObject<ProblemBackendStyle>(jsonLevelText);

        string levelText = $"S,{DEFAULT_SEPARATION}\r\n";
        levelText += $"GOAL, {problem.goal_length}\r\n";
        
        // nodes network1
        foreach (List<double> entry in problem.network1.node_positions){
            levelText+= $"V,0,{(int)entry[0]},{entry[1]},{entry[2]}\r\n";
        }

        // nodes network2
        foreach (List<double> entry in problem.network2.node_positions){
            levelText+= $"V,1,{(int)entry[0]},{entry[1]},{entry[2]}\r\n";
        }

        // edges network1
        foreach (List<int> edge in problem.network1.edges){
            levelText+= $"E,0,{edge[0]},{edge[1]}\r\n";
        }

        // edges network2
        foreach (List<int> edge in problem.network2.edges){
            levelText+= $"E,1,{edge[0]},{edge[1]}\r\n";
        }

        // labels
        // label format:
        // L,graph_id_1,node_id_1,graph_id_2,node_id_2,graph_id_3,node_id_3,..
        // hence, some parsing is needed

        Dictionary<int, List<(int graph_id, int node_id)>> labelDict = new Dictionary<int,List<(int graph_id, int node_id)>>();
        foreach (List<int> label in problem.network1.labels){
            if (!labelDict.ContainsKey(label[1])){
                labelDict[label[1]] = new List<(int graph_id, int node_id)>();
            } 
            labelDict[label[1]].Add((0,label[0]));
        }
        foreach (List<int> label in problem.network2.labels){
            if (!labelDict.ContainsKey(label[1])){
                labelDict[label[1]] = new List<(int graph_id, int node_id)>();
            } 
            labelDict[label[1]].Add((1,label[0]));
        }
        
        foreach (KeyValuePair<int, List<(int graph_id, int node_id)>> entry in labelDict){
            string labelText = "L";
            foreach ((int graph_id, int node_id) pair in entry.Value){
                labelText += $",{pair.graph_id},{pair.node_id}";
            }
            levelText += $"{labelText}\r\n";
        }

        // TODO parse move type
        if (problem.move_type == "rSPR moves" || problem.move_type == "tail moves"){
            levelText += "M,T\r\n";
        }
        if (problem.move_type == "rSPR moves" || problem.move_type == "head moves"){
            levelText += "M,H\r\n";
        }
        
        return levelText;
    }

    IEnumerator GetLevelTextFromBackend(int levelId)
    {
        loading = true;
        string filePath = $"http://phylofun.remiejanssen.nl/api/rearrangementproblems/{levelId}/";
        Debug.Log($"JSON level source text for {filePath}:");
        WWW webRequest = new WWW(filePath);
        yield return webRequest;
        levelText = webRequest.text;
        Debug.Log(levelText);
        Debug.Log("Parsing JSON to level source text:");
        try {
            levelText = ParseJSONLevelText(levelText);
        } 
        catch (System.NullReferenceException e){
            levelText = null;
        }        
        Debug.Log(levelText);
        loading = false;
    }


    IEnumerator SendLevelSolutionHandler(List<List<int>> isomorphism)
    {
        int levelId = GameState.CurrentLevel;
        string url = "http://phylofun.remiejanssen.nl/api/rearrangementsolutions/";
        
        string json_sequence = "[";
        foreach (RearrangementMove m in GameState.MovesUsedSequence){
            json_sequence += m.ToJson()+",";
        }
        json_sequence = json_sequence.TrimEnd(',') + "]";
        
        string isomorphismString = JsonConvert.SerializeObject(isomorphism);
        
        string solution_json = $"{{\"sequence\": {json_sequence}, \"problem\": \"http://phylofun.remiejanssen.nl/api/rearrangementproblems/{levelId}/\", \"isomorphism\": {isomorphismString}}}";
        
        Debug.Log(solution_json);
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(solution_json);
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
