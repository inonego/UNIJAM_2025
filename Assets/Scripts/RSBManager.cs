using System;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class RSBJudgerRandomValue
{
    public RSBJudgerBase Judger;
    public float Weight;
}

public class RSBManager : MonoBehaviour
{
    [Header("RSB Judger Change Count")]
    // 가위바위보 판정 조건 개수 범위입니다.
    public int MinJudgerCount = 3;
    public int MaxJudgerCount = 5;

    // 남은 가위바위보 판정 조건 개수입니다.
    public int LeftJudgerCount { get; private set; } = 0;

    // 기본 가위바위보 키 바인딩입니다.
    public RSBKeyBinding DefaultKeyBinding = new RSBKeyBinding();

    // 가위바위보 판정 조건에 대한 가중치 목록입니다.
    public List<RSBJudgerRandomValue> Judgers = new List<RSBJudgerRandomValue>();

#region 현재 상태

    // 현재 가위바위보 판정 조건입니다.
    public RSBJudgerBase CurrentJudger { get; private set; } = null;

    // 현재 가위바위보 상태입니다.
    public CurrentRSB CurrentRSB { get; private set; } = null;

#endregion

#region Event

    // 새로운 가위바위보 시작 시 호출됩니다.
    public event Action<CurrentRSB> OnGoNext;
    public event Action OnJudgerChanged;
    
#endregion

    // 다음 가위바위보를 시작합니다.
    public void GoNext()
    {
        if (LeftJudgerCount <= 0)
        {
            LeftJudgerCount = UnityEngine.Random.Range(MinJudgerCount, MaxJudgerCount + 1);

            // 랜덤으로 가위바위보 승리 조건을 선택합니다.
            SetRandomJudger();
        }
        
        LeftJudgerCount--;

        CurrentRSB = new CurrentRSB();

        // 랜덤으로 가위바위보를 선택합니다.
        CurrentRSB.SetRandomRSB();
        
        // 기본 키 바인딩을 설정합니다.
        CurrentRSB.SetKeyBinding(DefaultKeyBinding);

        // 랜덤으로 가위바위보 승리 조건을 선택합니다.
        CurrentJudger.SetCurrentRSB(CurrentRSB);

        // 가위바위보 시작 이벤트를 호출합니다.
        OnGoNext?.Invoke(CurrentRSB);
    }

    public RSBResult Judge(RSBType input)
    {
        return CurrentJudger.Judge(input);
    }

    // 랜덤으로 가위바위보 승리 조건을 선택합니다.
    private void SetRandomJudger()
    {
        float sum = 0f;

        for (int i = 0; i < Judgers.Count; i++)
        {
            sum += Judgers[i].Weight;
        }

        float randomValue = UnityEngine.Random.Range(0, sum);

        // 확률에 따라 가위바위보 승리 조건을 선택합니다.
        for (int i = 0; i < Judgers.Count; i++)
        {
            randomValue -= Judgers[i].Weight;

            if (randomValue < 0)
            {
                CurrentJudger = Judgers[i].Judger;
                
                OnJudgerChanged?.Invoke();  

                return;
            }
        }

        CurrentJudger = Judgers[0].Judger;

        // 가위바위보 판정 조건 변경 이벤트를 호출합니다.
        OnJudgerChanged?.Invoke();
    }

}
