using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using AYellowpaper.SerializedCollections.Editor.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public SerializedDictionary<RSBType, Sprite> EnemySpriteDictionary = new SerializedDictionary<RSBType, Sprite>();
    public SerializedDictionary<RSBType, Sprite> PlayerSpriteDictionary = new SerializedDictionary<RSBType, Sprite>();

    public string StageName;

    [Header("UI")]
    public TextMeshProUGUI StageNameUI;
    public TextMeshProUGUI NameUI;
    public TextMeshProUGUI DescriptionUI;
    public TextMeshProUGUI GameTimeUI;
    public TextMeshProUGUI RSBTimeUI;

    [Header("RSB UI")]
    public Image EnemyRSBImageUI;
    public Image PlayerRSBImageUI;

    private bool IsHandVisible = false;
    
    public Animator EnemyRSBAnimator;
    public Animation PlayerRSBAnimation;

    [Header("RSB Card")]
    public float CardShuffleInterval = 0.5f;
    public List<RSBCardUI> RSBCardList = new List<RSBCardUI>();

    private IEnumerator ShowCard()
    {
        for (int i = 0; i < RSBCardList.Count; i++)
        {
            RSBCardList[i].Show();

            yield return new WaitForSeconds(CardShuffleInterval);
        }
    }

    private IEnumerator HideCard()
    {
        for (int i = 0; i < RSBCardList.Count; i++)
        {
            RSBCardList[i].Hide();

            yield return new WaitForSeconds(CardShuffleInterval);
        }
    }

    private void HideAllRSB()
    {
        EnemyRSBAnimator.SetBool("IsVisible", false);

        if (IsHandVisible)
        {
            PlayerRSBAnimation.Play("HandHide");

            IsHandVisible = false;
        }
    }

    private void Start()
    {
        StageNameUI.text = StageName;

        StartCoroutine(ShowCard());

        // 메인 이미지 설정
        RSBGameManager.Instance.OnRSBStarted += (currentRSB) =>
        {
            RSBType rsbType = currentRSB.RSBType.Value;

            // 각 RSB에 대해서 키 입력 텍스트 설정
            for (int i = 0; i < RSBCardList.Count; i++)
            {
                RSBType cardRSBType = RSBCardList[i].RSBType;

                Key key = currentRSB.CurrentKeyBinding.Keys[(int)cardRSBType];

                RSBCardList[i].SetKeyLabelText(key);
            }

            // 적 카드 이미지를 보여줍니다.
            EnemyRSBImageUI.sprite = EnemySpriteDictionary[rsbType];

            // 가위바위보 시작 시 적 카드 보여주기 애니메이션 재생
            EnemyRSBAnimator.SetBool("IsVisible", true);

            // 플레이어의 입력이 주어진 경우
            currentRSB.OnInput += (input) =>
            {
                PlayerRSBImageUI.sprite = PlayerSpriteDictionary[input];

                PlayerRSBAnimation.Play("HandShow");

                IsHandVisible = true;

                RSBCardList[(int)input].Submit();

                if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SFX.RSB);
            };

            currentRSB.OnJudged += (result) =>
            {
                HideAllRSB();
            };
        };

        RSBGameManager.Instance.OnGameEnded += () =>
        {
            StartCoroutine(HideCard());
        };

        RSBGameManager.Instance.OnTweakerChanged += (rsbJudger) =>
        {
            NameUI.text = rsbJudger.Name;
            DescriptionUI.text = rsbJudger.Description;
        };
    }

    private void Update()
    {
        var gameManager = RSBGameManager.Instance;

        GameTimeUI.text = $"{gameManager.LeftTime:F0}";

        var rsbGameManager = gameManager.RSBManager;

        if (rsbGameManager.CurrentRSB != null)
        {
            RSBTimeUI.text = $"{rsbGameManager.CurrentRSB.LeftTime:F0}";
        }
    }
}
