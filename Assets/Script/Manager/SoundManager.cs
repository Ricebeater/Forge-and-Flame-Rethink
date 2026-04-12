using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundProfile
{
    [Header("Audio Config")]
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1f;

}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayRandomSFX(SoundProfile soundProfile)
    {
        if (soundProfile == null || soundProfile.clips == null || soundProfile.clips.Length == 0) return;

        int randomIndex = Random.Range(0, soundProfile.clips.Length);
        AudioClip selectedClip = soundProfile.clips[randomIndex];

        if (selectedClip == null) return;

        sfxSource.pitch = Random.Range(0.9f, 1.1f);

        float randomBaseVolume = Random.Range(0.8f, 1.0f);
        float finalVolume = randomBaseVolume * soundProfile.volume;

        sfxSource.PlayOneShot(selectedClip, finalVolume);
    }
    public void PlaySFX(SoundProfile soundProfile)
    {
        if (soundProfile == null || soundProfile.clips == null || soundProfile.clips.Length == 0) return;

        int randomIndex = Random.Range(0, soundProfile.clips.Length);
        AudioClip selectedClip = soundProfile.clips[randomIndex];

        if (selectedClip == null) return;

        sfxSource.pitch = 1f;

        sfxSource.PlayOneShot(selectedClip, soundProfile.volume);
    }

    public void PlayMusic(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayMusicWithFade(AudioClip newClip, float volume = 1f)
    {
        if (musicSource.clip == newClip) return;
        StartCoroutine(CrossFadeMusic(newClip, volume));
    }

    IEnumerator CrossFadeMusic(AudioClip newClip, float targetVolume)
    {
        float fadeDuration = 1.0f;
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop();

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(0, targetVolume, timer / fadeDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}