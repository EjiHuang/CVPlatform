using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace App_ui.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Public field

        /// <summary>
        /// 当前图片路径
        /// </summary>
        public string CurrImagePath
        {
            get => GetProperty(() => CurrImagePath);
            set => SetProperty(() => CurrImagePath, value, () =>
            {
                CurrBitmapImage = new BitmapImage(new Uri(CurrImagePath));
            });
        }

        public BitmapImage CurrBitmapImage
        {
            get => GetProperty(() => CurrBitmapImage);
            set => SetProperty(() => CurrBitmapImage, value);
        }

        #endregion

        #region Command

        /// <summary>
        /// 命令：打开新图片
        /// </summary>
        [Command]
        public void OpenNewImageCommand()
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
            if (ret.HasValue && ret.Value)
            {
                CurrImagePath = imgFileDialog.FileName;
            }
        }

        /// <summary>
        /// 命令：显示关于对话框
        /// </summary>
        [Command]
        public void ShowAboutDialogCommand()
        {
            MessageBox.Show("Test");
        }

        #endregion
    }
}
