using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinMessage : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject winTextGO;
    public void ActivateWinMessage()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        winTextGO.SetActive(true);
        winTextGO.GetComponent<TMP_Text>().text = "Player " + gameManager.GetWhoWinString() + " Wins!";

    }


}
