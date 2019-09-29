using App_ui.Common;
using App_ui.DllImport;
using App_ui.Views.SubViews;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace App_ui.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            // 控制台输出使用说明
            ConsoleText = CmdTag + ">.< Welcome to use CVPlatform." + Environment.NewLine;
            // 状态栏信息说明
            StatusText = "ready";
        }

        #endregion

        #region Public field

        /// <summary>
        /// 记录当前图片的路径
        /// </summary>
        public StringBuilder CurrImagePath
        {
            get => GetProperty(() => CurrImagePath);
            set => SetProperty(() => CurrImagePath, value, () =>
            {
                CurrBitmapImage = new BitmapImage(new Uri(value.ToString()));
                IsImportImage = CurrImagePath == null ? false : true;
            });
        }

        /// <summary>
        /// 交互数据
        /// </summary>
        public ImageInfo CurrImgInfo = new ImageInfo();

        /// <summary>
        /// 位图交换数据
        /// </summary>
        public BitmapInfo CurrBmpInfo = new BitmapInfo();

        /// <summary>
        /// 当前位图
        /// </summary>
        public Bitmap CurrBmp;

        /// <summary>
        /// 当前实时处理的图片
        /// </summary>
        public BitmapSource CurrBitmapImage
        {
            get => GetProperty(() => CurrBitmapImage);
            set => SetProperty(() => CurrBitmapImage, value);
        }

        /// <summary>
        /// 控制台输出文本
        /// </summary>
        public string ConsoleText
        {
            get => GetProperty(() => ConsoleText);
            set => SetProperty(() => ConsoleText, value);
        }

        /// <summary>
        ///     尾标
        /// </summary>
        public string CmdTag => DateTime.Now.ToString("T") + ": ";

        /// <summary>
        /// 状态栏信息
        /// </summary>
        public string StatusText
        {
            get => GetProperty(() => StatusText);
            set => SetProperty(() => StatusText, value);
        }

        /// <summary>
        /// 是否已经导入了图片
        /// </summary>
        public bool IsImportImage
        {
            get => GetProperty(() => IsImportImage);
            set => SetProperty(() => IsImportImage, value);
        }

        /// <summary>
        /// 保存单应性矩阵数组
        /// </summary>
        public List<IntPtr> HomographyMats = new List<IntPtr>();

        /// <summary>
        /// 计时器，用于计算执行时间，准确率不高
        /// </summary>
        public Stopwatch _watch;

        #endregion

        #region Private field



        #endregion

        #region Command

        /// <summary>
        /// 命令：导入图片
        /// </summary>
        /// <param name="obj"></param>
        [AsyncCommand]
        public void OpenNewImageCommand(object obj)
        {
            OpenFileDialog imgFileDialog = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.bmp;*.jpg;*.jpeg;*.png|" +
                            "Bitmap (*.bmp)|*.bmp|" +
                            "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                            "Portable Network Graphic (*.png)|*.png"
            };
            bool? ret = imgFileDialog.ShowDialog();

            // 计时开始
            _watch = Stopwatch.StartNew();

            if (ret.HasValue && ret.Value)
            {
                CurrImagePath = new StringBuilder(imgFileDialog.FileName);
                CVAlgorithms.CvpImread(CurrImagePath, ref CurrImgInfo);
                byte[] imagePixels = new byte[CurrImgInfo.size];
                Marshal.Copy(CurrImgInfo.data, imagePixels, 0, CurrImgInfo.size);
                Bitmap bmp = ImageEx.ConvertByteArrayToBitmap(imagePixels);
                CurrBitmapImage = ImageEx.Bitmap2BitmapImage(bmp);
                // 释放内存
                CVAlgorithms.ReleaseMemUseFree(CurrImgInfo.data);
                // 控制台输出提示
                ConsoleText += $"{CmdTag}{imgFileDialog.SafeFileName} import succeeded.{Environment.NewLine}";
            }
            // 计时结束
            _watch.Stop();
            StatusText = $"Execution time: {_watch.ElapsedMilliseconds} ms.";
        }

        /// <summary>
        /// 命令：灰度效果实现
        /// </summary>
        /// <param name="obj"></param>
        [AsyncCommand]
        public void EffectGrayCommand(object obj)
        {
            // 计时开始
            _watch = Stopwatch.StartNew();

            CurrBmp = ImageEx.BitmapImage2Bitmap(CurrBitmapImage);
            CurrBmpInfo = ImageEx.GetBitmapPixels(CurrBmp);

            CVAlgorithms.CvpGray(CurrBmpInfo.data, CurrBmp.Width, CurrBmp.Height, CurrBmpInfo.step, ref CurrImgInfo);

            byte[] imagePixels = new byte[CurrImgInfo.size];
            Marshal.Copy(CurrImgInfo.data, imagePixels, 0, CurrImgInfo.size);
            CurrBitmapImage = ImageEx.ByteToBitmapImage(imagePixels);
            // 释放内存
            CVAlgorithms.ReleaseMemUseFree(CurrImgInfo.data);

            // 计时结束
            _watch.Stop();
            StatusText = $"Execution time: {_watch.ElapsedMilliseconds} ms.";
            // 控制台输出提示
            ConsoleText += $"{CmdTag} Grayscale success.{Environment.NewLine}";
        }

        /// <summary>
        /// 命令：Blur处理
        /// </summary>
        /// <param name="obj"></param>
        [AsyncCommand]
        public void EffectBlurCommand(object obj)
        {
            CurrBmp = ImageEx.BitmapImage2Bitmap(CurrBitmapImage);
            CurrBmpInfo = ImageEx.GetBitmapPixels(CurrBmp);

            BlurView blurView = new BlurView { DataContext = this, OldBmpInfo = CurrBmpInfo };
            blurView.Show();

            // 控制台输出提示
            ConsoleText += $"{CmdTag} Blur success.{Environment.NewLine}";
        }

        /// <summary>
        /// 事件：对图像进行二值化处理
        /// </summary>
        /// <param name="obj"></param>
        [AsyncCommand]
        public void EffectThresholdCommand(object obj)
        {
            CurrBmp = ImageEx.BitmapImage2Bitmap(CurrBitmapImage);
            CurrBmpInfo = ImageEx.GetBitmapPixels(CurrBmp);

            ThresholdView thresholdView = new ThresholdView { DataContext = this, OldBmpInfo = CurrBmpInfo };
            thresholdView.Show();

            // 控制台输出提示
            ConsoleText += $"{CmdTag} Threshod success.{Environment.NewLine}";
        }

        #endregion
    }
}
