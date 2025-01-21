using System;

using System.Collections;
using System.Collections.Generic;

using AYellowpaper.SerializedCollections;

using UnityEngine;

[Serializable]
public class RSBTweakerContainer
{
    /// <summary>
    /// 남은 Tweaker가 변경되기 위한 횟수입니다.
    /// </summary>
    public int LeftTweakerCount      { get; private set; } = 0;

    /// <summary>
    /// 현재 출현한 가위바위보 Tweaker입니다.
    /// </summary>
    public RSBTweakerBase RaisedTweaker { get; private set; } = null;

    public SerializedDictionary<Gimmic, RSBTweakerBase> CurrentTweakers { get; private set; } = new SerializedDictionary<Gimmic, RSBTweakerBase>();

#region 이벤트

    /// <summary>
    /// 가위바위보 Tweaker가 변경되었을 때 호출되는 이벤트입니다.
    /// </summary>
    public event Action<RSBTweakerBase> OnTweakerChanged;
    
#endregion
    
    public void SetToDefaultTweaker()
    {
        CurrentTweakers[Gimmic.Judge] = RSBTweakerManager.Instance.DefaultTweakerJudge;
        CurrentTweakers[Gimmic.Key]   = RSBTweakerManager.Instance.DefaultTweakerKey;
    }
    
    public void ApplyTo(SingleRSB rsb)
    {
        foreach (var tweaker in CurrentTweakers.Values)
        {
            tweaker.ApplyGimmic(rsb);
        }
    }

    public void SetTweaker(RSBTweakerBase tweaker)
    {
        RaisedTweaker = CurrentTweakers[tweaker.GimicType] = tweaker;
    }
    
    public void RaiseTweaker(RSBPhase phase)
    {
        // 남은 가위바위보 판정 조건 카운트가 0 이하가 되면 
        if (LeftTweakerCount <= 0)
        {
            LeftTweakerCount = UnityEngine.Random.Range(phase.MinTweakerCount, phase.MaxTweakerCount + 1);
            
            // 새로운 Tweaker를 선택합니다.
            SetRandomTweaker(phase);
        }

        LeftTweakerCount--;
    }

    private void SetRandomTweaker(RSBPhase phase)
    {
        if (phase.TweakerList.Count <= 0)
        {
            Debug.LogError("가위바위보 판정 조건이 없습니다!");

            return;
        }
        
        if (phase.TweakerList.Count == 1)
        {
            SetTweaker(RaisedTweaker);

            // 가위바위보 판정 조건 변경 이벤트를 호출합니다.
            OnTweakerChanged?.Invoke(RaisedTweaker);

            return;
        }

        float sum = 0f;

        for (int i = 0; i < phase.TweakerList.Count; i++)
        {
            sum += phase.TweakerList[i].Weight;
        }

        // 이전 가위바위보 Tweaker를 저장합니다.
        RSBTweakerBase previousTweaker = RaisedTweaker;

        do
        {
            float randomValue = UnityEngine.Random.Range(0, sum);
            
            SetTweaker(phase.TweakerList[0].Tweaker);

            // 확률에 따라 가위바위보 승리 조건을 선택합니다.
            for (int i = 0; i < phase.TweakerList.Count; i++)
            {
                randomValue -= phase.TweakerList[i].Weight;

                if (randomValue < 0)
                {
                    SetTweaker(phase.TweakerList[i].Tweaker);

                    break;
                }
            }
        }
        // 이전 가위바위보 Tweaker와 같은 경우 다시 랜덤으로 선택합니다.
        while (previousTweaker == RaisedTweaker);

        // 가위바위보 판정 조건 변경 이벤트를 호출합니다.
        OnTweakerChanged?.Invoke(RaisedTweaker);
    }

}