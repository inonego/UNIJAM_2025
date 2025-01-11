﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance { get; private set; }

    [Header("보스 체력 바")]
    [SerializeField] Slider hpBar;          // 보스 Hp Bar를 나타내는 Slider
    [SerializeField] float duration;        // 슬라이더 전환 속도

    [Header("보스 HP 수치")]
    [SerializeField] float maxHp;           // 보스 최대체력
    [SerializeField] float baseHp;          // 보스 초기체력

    [Header("# 보스 게이지 증감 정보")]
    [SerializeField] float plusValue;
    [SerializeField] float minusValue;

    [Header("보스 스프라이트")]
    [SerializeField] Sprite boss30;
    [SerializeField] Sprite boss50;
    [SerializeField] Sprite boss70;
    [SerializeField] Sprite boss90;
    [SerializeField] Sprite rsbLoseSprite;
    [SerializeField] Sprite rsbWinSprite;
    [SerializeField] Sprite rsbDrawSprite;

    [Header("보스 비겼을 때 물음표")]
    [SerializeField] GameObject drawEffect;

    [Header("승리 / 패배 시 이벤트")]
    public UnityEvent OnWin;
    public UnityEvent OnLose;

    [SerializeField] AudioSource timer;

    private float currentHp;                // 보스 현재체력
    private Image fillIcon;                 // Slider FIll Icon이 0에 수렴할 경우 이미지 enabled = false 처리를 위함
    private SpriteRenderer spriteRenderer;  // 보스 SpriteRender 컴포넌트

    private new Animation animation;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Image[] images = hpBar.GetComponentsInChildren<Image>();
        fillIcon = images[1];

        hpBar.value = baseHp / maxHp;
        currentHp = baseHp;
        SetSprite();

        animation = GetComponentInChildren<Animation>();

        OnWin.AddListener(Win);
        OnLose.AddListener(Lose);
    }

    private void Start()
    {
        RSBGameManager.Instance.OnRSBEnded += OnRSBEnded;
        RSBGameManager.Instance.OnJudgerChanged += (Judge) => { SoundManager.instance.PlaySFX(SFX.Gimmic); };
        RSBGameManager.Instance.OnGameEnded += () => { timer.Stop(); };
    }

    private void OnRSBEnded(RSBResult result)
    {
        ReflectionRSBValue(result);
    }

    // RSBResult Win, Lose, Draw 값을 받게 됨
    public void ReflectionRSBValue(RSBResult result)
    {
        switch(result)
        {
            // 1. 게임 이겼을 때
            case RSBResult.Win:
                GetHpBarValue(plusValue);
                spriteRenderer.sprite = rsbWinSprite;
                if (Enum.TryParse(SceneManager.GetActiveScene().name+"_RSB_Win", out SFX win))
                {
                    SoundManager.instance.PlaySFX(win);
                }
                animation.Play("Boss Good");
                break;
            // 2. 게임 비겼을 때
            case RSBResult.Draw:
                spriteRenderer.sprite = rsbDrawSprite;
                Instantiate(drawEffect);
                SoundManager.instance.PlaySFX(SFX.Draw);
                break;
            // 3. 게임 졌을 때
            case RSBResult.Lose:
                GetHpBarValue(-minusValue);
                spriteRenderer.sprite = rsbLoseSprite;
                LoseEffect.instance.ShowDamageEffect();
                animation.Play("Boss Bad");
                if (Enum.TryParse(SceneManager.GetActiveScene().name + "_RSB_Lose", out SFX lose))
                {
                    SoundManager.instance.PlaySFX(lose);
                }
                break;
        }
        Invoke(nameof(SetSprite), 0.5f);
    }



    // Hp Slider.value와 보스의 Hp 값을 조정하는 함수
    private void GetHpBarValue(float _value)
    {
        StartCoroutine(ControlHpBar(_value));
        SetHpValue(_value);
    }

    // Slider.value의 값을 보간하여 애니메이션 처리하는 함수
    private IEnumerator ControlHpBar(float _value)
    {
        float currentHpRatio = hpBar.value;
        float targetHpRatio = (currentHp + _value) / maxHp;
        float elapsedTime = 0f;
        
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t  = elapsedTime / duration;
            float interpolValue = Mathf.Lerp(currentHpRatio, targetHpRatio, t);
            SetSliderValue(interpolValue);
            yield return null;
        }

        SetSliderValue(targetHpRatio);
        CheckSliderValue();
    }

    // 보스의 체력을 설정하는 함수
    private void SetHpValue(float _value)
    {
        currentHp += _value;

        if (currentHp <= 0)
            currentHp = 0;
        else if(currentHp >= maxHp)
            currentHp = maxHp;
    }

    // Slider.value를 설정하는 함수
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

    // 보스의 Slider.value가 1이면 승리 처리
    private void CheckSliderValue()
    {
        // 이겼을때
        if(hpBar.value >= 1f)
        {
            Debug.Log("이겼습니다!");
            hpBar.gameObject.SetActive(false);

            RSBGameManager.Instance.Stop();

            OnWin?.Invoke();
        }
        else
        // 죽었을때
        if (hpBar.value <= 0f)
        {
            Debug.Log("죽었습니다!");
            hpBar.gameObject.SetActive(false);

            RSBGameManager.Instance.Stop();

            OnLose?.Invoke();
        }
    }

    // 보스 체력 비율에 따라 Sprite 이미지를 설정
    private void SetSprite()
    {
        if (hpBar.value >= 0.9f)
        {
            spriteRenderer.sprite = boss90;
        }
        else if (hpBar.value >= 0.7f)
        {
            spriteRenderer.sprite = boss70;
        }
        else if (hpBar.value >= 0.5f)
        {
            spriteRenderer.sprite = boss50;
        }
        else
        {
            spriteRenderer.sprite = boss30;
        }
    }

    void Win()
    {
        StartCoroutine(WinCoroutine());
    }

    void Lose()
    {
        StartCoroutine(LoseCoroutine());
    }

    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        SoundManager.instance.PlaySFX(SFX.Clear);
    }

    IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        SoundManager.instance.PlaySFX(SFX.Fail);
    }
}
