using UnityEngine;

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
