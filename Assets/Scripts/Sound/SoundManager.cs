using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public SerializedDictionary<string, AudioClip> sfxs = new SerializedDictionary<string, AudioClip>();    // SFX 모음
    public SerializedDictionary<string, AudioClip> bgms = new SerializedDictionary<string, AudioClip>();    // BGM 모음

    [Header("# BGM")]
    [Range(0, 1)] public float bgmVolume;

    private AudioSource bgmPlayer;

    [Header("# SFX")]
    public int channels;
    [Range(0, 1)] public float sfxVolume;

    [Header("# Audio Mixer Group")]
    [SerializeField] AudioMixerGroup audioMixerGroup;
    [SerializeField] AudioMixer audioMixer;

    [Header("# Fade 시간")]
    [SerializeField] private float duration = 1f;

    private Queue<AudioSource> sfxQueue;
    private const string VolumeParameter = "Master"; // 노출된 매개변수 이름
    private float originVolume;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }


        Init();
    }

    private void Start()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "Title":
                PlayBGM(BGM.Menu);
                break;
            case "Stage1":
                PlayBGM(BGM.Stage1);
                break;
            case "Stage2":
                PlayBGM(BGM.Stage2);
                break;
            case "Stage3":
                PlayBGM(BGM.Stage3);
                break;
            default:
                break;
        }
    }

    #region Initalize
    void Init()
    {
        InitBGMPlayer();
        InitSFXPlayer();
    }

    void InitBGMPlayer()
    {
        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = this.transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();

        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.dopplerLevel = 0.0f;              // 입체효과 비활성화
        bgmPlayer.reverbZoneMix = 0.0f;             // 동굴과 같은 입체환경 반영 비활성화
        bgmPlayer.outputAudioMixerGroup = audioMixerGroup;
    }

    void InitSFXPlayer()
    {
        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = this.transform;
        AudioSource[] sfxPlayers = new AudioSource[channels];
        sfxQueue = new Queue<AudioSource>();

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].loop = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].dopplerLevel = 0.0f;      // 입체효과 비활성화
            sfxPlayers[i].reverbZoneMix = 0.0f;     // 동굴과 같은 입체환경 반영 비활성화
            sfxPlayers[i].outputAudioMixerGroup = audioMixerGroup;
            sfxQueue.Enqueue(sfxPlayers[i]);
        }

    }
    #endregion

    #region BGM
    public void PlayBGM(BGM bgm)
    {
        if (bgmPlayer == null)
        {
            Debug.Log("bgmPlayer가 초기화 되지 않았습니다");
            return;
        }

        AudioClip clip = bgms[bgm.ToString()];

        bgmPlayer.clip = clip;
        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        if (bgmPlayer == null)
        {
            Debug.Log("bgmPlayer가 초기화 되지 않았습니다");
            return;
        }

        bgmPlayer.Stop();
    }
    #endregion

    #region SFX
    public void PlaySFX(SFX _sfx)
    {
        if (sfxQueue.Count == 0)
        {
            Debug.LogWarning("사용 가능한 AudioSource가 없습니다.");
            return;
        }

        AudioSource player = sfxQueue.Dequeue();
        AudioClip clip = sfxs[_sfx.ToString()];

        player.clip = clip;
        player.Play();
        StartCoroutine(ReturnToQueueAfterPlay(player));
    }

    private IEnumerator ReturnToQueueAfterPlay(AudioSource player)
    {
        yield return new WaitForSeconds(player.clip.length);

        sfxQueue.Enqueue(player);
    }
    #endregion

    public void FadeInAudioGroup()
    {
        StartCoroutine(FadeInAudioGroupCoroutine());
    }

    public void FadeOutAudioGroup()
    {
        StartCoroutine(FadeOutAudioGroupCoroutine());
    }
    IEnumerator FadeInAudioGroupCoroutine()
    {
        float currentVolume = 0;
        //audioMixer.GetFloat(VolumeParameter, out currentVolume);

        float targetVolume = originVolume;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp를 사용해 부드럽게 볼륨 증가
            float newVolume = Mathf.Lerp(currentVolume, targetVolume, elapsedTime / (duration));
            audioMixer.SetFloat(VolumeParameter, newVolume);

            yield return null;
        }

        // 정확히 originVolume로 설정
        audioMixer.SetFloat(VolumeParameter, targetVolume);
    }

    IEnumerator FadeOutAudioGroupCoroutine()
    {
        audioMixer.GetFloat(VolumeParameter, out originVolume);

        float targetVolume = -80f; // 완전 무음 (-80dB)

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp를 사용해 부드럽게 볼륨 감소
            float newVolume = Mathf.Lerp(originVolume, targetVolume, elapsedTime / duration);
            audioMixer.SetFloat(VolumeParameter, newVolume);
            yield return null;
        }

        // 정확히 -80dB로 설정
        audioMixer.SetFloat(VolumeParameter, targetVolume);
    }

}


#region enum
public enum BGM
{
    Menu,
    Stage1,
    Stage2,
    Stage3
}

public enum SFX
{
    Stage1_RSB_Lose, Stage2_RSB_Lose, Stage3_RSB_Lose,
    Stage1_RSB_Win, Stage2_RSB_Win, Stage3_RSB_Win
}
#endregion