using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerButton : MonoBehaviour
{
    public string PlayerPrefsKey = "MusicMuted";

    public AudioSource audioSource;

    public Renderer speakerRenderer;
    private Color originalColor;

    public string PlayerTag = "HandTag";

    void Start()
    {
        if (speakerRenderer != null)
        {
            originalColor = speakerRenderer.material.color;
        }

        if (audioSource != null)
        {
            audioSource.mute = PlayerPrefs.GetInt(PlayerPrefsKey, 0) == 1;
            UpdateSpeakerColor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag) && audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
            PlayerPrefs.SetInt(PlayerPrefsKey, audioSource.mute ? 1 : 0);
            PlayerPrefs.Save();
            UpdateSpeakerColor();
        }
    }

    private void UpdateSpeakerColor()
    {
        if (speakerRenderer == null) return;
        speakerRenderer.material.color = audioSource != null && audioSource.mute ? Color.red : originalColor;
    }
}
