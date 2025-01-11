using UnityEngine;

[CreateAssetMenu(fileName = "RSBTweakerSame", menuName = "RSB/RSBTweaker Same")]
// 비겨야지만 이길 수 있음!
public class RSBTweakerSame : RSBTweakerBase
{
    public override RSBResult Judge(RSBType current, RSBType input)
    {
        // 비겨야지만 이길 수 있음!
        return current == input ? RSBResult.Win : RSBResult.Lose;
    }
}