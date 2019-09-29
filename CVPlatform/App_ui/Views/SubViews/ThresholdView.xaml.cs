using App_ui.Common;
using App_ui.DllImport;
using App_ui.ViewModels;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace App_ui.Views.SubViews
{
    /// <summary>
    /// ThresholdView.xaml 的交互逻辑
    /// </summary>
    public partial class ThresholdView : Window
    {
        public ThresholdView()
        {
            InitializeComponent();
        }

        #region Private field

        /// <summary>
        /// 当前操作的视图模型
        /// </summary>
        private MainViewModel vm;

        #endregion

        #region Public field

        /// <summary>
        /// 当前图片信息
        /// </summary>
        public ImageInfo CurrImgInfo;

        /// <summary>
        /// 保存旧的位图信息
        /// </summary>
        public BitmapInfo OldBmpInfo;

        /// <summary>
        /// 二值化算法标志
        /// </summary>
        enum ThresholdTypes
        {
            THRESH_BINARY = 0,
            THRESH_BINARY_INV = 1,
            THRESH_TRUNC = 2,
            THRESH_TOZERO = 3,
            THRESH_TOZERO_INV = 4,
            THRESH_MASK = 7,        // 不支持
            THRESH_OTSU = 8,        // 不支持32位
            THRESH_TRIANGLE = 16    // 不支持32位
        };

        /// <summary>
        /// 当前选择的二值化算法
        /// </summary>
        ThresholdTypes CurrThresholdType = 0;

        /// <summary>
        /// 当前操作的值
        /// </summary>
        public int CurrThresholdValue = 0;

        #endregion

        /// <summary>
        /// 事件：鼠标点击后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CurrThresholdValue = (int)(sender as Slider).Value;
            vm = DataContext as MainViewModel;

            // 计时开始
            vm._watch = Stopwatch.StartNew();

            CVAlgorithms.CvpThreshold(OldBmpInfo.data, vm.CurrBmp.Width, vm.CurrBmp.Height, OldBmpInfo.step, CurrThresholdValue, 255, (int)CurrThresholdType, ref CurrImgInfo);

            byte[] imagePixels = new byte[CurrImgInfo.size];
            Marshal.Copy(CurrImgInfo.data, imagePixels, 0, CurrImgInfo.size);
            vm.CurrBitmapImage = ImageEx.ByteToBitmapImage(imagePixels);
            // 释放内存
            CVAlgorithms.ReleaseMemUseFree(CurrImgInfo.data);

            // 计时结束
            vm._watch.Stop();
            vm.StatusText = "Execution time: " + vm._watch.ElapsedMilliseconds + " ms.";
        }

        /// <summary>
        /// 事件：确定按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 事件：取消按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;

            // 计时开始
            vm._watch = Stopwatch.StartNew();

            // 恢复最初图像状态，此处调用0内核blur函数跳过，实现过程看CvpBlur函数实现
            CVAlgorithms.CvpBlur(OldBmpInfo.data, vm.CurrBmp.Width, vm.CurrBmp.Height, OldBmpInfo.step, 0, 0, ref CurrImgInfo);

            byte[] imagePixels = new byte[CurrImgInfo.size];
            Marshal.Copy(CurrImgInfo.data, imagePixels, 0, CurrImgInfo.size);
            vm.CurrBitmapImage = ImageEx.ByteToBitmapImage(imagePixels);
            // 释放内存
            CVAlgorithms.ReleaseMemUseFree(CurrImgInfo.data);

            // 计时结束
            vm._watch.Stop();
            vm.StatusText = "Execution time: " + vm._watch.ElapsedMilliseconds + " ms.";
            Close();
        }

        /// <summary>
        /// 事件：下拉框选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cb_method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = (e.AddedItems[0] as ComboBoxItem).Content as string;
            Enum.TryParse(item, out CurrThresholdType);
            vm = DataContext as MainViewModel;
            if (null != vm) // 防止第一次加载没初始化视图模型的情况
            {
                // 计时开始
                vm._watch = Stopwatch.StartNew();

                CVAlgorithms.CvpThreshold(OldBmpInfo.data, vm.CurrBmp.Width, vm.CurrBmp.Height, OldBmpInfo.step, CurrThresholdValue, 255, (int)CurrThresholdType, ref CurrImgInfo);
                byte[] imagePixels = new byte[CurrImgInfo.size];
                Marshal.Copy(CurrImgInfo.data, imagePixels, 0, CurrImgInfo.size);
                vm.CurrBitmapImage = ImageEx.ByteToBitmapImage(imagePixels);
                // 释放内存
                CVAlgorithms.ReleaseMemUseFree(CurrImgInfo.data);

                // 计时结束
                vm._watch.Stop();
                vm.StatusText = "Execution time: " + vm._watch.ElapsedMilliseconds + " ms.";
            }
        }
    }
}
