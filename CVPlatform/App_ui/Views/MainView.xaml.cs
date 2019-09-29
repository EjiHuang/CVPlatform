using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace App_ui
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     事件：文本框编辑事件，使文本框一直保持文本最后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConsoleView_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConsoleView.SelectionStart = ConsoleView.Text.Length;
            ConsoleView.ScrollToEnd();
        }

    }
}
