using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public GameObject gameOverPanel;

    public Text scoreText;
    public Text highscoreText;

    public Text timerText;
    public int timerValue;

    int score = 0;
    int highscore = 0;


    public int Score{ get { return score; } }

    public int HighScore { get { return highscore; } 
        set 
        { 
            highscore = value;
            highscoreText.text = "HIGHSCORE: " + highscore.ToString();

        }
    }



    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOverPanel.SetActive(false);
        timerText.text ="Time: " +timerValue.ToString();
        StartCoroutine("CountDownTimerRoutine");

        highscore = PlayerPrefs.GetInt("highscore", 0);
        scoreText.text = score.ToString() + " POINTS";
        highscoreText.text = "HIGHSCORE: " + highscore.ToString();


        UpdateHighScoreText();
    }

    private void OnEnable()
    {
        UpdateHighScoreText();
    }

    public void UpdateHighScoreText()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        highscoreText.text = "HIGHSCORE: " + highscore.ToString();

    }

    public void AddPoint(int Points=1)
    {
        score += Points;
        scoreText.text = score.ToString() + " POINTS";
        if (score > highscore)
        {
            PlayerPrefs.SetInt("highscore", score);
            highscoreText.text = "HIGHSCORE: " + score.ToString();
        }
    }

    IEnumerator CountDownTimerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timerValue -= 1;
            timerText.text = "Time: " + timerValue.ToString();

            if(timerValue<=0)
            {
                FindObjectOfType<Slingshot>().isGameOver = true;
                GameManager.instance.GameOver();
                //gameOverPanel.SetActive(true);
                break;
            }
        }
    }
}
