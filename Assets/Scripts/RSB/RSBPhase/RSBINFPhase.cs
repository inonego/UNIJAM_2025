using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "RSBINFPhase", menuName = "RSB/RSB Phase/RSB INF Phase")]
public class RSBINFPhase : RSBPhase
{
    [Header("Judger Count")]
    public int TargetMinTweakerCount = 1;
    public int TargetMaxTweakerCount = 1;
    
    public float TweakerCountPowerValue = 1f;

    [Header("Judge Time")]
    public float TargetJudgeTime = 1f;
    public float JudgeTimePowerValue = 1f;

    [Header("Boss")]
    public int TargetBossPlusValue = 4;
    public float TargetBossMinusMultiplier = 2.15f;
    public float BossPowerValue = 1f;

    [Header("Minus")]
    public float TargetMinusPerSecond = 1f;
    public float MinusPerSecondPowerValue = 1f;

    public override void Initialize()
    {
        base.Initialize();
    }

    private float GetPowerValue(float initial, float target, float a, float value)
    {
        return (initial - target) * Mathf.Pow(a, -value) + target;
    }

    private void UpdateParameters(RSBPhase basePhase, float time)
    {
        JudgeTime = GetPowerValue(initial: basePhase.JudgeTime, target: TargetJudgeTime, a: JudgeTimePowerValue, value: time);

        MinTweakerCount = Mathf.RoundToInt(GetPowerValue(initial: basePhase.MinTweakerCount, target: TargetMinTweakerCount, a: TweakerCountPowerValue, value: time));
        MaxTweakerCount = Mathf.RoundToInt(GetPowerValue(initial: basePhase.MaxTweakerCount, target: TargetMaxTweakerCount, a: TweakerCountPowerValue, value: time));

        float bossPlusValue = GetPowerValue(initial: basePhase.BossPlusValue, target: TargetBossPlusValue, a: BossPowerValue, value: time);

        BossPlusValue = Mathf.RoundToInt(bossPlusValue);
        BossMinusValue = Mathf.RoundToInt(bossPlusValue * TargetBossMinusMultiplier);

        MinusPerSecond = GetPowerValue(initial: basePhase.MinusPerSecond, target: TargetMinusPerSecond, a: MinusPerSecondPowerValue, value: time);
    }

    public override void UpdateAll(RSBPhase basePhase, float currentTime)
    {
        base.UpdateAll(basePhase, currentTime);

        UpdateParameters(basePhase, currentTime);
    }
}
