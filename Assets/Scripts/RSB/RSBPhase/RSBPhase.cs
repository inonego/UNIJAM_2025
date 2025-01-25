using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class RSBTweakerRandomValue
{
    public RSBTweakerBase Tweaker;
    public float Weight;
}

[CreateAssetMenu(fileName = "RSBPhase", menuName = "RSB/RSB Phase/RSB Phase")]
public class RSBPhase : ScriptableObject
{
    [Header("RSB Judger Change Count")]
    // 가위바위보 판정 조건 개수 범위입니다.
    public int MinTweakerCount = 3;
    public int MaxTweakerCount = 5;

    [Header("RSB Judgers")]
    // 가위바위보 판정 조건에 대한 가중치 목록입니다.
    public List<RSBTweakerRandomValue> TweakerList = new List<RSBTweakerRandomValue>();

    public float JudgeTime = 5f;

    public int BossPlusValue = 4;
    public int BossMinusValue = 4;

    public float MinusPerSecond = 1f;

    public virtual void Initialize() {}

    public virtual void UpdateAll(RSBPhase basePhase, float currentTime) {}
}
