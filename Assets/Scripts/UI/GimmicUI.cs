using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class GimmicUI : MonoBehaviour
{
    public static GimmicUI instance;

    [SerializeField] GameObject panel;
    [SerializeField] Image gimmicText;

    private Animator animator;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        animator = gimmicText.GetComponent<Animator>();
    }

    public void ShowGimmicText(RSBTweakerBase tweaker)
    {
        panel.SetActive(true);
        InitObject();

        gimmicText.gameObject.SetActive(true);
        gimmicText.sprite = tweaker.ShowGimmicText;
        gimmicText.SetNativeSize();

        animator.SetTrigger("Show");
        Invoke(nameof(SetPanelFalse), 1.2f);
    }

    void SetPanelFalse()
    {
        panel.SetActive(false);
    }

    private void InitObject()
    {
        gimmicText.gameObject.SetActive(false);
    }
}