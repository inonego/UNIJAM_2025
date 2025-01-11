using UnityEngine;

[CreateAssetMenu(fileName = "DefaultRSBTweaker", menuName = "RSB/Default RSBTweaker")]
// 일반적인 가위바위보 승리 조건
public class DefaultRSBTweaker : RSBTweakerBase
{
    public override Gimmic GimicType => Gimmic.WIN;

    public override RSBResult Judge(RSBType current, RSBType input)
    {
        // 일반적인 가위바위보 승리 조건
        // 가위 > 보, 보 > 바위, 바위 > 가위
        return (RSBResult)((input - current + 3) % 3);
    }
}
