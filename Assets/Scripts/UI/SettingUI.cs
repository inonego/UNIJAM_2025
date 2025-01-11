using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SettingUI : MonoBehaviour
{
    [Header("# 설정 UI")]
    [SerializeField] GameObject settingPanel;

    [SerializeField] private AudioMixer audioMixer; // Audio Mixer를 연결
    [SerializeField] Slider volumeSlider;
    private const string VolumeParameter = "Master"; // 노출된 매개변수 이름

    private void Awake()
    {
        SetSliderValue();

        // 슬라이더 값 변경 이벤트 연결
        volumeSlider.onValueChanged.AddListener(SetVolume);

        if(settingPanel.activeSelf)
            settingPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        }
    }

    private void OnEnable()
    {
        SetSliderValue();
    }

    public void SetVolume(float value)
    {
        audioMixer.SetFloat(VolumeParameter, Mathf.Log10(value) * 20);
    }

    void SetSliderValue()
    {
        float currentVolume;
        audioMixer.GetFloat(VolumeParameter, out currentVolume);
        volumeSlider.value = Mathf.Pow(10, currentVolume / 20);
        Debug.Log(audioMixer.GetFloat(VolumeParameter, out currentVolume));
        Debug.Log(volumeSlider.value);
    }

    public void OnClickCloseBtn()
    {
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OnClickRetryBtn()
    {
        FadeManager.instance.FadeOut(onComplete: () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void OnClickMainBtn()
    {
        FadeManager.instance.FadeOut(onComplete: () =>
        {
            SceneManager.LoadScene("Title");
        });
    }
}
