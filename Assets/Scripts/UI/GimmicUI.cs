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

    /*private void Update()
    {
        if (Keyboard.current[Key.Q].wasPressedThisFrame)
            ShowGimmicText(Gimmic.DRAW);
        else if(Keyboard.current[Key.W].wasPressedThisFrame)
            ShowGimmicText(Gimmic.WIN);
        else if (Keyboard.current[Key.E].wasPressedThisFrame)
            ShowGimmicText(Gimmic.CHANGE);
        else if (Keyboard.current[Key.R].wasPressedThisFrame)
            ShowGimmicText(Gimmic.LOSE);
    }*/
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
    LOSE    = 3
}
