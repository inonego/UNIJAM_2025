using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public SerializedDictionary<RSBType, Sprite> SpriteDictionary = new SerializedDictionary<RSBType, Sprite>();

    public TextMeshProUGUI NameUI;
    public TextMeshProUGUI DescriptionUI;

    public Image CurrentRSBImageUI;

    public List<RSBCardUI> RSBCardList = new List<RSBCardUI>();

    private void Start()
    {
        // 각 RSB에 대해서 키 입력 텍스트 설정
        for (int i = 0; i < RSBCardList.Count; i++)
        {
            int index = i;

            RSBGameManager.Instance.OnRSBStarted += (currentRSB)=>
            {
                RSBType rsbType = RSBCardList[index].RSBType;

                Key key = currentRSB.KeyBinding.Keys[(int)rsbType];

                RSBCardList[index].SetKeyLabelText(key);
            };
        }

        // 메인 이미지 설정
        RSBGameManager.Instance.OnRSBStarted += (currentRSB) =>
        {
            RSBType rsbType = currentRSB.RSBType.Value;

            CurrentRSBImageUI.sprite = SpriteDictionary[rsbType];
        };

        RSBGameManager.Instance.OnJudgerChanged += (rsbJudger) =>
        {
            NameUI.text = rsbJudger.Name;
            DescriptionUI.text = rsbJudger.Description;
        };
    }
}
