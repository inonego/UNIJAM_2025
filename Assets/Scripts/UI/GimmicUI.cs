using UnityEngine;
using UnityEngine.InputSystem;

public class GimmicUI : MonoBehaviour
{
    public static GimmicUI instance;

    [Header("# 기믹 종류")]
    [SerializeField] GameObject[] gimmicList;
    [SerializeField] GameObject panel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ShowGimmicText(Gimmic type)
    {
        panel.SetActive(true);
        InitObject();
        gimmicList[(int)type].SetActive(true);
        Invoke(nameof(SetPanelFalse), 1.2f);
    }

    void SetPanelFalse()
    {
        panel.SetActive(false);
    }

    private void InitObject()
    {
        for (int i = 0; i < gimmicList.Length; i++)
        {
            gimmicList[i].SetActive(false);
        }
    }
}

public enum Gimmic
{ 
    DRAW    = 0,
    WIN     = 1,
    CHANGE  = 2,
    LOSE    = 3,
    ORIGIN  = 4
}
