﻿/*********************************************
 * BFramework
 * 通用加载助手（加载器）
 * 创建时间：2023/01/29 14:36:31
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// 通用加载助手（加载器）
    /// </summary>
    public class LoadHelper
    {
        public LoadHelper() { }

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
        /// 图集名列表
        /// </summary>
        private List<string> _atlasNameList;

        /// <summary>
        /// 资源名列表
        /// </summary>
        private List<string> _objNameList;

        /// <summary>
        /// 同步加载Sprite
        /// </summary>
        public Sprite GetSpriteSync(string atlasName, string spriteName)
        {
            if (_atlasNameList == null)
            {
                _atlasNameList = new List<string>();
            }
            //真正加载Sp的地方
            var sp = GameGod.Instance.LoadManager.GetSprite(atlasName, spriteName);
            if (sp == null)
            {
                GameGod.Instance.Log(E_Log.Error, "图片资源为空", spriteName);
                return null;
            }
            //追加到记录列表里
            if (!_atlasNameList.Contains(atlasName))
            {
                _atlasNameList.Add(atlasName);
            }
            return sp;
        }

        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        public async UniTask<Sprite> GetSpriteAsync(string atlasName, string spriteName)
        {
            var sp = GetSpriteSync(atlasName, spriteName);
            await UniTask.CompletedTask;
            return sp;
        }

        /// <summary>
        /// 加载资源 带后缀
        /// </summary>
        public T LoadSync<T>(string objName) where T : Object
        {
            var obj = LoadSync(objName) as T;
            return obj;
        }

        /// <summary>
        /// 加载资源 带后缀
        /// </summary>
        public Object LoadSync(string objName)
        {
            if (_objNameList == null)
            {
                _objNameList = new List<string>();
            }
            //真正加载资源的地方
            var obj = GameGod.Instance.LoadManager.LoadSync(objName);
            if (obj == null)
            {
                GameGod.Instance.Log(E_Log.Error, "加载资源为空", objName);
                return null;
            }
            //记录名字即可
            if (!_objNameList.Contains(objName))
            {
                _objNameList.Add(objName);
            }
            return obj;
        }

        /// <summary>
        /// 异步加载资源 带后缀
        /// </summary>
        public async UniTask<T> LoadAsync<T>(string objName) where T : Object
        {
            var obj = await LoadAsync(objName) as T;
            return obj;
        }

        /// <summary>
        /// 异步加载资源 带后缀
        /// </summary>
        public async UniTask<Object> LoadAsync(string objName)
        {
            var obj = LoadSync(objName);
            await UniTask.CompletedTask;
            return obj;
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">加载场景一定要用全大小写名，全小写的名字只能用于预先放在BuildSetting里使用</param>
        public void LoadSceneSync(string sceneName)
        {
            if (!GameEntry.Instance.IsEditorMode || GameEntry.Instance.IsRunABPackage)
            {
                LoadSync(sceneName + ".unity");
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
#if UNITY_EDITOR
            else
            {
                string path = GameGod.Instance.LoadManager.GetObjAssetPath(sceneName + ".unity");
                var parameters = new LoadSceneParameters(LoadSceneMode.Single);
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path, parameters);
            }
#endif
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">加载场景一定要用全大小写名，全小写的名字只能用于预先放在BuildSetting里使用</param>
        public async UniTask LoadSceneAsync(string sceneName)
        {
            if (!GameEntry.Instance.IsEditorMode || GameEntry.Instance.IsRunABPackage)
            {
                await LoadAsync(sceneName + ".unity");
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            }
#if UNITY_EDITOR
            else
            {
                string path = GameGod.Instance.LoadManager.GetObjAssetPath(sceneName + ".unity");
                var parameters = new LoadSceneParameters(LoadSceneMode.Single);
                await UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, parameters);
            }
#endif
        }

        /// <summary>
        /// 直接创建对象
        /// </summary>
        public GameObject CreateGameObjectSync(string objName, Transform trans = null)
        {
            var obj = LoadSync<GameObject>(objName);
            return Object.Instantiate(obj, trans);
        }

        /// <summary>
        /// 异步直接创建对象
        /// </summary>
        public async UniTask<GameObject> CreateGameObjectAsync(string objName, Transform trans = null)
        {
            var obj = await LoadAsync<GameObject>(objName);
            return Object.Instantiate(obj, trans);
        }

        /// <summary>
        /// 卸载全部资源
        /// </summary>
        public void UnloadAll()
        {
            UnloadAllSprite();
            UnloadAllObject();
        }

        /// <summary>
        /// 卸载全部Obj
        /// </summary>
        private void UnloadAllObject()
        {
            if (_objNameList == null)
            {
                return;
            }

            //关闭前移除全部Obj
            for (int i = 0, count = _objNameList.Count; i < count; i++)
            {
                string objName = _objNameList[i];
                GameGod.Instance.LoadManager.UnloadAsset(objName);
                GameGod.Instance.Log(E_Log.Framework, "Unload Object", objName);
            }
            _objNameList.Clear();
        }

        /// <summary>
        /// 卸载全部Sprite
        /// </summary>
        private void UnloadAllSprite()
        {
            if (_atlasNameList == null)
            {
                return;
            }

            //关闭前移除全部Sprite
            for (int i = 0, count = _atlasNameList.Count; i < count; i++)
            {
                string spriteName = _atlasNameList[i];
                GameGod.Instance.LoadManager.UnloadAsset(spriteName);
                GameGod.Instance.Log(E_Log.Framework, "Unload Atlas", spriteName);
            }
            _atlasNameList.Clear();
        }
    }
}