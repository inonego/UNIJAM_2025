using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("# 대사")]
    [TextArea(1, 5)][SerializeField] string[] script;
    [SerializeField] Image[] cutScenes;

    [Header("# 대사 출력 간격")]
    [SerializeField] float delay;

    [Header("# 넘어갈 씬 이름")]
    [SerializeField] string nextSceneName;

    [Header("# 타이핑 소리")]
    [SerializeField] AudioSource typingAudioSource;

    private bool isSkipped = false;

    private Image imagePanel;
    private TMP_Text dialogue;
    private int currentCounter;
    private WaitForSeconds typingTime = new WaitForSeconds(0.05f);

    private void Awake()
    {
        dialogue = GetComponentInChildren<TMP_Text>();
        imagePanel = GetComponentInChildren<Image>();
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

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Skip();
        }
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

        typingAudioSource.Play();

        // 2. 키보드 치는듯한 연출로 dialogue 초기화
        for (int i = 0; i < script[currentCounter].Length; i++)
        {
            dialogue.text += script[currentCounter][i];
            yield return typingTime;
        }

        // 3. 대사 카운터 증가
        ++currentCounter;

        typingAudioSource.Stop();

        // 4. 대사 읽을 시간을 위해 약간의 delay
        yield return new WaitForSeconds(delay);


        // 5. FadeOut 후 대사가 끝났다면 다음씬 로드. 대사가 남았다면 FadeIn후 대사 출력
        if (currentCounter < script.Length)
        {
            FadeManager.instance.FadeIn(image: cutScenes[currentCounter - 1], onComplete: () =>
            {
                dialogue.text = "";
                ShowDialogue();
            });
        }
        else
        {
            Skip();
        }
    }

    private void Skip()
    {
        if (isSkipped) return;
        
        isSkipped = true;

        FadeManager.instance.FadeOut(onComplete: () =>
        {
            SceneManager.LoadScene(nextSceneName);
        });
    }

}
