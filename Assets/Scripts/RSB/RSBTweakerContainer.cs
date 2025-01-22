using System;

using System.Collections;
using System.Collections.Generic;

using AYellowpaper.SerializedCollections;

using UnityEngine;

public struct TweakerChangedEventArgs
{
    public RSBTweakerBase RaisedTweaker;
    public SerializedDictionary<Gimmic, RSBTweakerBase> CurrentTweakers;
}

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

    private HashSet<RSBTweakerBase> initializedTweakers = new HashSet<RSBTweakerBase>();

#region 이벤트

    /// <summary>
    /// 가위바위보 Tweaker가 변경되었을 때 호출되는 이벤트입니다.
    /// </summary>
    public event Action<TweakerChangedEventArgs> OnTweakerChanged;
    
#endregion
    
    public void SetToDefaultTweaker()
    {
        CurrentTweakers[Gimmic.Judge] = RSBTweakerManager.Instance.DefaultTweakerJudge;
        CurrentTweakers[Gimmic.Key]   = RSBTweakerManager.Instance.DefaultTweakerKey;

        foreach (var tweaker in CurrentTweakers.Values)
        {
            InitializeTweaker(tweaker);
        }
    }
    
    public void ApplyTo(SingleRSB rsb)
    {
        foreach (var tweaker in CurrentTweakers.Values)
        {
            tweaker.ApplyGimmic(rsb);
        }
    }

    private void InitializeTweaker(RSBTweakerBase tweaker)
    {
        if (!initializedTweakers.Contains(tweaker))
        {
            tweaker.Initialize();
            initializedTweakers.Add(tweaker);

            Debug.Log($"Initialized {tweaker.Name}");
        }
    }

    public void SetTweaker(RSBTweakerBase tweaker)
    {
        InitializeTweaker(tweaker);
        
        RaisedTweaker = CurrentTweakers[tweaker.GimicType] = tweaker;

        tweaker.OnSelected();

        Debug.Log($"Set {tweaker.Name}");

        OnTweakerChanged?.Invoke(new TweakerChangedEventArgs()
        {
            RaisedTweaker = tweaker,
            CurrentTweakers = CurrentTweakers
        });
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
            SetTweaker(phase.TweakerList[0].Tweaker);

            return;
        }

        // 이전 가위바위보 Tweaker를 저장합니다.
        RSBTweakerBase previousTweaker = RaisedTweaker;
        RSBTweakerBase currentTweaker = null;

        float sum = 0f;

        for (int i = 0; i < phase.TweakerList.Count; i++)
        {
            sum += phase.TweakerList[i].Weight;
        }

        if (sum <= 0)
        {
            Debug.LogError("가위바위보 판정 조건의 가중치 값이 0 이하입니다!");

            return;
        }

        do
        {
            float randomValue = UnityEngine.Random.Range(0, sum);
            
            currentTweaker = phase.TweakerList[0].Tweaker;

            // 확률에 따라 가위바위보 승리 조건을 선택합니다.
            for (int i = 0; i < phase.TweakerList.Count; i++)
            {
                randomValue -= phase.TweakerList[i].Weight;

                if (randomValue < 0)
                {
                    currentTweaker = phase.TweakerList[i].Tweaker;

                    break;
                }
            }
        }
        // 이전 가위바위보 Tweaker와 같은 경우 다시 랜덤으로 선택합니다.
        while (previousTweaker == currentTweaker);

        SetTweaker(currentTweaker);
    }

}