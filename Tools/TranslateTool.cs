using HDF.Windows.Tools.Common;
using HDF.Windows.Tools.Other;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HDF.Windows.Tools
{

    public class TranslateTool : IHDFTool
    {
        public string ID => "8DB07AEA-4BF7-4583-BDD3-0782D67C3922";
        public string Name => "翻译工具";

        public bool EnableHotKey => true;

        private readonly HotKeys[] hotkeys = new HotKeys[] {
            new HotKeys(Keys.H, ModifierKeys.Control),
            new HotKeys(Keys.Q, ModifierKeys.Control),
            new HotKeys(Keys.B, ModifierKeys.Control),
            new HotKeys(Keys.G, ModifierKeys.Control),
        };
        public HotKeys[] HotKeys => hotkeys;

        private TranslateForm form;
        private IntPtr handle;

        public void Run()
        {
            form.Visible = true;
            form.Activate();
        }
        public void Load()
        {
            form ??= new TranslateForm();
            handle = form.Handle;
            var res = AddClipboardFormatListener(handle);
            Log.Write($"加载【{this.Name}】--> 添加剪切板监听：{res}");
        }

        public void Unload()
        {
            var res = RemoveClipboardFormatListener(handle);
            Log.Write($"卸载【{this.Name}】--> 添加剪切板监听：{res}");
        }

        public bool HandlerHotKey(HotKeys hotkey) => hotkey switch
        {
            (Keys.H, ModifierKeys.Control) => ShowOrHide(),
            (Keys.Q, ModifierKeys.Control) => Input(),
            (Keys.B, ModifierKeys.Control) => Search(true),
            (Keys.G, ModifierKeys.Control) => Search(false),
            _ => false,
        };

        public void SetContextMenu(ContextMenu menu)
        {
            menu.MenuItems.Add(this.Name, (sender, e) =>
            {
                this.Run();
            });
        }

        private bool ShowOrHide()
        {
            form.Visible = !form.Visible;
            if (form.Visible)
                form.Activate();
            return true;
        }

        private bool Input()
        {
            this.Run();
            form.txt_Key.Clear();
            form.txt_Key.Focus();
            return true;
        }

        private bool Search(bool type)
        {
            if (!Clipboard.ContainsText())
                return false;

            var text = Clipboard.GetText();
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return false;

            if (type)
                Process.Start("https://www.baidu.com/s?ie=UTF-8&wd=" + text);
            else
                Process.Start("https://google.com/search?q=" + text);

            return true;
        }



        /// <summary>
        /// 添加剪切板监听
        /// </summary>
        /// <param name="hwnd">关联的</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// 移除剪切板监听
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
    }


}
