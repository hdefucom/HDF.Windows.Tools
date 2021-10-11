using System.Windows.Forms;

namespace HDF.Windows.Tools.Common
{
    /// <summary>
    /// 修饰键
    /// </summary>
    public enum ModifierKeys : uint
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }


    /// <summary>
    /// 全局热键结构
    /// </summary>
    public struct HotKeys
    {
        /// <summary>
        /// 常规键
        /// </summary>
        public Keys Key { get; set; }
        /// <summary>
        /// 修饰键
        /// </summary>
        public ModifierKeys Modifier { get; set; }
        public HotKeys(Keys key, ModifierKeys m)
        {
            Key = key;
            Modifier = m;
        }

        public void Deconstruct(out Keys Key, out ModifierKeys Modifier)
        {
            Key = this.Key;
            Modifier = this.Modifier;
        }
    }

}
