using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        StartCoroutine(GetLevelTextFromFile(levelId,worldId));
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



}
