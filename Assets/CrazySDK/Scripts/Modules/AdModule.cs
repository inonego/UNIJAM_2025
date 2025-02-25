using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class AdModule : MonoBehaviour
    {
        public AdblockStatus AdblockStatus { get; private set; } = AdblockStatus.Detecting;
        private Action _onAdStarted;
        private Action<SdkError> _onAdError;
        private Action _onAdFinished;
        private bool _origRunInBackground;
        private float _origTimeScale;
        private float _origAudioListenerVolume;
        private bool _adRequestInProgress;
        private CrazySDK _crazySDK;
        private bool? _hasAdblock;
        private readonly List<Action<bool>> _adblockCallbacks = new List<Action<bool>>();

        public void Init(CrazySDK crazySDK)
        {
            _crazySDK = crazySDK;

            if (Application.isEditor)
            {
                SetAdblockDetectionStatus(false);
            }
        }

        public void HasAdblock(Action<bool> callback)
        {
            if (_hasAdblock.HasValue)
            {
                callback(_hasAdblock.Value);
            }
            else
            {
                _adblockCallbacks.Add(callback);
            }
        }

        public void JSLibCallback_AdblockDetectionResult(int result)
        {
            var detected = result == 1;
            _crazySDK.DebugLog($"Adblock present: {result} = {detected}, calling {_adblockCallbacks.Count} callbacks");
            SetAdblockDetectionStatus(detected);
        }

        private void SetAdblockDetectionStatus(bool detected)
        {
            _hasAdblock = detected;
            AdblockStatus = detected ? AdblockStatus.Present : AdblockStatus.Missing;
            _adblockCallbacks.ForEach(a => a(detected));
            _adblockCallbacks.Clear();
        }

        public void RequestAd(CrazyAdType adType, Action adStarted, Action<SdkError> adError, Action adFinished)
        {
            if (!Application.isEditor && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return;
            }

            if (!CrazySDK.IsInitialized)
            {
                // don't rely on _crazySDK.WrapSDKAction to catch this when calling ads,
                // cause the game will get frozen because the timeScale is set before calling WrapSDKAction
                throw new Exception("CrazySDK not initialized. Please call CrazySDK.Instance.Init() before using the SDK.");
            }

            if (_adRequestInProgress)
            {
                _crazySDK.DebugLog("Ad request in progress, ignore " + adType + " request.");
                return;
            }

            _crazySDK.DebugLog("Requesting CrazyAd Type: " + adType);

            _onAdStarted = adStarted;
            _onAdError = adError;
            _onAdFinished = adFinished;
            _adRequestInProgress = true;

            if (_crazySDK.Settings.pauseGameDuringAd)
            {
                _origTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }

            _origAudioListenerVolume = AudioListener.volume;
            _origRunInBackground = Application.runInBackground;

            AudioListener.volume = 0;
            Application.runInBackground = true;

            _crazySDK.WrapSDKAction(
                () =>
                {
                    RequestAdSDK(adType.ToString().ToLower());
                },
                () =>
                {
                    SimulateAdBreak(adType);
                }
            );
        }

        private IEnumerator InvokeRealtimeCoroutine(Action action, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action();
        }

        private void SimulateAdBreak(CrazyAdType adType)
        {
            var adTypeStr = adType.ToString();

            if (_crazySDK.Settings.disableAdPreviews)
            {
                Debug.Log(
                    "[CrazySDK] "
                        + char.ToUpper(adTypeStr[0])
                        + adTypeStr.Substring(1)
                        + " ad displayed successfully (simulation is disabled in CrazyGamesSettings)."
                );
                JSLibCallback_AdFinished();
                return;
            }

            Debug.Log("CrazyAds: Editor ad simulation, game will resume in 3 seconds");
            var adPreview = new GameObject("CrazyAdPreview").AddComponent<CrazyAdPreview>();
            adPreview.labelText = char.ToUpper(adTypeStr[0]) + adTypeStr.Substring(1) + " ad simulation, the game will resume in 3 seconds";

            JSLibCallback_AdStarted();
            StartCoroutine(InvokeRealtimeCoroutine(EndSimulation, 3));
        }

        private void EndSimulation()
        {
            DestroyImmediate(GameObject.Find("CrazyAdPreview"));
            JSLibCallback_AdFinished();
        }

        private void CleanupAd()
        {
            _adRequestInProgress = false;
            if (_crazySDK.Settings.pauseGameDuringAd)
            {
                Time.timeScale = _origTimeScale;
            }

            AudioListener.volume = _origAudioListenerVolume;
            Application.runInBackground = _origRunInBackground;
        }

        private void JSLibCallback_AdError(string errorJson)
        {
            var error = JsonUtility.FromJson<SdkError>(errorJson);
            _crazySDK.DebugLog("Ad Error: " + error);
            CleanupAd();
            _onAdError?.Invoke(error);
        }

        private void JSLibCallback_AdFinished()
        {
            _crazySDK.DebugLog("Ad Finished");
            CleanupAd();
            _onAdFinished?.Invoke();
        }

        private void JSLibCallback_AdStarted()
        {
            _crazySDK.DebugLog("Ad Started");
            _onAdStarted?.Invoke();
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void RequestAdSDK(string adType);
#else
        private void RequestAdSDK(string adType) { }
#endif
    }

    public enum CrazyAdType
    {
        Midgame,
        Rewarded,
    }

    public enum AdblockStatus
    {
        Detecting,
        Present,
        Missing,
    }
}
