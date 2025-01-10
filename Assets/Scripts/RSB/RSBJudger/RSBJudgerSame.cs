using UnityEngine;

[CreateAssetMenu(fileName = "RSBJudgerSame", menuName = "RSB/RSBJudger Same")]
// 비겨야지만 이길 수 있음!
public class RSBJudgerSame : RSBJudgerBase
{
    public override RSBResult Judge()
    {
        // 비겨야지만 이길 수 있음!
        return CurrentRSB.RSBType == CurrentRSB.Input ? RSBResult.Win : RSBResult.Lose;
    }
}