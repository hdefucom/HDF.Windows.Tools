using HDF.Windows.Tools.Common;
using HDF.Windows.Tools.Other.SystemStatus;
using System.Windows.Forms;

namespace HDF.Windows.Tools.Tools
{
    internal class SystemSatusTool : IHDFTool
    {
        public string ID => "E1463C19-960A-4C7A-B29A-11ECA022B19A";

        public string Name => "系统状态监测工具";


        public bool EnableHotKey => false;//暂未完成，暂时禁用

        private readonly HotKeys[] hotkeys = new HotKeys[] {
            new HotKeys(Keys.H, ModifierKeys.Control| ModifierKeys.Shift),
        };
        public HotKeys[] HotKeys => hotkeys;

        private SystemStatusForm form;

        public void Run()
        {
            form ??= new SystemStatusForm();
            if (form.Disposing || form.IsDisposed)
                form = new SystemStatusForm();
            form.Visible = true;
            //form.Activate();
        }
        public void Load()
        {
            //form ??= new HandleInfoForm();
        }

        public void Unload()
        {
            form?.Close();
            form?.Dispose();
        }

        public bool HandlerHotKey(HotKeys hotkey) => hotkey switch
        {
            (Keys.H, ModifierKeys.Control | ModifierKeys.Shift) => ShowOrHide(),
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
            form ??= new SystemStatusForm();
            if (form.Disposing || form.IsDisposed)
                form = new SystemStatusForm();

            form.Visible = !form.Visible;
            if (form.Visible)
                form.Activate();
            return true;
        }

    }
}
