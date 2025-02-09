using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance { get; private set; }

#region 보스 체력

    [Header("보스 HP 수치")]
    public float maxHp;           // 보스 최대체력
    public float baseHp;          // 보스 초기체력

    public float currentHp { get; private set; }                // 보스 현재체력


    [Header("승리 / 패배 가능 여부")]
    public bool CanWin = true;
    public bool CanLose = true;

    public int RSBLoseCount = 0;
    public float MinusPerRSBLose = 10f;

#endregion

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

    [Header("# 결과 패널")]
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    [SerializeField] AudioSource audioTimerSound;

    private Animation anim;
    private SpriteRenderer spriteRenderer;  // 보스 SpriteRender 컴포넌트

    private Image fillIcon;                 // Slider FIll Icon이 0에 수렴할 경우 이미지 enabled = false 처리를 위함

    private void Awake()
    {
        if(Instance == null) Instance = this;

        anim = GetComponentInChildren<Animation>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        SetSprite();

        currentHp = baseHp;
    }

    private RSBPhase currentPhase;

    private Coroutine applyMinusPerSecondCoroutine;

    private IEnumerator ApplyMinusPerSecond()
    {
        while(true)
        {
            if (currentPhase != null)
            {
                SetHp(currentHp - currentPhase.MinusPerSecond * Time.deltaTime);
            }

            yield return null;
        }
    }

    private void Start()
    {
        StageManager.Instance.OnRSBEnded       += OnRSBEnded;
        StageManager.Instance.OnTweakerChanged += OnTweakerChanged;
        StageManager.Instance.OnStageStarted   += OnStageStarted;
        StageManager.Instance.OnStageEnded     += OnStageEnded;
        StageManager.Instance.OnPhaseChanged   += OnPhaseChanged;
    }

#region 이벤트 메서드

    private void OnRSBEnded(RSBResult result)
    {
        ReflectionRSBValue(result);
    }

    private void OnTweakerChanged(TweakerChangedEventArgs args)
    {
        SoundManager.instance?.PlaySFX(SFX.Gimmic); 
    }

    private void OnStageStarted()
    {
        applyMinusPerSecondCoroutine = StartCoroutine(ApplyMinusPerSecond());
    }

    private void OnStageEnded(bool isTimeOver)
    {
        Debug.Log("OnGameEnded");

        audioTimerSound.Stop();

        if (applyMinusPerSecondCoroutine != null)
        {
            StopCoroutine(applyMinusPerSecondCoroutine);
        }

        // 이겼을때
        if(currentHp >= maxHp && CanWin)
        {
            Win();
        }
        else
        // 죽었을때
        if ((currentHp <= 0f && CanLose) || isTimeOver)
        {
            Lose();
        }
    }

    private void OnPhaseChanged(RSBPhase phase)
    {
        currentPhase = phase;
    }

#endregion

    // RSBResult Win, Lose, Draw 값을 받게 됨
    public void ReflectionRSBValue(RSBResult result)
    {
        switch(result)
        {
            // 1. 게임 이겼을 때
            case RSBResult.Win:
                SetHp(currentHp + currentPhase.BossPlusValue);

                spriteRenderer.sprite = rsbWinSprite;

                if (Enum.TryParse(SceneManager.GetActiveScene().name+"_RSB_Win", out SFX win))
                {
                    SoundManager.instance?.PlaySFX(win);
                }
                
                anim.Play("Boss Good");
                break;

            // 2. 게임 비겼을 때
            case RSBResult.Draw:
                spriteRenderer.sprite = rsbDrawSprite;

                SoundManager.instance?.PlaySFX(SFX.Draw);

                Instantiate(drawEffect);
                break;

            // 3. 게임 졌을 때
            case RSBResult.Lose:
                Debug.Log(currentPhase.BossMinusValue + MinusPerRSBLose * RSBLoseCount);

                SetHp(currentHp - (currentPhase.BossMinusValue + MinusPerRSBLose * RSBLoseCount++));

                spriteRenderer.sprite = rsbLoseSprite;

                LoseEffect.instance.ShowDamageEffect();

                if (Enum.TryParse(SceneManager.GetActiveScene().name + "_RSB_Lose", out SFX lose))
                {
                    SoundManager.instance?.PlaySFX(lose);
                }

                anim.Play("Boss Bad");
                break;
        }
        
        Invoke(nameof(SetSprite), 0.5f);
    }

    // 보스의 체력을 설정하는 함수
    private void SetHp(float _value)
    {
        currentHp = Mathf.Clamp(_value, 0, maxHp);

        CheckHp();
    }

    private void CheckHp()
    {
        if (!StageManager.Instance.IsGameRunning) return;
        
        if(currentHp >= maxHp && CanWin || currentHp <= 0 && CanLose)
        {
            Debug.Log("Stop");

            StageManager.Instance.Stop();
        }
    }

    // 보스 체력 비율에 따라 Sprite 이미지를 설정
    private void SetSprite()
    {
        float value = currentHp / maxHp;
        
        if (value >= 0.9f)
        {
            spriteRenderer.sprite = boss90;
        }
        else if (value >= 0.7f)
        {
            spriteRenderer.sprite = boss70;
        }
        else if (value >= 0.5f)
        {
            spriteRenderer.sprite = boss50;
        }
        else
        {
            spriteRenderer.sprite = boss30;
        }
    }

#region 승리 / 패배

    void Win()
    {
        StartCoroutine(WinCoroutine());

        OnWin?.Invoke();

        Debug.Log("Win");
    }

    void Lose()
    {
        StartCoroutine(LoseCoroutine());

        OnLose?.Invoke();

        Debug.Log("Lose");
    }

    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        SoundManager.instance?.PlaySFX(SFX.Clear);
        winPanel.SetActive(true);
    }

    IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        SoundManager.instance?.PlaySFX(SFX.Fail);
        losePanel.SetActive(true);
    }

#endregion
}
