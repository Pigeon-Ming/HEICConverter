using HEICConverter.Models;
using HEICConverter.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HEICConverter
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        Microsoft.UI.Windowing.AppWindow m_appWindow;
        public MainWindow()
        {
            this.InitializeComponent();
            m_appWindow = GetAppWindowForCurrentWindow();
            //m_appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);  // This line
            // 调整窗口位置和大小，以屏幕像素为单位
            m_appWindow.Resize(new SizeInt32(1022, 642));
            this.ExtendsContentIntoTitleBar = true;
            SetVersionMessage();
        }
        private Microsoft.UI.Windowing.AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

            return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
        }

        private void Dev_OpenConverterWindow_Click(object sender, RoutedEventArgs e)
        {
            App.converter_window.Activate();
            //converterWindow.Activate();
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            if(!App.IsConverterWindowActivated)
                Environment.Exit(0);
        }

        private async void CheckForUpdate_Button_Click(object sender, RoutedEventArgs e)
        {
            await CheckForUpdateAsync();
        }

        private async void SetAsDefaultAPPButton_Click(object sender, RoutedEventArgs e)
        {
            await RequestSetDefaultAppAsync();
        }

        public void SetVersionMessage()
        {
            VersionTextBlock.Text = string.Format("{0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
        }

        public async Task RequestSetDefaultAppAsync()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:defaultapps?fileType=.heic"));
        }

        async Task CheckForUpdateAsync()
        {
            var http = new HttpClient();
            var response = await http.GetAsync("https://pigeon-ming.github.io/Versions/HEICConverter.txt");
            var result = await response.Content.ReadAsStringAsync();
            var response_Log = await http.GetAsync("https://pigeon-ming.github.io/Versions/HEICConverter.txt");
            var result_Log = await response_Log.Content.ReadAsStringAsync();
            string appVersion = Package.Current.Id.Version.Build.ToString();
            if (String.IsNullOrEmpty(result))
            {
                CheckUpdate_InfoBar.Title = "检查失败";
                CheckUpdate_InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                CheckUpdate_InfoBar.ActionButton = null;
                CheckUpdate_InfoBar.IsOpen = true;
            }
            //Debug.WriteLine(result.Substring(result.LastIndexOf(".") + 1, result.Length - result.LastIndexOf(".") - 1));
            if (Convert.ToInt32(appVersion) < Convert.ToInt32(result.Substring(result.LastIndexOf(".") + 1, result.Length - result.LastIndexOf(".") - 1)))
            {

                CheckUpdate_InfoBar.Title = "发现新版本";
                CheckUpdate_InfoBar.Content = result_Log + '\n';
                CheckUpdate_InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Warning;
                CheckUpdate_InfoBar.UpdateLayout();
                CheckUpdate_InfoBar.IsOpen = true;
            }
            else
            {
                CheckUpdate_InfoBar.Title = "您使用的是最新版本";
                CheckUpdate_InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                CheckUpdate_InfoBar.ActionButton = null;
                CheckUpdate_InfoBar.IsOpen = true;
            }
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = (HyperlinkButton)sender;
            if (hyperlinkButton.NavigateUri != null)
            {
                await Launcher.LaunchUriAsync(hyperlinkButton.NavigateUri);
            }
        }

        private void LicenseTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            LicenseTextBox.Text = "Apache License\r\nVersion 2.0, January 2004\r\nhttp://www.apache.org/licenses/\r\n\r\nTERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION\r\n\r\n1. Definitions.\r\n\r\n\"License\" shall mean the terms and conditions for use, reproduction, and distribution as defined by Sections 1 through 9 of this document.\r\n\r\n\"Licensor\" shall mean the copyright owner or entity authorized by the copyright owner that is granting the License.\r\n\r\n\"Legal Entity\" shall mean the union of the acting entity and all other entities that control, are controlled by, or are under common control with that entity. For the purposes of this definition, \"control\" means (i) the power, direct or indirect, to cause the direction or management of such entity, whether by contract or otherwise, or (ii) ownership of fifty percent (50%) or more of the outstanding shares, or (iii) beneficial ownership of such entity.\r\n\r\n\"You\" (or \"Your\") shall mean an individual or Legal Entity exercising permissions granted by this License.\r\n\r\n\"Source\" form shall mean the preferred form for making modifications, including but not limited to software source code, documentation source, and configuration files.\r\n\r\n\"Object\" form shall mean any form resulting from mechanical transformation or translation of a Source form, including but not limited to compiled object code, generated documentation, and conversions to other media types.\r\n\r\n\"Work\" shall mean the work of authorship, whether in Source or Object form, made available under the License, as indicated by a copyright notice that is included in or attached to the work (an example is provided in the Appendix below).\r\n\r\n\"Derivative Works\" shall mean any work, whether in Source or Object form, that is based on (or derived from) the Work and for which the editorial revisions, annotations, elaborations, or other modifications represent, as a whole, an original work of authorship. For the purposes of this License, Derivative Works shall not include works that remain separable from, or merely link (or bind by name) to the interfaces of, the Work and Derivative Works thereof.\r\n\r\n\"Contribution\" shall mean any work of authorship, including the original version of the Work and any modifications or additions to that Work or Derivative Works thereof, that is intentionally submitted to Licensor for inclusion in the Work by the copyright owner or by an individual or Legal Entity authorized to submit on behalf of the copyright owner. For the purposes of this definition, \"submitted\" means any form of electronic, verbal, or written communication sent to the Licensor or its representatives, including but not limited to communication on electronic mailing lists, source code control systems, and issue tracking systems that are managed by, or on behalf of, the Licensor for the purpose of discussing and improving the Work, but excluding communication that is conspicuously marked or otherwise designated in writing by the copyright owner as \"Not a Contribution.\"\r\n\r\n\"Contributor\" shall mean Licensor and any individual or Legal Entity on behalf of whom a Contribution has been received by Licensor and subsequently incorporated within the Work.\r\n\r\n2. Grant of Copyright License. Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable copyright license to reproduce, prepare Derivative Works of, publicly display, publicly perform, sublicense, and distribute the Work and such Derivative Works in Source or Object form.\r\n\r\n3. Grant of Patent License. Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable (except as stated in this section) patent license to make, have made, use, offer to sell, sell, import, and otherwise transfer the Work, where such license applies only to those patent claims licensable by such Contributor that are necessarily infringed by their Contribution(s) alone or by combination of their Contribution(s) with the Work to which such Contribution(s) was submitted. If You institute patent litigation against any entity (including a cross-claim or counterclaim in a lawsuit) alleging that the Work or a Contribution incorporated within the Work constitutes direct or contributory patent infringement, then any patent licenses granted to You under this License for that Work shall terminate as of the date such litigation is filed.\r\n\r\n4. Redistribution. You may reproduce and distribute copies of the Work or Derivative Works thereof in any medium, with or without modifications, and in Source or Object form, provided that You meet the following conditions:\r\n\r\nYou must give any other recipients of the Work or Derivative Works a copy of this License; and\r\nYou must cause any modified files to carry prominent notices stating that You changed the files; and\r\nYou must retain, in the Source form of any Derivative Works that You distribute, all copyright, patent, trademark, and attribution notices from the Source form of the Work, excluding those notices that do not pertain to any part of the Derivative Works; and\r\nIf the Work includes a \"NOTICE\" text file as part of its distribution, then any Derivative Works that You distribute must include a readable copy of the attribution notices contained within such NOTICE file, excluding those notices that do not pertain to any part of the Derivative Works, in at least one of the following places: within a NOTICE text file distributed as part of the Derivative Works; within the Source form or documentation, if provided along with the Derivative Works; or, within a display generated by the Derivative Works, if and wherever such third-party notices normally appear. The contents of the NOTICE file are for informational purposes only and do not modify the License. You may add Your own attribution notices within Derivative Works that You distribute, alongside or as an addendum to the NOTICE text from the Work, provided that such additional attribution notices cannot be construed as modifying the License.\r\nYou may add Your own copyright statement to Your modifications and may provide additional or different license terms and conditions for use, reproduction, or distribution of Your modifications, or for any such Derivative Works as a whole, provided Your use, reproduction, and distribution of the Work otherwise complies with the conditions stated in this License.\r\n\r\n5. Submission of Contributions. Unless You explicitly state otherwise, any Contribution intentionally submitted for inclusion in the Work by You to the Licensor shall be under the terms and conditions of this License, without any additional terms or conditions. Notwithstanding the above, nothing herein shall supersede or modify the terms of any separate license agreement you may have executed with Licensor regarding such Contributions.\r\n\r\n6. Trademarks. This License does not grant permission to use the trade names, trademarks, service marks, or product names of the Licensor, except as required for reasonable and customary use in describing the origin of the Work and reproducing the content of the NOTICE file.\r\n\r\n7. Disclaimer of Warranty. Unless required by applicable law or agreed to in writing, Licensor provides the Work (and each Contributor provides its Contributions) on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied, including, without limitation, any warranties or conditions of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A PARTICULAR PURPOSE. You are solely responsible for determining the appropriateness of using or redistributing the Work and assume any risks associated with Your exercise of permissions under this License.\r\n\r\n8. Limitation of Liability. In no event and under no legal theory, whether in tort (including negligence), contract, or otherwise, unless required by applicable law (such as deliberate and grossly negligent acts) or agreed to in writing, shall any Contributor be liable to You for damages, including any direct, indirect, special, incidental, or consequential damages of any character arising as a result of this License or out of the use or inability to use the Work (including but not limited to damages for loss of goodwill, work stoppage, computer failure or malfunction, or any and all other commercial damages or losses), even if such Contributor has been advised of the possibility of such damages.\r\n\r\n9. Accepting Warranty or Additional Liability. While redistributing the Work or Derivative Works thereof, You may choose to offer, and charge a fee for, acceptance of support, warranty, indemnity, or other liability obligations and/or rights consistent with this License. However, in accepting such obligations, You may act only on Your own behalf and on Your sole responsibility, not on behalf of any other Contributor, and only if You agree to indemnify, defend, and hold each Contributor harmless for any liability incurred by, or claims asserted against, such Contributor by reason of your accepting any such warranty or additional liability.\r\n\r\nEND OF TERMS AND CONDITIONS";
        }

        private async void OpenFileControls_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                items = items.OfType<StorageFile>()
                    .Where(s => s.FileType.Equals(".heic") || s.FileType.Equals(".HEIC")).ToList() as IReadOnlyList<IStorageItem>;
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        App.converter_window.AddFile(item as StorageFile);
                    }
                    App.TryToActivateConverterWindow();
                }
            }
        }

        private void OpenFileControls_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "添加文件";
        }

        public async Task<List<StorageFile>> OpenFilesAsync()
        {
            var openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".heic");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            return (await openPicker.PickMultipleFilesAsync()).ToList();
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            List<StorageFile>files = await OpenFilesAsync();
            if (files != null && files.Count != 0)
            {
                foreach (var item in files)
                {
                    App.converter_window.AddFile(item as StorageFile);
                }
                App.TryToActivateConverterWindow();
            }
        }
    }
}
