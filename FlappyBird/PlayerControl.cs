using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    float flapValue = 350;

    bool isGameStart = false;

    
    public bool IsGameStart()
    {
        return isGameStart;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    float rotationValue = 0;
    float rotationSpeed = 35;
    void CheckRotateBird()
    {
        if (rb.velocity.y >= 0 && transform.rotation.z < 40.0)
        {
            rotationValue += Time.deltaTime * rotationSpeed;
        }
        else
        {
            rotationValue -= Time.deltaTime * rotationSpeed;
        }
        transform.rotation = Quaternion.Euler(0, 0, rotationValue);
    }

    bool IsOOB()
    {
        return rb.transform.position.y > 5;
    }

    void ResetLevel()
    {
        SceneManager.LoadScene(0);
    }
    bool isGameOver = false;
    public bool IsGameOver()
    {
        return isGameOver;
    }
    public void SetGameOver()
    {
        rb.velocity = new Vector2(0, 0);
        rb.isKinematic = true;
        isGameOver = true;
        PlayGameOverSound();
    }
    // Update is called once per frame

    bool GetActionInput()
    {
        return Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }
    void StartGameOnPress()
    {
        if (GetActionInput() && !isGameStart)
        {
            isGameStart = true;
            rb.isKinematic = false;
        }
    }

    private AudioSource audioSource;
    public AudioSource AudioSource_
    {
        get { return audioSource; }
    }
    public AudioClip FlapSoundSFX;
    public AudioClip GameOverSoundSFX;
    
    void PlayGameOverSound()
    {
        audioSource.PlayOneShot(GameOverSoundSFX);
    }
    void PlayFlapSound()
    {
        audioSource.PlayOneShot(FlapSoundSFX);
    }

    public bool IsGame()
    {
        return !isGameOver && isGameStart;
    }
    void Update()
    {
        StartGameOnPress();

        if (isGameOver)
        {
            if (GetActionInput())
            {
                ResetLevel();
            }
            return;
        }

        if (!isGameStart)
        {
            return;
        }
        if (GetActionInput() && !IsOOB())
        {
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(Vector2.up * flapValue);
            PlayFlapSound();
        }
        CheckRotateBird();

    }
}
