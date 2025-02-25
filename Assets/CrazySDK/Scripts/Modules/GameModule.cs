using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class GameSettings
    {
        public bool disableChat;
    }

    public class GameModule : MonoBehaviour
    {
        private CrazySDK _crazySDK;

        [Obsolete("IsInstantJoin is deprecated, please use IsInstantMultiplayer")]
        public bool IsInstantJoin => Application.isEditor ? false : GetInviteLinkParameter("instantJoin") == "true";
        public bool IsInstantMultiplayer => Application.isEditor ? false : GetInviteLinkParameter("instantJoin") == "true";

        public GameSettings Settings
        {
            get
            {
                if (Application.isEditor)
                {
                    return new GameSettings() { disableChat = false };
                }
                return JsonUtility.FromJson<GameSettings>(GetGameSettingsJSONSDK());
            }
        }

        public void Init(CrazySDK crazySDK)
        {
            _crazySDK = crazySDK;
        }

        public void HappyTime()
        {
            _crazySDK.DebugLog("Happy time!");
            _crazySDK.WrapSDKAction(HappyTimeSDK, () => { });
        }

        public void GameplayStart()
        {
            _crazySDK.DebugLog("Gameplay start called");
            _crazySDK.WrapSDKAction(GameplayStartSDK, () => { });
        }

        public void GameplayStop()
        {
            _crazySDK.DebugLog("Gameplay stop called");
            _crazySDK.WrapSDKAction(GameplayStopSDK, () => { });
        }

        public string GetInviteLinkParameter(string paramName)
        {
            if (Application.isEditor)
            {
                Debug.LogError("Cannot parse url in Unity editor, try running it in a browser");
                return "";
            }

            var paramValue = GetInviteLinkParamSDK(paramName);
            return string.IsNullOrEmpty(paramValue) ? null : paramValue;
        }

        public string InviteLink(Dictionary<string, string> parameters)
        {
            return _crazySDK.WrapSDKFunc(
                () =>
                {
                    var paramsJson = Utils.ConvertDictionaryToJson(parameters);
                    _crazySDK.DebugLog($"Requesting invite URL with params: {paramsJson}");
                    return RequestInviteUrlSDK(paramsJson);
                },
                () =>
                {
                    _crazySDK.DebugLog("Invite URL requested");
                    const string baseUrl = "https://crazygames.com/game/your-game";
                    return Utils.AppendQueryParameters(baseUrl, parameters);
                }
            );
        }

        public string ShowInviteButton(Dictionary<string, string> parameters)
        {
            return _crazySDK.WrapSDKFunc(
                () =>
                {
                    var paramsJson = Utils.ConvertDictionaryToJson(parameters);
                    _crazySDK.DebugLog($"Requesting invite button with params: {paramsJson}");
                    return ShowInviteButtonSDK(paramsJson);
                },
                () =>
                {
                    _crazySDK.DebugLog("Invite button called");
                    const string baseUrl = "https://crazygames.com/game/your-game";
                    return Utils.AppendQueryParameters(baseUrl, parameters);
                }
            );
        }

        public void HideInviteButton()
        {
            _crazySDK.DebugLog("Hide invite button called");
            _crazySDK.WrapSDKAction(HideInviteButtonSDK, () => { });
        }

        public void CopyToClipboard(string text)
        {
            _crazySDK.WrapSDKAction(
                () => CopyToClipboardSDK(text),
                () =>
                {
                    GUIUtility.systemCopyBuffer = text;
                }
            );
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void HappyTimeSDK();

        [DllImport("__Internal")]
        private static extern void GameplayStartSDK();

        [DllImport("__Internal")]
        private static extern void GameplayStopSDK();

        [DllImport("__Internal")]
        private static extern string RequestInviteUrlSDK(string urlParams);

        [DllImport("__Internal")]
        private static extern string GetInviteLinkParamSDK(string paramName);

        [DllImport("__Internal")]
        private static extern string ShowInviteButtonSDK(string urlParams);

        [DllImport("__Internal")]
        private static extern void HideInviteButtonSDK();

        [DllImport("__Internal")]
        private static extern void CopyToClipboardSDK(string text);

        [DllImport("__Internal")]
        private static extern string GetGameSettingsJSONSDK();
#else
        private void HappyTimeSDK() { }

        private void GameplayStartSDK() { }

        private void GameplayStopSDK() { }

        private string RequestInviteUrlSDK(string urlParams)
        {
            return "";
        }

        private string GetInviteLinkParamSDK(string paramName)
        {
            return "";
        }

        private string ShowInviteButtonSDK(string urlParams)
        {
            return "";
        }

        private void HideInviteButtonSDK() { }

        private void CopyToClipboardSDK(string text) { }

        private string GetGameSettingsJSONSDK()
        {
            return "";
        }
#endif
    }
}
