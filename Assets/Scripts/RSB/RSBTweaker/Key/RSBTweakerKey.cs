using UnityEngine;

using AYellowpaper.SerializedCollections;

using System.Linq;

[CreateAssetMenu(fileName = "RSBTweakerKey", menuName = "RSB/RSB Tweaker/Key")]
public class RSBTweakerKey : RSBTweakerBase
{
    public override Gimmic GimicType => Gimmic.Key;

    public RSBKeyBinding DefaultKeyBinding;

    public SerializedDictionary<RSBKeyBindingType, RSBKeyBinding> RandomKeyBindings = new SerializedDictionary<RSBKeyBindingType, RSBKeyBinding>();
    
    public override void ApplyGimmic(SingleRSB currentRSB)
    {
        if (RandomKeyBindings.Count == 0)
        {
            // 기본 키 바인딩으로 변경합니다.
            currentRSB.CurrentKeyBinding = DefaultKeyBinding;
        }
        else
        {
            var KeyBindingList = RandomKeyBindings.Keys.ToList();

            // 랜덤으로 키 바인딩을 선택합니다.
            RSBKeyBindingType randomKeyBindingType = KeyBindingList[UnityEngine.Random.Range(0, KeyBindingList.Count)];

            currentRSB.CurrentKeyBinding = RandomKeyBindings[randomKeyBindingType];
        }
    }
}
