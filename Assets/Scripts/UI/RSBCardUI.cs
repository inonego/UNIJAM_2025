using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class RSBCardUI : MonoBehaviour
{
    public RSBType RSBType;
    public Image Image;
    public TextMeshProUGUI KeyLabel;

    [Header("카드 상태")]
    public bool IsCardEnabled = true;
    public GameObject XMark;

    [Header("카드 색상")]
    public Color EnabledColor;
    public Color DisabledColor;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Image.color = IsCardEnabled ? EnabledColor : DisabledColor;
        
        XMark.SetActive(!IsCardEnabled);
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
