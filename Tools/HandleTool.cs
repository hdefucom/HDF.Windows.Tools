using HDF.Windows.Tools.Common;
using HDF.Windows.Tools.Other;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HDF.Windows.Tools
{

    public class HandleTool : IHDFTool
    {
        public string ID => "2574E215-D8A8-4451-8675-89B16EFFC9E7";
        public string Name => "句柄工具";

        public bool EnableHotKey => true;

        private readonly HotKeys[] hotkeys = new HotKeys[] {
            new HotKeys(Keys.J, ModifierKeys.Control),
        };
        public HotKeys[] HotKeys => hotkeys;

        private HandleInfoForm form;

        public void Run()
        {
            form ??= new HandleInfoForm();
            form.Visible = true;
            form.Activate();
        }
        public void Load()
        {
            form ??= new HandleInfoForm();
        }

        public void Unload()
        {
            form.Close();
            form.Dispose();
        }

        public bool HandlerHotKey(HotKeys hotkey) => hotkey switch
        {
            (Keys.J, ModifierKeys.Control) => ShowOrHide(),
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




    }


}
