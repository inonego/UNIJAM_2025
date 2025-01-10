using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

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
    public string Description;
    public Sprite Icon;

    public CurrentRSB CurrentRSB { get; private set; } = null;

    public virtual void SetCurrentRSB(CurrentRSB currentRSB)
    {
        CurrentRSB = currentRSB;
    }

    public abstract RSBResult Judge(RSBType input);
}   

[CreateAssetMenu(fileName = "DefaultRSBJudger", menuName = "RSB/Default RSBJudger")]
// 일반적인 가위바위보 승리 조건
public class DefaultRSBJudger : RSBJudgerBase
{
    public override RSBResult Judge(RSBType input)
    {
        // 일반적인 가위바위보 승리 조건
        // 가위 > 보, 보 > 바위, 바위 > 가위
        return (RSBResult)((input - CurrentRSB.RSBType + 3) % 3);
    }
}

[CreateAssetMenu(fileName = "RSBJudgerInverse", menuName = "RSB/RSBJudger Inverse")]
// 일반적인 가위바위보 승리 조건의 반대
public class RSBJudgerInverse : RSBJudgerBase
{
    public override RSBResult Judge(RSBType input)
    {
        // 일반적인 가위바위보 승리 조건의 반대
        return (RSBResult)((CurrentRSB.RSBType - input + 3) % 3);
    }
}

[CreateAssetMenu(fileName = "RSBJudgerSame", menuName = "RSB/RSBJudger Same")]
// 비겨야지만 이길 수 있음!
public class RSBJudgerSame : RSBJudgerBase
{
    public override RSBResult Judge(RSBType input)
    {
        // 비겨야지만 이길 수 있음!
        return CurrentRSB.RSBType == input ? RSBResult.Win : RSBResult.Lose;
    }
}

[CreateAssetMenu(fileName = "RSBJudgerKey", menuName = "RSB/RSBJudger Key")]
// 키 바인딩을 통해 승리 조건을 선택합니다.
public class RSBJudgerKey : DefaultRSBJudger
{
    public SerializedDictionary<RSBKeyBindingType, RSBKeyBinding> KeyBindings = new SerializedDictionary<RSBKeyBindingType, RSBKeyBinding>();

    private List<RSBKeyBindingType> KeyBindingList = null;

    public override void SetCurrentRSB(CurrentRSB currentRSB)
    {
        base.SetCurrentRSB(currentRSB);

        if (KeyBindingList == null)
        {
            KeyBindingList = KeyBindings.Keys.ToList();
        }

        // 랜덤으로 키 바인딩을 선택합니다.
        RSBKeyBindingType randomKeyBindingType = KeyBindingList[UnityEngine.Random.Range(0, KeyBindingList.Count)];

        currentRSB.SetKeyBinding(KeyBindings[randomKeyBindingType]);
    }
}

[Serializable]
public class CurrentRSB
{
    // AI가 낸 가위바위보
    public RSBType RSBType          { get; private set; }

    // 키 바인딩 목록입니다.
    public RSBKeyBinding KeyBinding { get; private set; } = null;

    public void SetRandomRSB()
    {
        RSBType = (RSBType)UnityEngine.Random.Range(0, 3);
    }

    public void SetKeyBinding(RSBKeyBinding keyBinding)
    {
        KeyBinding = keyBinding;
    }
}

