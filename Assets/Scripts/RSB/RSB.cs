using System;
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

[Serializable]
public class CurrentRSB
{
    public bool IsWorking { get; private set; } = false;

    public float JudgeDelayTime = 0.72f;

    private Timer Timer = new Timer();
    
    private Timer JudgeDelayTimer = new Timer();

    public readonly RSBTweakerBase CurrentTweaker;

    public readonly RSBKeyBinding CurrentKeyBinding;

    // AI가 낸 가위바위보
    public RSBType? RSBType         { get; private set; } = null;

    // 플레이어가 낸 가위바위보
    public RSBType? Input           { get; private set; } = null;

    public float ElapsedTime => Timer.Time.ElapsedTime;
    public float LeftTime => Timer.Time.LeftTime;

    public event Action<RSBType> OnInput;
    public event Action<RSBResult> OnJudged;

    public CurrentRSB(RSBTweakerBase tweaker)
    {
        Timer.OnEnded += OnTimerEnded;

        JudgeDelayTimer.OnEnded += OnJudgeDelayTimerEnded;

        CurrentTweaker = tweaker;

        CurrentKeyBinding = CurrentTweaker.GetKeyBinding();
    }

    public void SetRandomRSB()
    {
        RSBType = (RSBType)UnityEngine.Random.Range(0, 3);
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

    /// <summary>
    /// 가위바위보 게임을 시작합니다.
    /// </summary>
    /// <param name="judgeTime"></param>
    public void Start(float judgeTime)
    {
        IsWorking = true;

        Timer.Start(judgeTime);

        CurrentTweaker.Initialize();
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

                input = (RSBType)i;
            }
        }

        if (input != null)
        {
            Input = input.Value;

            OnInput?.Invoke(input.Value);

            JudgeDelayed();
        }
    }

    private void OnTimerEnded(Timer sender, Timer.EndedEventArgs e)
    {
        Stop();

        // 타이머가 끝났는데 플레이어가 아무것도 누르지 않았다면 패배합니다.
        if (Input == null)
        {
            OnJudged?.Invoke(RSBResult.Lose);
        }
        else
        {
            Judge();
        }
    }

    private void OnJudgeDelayTimerEnded(Timer sender, Timer.EndedEventArgs e)
    {
        Judge();
    }

    private void Judge()
    {
        RSBResult result = CurrentTweaker.Judge(RSBType.Value, Input.Value);

        OnJudged?.Invoke(result);
    }

    private void JudgeDelayed()
    {
        Stop();

        JudgeDelayTimer.Start(JudgeDelayTime);
    }
}

