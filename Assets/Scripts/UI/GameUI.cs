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
    public Animation PlayerRSBAnimation;

    private bool HasPlayerRSBAnimationEverPlayed = false;

    [Header("RSB Card")]
    public List<RSBCardUI> RSBCardList = new List<RSBCardUI>();

    private void Start()
    {
        StageNameUI.text = StageName;

        // 각 RSB에 대해서 키 입력 텍스트 설정
        for (int i = 0; i < RSBCardList.Count; i++)
        {
            int index = i;

            RSBGameManager.Instance.OnRSBStarted += (currentRSB)=>
            {
                RSBType rsbType = RSBCardList[index].RSBType;

                Key key = currentRSB.CurrentKeyBinding.Keys[(int)rsbType];

                RSBCardList[index].SetKeyLabelText(key);
            };
        }

        // 메인 이미지 설정
        RSBGameManager.Instance.OnRSBStarted += (currentRSB) =>
        {
            RSBType rsbType = currentRSB.RSBType.Value;

            EnemyRSBImageUI.sprite = EnemySpriteDictionary[rsbType];

            currentRSB.OnInput += (input) =>
            {
                PlayerRSBImageUI.sprite = PlayerSpriteDictionary[input];

                if (!HasPlayerRSBAnimationEverPlayed)
                {
                    PlayerRSBAnimation.Play("HandAnimation");

                    HasPlayerRSBAnimationEverPlayed = true;
                }
                else
                {
                    PlayerRSBAnimation.Play("HandAnimationSecond");
                }
            };
        };

        RSBGameManager.Instance.OnJudgerChanged += (rsbJudger) =>
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
