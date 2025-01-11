using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RSBJudgerRandomValue
{
    public RSBJudgerBase Judger;
    public float Weight;
}

public class RSBManager : MonoBehaviour
{
#region 기본 설정
    
    [Header("RSB Key Bindings")]
    // 기본 가위바위보 키 바인딩입니다.
    public RSBKeyBinding DefaultKeyBinding = new RSBKeyBinding();

    // 남은 가위바위보 판정 조건 개수입니다.
    public int LeftJudgerCount { get; private set; } = 0;

    public RSBPhase CurrentPhase;
#endregion

#region 현재 상태

    // 현재 가위바위보 판정 조건입니다.
    public RSBJudgerBase CurrentJudger { get; private set; } = null;

    // 현재 가위바위보 상태입니다.
    public CurrentRSB CurrentRSB { get; private set; } = null;

#endregion

#region Event

    // 새로운 가위바위보 시작 시 호출됩니다.
    public event Action<CurrentRSB> OnNewRSB;
    public event Action<RSBJudgerBase> OnJudgerChanged;
    
#endregion

    private void Update()
    {
        CurrentRSB?.Update();
    }

    private CurrentRSB CreateRSB()
    {
        var CurrentRSB = new CurrentRSB();

        if (CurrentRSB.RSBType == null)
        {
            // 랜덤으로 가위바위보를 선택합니다.
            CurrentRSB.SetRandomRSB();
        }
        
        // 기본 키 바인딩을 설정합니다.
        CurrentRSB.SetKeyBinding(DefaultKeyBinding);

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
            LeftJudgerCount = UnityEngine.Random.Range(CurrentPhase.MinJudgerCount, CurrentPhase.MaxJudgerCount + 1);

            SetRandomJudger();
        }
        
        if (CurrentJudger == null)
        {
            Debug.LogError("현재 가위바위보 판정 조건이 없습니다!");

            return;
        }

        LeftJudgerCount--;

        // 새로운 가위바위보를 생성합니다.
        CurrentRSB = CreateRSB();

        // 랜덤으로 가위바위보 승리 조건을 선택합니다.
        CurrentJudger.SetCurrentRSB(CurrentRSB);

        // 가위바위보 시작 이벤트를 호출합니다.
        OnNewRSB?.Invoke(CurrentRSB);

        // CurrentRSB를 시작합니다.
        CurrentRSB.Start(CurrentPhase.JudgeTime);
    }

    // 랜덤으로 가위바위보 승리 조건을 선택합니다.
    private void SetRandomJudger()
    {
        float sum = 0f;

        for (int i = 0; i < CurrentPhase.Judgers.Count; i++)
        {
            sum += CurrentPhase.Judgers[i].Weight;
        }

        float randomValue = UnityEngine.Random.Range(0, sum);

        // 확률에 따라 가위바위보 승리 조건을 선택합니다.
        for (int i = 0; i < CurrentPhase.Judgers.Count; i++)
        {
            randomValue -= CurrentPhase.Judgers[i].Weight;

            if (randomValue < 0)
            {
                CurrentJudger = CurrentPhase.Judgers[i].Judger;
                
                OnJudgerChanged?.Invoke(CurrentJudger);

                return;
            }
        }

        if (CurrentPhase.Judgers.Count > 0)
        {
            CurrentJudger = CurrentPhase.Judgers[0].Judger;

            // 가위바위보 판정 조건 변경 이벤트를 호출합니다.
            OnJudgerChanged?.Invoke(CurrentJudger);
        }
    }

    public void Clear()
    {
        CurrentRSB.Stop();

        CurrentRSB = null;
        CurrentJudger = null;
    }

}
