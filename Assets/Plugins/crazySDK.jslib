mergeInto(LibraryManager.library, {
  InitSDK: function (version) {
    // to avoid warnings about Unity stringify beeing obsolete
    if (typeof UTF8ToString !== 'undefined') {
      window.unityStringify = UTF8ToString;
    } else {
      window.unityStringify = Pointer_stringify;
    }

    window.UnitySDK = {
      version: window.unityStringify(version),
      pointerLockElement: undefined,
      unlockPointer: function () {
        this.pointerLockElement = document.pointerLockElement || null;
        if (this.pointerLockElement && document.exitPointerLock) {
          document.exitPointerLock();
        }
      },
      lockPointer: function () {
        if (this.pointerLockElement && this.pointerLockElement.requestPointerLock) {
          this.pointerLockElement.requestPointerLock();
        }
      }
    };

    var initOptions = {
      wrapper: {
        engine: 'unity',
        sdkVersion: window.unityStringify(version)
      }
    };

    var script = document.createElement('script');
    script.src = 'https://sdk.crazygames.com/crazygames-sdk-v3.js';
    document.head.appendChild(script);
    script.addEventListener('load', function () {
      window.CrazyGames.SDK.init(initOptions).then(function () {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_Init');
        window.CrazyGames.SDK.ad.hasAdblock().then(function (result) {
          SendMessage('CrazySDKSingleton', 'JSLibCallback_AdblockDetectionResult', result ? 1 : 0);
        });
        window.CrazyGames.SDK.user.addAuthListener(function (user) {
          SendMessage('CrazySDKSingleton', 'JSLibCallback_AuthListener', JSON.stringify({ userJson: JSON.stringify(user) }));
        });
      });
    });
  },

  /** SDK.ad module */
  RequestAdSDK: function (adType) {
    var adTypeStr = window.unityStringify(adType);
    var callbacks = {
      adStarted: function () {
        window.UnitySDK.unlockPointer();
        SendMessage('CrazySDKSingleton', 'JSLibCallback_AdStarted');
      },
      adFinished: function () {
        window.UnitySDK.lockPointer();
        SendMessage('CrazySDKSingleton', 'JSLibCallback_AdFinished');
      },
      adError: function (error) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_AdError', JSON.stringify(error));
      }
    };
    window.CrazyGames.SDK.ad.requestAd(adTypeStr, callbacks);
  },

  /** SDK.banner module */
  RequestBannersSDK: function (bannersJSON) {
    var banners = JSON.parse(window.unityStringify(bannersJSON));
    window.CrazyGames.SDK.banner.requestOverlayBanners(banners, function (bannerId, message, error) {});
  },

  /** SDK.game module */
  HappyTimeSDK: function () {
    window.CrazyGames.SDK.game.happytime();
  },
  GameplayStartSDK: function () {
    window.CrazyGames.SDK.game.gameplayStart();
  },
  GameplayStopSDK: function () {
    window.CrazyGames.SDK.game.gameplayStop();
  },
  RequestInviteUrlSDK: function (paramsStr) {
    var params = JSON.parse(window.unityStringify(paramsStr));
    var url = window.CrazyGames.SDK.game.inviteLink(params);
    var bufferSize = lengthBytesUTF8(url) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(url, buffer, bufferSize);
    return buffer;
  },
  GetInviteLinkParamSDK: function (paramName) {
    var urlParams = new URLSearchParams(window.location.search);
    var paramValue = urlParams.get(window.unityStringify(paramName));
    if (paramValue === null) {
      paramValue = '';
    }
    var bufferSize = lengthBytesUTF8(paramValue) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(paramValue, buffer, bufferSize);
    return buffer;
  },
  ShowInviteButtonSDK: function (paramsStr) {
    var params = JSON.parse(window.unityStringify(paramsStr));
    var url = window.CrazyGames.SDK.game.showInviteButton(params);
    var bufferSize = lengthBytesUTF8(url) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(url, buffer, bufferSize);
    return buffer;
  },
  HideInviteButtonSDK: function () {
    window.CrazyGames.SDK.game.hideInviteButton();
  },
  GetGameSettingsJSONSDK: function () {
    var settingsJson = JSON.stringify(window.CrazyGames.SDK.game.settings);
    var bufferSize = lengthBytesUTF8(settingsJson) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(settingsJson, buffer, bufferSize);
    return buffer;
  },

  /** SDK.user module */
  IsUserAccountAvailableSDK: function () {
    return window.CrazyGames.SDK.user.isUserAccountAvailable;
  },
  ShowAuthPromptSDK: function () {
    window.CrazyGames.SDK.user
      .showAuthPrompt()
      .then(function (user) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_ShowAuthPrompt', JSON.stringify({ userJson: JSON.stringify(user) }));
      })
      .catch(function (error) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_ShowAuthPrompt', JSON.stringify({ errorJson: JSON.stringify(error) }));
      });
  },
  ShowAccountLinkPromptSDK: function () {
    window.CrazyGames.SDK.user
      .showAccountLinkPrompt()
      .then(function (response) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_ShowAccountLinkPrompt', JSON.stringify({ linkAccountResponseStr: JSON.stringify(response) }));
      })
      .catch(function (error) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_ShowAccountLinkPrompt', JSON.stringify({ errorJson: JSON.stringify(error) }));
      });
  },
  GetUserSDK: function () {
    window.CrazyGames.SDK.user
      .getUser()
      .then(function (user) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_GetUser', JSON.stringify({ userJson: JSON.stringify(user) }));
      })
      .catch(function (error) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_GetUser', JSON.stringify({ errorJson: JSON.stringify(error) }));
      });
  },
  GetUserTokenSDK: function () {
    window.CrazyGames.SDK.user
      .getUserToken()
      .then(function (token) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_GetUserToken', JSON.stringify({ token: token }));
      })
      .catch(function (error) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_GetUserToken', JSON.stringify({ errorJson: JSON.stringify(error) }));
      });
  },
  GetXsollaUserTokenSDK: function () {
    window.CrazyGames.SDK.user
      .getXsollaUserToken()
      .then(function (token) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_GetXsollaUserToken', JSON.stringify({ token: token }));
      })
      .catch(function (error) {
        SendMessage('CrazySDKSingleton', 'JSLibCallback_GetXsollaUserToken', JSON.stringify({ errorJson: JSON.stringify(error) }));
      });
  },
  AddUserScoreSDK: function (score) {
    window.CrazyGames.SDK.user.addScore(score);
  },

  /** SDK.data module */
  DataClearSDK: function () {
    window.CrazyGames.SDK.data.clear();
  },
  DataGetItemSDK: function (key) {
    var data = window.CrazyGames.SDK.data.getItem(window.unityStringify(key));
    // DataModule from Unity won't call this method if the key doesn't exist, it will return the default value
    var bufferSize = lengthBytesUTF8(data + '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(data, buffer, bufferSize);
    return buffer;
  },
  DataHasKeySDK: function (key) {
    var data = window.CrazyGames.SDK.data.getItem(window.unityStringify(key));
    return data !== null;
  },
  DataRemoveItemSDK: function (key) {
    window.CrazyGames.SDK.data.removeItem(window.unityStringify(key));
  },
  DataSetItemSDK: function (key, value) {
    window.CrazyGames.SDK.data.setItem(window.unityStringify(key), window.unityStringify(value));
  },

  /** SDK.analytics module */
  AnalyticsTrackOrderSDK: function (provider, order) {
    window.CrazyGames.SDK.analytics.trackOrder(window.unityStringify(provider), JSON.parse(window.unityStringify(order)));
  },

  /** other */
  CopyToClipboardSDK: function (text) {
    const elem = document.createElement('textarea');
    elem.value = window.unityStringify(text);
    document.body.appendChild(elem);
    elem.select();
    elem.setSelectionRange(0, 99999);
    document.execCommand('copy');
    document.body.removeChild(elem);
  },
  SyncUnityGameDataSDK: function () {
    window.CrazyGames.SDK.data.syncUnityGameData();
  },
  GetIsQaToolSDK: function () {
    return window.CrazyGames.SDK.isQaTool;
  },
  GetEnvironmentSDK: function () {
    var environment = window.CrazyGames.SDK.environment;
    var bufferSize = lengthBytesUTF8(environment) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(environment, buffer, bufferSize);
    return buffer;
  },
  GetSystemInfoSDK: function () {
    var systemInfoJson = JSON.stringify(window.CrazyGames.SDK.user.systemInfo);
    var bufferSize = lengthBytesUTF8(systemInfoJson) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(systemInfoJson, buffer, bufferSize);
    return buffer;
  }
});
