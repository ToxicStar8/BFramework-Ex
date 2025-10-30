/*********************************************
 * BFramework
 * 图集生成工具
 * 创建时间：2023/01/29 16:37:36
 *********************************************/
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Framework
{
    /// <summary>
    /// 图集生成
    /// </summary>
    public class BuildSpriteAtlas : Editor
    {
        /// <summary>
        /// 图集的 Asset 相对路径（用于 AssetDatabase）
        /// </summary>
        public static string SpriteAssetPath = "Assets/GameData/Art/Sprites";

        /// <summary>
        /// 图集的磁盘绝对路径（用于文件系统枚举）
        /// </summary>
        public static string SpriteDiskPath = Application.dataPath + "/GameData/Art/Sprites";

        /// <summary>
        /// 图片代码磁盘路径
        /// </summary>
        public static string SpriteScriptPath = Application.dataPath + "/GameData/Scripts/Define/AtlasSprite.cs";

        /// <summary>
        /// 图集代码磁盘路径
        /// </summary>
        public static string AtlasScriptPath = Application.dataPath + "/GameData/Scripts/Define/AtlasName.cs";

        [MenuItem("BFramework/Build SpriteAtlas", false, 0)]
        public static void BuildAtlas()
        {
            GenerateSpriteAtlas();
            GenerateSpriteScript();
            GenerateAtlasScript();
            // 主动打包一次（可选）
            PackAllAtlasesInFolder(SpriteAssetPath);

            Debug.Log("图集代码生成完毕!");
            System.GC.Collect();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成图片的图集（使用 .spriteatlas，由 V2 导入器接管）
        /// </summary>
        private static void GenerateSpriteAtlas()
        {
            var pathDir = new DirectoryInfo(SpriteDiskPath);
            if (!pathDir.Exists)
            {
                Debug.LogWarning($"路径不存在：{SpriteDiskPath}");
                return;
            }

            foreach (var item in pathDir.GetFileSystemInfos("*.*", SearchOption.TopDirectoryOnly))
            {
                if (item is not DirectoryInfo parentDir) continue;

                // 确保子目录存在（Assets 相对路径）
                string childAssetFolder = $"{SpriteAssetPath}/{parentDir.Name}";
                EnsureFolder(childAssetFolder);

                string atlasName = parentDir.Name + ".spriteatlas";
                string atlasAssetPath = $"{childAssetFolder}/{atlasName}";

                // 删除旧的 atlas 资产（使用 AssetDatabase，避免 meta 残留）
                var existAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasAssetPath);
                if (existAtlas != null)
                {
                    AssetDatabase.DeleteAsset(atlasAssetPath);
                }

                // 创建 atlas，并设置参数
                var atlas = new SpriteAtlas();

                var packSetting = new SpriteAtlasPackingSettings
                {
                    blockOffset = 1,
                    enableRotation = false,
                    enableTightPacking = false,
                    padding = 2,
                };
                atlas.SetPackingSettings(packSetting);

                var textureSetting = new SpriteAtlasTextureSettings
                {
                    readable = false,
                    generateMipMaps = false,
                    sRGB = true,
                    filterMode = FilterMode.Bilinear,
                };
                atlas.SetTextureSettings(textureSetting);

                // 可按平台分别设置（示例：通用设置）
                var platformSetting = new TextureImporterPlatformSettings
                {
                    name = string.Empty, // Default
                    maxTextureSize = 2048,
                    format = TextureImporterFormat.Automatic,
                    crunchedCompression = true,
                    textureCompression = TextureImporterCompression.Compressed,
                    compressionQuality = 50,
                };
                atlas.SetPlatformSettings(platformSetting);

                // 创建 Asset
                AssetDatabase.CreateAsset(atlas, atlasAssetPath);

                // 添加整个子文件夹为 packable
                Object folderObj = AssetDatabase.LoadAssetAtPath<Object>(childAssetFolder);
                if (folderObj != null)
                {
                    atlas.Add(new[] { folderObj });
                }

                // 允许包含到构建（可选）
                SpriteAtlasExtensions.SetIncludeInBuild(atlas, true);

                EditorUtility.SetDirty(atlas);
                AssetDatabase.SaveAssets();
            }

            Debug.Log("图集生成完毕!");
        }

        /// <summary>
        /// 生成图片代码文件
        /// </summary>
        private static void GenerateSpriteScript()
        {
            string temp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：AtlasSprite.cs
 * 创建时间：#Time
 *********************************************/
using Framework;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 快捷获得Sprite
    /// </summary>
    public static class AtlasSprite
    {#SpriteList
    }
}
";
            string spListStr = string.Empty;
            var pathDir = new DirectoryInfo(SpriteDiskPath);
            if (!pathDir.Exists)
            {
                Debug.LogWarning($"路径不存在：{SpriteDiskPath}");
            }
            else
            {
                //最外层 用于生成图集名称
                foreach (var dirInfo in pathDir.GetDirectories("*.*", SearchOption.TopDirectoryOnly))
                {
                    //第二层 生成脚本文件的地方
                    foreach (var item in dirInfo.GetFileSystemInfos("*.*", SearchOption.AllDirectories))
                    {
                        if (item is not FileInfo fileInfo) continue;

                        var ext = fileInfo.Extension.ToLowerInvariant();
                        // 过滤 meta 与 atlas 本体
                        if (ext == ".meta" || ext == ".spriteatlas") continue;

                        //将图片带下划线的全部转换为首字母大写并去除下划线
                        var fnName = $"{dirInfo.Name}_";
                        var nameNoExt = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        var strArr = nameNoExt.Split('_');

                        if (strArr.Length != 1)
                        {
                            for (int i = 0; i < strArr.Length; i++)
                            {
                                fnName += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strArr[i]);
                            }
                        }
                        else
                        {
                            fnName += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameNoExt.Substring(0, 1)) + nameNoExt.Remove(0, 1);
                        }

                        spListStr += $"\r\n        public static Sprite {fnName}() => UIManager.instance.LoadSprite(AtlasName.{dirInfo.Name}, \"{nameNoExt}\");";
                    }
                }
            }

            var scripts = File.CreateText(SpriteScriptPath);
            temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            temp = temp.Replace("#SpriteList", spListStr);
            scripts.Write(temp);
            scripts.Close();
        }

        /// <summary>
        /// 生成图集代码文件
        /// </summary>
        private static void GenerateAtlasScript()
        {
            string temp = @"/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：AtlasName.cs
 * 创建时间：#Time
 *********************************************/
using Framework;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 快捷获得图集名
    /// </summary>
    public static class AtlasName
    {#AtlasList
    }
}
";
            string atlasNameStr = string.Empty;
            var pathDir = new DirectoryInfo(SpriteDiskPath);
            if (!pathDir.Exists)
            {
                Debug.LogWarning($"路径不存在：{SpriteDiskPath}");
            }
            else
            {
                foreach (var dirInfo in pathDir.GetDirectories("*.*", SearchOption.TopDirectoryOnly))
                {
                    // 统一用 .spriteatlas
                    atlasNameStr += $"\r\n        public static string {dirInfo.Name} => \"{dirInfo.Name}.spriteatlas\";";
                }
            }

            var scripts = File.CreateText(AtlasScriptPath);
            temp = temp.Replace("#Time", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            temp = temp.Replace("#AtlasList", atlasNameStr);
            scripts.Write(temp);
            scripts.Close();
        }

        /// <summary>
        /// 确保 Asset 相对路径下的文件夹存在（Assets/...）
        /// </summary>
        private static void EnsureFolder(string assetFolder)
        {
            assetFolder = assetFolder.Replace("\\", "/");
            if (AssetDatabase.IsValidFolder(assetFolder)) return;

            var parent = Path.GetDirectoryName(assetFolder)?.Replace("\\", "/");
            var name = Path.GetFileName(assetFolder);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }
            if (!AssetDatabase.IsValidFolder(assetFolder))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        /// <summary>
        /// 打包指定目录下的所有 SpriteAtlas（可选）
        /// </summary>
        private static void PackAllAtlasesInFolder(string assetFolder)
        {
            var guids = AssetDatabase.FindAssets("t:SpriteAtlas", new[] { assetFolder });
            var atlases = new SpriteAtlas[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                atlases[i] = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            }
            if (atlases.Length > 0)
            {
                SpriteAtlasUtility.PackAtlases(atlases, EditorUserBuildSettings.activeBuildTarget, false);
            }
        }
    }
}