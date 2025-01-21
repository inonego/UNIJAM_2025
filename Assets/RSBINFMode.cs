using TMPro;
using UnityEngine;

public class RSBINFMode : MonoBehaviour
{
    public float ElapsedTime { get; private set; } = 0f;
    public int Score { get; private set; } = 0;

    [Header("UI")]
    public TextMeshProUGUI ScoreUI;
    public TextMeshProUGUI TimeUI;
    public TextMeshProUGUI ResultScoreUI;
    public TextMeshProUGUI ResultTimeUI;

    private void Start()
    {
        StageManager.Instance.OnRSBEnded += OnRSBEnded;
    }

    private void Update()
    {
        if (StageManager.Instance.IsGameRunning)
        {
            ElapsedTime += Time.deltaTime;
        }

        ScoreUI.text = $"{Score}";
        TimeUI.text = $"{ElapsedTime:F0}";

        ResultScoreUI.text = $"{Score}";
        ResultTimeUI.text = $"{ElapsedTime:F0}";

        if (float.IsInfinity(StageManager.Instance.LeftTime))
        {
            StageManager.Instance.UpdatePhase(ElapsedTime);
        }
    }

    private void OnRSBEnded(RSBResult result)
    {
        if (result == RSBResult.Win) Score++;
    }
}
