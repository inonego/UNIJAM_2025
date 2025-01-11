using System.Linq;

using AYellowpaper.SerializedCollections;

using UnityEngine;

[CreateAssetMenu(fileName = "RSBTweakerKey", menuName = "RSB/RSBTweaker Key")]
// 키 바인딩을 통해 승리 조건을 선택합니다.
public class RSBTweakerKey : DefaultRSBTweaker
{
    public override Gimmic GimicType => Gimmic.CHANGE;

    public SerializedDictionary<RSBKeyBindingType, RSBKeyBinding> KeyBindings = new SerializedDictionary<RSBKeyBindingType, RSBKeyBinding>();

    public override RSBKeyBinding GetKeyBinding()
    {
        var KeyBindingList = KeyBindings.Keys.ToList();

        // 랜덤으로 키 바인딩을 선택합니다.
        RSBKeyBindingType randomKeyBindingType = KeyBindingList[UnityEngine.Random.Range(0, KeyBindingList.Count)];

        return KeyBindings[randomKeyBindingType];
    }
}