using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;
using Microsoft.UI;
using Windows.Storage;
using System.Threading.Tasks;
using WinUIEx;
using Windows.UI.Shell;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using System.Diagnostics;
using Windows.Storage.Pickers;
using System.Runtime.InteropServices;
using Windows.UI.Accessibility;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HEICConverter.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConverterWindow : Window
    {
        Microsoft.UI.Windowing.AppWindow m_appWindow;

        private List<StorageFile> Files { get; set; } = new List<StorageFile>();


        bool IsConverting { get; set; } = false;

        DispatcherTimer UpdateViewTimer { get; set; } = new DispatcherTimer();
        DispatcherTimer LoadTimer { get; set; } = new DispatcherTimer();

        public void AddFile(StorageFile storageFile)
        {
            if(!IsConverting)
                Files.Add(storageFile);
        }
        
        public ConverterWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;

            



            UpdateViewTimer.Interval = TimeSpan.FromSeconds(1);
            UpdateViewTimer.Tick += UpdateViewTimer_Tick;

            Activated += Window_Activated;
            LoadTimer.Interval = TimeSpan.FromSeconds(1);
            LoadTimer.Tick += LoadTimer_Tick;
            LoadTimer.Start();

            
        }

        private void LoadTimer_Tick(object sender, object e)
        {
            if(Files.Count !=0 && isActivated == false)
            {
                this.Activate();
            }
        }

        public bool isActivated { get; set; } = false;

        int itemsCount { get; set; } = -1;

        private void UpdateViewTimer_Tick(object sender, object e)
        {
            UpdateView();
        }

        void UpdateView()
        {
            if (Files.Count == 0)
            {
                EmptyTipStackPanel.Visibility = Visibility.Visible;
                ConvertButton.IsEnabled = false;
            }
            else
            {
                EmptyTipStackPanel.Visibility = Visibility.Collapsed;
                ConvertButton.IsEnabled = true;
            }
            if (itemsCount != Files.Count)
            {


                ProgressText.Text = "等待转换……";


                Files = Files.GroupBy(file => file.Path, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();
                itemsCount = Files.Count;

                RefreshListView();
                FileCount.Text = $"已选择{Files.Count}个文件";
            }
        }

        public void RefreshListView()
        {
            //FilePathListView.Items.Clear();
            //foreach (var item in FilePaths)
            //{
            //    FilePathListView.Items.Add(item);
            //}
            FilePathListView.ItemsSource = null;
            FilePathListView.ItemsSource = Files;
            //Bindings.Update();
        }

        void SetWindowSettings()
        {
            m_appWindow = GetAppWindowForCurrentWindow();
            if (localSettings.Values["ConverterWindow_Height"] == null || localSettings.Values["ConverterWindow_Width"] == null)
                m_appWindow.Resize(new SizeInt32(707, 390));
            else
            {
                m_appWindow.Resize(new SizeInt32(Convert.ToInt32(localSettings.Values["ConverterWindow_Width"]), Convert.ToInt32(localSettings.Values["ConverterWindow_Height"])));
            }
            SizeChanged += ConverterWindow_SizeChanged;

            WindowManager.Get(this).IsMinimizable = false;
            WindowManager.Get(this).IsMaximizable = false;
            WindowManager.Get(this).IsResizable = true;
            WindowManager.Get(this).IsAlwaysOnTop = true;
        }

        private void ConverterWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            localSettings.Values["ConverterWindow_Height"] = m_appWindow.Size.Height;
            localSettings.Values["ConverterWindow_Width"] = m_appWindow.Size.Width;
        }

        private Microsoft.UI.Windowing.AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

            return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            CancelFlag = false;
            StartConvertAsync();
        }

        bool CancelFlag = false;

        private void CancelConvertButton_Click(object sender, RoutedEventArgs e)
        {
            CancelConvertButton.Content = "正在停止转换……";
            CancelFlag = true;
        }

        public async void StartConvertAsync()
        {
            ConvertButton.Visibility = Visibility.Collapsed;
            CancelConvertButton.Visibility = Visibility.Visible;
            IsConverting = true;
            ConvertProgressBar.Visibility = Visibility.Visible;
            ConvertModeRadioButtons.IsEnabled = false;
            DeleteHEICFileAfterConvert_CheckBox.IsEnabled = false;
            ConvertButton.IsEnabled = false;
            FilePathListView.IsEnabled = false;
            bool DeleteHEICFileAfterConvert = (bool)DeleteHEICFileAfterConvert_CheckBox.IsChecked;
            int fileIndex = 0;
            ConvertProgressBar.Maximum = Files.Count;
            UpdateViewTimer.Stop();
            ConvertingProgressRing.IsActive = true;
            switch (ConvertModeRadioButtons.SelectedIndex)
            {
                case 0://JPG
                    
                    foreach (var item in Files)
                    {
                        ProgressText.Text = $"({++fileIndex}/{Files.Count})正在转换文件{item.Name}";
                        ConvertProgressBar.Value = fileIndex;
                        if (CancelFlag)
                            break;
                        await Task.Run(async () =>
                        {
                            await Converter.HEIC2jpg(item.Path);
                        });
                    }
                    break;
                case 1://PNG
                    foreach (var item in Files)
                    {
                        ConvertProgressBar.Value = fileIndex;
                        ProgressText.Text = $"({++fileIndex}/{Files.Count})正在转换文件{item.Name}";
                        if (CancelFlag)
                            break;
                        await Task.Run(async () =>
                        {
                            await Converter.HEIC2png(item.Path);
                        });
                    }
                    break;
            }
            fileIndex = 0;
            if (DeleteHEICFileAfterConvert)
            {
                int deleteFailedFiles = 0;
                foreach (var item in Files)
                {
                    ConvertProgressBar.Value = fileIndex;
                    ProgressText.Text = $"({++fileIndex}/{Files.Count})正在删除文件{item.Name}";
                    if (CancelFlag)
                        break;
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await item.DeleteAsync();
                        }
                        catch
                        {
                            deleteFailedFiles++;
                        }
                    });
                }
                if (CancelFlag)
                    ProgressText.Text = "转换已取消。";
                else if (deleteFailedFiles!=0)
                    ProgressText.Text = $"转换完成，{deleteFailedFiles}个文件删除失败。";
                else
                    ProgressText.Text = "转换完成";
            }
            else
            {
                if (CancelFlag)
                    ProgressText.Text = "转换已取消。";
                else
                    ProgressText.Text = "转换完成";
            }
            Files.Clear();
            itemsCount = 0;
            RefreshListView();
            ConvertProgressBar.Visibility = Visibility.Collapsed;
            ConvertModeRadioButtons.IsEnabled = true;
            DeleteHEICFileAfterConvert_CheckBox.IsEnabled = true;
            ConvertButton.IsEnabled = true;
            FilePathListView.IsEnabled = true;
            ConvertingProgressRing.IsActive = false;
            IsConverting = false;
            CancelFlag = false;
            ConvertButton.Visibility = Visibility.Visible;
            CancelConvertButton.Visibility = Visibility.Collapsed;
            FileCount.Text = "已选择0个文件";
            UpdateView();
            UpdateViewTimer.Start();
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            LoadTimer.Stop();
            UpdateViewTimer.Start();
            InitSettings();
            UpdateView();
            isActivated = true;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            UpdateViewTimer.Stop();
            Activated -= Window_Activated;
            isActivated = false;
            if (MainWindow.MainWindowRunning == false)
                Environment.Exit(0);
        }

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public void InitSettings()
        {
           

            // 存储设置
            if (localSettings.Values["ConvertSetting_DeleteAfterConvert"]==null)
                localSettings.Values["ConvertSetting_DeleteAfterConvert"] = false;
            if(localSettings.Values["ConvertSetting_ConvertTo"] == null)
                localSettings.Values["ConvertSetting_ConvertTo"] = 0;

            if ((bool)localSettings.Values["ConvertSetting_DeleteAfterConvert"] == true)
                DeleteHEICFileAfterConvert_CheckBox.IsChecked = true;
            if(localSettings.Values["ConvertSetting_ConvertTo"]!=null)
            {
                ConvertModeRadioButtons.SelectedIndex = (int)localSettings.Values["ConvertSetting_ConvertTo"];
            }


        }

        private void ConvertModeRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            localSettings.Values["ConvertSetting_ConvertTo"] = ConvertModeRadioButtons.SelectedIndex;
        }

        private void DeleteHEICFileAfterConvert_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["ConvertSetting_DeleteAfterConvert"] = true;
        }

        private void DeleteHEICFileAfterConvert_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            localSettings.Values["ConvertSetting_DeleteAfterConvert"] = false;
        }

        private void FileDropBorder_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "添加文件";
        }

        private async void FileDropBorder_Drop(object sender, DragEventArgs e)
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
                        AddFile(item as StorageFile);
                    }
                }
            }
        }

        StorageFile MenuStorageFile { get; set; }

        private void ConvertQueueMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MenuStorageFile == null)
                return;
            Files.Remove(MenuStorageFile);
        }

        private async void ConvertQueueMenu_OpenFilePlace_Click(object sender, RoutedEventArgs e)
        {
            if (MenuStorageFile == null)
                return;
            await Launcher.LaunchFolderAsync(await MenuStorageFile.GetParentAsync());
        }

        private void FilePathListView_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (e.OriginalSource != null)
                MenuStorageFile = (StorageFile)(e.OriginalSource as FrameworkElement).DataContext;
            else
                MenuStorageFile = null;
        }

        private void FilePathListView_MenuFlyout_Opened(object sender, object e)
        {
            if (MenuStorageFile == null)
            {
                ConvertQueueMenu_OpenFilePlace.Visibility = Visibility.Collapsed;
                ConvertQueueMenu_Delete.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConvertQueueMenu_OpenFilePlace.Visibility = Visibility.Visible;
                ConvertQueueMenu_Delete.Visibility = Visibility.Visible;
            }
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

        private async void Menu_OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            List<StorageFile>file = await OpenFilesAsync();
            if (file != null)
                Files = Files.Concat(file).ToList();
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowSettings();
        }



        //private void RefreshButton_Click(object sender, RoutedEventArgs e)
        //{
        //    RefreshListView();
        //    Debug.WriteLine("共有："+Files.Count+"个文件");
        //    foreach(var item in Files)
        //    {
        //        Debug.WriteLine(item);
        //    }
        //}
    }
}
