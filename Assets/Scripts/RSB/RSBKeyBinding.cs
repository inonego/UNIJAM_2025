using System;
using System.Collections.Generic;
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

    [SerializeField] private Key S;
    [SerializeField] private Key R;
    [SerializeField] private Key P;

    private Key[] keys = new Key[3];
    public IReadOnlyList<Key> Keys
    {
        get
        {
            keys[0] = S;
            keys[1] = R;
            keys[2] = P;

            return keys;
        }
    }
}
