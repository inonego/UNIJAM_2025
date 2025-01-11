using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "RSBPhase", menuName = "RSB/RSB Phase/RSB Phase")]
public class RSBPhase : ScriptableObject
{
    [Serializable]
    public class DATA
    {
        [Header("RSB Judger Change Count")]
        // 가위바위보 판정 조건 개수 범위입니다.
        public int MinJudgerCount = 3;
        public int MaxJudgerCount = 5;

        [Header("RSB Judgers")]
        // 가위바위보 판정 조건에 대한 가중치 목록입니다.
        public List<RSBJudgerRandomValue> Judgers = new List<RSBJudgerRandomValue>();

        public float JudgeTime = 5f;

        public int BossPlusValue = 4;
        public int BossMinusValue = 4;

        // Copy constructor.
        public DATA(DATA previous)
        {
            MinJudgerCount = previous.MinJudgerCount;
            MaxJudgerCount = previous.MaxJudgerCount;

            Judgers.AddRange(previous.Judgers);

            JudgeTime = previous.JudgeTime;

            BossPlusValue = previous.BossPlusValue;
            BossMinusValue = previous.BossMinusValue;
        }
    }

    [SerializeField] protected DATA data;

    [field: NonSerialized] public DATA current { get; private set; } = null;

    public virtual void Initialize() 
    {
        current = new DATA(data);
    }

    public virtual void UpdateAll(float currentTime)
    {

    }
}
