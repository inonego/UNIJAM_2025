using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "RSBINFPhase", menuName = "RSB/RSB Phase/RSB INF Phase")]
public class RSBINFPhase : RSBPhase
{
    [Header("Judger Count")]
    public int TargetMinJudgerCount = 1;
    public int TargetMaxJudgerCount = 1;
    
    public float JudgeCountPowerValue = 1f;

    [Header("Judge Time")]
    public float TargetJudgeTime = 1f;
    public float JudgeTimePowerValue = 1f;

    [Header("Boss")]
    public int TargetBossPlusValue = 4;
    public int TargetBossMinusValue = 4;
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

    private void UpdateParameters(float time)
    {
        current.JudgeTime = GetPowerValue(initial: data.JudgeTime, target: TargetJudgeTime, a: JudgeTimePowerValue, value: time);

        current.MinJudgerCount = Mathf.RoundToInt(GetPowerValue(initial: data.MinJudgerCount, target: TargetMinJudgerCount, a: JudgeCountPowerValue, value: time));
        current.MaxJudgerCount = Mathf.RoundToInt(GetPowerValue(initial: data.MaxJudgerCount, target: TargetMaxJudgerCount, a: JudgeCountPowerValue, value: time));

        current.BossPlusValue = Mathf.RoundToInt(GetPowerValue(initial: data.BossPlusValue, target: TargetBossPlusValue, a: BossPowerValue, value: time));
        current.BossMinusValue = Mathf.RoundToInt(GetPowerValue(initial: data.BossMinusValue, target: TargetBossMinusValue, a: BossPowerValue, value: time));

        current.MinusPerSecond = GetPowerValue(initial: data.MinusPerSecond, target: TargetMinusPerSecond, a: MinusPerSecondPowerValue, value: time);
    }

    public override void UpdateAll(float currentTime)
    {
        base.UpdateAll(currentTime);

        UpdateParameters(currentTime);
    }
}
