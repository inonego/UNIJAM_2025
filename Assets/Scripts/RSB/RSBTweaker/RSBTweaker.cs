using System;

using UnityEngine;

public enum RSBTweakerType
{
    Key,
    Judge,
}

[Serializable]
public abstract class RSBTweakerBase : ScriptableObject
{
    public Sprite Icon;
    [TextArea(1, 1)] public string Name;
    [TextArea(3, 5)] public string Description;

    public abstract Gimmic GimicType { get; }

    public abstract void ApplyGimmic(SingleRSB currentRSB);
}   