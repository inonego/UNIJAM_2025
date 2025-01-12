using TMPro;
using UnityEngine;

public class RSBINFMode : MonoBehaviour
{
    public float ElapsedTime { get; private set; } = 0f;
    public int Score { get; private set; } = 0;

    [Header("UI")]
    public TextMeshProUGUI ScoreUI;
    public TextMeshProUGUI TimeUI;

    private void Start()
    {
        RSBGameManager.Instance.OnRSBEnded += OnRSBEnded;
    }

    private void Update()
    {
        if (RSBGameManager.Instance.IsGameRunning)
        {
            ElapsedTime += Time.deltaTime;
        }

        ScoreUI.text = $"{Score}";
        TimeUI.text = $"{ElapsedTime:F0}";

        if (float.IsInfinity(RSBGameManager.Instance.LeftTime))
        {
            RSBGameManager.Instance.UpdatePhase(ElapsedTime);
        }
    }

    private void OnRSBEnded(RSBResult result)
    {
        if (result == RSBResult.Win) Score++;
    }
}
