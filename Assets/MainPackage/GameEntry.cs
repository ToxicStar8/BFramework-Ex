/*********************************************
 * BFramework
 * 游戏入口
 * 创建时间：2023/06/16 16:54:23
 *********************************************/
using Obfuz;
using Obfuz.EncryptionVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using YooAsset;

namespace MainPackage
{
    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    public class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;
        private readonly string _targetCDNUrl;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;

#if UNITY_STANDALONE_WIN
            _targetCDNUrl = $"/Windows/{Application.version}";
#elif UNITY_STANDALONE_OSX
        _targetCDNUrl = $"/Mac/{Application.version}";
#elif UNITY_ANDROID
        _targetCDNUrl = $"/Android/{Application.version}";
#elif UNITY_IOS
        _targetCDNUrl = $"/IOS/{Application.version}";
#endif

        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            var url = $"{_defaultHostServer}/{_targetCDNUrl}/{fileName}";
            GameEntry.Instance.Log(E_Log.Framework, $"使用主地址:{url}");
            return url;
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            var url = $"{_fallbackHostServer}/{_targetCDNUrl}/{fileName}";
            GameEntry.Instance.Log(E_Log.Framework, $"使用备用地址:{url}");
            return url;
        }
    }

    /// <summary>
    /// 游戏入口
    /// </summary>
    public class GameEntry : MonoBehaviour
    {
        /// <summary>
        /// UI根节点
        /// </summary>
        [SerializeField]
        public GameObject UIRoot;
        [SerializeField]
        public RectTransform UIRootRect;

        /// <summary>
        /// UI根节点下的层级
        /// </summary>
        private static Dictionary<E_UILevel, RectTransform> _uiRootDic;

        [SerializeField]
        public Camera UICamera;

        [SerializeField]
        public Transform ObjPool;

        [SerializeField]
        public Transform GameStart;

        [Header("YooAsset运行模式")]
        [SerializeField]
        private EPlayMode _playMode;

        [Header("加载界面")]
        [SerializeField]
        public WinLoading WinLoading;

        /// <summary>
        /// YooAsset资源版本
        /// </summary>
        private static string _packageVersion;

#if UNITY_EDITOR
        [Header("时间倍率")]
        [SerializeField]
        public bool IsEnableTimeScale;
        [SerializeField]
        public float TimeScale;
#endif

        public static GameEntry Instance { private set; get; }

        /// <summary>
        /// 加载混淆
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnAfterAssembliesLoaded()
        {
            var textAsset = Resources.Load<TextAsset>("Obfuz/defaultStaticSecretKey");
            EncryptionService<DefaultStaticEncryptionScope>.Encryptor = new GeneratedEncryptionVirtualMachine(textAsset.bytes);
        }

        private void Awake()
        {
            Instance = this;
            _uiRootDic = new Dictionary<E_UILevel, RectTransform>();
            DontDestroyOnLoad(Instance);
            DontDestroyOnLoad(UIRoot);
            DontDestroyOnLoad(ObjPool);
        }

        private void Start()
        {
            WinLoading.SetIsComplete(false);
            StartCoroutine(CheckUpdate());
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (IsEnableTimeScale)
            {
                Time.timeScale = TimeScale;
            }
        }
#endif

        /// <summary>
        /// 开始下载AB包
        /// </summary>
        private IEnumerator CheckUpdate()
        {
            yield return InitYooAsset(_playMode);

            Log(E_Log.Framework, "热更代码", "启动中");
            //加载热更DLL
            var package = YooAssets.GetPackage(GlobalDefine.PackageName);
            var assetHandle = package.LoadAssetAsync<TextAsset>(GlobalDefine.HotfixDllName);
            yield return assetHandle;
            var assTextAsset = assetHandle.GetAssetObject<TextAsset>();
            Assembly ass = Assembly.Load(assTextAsset.bytes);
            Log(E_Log.Framework, "热更代码", "DLL加载完毕");

            Log(E_Log.Framework, "补元", "开始");
            var allAssetHandle = package.LoadAllAssetsAsync<TextAsset>(GlobalDefine.HotfixDllName);
            yield return allAssetHandle;
            foreach (var assetObj in allAssetHandle.AllAssetObjects)
            {
                Log(E_Log.Framework, "assetObj.name", assetObj.name);
                if (assetObj.name.EndsWith(".bytes") && assetObj.name != GlobalDefine.HotfixDllName)
                {
                    var textAsset = assetObj as TextAsset;
                    HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HybridCLR.HomologousImageMode.SuperSet);
                }
            }
            Log(E_Log.Framework, "补元", "完毕");

            Log(E_Log.Framework, "加载热更", "开始");
            //原生加载热更
            var hotfixObj = package.LoadAssetSync<GameObject>("HotUpdatePrefab").GetAssetObject<GameObject>();
            GameObject hotfixPrefab = Instantiate(hotfixObj, transform);
            hotfixPrefab.name = "[Hotfix]";
            //反射加载
            //Type entryType = ass.GetType("GameData.HotUpdateMain");
            //var hotfixPrefab = new GameObject();
            //hotfixPrefab.AddComponent(entryType);
            //hotfixPrefab.name = "[Hotfix]";
            Log(E_Log.Framework, "加载热更", "完毕");

            WinLoading.SetIsComplete(true);
        }

        #region YooAsset
        /// <summary>
        /// 初始化YooAsset资源系统
        /// </summary>
        private IEnumerator InitYooAsset(EPlayMode playMode)
        {
            yield return CreatePackageAsync(playMode);
            yield return RequestPackageVersion();
            yield return UpdatePackageManifest();
            yield return DownloadABPackage();
        }

        /// <summary>
        /// 创建YooAsset资源包
        /// </summary>
        private IEnumerator CreatePackageAsync(EPlayMode playMode)
        {
            YooAssets.Destroy();
            YooAssets.Initialize();

            var package = YooAssets.CreatePackage(GlobalDefine.PackageName);
            YooAssets.SetDefaultPackage(package);

#if !UNITY_EDITOR
            playMode = EPlayMode.HostPlayMode;
#endif
            Log(E_Log.Framework, $"YooAsset Mode={playMode}");

            switch (playMode)
            {
                case EPlayMode.EditorSimulateMode:
                    {
                        var buildResult = EditorSimulateModeHelper.SimulateBuild(GlobalDefine.PackageName);
                        var packageRoot = buildResult.PackageRootDirectory;
                        var fileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);

                        var createParameters = new EditorSimulateModeParameters();
                        createParameters.EditorFileSystemParameters = fileSystemParams;

                        var initOperation = package.InitializeAsync(createParameters);
                        yield return initOperation;
                        CheckInitOperation(initOperation);
                        break;
                    }
                case EPlayMode.OfflinePlayMode:
                    {
                        var fileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

                        var createParameters = new OfflinePlayModeParameters();
                        createParameters.BuildinFileSystemParameters = fileSystemParams;

                        var initOperation = package.InitializeAsync(createParameters);
                        yield return initOperation;
                        CheckInitOperation(initOperation);
                        break;
                    }
                case EPlayMode.HostPlayMode:
                    {
                        string defaultHostServer = GlobalDefine.ReleaseServerCDNUrl1;
                        string fallbackHostServer = GlobalDefine.ReleaseServerCDNUrl2;

                        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                        var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                        var createParameters = new HostPlayModeParameters();
                        createParameters.CacheFileSystemParameters = cacheFileSystemParams;

                        var initOperation = package.InitializeAsync(createParameters);
                        yield return initOperation;
                        CheckInitOperation(initOperation);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckInitOperation(InitializationOperation initOperation)
        {
            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Log(E_Log.Framework, "资源包初始化成功！");
            }
            else
            {
                Log(E_Log.Error, "资源包初始化失败");
                WinLoading.ShowError();
            }
        }

        /// <summary>
        /// 获取资源版本
        /// </summary>
        private IEnumerator RequestPackageVersion()
        {
            var package = YooAssets.GetPackage(GlobalDefine.PackageName);
            var operation = package.RequestPackageVersionAsync(false);
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                _packageVersion = operation.PackageVersion;
                Log(E_Log.Framework, $"资源版本:{_packageVersion}");
            }
            else
            {
                Log(E_Log.Error, operation.Error);
                WinLoading.ShowError();
            }
        }

        /// <summary>
        /// 更新资源清单
        /// </summary>
        private IEnumerator UpdatePackageManifest()
        {
            var package = YooAssets.GetPackage(GlobalDefine.PackageName);
            var operation = package.UpdatePackageManifestAsync(_packageVersion);
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                Log(E_Log.Framework, $"更新资源清单:{_packageVersion}");
            }
            else
            {
                Log(E_Log.Error, operation.Error);
                WinLoading.ShowError();
            }
        }

        /// <summary>
        /// 资源包下载
        /// </summary>
        private IEnumerator DownloadABPackage()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var package = YooAssets.GetPackage(GlobalDefine.PackageName);
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (downloader.TotalDownloadCount == 0)
            {
                yield break;
            }

            downloader.DownloadFinishCallback = OnDownloadFinishFunction;
            downloader.DownloadErrorCallback = OnDownloadErrorFunction;
            downloader.DownloadUpdateCallback = OnDownloadUpdateFunction;
            downloader.DownloadFileBeginCallback = OnDownloadFileBeginFunction;

            downloader.BeginDownload();
            yield return downloader;

            if (downloader.Status == EOperationStatus.Succeed)
            {
                Log(E_Log.Framework, "资源包下载成功！");
            }
            else
            {
                Log(E_Log.Error, "资源包下载失败！");
                WinLoading.ShowError();
            }
        }

        private void OnDownloadFinishFunction(DownloaderFinishData data)
        {
            Log(E_Log.Framework, "资源包下载成功回调");
        }

        private void OnDownloadErrorFunction(DownloadErrorData data)
        {
            WinLoading.ShowError();
        }

        private void OnDownloadUpdateFunction(DownloadUpdateData data)
        {
            long curDownloadBytes = data.CurrentDownloadBytes;
            long totalDownloadBytes = data.TotalDownloadBytes;
            WinLoading.SetOneTips("正在为您下载游戏必要的资源包...");
            WinLoading.SetTwoTips($"{((float)curDownloadBytes / 1024 / 1024).ToString("F2")}MB/{((float)totalDownloadBytes / 1024 / 1024).ToString("F2")}MB");
            WinLoading.SetProgress(curDownloadBytes / totalDownloadBytes);
        }

        private static void OnDownloadFileBeginFunction(DownloadFileData data)
        {
        }
        #endregion

        /// <summary>
        /// 获得UI根节点下的层级节点
        /// </summary>
        /// <param name="uiLevel"></param>
        /// <returns></returns>
        public RectTransform GetUILevelTrans(E_UILevel uiLevel)
        {
            if (!_uiRootDic.TryGetValue(uiLevel, out var rect))
            {
                rect = UIRootRect.Find(uiLevel.ToString()) as RectTransform;
                _uiRootDic[uiLevel] = rect;
            }
            return rect;
        }

        /// <summary>
        /// Log
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void Log(E_Log logType, string title = null, string content = null, string color = null)
        {
            string tempStr = string.Empty;
            if (title == null || content == null)
            {
                tempStr = "<color={0}>{1}</color>";
            }
            else
            {
                tempStr = "<color={0}>{1}</color>===><color={0}>{2}</color>";
            }

            switch (logType)
            {
                case E_Log.Log:
                    Debug.Log(string.Format(tempStr, "white", title, content));
                    break;
                case E_Log.Framework:
                    Debug.Log(string.Format(tempStr, "magenta", title, content));
                    break;
                case E_Log.Proto:
                    Debug.Log(string.Format(tempStr, "#00ffff", title, content));
                    break;
                case E_Log.Audio:
                    Debug.Log(string.Format(tempStr, "#9fff85", title, content));
                    break;
                case E_Log.Error:
                    Debug.LogError(string.Format(tempStr, "red", title, content));
                    break;
                case E_Log.Warning:
                    Debug.LogWarning(string.Format(tempStr, "yellow" , title, content));
                    break;
                case E_Log.Exception:
                    Debug.LogException(new Exception(string.Format(tempStr, "red", title, content)));
                    break;
                case E_Log.Custom:
                    Debug.Log(string.Format(tempStr, color, title, content));
                    break;
            }
        }
    }

    /// <summary>
    /// UI层级
    /// </summary>
    public enum E_UILevel
    {
        Background,
        Common,
        Pop,
        Loading,
        Tips,
    }

    /// <summary>
    /// Log类型
    /// </summary>
    public enum E_Log
    {
        Log,        //普通Log
        Framework,  //框架Log
        Proto,      //联网Log
        Audio,      //音频Log
        Error,      //错误Log
        Warning,    //警告Log
        Exception,  //异常Log
        Custom,     //自定义颜色Log
    }
}