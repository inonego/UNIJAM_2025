using UnityCommunity.UnitySingleton;

using UnityEngine;

using UnityEngine.SceneManagement;

public class TitleManager : MonoSingleton<TitleManager>
{
    [Header("# 스테이지 선택 팝업")]
    [SerializeField] GameObject stagePopup;

    [Header("# 설정 팝업")]
    [SerializeField] GameObject settingPopup;

    protected override void Awake()
    {
        base.Awake();

        if(stagePopup.activeSelf)
            stagePopup.SetActive(false);

        if(settingPopup.activeSelf)
            settingPopup.SetActive(false);
    }


    public void LoadScene(string sceneName)
    {
        FadeManager.instance.FadeOut(onComplete: () => { SceneManager.LoadScene(sceneName); });
    }

    public void OnClickStagePopUpBtn()
    {
        if (!stagePopup.activeSelf)
            stagePopup.SetActive(true);
    }

    public void DeactiveStagePopUp()
    {
        if (stagePopup.activeSelf)
            stagePopup.SetActive(false);
    }

    public void OnClickSettingPopUpBtn()
    {
        if(!settingPopup.activeSelf)
            settingPopup.SetActive(true);
    }

    public void DeactiveSettingPopUp()
    {
        if(settingPopup.activeSelf)
            settingPopup.SetActive(false);

        Time.timeScale = 1f;
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
}
