using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider voiceSlider;

    private void Start()
    {
        if (AudioManager.Instance == null) return;

        SetupSlider(musicSlider, AudioManager.Instance.MusicVolume, AudioManager.Instance.SetMusicVolume);
        SetupSlider(sfxSlider, AudioManager.Instance.SfxVolume, AudioManager.Instance.SetSfxVolume);
        SetupSlider(voiceSlider, AudioManager.Instance.VoiceVolume, AudioManager.Instance.SetVoiceVolume);
    }

    private void OnDestroy()
    {
        if (musicSlider != null) musicSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();
        if (voiceSlider != null) voiceSlider.onValueChanged.RemoveAllListeners();
    }

    private void SetupSlider(Slider slider, float value, UnityEngine.Events.UnityAction<float> onChanged)
    {
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(onChanged);
    }
}
