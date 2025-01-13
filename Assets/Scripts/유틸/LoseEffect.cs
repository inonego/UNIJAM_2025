using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LoseEffect : MonoBehaviour
{
    public static LoseEffect instance;

    [Header("# Damage Effect 시간")]
    [SerializeField] float startDuration;
    [SerializeField] float endDuration;
    [SerializeField] float targetValue;

    private Volume damageVolume;
    private Vignette vignette;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        damageVolume = GetComponent<Volume>();
        damageVolume.profile.TryGet(out vignette);
    }

    public void ShowDamageEffect()
    {
        Debug.Log("Damage Effect");
        StartCoroutine(AdjustVignette());
    }

    private IEnumerator AdjustVignette()
    {
        float elapsedTime = 0f;

        // 1단계: 0 -> 0.3 (0.6초 동안 증가)
        while (elapsedTime < startDuration)
        {
            elapsedTime += Time.deltaTime;
            float intensity = Mathf.Lerp(0f, targetValue, elapsedTime / startDuration);
            vignette.intensity.value = intensity;
            Debug.Log(vignette.intensity.value);
            yield return null;
        }

        // 강제로 최대 값 설정 (오차 방지)
        vignette.intensity.value = targetValue;

        // 2단계: 0.3 -> 0 (3초 동안 감소)
        elapsedTime = 0f;
        while (elapsedTime < endDuration)
        {
            elapsedTime += Time.deltaTime;
            float intensity = Mathf.Lerp(targetValue, 0f, elapsedTime / endDuration);
            vignette.intensity.value = intensity;
            yield return null;
        }

        // 강제로 최소 값 설정 (오차 방지)
        vignette.intensity.value = 0f;
    }


}
