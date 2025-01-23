using System;

using UnityEngine;


public enum Gimmic
{ 
    Judge,
    Key,
    LockKey,
}

[Serializable]
public abstract class RSBTweakerBase : ScriptableObject
{
    public Sprite Icon;
    [TextArea(1, 1)] public string Name;
    [TextArea(3, 5)] public string Description;

    public Sprite ShowGimmicText;

    public abstract Gimmic GimicType { get; }

    public virtual void Initialize() {}

    public virtual void OnSelected() {}

    public abstract void ApplyGimmic(SingleRSB currentRSB);
}   