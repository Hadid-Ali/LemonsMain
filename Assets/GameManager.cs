using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayFabManager PlayFabManager;
    [SerializeField] GameObject GameOverText;
    int maxPlatform;


    public static GameManager instance;


    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        maxPlatform = ScoreManager.instance.Score;
        GameOverText.SetActive(true);
        PlayFabManager.SendLeaderboard(maxPlatform);
        PlayFabManager.GameOver();
    }
}