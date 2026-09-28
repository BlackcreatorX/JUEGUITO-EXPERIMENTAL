using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        float savedBGM = PlayerPrefs.GetFloat("VolumeBGM", 0.5f);
        float savedSFX = PlayerPrefs.GetFloat("VolumeSFX", 0.5f);

        bgmSlider.value = savedBGM;
        sfxSlider.value = savedSFX;

        SoundManager.Instance.SetMusicVolume(savedBGM);
        SoundManager.Instance.SetSFXVolume(savedSFX);

        bgmSlider.onValueChanged.AddListener(ActualizarVolumenMusica);
        sfxSlider.onValueChanged.AddListener(ActualizarVolumenSFX);
    }

    private void ActualizarVolumenMusica(float valor)
    {
        SoundManager.Instance.SetMusicVolume(valor);
        PlayerPrefs.SetFloat("VolumeBGM", valor);
    }

    private void ActualizarVolumenSFX(float valor)
    {
        SoundManager.Instance.SetSFXVolume(valor);
        PlayerPrefs.SetFloat("VolumeSFX", valor);
    }

    private void OnDestroy()
    {
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        PlayerPrefs.Save();
    }
}