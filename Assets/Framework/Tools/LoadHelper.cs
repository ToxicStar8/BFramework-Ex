/*********************************************
 * BFramework
 * 轻量化加载器
 * 更新时间：2026/04/30 15:19:41
 *********************************************/
using MainPackage;
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Object = UnityEngine.Object;
using YooAsset;

namespace Framework
{
    /// <summary>
    /// 通用加载助手（加载器）
    /// </summary>
    public class LoadHelper
    {
        public LoadHelper()
        {
            _assetHandleDic = new();
            _spriteDic = new();
            _gameObjectDic = new();
        }

        /// <summary>
        /// 当前资源包
        /// </summary>
        public ResourcePackage Package => YooAssets.GetPackage(GlobalDefine.PackageName);

        /// <summary>
        /// 创建加载器
        /// </summary>
        public static LoadHelper Create()
        {
            var pool = GameGod.Instance.PoolManager.CreateClassObjectPool<LoadHelper>();
            var loadHelper = pool.CreateClassObj();
            loadHelper._isRecycling = false;
            return loadHelper;
        }
        
        /// <summary>
        /// 发起回收，不等待完成；需要等待时使用 RecycleAsync。
        /// </summary>
        public static void Recycle(LoadHelper loadHelper)
        {
            RecycleAsync(loadHelper).Forget();
        }

        /// <summary>
        /// 等待实例销毁、资源句柄释放及加载器归池完成。
        /// </summary>
        public static async UniTask RecycleAsync(LoadHelper loadHelper)
        {
            if (loadHelper._isRecycling)
            {
                GameGod.Instance.Log(E_Log.Error, "加载器正在回收，不能重复回收。");
                return;
            }
            loadHelper._isRecycling = true;
            var poolManager = GameGod.Instance.PoolManager;
            var pool = poolManager.CreateClassObjectPool<LoadHelper>();
            await loadHelper.UnloadAll();

            // 退出时池管理器可能已销毁，资源仍需释放，但不再归池。
            if (poolManager.ClassObjectPoolDic != null && pool.ClassQueue != null)
            {
                pool.Recycle(loadHelper);
            }
        }

        /// <summary>
        /// 按资源名和加载类型分组的资源句柄字典
        /// </summary>
        private Dictionary<string, Dictionary<Type, AssetHandle>> _assetHandleDic;

        /// <summary>
        /// 按图集资源名分组的 Sprite 缓存，便于单独卸载图集。
        /// </summary>
        private Dictionary<string, Dictionary<string, Sprite>> _spriteDic;

        /// <summary>
        /// 按资源名分组的实例对象
        /// </summary>
        private Dictionary<string, List<GameObject>> _gameObjectDic;

        private CancellationTokenSource _loadCancellation = new();
        private bool _isRecycling;

        #region 加载图片
        /// <summary>
        /// 同步加载Sprite
        /// </summary>
        public Sprite LoadSpriteSync(string atlasName, string spriteName)
        {
            //因为美术资源太多重复的名字，这里直接使用 [文件夹_文件] 的格式来做可寻址
            var spaName = $"{atlasName}_{atlasName}";
            var spName = $"{atlasName}_{spriteName}";
            if (TryGetSprite(spaName, spriteName, out var cachedSprite))
            {
                return cachedSprite;
            }

            var spa = LoadSync<SpriteAtlas>(spaName);
            if (spa == null)
            {
                GameGod.Instance.Log(E_Log.Error, $"SpriteAtlas is Null!{spaName}");
                return null;
            }

            var sprite = spa.GetSprite(spriteName);
            if (sprite == null)
            {
                GameGod.Instance.Log(E_Log.Error, $"Sprite is Null!{spName}");
                return null;
            }

            TrackSprite(spaName, spriteName, sprite);
            return sprite;
        }

        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        public async UniTask<Sprite> GetSpriteAsync(string atlasName, string spriteName)
        {
            var cancellationToken = _loadCancellation.Token;
            cancellationToken.ThrowIfCancellationRequested();
            //因为美术资源太多重复的名字，这里直接使用 [文件夹_文件] 的格式来做可寻址
            var spaName = $"{atlasName}_{atlasName}";
            var spName = $"{atlasName}_{spriteName}";
            if (TryGetSprite(spaName, spriteName, out var cachedSprite))
            {
                return cachedSprite;
            }

            var spa = await LoadAsync<SpriteAtlas>(spaName);
            cancellationToken.ThrowIfCancellationRequested();
            // 等待图集时，另一个请求可能已经取得并缓存了同一个 Sprite。
            if (TryGetSprite(spaName, spriteName, out cachedSprite))
            {
                return cachedSprite;
            }
            if (spa == null)
            {
                GameGod.Instance.Log(E_Log.Error, $"SpriteAtlas is Null!{spaName}");
                return null;
            }

            var sprite = spa.GetSprite(spriteName);
            if (sprite == null)
            {
                GameGod.Instance.Log(E_Log.Error, $"Sprite is Null!{spName}");
                return null;
            }

            TrackSprite(spaName, spriteName, sprite);
            return sprite;
        }
        #endregion

        #region 加载资源
        /// <summary>
        /// 加载资源 带后缀；失败时记录错误日志并返回 null。
        /// </summary>
        public T LoadSync<T>(string objName) where T : Object
        {
            var assetType = typeof(T);
            if (!TryGetAssetHandle(objName, assetType, out var handle))
            {
                handle = Package.LoadAssetSync<T>(objName);
                TrackAssetHandle(objName, assetType, handle);
            }
            // 缓存中也可能是尚未完成的异步句柄。
            if (!handle.IsDone)
            {
                handle.WaitForAsyncComplete();
            }
            if (!ValidateAssetHandle(objName, assetType, handle))
            {
                return null;
            }
            return handle.GetAssetObject<T>();
        }

        /// <summary>
        /// 加载资源 带后缀
        /// </summary>
        public Object LoadSync(string objName)
        {
            var obj = LoadSync<Object>(objName);
            return obj;
        }

        /// <summary>
        /// 异步加载资源 带后缀；失败时记录错误日志并返回 null。
        /// </summary>
        public async UniTask<T> LoadAsync<T>(string objName) where T : Object
        {
            var cancellationToken = _loadCancellation.Token;
            cancellationToken.ThrowIfCancellationRequested();
            var assetType = typeof(T);
            if (!TryGetAssetHandle(objName, assetType, out var handle))
            {
                handle = Package.LoadAssetAsync<T>(objName);
                // 未完成的句柄也由加载器持有，取消请求后仍能统一释放。
                TrackAssetHandle(objName, assetType, handle);
            }
            if (!handle.IsDone)
            {
                await UniTask.WaitUntil(() => !handle.IsValid || handle.IsDone,
                    cancellationToken: cancellationToken, cancelImmediately: true);
            }
            cancellationToken.ThrowIfCancellationRequested();
            if (!ValidateAssetHandle(objName, assetType, handle))
            {
                return null;
            }
            return handle.GetAssetObject<T>();
        }

        /// <summary>
        /// 异步加载资源 带后缀
        /// </summary>
        public async UniTask<Object> LoadAsync(string objName)
        {
            var obj = await LoadAsync<Object>(objName);
            return obj;
        }

        /// <summary>
        /// 同步创建实例对象
        /// </summary>
        public GameObject CreateGameObjectSync(string objName, Transform trans = null)
        {
            var obj = LoadSync<GameObject>(objName);
            if (obj == null)
            {
                GameGod.Instance.Log(E_Log.Error, "创建实例资源为空", objName);
                return null;
            }

            var go = Object.Instantiate(obj, trans);
            TrackGameObject(objName, go);
            return go;
        }

        /// <summary>
        /// 异步创建实例对象
        /// </summary>
        public async UniTask<GameObject> CreateGameObjectAsync(string objName, Transform trans = null)
        {
            var cancellationToken = _loadCancellation.Token;
            var obj = await LoadAsync<GameObject>(objName);
            cancellationToken.ThrowIfCancellationRequested();
            if (obj == null)
            {
                GameGod.Instance.Log(E_Log.Error, "创建实例资源为空", objName);
                return null;
            }

            var go = Object.Instantiate(obj, trans);
            TrackGameObject(objName, go);
            return go;
        }

        private bool ValidateAssetHandle(string objName, Type assetType, AssetHandle handle)
        {
            // 并发等待同一失败句柄时，首个请求会释放它，其他请求同样报告失败。
            if (!handle.IsValid)
            {
                GameGod.Instance.Log(E_Log.Error, "资源加载失败，句柄已释放", objName);
                return false;
            }
            // 检查所有权后才能清理失败：移出缓存的句柄已由单独卸载流程接管。
            if (!TryGetAssetHandle(objName, assetType, out var currentHandle) || currentHandle != handle)
            {
                GameGod.Instance.Log(E_Log.Error, "资源已单独卸载", objName);
                return false;
            }
            if (handle.Status == EOperationStatus.Succeeded)
            {
                return true;
            }

            var error = handle.Error;
            var handles = _assetHandleDic[objName];
            handles.Remove(assetType);
            if (handles.Count == 0)
            {
                _assetHandleDic.Remove(objName);
            }
            handle.Release();
            GameGod.Instance.Log(E_Log.Error, $"资源加载失败：{objName}", error);
            return false;
        }

        private bool TryGetAssetHandle(string objName, Type assetType, out AssetHandle handle)
        {
            handle = null;
            return _assetHandleDic.TryGetValue(objName, out var handles) && handles.TryGetValue(assetType, out handle);
        }

        private void TrackAssetHandle(string objName, Type assetType, AssetHandle handle)
        {
            if (handle == null)
            {
                return;
            }

            if (!_assetHandleDic.TryGetValue(objName, out var handles))
            {
                handles = new Dictionary<Type, AssetHandle>();
                _assetHandleDic.Add(objName, handles);
            }
            handles[assetType] = handle;
        }

        private bool TryGetSprite(string atlasName, string spriteName, out Sprite sprite)
        {
            sprite = null;
            return _spriteDic.TryGetValue(atlasName, out var sprites) && sprites.TryGetValue(spriteName, out sprite);
        }

        private void TrackSprite(string atlasName, string spriteName, Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            if (!_spriteDic.TryGetValue(atlasName, out var sprites))
            {
                sprites = new Dictionary<string, Sprite>();
                _spriteDic.Add(atlasName, sprites);
            }
            sprites[spriteName] = sprite;
        }

        private void TrackGameObject(string objName, GameObject go)
        {
            if (go == null)
            {
                return;
            }

            if (!_gameObjectDic.TryGetValue(objName, out var gameObjects))
            {
                gameObjects = new List<GameObject>();
                _gameObjectDic.Add(objName, gameObjects);
            }
            gameObjects.Add(go);
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">加载场景一定要用全大小写名，全小写的名字只能用于预先放在BuildSetting里使用</param>
        public void LoadSceneSync(string sceneName)
        {
            var handle = Package.LoadSceneSync(sceneName + ".unity", LoadSceneMode.Single);
            ValidateSceneHandle(sceneName, handle);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">加载场景一定要用全大小写名，全小写的名字只能用于预先放在BuildSetting里使用</param>
        public async UniTask LoadSceneAsync(string sceneName)
        {
            var cancellationToken = _loadCancellation.Token;
            cancellationToken.ThrowIfCancellationRequested();
            var handle = Package.LoadSceneAsync(sceneName + ".unity", LoadSceneMode.Single);
            // 取消等待不终止 Unity 的场景操作；后续失败时仍需释放句柄。
            handle.Completed += completedHandle =>
            {
                if (cancellationToken.IsCancellationRequested && completedHandle.Status != EOperationStatus.Succeeded)
                {
                    completedHandle.Release();
                }
            };
            if (!handle.IsDone)
            {
                await UniTask.WaitUntil(() => !handle.IsValid || handle.IsDone,
                    cancellationToken: cancellationToken, cancelImmediately: true);
            }
            cancellationToken.ThrowIfCancellationRequested();
            ValidateSceneHandle(sceneName, handle);
        }

        private static void ValidateSceneHandle(string sceneName, SceneHandle handle)
        {
            if (!handle.IsValid)
            {
                GameGod.Instance.Log(E_Log.Error, "场景加载失败，句柄已释放", sceneName);
                return;
            }
            if (handle.Status == EOperationStatus.Succeeded)
            {
                // YooAsset 持有成功的场景句柄，并在场景卸载时自动释放。
                return;
            }

            var error = handle.Error;
            handle.Release();
            GameGod.Instance.Log(E_Log.Error, $"场景加载失败：{sceneName}", error);
        }
        #endregion

        #region 卸载资源
        /// <summary>
        /// 发起单资源卸载，不等待完成；资源名必须与加载时一致，图集使用其资源名。
        /// 调用前应停止使用该资源；此接口不销毁 Prefab 实例。
        /// </summary>
        public void Unload(string objName)
        {
            Unload(objName, false);
        }

        /// <summary>
        /// 发起单资源卸载；destroyInstances 为 true 时，同时销毁并移除本加载器为该资源创建的全部实例。
        /// </summary>
        public void Unload(string objName, bool destroyInstances)
        {
            UnloadAsync(objName, destroyInstances).Forget();
        }

        /// <summary>
        /// 卸载指定资源及其 Sprite 缓存，等待下一帧后释放该名称下各加载类型的句柄。
        /// 调用前应停止使用该资源并销毁相关实例；不允许卸载正在加载的资源。
        /// 返回是否释放了句柄，不代表共享 Bundle 已从内存卸载。
        /// </summary>
        public UniTask<bool> UnloadAsync(string objName)
        {
            return UnloadAsync(objName, false);
        }

        /// <summary>
        /// 卸载指定资源及其 Sprite 缓存；destroyInstances 为 true 时，先销毁并移除该资源的实例，
        /// 否则调用前应自行销毁相关实例。等待下一帧后释放句柄，不允许卸载正在加载的资源。
        /// 返回是否释放了句柄，不代表共享 Bundle 已从内存卸载。
        /// </summary>
        public async UniTask<bool> UnloadAsync(string objName, bool destroyInstances)
        {
            _loadCancellation.Token.ThrowIfCancellationRequested();
            if (!_assetHandleDic.TryGetValue(objName, out var handles))
            {
                if (destroyInstances)
                {
                    DestroyGameObjects(objName);
                }
                return false;
            }
            foreach (var handle in handles.Values)
            {
                if (!handle.IsDone)
                {
                    GameGod.Instance.Log(E_Log.Error, "资源正在加载，不能单独卸载", objName);
                    return false;
                }
            }

            _assetHandleDic.Remove(objName);
            if (destroyInstances)
            {
                DestroyGameObjects(objName);
            }
            ClearSpriteCache(objName);
            await UniTask.NextFrame();
            foreach (var handle in handles.Values)
            {
                handle.Release();
            }
            return true;
        }

        /// <summary>
        /// 销毁实例，等待下一帧后释放资源句柄；完成前不得继续使用此加载器。
        /// </summary>
        public async UniTask UnloadAll()
        {
            // 同步取消本次持有期间的等待，阻止旧请求继续创建实例或取得 Sprite。
            _loadCancellation.Cancel();
            DestroyAllGameObject();
            ClearSpriteCache();
            // NextFrame 保证跨过本帧的 Destroy 阶段；Yield 可能仍在本帧恢复。
            await UniTask.NextFrame();
            ReleaseAllAssetHandle();
            _loadCancellation.Dispose();
            _loadCancellation = new CancellationTokenSource();
        }

        /// <summary>
        /// 销毁全部实例对象
        /// </summary>
        private void DestroyAllGameObject()
        {
            if (_gameObjectDic == null)
            {
                return;
            }

            foreach (var gameObjects in _gameObjectDic.Values)
            {
                DestroyGameObjects(gameObjects);
            }
            _gameObjectDic.Clear();
        }

        private void DestroyGameObjects(string objName)
        {
            if (_gameObjectDic.TryGetValue(objName, out var gameObjects))
            {
                // 先移出记录，避免销毁回调中新建的同名实例被本次卸载清理。
                _gameObjectDic.Remove(objName);
                DestroyGameObjects(gameObjects);
            }
        }

        private static void DestroyGameObjects(List<GameObject> gameObjects)
        {
            foreach (var go in gameObjects)
            {
                if (go != null)
                {
                    Object.Destroy(go);
                }
            }
            gameObjects.Clear();
        }

        /// <summary>
        /// 清理Sprite缓存
        /// </summary>
        private void ClearSpriteCache()
        {
            foreach (var sprites in _spriteDic.Values)
            {
                DestroySprites(sprites);
            }
            _spriteDic.Clear();
        }

        private void ClearSpriteCache(string atlasName)
        {
            if (_spriteDic.TryGetValue(atlasName, out var sprites))
            {
                DestroySprites(sprites);
                _spriteDic.Remove(atlasName);
            }
        }

        private static void DestroySprites(Dictionary<string, Sprite> sprites)
        {
            // GetSprite 返回的是克隆 Sprite，先销毁，再释放图集句柄。
            foreach (var sprite in sprites.Values)
            {
                if (sprite != null)
                {
                    Object.Destroy(sprite);
                }
            }
        }

        /// <summary>
        /// 释放全部资源句柄
        /// </summary>
        private void ReleaseAllAssetHandle()
        {
            if (_assetHandleDic.Count == 0)
            {
                return;
            }

            foreach (var handles in _assetHandleDic.Values)
            {
                foreach (var handle in handles.Values)
                {
                    handle.Release();
                }
            }
            _assetHandleDic.Clear();
        }
        #endregion
    }
}
