using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Canales de Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Mixer y Ajustes")]
    public AudioMixer mainMixer;

    private Coroutine activeFadeCoroutine;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void ProcessAudioInstructions(AudioInstructions instructions)
    {
        if (instructions == null) return;

        if (!string.IsNullOrEmpty(instructions.sfx))
            PlaySFX(instructions.sfx);

        if (!string.IsNullOrEmpty(instructions.bgmAction))
            HandleBGMAction(instructions);
    }

    private void PlaySFX(string clipName)
    {
        AudioClip clip = (AudioClip)Resources.Load($"SFX/{clipName}");

        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX no encontrado en Resources/SFX/: {clipName}");
        }
    }

    private void HandleBGMAction(AudioInstructions instructions)
    {
        AudioClip clip = string.IsNullOrEmpty(instructions.bgm) ? null : (AudioClip)Resources.Load($"BGM/{instructions.bgm}");

        if (activeFadeCoroutine != null) StopCoroutine(activeFadeCoroutine);

        switch (instructions.bgmAction)
        {
            case "play":
                if (clip != null)
                {
                    bgmSource.clip = clip;
                    bgmSource.volume = 1f;
                    bgmSource.Play();
                }
                break;

            case "stop":
                bgmSource.Stop();
                Resources.UnloadUnusedAssets();
                break;

            case "fadeIn":
                if (clip != null)
                {
                    bgmSource.clip = clip;
                    bgmSource.Play();
                    activeFadeCoroutine = StartCoroutine(FadeRoutine(bgmSource, 0f, 1f, instructions.fadeDuration));
                }
                break;

            case "fadeOut":
                activeFadeCoroutine = StartCoroutine(FadeRoutine(bgmSource, bgmSource.volume, 0f, instructions.fadeDuration));
                break;

            case "volumeDown":
                activeFadeCoroutine = StartCoroutine(FadeRoutine(bgmSource, bgmSource.volume, 0.3f, instructions.fadeDuration));
                break;

            case "volumeUp":
                activeFadeCoroutine = StartCoroutine(FadeRoutine(bgmSource, bgmSource.volume, 1f, instructions.fadeDuration));
                break;
        }
    }

    private IEnumerator FadeRoutine(AudioSource source, float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0;
        source.volume = startVolume;

        if (duration <= 0)
        {
            source.volume = targetVolume;
            yield break;
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        source.volume = targetVolume;

        if (targetVolume == 0f)
        {
            source.Stop();
            Resources.UnloadUnusedAssets();
        }
    }

    public void SetMusicVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("VolumeBGM", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("VolumeSFX", Mathf.Log10(sliderValue) * 20);
    }
}