using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEICConverter.Models
{
    class ContentDialogHelper
    {
        public static XamlRoot XamlRoot { get; set; }
        static ContentDialog dialog = new ContentDialog();
        public static async Task ShowDialog(string Title,string Content)
        {


            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = XamlRoot;
            dialog.Title = Title;
            dialog.PrimaryButtonText = "确定";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = Content;

            var result = await dialog.ShowAsync();

        }
    }
}
