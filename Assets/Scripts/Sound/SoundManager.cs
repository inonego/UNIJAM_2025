using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Collections;
using System;

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

    private Queue<AudioSource> sfxQueue;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        Init();
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

}

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
