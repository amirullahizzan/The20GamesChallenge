using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPerLetter : MonoBehaviour
{
    [SerializeField] TextPerLetter previousTarget; //Target self to trigger without Target
    [SerializeField] float setPerLetterCooldown = 2.0f;
    [HideInInspector] public bool IsTextDisplayed = false;
    TMP_Text tmpText;
    string textOnInit;
    string currentDisplayedText = "";
    Cooldown textPerLetterCooldown = new();
    AudioSource audioSource;
    [SerializeField] AudioClip OnAppearSFX;
    void PlaySFX()
    {
        if (audioSource && OnAppearSFX) audioSource.PlayOneShot(OnAppearSFX);
    }
    // Start is called before the first frame update
    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        textPerLetterCooldown.SetNewCooldown(setPerLetterCooldown);
        textOnInit = tmpText.text;
        tmpText.text = "";
    }


    void Update()
    {
        if (!previousTarget)
        {
            if (textPerLetterCooldown.IsCooldown) return;

            if (currentDisplayedText.Length < textOnInit.Length)
            {
                currentDisplayedText += textOnInit[currentDisplayedText.Length];
                tmpText.text = currentDisplayedText;
                PlaySFX();
                textPerLetterCooldown.StartCooldown();
            }
            else
            {
                IsTextDisplayed = true;
                Destroy(this);
            }
        }
    }

}
