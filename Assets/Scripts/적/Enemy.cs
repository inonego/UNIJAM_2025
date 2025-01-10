using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance { get; private set; }

    [Header("보스 체력 바")]
    [SerializeField] Slider hpBar;    // 보스 Hp Bar를 나타내는 Slider
    [SerializeField] float duration;  // 슬라이더 전환 속도

    [Header("보스 HP 수치")]
    [SerializeField] float maxHp;     // 보스 최대체력
    [SerializeField] float baseHp;    // 보스 초기체력

    private float currentHp;          // 보스 현재체력
    private float varianceValue = 5f;
    private Image fillIcon;           // Slider FIll Icon이 0에 수렴할 경우 이미지 enabled = false 처리를 위함

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        Image[] images = hpBar.GetComponentsInChildren<Image>();
        fillIcon = images[1];

        hpBar.value = baseHp / maxHp;
        currentHp = baseHp;
        SetSliderValue(currentHp);
    }

    // RSBResult Win, Lose, Draw 값을 받게 됨
    /*public void ReflectionRSBValue(RSBResult result)
    {
        switch(result)
        {
            case RSBResult.Win:
                GetHpBarValue(varianceValue);
                // 이겼을 때의 로직 처리
                break;
            case RSBResult.Draw:
                // 비겼을 때의 로직 처리
                break;
            case RSBResult.Lose:
                GetHpBarValue(-varianceValue);
                // 졌을 때의 로직 처리
                break;
        }
    }*/

    void GetHpBarValue(float _value)
    {
        StartCoroutine(ControlHpBar(_value));
        SetHpValue(_value);
    }

    IEnumerator ControlHpBar(float _value)
    {
        float currentHpRatio = hpBar.value;
        float targetHpRatio = (currentHp + _value) / maxHp;
        float elapsedTime = 0f;
        
        // Duration 만큼 선형보간
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t  = elapsedTime / duration;
            float interpolValue = Mathf.Lerp(currentHpRatio, targetHpRatio, t);
            SetSliderValue(interpolValue);
            IsSlider1F();
            yield return null;
        }

        SetSliderValue(targetHpRatio);
    }

    private void SetHpValue(float _value)
    {
        currentHp += _value;

        if (currentHp <= 0)
            currentHp = 0;
        else if(currentHp >= maxHp)
            currentHp = maxHp;
    }

    private void SetSliderValue(float _interpolValue)
    {
        if(_interpolValue <= 0.03)
        {
            hpBar.value = 0f;
            fillIcon.enabled = false;
        }
        else
        {
            hpBar.value = _interpolValue;
            fillIcon.enabled = true;
        }
    }

    private void IsSlider1F()
    {
        if(hpBar.value >= 1f)
        {
            // 이겼을 때의 로직 처리
        }
    }
}
