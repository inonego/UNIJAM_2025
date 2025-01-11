using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // Audio Mixer를 연결

    private const string VolumeParameter = "Master"; // 노출된 매개변수 이름
    private Slider volumeSlider;   // Slider를 연결

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();

        float currentVolume=0;
        audioMixer.GetFloat(VolumeParameter, out currentVolume);
        volumeSlider.value = currentVolume;

        // 슬라이더 값 변경 이벤트 연결
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        audioMixer.SetFloat(VolumeParameter, Mathf.Log10(value) * 20);
    }
}
