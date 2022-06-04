using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{

    [Header("Username Related")]
    [SerializeField] GameObject userNamePanel;
    [SerializeField] Button confirmButton;
    [SerializeField] InputField userNameInput;


    [Header("Game Related")]
    [SerializeField] CameraMove cameraMove;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] GameObject masterSlingShot;

    [SerializeField] GameObject background;

    [SerializeField] Button playButton;
    [SerializeField] Button leaderboardButton;



    [Header("leaderboard related")]
    [SerializeField] GameObject leaderboardPanel;
    [SerializeField] Transform rowsParent;
    [SerializeField] GameObject rowPrefab;

    public static PlayFabManager instance;


    private string username = "";

    //Start is called before the first frame update
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

        Login();


        confirmButton.onClick.AddListener(OnUserNameConfirm);

        userNamePanel.SetActive(false);

        cameraMove.enabled = false;
        scoreManager.enabled = false;
        masterSlingShot.SetActive(false);

        playButton.gameObject.SetActive(true);
        leaderboardButton.gameObject.SetActive(true);
        background.SetActive(true);


    }
    private void OnDestroy()
    {
        confirmButton.onClick.RemoveListener(OnUserNameConfirm);
    }


    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }
    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");

        GetLeaderboard();


        if (result.InfoResultPayload.PlayerProfile != null)
            username = result.InfoResultPayload.PlayerProfile.DisplayName;

        

    }

    private void OnUserNameConfirm()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userNameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);


    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("updated display name");

        userNamePanel.SetActive(false);
        //Time.timeScale = 1f;
        //SceneManager.LoadScene(1);

        cameraMove.enabled = true;
        scoreManager.enabled = true;
        masterSlingShot.SetActive(true);

        playButton.gameObject.SetActive(false);
        leaderboardButton.gameObject.SetActive(false);
        background.SetActive(false);

    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error while executing Playfab call!");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "LemonScoreBoard", // <- ✏️ LemonScoreBoard
                    //StatisticName = "TestLeaderboard", // <- ✏️ LemonScoreBoard
                    Value = score
                    //Value = Random.Range(10,100) <- ⭐️ Use this to test out random send data
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful leaderboard sent!");
    }





    //********************************************************************************************
    // Get leaderboard
    /// <summary>
    /// ON LEADERBOARD BUTTON
    /// </summary>
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "LemonScoreBoard",
            StartPosition = 0,
            MaxResultsCount = 70
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);

    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        int highscore = 0;
        if (result.Leaderboard.Count > 0)
        {
            for (int i = 0; i < 1; i++)
            {
                highscore = result.Leaderboard[i].StatValue;
            }
        }

        Debug.Log("Highscore value from leaderboard" + highscore);

        PlayerPrefs.SetInt("highscore", highscore);

        //ScoreManager.instance.UpdateHighScoreText();




        //foreach (var item in result.Leaderboard)
        //{

        //    GameObject newGo = Instantiate(rowPrefab, rowsParent);

        //    Text[] texts = newGo.GetComponentsInChildren<Text>();
        //    texts[0].text = (item.Position + 1).ToString();
        //    texts[1].text = item.DisplayName;
        //    texts[2].text = item.StatValue.ToString();



        //    Debug.Log(string.Format("PLACE: {0} | ID: {1} | VALUE: {2}",
        //        item.Position, item.PlayFabId, item.StatValue));
        //}
    }


    public void OnPlayButton()
    {

        if (string.IsNullOrEmpty(username))
        {
            userNamePanel.SetActive(true);

        }
        else
        {
            userNamePanel.SetActive(false);

            //Time.timeScale = 1;
            //SceneManager.LoadScene(1);
            cameraMove.enabled = true;
            scoreManager.enabled = true;
            masterSlingShot.SetActive(true);

            playButton.gameObject.SetActive(false);
            leaderboardButton.gameObject.SetActive(false);
            background.SetActive(false);
        }
    }

    public void OnLeaderboardButton()
    {
        ShowLeaderboard();
    }

    public void OnBackLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    private void ShowLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "LemonScoreBoard",
            StartPosition = 0,
            MaxResultsCount = 70
        };
        PlayFabClientAPI.GetLeaderboard(request, UpdateLeaderboardEntries, OnError);

    }



    private void UpdateLeaderboardEntries(GetLeaderboardResult result)
    {
        //destroying old entries
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {

            GameObject newGo = Instantiate(rowPrefab, rowsParent);

            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();



            Debug.Log(string.Format("PLACE: {0} | ID: {1} | VALUE: {2}",
                item.Position, item.PlayFabId, item.StatValue));
        }

        //enabling leaderboard UI after everything filled up
        leaderboardPanel.SetActive(true);

    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        cameraMove.enabled = false;
        scoreManager.enabled = false;
        masterSlingShot.SetActive(false);

        Cursor.visible = true; //showing the mouse cursor
        Cursor.lockState = CursorLockMode.None; //unlocked the cursor
    }
}
