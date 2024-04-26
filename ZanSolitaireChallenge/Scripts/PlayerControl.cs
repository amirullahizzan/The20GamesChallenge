using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    CardSystem cardSystem;

    private void Awake()
    {
        cardSystem = GetComponent<CardSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cardSystem.AttemptDrawCards();
        }
    }
}
