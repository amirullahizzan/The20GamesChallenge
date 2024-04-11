using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    TMP_Text scoreText;
    int playerScore = 0;
    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddScore()
    {
        playerScore++;
    }

    public int GetScore()
    {
        return playerScore;
    }

    public void UpdateScore()
    {
        scoreText.text = playerScore.ToString();
    }

   
}
