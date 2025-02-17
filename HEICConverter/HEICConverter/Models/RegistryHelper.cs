using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace HEICConverter.Models
{
    //public class RegistryHelper
    //{
    //    public static void RegisterContextMenu(string fileExtension, string menuName, string menuText,string appPath)
    //    {
    //        // 创建文件类型的注册表项
    //        ProcessStartInfo procInfo = new ProcessStartInfo();
    //        procInfo.UseShellExecute = true;
    //        procInfo.FileName = Process.GetCurrentProcess().MainModule.FileName; // 当前应用程序路径
    //        procInfo.Verb = "runas"; // 请求管理员权限

    //        try
    //        {
    //            // 启动新进程
    //            Process.Start(procInfo);
    //            using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{fileExtension}\\Shell\\{menuName}"))
    //            {
    //                key.SetValue("", menuText);
    //                Debug.WriteLine(key.ToString());
    //            }

    //            // 创建菜单项命令
    //            using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{fileExtension}\\Shell\\{menuName}\\Command"))
    //            {
    //                key.SetValue("", $"\"{appPath}\" \"%1\"");
    //                Debug.WriteLine(key.ToString());
    //            }
    //        }
    //        catch (System.ComponentModel.Win32Exception)
    //        {
    //            // 用户可能取消了 UAC 提示
    //            Console.WriteLine("用户取消了 UAC 提示。");
    //            return;
    //        }

            

    //        //// 设置默认关联（可选）
    //        //using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{fileExtension}"))
    //        //{
    //        //    key.SetValue("", "YourApp.txt");
    //        //}

    //        //// 创建文件类型描述（可选）
    //        //using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Classes\\YourApp.txt"))
    //        //{
    //        //    key.SetValue("", "Text Document for YourApp");
    //        //}
    //    }

    //    public static void UnregisterContextMenu(string fileExtension, string commandName)
    //    {
    //        // 删除文件类型的注册表项
    //        string keyPath = $"SystemFileAssociations\\{fileExtension}\\Shell\\{commandName}";
    //        Debug.WriteLine("删除注册表：" + keyPath);
    //        Registry.CurrentUser.DeleteSubKeyTree(keyPath, false);
    //    }
    //}
}
