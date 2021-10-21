using HDF.Windows.Tools.Common;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HDF.Windows.Tools.Other
{
    public partial class HandleInfoForm : Form
    {

        private bool isMouseUp = false;

        private int hwnd;

        private StringBuilder name = new StringBuilder(256);


        public HandleInfoForm()
        {
            InitializeComponent();

            this.SaveFormRectangle("location-句柄工具");
        }

        private void btn_Get_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseUp = true;
            Cursor = Cursors.Cross;
        }

        private void btn_Get_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseUp = false;
            Cursor = Cursors.Default;
        }

        private void btn_Get_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseUp)
                return;

            var p = btn_Get.PointToScreen(e.Location);
            //GetCursorPos(ref p);
            label1.Text = "坐标：X=" + p.X + "  |  Y=" + p.Y;

            //根据坐标获取句柄
            if (IntPtr.Size == 4)
                hwnd = WindowFromPoint(p.X, p.Y);
            else if (IntPtr.Size == 8)
                hwnd = WindowFromPoint(p);
            label2.Text = "句柄：" + hwnd;

            //获取标题名
            GetWindowText(hwnd, name, 256);
            label3.Text = "标题：" + name.ToString();

            //获取类名
            GetClassName(hwnd, name, 256);
            label4.Text = "名称：" + name.ToString();
        }

        private void btn_UpdataTitle_Click(object sender, EventArgs e)
        {
            SendMessage(hwnd, 12, 0, textBox1.Text);
        }





        #region

        /// <summary>
        /// X86使用
        /// </summary>
        /// <param name="xPoint"></param>
        /// <param name="yPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int WindowFromPoint(int xPoint, int yPoint);
        /// <summary>
        /// X64使用
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hwnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassName(int hwnd, StringBuilder lpstring, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, string lParam);
        #endregion

    }
}
