using System;

using UnityEngine;

[Serializable]
public abstract class RSBTweakerBase : ScriptableObject
{
    public Sprite Icon;
    [TextArea(1, 1)] public string Name;
    [TextArea(2, 5)] public string Description;

    public RSBKeyBinding DefaultKeyBinding;

    public abstract Gimmic GimicType { get; }

    public virtual void Initialize() { }

    public virtual RSBKeyBinding GetKeyBinding() => DefaultKeyBinding;
    
    public abstract RSBResult Judge(RSBType current, RSBType input);
}   