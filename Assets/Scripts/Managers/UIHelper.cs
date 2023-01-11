using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Managers
{
    public class UIHelper
    {
        public static async void SetRawImage(string addressableKey, RawImage image)
        {
            var handle = Addressables.LoadAssetAsync<Texture>(addressableKey);
            await handle;
            image.texture = handle.Result;
            image.SetNativeSize();
        }

        public static async UniTask SetImageSprite(string addressableKey, Image image, bool setNativeSize)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
            await handle;
            image.sprite = handle.Result;
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
            image.enabled = true;
        }
    }
}