using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class RSBCardUI : MonoBehaviour
{
    public RSBType RSBType;
    public Image Image;
    public TextMeshProUGUI KeyLabel;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetKeyLabelText(Key key)
    {        
        KeyLabel.text = key.ToString();
    }

    public void Show()
    {
        animator.SetTrigger("Show");
    }

    public void Hide()
    {
        animator.SetTrigger("Hide");
    }

    public void Submit()
    {
        animator.SetTrigger("Submit");
    }
}
