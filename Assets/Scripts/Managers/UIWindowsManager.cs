using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = System.Object;

namespace Managers
{
    public class UIWindowsManager : MonoBehaviour
    {
        private Dictionary<string, GameObject> _existWindowList;
        public static UIWindowsManager Instance;
        private Camera _mainCamera;

        //Remember to set UICamera
        void Awake()
        {
            Instance = this;
            _existWindowList = new Dictionary<string, GameObject>();
        }
        

        // The last para of the callback function should always be the window instance
        private async UniTask<GameObject> GetUIWindowAsync(string windowID)
        {
            if (_existWindowList.ContainsKey(windowID))
            {
                await UniTask.WaitUntil(() => !_existWindowList.ContainsKey(windowID) || (_existWindowList[windowID] != null));
                _existWindowList[windowID].SetActive(false);
                return _existWindowList[windowID];
            }

            _existWindowList.Add(windowID, null);
            var handler = Addressables.LoadAssetAsync<GameObject>(windowID);
            await handler;
            if (handler.Status == AsyncOperationStatus.Failed)
            {
                _existWindowList.Remove(windowID);
                return null;
            }

            var result = handler.Result;
            GameObject windowInstance;
            
            windowInstance = Instantiate(result, this.transform);

            _existWindowList[windowID] = windowInstance;
            windowInstance.SetActive(false);
            return windowInstance;
        }

        public async UniTask<GameObject> ShowUIWindowAsync(string windowID)
        {
            await UniTask.WaitUntil(()=>Instance != null);
            Debug.Log("Showing " + windowID);
            GameObject uiWindow;

            if (!_existWindowList.ContainsKey(windowID))
            {
                uiWindow = await GetUIWindowAsync(windowID);
            }
            else
            {
                await UniTask.WaitUntil(() =>
                {
                    return !_existWindowList.ContainsKey(windowID) || (_existWindowList[windowID] != null);
                });
                uiWindow = _existWindowList[windowID];
            }

            uiWindow.SetActive(true);

            BringWindowToTop(windowID);
            return uiWindow;
        }

        public void BringWindowToTop(string windowID)
        {
            if (!_existWindowList.ContainsKey(windowID))
                return;
            _existWindowList[windowID].transform.SetAsLastSibling();
        }

        public void HideUIWindow(string windowID)
        {
            if (ReferenceEquals(_existWindowList[windowID], null))
            {
                Debug.LogError("No such UI");
                return;
            }

            GameObject uiWindow = _existWindowList[windowID];

            uiWindow.SetActive(false);
        }

        public void DisableUIWindow(string windowID)
        {
            if (Object.ReferenceEquals(_existWindowList[windowID], null))
            {
                Debug.LogError("No such UI");
                return;
            }

            GameObject uiWindow = _existWindowList[windowID];
            uiWindow.SetActive(false);
        }
        
    }
}