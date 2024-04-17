using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{

    // Start is called before the first frame update
    public Action mainUpdateEvent;
    public GameObject playerGO;
    public GameObject gameStateDisplayGO;
    public Animator backgroundAnimator;

    public ObstacleSpawner obstacleSpawner;
    [SerializeField] public Score score;
    bool isPressed = false;
    public bool IsPressed => isPressed;
    bool isGameStart = false;
    public bool IsGameStart => isGameStart;
    private void Awake()
    {
        score.score = PlayerPrefs.GetInt("Score", 0);

        Application.targetFrameRate = 60;
    }
    void Start()
    {
        mainUpdateEvent += OnWaitStartGame;
        score.UpdateScore();
        if(score.score <= 0) score.score = 0;
    }

    void Update()
    {
        mainUpdateEvent?.Invoke();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            float scoreAsSpeed = score.score * 0.01f;
            obstacleSpawner.SpeedUpObstacle(0.1f + scoreAsSpeed);
        }
        else
        {
            obstacleSpawner.RevertObstacleSpeed();
        }
    }


    void StartGame()
    {
        isGameStart = true;
        backgroundAnimator.SetBool("isGameStart", true);
        ObstacleSpawner obstacleSpawner = GameObject.Find("ObstacleSpawner").GetComponent<ObstacleSpawner>();
        mainUpdateEvent += obstacleSpawner.SpawnObstacleInCooldown;
        mainUpdateEvent += obstacleSpawner.UpdateObstacles;
        mainUpdateEvent -= OnWaitStartGame;
        Rigidbody2D playerRb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        playerRb.isKinematic = false;
        mainUpdateEvent += OnPressEvent;
        mainUpdateEvent += score.UpdateBody;
        mainUpdateEvent += score.SaveGameEventRepeat;
    }

    public void OnPlayerDeath()
    {
        ObstacleSpawner obstacleSpawner = GameObject.Find("ObstacleSpawner").GetComponent<ObstacleSpawner>();
        mainUpdateEvent -= obstacleSpawner.SpawnObstacleInCooldown;
        mainUpdateEvent -= OnPressEvent;
        mainUpdateEvent -= score.UpdateBody;
        mainUpdateEvent -= score.SaveGameEventRepeat;
        gameStateDisplayGO.SetActive(true);
        gameStateDisplayGO.GetComponent<TMP_Text>().text += score.GetScore().ToString();
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.Save();
        Invoke("AddEventRestartGame", 2.0f);
    }

    void AddEventRestartGame()
    {
        mainUpdateEvent += RestartGameEvent;
    }
    void RestartGameEvent()
    {
        if (IsInputHeld())
        {
            SceneManager.LoadScene(0);
        }


    }

    void OnPressEvent()
    {
        isPressed = IsInputHeld();
    }

    void OnWaitStartGame()
    {
        if (IsInputHeld() && !isGameStart)
        {
            StartGame();
        }
    }


    bool IsInputHeld()
    {
        return Input.GetKey(KeyCode.Space) || IsTouchInputHeld();
    }
    bool IsTouchInputHeld()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }
}

[System.Serializable]
public class Score
{
public ObstacleSpawner obstacleSpawner;
    
    public TMP_Text scoreText;
    [HideInInspector] public float score = 0;
    public float GetScore() => (int)score;

    public void UpdateBody()
    {
        AddScore();
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text = ((int)score).ToString();

        PlayerPrefs.SetInt("Score", (int)score);
    }

    public void AddScore()
    {
        score += obstacleSpawner.ObstacleSpeedModifier * Time.deltaTime;
    }
    public void AddScore(float customValue)
    {
        score += customValue;
    }
    Cooldown safeCooldown = new(2.0f);

    public void SaveGameEventRepeat()
    {
        if (!safeCooldown.IsCooldown)
        {
            PlayerPrefs.Save();
            safeCooldown.StartCooldown();
            Debug.Log("safeCooldown");
        }
    }


}