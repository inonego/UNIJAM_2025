using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("# 대사")]
    [TextArea(1,5)][SerializeField] string[] script;
    [SerializeField] Sprite[] cutScenes;

    [Header("# 대사 출력 간격")]
    [SerializeField] float delay;

    [Header("# 넘어갈 씬 이름")]
    [SerializeField] string nextSceneName;

    private Image imagePanel;
    private TMP_Text dialogue;
    private int currentCounter;
    private WaitForSeconds typingTime = new WaitForSeconds(0.05f);

    private void Awake()
    {
        dialogue = GetComponentInChildren<TMP_Text>();
        imagePanel = GetComponentInChildren<Image>();
        imagePanel.sprite = cutScenes[0];
        dialogue.text = "";
        currentCounter = 0;
    }

    private void Start()
    {
        FadeManager.instance.FadeIn(onComplete: () =>
        {
            ShowDialogue();
        });
    }

    public void ShowDialogue()
    {
        if (currentCounter < script.Length)
            StartCoroutine(ActiveDialogueCoroutine());
    }

    private IEnumerator ActiveDialogueCoroutine()
    {
        // 1. 대사 초기화
        dialogue.text = "";

        // 2. currentCounter에 맞는 스프라이트 할당
        imagePanel.sprite = cutScenes[currentCounter];

        // 3. 키보드 치는듯한 연출로 dialogue 초기화
        for (int i = 0; i < script[currentCounter].Length; i++)
        {
            dialogue.text += script[currentCounter][i];
            yield return typingTime;
        }

        // 4. 대사 카운터 증가
        ++currentCounter;

        // 5. 대사 읽을 시간을 위해 약간의 delay
        yield return new WaitForSeconds(delay);


        // 6. FadeOut 후 대사가 끝났다면 다음씬 로드. 대사가 남았다면 FadeIn후 대사 출력
        FadeManager.instance.FadeOut(onComplete: () =>
        {
            if(currentCounter < script.Length)
            {
                dialogue.text = "";
                FadeManager.instance.FadeIn(onComplete: () =>
                {
                    ShowDialogue();
                });
            }
            else
                SceneManager.LoadScene(nextSceneName);
        });
    }


}
