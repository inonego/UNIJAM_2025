using System;
using inonego;
using UnityCommunity.UnitySingleton;
using UnityEngine;

public class RSBGameManager : MonoSingleton<RSBGameManager>
{
    public RSBManager RSBManager;

    public float GameTime = 180f;
    public float IntervalTime = 1f;

    private Timer GameTimer = new Timer();

    private Timer IntervalTimer = new Timer();

    public float ElapsedTime    => GameTimer.Time.ElapsedTime;
    public float LeftTime       => GameTimer.Time.LeftTime;

    public event Action OnGameStarted;
    public event Action OnGameEnded;
    public event Action<CurrentRSB> OnRSBStarted;
    public event Action<RSBResult> OnRSBEnded;

    public event Action<RSBJudgerBase> OnJudgerChanged;

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
        RSBManager.OnJudgerChanged += OnJudgerChanged;
        
        Start(GameTime);
    }

    private void Update()
    {
        GameTimer.Update();

        IntervalTimer.Update();
    }

#endregion

#region 게임 시작 및 종료 메서드

    public void Start(float time)
    {
        OnGameStarted?.Invoke();

        GameTimer.Start(time);

        RSBManager.GoNext();
    }

    public void Stop()
    {
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

