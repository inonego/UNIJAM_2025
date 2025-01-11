using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "RSBPhase", menuName = "RSB/RSB Phase/RSB Phase")]
public class RSBPhase : ScriptableObject
{
    [Header("RSB Judger Change Count")]
    // 가위바위보 판정 조건 개수 범위입니다.
    public int MinJudgerCount = 3;
    public int MaxJudgerCount = 5;

    [Header("RSB Judgers")]
    // 가위바위보 판정 조건에 대한 가중치 목록입니다.
    public List<RSBJudgerRandomValue> Judgers = new List<RSBJudgerRandomValue>();

    public float JudgeTime = 5f;
}
