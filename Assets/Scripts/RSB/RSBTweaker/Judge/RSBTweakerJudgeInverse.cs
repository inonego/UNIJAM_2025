using UnityEngine;

[CreateAssetMenu(fileName = "RSBTweakerJudgeInverse", menuName = "RSB/RSB Tweaker/Judge Inverse")]
// 일반적인 가위바위보 승리 조건의 반대
public class RSBTweakerJudgeInverse : RSBTweakerBase
{
    public override Gimmic GimicType => Gimmic.Judge;

    public RSBResult Judge(RSBType current, RSBType input)
    {
        // 일반적인 가위바위보 승리 조건의 반대
        return (RSBResult)((current - input + 3) % 3);
    }

    public override void ApplyGimmic(SingleRSB currentRSB)
    {
        currentRSB.Judge = Judge;
    }
}