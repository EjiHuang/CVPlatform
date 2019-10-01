using App_ui.DllImport;
using App_ui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    /// GrayHistView.xaml 的交互逻辑
    /// </summary>
    public partial class GrayHistView : Window
    {
        public GrayHistView()
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
        /// 表格值
        /// </summary>
        public int[] ChartValues { get; set; }

        #endregion

        /// <summary>
        /// 事件：窗口加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as MainViewModel;

            CVAlgorithms.CvpCalcGrayHist(vm.CurrBmpInfo.data, vm.CurrBmp.Width, vm.CurrBmp.Height, vm.CurrBmpInfo.step, ref CurrImgInfo);

            int[] hist = new int[256];
            Marshal.Copy(CurrImgInfo.r4, hist, 0, 256);
            CVAlgorithms.ReleaseMemUseDelete(CurrImgInfo.r4);

            vm.StatusText = $"Execution time: {CurrImgInfo.r1} ms.";

            // 绘图
            ChartValues = hist.Select(i => i).ToArray();
        }
    }
}
