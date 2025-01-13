using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // Audio Mixer를 연결

    private const string VolumeParameter = "Master"; // 노출된 매개변수 이름
    [SerializeField] Slider volumeSlider;   // Slider를 연결

    private void Awake()
    {
        float defaultVolume = 1f; // 슬라이더 기본값 (0~1 범위)
        audioMixer.SetFloat(VolumeParameter, Mathf.Log10(defaultVolume) * 20);

        float currentVolume;
        audioMixer.GetFloat(VolumeParameter, out currentVolume);

        volumeSlider.value = Mathf.Pow(10, currentVolume / 20);

        Debug.Log($"Slider Value: {volumeSlider.value}, Current Volume: {currentVolume}");
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        audioMixer.SetFloat(VolumeParameter, Mathf.Log10(value) * 20);
    }
}
