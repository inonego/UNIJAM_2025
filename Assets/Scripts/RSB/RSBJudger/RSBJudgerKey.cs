using System.Linq;

using AYellowpaper.SerializedCollections;

using UnityEngine;

[CreateAssetMenu(fileName = "RSBJudgerKey", menuName = "RSB/RSBJudger Key")]
// 키 바인딩을 통해 승리 조건을 선택합니다.
public class RSBJudgerKey : DefaultRSBJudger
{
    public SerializedDictionary<RSBKeyBindingType, RSBKeyBinding> KeyBindings = new SerializedDictionary<RSBKeyBindingType, RSBKeyBinding>();

    public override void SetCurrentRSB(CurrentRSB currentRSB)
    {
        base.SetCurrentRSB(currentRSB);

        var KeyBindingList = KeyBindings.Keys.ToList();

        // 랜덤으로 키 바인딩을 선택합니다.
        RSBKeyBindingType randomKeyBindingType = KeyBindingList[UnityEngine.Random.Range(0, KeyBindingList.Count)];

        currentRSB.SetKeyBinding(KeyBindings[randomKeyBindingType]);
    }
}