using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
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

public abstract class RSBJudgerBase : ScriptableObject
{
    public string Name;
    [TextArea(2, 5)]
    public string Description;
    public Sprite Icon;

    public CurrentRSB CurrentRSB { get; private set; } = null;

    public virtual void SetCurrentRSB(CurrentRSB currentRSB)
    {
        CurrentRSB = currentRSB;
        
        CurrentRSB.JudgeFunc = Judge;
    }

    public abstract RSBResult Judge();
}   

[CreateAssetMenu(fileName = "DefaultRSBJudger", menuName = "RSB/Default RSBJudger")]
// 일반적인 가위바위보 승리 조건
public class DefaultRSBJudger : RSBJudgerBase
{
    public override RSBResult Judge()
    {
        // 일반적인 가위바위보 승리 조건
        // 가위 > 보, 보 > 바위, 바위 > 가위
        return (RSBResult)((CurrentRSB.Input - CurrentRSB.RSBType + 3) % 3);
    }
}

[CreateAssetMenu(fileName = "RSBJudgerInverse", menuName = "RSB/RSBJudger Inverse")]
// 일반적인 가위바위보 승리 조건의 반대
public class RSBJudgerInverse : RSBJudgerBase
{
    public override RSBResult Judge()
    {
        // 일반적인 가위바위보 승리 조건의 반대
        return (RSBResult)((CurrentRSB.RSBType - CurrentRSB.Input + 3) % 3);
    }
}

[CreateAssetMenu(fileName = "RSBJudgerSame", menuName = "RSB/RSBJudger Same")]
// 비겨야지만 이길 수 있음!
public class RSBJudgerSame : RSBJudgerBase
{
    public override RSBResult Judge()
    {
        // 비겨야지만 이길 수 있음!
        return CurrentRSB.RSBType == CurrentRSB.Input ? RSBResult.Win : RSBResult.Lose;
    }
}

[CreateAssetMenu(fileName = "RSBJudgerKey", menuName = "RSB/RSBJudger Key")]
// 키 바인딩을 통해 승리 조건을 선택합니다.
public class RSBJudgerKey : DefaultRSBJudger
{
    public SerializedDictionary<RSBKeyBindingType, RSBKeyBinding> KeyBindings = new SerializedDictionary<RSBKeyBindingType, RSBKeyBinding>();

    public override void SetCurrentRSB(CurrentRSB currentRSB)
    {
        base.SetCurrentRSB(currentRSB);

        var KeyBindingList = KeyBindings.Keys.ToList();

        // 랜덤으로 키 바인딩을 선택합니다.
        RSBKeyBindingType randomKeyBindingType = KeyBindingList[UnityEngine.Random.Range(0, KeyBindingList.Count)];

        currentRSB.SetKeyBinding(KeyBindings[randomKeyBindingType]);
    }
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

    public event Action<RSBResult> OnJudged;
    public Func<RSBResult> JudgeFunc;

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

            Debug.Log(key);

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

