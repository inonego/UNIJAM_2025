using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CrazyGames
{
    public class CrazyBanner : MonoBehaviour
    {
        public enum BannerSize
        {
            Leaderboard_728x90,
            Medium_300x250,
            Mobile_320x50,
            Main_Banner_468x60,
            Large_Mobile_320x100,
        }

        private Image backgroundImage;

        public string id;

        [SerializeField]
        private BannerSize size;

        public BannerSize Size
        {
            get => size;
            set
            {
                size = value;
                var banner = (RectTransform)transform.Find("Banner");
                switch (value)
                {
                    case BannerSize.Mobile_320x50:
                        banner.sizeDelta = new Vector2(320, 50);
                        break;
                    case BannerSize.Medium_300x250:
                        banner.sizeDelta = new Vector2(300, 250);
                        break;
                    case BannerSize.Leaderboard_728x90:
                        banner.sizeDelta = new Vector2(728, 90);
                        break;
                    case BannerSize.Main_Banner_468x60:
                        banner.sizeDelta = new Vector2(468, 60);
                        break;
                    case BannerSize.Large_Mobile_320x100:
                        banner.sizeDelta = new Vector2(320, 100);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public Vector2 Position
        {
            get
            {
                var banner = (RectTransform)transform.Find("Banner");
                return banner.anchoredPosition;
            }
            set
            {
                var banner = (RectTransform)transform.Find("Banner");
                banner.anchoredPosition = value;
            }
        }

        private void Awake()
        {
            backgroundImage = transform.GetComponentInChildren<Image>();
            if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor)
            {
                backgroundImage.color = new Color(0, 0, 0, 0);
            }

            RegenerateId();
            CrazySDK.Banner.RegisterBanner(this);
        }

        private void OnDestroy()
        {
            if (!CrazySDK.IsShutDown)
            {
                // in editor when leaving play mode avoid calling CrazySDK.Banner which will
                // create a new SDK instance and then generate a console error about improper shut down
                CrazySDK.Banner.UnregisterBanner(this);
            }
        }

        public void SimulateRefresh()
        {
            backgroundImage.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        public void RegenerateId()
        {
            id = Guid.NewGuid().ToString();
        }

        public bool IsVisible() => gameObject.activeInHierarchy;
    }
}
