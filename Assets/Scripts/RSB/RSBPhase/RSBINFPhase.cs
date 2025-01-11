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
    public float TargetTime = 60f;

    public float JudgeTimePowerValue = 1f;
    public float MinJudgeTime = 1f;

    public override void Initialize()
    {
        base.Initialize();
    }

    private float GetPowerValue(float initial, float min, float a, float time)
    {
        return (initial - min) * Mathf.Pow(a, -time) + min;
    }

    private void UpdateParameters(float time)
    {
        current.JudgeTime = GetPowerValue(initial: data.JudgeTime, min: MinJudgeTime, a: JudgeTimePowerValue, time: time);

        current.MinJudgerCount = Mathf.RoundToInt(GetPowerValue(initial: data.MinJudgerCount, min: TargetMinJudgerCount, a: JudgeCountPowerValue, time: time));
        current.MaxJudgerCount = Mathf.RoundToInt(GetPowerValue(initial: data.MaxJudgerCount, min: TargetMaxJudgerCount, a: JudgeCountPowerValue, time: time));

        Debug.Log($"MinJudgerCount: {current.MinJudgerCount}, MaxJudgerCount: {current.MaxJudgerCount}");
    }

    public override void UpdateAll(float currentTime)
    {
        base.UpdateAll(currentTime);

        UpdateParameters(currentTime);
    }
}
