using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    private enum GameState { MAINMENU, INTRO, GAME, GAMEOVER}

    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private GameObject deers;

    [SerializeField]
    private int playMinutes;

    [HideInInspector]
    public static GameManager instance;

    private int score = 0;
    private float timer;
    private GameState gameState
    {
        get
        {
            return gameState;
        }
        set
        {
            TurnAllOff();
            switch(gameState)
            {
                case GameState.MAINMENU:
                    break;
                case GameState.INTRO:
                    break;
                case GameState.GAME:
                    deers.SetActive(true);
                    scoreText.gameObject.SetActive(true);
                    timerText.gameObject.SetActive(true);
                    DisplayScore();
                    DisplayTime();
                    break;
                case GameState.GAMEOVER:
                    break;
            }
        }
    }
    private void TurnAllOff()
    {
        deers.SetActive(false);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    private void Start()
    {
        gameState = GameState.MAINMENU;
    }

    private void Awake()
    {
        instance = this;
        timer = playMinutes * 60f;
    }

    private void Update()
    {
        if(gameState == GameState.GAME)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                DisplayTime();
            }
            else
            {
                gameState = GameState.GAMEOVER;
            }
        }
        
    }

    public void Play()
    {
        gameState = GameState.INTRO;
    }

    public void IncreaseScore()
    {
        score++;
        DisplayScore();
    }

    public void DecreaseScore()
    {
        score--;
        DisplayScore();
    }

    private void DisplayScore()
    {
        scoreText.SetText("Deers: " + score);
    }

    private void DisplayTime()
    {
        if (timer > 60)
            timerText.SetText($"{(int)(timer / 60)}:{timer % 60:00}");
        else
            timerText.SetText($"{timer%60:F0}:{timer*100%100:00}");
    }
}
