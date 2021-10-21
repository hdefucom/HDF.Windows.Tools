using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HDF.Windows.Tools.Common
{
    public static class SaveFormRectangleExtension
    {
        public static void SaveFormRectangle(this Form form, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("文件名无效", nameof(filename));

            //记录位置大小
            var file = Application.StartupPath + "\\" + filename;
            if (File.Exists(file))
            {
                var data = File.ReadAllText(file);
                var location = data.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                form.Location = new System.Drawing.Point(location[0], location[1]);
            }
            form.FormClosed += (sender, e) => File.WriteAllText(file, $"{form.Location.X},{form.Location.Y}");

        }
    }
}
