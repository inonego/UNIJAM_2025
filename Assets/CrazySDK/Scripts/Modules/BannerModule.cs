using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class BannerModule : MonoBehaviour
    {
        private CrazySDK _crazySDK;
        private readonly List<CrazyBanner> _banners = new List<CrazyBanner>();

        public List<CrazyBanner> Banners => _banners;

        public void Init(CrazySDK crazySDK)
        {
            _crazySDK = crazySDK;
        }

        /**
         * Banners that are active (activeInHierarchy) will be refreshed.
         * Banners that were removed or hidden in the meantime will get hidden.
         */
        public void RefreshBanners()
        {
            var visibleBanners = _banners.Where(b => b.IsVisible()).ToList();
            _crazySDK.DebugLog($"Refreshing {visibleBanners.Count} banners");
            _crazySDK.WrapSDKAction(
                () =>
                {
                    visibleBanners.ForEach(banner => banner.RegenerateId());
                    var bannerToRefresh = visibleBanners
                        .Select(
                            (crazyBanner) =>
                            {
                                string size;
                                switch (crazyBanner.Size)
                                {
                                    case CrazyBanner.BannerSize.Leaderboard_728x90:
                                        size = "728x90";
                                        break;
                                    case CrazyBanner.BannerSize.Medium_300x250:
                                        size = "300x250";
                                        break;
                                    case CrazyBanner.BannerSize.Mobile_320x50:
                                        size = "320x50";
                                        break;
                                    case CrazyBanner.BannerSize.Large_Mobile_320x100:
                                        size = "320x100";
                                        break;
                                    case CrazyBanner.BannerSize.Main_Banner_468x60:
                                        size = "468x60";
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                var bannerTransform = (RectTransform)crazyBanner.transform.Find("Banner");
                                var anchor = new Vector2(
                                    (bannerTransform.anchorMin.x + bannerTransform.anchorMax.x) / 2,
                                    (bannerTransform.anchorMin.y + bannerTransform.anchorMax.y) / 2
                                );
                                return new Banner(crazyBanner.id, size, crazyBanner.Position, anchor, bannerTransform.pivot);
                            }
                        )
                        .ToArray();
                    var bannersJson = Utils.ToJson(bannerToRefresh);
                    _crazySDK.DebugLog("Requesting banners from Jslib: " + bannersJson);
                    RequestBannersSDK(bannersJson);
                },
                () =>
                {
                    visibleBanners.ForEach(b => b.SimulateRefresh());
                }
            );
        }

        public void RegisterBanner(CrazyBanner banner)
        {
            if (!_banners.Contains(banner))
                _banners.Add(banner);
        }

        public void UnregisterBanner(CrazyBanner banner)
        {
            _banners.Remove(banner);
        }

#if UNITY_WEBGL

        [DllImport("__Internal")]
        private static extern string RequestBannersSDK(string bannerRequestJson);
#else
        private string RequestBannersSDK(string bannerRequestJson)
        {
            return "";
        }
#endif
    }

    [Serializable]
    public class Banner
    {
        public string id;
        public string size;
        public Vector2 position;
        public Vector2 anchor;
        public Vector2 pivot;

        public Banner(string id, string size, Vector2 position, Vector2 anchor, Vector2 pivot)
        {
            this.id = id;
            this.size = size;
            this.position = position;
            this.anchor = anchor;
            this.pivot = pivot;
        }
    }
}
