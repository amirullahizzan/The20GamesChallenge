using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scorer : MonoBehaviour
{
    // Start is called before the first frame update
    int score = 0;
    public TMP_Text scoreText;
    void Start()
    {
    }

    // Update is called once per frame

    void UpdateText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
    
    public AudioClip GetPointSFX;
    public void PlayGetPointSound()
    {
        PlayerControl playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
        playerControl.AudioSource_.PlayOneShot(GetPointSFX);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Obstacle"))
        {
            score++;
            UpdateText();
            PlayGetPointSound();
            print(score);
        }
    }
}
