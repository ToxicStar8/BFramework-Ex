/*********************************************
 * BFramework
 * 轻量化加载器
 * 更新时间：2026/04/30 15:19:41
 *********************************************/
using MainPackage;
using System.Collections.Generic;
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
            _gameObjectList = new();
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
            return pool.CreateClassObj();
        }
        
        /// <summary>
        /// 回收加载器
        /// </summary>
        public static void Recycle(LoadHelper loadHelper)
        {
            var pool = GameGod.Instance.PoolManager.CreateClassObjectPool<LoadHelper>();
            loadHelper.UnloadAll();
            pool.Recycle(loadHelper);
        }

        /// <summary>
        /// 已加载资源句柄字典
        /// </summary>
        private Dictionary<string, AssetHandle> _assetHandleDic;

        /// <summary>
        /// 已加载Sprite字典
        /// </summary>
        private Dictionary<string, Sprite> _spriteDic;

        /// <summary>
        /// 实例对象列表
        /// </summary>
        private List<GameObject> _gameObjectList;

        #region 加载图片
        /// <summary>
        /// 同步加载Sprite
        /// </summary>
        public Sprite LoadSpriteSync(string atlasName, string spriteName)
        {
            //因为美术资源太多重复的名字，这里直接使用 [文件夹_文件] 的格式来做可寻址
            var spaName = $"{atlasName}_{atlasName}";
            var spName = $"{atlasName}_{spriteName}";
            if (_spriteDic.TryGetValue(spName, out var cachedSprite))
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

            TrackSprite(spName, sprite);
            return sprite;
        }

        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        public async UniTask<Sprite> GetSpriteAsync(string atlasName, string spriteName)
        {
            //因为美术资源太多重复的名字，这里直接使用 [文件夹_文件] 的格式来做可寻址
            var spaName = $"{atlasName}_{atlasName}";
            var spName = $"{atlasName}_{spriteName}";
            if (_spriteDic.TryGetValue(spName, out var cachedSprite))
            {
                return cachedSprite;
            }

            var spa = await LoadAsync<SpriteAtlas>(spaName);
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

            TrackSprite(spName, sprite);
            return sprite;
        }
        #endregion

        #region 加载资源
        /// <summary>
        /// 加载资源 带后缀
        /// </summary>
        public T LoadSync<T>(string objName) where T : Object
        {
            if (!_assetHandleDic.TryGetValue(objName, out var handle))
            {
                handle = Package.LoadAssetSync<T>(objName);
                TrackAssetHandle(objName, handle);
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
        /// 异步加载资源 带后缀
        /// </summary>
        public async UniTask<T> LoadAsync<T>(string objName) where T : Object
        {
            if (!_assetHandleDic.TryGetValue(objName, out var handle))
            {
                handle = Package.LoadAssetAsync<T>(objName);
                await handle;
                TrackAssetHandle(objName, handle);
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
            TrackGameObject(go);
            return go;
        }

        /// <summary>
        /// 异步创建实例对象
        /// </summary>
        public async UniTask<GameObject> CreateGameObjectAsync(string objName, Transform trans = null)
        {
            var obj = await LoadAsync<GameObject>(objName);
            if (obj == null)
            {
                GameGod.Instance.Log(E_Log.Error, "创建实例资源为空", objName);
                return null;
            }

            var go = Object.Instantiate(obj, trans);
            TrackGameObject(go);
            return go;
        }

        private void TrackAssetHandle(string objName, AssetHandle handle)
        {
            if (handle == null)
            {
                return;
            }

            _assetHandleDic[objName] = handle;
        }

        private void TrackSprite(string spriteName, Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            _spriteDic[spriteName] = sprite;
        }

        private void TrackGameObject(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            _gameObjectList.Add(go);
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">加载场景一定要用全大小写名，全小写的名字只能用于预先放在BuildSetting里使用</param>
        public void LoadSceneSync(string sceneName)
        {
            LoadSync(sceneName + ".unity");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">加载场景一定要用全大小写名，全小写的名字只能用于预先放在BuildSetting里使用</param>
        public async UniTask LoadSceneAsync(string sceneName)
        {
            await LoadAsync(sceneName + ".unity");
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        #endregion

        #region 卸载资源
        /// <summary>
        /// 卸载全部资源
        /// </summary>
        public void UnloadAll()
        {
            DestroyAllGameObject();
            ClearSpriteCache();
            ReleaseAllAssetHandle();
        }

        /// <summary>
        /// 销毁全部实例对象
        /// </summary>
        private void DestroyAllGameObject()
        {
            if (_gameObjectList == null)
            {
                return;
            }

            foreach (var go in _gameObjectList)
            {
                if (go != null)
                {
                    Object.Destroy(go);
                }
            }
            _gameObjectList.Clear();
        }

        /// <summary>
        /// 清理Sprite缓存
        /// </summary>
        private void ClearSpriteCache()
        {
            if (_spriteDic == null)
            {
                return;
            }

            _spriteDic.Clear();
        }

        /// <summary>
        /// 释放全部资源句柄
        /// </summary>
        private void ReleaseAllAssetHandle()
        {
            if (_assetHandleDic == null)
            {
                return;
            }

            foreach (var item in _assetHandleDic)
            {
                item.Value?.Release();
            }
            _assetHandleDic.Clear();
        }
        #endregion
    }
}