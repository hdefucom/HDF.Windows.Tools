using System;
using System.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using HDF.Windows.Tools.Common;
using System.IO;
using Microsoft.Win32;

namespace HDF.Windows.Tools
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new Mutex(true, "HDF.Windows.Tools", out bool setup);
            if (!setup)
                Environment.Exit(1);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //注册其他工具
            if (Directory.Exists(Application.StartupPath + "\\tools")
                && Directory.GetFiles(Application.StartupPath + "\\tools", "*.dll", SearchOption.TopDirectoryOnly) is var files
                && files.Length > 0)
            {
                foreach (var item in files)
                {
                    var assembly = Assembly.Load(item);
                    foreach (var type in assembly.GetTypes().Where(t => t.IsClass && typeof(IHDFTool).IsAssignableFrom(t)))
                    {
                        var tool = Activator.CreateInstance(type) as IHDFTool;
                        ToolList.Register(tool);
                    }
                }
            }
            //注册当前程序集工具
            foreach (var type in typeof(Program).Assembly.GetTypes().Where(t => t.IsClass && typeof(IHDFTool).IsAssignableFrom(t)))
            {
                var tool = Activator.CreateInstance(type) as IHDFTool;
                ToolList.Register(tool);
            }

            ToolList.Load();
            Application.ApplicationExit += (sender, e) => ToolList.Unload();
            Application.AddMessageFilter(new HotKeyMessageFilter());
            Application.Run(new HDFToolsApplicationContext());
        }

        private class HotKeyMessageFilter : IMessageFilter
        {
            const int WM_HOTKEY = 0x0312; //如果m.Msg的值为0x0312那么表示用户按下了热键
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg != WM_HOTKEY)
                    return false;

                var key = (Keys)((m.LParam.ToInt32() >> 16) & 0xFFFF);
                var flag = (ModifierKeys)(m.LParam.ToInt32() & 0xFFFF);
                return ToolList.HandleHotKeys(new HotKeys(key, flag));
            }
        }


        private class HDFToolsApplicationContext : ApplicationContext
        {
            private NotifyIcon notifyIcon;

            public HDFToolsApplicationContext()
            {
                notifyIcon = new NotifyIcon();
                notifyIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("HDF.Windows.Tools.hdf.ico"));
                notifyIcon.ContextMenu = new ContextMenu();
                ToolList.SetContextMenu(notifyIcon.ContextMenu);

                notifyIcon.ContextMenu.MenuItems.Add("-");//添加分隔线，不同于ContextMenuStrip有显式的分隔线控件对象，ContextMenu只能设置文本为‘-’来添加分隔线。
                //读取开机自启配置
                using RegistryKey run = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                var auto = run.GetValue("HDF.Windows.Tools") != null;
                run.Close();

                //添加开机自启菜单
                var item = new MenuItem("开机自启", (sender, e) =>
                {
                    if (sender is not MenuItem m)
                        return;
                    using RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    if (m.Checked)
                    {
                        rk.DeleteValue("HDF.Windows.Tools");
                        rk.Close();
                    }
                    else
                    {
                        rk.SetValue("HDF.Windows.Tools", Application.ExecutablePath);
                        rk.Close();
                    }
                    m.Checked = !m.Checked;
                })
                { Checked = auto };
                notifyIcon.ContextMenu.MenuItems.Add(item);
                //添加退出菜单
                notifyIcon.ContextMenu.MenuItems.Add("退出", (sender, e) =>
                {
                    notifyIcon.Dispose();
                    Application.Exit();
                });
                notifyIcon.Visible = true;
            }
        }























    }
}
