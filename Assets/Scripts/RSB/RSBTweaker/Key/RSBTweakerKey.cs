using UnityEngine;

using AYellowpaper.SerializedCollections;

using System.Linq;

[CreateAssetMenu(fileName = "RSBTweakerKey", menuName = "RSB/RSB Tweaker/Key")]
public class RSBTweakerKey : RSBTweakerBase
{
    public override Gimmic GimicType => Gimmic.Key;

    private bool hasBeenSelected = false;

    public RSBKeyBinding DefaultKeyBinding;

    public SerializedDictionary<RSBKeyBindingType, RSBKeyBinding> RandomKeyBindings = new SerializedDictionary<RSBKeyBindingType, RSBKeyBinding>();
    
    public string DefaultName = "원 위치!";
    public string RandomName = "위치 변경!";

    public override void Initialize()
    {
        Name = DefaultName;

        hasBeenSelected = false;
    }

    public override void OnSelected()
    {
        hasBeenSelected = !hasBeenSelected;

        Name = hasBeenSelected ? RandomName : DefaultName;
    }

    public override void ApplyGimmic(SingleRSB currentRSB)
    {   
        if (!hasBeenSelected)
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

