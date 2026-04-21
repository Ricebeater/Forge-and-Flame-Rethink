using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource forgeSource;

    [Header("Audio Clips")]
    public AudioClip ambientClip;
    public AudioClip forgeClip;

    [Header("Settings")]
    [Range(0f, 1f)] public float ambientVolume = 0.5f;
    [Range(0f, 1f)] public float forgeVolume = 0.7f;

    void Start()
    {
        if (ambientSource != null && ambientClip != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.volume = ambientVolume;
            ambientSource.Play();
        }

        if (forgeSource != null && forgeClip != null)
        {
            forgeSource.clip = forgeClip;
            forgeSource.loop = true;
            forgeSource.volume = forgeVolume;
            forgeSource.Play();
        }
    }

    public void SetVolume(float vol)
    {
        ambientSource.volume = vol * ambientVolume;
        forgeSource.volume = vol * forgeVolume;
    }
}