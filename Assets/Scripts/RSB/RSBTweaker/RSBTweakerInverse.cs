using UnityEngine;

[CreateAssetMenu(fileName = "RSBTweakerInverse", menuName = "RSB/RSBTweaker Inverse")]
// 일반적인 가위바위보 승리 조건의 반대
public class RSBTweakerInverse : RSBTweakerBase
{

    public override RSBResult Judge(RSBType current, RSBType input)
    {
        // 일반적인 가위바위보 승리 조건의 반대
        return (RSBResult)((current - input + 3) % 3);
    }
}