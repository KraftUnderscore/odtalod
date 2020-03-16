using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private enum GameState { MAINMENU, INTRO, GAME, GAMEOVER}

    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private TextMeshProUGUI continueText;
    [SerializeField]
    private GameObject backPanel;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject gameOverMenu;
    [SerializeField]
    private GameObject playerControls;
    [SerializeField]
    private GameObject deers;
    [SerializeField]
    private GameObject introMenu;
    private int introStep = 0;

    [SerializeField]
    private int playMinutes;

    private AudioSource confirmSound;

    [HideInInspector]
    public static GameManager instance;

    private int score = 0;
    private float timer;
    private GameState _gameState;
    private GameState gameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            TurnAllOff();
            switch(value)
            {
                case GameState.MAINMENU:
                    mainMenu.SetActive(true);
                    backPanel.SetActive(true);
                    break;
                case GameState.INTRO:
                    introMenu.SetActive(true);
                    backPanel.SetActive(true);
                    continueText.gameObject.SetActive(true);
                    break;
                case GameState.GAME:
                    deers.SetActive(true);
                    scoreText.gameObject.SetActive(true);
                    timerText.gameObject.SetActive(true);
                    playerControls.SetActive(true);
                    DisplayScore();
                    DisplayTime();
                    break;
                case GameState.GAMEOVER:
                    gameOverMenu.SetActive(true);
                    scoreText.gameObject.SetActive(true);
                    Time.timeScale = 0;
                    break;
            }
            _gameState = value;
        }
    }
    private void TurnAllOff()
    {
        deers.SetActive(false);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        continueText.gameObject.SetActive(false);
        introMenu.SetActive(false);
        backPanel.SetActive(false);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        playerControls.SetActive(false);
    }

    private void Start()
    {
        gameState = GameState.MAINMENU;
    }

    private void Awake()
    {
        instance = this;
        timer = playMinutes * 60f;
        confirmSound = GetComponent<AudioSource>();
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
        if(gameState == GameState.INTRO)
        {
            if(Input.GetButtonDown("Grab"))
            {
                introStep++;
                if(introStep < introMenu.transform.childCount)
                {
                    introMenu.transform.GetChild(introStep - 1).gameObject.SetActive(false);
                    introMenu.transform.GetChild(introStep).gameObject.SetActive(true);
                    confirmSound.Play();
                }
                else
                {
                    gameState = GameState.GAME;
                }
            }
        }
    }

    public void Play()
    {
        gameState = GameState.INTRO;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
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
