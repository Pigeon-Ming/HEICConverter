using HEICConverter.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HEICConverter
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        /// 

        static string filePath { get; set; }


        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // If this is the first instance launched, then register it as the "main" instance.
            // If this isn't the first instance launched, then "main" will already be registered,
            // so retrieve it.
            var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");
            string[] arguments = Environment.GetCommandLineArgs();
            // If the instance that's executing the OnLaunched handler right now
            // isn't the "main" instance.
            InitConverterWindow();
            if (!mainInstance.IsCurrent)
            {
                // Redirect the activation (and args) to the "main" instance, and exit.
                var activatedEventArgs =
                    Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
                //if (!String.IsNullOrEmpty(filePath))
                //    activatedEventArgs. = filePath;
                await mainInstance.RedirectActivationToAsync(activatedEventArgs);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }else
            {
                mainInstance.Activated += MainInstance_Activated;
            }


            

            
            if (arguments.Length > 1)
            {
                
                filePath = arguments[1];
                //converter_window.DebugText = filePath;
                // 处理文件转换逻辑
                //Converter.HEIC2jpg(filePath);
                Debug.WriteLine("添加文件：" + filePath);
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
                converter_window.AddFile(storageFile);
                TryToActivateConverterWindow();
            }
            else
            {
                m_window = new MainWindow();
                m_window.Activate();
            }
            
        }

        public static bool IsConverterWindowActivated { get; set; } = false;

        public void InitConverterWindow()
        {
            if (IsConverterWindowActivated == false)
            {
                converter_window = new ConverterWindow();
                converter_window.Activated += Converter_window_Activated;
                converter_window.Closed += Converter_window_Closed;
            }
        }

        private void Converter_window_Activated(object sender, WindowActivatedEventArgs args)
        {
            IsConverterWindowActivated = true;
        }

        private void Converter_window_Closed(object sender, WindowEventArgs args)
        {
            converter_window.Closed -= Converter_window_Closed;
            converter_window.Activated -= Converter_window_Activated;
            IsConverterWindowActivated = false;
            InitConverterWindow();
        }

        public static void TryToActivateConverterWindow()
        {
            if(!IsConverterWindowActivated)
                converter_window.Activate();
        }

        private void MainInstance_Activated(object sender, Microsoft.Windows.AppLifecycle.AppActivationArguments e)
        {
            
            List<IStorageItem> files =  (((Windows.ApplicationModel.Activation.FileActivatedEventArgs)e.Data).Files).ToList();
            Debug.WriteLine("激活主实例");
            if (files!=null && files.Count!=0)
            {
                //converter_window.DebugText = "Here";
                Debug.WriteLine("File个数:"+files.Count);

                foreach (var item in files)
                {
                    converter_window.AddFile((StorageFile)item);
                }
            }
        }

        private Window m_window;
        public static ConverterWindow converter_window { get; set; }
    }
}
