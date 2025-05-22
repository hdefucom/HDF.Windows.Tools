using HDF.Common;
using HDF.Common.Windows;
using HDF.Windows.Tools.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDF.Windows.Tools.Other
{
    public partial class TranslateForm : Form
    {
        public TranslateForm()
        {
            InitializeComponent();
            this.SetBorderShadows();

            this.SaveFormRectangle("location-翻译工具");


            taskbar = new TaskbarTranslateForm();
            if (File.Exists(Application.StartupPath + "\\enable-taskbar-tool"))
                taskbar.Show();

            //读取翻译API配置
            var path = Application.StartupPath + "\\config";
            if (File.Exists(path))
            {
                var config = File.ReadAllLines(path);
                if ((config?.Length ?? 0) < 2)
                {
                    txt_Key.Text = "程序配置错误";
                    taskbar.SetTextBox("程序配置错误");
                    return;
                }
                TranslateExtensions.BaiduAppId = config[0];
                TranslateExtensions.BaiduKey = config[1];
            }
            else
            {
                txt_Key.Text = "请先配置API接口信息";
                taskbar.SetTextBox("请先配置API接口信息");
                return;
            }



        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            taskbar?.Close();
        }

        private TaskbarTranslateForm taskbar;


        private int lastTickCount;//防止api调用过快，
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x031D)//WM_CLIPBOARDUPDATE
            {
                if (Environment.TickCount - this.lastTickCount >= 200)
                    OnClipboardUpdate();
                this.lastTickCount = Environment.TickCount;
                m.Result = IntPtr.Zero;
            }
            else if (m.Msg != 0xA3)
                base.WndProc(ref m);

            this.SetDragPosition(ref m);
        }

        public void OnClipboardUpdate()
        {
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                text = text.Trim();
                if (string.IsNullOrEmpty(text))
                    return;
                if (text.Length > 1000)
                    text = text.Substring(0, 1000);
                try
                {
                    new Task(() =>
                    {
                        var resstring = TranslateExtensions.BaiduTranslate(text);
                        var res = Newtonsoft.Json.JsonConvert.DeserializeObject<TranslateExtensions.BaiduTranslateApiResult>(resstring);


                        var str = "";

                        if (res == null)
                        {
                            text = "请求返回序列化：null";
                            str = "请求返回序列化：null";
                        }
                        else if (res.error_code != null)
                        {
                            text = "请求返回error_code：" + res.error_code;
                            str = "请求返回error_code：" + res.error_code;
                        }
                        else
                            str = string.Join(Environment.NewLine, res.trans_result.Select(r => r.dst));


                        this.Invoke(() =>
                        {
                            txt_Key.Text = text;
                            txt_Target.Text = str;
                            taskbar.SetTextBox(str);
                        });


                    }).Start();
                }
                catch (Exception ex)
                {
                    txt_Key.Text = "又出Bug了！~(￣▽￣)~*";
                    txt_Target.Text = ex.ToString();
                    taskbar.SetTextBox("又出Bug了！~(￣▽￣)~*");
                }
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (!cb_Fixed.Checked)
                this.Visible = false;
        }

        private void txt_Key_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var str = txt_Key.Text.Trim();
                if (string.IsNullOrEmpty(str))
                    return;

                Clipboard.SetText(str);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Visible = !this.Visible;
            }
        }




    }

}
