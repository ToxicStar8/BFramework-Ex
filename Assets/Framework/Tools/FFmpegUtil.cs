/*********************************************
 * BFramework
 * FFmpeg工具
 * 创建时间：2023/06/30 15:34:41
 *********************************************/
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Framework
{
    /// <summary>
    /// FFmpeg工具
    /// </summary>
    public static class FFmpegUtil
    {
        private static FFmpegInfo _ffpInfo = new FFmpegInfo();

        /// <summary>
        /// 执行命令行
        /// </summary>
        public static FFmpegInfo Execute(string cmdReadStr, Action callback)
        {
            _ffpInfo.Execute(cmdReadStr, callback);
            return _ffpInfo;
        }

        /// <summary>
        /// 视频合成
        /// </summary>
        /// <param name="outPath">输出的文件夹</param>
        /// <param name="newVideoName">新视频的名字</param>
        public static void CombineVideo(string video1FullPath, string video2FullPath, Vector2 cutSize, Vector2 video2Pos, string outPath = null, string newVideoName = null)
        {
            //常规检测
            newVideoName = CheckNewVideoName(newVideoName);
            outPath = CheckOutPath(outPath);
            //正式执行
            var outFullPath = outPath + $"/{newVideoName}.mp4";
            Execute($"-i {video1FullPath} -i {video2FullPath} -filter_complex \"pad=1080:1920:color=green[x0];[0:v]scale=w=1080:h=1920[inn0];[x0][inn0]overlay=0:0[x1];[1:v]scale=w={cutSize.x}:h={cutSize.y}[inn1];[x1][inn1]overlay={video2Pos.x}:{video2Pos.y}\" {outFullPath}", null);
        }

        /// <summary>
        /// 视频裁剪
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="intervalTime">裁剪时长</param>
        /// <param name="inputFullPath">输入的文件全路径名</param>
        /// <param name="outPath">输出的文件夹</param>
        /// <param name="newVideoName">新视频的名字</param>
        public static void CutVideo(int startTime, int intervalTime, string inputFullPath, string outPath = null, string newVideoName = null)
        {
            //常规检测
            newVideoName = CheckNewVideoName(newVideoName);
            CheckOutPath(outPath);
            //正式执行
            var outFullPath = outPath + $"/{newVideoName}.mp4";
            Execute($"-i {inputFullPath} -ss {startTime} -t {intervalTime} -codec copy {outFullPath}", null);
        }

        /// <summary>
        /// 检测是否有视频名
        /// </summary>
        public static string CheckNewVideoName(string newVideoName)
        {
            //视频名
            if (string.IsNullOrWhiteSpace(newVideoName))
            {
                newVideoName = Application.productName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            }
            return newVideoName;
        }

        /// <summary>
        /// 检测是否有输出路径
        /// </summary>
        public static string CheckOutPath(string outPath)
        {
            //输出位置
            if (string.IsNullOrWhiteSpace(outPath))
            {
                outPath = Application.persistentDataPath + "/Video";
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
            }
            return outPath;
        }

        /// <summary>
        /// 停止当前的行为
        /// </summary>
        public static void CloseFFmpeg()
        {
            _ffpInfo.CloseFFmpegProcess();
        }

        public class FFmpegInfo
        {
            private Process _ffp;
            private string _ffpPath = Application.streamingAssetsPath + "/ffmpeg.exe";
            private bool _isRunning;

            /// <summary>
            /// 执行FFmpeg的命令行
            /// </summary>
            /// <param name="cmdReadStr"></param>
            /// <param name="isShowCmd"></param>
            public void Execute(string cmdReadStr, Action callback, bool isShowCmd = false)
            {
                //正在执行中
                if (_isRunning)
                {
                    Debug.LogError("视频处理正在执行中，请稍后再试！");
                    return;
                }
                //杀死已有的ffmpeg进程，不要加.exe后缀
                Process[] goDie = Process.GetProcessesByName("ffmpeg");
                foreach (Process p in goDie)
                {
                    Debug.Log("杀死已有的ffmpeg进程");
                    p.Kill();
                }
                //执行新的
                new Thread(() =>
                {
                    _isRunning = true;
                    Thread.CurrentThread.IsBackground = true;

                    _ffp = new Process();
                    _ffp.StartInfo.RedirectStandardOutput = true;
                    _ffp.StartInfo.RedirectStandardError = true;
                    _ffp.StartInfo.FileName = _ffpPath;                 // 进程可执行文件位置
                    _ffp.StartInfo.Arguments = cmdReadStr;              // 传给可执行文件的命令行参数
                    _ffp.StartInfo.CreateNoWindow = !isShowCmd;         // 是否显示控制台窗口
                    _ffp.StartInfo.UseShellExecute = isShowCmd;         // 是否使用操作系统Shell程序启动进程
                    _ffp.StartInfo.RedirectStandardInput = true;        //这句一定需要，用于模拟该进程控制台的输入
                    _ffp.OutputDataReceived += (s, e) => ShowResult(e);
                    _ffp.ErrorDataReceived += (s, e) => ShowResult(e);
                    _ffp.Start();

                    _ffp.BeginOutputReadLine();
                    _ffp.BeginErrorReadLine();

                    _ffp.WaitForExit();

                    Debug.Log("ffmpeg执行完毕");
                    _isRunning = false;
                    _ffp.Close();
                    callback?.Invoke();
                }).Start();
            }

            private static void ShowResult(DataReceivedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.Log(e.Data);
                }
            }

            /// <summary>
            /// 主动退出进程
            /// </summary>
            public void CloseFFmpegProcess()
            {
                if (_ffp != null)
                {
                    try
                    {
                        _ffp.StandardInput.WriteLine("q");//在这个进程的控制台中模拟输入q,用于暂停录制
                        _ffp.Close();
                        _ffp.Dispose();

                        _isRunning = false;

                        _ffp = null;
                        Debug.Log("主动退出FFmpeg进程");
                    }
                    catch (Exception)
                    {
                        Debug.LogError("没有进程可杀");
                    }

                }
                else
                {
                    Debug.LogError("FFmpeg进程为空");
                }
            }
        }
    }
}
