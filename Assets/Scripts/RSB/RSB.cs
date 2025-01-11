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
public abstract class RSBJudgerBase : ScriptableObject
{
    public Sprite Icon;
    [TextArea(1, 1)] public string Name;
    [TextArea(2, 5)] public string Description;

    public CurrentRSB CurrentRSB { get; private set; } = null;

    public virtual void SetCurrentRSB(CurrentRSB currentRSB)
    {
        CurrentRSB = currentRSB;
        
        CurrentRSB.JudgeFunc = Judge;
    }

    public abstract RSBResult Judge();
}   

[Serializable]
public class CurrentRSB
{
    public bool IsWorking { get; private set; } = false;

    private Timer Timer = new Timer();

    // AI가 낸 가위바위보
    public RSBType? RSBType         { get; private set; } = null;

    // 플레이어가 낸 가위바위보
    public RSBType? Input           { get; private set; } = null;

    // 키 바인딩 목록입니다.
    public RSBKeyBinding KeyBinding { get; private set; } = null;

    public float ElapsedTime => Timer.Time.ElapsedTime;
    public float LeftTime => Timer.Time.LeftTime;

    public event Action<RSBResult> OnJudged;
    public Func<RSBResult> JudgeFunc;

    public CurrentRSB()
    {
        Timer.OnEnded += OnTimerEnded;
    }

    public void SetRandomRSB()
    {
        RSBType = (RSBType)UnityEngine.Random.Range(0, 3);
    }

    public void SetKeyBinding(RSBKeyBinding keyBinding)
    {
        KeyBinding = keyBinding;
    }

    /// <summary>
    /// 가위바위보 게임을 업데이트합니다.
    /// </summary>
    public void Update()
    {
        if (!IsWorking) return;

        Timer.Update();

        ProcessInput();
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
    }

    // 플레이어의 입력을 처리합니다.
    private void ProcessInput()
    {
        RSBType? input = null;

        for (int i = 0; i < KeyBinding.Keys.Count; i++)
        {
            Key key = KeyBinding.Keys[i];

            if (Keyboard.current[key].isPressed)
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

            Judge();
        }
    }

    private void OnTimerEnded(Timer sender, Timer.EndedEventArgs e)
    {
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

    private void Judge()
    {
        RSBResult result = JudgeFunc();

        OnJudged?.Invoke(result);

        Stop();
    }
}

