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

/// <summary>
/// 하나의 스테이지를 총괄하는 매너저 클래스입니다.
/// </summary>
public class StageManager : MonoSingleton<StageManager>
{

#region 게임 상태

    public bool IsGameRunning => GameTimer.IsWorking;
    
#endregion

#region 게임 시간 설정

    /// <summary>
    /// 전체 게임 시간입니다.
    /// </summary>
    public float GameTime = 180f;

    /// <summary>
    /// 각 가위바위보 사이의 간격 시간입니다.
    /// </summary>
    public float IntervalTime = 1f;

    private Timer GameTimer = new Timer();
    private Timer IntervalTimer = new Timer();

    public float ElapsedTime    => GameTimer.Time.ElapsedTime;
    public float LeftTime       => GameTimer.Time.LeftTime;

    /// <summary>
    /// 시간 별 페이즈에 대한 정보입니다.
    /// </summary>
    public List<RSBPhaseTime> PhaseTimes = new List<RSBPhaseTime>();

#endregion

#region 현재 상태

    /// <summary>
    /// 현재 페이즈입니다.
    /// </summary>
    public RSBPhase CurrentPhase = null;

    /// <summary>
    /// 가위바위보 Tweaker를 관리하는 컨테이너입니다.
    /// </summary>
    [field: SerializeField] public RSBTweakerContainer TweakerContainer { get; private set; } = new RSBTweakerContainer();

    /// <summary>
    /// 현재 가위바위보에 대한 정보입니다.
    /// </summary>
    public SingleRSB CurrentRSB { get; private set; } = null;

#endregion

#region 게임 이벤트
    /// <summary>
    /// 게임 시작 이벤트입니다.
    /// </summary>
    public event Action OnStageStarted;

    /// <summary>
    /// 게임 종료 이벤트입니다.
    /// </summary>
    public event Action<bool> OnStageEnded;

    /// <summary>
    /// 가위바위보 시작 이벤트입니다.
    /// </summary>
    public event Action<SingleRSB> OnRSBStarted;

    /// <summary>
    /// 가위바위보 종료 이벤트입니다.
    /// </summary>
    public event Action<RSBResult> OnRSBEnded;

    /// <summary>
    /// 현재 가위바위보의 Tweaker 변경 이벤트입니다.
    /// </summary>
    public event Action<RSBTweakerBase> OnTweakerChanged;

    /// <summary>
    /// 페이즈 변경 이벤트입니다.
    /// </summary>
    public event Action<RSBPhase> OnPhaseChanged;

#endregion

#region 유니티 이벤트

    protected override void Awake()
    {
        base.Awake();

        GameTimer.OnEnded += OnGameTimerEnded;
        IntervalTimer.OnEnded += OnIntervalTimerEnded;

        // 현재 컨테이너를 기본 설정으로 초기화합니다.
        TweakerContainer.SetToDefaultTweaker();

        TweakerContainer.OnTweakerChanged += OnTweakerChanged;
    }

    private void Start()
    {        
        Start(GameTime);
    }

    private void Update()
    {
        if (IsGameRunning)
        {
            IntervalTimer.Update();

            // 시간이 무한이 아닌 경우
            if (float.IsFinite(LeftTime))
            {
                GameTimer.Update();

                UpdatePhase(ElapsedTime);
            }
            
            CurrentRSB?.Update();
        }
    }

#endregion

#region 게임 시작 및 종료 메서드

    public void Start(float time)
    {
        // 페이즈가 있는 경우
        if (PhaseTimes.Count > 0)
        {
            // 게임 시작 이벤트를 호출합니다.
            OnStageStarted?.Invoke();

            // 게임 타이머를 시작합니다.
            GameTimer.Start(time);

            // 다음 가위바위보를 시작합니다.
            GoNext();
        }
        else
        {
            Debug.LogError("페이즈가 없습니다!");
        }
    }

    public void Stop()
    {
        if (IsGameRunning) End(false);
    }

    // 게임 마무리 작업
    private void End(bool isTimeOver)
    {
        OnStageEnded?.Invoke(isTimeOver);

        StopAll();

        CurrentRSB = null;
    }
    
    private void StopAll()
    {
        CurrentRSB.Stop();

        GameTimer.Stop();

        IntervalTimer.Stop();
    }

#endregion

#region 페이즈 관련

    /// <summary>
    /// 페이즈를 설정합니다.
    /// </summary>
    /// <param name="phase"></param>
    public void SetPhase(RSBPhase phase)
    {
        if (phase != null && CurrentPhase != phase)
        {
            CurrentPhase = Instantiate(phase);

            // 페이즈를 초기화합니다.
            phase.Initialize();

            // 페이즈 변경 이벤트를 호출합니다.
            OnPhaseChanged?.Invoke(phase);
        }
    }

    /// <summary>
    /// 페이즈를 업데이트합니다.
    /// </summary>
    /// <param name="elapsedTime"></param>
    public void UpdatePhase(float elapsedTime)
    {        
        RSBPhase currentPhase = null;

        // 각 페이즈 별로 시간이 지났는지 확인합니다.
        foreach (var phaseTime in PhaseTimes)
        {
            if (elapsedTime >= phaseTime.StartTime)
            {
                currentPhase = phaseTime.Phase;
            }
        }
        
        // 페이즈를 설정합니다.
        SetPhase(currentPhase);

        currentPhase.UpdateAll(elapsedTime);
    }

#endregion

#region 가위바위보 관련

    /// <summary>
    /// 새로운 가위바위보 게임을 만듭니다.
    /// </summary>
    /// <returns></returns>
    private SingleRSB CreateRSB()
    {   
        var CurrentRSB = new SingleRSB(TweakerContainer);

        if (CurrentRSB.AI == null)
        {
            // 랜덤으로 가위바위보를 선택합니다.
            CurrentRSB.SetRandomRSB();
        }

        return CurrentRSB;
    }
    
    /// <summary>
    /// 다음 가위바위보를 시작합니다.
    /// </summary>
    public void GoNext()
    {
        if (CurrentPhase == null)
        {
            Debug.LogError("현재 페이즈가 없습니다!");
        
            return;
        }

        // 새로운 가위바위보를 생성합니다.
        CurrentRSB = CreateRSB();

        // 페이즈에 따른 현재 기믹을 설정합니다.
        TweakerContainer.RaiseTweaker(CurrentPhase);

        // 기믹을 적용합니다.
        TweakerContainer.ApplyTo(CurrentRSB);

        // 가위바위보를 시작합니다.
        CurrentRSB.Start(CurrentPhase.JudgeTime);

        // 가위바위보 판정 완료 이벤트를 등록합니다.
        CurrentRSB.OnJudged += OnCurrentRSBJudged;

        // 가위바위보 시작 이벤트를 호출합니다.
        OnRSBStarted?.Invoke(CurrentRSB);
    }
#endregion

#region 이벤트 메서드

    // 가위바위보 판정 완료 시 호출
    private void OnCurrentRSBJudged(RSBResult result)
    {
        OnRSBEnded?.Invoke(result);

        IntervalTimer.Start(IntervalTime);
    }

    // 게임 타이머 종료 시 호출
    private void OnGameTimerEnded(Timer timer, Timer.EndedEventArgs args)
    {
        End(true);
    }

    // 가위바위보 간 간격 타이머 종료 시 호출
    private void OnIntervalTimerEnded(Timer timer, Timer.EndedEventArgs args)
    {
        GoNext();
    }

#endregion

}

