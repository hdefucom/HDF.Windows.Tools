using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HDF.Windows.Tools
{
    public partial class TaskbarTranslateForm : Form
    {
        public TaskbarTranslateForm()
        {
            InitializeComponent();

            height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            x = Screen.PrimaryScreen.Bounds.Width - this.Width - 1020;


            var file = Application.StartupPath + "\\location-翻译工具-任务栏";
            if (File.Exists(file))
            {
                var data = File.ReadAllText(file);
                x = Convert.ToInt32(data);
            }
        }



        int x;
        int height;



        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public extern static IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);





        public void SetTextBox(string msg)
        {
            try
            {
                textBox1.Text = msg;
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var parent = FindWindow("Shell_TrayWnd", null);

            SetParent(this.Handle, parent);

            SetWindowPos(this.Handle, new IntPtr(0), x, 0,
                this.Width, height, 0x2000);


        }




    }
}
