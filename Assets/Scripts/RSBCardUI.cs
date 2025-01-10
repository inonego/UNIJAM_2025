using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class RSBCardUI : MonoBehaviour
{
    public RSBType RSBType;
    public Image Image;
    public TextMeshProUGUI KeyLabel;

    public void SetKeyLabelText(Key key)
    {        
        KeyLabel.text = key.ToString();
    }
}
