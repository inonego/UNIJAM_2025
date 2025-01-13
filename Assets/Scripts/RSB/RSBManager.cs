using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RSBJudgerRandomValue
{
    public RSBTweakerBase Judger;
    public float Weight;
}

public class RSBManager : MonoBehaviour
{
#region 기본 설정
    // 남은 가위바위보 판정 조건 개수입니다.
    public int LeftJudgerCount      { get; private set; } = 0;

    public RSBPhase CurrentPhase    = null;
#endregion

#region 현재 상태

    // 현재 가위바위보 판정 조건입니다.
    public RSBTweakerBase CurrentTweaker { get; private set; } = null;

    // 현재 가위바위보 상태입니다.
    public CurrentRSB CurrentRSB { get; private set; } = null;

#endregion

#region Event

    // 새로운 가위바위보 시작 시 호출됩니다.
    public event Action<CurrentRSB> OnNewRSB;
    public event Action<RSBTweakerBase> OnTweakerChanged;
    
#endregion

    private void Update()
    {
        CurrentRSB?.Update();
    }

    private CurrentRSB CreateRSB()
    {
        var CurrentRSB = new CurrentRSB(CurrentTweaker);

        if (CurrentRSB.RSBType == null)
        {
            // 랜덤으로 가위바위보를 선택합니다.
            CurrentRSB.SetRandomRSB();
        }

        return CurrentRSB;
    }
    
    // 다음 가위바위보를 시작합니다.
    public void GoNext()
    {
        if (CurrentPhase == null)
        {
            Debug.LogError("현재 페이즈가 없습니다!");
        
            return;
        }

        // 남은 가위바위보 판정 조건 카운트가 0 이하가 되면 
        if (LeftJudgerCount <= 0)
        {
            LeftJudgerCount = UnityEngine.Random.Range(CurrentPhase.current.MinJudgerCount, CurrentPhase.current.MaxJudgerCount + 1);

            SetRandomJudger();
        }
        
        if (CurrentTweaker == null)
        {
            Debug.LogError("현재 가위바위보 판정 조건이 없습니다!");

            return;
        }

        LeftJudgerCount--;

        // 새로운 가위바위보를 생성합니다.
        CurrentRSB = CreateRSB();

        // 가위바위보 시작 이벤트를 호출합니다.
        OnNewRSB?.Invoke(CurrentRSB);

        // CurrentRSB를 시작합니다.
        CurrentRSB.Start(CurrentPhase.current.JudgeTime);
    }

    // 랜덤으로 가위바위보 승리 조건을 선택합니다.
    private void SetRandomJudger()
    {
        if (CurrentPhase.current.Judgers.Count <= 0)
        {
            Debug.LogError("가위바위보 판정 조건이 없습니다!");

            return;
        }
        
        if (CurrentPhase.current.Judgers.Count == 1)
        {
            CurrentTweaker = CurrentPhase.current.Judgers[0].Judger;

            // 가위바위보 판정 조건 변경 이벤트를 호출합니다.
            OnTweakerChanged?.Invoke(CurrentTweaker);

            return;
        }

        float sum = 0f;

        for (int i = 0; i < CurrentPhase.current.Judgers.Count; i++)
        {
            sum += CurrentPhase.current.Judgers[i].Weight;
        }

        // 이전 가위바위보 Tweaker를 저장합니다.
        RSBTweakerBase previousTweaker = CurrentTweaker;

        do
        {
            float randomValue = UnityEngine.Random.Range(0, sum);
            
            CurrentTweaker = CurrentPhase.current.Judgers[0].Judger;

            // 확률에 따라 가위바위보 승리 조건을 선택합니다.
            for (int i = 0; i < CurrentPhase.current.Judgers.Count; i++)
            {
                randomValue -= CurrentPhase.current.Judgers[i].Weight;

                if (randomValue < 0)
                {
                    CurrentTweaker = CurrentPhase.current.Judgers[i].Judger;

                    break;
                }
            }
        }
        // 이전 가위바위보 Tweaker와 같은 경우 다시 랜덤으로 선택합니다.
        while (previousTweaker == CurrentTweaker);

        // 가위바위보 판정 조건 변경 이벤트를 호출합니다.
        OnTweakerChanged?.Invoke(CurrentTweaker);
    }

    public void Clear()
    {
        CurrentRSB.Stop();

        CurrentRSB = null;
        CurrentTweaker = null;
    }

}
