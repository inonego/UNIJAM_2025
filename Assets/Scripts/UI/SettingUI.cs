using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SettingUI : MonoBehaviour
{
    public static SettingUI Instance;

    [Header("# 설정 UI")]
    [SerializeField] GameObject settingPanel;

    [SerializeField] private AudioMixer audioMixer; // Audio Mixer를 연결
    [SerializeField] Slider volumeSlider;
    private const string VolumeParameter = "Master"; // 노출된 매개변수 이름
    private bool isFading;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        SetSliderValue();

        isFading = false;
        // 슬라이더 값 변경 이벤트 연결
        volumeSlider.onValueChanged.AddListener(SetVolume);

        if(settingPanel.activeSelf)
            settingPanel.SetActive(false);
    }

    private void Update()
    {
        if (isFading)
            return;

        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            if(settingPanel.activeSelf)
            {
                settingPanel.SetActive(false);
                Time.timeScale = 1.0f;
            }
            else
            {
                settingPanel.SetActive(true);
                Time.timeScale = 0.0f;
            }
        }
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
    }

    public void OnClickCloseBtn()
    {
        Time.timeScale = 1.0f;
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OnClickRetryBtn()
    {
        Time.timeScale = 1.0f;

        FadeManager.instance.FadeOut(onComplete: () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void OnClickMainBtn()
    {
        Time.timeScale = 1.0f;

        FadeManager.instance.FadeOut(onComplete: () =>
        {
            SceneManager.LoadScene("Title");
        });
    }

    public void SetIsFading(bool isFading)
    {
        this.isFading = isFading;
    }
}
