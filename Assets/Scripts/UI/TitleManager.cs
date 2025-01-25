using UnityCommunity.UnitySingleton;
using UnityEngine;


public class TitleManager : MonoSingleton<TitleManager>
{
    [Header("# 스테이지 선택 팝업")]
    [SerializeField] GameObject stagePopup;

    [Header("# 설정 팝업")]
    [SerializeField] GameObject settingPopup;

    [Header("# 게임방법 팝업")]
    [SerializeField] GameObject howToPlayPopup;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        howToPlayPopup.transform.localScale = Vector3.zero;

        stagePopup.transform.localScale = Vector3.zero;

        settingPopup.transform.localScale = Vector3.zero;
    }


    public void OnClickStagePopUpBtn()
    {
        stagePopup.transform.localScale = Vector3.one;
    }

    public void DeactiveStagePopUp()
    {
        stagePopup.transform.localScale = Vector3.zero;
    }

    public void OnClickSettingPopUpBtn()
    {
        settingPopup.transform.localScale = Vector3.one;
    }

    public void DeactiveSettingPopUp()
    {
        settingPopup.transform.localScale = Vector3.zero;

        Time.timeScale = 1f;
    }

    public void OnClickHowToPlayPopUpBtn()
    {
        howToPlayPopup.transform.localScale = Vector3.one;
    }

    public void DeactiveHowToPlayPopUp()
    {
        howToPlayPopup.transform.localScale = Vector3.zero;
    }

    public void OnClickExitBtn()
    {
        FadeManager.instance.FadeOut(onComplete: () =>
        {
#if UNITY_EDITOR
            Debug.Log("에디터 종료");
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit(); // 어플리케이션 종료
            #endif
        });
    }

    public void OnClickBtn()
    {
        SoundManager.instance.PlaySFX(SFX.BtnClick);
    }
}
