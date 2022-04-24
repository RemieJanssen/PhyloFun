using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class LevelSelectScienceScreen : MonoBehaviour
{
    #region fields
    [SerializeField]
    GameObject[] levelButtons;
    [SerializeField]
    GameObject NextButton;
    [SerializeField]
    GameObject PreviousButton;
    int numberOfButtons;
    List<int> levelIdsCurrentPage;
    string currentPageUrl;
    string nextPageUrl = null;
    string previousPageUrl = null;
    bool requestingLevels = false;
    bool waitingForLevels = false;
    #endregion


    void Start()
    {
        numberOfButtons = levelButtons.Length;
        currentPageUrl = $"http://phylofun.remiejanssen.nl/api/rearrangementproblems/?page_size={numberOfButtons}";
        LoadButtons();
    }

    void Update()
    {
        if (waitingForLevels && !requestingLevels){
            SetButtons();
            waitingForLevels = false;
        }
    }

    void SetButtons()
    {
        Debug.Log($"Hello, setting {numberOfButtons} buttons");
        for (int i = 0; i < numberOfButtons; i++)
        {
            GameObject button = levelButtons[i];
            LevelButton buttonScript = button.GetComponent<LevelButton>();

            if (i < levelIdsCurrentPage.Count){
                int id = levelIdsCurrentPage[i];            
                buttonScript.LevelNumber = id;
                button.SetActive(true);
                buttonScript.Active = true;
            }
            else {
                //button not present because the level does not exist
                button.SetActive(false);
            }
        }
        // Now set the previous and next buttons
        NextButton.SetActive(nextPageUrl!=null);
        PreviousButton.SetActive(previousPageUrl!=null);
    }

    void LoadButtons()
    {
        // Set all the level buttons.
        StartCoroutine(GetLevelIdsFromBackend());
        waitingForLevels = true;
    }

    public void NextLevels()
    {
        currentPageUrl = nextPageUrl;
        LoadButtons();
    }

    public void PreviousLevels()
    {
        currentPageUrl = previousPageUrl;
        LoadButtons();
    }



    IEnumerator GetLevelIdsFromBackend()
    {
        requestingLevels = true;
        UnityWebRequest uwr = new UnityWebRequest(currentPageUrl, "GET");
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();
        string result = uwr.downloadHandler.text;
        Debug.Log("Parsing JSON to levelIds");
        Debug.Log(result);
        ProblemListBackendStyle problems = JsonConvert.DeserializeObject<ProblemListBackendStyle>(result);
        levelIdsCurrentPage = (from problem in problems.results select problem.id).ToList();
        nextPageUrl = problems.next;
        previousPageUrl = problems.previous;
        Debug.Log(levelIdsCurrentPage.Count);
        Debug.Log(nextPageUrl);
        Debug.Log(problems.results[0]);
        requestingLevels = false;
    }

}
