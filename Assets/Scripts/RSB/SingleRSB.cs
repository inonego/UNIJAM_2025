using System;
using System.Collections.Generic;

using inonego;

using UnityEngine;
using UnityEngine.InputSystem;

public enum RSBType
{
    Scissors    = 0,
    Rock        = 1,
    Paper       = 2
}

public enum RSBResult
{
    Draw    = 0,
    Win     = 1,
    Lose    = 2,
}

/// <summary>
/// 하나의 가위바위보를 나타냅니다.
/// </summary>
[Serializable]
public class SingleRSB
{
    #region 상태

    public bool IsWorking { get; private set; } = false;

    #endregion

    #region 가위바위보 결과 값

    // AI가 낸 가위바위보
    public RSBType? AI         { get; private set; } = null;

    // 플레이어가 낸 가위바위보
    public RSBType? User           { get; private set; } = null;

    #endregion

    #region 기믹 로직용

    /// <summary>
    /// 키 바인딩을 설정합니다.
    /// </summary>
    public RSBKeyBinding CurrentKeyBinding;

    /// <summary>
    /// 카드의 셔플 순서를 설정합니다.
    /// 셔플은 다른 기믹이 끝나고 실행됩니다.
    /// </summary>
    public List<int> CardShuffleIndexList = new List<int>() { 1, 2, 3 };

    public List<bool> CardLockList = new List<bool>() { false, false, false };

    public delegate RSBResult JudgeAction(RSBType ai, RSBType user);

    /// <summary>
    /// 판정 로직을 설정합니다.
    /// </summary>
    public JudgeAction Judge = null;

    #endregion

    #region 시간 관리

    /// <summary>
    /// 판정하는데까지 걸리는 시간을 설정합니다.
    /// </summary>
    public float JudgeDelayTime = 0.72f;

    private Timer Timer = new Timer();
    private Timer JudgeDelayTimer = new Timer();

    public float ElapsedTime => Timer.Time.ElapsedTime;
    public float LeftTime    => Timer.Time.LeftTime;

    #endregion

    #region 이벤트

    public event Action<RSBType> OnInput;
    public event Action<RSBResult> OnJudged;

    #endregion

    #region 메인 로직

    public SingleRSB(RSBTweakerContainer container)
    {
        Timer.OnEnded += OnTimerEnded;

        JudgeDelayTimer.OnEnded += OnJudgeDelayTimerEnded;
    }

    /// <summary>
    /// 가위바위보 게임을 업데이트합니다.
    /// </summary>
    public void Update()
    {
        Timer.Update();
        JudgeDelayTimer.Update();

        if (IsWorking)
        {
            ProcessInput();
        }
    }

    #endregion

    #region 가위바위보 핵심 로직

    /// <summary>
    /// 랜덤으로 가위바위보 값을 설정합니다.
    /// </summary>
    public void SetRandomRSB()
    {
        AI = (RSBType)UnityEngine.Random.Range(0, 3);
    }

    /// <summary>
    /// 가위바위보 게임을 시작합니다.
    /// </summary>
    /// <param name="judgeTime"></param>
    public void Start(float judgeTime)
    {
        IsWorking = true;

        Timer.Start(judgeTime);
    }

    /// <summary>
    /// 가위바위보 게임을 종료합니다.
    /// </summary>
    public void Stop()
    {
        IsWorking = false;

        Timer.Stop();
        JudgeDelayTimer.Stop();
    }

    // 플레이어의 입력을 처리합니다.
    private void ProcessInput()
    {
        if (Time.timeScale <= 1E-5) return;

        RSBType? input = null;

        for (int i = 0; i < CurrentKeyBinding.Keys.Count; i++)
        {
            Key key = CurrentKeyBinding.Keys[i];

            if (Keyboard.current[key].wasPressedThisFrame)
            {
                // 키가 중복되면 무효화합니다.
                if (input != null)
                {
                    input = null;

                    break;
                }

                if (!CardLockList[i])
                {
                    // 키 바인딩에 맞춰서 입력 값을 설정합니다.
                    input = (RSBType)i;
                }
            }
        }

        // 입력을 받으면
        if (input != null)
        {
            User = input.Value;

            OnInput?.Invoke(input.Value);

            // 판정을 하긴 하는데 조금 뒤에 합니다.
            JudgeDelayed();
        }
    }

    private void OnTimerEnded(Timer sender, Timer.EndedEventArgs e)
    {
        Stop();

        // 타이머가 끝났는데 플레이어가 아무것도 누르지 않았다면 패배합니다.
        if (User == null)
        {
            OnJudged?.Invoke(RSBResult.Lose);
        }
        else
        {
            DoJudge();
        }
    }

    private void OnJudgeDelayTimerEnded(Timer sender, Timer.EndedEventArgs e)
    {
        DoJudge();
    }

    /// <summary>
    /// 실제로 판정을 처리합니다.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void DoJudge()
    {
        RSBResult result = Judge?.Invoke(AI.Value, User.Value) ?? throw new Exception("판정 메서드가 설정되지 않았습니다!");

        OnJudged?.Invoke(result);
    }

    private void JudgeDelayed()
    {
        Stop();

        JudgeDelayTimer.Start(JudgeDelayTime);
    }
}

#endregion