using App_ui.Common;
using App_ui.DllImport;
using App_ui.ViewModels;
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

        #region Public field

        /// <summary>
        /// 当前图片信息
        /// </summary>
        public ImageInfo CurrImgInfo;

        /// <summary>
        /// 保存旧的位图信息
        /// </summary>
        public BitmapInfo OldBmpInfo;

        #endregion

        /// <summary>
        /// 事件：鼠标点击后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            var value = (int)(sender as Slider).Value;

            // 计时开始
            vm._watch = Stopwatch.StartNew();

            CVAlgorithms.CvpThreshold(OldBmpInfo.data, vm.CurrBmp.Width, vm.CurrBmp.Height, OldBmpInfo.step, value, 255, 0, ref CurrImgInfo);

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

        }
    }
}
