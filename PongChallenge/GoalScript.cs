using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public enum EGoal
    {
        Left = 1,
        Right
    }
    public EGoal goalPosType = 0;
    public AudioClip GoalSFX;
    bool isTouchEnabled = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.IsSomeoneWin()) return;

        if (collision.tag == "Ball")
        {
            ScoreOnGoalEnum();
            gameManager.audioSource.PlayOneShot(GoalSFX, 0.6f);
        }
    }

    public GameObject player1ScoreGO;
    public GameObject player2ScoreGO;
    void ScoreOnGoalEnum()
    {
        if (goalPosType == EGoal.Left)
        {
            ProcessScore(player1ScoreGO);

        }
        else
        {
            ProcessScore(player2ScoreGO);
        }
    }
    WinMessage winMessage;
    void ProcessScore(GameObject scoreGO)
    {
        Score playerScore = scoreGO.GetComponent<Score>();
        playerScore.AddScore();
        playerScore.UpdateScore();

        PlayerControl playerControl = GameObject.Find("ControlManager").GetComponent<PlayerControl>();
        GameManager gameManager = playerControl.gameManager;
        gameManager.DestroyInstantiatedBall();
        gameManager.CreateNewBall();
        if (gameManager.IsSomeoneWin())
        {
            winMessage = GameObject.Find("WinMessage").GetComponent<WinMessage>();
            winMessage.ActivateWinMessage();
            gameManager.mainEvent -= playerControl.ControlPong;
            gameManager.mainEvent += RestartOnTapEvent;

            Invoke("EnableTouch", PREVENT_TOUCH_TIMER);
        }
    }

    void EnableTouch()
    {
        isTouchEnabled = true;
    }

    const float PREVENT_TOUCH_TIMER = 2.0f;
    void RestartOnTapEvent()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!isTouchEnabled) return;
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.anyKey)
        {
            gameManager.RestartGame();
        }
    }
}
