using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonClickController : MonoBehaviour
{
    private AudioSource audioSource;
    private Button button;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            Debug.LogError("Audio Source Missing on " + gameObject.name + " Button");
        }

        button = GetComponent<Button>();
        if(button == null)
        {
            Debug.LogError("Button Component Missing on " + gameObject.name);
        }
        if(button != null){
            button.onClick.AddListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        if(button != null){
            button.onClick.RemoveListener(PlaySound);
        }
    }
}