using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GimmicUI : MonoBehaviour
{
    public static GimmicUI instance;

    [Header("# 기믹 종류")]
    [SerializeField] SerializedDictionary<RSBTweakerBase, GameObject> gimmicList;
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
    public void ShowGimmicText(RSBTweakerBase tweaker)
    {
        panel.SetActive(true);
        InitObject();
        gimmicList[tweaker].SetActive(true);
        Invoke(nameof(SetPanelFalse), 1.2f);
    }

    void SetPanelFalse()
    {
        panel.SetActive(false);
    }

    private void InitObject()
    {
        foreach (var gimmic in gimmicList)
        {
            gimmic.Value.SetActive(false);
        }
    }
}