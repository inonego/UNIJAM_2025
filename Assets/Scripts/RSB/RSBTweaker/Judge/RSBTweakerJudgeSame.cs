using UnityEngine;

[CreateAssetMenu(fileName = "RSBTweakerJudgeSame", menuName = "RSB/RSB Tweaker/Judge Same")]
// 비겨야지만 이길 수 있음!
public class RSBTweakerJudgeSame : RSBTweakerBase
{
    public override Gimmic GimicType => Gimmic.Judge;

    public RSBResult Judge(RSBType current, RSBType input)
    {
        // 비겨야지만 이길 수 있음!
        return current == input ? RSBResult.Win : RSBResult.Lose;
    }

    public override void ApplyGimmic(SingleRSB currentRSB)
    {
        currentRSB.Judge = Judge;
    }
}