/*********************************************
 * BFramework
 * 游戏入口
 * 创建时间：2023/06/16 16:54:23
 *********************************************/
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
    public class RemoteServices : IRemoteService
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;
        private readonly string _targetCDNUrl;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;

#if UNITY_STANDALONE_WIN
            _targetCDNUrl = $"/Windows/{Application.version}/";
#elif UNITY_STANDALONE_OSX
            _targetCDNUrl = $"/Mac/{Application.version}/";
#elif UNITY_ANDROID
            _targetCDNUrl = $"/Android/{Application.version}/";
#elif UNITY_IOS
            _targetCDNUrl = $"/IOS/{Application.version}/";
#endif

        }

        public IReadOnlyList<string> GetRemoteUrls(string fileName)
        {
            return new[]
            {
                $"{_defaultHostServer}{_targetCDNUrl}{fileName}",
                $"{_fallbackHostServer}{_targetCDNUrl}{fileName}"
            };
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
            //原生加载热更
            var hotfixHandle = package.LoadAssetAsync<GameObject>("HotUpdatePrefab");
            yield return hotfixHandle;
            var hotfixObj = hotfixHandle.GetAssetObject<GameObject>();
            GameObject hotfixPrefab = Instantiate(hotfixObj, transform);
            hotfixPrefab.name = "[Hotfix]";
            //反射加载
            //Type entryType = ass.GetType("GameData.HotUpdateMain");
            //var hotfixPrefab = new GameObject();
            //hotfixPrefab.AddComponent(entryType);
            //hotfixPrefab.name = "[Hotfix]";
            Log(E_Log.Framework, "加载热更", "完毕");

            // HotUpdateMain.Start 初始化完成后会自毁，等实例销毁再释放启动资源。
            yield return new WaitUntil(() => hotfixPrefab == null);

            hotfixHandle.Release();

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

#if !UNITY_EDITOR
            playMode = EPlayMode.HostPlayMode;
#endif
            Log(E_Log.Framework, $"YooAsset Mode={playMode}");

            switch (playMode)
            {
                case EPlayMode.EditorSimulateMode:
                    {
                        var buildResult = EditorSimulateBuildInvoker.Build(GlobalDefine.PackageName, (int)EBundleType.VirtualAssetBundle);
                        var packageRoot = buildResult.PackageRootDirectory;
                        var fileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);

                        var createParameters = new EditorSimulateModeOptions();
                        createParameters.AutoUnloadBundleWhenUnused = true;
                        createParameters.EditorFileSystemParameters = fileSystemParams;

                        var initOperation = package.InitializePackageAsync(createParameters);
                        yield return initOperation;
                        CheckInitOperation(initOperation);
                        break;
                    }
                case EPlayMode.OfflinePlayMode:
                    {
                        var fileSystemParams = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();

                        var createParameters = new OfflinePlayModeOptions();
                        createParameters.AutoUnloadBundleWhenUnused = true;
                        createParameters.BuiltinFileSystemParameters = fileSystemParams;

                        var initOperation = package.InitializePackageAsync(createParameters);
                        yield return initOperation;
                        CheckInitOperation(initOperation);
                        break;
                    }
                case EPlayMode.HostPlayMode:
                    {
                        string defaultHostServer = GlobalDefine.ReleaseServerCDNUrl1;
                        string fallbackHostServer = GlobalDefine.ReleaseServerCDNUrl2;

                        IRemoteService remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                        var cacheFileSystemParams = FileSystemParameters.CreateDefaultSandboxFileSystemParameters(remoteServices);
                        var builtinFileSystemParams = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();

                        var createParameters = new HostPlayModeOptions();
                        createParameters.AutoUnloadBundleWhenUnused = true;
                        createParameters.BuiltinFileSystemParameters = builtinFileSystemParams;
                        createParameters.CacheFileSystemParameters = cacheFileSystemParams;

                        var initOperation = package.InitializePackageAsync(createParameters);
                        yield return initOperation;
                        CheckInitOperation(initOperation);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckInitOperation(InitializePackageOperation initOperation)
        {
            if (initOperation.Status == EOperationStatus.Succeeded)
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
            var operation = package.RequestPackageVersionAsync();
            yield return operation;

            if (operation.Status == EOperationStatus.Succeeded)
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
            var operation = package.LoadPackageManifestAsync(new LoadPackageManifestOptions(_packageVersion, 60));
            yield return operation;

            if (operation.Status == EOperationStatus.Succeeded)
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
            var downloader = package.CreateResourceDownloader(new ResourceDownloaderOptions(downloadingMaxNum, failedTryAgain));

            if (downloader.TotalDownloadCount == 0)
            {
                yield break;
            }

            //注册回调方法
            downloader.DownloadCompleted += OnDownloadCompletedFunction;                //当下载器结束（无论成功或失败）
            downloader.DownloadError += OnDownloadErrorFunction;                        //当下载器发生错误
            downloader.DownloadProgressChanged += OnDownloadProgressChangedFunction;    //当下载进度发生变化
            downloader.DownloadFileStarted += OnDownloadFileStartedFunction;            //当开始下载某个文件

            downloader.StartDownload();
            yield return downloader;

            if (downloader.Status == EOperationStatus.Succeeded)
            {
                Log(E_Log.Framework, "资源包下载成功！");
            }
            else
            {
                Log(E_Log.Error, "资源包下载失败！");
                WinLoading.ShowError();
            }
        }

        private void OnDownloadCompletedFunction(DownloadCompletedEventArgs args)
        {
            Log(E_Log.Framework, "资源包下载成功回调");
        }

        private void OnDownloadErrorFunction(DownloadErrorEventArgs args)
        {
            WinLoading.ShowError();
        }

        private void OnDownloadProgressChangedFunction(DownloadProgressChangedEventArgs args)
        {
            long curDownloadBytes = args.CurrentDownloadBytes;
            long totalDownloadBytes = args.TotalDownloadBytes;
            WinLoading.SetOneTips("正在为您下载游戏必要的资源包...");
            WinLoading.SetTwoTips($"{((float)curDownloadBytes / 1024 / 1024).ToString("F2")}MB/{((float)totalDownloadBytes / 1024 / 1024).ToString("F2")}MB");
            WinLoading.SetProgress(curDownloadBytes / totalDownloadBytes);
        }

        private void OnDownloadFileStartedFunction(DownloadFileStartedEventArgs args)
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