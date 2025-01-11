using System;
using System.Collections.Generic;
using inonego;
using UnityCommunity.UnitySingleton;
using UnityEngine;

[Serializable]
public class RSBPhaseTime
{
    public RSBPhase Phase;
    public float StartTime;
}

public class RSBGameManager : MonoSingleton<RSBGameManager>
{
    public RSBManager RSBManager;

#region 게임 시간 설정

    public float GameTime = 180f;
    public float IntervalTime = 1f;

    private Timer GameTimer = new Timer();

    private Timer IntervalTimer = new Timer();

    public float ElapsedTime    => GameTimer.Time.ElapsedTime;
    public float LeftTime       => GameTimer.Time.LeftTime;

#endregion

    public bool IsGameRunning => GameTimer.IsWorking;

#region 게임 이벤트

    public event Action OnGameStarted;
    public event Action OnGameEnded;
    public event Action<CurrentRSB> OnRSBStarted;
    public event Action<RSBResult> OnRSBEnded;

    public event Action<RSBTweakerBase> OnTweakerChanged;

    public event Action<RSBPhase> OnPhaseChanged;

    public List<RSBPhaseTime> PhaseTimes = new List<RSBPhaseTime>();

#endregion

#region 유니티 이벤트

    protected override void Awake()
    {
        base.Awake();

        GameTimer.OnEnded += OnGameTimerEnded;
        IntervalTimer.OnEnded += OnIntervalTimerEnded;
    }

    private void Start()
    {
        RSBManager.OnNewRSB += OnNewRSB;
        RSBManager.OnTweakerChanged += OnTweakerChanged;
        
        Start(GameTime);
    }

    private void Update()
    {
        if (float.IsFinite(LeftTime))
        {
            GameTimer.Update();

            if (IsGameRunning)
            {
                UpdatePhase();
            }
        }
        
        IntervalTimer.Update();
    }

#endregion

#region 로직 처리 메서드

    public void SetPhase(RSBPhase phase)
    {
        if (phase != null && RSBManager.CurrentPhase != phase)
        {
            RSBManager.CurrentPhase = phase;

            phase.Initialize();

            OnPhaseChanged?.Invoke(phase);

            Debug.Log("페이즈 변경!");
        }
    }

    private void UpdatePhase()
    {
        RSBPhase currentPhase = null;

        foreach (var phaseTime in PhaseTimes)
        {
            if (ElapsedTime >= phaseTime.StartTime)
            {
                currentPhase = phaseTime.Phase;
            }
        }
        
        SetPhase(currentPhase);

        currentPhase.UpdateAll(ElapsedTime);
    }

#endregion

#region 게임 시작 및 종료 메서드

    public void Start(float time)
    {
        if (PhaseTimes.Count > 0)
        {
            OnGameStarted?.Invoke();

            GameTimer.Start(time);

            SetPhase(PhaseTimes[0].Phase);

            RSBManager.GoNext();
        }
        else
        {
            Debug.LogError("페이즈가 없습니다!");
        }
    }

    public void Stop()
    {
        OnGameEnded?.Invoke();

        StopAll();

        RSBManager.Clear();
    }
    
    private void StopAll()
    {
        GameTimer.Stop();

        IntervalTimer.Stop();
    }

#endregion

#region 이벤트 메서드

    // 새로운 가위바위보 시작 시 호출
    private void OnNewRSB(CurrentRSB currentRSB)
    {
        currentRSB.OnJudged += OnCurrentRSBJudged;

        OnRSBStarted?.Invoke(currentRSB);
    }

    // 가위바위보 판정 완료 시 호출
    private void OnCurrentRSBJudged(RSBResult result)
    {
        IntervalTimer.Start(IntervalTime);

        OnRSBEnded?.Invoke(result);
    }

    // 게임 타이머 종료 시 호출
    private void OnGameTimerEnded(Timer timer, Timer.EndedEventArgs args)
    {
        Stop();

        OnGameEnded?.Invoke();
    }

    // 가위바위보 간 간격 타이머 종료 시 호출
    private void OnIntervalTimerEnded(Timer timer, Timer.EndedEventArgs args)
    {
        RSBManager.GoNext();
    }

#endregion

}

