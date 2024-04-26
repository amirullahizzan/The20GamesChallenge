using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [SerializeField] GameObject winScreenGO;

    bool isGameStart = false;
    public bool IsGameStart => isGameStart;
    bool isGameWin = false;
    public bool IsGameWin => isGameWin;
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetWinScreen();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (!isGameStart && Input.GetMouseButtonDown(0))
        {
            isGameStart = true;
            GetComponent<CardSystem>().PlayDistributeCardSFX();
        }

        UpdateTime();
    }

    public void SetWinScreen()
    {
        winScreenGO.SetActive(true);
        Destroy(GetComponent<ClickManager>());
        Destroy(GetComponent<CardSystem>());
    }

    float timeSecond = 0;
    float timeMinute = 0;
    void UpdateTime()
    {
        timeSecond += Time.deltaTime;
        if (timeSecond >= 60)
        {
            timeMinute++;
            timeSecond = 0;
        }
        UpdateTimeText();
    }
    [SerializeField] TMP_Text gameTimeText;
    void UpdateTimeText()
    {
        gameTimeText.text = ((int)timeMinute).ToString() + " : " + ((int)timeSecond).ToString();
    }

    internal void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
