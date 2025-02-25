using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class CrazySDK : MonoBehaviour
    {
        public const string Version = "5.14.0";
        public CrazySettings Settings { get; private set; }
        public static bool IsInitialized { get; private set; }
        public static bool IsShutDown { get; private set; }
        private static bool _initializationRequested;
        private static CrazySDK _sdkSingleton;
        private static UserModule _userModule;
        private static GameModule _gameModule;
        private static AdModule _adModule;
        private static BannerModule _bannerModule;
        private static DataModule _dataModule;
        private static AnalyticsModule _analyticsModule;
        private static bool _debug;
        private static bool _isOnWhitelistedDomain;
        private static readonly List<Action> InitCallbacks = new List<Action>();

        public static UserModule User
        {
            get
            {
                EnsureSingletonExists();
                return _userModule;
            }
        }

        public static GameModule Game
        {
            get
            {
                EnsureSingletonExists();
                return _gameModule;
            }
        }

        public static AdModule Ad
        {
            get
            {
                EnsureSingletonExists();
                return _adModule;
            }
        }

        public static BannerModule Banner
        {
            get
            {
                EnsureSingletonExists();
                return _bannerModule;
            }
        }

        public static DataModule Data
        {
            get
            {
                EnsureSingletonExists();
                return _dataModule;
            }
        }

        public static AnalyticsModule Analytics
        {
            get
            {
                EnsureSingletonExists();
                return _analyticsModule;
            }
        }

        public static bool IsQaTool
        {
            get
            {
                EnsureSingletonExists();
                return _sdkSingleton.WrapSDKFunc(GetIsQaToolSDK, () => false);
            }
        }

        public static string Environment
        {
            get
            {
                EnsureSingletonExists();
                return _sdkSingleton.WrapSDKFunc(GetEnvironmentSDK, () => "editor");
            }
        }

        [Obsolete("Please use IsAvailable instead")]
        public static bool IsOnCrazyGames => IsAvailable;

        /**
         * Returns 'true' if the game is running on CrazyGames domains, or on our partner websites embedding the game.
         * Returns 'true' in the Editor, since CrazySDK works in the Editor also.
         * Returns 'true' on localhost, since CrazySDK works on localhost also (be sure to use the provided WebGL template).
         * Returns 'false' when the game is running on any whitelisted domains in CrazyGamesSettings file.
         */
        public static bool IsAvailable
        {
            get
            {
                if (Application.isEditor)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(Application.absoluteURL) || Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    return false;
                }

                var host = new Uri(Application.absoluteURL).Host;

                var isLocalhost = host == "localhost" || host == "127.0.0.1" || host == "::1";

                if (isLocalhost)
                {
                    return true;
                }

                return host.EndsWith("crazygames.com") || host.EndsWith("dev-crazygames.be");
            }
        }

        public void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && !SiteLock.DidRun)
            {
                // sitelock may have been disabled from running by altering the built game files
                SiteLockFreezeGame(4);
            }
        }

        /**
         * Init can be called multiple times, it will not cause any issues. Feel free to call it multiple times
         * if you need the init callback in multiple places.
         */
        public static void Init(Action callback)
        {
            if (!Application.isEditor && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                throw new Exception("CrazySDK can be used only in Editor and WebGL builds");
            }

            if (IsInitialized)
            {
                callback();
                return;
            }

            EnsureSingletonExists();

            if (Application.isEditor)
            {
                if (!IsInitialized)
                {
                    IsInitialized = true;
                    _isOnWhitelistedDomain = false;
                    _debug = true;
                }

                callback();
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (_initializationRequested)
                {
                    InitCallbacks.Add(callback);
                }
                else
                {
                    InitCallbacks.Add(callback);
                    _initializationRequested = true;
                    _isOnWhitelistedDomain = SiteLock.IsOnWhitelistedDomain();
                    _debug = Application.absoluteURL.Contains("sdk_debug=true") || Debug.isDebugBuild;
                    InitSDK(Version, "CrazyGames.CrazySDK");
                }
            }
        }

        public void JSLibCallback_Init()
        {
            IsInitialized = true;
            InitCallbacks.ForEach(a => a());
            InitCallbacks.Clear();
        }

        private static void EnsureSingletonExists()
        {
            if (_sdkSingleton != null)
            {
                return;
            }

            var singletonObject = new GameObject();
            _sdkSingleton = singletonObject.AddComponent<CrazySDK>();
            _userModule = singletonObject.AddComponent<UserModule>();
            _userModule.Init(_sdkSingleton);
            _gameModule = singletonObject.AddComponent<GameModule>();
            _gameModule.Init(_sdkSingleton);
            _adModule = singletonObject.AddComponent<AdModule>();
            _adModule.Init(_sdkSingleton);
            _bannerModule = singletonObject.AddComponent<BannerModule>();
            _bannerModule.Init(_sdkSingleton);
            _dataModule = singletonObject.AddComponent<DataModule>();
            _dataModule.Init(_sdkSingleton);
            _analyticsModule = singletonObject.AddComponent<AnalyticsModule>();
            _analyticsModule.Init(_sdkSingleton);

            _sdkSingleton.Settings = Resources.Load<CrazySettings>("CrazyGamesSettings");
            if (_sdkSingleton.Settings == null)
            {
                Debug.LogError("Failed to load CrazySDK/Resources/CrazyGamesSettings");
            }

            DontDestroyOnLoad(singletonObject);
            singletonObject.name = "CrazySDKSingleton";
        }

        public void DebugLog(string msg)
        {
            if (_isOnWhitelistedDomain)
            {
                // some devs don't want logs from the SDK on their domains, so to be sure, completely disable all logging when running on whitelisted domains if there isn't a gameframe
                return;
            }

            if (Application.isEditor && Settings.disableSdkLogs)
            {
                return;
            }

            if (_debug)
                Debug.Log("[CrazySDK] " + msg);
        }

        public T WrapSDKFunc<T>(Func<T> liveFunc, Func<T> editorFunc)
        {
            if (!IsInitialized)
            {
                throw new Exception("CrazySDK not initialized. Please call CrazySDK.Instance.Init() before using the SDK.");
            }

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return liveFunc();
            }

            if (Application.isEditor)
            {
                return editorFunc();
            }

            throw new Exception($"CrazySDK unsupported platform {Application.platform}");
        }

        public void WrapSDKAction(Action liveAction, Action editorAction)
        {
            if (!IsInitialized)
            {
                throw new Exception("CrazySDK not initialized. Please call CrazySDK.Instance.Init() before using the SDK.");
            }

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                liveAction();
            }
            else if (Application.isEditor)
            {
                editorAction();
            }
            else
            {
                throw new Exception($"CrazySDK unsupported platform {Application.platform}");
            }
        }

        public static void SiteLockFreezeGame(int code)
        {
            EnsureSingletonExists();
            _sdkSingleton.StartCoroutine(_sdkSingleton.SiteLockFreezeGameCoroutine(code));
        }

        private IEnumerator SiteLockFreezeGameCoroutine(int code)
        {
            Debug.Log($"SiteLock v{SiteLock.sitelockVersion} activated, please check CrazySDK Unity docs for more info. Code {code}");

            // this wait is required so the crash log manages to reach the browser console
            yield return new WaitForSecondsRealtime(0.5f);

            Crash(0);
        }

        private static void Crash(int i)
        {
            Crash(i++);
        }

        private void OnDestroy()
        {
            IsShutDown = true;
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void InitSDK(string version, string objectName);

        [DllImport("__Internal")]
        private static extern bool GetIsQaToolSDK();

        [DllImport("__Internal")]
        private static extern string GetEnvironmentSDK();
#else
        // Preventing build to fail when using another platform than WebGL
        private static void InitSDK(string version, string objectName) { }

        private static bool GetIsQaToolSDK()
        {
            return false;
        }

        private static string GetEnvironmentSDK()
        {
            return "";
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeMethodLoad()
        {
            // When "Reload Domain" is disabled in Editor settings to improve startup speed,
            // static variables should be cleaned manually before every game start.
            IsInitialized = false;
            IsShutDown = false;
            InitCallbacks.Clear();
            _initializationRequested = false;
            _sdkSingleton = null;
            _userModule = null;
            _gameModule = null;
            _adModule = null;
            _bannerModule = null;
            _debug = false;
            _isOnWhitelistedDomain = false;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SiteLock.Check();
            }
        }
    }

    [Serializable]
    public class SystemInfo
    {
        public string countryCode;

        /** For browser and os, format is the same as https://github.com/faisalman/ua-parser-js **/
        public Software browser;

        /** For browser and os, format is the same as https://github.com/faisalman/ua-parser-js **/
        public Software os;

        public Device device;

        public override string ToString()
        {
            return $"Country code: {countryCode}, Browser: [{browser}], OS: [{os}], Device: [{device}]";
        }
    }

    [Serializable]
    public class Software
    {
        public string name;
        public string version;

        public override string ToString()
        {
            return $"Name: {name}, Version: {version}";
        }
    }

    [Serializable]
    public class Device
    {
        /** Possible values: "desktop", "tablet", "mobile" */
        public string type;

        public override string ToString()
        {
            return $"Type: {type}";
        }
    }

    [Serializable]
    public class SdkError
    {
        public string code;
        public string message;

        public override string ToString()
        {
            return $"SdkError: code = {code}, message = {message}";
        }
    }
}
