using UnityEngine;

[CreateAssetMenu(fileName = "RSBTweakerJudge", menuName = "RSB/RSB Tweaker/Judge")]
// 일반적인 가위바위보 승리 조건
public class RSBTweakerJudge : RSBTweakerBase
{
    public override Gimmic GimicType => Gimmic.Judge;

    public RSBResult Judge(RSBType current, RSBType input)
    {
        // 일반적인 가위바위보 승리 조건
        // 가위 > 보, 보 > 바위, 바위 > 가위
        return (RSBResult)((input - current + 3) % 3);
    }

    public override void ApplyGimmic(SingleRSB currentRSB)
    {
        currentRSB.Judge = Judge;
    }
}

