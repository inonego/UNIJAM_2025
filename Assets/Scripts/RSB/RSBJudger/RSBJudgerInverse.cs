using UnityEngine;

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