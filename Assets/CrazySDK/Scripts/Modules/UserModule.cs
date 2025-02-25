using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class UserModule : MonoBehaviour
    {
        private CrazySDK _crazySDK;

        public void Init(CrazySDK crazySDK)
        {
            _crazySDK = crazySDK;
        }

        private readonly List<Action<PortalUser>> _getUserCallbacks = new List<Action<PortalUser>>();
        private readonly List<Action<PortalUser>> _authListeners = new List<Action<PortalUser>>();
        private readonly List<Action<SdkError, string>> _getUserTokenCallbacks = new List<Action<SdkError, string>>();

        private readonly List<Action<SdkError, string>> _getXsollaUserTokenCallbacks = new List<Action<SdkError, string>>();

        private Action<SdkError, PortalUser> _authPromptCallback;
        private Action<SdkError, bool> _accountLinkPromptCallback;

        private readonly PortalUser _demoUser1 = new PortalUser
        {
            username = "User1",
            profilePictureUrl = "https://images.crazygames.com/userportal/avatars/1.png",
        };

        private readonly PortalUser _demoUser2 = new PortalUser
        {
            username = "User2",
            profilePictureUrl = "https://images.crazygames.com/userportal/avatars/2.png",
        };

        private const string User1Token =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJpZFVzZXIxIiwidXNlcm5hbWUiOiJVc2VyMSIsImdhbWVJZCI6InlvdXJHYW1lSWQiLCJwcm9maWxlUGljdHVyZVVybCI6Imh0dHBzOi8vaW1hZ2VzLmNyYXp5Z2FtZXMuY29tL3VzZXJwb3J0YWwvYXZhdGFycy8xLnBuZyIsImlhdCI6MTY2ODU5MzMxNCwiZXhwIjo0ODI0Mjg4NTE0fQ.u4N2DzCC6Vmo6Gys-XSl8rHQR0NUJAcWQWr29eLd54qMDPbCopPG0kye8TAidOU6dWAqNWO_kERbl75nTxNcJjbW4yqBS_bIPingIhuCCJsjvnQPkalfmVotmoZGQP21Q9MyZPfpjZNogioA3a0vm6APXAqzudTA9lTioztnT4YvgndISngOMQVNoDCJ_DgCbKy8GFQDcCN-AHFFb0WIVWiTYszv-9JZohUzULt-ueYL31pXVGHQ5C4rHESEg7LYzx1IaLKw1zcoYGxul0RxR35nm3yD_bGa6fQVzCfnKnhEBRifUZsIN1LfIHfNB23ZOh1llG7PEOdvtCSfIxP9ZK6t4gRkGn1VsqZFAMhq1LgJebO8hcm_Iqx0wF3WkdMysoQuWThTNKnwmphv9pguuALILYJluUP8UQll3qiF6gzoLPy1MfD_9l4TPZeP9Bv50B-Tm6Lk3OW248jyuFRKP_VgwZutTb5pJ7LggFcqWFXsBv5ZG3V2zsfVwpAPDhpmb4ykjoB2xLSuxjrvs1dzMhl02QAQjqTUgHj4fstgX-2jYowDyyPjj6JbT2ZC7vrrdmPvc8AcNvyCszamfRYjexElGaeJDDt6vtRuJw_JVwsCLaYHGif4UaKCoe6BECg3mRVUkH08Nm-qQPQw9slpYZmxckFEQQPCGkkPhgOBFkE";

        private const string User2Token =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJpZFVzZXIyIiwidXNlcm5hbWUiOiJVc2VyMiIsImdhbWVJZCI6InlvdXJHYW1lSWQiLCJwcm9maWxlUGljdHVyZVVybCI6Imh0dHBzOi8vaW1hZ2VzLmNyYXp5Z2FtZXMuY29tL3VzZXJwb3J0YWwvYXZhdGFycy8yLnBuZyIsImlhdCI6MTY2ODU5MzMxNCwiZXhwIjo0ODI0Mjg4NTE0fQ.kh60HYKR8txKvLoCB6dQ9hQG8Mu1UgtneTGs3Y15HvBWrZoLKp3x3pTf_Vhq8xzs7fQYJKr94zYAxxFRztHey7Tv7PBBmPESUFo8Le-_s2xDy982sFhpM6XDt84ohhvEuJEsOW8wIcCaCK6wzm6UWY6n1bpw1cO1KNASyZRSkDRhfyzDVJ5e167OLgGe3euodTHgClJPDv0ZYhle9KH86PepWamCm0429VrzyOu6QdbtFcRlRNZVnTtgNrCpyvss4AyDhnY5qV9yng7xHVt4zlocP_Z7btBL_kxrzYskhJi6KYuQAYmqLxqHSDnehlIvgO4cdEpJA_FOTeACTohhEu8zjXRrfdAFvEe0W6qqUo5HNFoElRoxYWf11WGSdrJCjpF4Qei9BPgprFaVH2Xi-ITAjKyElQxeKs5p6VmvaMAGwdqZgM4fm7OSex8QQGK0HFJ7wFoCTV5PLl-MBVvTSTfemJMWEwc8od124gwT_uGdDKrASovT2pijgBsAi3jxwgfEr1RPq8uCgZtksrTqaAM9TMv9Z6Zv35pdgTrWzTrOn-G-uc4EPZq7iQaEnglWEFj8Qsm_nMQMgtIM7MYG8KwPC4if3-Yc8KwaAL_taVvkqyMaV3W5j4MX9b1bbf_fw3jrGt74MACb7FtgzKudxoz2CXKZqTppadxS_IOnlMk";

        private const string ExpiredToken =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJpZFVzZXIxIiwidXNlcm5hbWUiOiJVc2VyMSIsImdhbWVJZCI6InlvdXJHYW1lSWQiLCJwcm9maWxlUGljdHVyZVVybCI6Imh0dHBzOi8vaW1hZ2VzLmNyYXp5Z2FtZXMuY29tL3VzZXJwb3J0YWwvYXZhdGFycy8xLnBuZyIsImlhdCI6MTY2ODU5MzQ5MSwiZXhwIjoxNjY4NTkzNDkyfQ.l_0cyeD-suEM7n9l-Vb2nP5vTJi-e3HwZQWLUESJMdVTX1zPDuQhwnSgHhcGVGFnhG5Wvtni-ElpM8HnVNvY7hRthbeP23n2ScAJBvAX10vrzPuLJRn_Nj_5GcRQpK4fH813Ij8ZWuOaS2hD4gKaEUessZs5n5hNBTQN9T5j4wkNvfhuw9vqtVOha2aPveqeVy1eA5XAWI7IirHi31-Dw60MSVgsp3r4tpYEHTlUPktzLsQvO9Sk9IE7iybg9ycoFoS6L1eAvxGWVF1BMHRerPwdOV9CN0rtrqrTM3pyb1fpmFfgQpoA8AgPuVrU58mwyeTpUQ4WSrPrltGjxxfiGQOATBDBrJk5V173BfUgBEgAEP0TifWAQt02iijJa9G6q-V8p0GWto1EYSdvEDmG0YhoRBVxnOQH3U1Fu0yxMWGMm9VmZVVhVN8PpLjitEhP4Gn33IafpS05d1-Q0NFMb9-FvQCdtXjTaGbaBVIeBN-aO0r4ERvoBE9R0AUrywd9Z2zK_qKRvp35NyryLjnedsYt5Xrc9TA2uDMR77TjByyqsdQ_qv4zhLfhuiMiweXyPfYzltAiNJmEUohxlP7OvH33B6xpT7Qz2ZyEeMHBrQRQGGlT6MowcMYx_2LFNSK8PwZJNlMs0Uw_uCIu-4TvqleVleIg7sLhWiijw1cxtmM";

        /// <summary>
        /// User account is available only on CrazyGames domains. It is not available on your whitelisted domains from CrazyGamesSettings,
        /// or on any partner websites the embed the game from CrazyGames domains.
        /// </summary>
        public bool IsUserAccountAvailable =>
            _crazySDK.WrapSDKFunc(
                IsUserAccountAvailableSDK,
                () =>
                {
                    var settings = _crazySDK.Settings;
                    _crazySDK.DebugLog(
                        "Is user account available simulation for Unity editor, returning: " + settings.accountAvailableResponse
                    );
                    switch (settings.accountAvailableResponse)
                    {
                        case CrazySettingsAccountAvailableResponse.Yes:
                            return true;
                        case CrazySettingsAccountAvailableResponse.No:
                            return false;
                        default:
                            Debug.LogError("Unhandled case " + settings.accountAvailableResponse + ", returning 'true'");
                            return true;
                    }
                }
            );

        /// <summary>
        /// Retrieves the user that is currently signed in.
        /// The callback receives null if the user is not signed in.<br/><br/>
        /// You can customize the user returned in the editor in the CrazyGamesSettings file.
        /// </summary>
        public void GetUser(Action<PortalUser> callback)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    // the game may call this method multiple times quickly,
                    // but jslib doesn't handle multiple callbacks from the sdk, so store in game a queue of callbacks
                    _getUserCallbacks.Add(callback);
                    if (_getUserCallbacks.Count == 1)
                    {
                        GetUserSDK();
                    }
                },
                () =>
                {
                    var settings = _crazySDK.Settings;
                    _crazySDK.DebugLog("Get user simulation for Unity editor, returning user: " + settings.getUserResponse);
                    switch (settings.getUserResponse)
                    {
                        case CrazySettingsTestUser.User1:
                            callback(_demoUser1);
                            return;
                        case CrazySettingsTestUser.User2:
                            callback(_demoUser2);
                            return;
                        case CrazySettingsTestUser.UserLoggedOut:
                            callback(null);
                            return;
                        default:
                            Debug.LogError("Unhandled user " + settings.getUserResponse + ", returning null user");
                            callback(null);
                            return;
                    }
                }
            );
        }

        /// <summary>
        /// Retrieve the user token for in-game user accounts.
        /// To use this feature, be sure the in-game account feature is enabled for your game.
        /// Callback param1 = error message (null if everything is fine), param2 = token.
        /// Possible error codes: "userNotAuthenticated", "unexpectedError".
        /// </summary>
        public void GetUserToken(Action<SdkError, string> callback)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    // the game may call this method multiple times quickly, but jslib doesn't handle multiple callbacks from the sdk, so store in game a queue of callbacks
                    _getUserTokenCallbacks.Add(callback);
                    if (_getUserTokenCallbacks.Count == 1)
                    {
                        GetUserTokenSDK();
                    }
                },
                () =>
                {
                    var settings = _crazySDK.Settings;
                    _crazySDK.DebugLog("Get user token simulation for Unity editor, returning: " + settings.getTokenResponse);
                    switch (settings.getTokenResponse)
                    {
                        case CrazySettingsGetTokenResponse.User1:
                            callback(null, User1Token);
                            return;
                        case CrazySettingsGetTokenResponse.User2:
                            callback(null, User2Token);
                            return;
                        case CrazySettingsGetTokenResponse.UserLoggedOut:
                            callback(new SdkError { code = "userNotAuthenticated" }, null);
                            return;
                        case CrazySettingsGetTokenResponse.ExpiredToken:
                            callback(null, ExpiredToken);
                            return;
                        default:
                            Debug.LogError("Unhandled case " + settings.getTokenResponse + ", returning 'userNotAuthenticated'");
                            callback(new SdkError { code = "userNotAuthenticated" }, null);
                            return;
                    }
                }
            );
        }

        /// <summary>
        /// Retrieve Xsolla user token, used to interact with Xsolla SDK.
        /// Callback param1 = error message (null if everything is fine), param2 = token.
        /// </summary>
        public void GetXsollaUserToken(Action<SdkError, string> callback)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    // the game may call this method multiple times quickly,
                    // but jslib doesn't handle multiple callbacks from the sdk, so store in game a queue of callbacks
                    _getXsollaUserTokenCallbacks.Add(callback);
                    if (_getXsollaUserTokenCallbacks.Count == 1)
                    {
                        GetXsollaUserTokenSDK();
                    }
                },
                () =>
                {
                    _crazySDK.DebugLog("Get Xsolla user token simulation for Unity editor");
                    callback(null, "Demo editor Xsolla token");
                }
            );
        }

        /// <summary>
        /// Show auth prompt so the user can sign in.
        /// Callback param1 = error message (null if everything is fine), param2 = user object.
        /// Possible error messages: "showAuthPromptInProgress", "userAlreadySignedIn", "userCancelled".<br/><br/>
        /// You can customize the response returned in the editor in the CrazyGamesSettings files.
        /// </summary>
        public void ShowAuthPrompt(Action<SdkError, PortalUser> callback)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    if (_authPromptCallback != null)
                    {
                        // this is the only error we need to handle here, since JSlib doesn't handle multiple callbacks from the SDK
                        callback(new SdkError { code = "showAuthPromptInProgress" }, null);
                    }

                    _authPromptCallback = callback;
                    ShowAuthPromptSDK();
                },
                () =>
                {
                    var settings = _crazySDK.Settings;
                    _crazySDK.DebugLog("Show auth prompt simulation for Unity editor, returning: " + settings.authPromptResponse);
                    switch (settings.authPromptResponse)
                    {
                        case CrazySettingsAuthPromptResponse.User1:
                            callback(null, _demoUser1);
                            return;
                        case CrazySettingsAuthPromptResponse.User2:
                            callback(null, _demoUser2);
                            return;
                        case CrazySettingsAuthPromptResponse.UserCancelled:
                            callback(new SdkError { code = "userCancelled" }, null);
                            return;
                        default:
                            Debug.LogError("Unhandled case " + settings.authPromptResponse + ", returning 'userCancelled'");
                            callback(new SdkError { code = "userCancelled" }, null);
                            return;
                    }
                }
            );
        }

        /// <summary>
        /// Shows a prompt asking the user if they want to link their CrazyGames account to current in-game account.
        /// Callback param1 = error message (null if everything is fine), param2 = link response.
        /// Possible error messages: "showAccountLinkPromptInProgress", "userNotAuthenticated"
        /// </summary>
        public void ShowAccountLinkPrompt(Action<SdkError, bool> callback)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    if (_accountLinkPromptCallback != null)
                    {
                        // this is the only error we need to handle here, since JSlib doesn't handle multiple callbacks from the SDK
                        callback(new SdkError { code = "showAccountLinkPromptInProgress" }, false);
                    }

                    _accountLinkPromptCallback = callback;
                    ShowAccountLinkPromptSDK();
                },
                () =>
                {
                    var settings = _crazySDK.Settings;
                    _crazySDK.DebugLog("Show auth prompt simulation for Unity editor, returning: " + settings.linkAccountResponse);
                    switch (settings.linkAccountResponse)
                    {
                        case CrazySettingsLinkAccountResponse.Yes:
                            callback(null, true);
                            return;
                        case CrazySettingsLinkAccountResponse.No:
                            callback(null, false);
                            return;
                        case CrazySettingsLinkAccountResponse.UserLoggedOut:
                            callback(new SdkError { code = "userNotAuthenticated" }, false);
                            return;
                        default:
                            Debug.LogError("Unhandled case " + settings.linkAccountResponse + ", returning 'userNotAuthenticated'");
                            callback(new SdkError { code = "userNotAuthenticated" }, false);
                            return;
                    }
                }
            );
        }

        /// <summary>
        /// Register a new listener that is called everytime the user signs in on the platform. When the user signs out, the page is refreshed, so for sign out there is no auth listener call.
        /// </summary>
        public void AddAuthListener(Action<PortalUser> listener)
        {
            _authListeners.Add(listener);
        }

        public void RemoveAuthListener(Action<PortalUser> listener)
        {
            _authListeners.Remove(listener);
        }

        /// <summary>
        /// Requests to save the user's PlayerPref file to Automatic progress save (APS).
        /// </summary>
        public void SyncUnityGameData()
        {
            _crazySDK.WrapSDKAction(
                SyncUnityGameDataSDK,
                () =>
                {
                    _crazySDK.DebugLog("Requested to sync UnityGameData");
                }
            );
        }

        public void AddScore(float score)
        {
            if (Application.isEditor)
            {
                _crazySDK.DebugLog("Submit score: " + score);
                return;
            }

            AddUserScoreSDK(score);
        }

        /** Retrieve user system info (country, OS, browser, device)  */
        public SystemInfo SystemInfo
        {
            get
            {
                return _crazySDK.WrapSDKFunc(
                    () =>
                    {
                        var systemInfoStr = GetSystemInfoSDK();
                        _crazySDK.DebugLog($"System info: {systemInfoStr}");
                        return JsonUtility.FromJson<SystemInfo>(systemInfoStr);
                    },
                    () =>
                    {
                        string deviceType;

                        switch (_crazySDK.Settings.deviceType)
                        {
                            case CrazySettingsDeviceType.tablet:
                                deviceType = "tablet";
                                break;
                            case CrazySettingsDeviceType.mobile:
                                deviceType = "mobile";
                                break;
                            case CrazySettingsDeviceType.desktop:
                            default:
                                deviceType = "desktop";
                                break;
                        }

                        return new SystemInfo
                        {
                            countryCode = "DemoCountry",
                            browser = new Software { name = "DemoBrowser", version = "DemoVersion" },
                            os = new Software { name = "DemoSoftware", version = "DemoVersion" },
                            device = new Device { type = deviceType },
                        };
                    }
                );
            }
        }

        private void JSLibCallback_GetUser(string responseStr)
        {
            var response = JsonUtility.FromJson<UserCallbackReply>(responseStr);
            var error = response.Error;
            if (error != null)
            {
                // an error normally doesn't occur when requesting the user
                Debug.LogError("Get user error: " + error);
            }

            var tempList = _getUserCallbacks.Select(c => c).ToList(); // use a temp list, the main list may get modified if a callback will call get user again
            _getUserCallbacks.Clear();
            tempList.ForEach(c => c(response.User));
        }

        private void JSLibCallback_GetUserToken(string responseStr)
        {
            var response = JsonUtility.FromJson<TokenCallbackReply>(responseStr);

            var tempList = _getUserTokenCallbacks.Select(c => c).ToList(); // use a temp list, the main list may get modified if a callback will call get user token again
            _getUserTokenCallbacks.Clear();
            tempList.ForEach(c => c(response.Error, response.token));
        }

        private void JSLibCallback_GetXsollaUserToken(string responseStr)
        {
            var response = JsonUtility.FromJson<TokenCallbackReply>(responseStr);

            var tempList = _getXsollaUserTokenCallbacks.Select(c => c).ToList(); // use a temp list, the main list may get modified if a callback will call get user token again
            _getXsollaUserTokenCallbacks.Clear();
            tempList.ForEach(c => c(response.Error, response.token));
        }

        private void JSLibCallback_ShowAccountLinkPrompt(string responseStr)
        {
            var response = JsonUtility.FromJson<LinkAccountResponseCallbackReply>(responseStr);
            var linkAccountResponse = response.Response;
            _accountLinkPromptCallback(response.Error, linkAccountResponse != null && linkAccountResponse.response == "yes");
            _accountLinkPromptCallback = null;
        }

        private void JSLibCallback_ShowAuthPrompt(string responseStr)
        {
            var response = JsonUtility.FromJson<UserCallbackReply>(responseStr);
            _authPromptCallback(response.Error, response.User);
            _authPromptCallback = null;
        }

        private void JSLibCallback_AuthListener(string responseStr)
        {
            var response = JsonUtility.FromJson<UserCallbackReply>(responseStr);

            var tempList = _authListeners.Select(c => c).ToList(); // use a temp list, the main list may get modified if a callback adds/removes an auth listener
            tempList.ForEach(c => c(response.User));
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string ShowAuthPromptSDK();

        [DllImport("__Internal")]
        private static extern string ShowAccountLinkPromptSDK();

        [DllImport("__Internal")]
        private static extern string GetUserSDK();

        [DllImport("__Internal")]
        private static extern string GetUserTokenSDK();

        [DllImport("__Internal")]
        private static extern string GetXsollaUserTokenSDK();

        [DllImport("__Internal")]
        private static extern void AddUserScoreSDK(float score);

        [DllImport("__Internal")]
        private static extern void SyncUnityGameDataSDK();

        [DllImport("__Internal")]
        private static extern bool IsUserAccountAvailableSDK();

        [DllImport("__Internal")]
        private static extern string GetSystemInfoSDK();
#else
        // Preventing build to fail when using another platform than WebGL
        private void ShowAuthPromptSDK() { }

        private void ShowAccountLinkPromptSDK() { }

        private void GetUserSDK() { }

        private void GetUserTokenSDK() { }

        private void GetXsollaUserTokenSDK() { }

        private void AddUserScoreSDK(float score) { }

        private void SyncUnityGameDataSDK() { }

        private bool IsUserAccountAvailableSDK()
        {
            return false;
        }

        private string GetSystemInfoSDK()
        {
            return "";
        }
#endif

        [Serializable]
        private class TokenCallbackReply
        {
            public string errorJson; // JsonUtility initializes null class with empty fields, so pass them as JSON
            public string token;

            public SdkError Error =>
                errorJson == null || errorJson == "null" || errorJson == "undefined" ? null : JsonUtility.FromJson<SdkError>(errorJson);
        }

        [Serializable]
        private class UserCallbackReply
        {
            public string errorJson; // JsonUtility initializes null class with empty fields, so pass them as JSON
            public string userJson;

            public PortalUser User =>
                userJson == null || userJson == "null" || userJson == "undefined" ? null : JsonUtility.FromJson<PortalUser>(userJson);

            public SdkError Error =>
                errorJson == null || errorJson == "null" || errorJson == "undefined" ? null : JsonUtility.FromJson<SdkError>(errorJson);
        }

        [Serializable]
        private class LinkAccountResponse
        {
            public string response;
        }

        [Serializable]
        private class LinkAccountResponseCallbackReply
        {
            public string errorJson; // JsonUtility initializes null class with empty fields, so pass them as JSON
            public string linkAccountResponseStr;

            public LinkAccountResponse Response =>
                linkAccountResponseStr == null || linkAccountResponseStr == "null" || linkAccountResponseStr == "undefined"
                    ? null
                    : JsonUtility.FromJson<LinkAccountResponse>(linkAccountResponseStr);

            public SdkError Error =>
                errorJson == null || errorJson == "null" || errorJson == "undefined" ? null : JsonUtility.FromJson<SdkError>(errorJson);
        }
    }

    [Serializable]
    public class PortalUser
    {
        public string username;
        public string profilePictureUrl;

        public override string ToString()
        {
            return base.ToString() + "Username = " + username + ", profile picture url = " + profilePictureUrl;
        }
    }
}
