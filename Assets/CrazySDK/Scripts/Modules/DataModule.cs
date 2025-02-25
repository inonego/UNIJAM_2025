using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class DataModule : MonoBehaviour
    {
        private CrazySDK _crazySDK;

        public void Init(CrazySDK crazySDK)
        {
            _crazySDK = crazySDK;
        }

        public void SetInt(string key, int value)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    DataSetItemSDK(key, value + "");
                },
                () =>
                {
                    PlayerPrefs.SetInt(key, value);
                }
            );
        }

        public int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        public int GetInt(string key, int defaultValue)
        {
            return _crazySDK.WrapSDKFunc(
                () => HasKey(key) ? int.Parse(DataGetItemSDK(key)) : defaultValue,
                () => PlayerPrefs.GetInt(key, defaultValue)
            );
        }

        public void SetFloat(string key, float value)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    DataSetItemSDK(key, value.ToString(CultureInfo.InvariantCulture));
                },
                () =>
                {
                    PlayerPrefs.SetFloat(key, value);
                }
            );
        }

        public float GetFloat(string key)
        {
            return GetFloat(key, 0.0f);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return _crazySDK.WrapSDKFunc(
                () => HasKey(key) ? float.Parse(DataGetItemSDK(key), CultureInfo.InvariantCulture) : defaultValue,
                () => PlayerPrefs.GetFloat(key, defaultValue)
            );
        }

        public void SetString(string key, string value)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    if (value == null)
                    {
                        DeleteKey(key);
                    }
                    else
                    {
                        DataSetItemSDK(key, value);
                    }
                },
                () =>
                {
                    PlayerPrefs.SetString(key, value);
                }
            );
        }

        public string GetString(string key)
        {
            return GetString(key, "");
        }

        public string GetString(string key, string defaultValue)
        {
            return _crazySDK.WrapSDKFunc(
                () => HasKey(key) ? DataGetItemSDK(key) : defaultValue,
                () => PlayerPrefs.GetString(key, defaultValue)
            );
        }

        public bool HasKey(string key)
        {
            return _crazySDK.WrapSDKFunc(() => DataHasKeySDK(key), () => PlayerPrefs.HasKey(key));
        }

        public void DeleteKey(string key)
        {
            _crazySDK.WrapSDKAction(
                () =>
                {
                    DataRemoveItemSDK(key);
                },
                () =>
                {
                    PlayerPrefs.DeleteKey(key);
                }
            );
        }

        public void DeleteAll()
        {
            _crazySDK.WrapSDKAction(DataClearSDK, PlayerPrefs.DeleteAll);
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void DataClearSDK();

        [DllImport("__Internal")]
        private static extern string DataGetItemSDK(string key);

        [DllImport("__Internal")]
        private static extern void DataRemoveItemSDK(string key);

        [DllImport("__Internal")]
        private static extern void DataSetItemSDK(string key, string value);

        [DllImport("__Internal")]
        private static extern bool DataHasKeySDK(string key);

#else
        private static void DataClearSDK() { }

        private static string DataGetItemSDK(string key)
        {
            return "";
        }

        private static void DataRemoveItemSDK(string key) { }

        private static void DataSetItemSDK(string key, string value) { }

        private static bool DataHasKeySDK(string key)
        {
            return false;
        }
#endif
    }
}
