using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] List<GameObject> foundations = null;
    StageManager stageManager;
    int score = 0;

    private void Awake()
    {
        stageManager = GetComponent<StageManager>();
        cardSys = GetComponent<CardSystem>();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddScore()
    {
        score++;
    }


    CardSystem cardSys;
    public void CopyLastCardAsFoundation(int copyTargetRow)
    {

        for (int h = 0; h < CardSystem.MAX_CARDS_COLUMNS; h++)
        {
            if (cardSys.CardsGOArray[h, copyTargetRow] == null) break;
            else if (cardSys.CardsGOArray[h + 1, copyTargetRow] == null)
            {
                GameObject lastCard = cardSys.CardsGOArray[h, copyTargetRow];
                Instantiate(
                    lastCard,
                    foundations[score - 1].transform.position,
                     transform.rotation);
            }
        }

        if (score >= foundations.Count)
        {
            stageManager.SetWinScreen();
        }
    }
}
