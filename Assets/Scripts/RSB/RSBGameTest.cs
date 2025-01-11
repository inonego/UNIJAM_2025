using UnityEngine;

public class RSBGameTest : MonoBehaviour
{
    public void Start()
    {
        RSBGameManager.Instance.OnGameStarted += OnGameStarted;
        RSBGameManager.Instance.OnGameEnded += OnGameEnded;
        RSBGameManager.Instance.OnRSBStarted += OnRSBStarted;
        RSBGameManager.Instance.OnRSBEnded += OnRSBEnded;
        
        RSBGameManager.Instance.OnJudgerChanged += OnJudgerChanged;
    }

    private void OnGameStarted()
    {
        Debug.Log("게임 시작");
    }

    private void OnGameEnded()
    {
        Debug.Log("게임 종료");
    }

    private void OnRSBStarted(CurrentRSB currentRSB)
    {
        Debug.Log("가위바위보 시작 == " + currentRSB.RSBType);
    }

    private void OnRSBEnded(RSBResult result)
    {
        Debug.Log("가위바위보 종료 ==" + result);
    }

    private void OnJudgerChanged(RSBTweakerBase judger)
    {
        Debug.Log("판정자 변경 == " + judger.GetType());
    }
}
