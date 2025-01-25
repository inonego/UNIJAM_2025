using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    [Header("# 페이드 시간")]
    [SerializeField] private float fadeDuration = 1f;

    private Image defaultImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        defaultImage = GetComponentInChildren<Image>();
        defaultImage.enabled = false;
    }

    private void Start()
    {
        FadeIn();
    }
    
    public void FadeOut(Image image = null, Action onComplete = null)
    {
        if (image == null)
            image = defaultImage;

        SettingUI.Instance.SetIsFading(true);

        StartCoroutine(FadeOutCoroutine(image, onComplete));
        SoundManager.instance.FadeOutAudioGroup();

    }

    public void FadeIn(Image image = null, Action onComplete = null)
    {
        if (image == null)
            image = defaultImage;

        SettingUI.Instance.SetIsFading(true);

        StartCoroutine(FadeInCoroutine(image, onComplete));

        SoundManager.instance.FadeInAudioGroup();
    }

    private IEnumerator FadeInCoroutine(Image image, Action onComplete)
    {
        image.enabled = true;

        float elapsedTime = 0f;
        SetAlpha(image, 1f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            SetAlpha(image, alpha);
            yield return null;
        }

        // 오차방지 차원에서 투명도를 0으로 설정
        SetAlpha(image, 0f);

        // 이미지 비활성화
        image.enabled = false;

        // ESC 방지 해제
        SettingUI.Instance.SetIsFading(false);

        // fadeDuration만큼의 딜레이
        yield return new WaitForSeconds(fadeDuration);

        // 콜백함수 호출
        onComplete?.Invoke();
    }

    private IEnumerator FadeOutCoroutine(Image image, Action onComplete)
    {
        image.enabled = true;

        float elapsedTime = 0f;
        SetAlpha(image, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            SetAlpha(image, alpha);
            yield return null;
        }

        // 오차방지 차원에서 투명도를 1로 설정
        SetAlpha(image, 1f);

        // ESC 방지 해제
        SettingUI.Instance.SetIsFading(false);

        // fadeDuration만큼의 딜레이
        yield return new WaitForSeconds(fadeDuration);

        // 콜백함수 호출
        onComplete?.Invoke();
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

}
