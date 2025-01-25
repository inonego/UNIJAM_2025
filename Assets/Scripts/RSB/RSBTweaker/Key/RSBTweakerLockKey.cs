using UnityEngine;

using AYellowpaper.SerializedCollections;

using System.Linq;

[CreateAssetMenu(fileName = "RSBTweakerLockKey", menuName = "RSB/RSB Tweaker/Lock Key")]
public class RSBTweakerLockKey : RSBTweakerBase
{
    public override Gimmic GimicType => Gimmic.LockKey;

    public bool HasBeenSelected = false;

    public RSBKeyBinding DefaultKeyBinding;
    
    public string DefaultName = "봉인 해제!";
    public string LockName = "봉인!";

    public Sprite DefaultShowGimmicText;
    public Sprite LockShowGimmicText;

    public override void Initialize()
    {
        Name = DefaultName;

        HasBeenSelected = false;
    }

    public override void OnSelected()
    {
        HasBeenSelected = !HasBeenSelected;

        Name = HasBeenSelected ? LockName : DefaultName;

        ShowGimmicText = HasBeenSelected ? LockShowGimmicText : DefaultShowGimmicText;
    }

    public override void ApplyGimmic(SingleRSB currentRSB)
    {   
        for (int i = 0; i < currentRSB.CardLockList.Count; i++)
        {
            currentRSB.CardLockList[i] = false;
        }

        if (HasBeenSelected)
        {
            currentRSB.CardLockList[Random.Range(0, currentRSB.CardLockList.Count)] = true;
        }
    }
}

