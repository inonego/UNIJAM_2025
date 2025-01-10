using System;

using UnityEngine;
using UnityEngine.InputSystem;

public enum RSBKeyBindingType
{
    None, U, D
}

[Serializable]
public class RSBKeyBinding
{
    public string Name;

    public InputActionReference S;
    public InputActionReference R;
    public InputActionReference P;
}
