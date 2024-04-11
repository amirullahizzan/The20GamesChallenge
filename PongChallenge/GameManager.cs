using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject BallGO;
    GameObject instantiatedBallGO;
    public Score score1;
    public Score score2;
    [HideInInspector] public AudioSource audioSource;
    
    const int MAX_SCORE = 5;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        Application.targetFrameRate = 60;
        instantiatedBallGO = Instantiate(BallGO);
    }
    public Action mainEvent;
    private void Update()
    {
        mainEvent?.Invoke();
    }


    public void DestroyInstantiatedBall()
    {
        Destroy(instantiatedBallGO);
    }
    public void CreateNewBall()
    {
     instantiatedBallGO = Instantiate(BallGO);
    }

  
    public bool IsSomeoneWin()
    {
        return score1.GetScore() >= MAX_SCORE || score2.GetScore() >= MAX_SCORE;
    }
    public string GetWhoWinString()
    {
        if (score1.GetScore() >= MAX_SCORE)
        {
            return "1";
        }
        else if (score2.GetScore() >= MAX_SCORE)
        {
            return "2";
        }

        return "ERROR WIN";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
