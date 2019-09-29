using App_ui.Common;
using App_ui.DllImport;
using App_ui.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace App_ui.Views.SubViews
{
    /// <summary>
    /// BlurView.xaml 的交互逻辑
    /// </summary>
    public partial class BlurView : Window
    {
        public BlurView()
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
        /// 事件：文本框文本预改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.\\-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        /// <summary>
        /// 事件：文本框按键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

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

            CVAlgorithms.CvpBlur(OldBmpInfo.data, vm.CurrBmp.Width, vm.CurrBmp.Height, OldBmpInfo.step, value, value, ref CurrImgInfo);

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
        /// 确定按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 取消按钮单击事件
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
    }
}
