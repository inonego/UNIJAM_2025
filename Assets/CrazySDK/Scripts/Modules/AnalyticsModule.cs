using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class AnalyticsModule : MonoBehaviour
    {
        private CrazySDK _crazySDK;

        public void Init(CrazySDK crazySDK)
        {
            _crazySDK = crazySDK;
        }

        public void TrackOrder(PaymentProvider provider, object order)
        {
            switch (provider)
            {
                case PaymentProvider.Xsolla:
                    _crazySDK.WrapSDKAction(
                        () =>
                        {
                            AnalyticsTrackOrderSDK("xsolla", JsonUtility.ToJson(order));
                        },
                        () =>
                        {
                            _crazySDK.DebugLog("Tracking Xsolla order " + JsonUtility.ToJson(order));
                        }
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
            }
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void AnalyticsTrackOrderSDK(string provider, string order);
#else
        private void AnalyticsTrackOrderSDK(string provider, string order) { }
#endif
    }

    public enum PaymentProvider
    {
        Xsolla,
    }
}
