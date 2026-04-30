///*********************************************
// * BFramework
// * 打AB包工具
// * 创建时间：2022/12/29 15:23:46
// *********************************************/

using HybridCLR.Editor;
using HybridCLR.Editor.AOT;
using HybridCLR.Editor.Commands;
using Newtonsoft.Json;
using Obfuz.Settings;
using Obfuz4HybridCLR;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// HybridCLR扩展
    /// </summary>
    public class HybridCLREditor : Editor
    {
        /// <summary>
        /// 项目路径
        /// </summary>
        public static string RootPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        /// <summary>
        /// AotDll输出路径
        /// </summary>
        public static string AotDllOutPath = RootPath + "HybridCLRData/AssembliesPostIl2CppStrip/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        /// <summary>
        /// 加密Dll输出路径
        /// </summary>
        public static string ObfuzDllOutPath = RootPath + "Library/Obfuz/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/ObfuscatedHotUpdateAssemblies";
        /// <summary>
        /// 普通Dll输出路径
        /// </summary>
        public static string NormalDllOutPath = RootPath + "HybridCLRData/HotUpdateDlls/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        /// <summary>
        /// 项目HotfixDll输出路径
        /// </summary>
        public static string AssetHotfixDllPath = RootPath + "Assets/Hotfix/";

        ////处理加密DLL和AOTDLL
        //[MenuItem("HybridCLR/Move AOT And Obfuz Dll", false, 2)]
        //public static void MoveAOTAndObfuzDll()
        //{
        //    //清空目录下的Dll
        //    ClearHotfixDirDll();

        //    //AOTDll
        //    foreach (var dll in AOTGenericReferences.PatchedAOTAssemblyList)
        //    {
        //        string aotDllPath = AotDllOutPath + "/" + dll;
        //        if (File.Exists(aotDllPath))
        //        {
        //            string targetPath = AssetHotfixDllPath + dll + ".bytes";
        //            File.Copy(aotDllPath, targetPath, true);
        //        }
        //    }

        //    //混淆Dll
        //    foreach (var filePath in Directory.GetFiles(ObfuzDllOutPath))
        //    {
        //        if (File.Exists(filePath))
        //        {
        //            string targetPath = AssetHotfixDllPath + Path.GetFileName(filePath) + ".bytes";
        //            File.Copy(filePath, targetPath, true);
        //        }
        //    }

        //    AssetDatabase.Refresh();
        //    Debug.Log("混淆Dll与AOTDll移动完成");
        //    //回收资源
        //    System.GC.Collect();
        //    //刷新编辑器
        //    AssetDatabase.Refresh();
        //}

        ////处理普通DLL和AOTDLL
        //[MenuItem("HybridCLR/Move AOT And Normal Dll", false, 2)]
        //public static void MoveAOTAndNormalDll()
        //{
        //    //清空目录下的Dll
        //    ClearHotfixDirDll();

        //    //AOTDll
        //    foreach (var dll in AOTGenericReferences.PatchedAOTAssemblyList)
        //    {
        //        string aotDllPath = AotDllOutPath + "/" + dll;
        //        if (File.Exists(aotDllPath))
        //        {
        //            string targetPath = AssetHotfixDllPath + dll + ".bytes";
        //            File.Copy(aotDllPath, targetPath, true);
        //        }
        //    }

        //    //普通Dll
        //    foreach (var filePath in Directory.GetFiles(NormalDllOutPath))
        //    {
        //        if (File.Exists(filePath))
        //        {
        //            string targetPath = AssetHotfixDllPath + Path.GetFileName(filePath) + ".bytes";
        //            File.Copy(filePath, targetPath, true);
        //        }
        //    }

        //    AssetDatabase.Refresh();
        //    Debug.Log("热更Dll与AOTDll移动完成");
        //    //回收资源
        //    System.GC.Collect();
        //    //刷新编辑器
        //    AssetDatabase.Refresh();
        //}

        /// <summary>
        /// 清空目录下的Dll
        /// </summary>
        private static void ClearHotfixDirDll()
        {
            var dir = new DirectoryInfo(AssetHotfixDllPath);
            foreach (var item in dir.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                if (item.Extension == ".bytes")
                {
                    File.Delete(item.FullName);
                }
            }
        }

        //生成 混淆+多态 DLL
        [MenuItem("HybridCLR/ObfuzExtension/CompileAndObfuscatePolymorphicDll")]
        public static void CompileAndObfuscatePolymorphicDll()
        {
            ClearHotfixDirDll();

            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);

            string obfuscatedHotUpdateDllPath = PrebuildCommandExt.GetObfuscatedHotUpdateAssemblyOutputPath(target);
            //生成混淆DLL
            ObfuscateUtil.ObfuscateHotUpdateAssemblies(target, obfuscatedHotUpdateDllPath);

            Directory.CreateDirectory(Application.streamingAssetsPath);

            string hotUpdateDllPath = $"{SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target)}";
            List<string> obfuscationRelativeAssemblyNames = ObfuzSettings.Instance.assemblySettings.GetObfuscationRelativeAssemblyNames();

            string srcDir = string.Empty;
            string srcFile = string.Empty;
            string dstFile = string.Empty;
            foreach (string assName in SettingsUtil.HotUpdateAssemblyNamesIncludePreserved)
            {
                srcDir = obfuscationRelativeAssemblyNames.Contains(assName) ? obfuscatedHotUpdateDllPath : hotUpdateDllPath;
                srcFile = $"{srcDir}/{assName}.dll";
                dstFile = $"{Application.dataPath}/Hotfix/{assName}.dll.bytes";
                if (File.Exists(srcFile))
                {
                    //File.Copy(srcFile, dstFile, true);
                    //生成多态DLL
                    ObfuscateUtil.GeneratePolymorphicDll(srcFile, dstFile);
                    Debug.Log($"[ObfuscateAndPolymorphic] Copy {srcFile} to {dstFile}");
                }
            }

            //需要生成多态DLL的补充元数据程序集
            foreach (string assName in AOTGenericReferences.PatchedAOTAssemblyList)
            {
                srcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
                srcFile = $"{srcDir}/{assName}";
                dstFile = $"{Application.dataPath}/Hotfix/{assName}.bytes";
                if (!File.Exists(srcFile))
                    continue;
                //生成多态DLL
                ObfuscateUtil.GeneratePolymorphicDll(srcFile, dstFile);
                Debug.Log($"[ObfuscateAndPolymorphic] Copy {srcFile} to {dstFile}");
            }
        }
    }
}
