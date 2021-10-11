using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HDF.Windows.Tools.Common
{
    public static class ToolList
    {

        static readonly Dictionary<IHDFTool, uint?> Dict = new Dictionary<IHDFTool, uint?>();

        /// <summary>
        /// 注册工具，在Load之前调用
        /// </summary>
        /// <param name="tool"></param>
        public static void Register(IHDFTool tool)
        {
            if (Dict.Keys.Any(t => t.ID == tool.ID))
                throw new ArgumentException($"已经存在Name为\"{tool.ID}\"的工具了", nameof(tool));

            uint? id = tool.EnableHotKey ? GlobalAddAtom(tool.ID) : null;
            Dict.Add(tool, id);
        }

        /// <summary>
        /// 程序初始化时调用，进行初始化工具操作，例如注册全局热键
        /// </summary>
        public static void Load()
        {
            foreach (var item in Dict)
            {
                var tool = item.Key;
                Log.Write($"加载【{tool.Name}】--> 开始");
                tool.Load();
                if (!tool.EnableHotKey)
                {
                    Log.Write($"加载【{tool.Name}】--> 结束");
                    continue;
                }

                var id = item.Value;
                foreach (var hot in tool.HotKeys)
                {
                    var res = RegisterHotKey(IntPtr.Zero, (int)id, hot.Modifier, hot.Key);
                    Log.Write($"注册【{tool.Name}】热键【{hot.Modifier}+{hot.Key}：{res}】");
                }
                Log.Write($"加载【{tool.Name}】--> 结束");
            }
        }

        /// <summary>
        /// 程序结束退出时调用，进行卸载工具操作，例如注销全局热键
        /// </summary>
        public static void Unload()
        {
            foreach (var item in Dict)
            {
                var tool = item.Key;
                Log.Write($"卸载【{tool.Name}】--> 开始");
                tool.Unload();
                if (!tool.EnableHotKey)
                {
                    Log.Write($"卸载【{tool.Name}】--> 结束");
                    continue;
                }

                var id = item.Value;
                foreach (var hot in tool.HotKeys)
                {
                    var res = UnregisterHotKey(IntPtr.Zero, (int)id);
                    Log.Write($"注销【{tool.Name}】热键【{hot.Modifier}+{hot.Key}：{res}】");
                }
                GlobalDeleteAtom((int)id);
                Log.Write($"卸载【{tool.Name}】--> 结束");
            }
        }


        public static void SetContextMenu(ContextMenu menu)
        {
            foreach (var tool in Dict.Keys)
            {
                tool.SetContextMenu(menu);
            }
        }

        public static bool HandleHotKeys(HotKeys hotkey)
        {
            var res = false;
            foreach (var tool in Dict.Keys)
            {
                if (!tool.EnableHotKey)
                    continue;
                if (tool.HotKeys.Contains(hotkey))
                    res |= tool.HandlerHotKey(hotkey);
            }
            return res;
        }


        /// <summary>
        /// 将字符添加至全局原子表
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns>返回唯一标识，如果失败则返回0</returns>
        /// <remarks>
        /// <para>如果字符串中已经存在于全局原子表中，则返回现有的字符串的原子，并且原子的引用计数加1。</para>
        /// <para>与原子相关的字符串不会从内存中删除，直到它的引用计数为零。</para>
        /// <para>全局原子不会在应用程序终止时自动删除。每次调用GlobalAddAtom函数，必须相应的调用GlobalDeleteAtom函数删除原子。</para>
        /// </remarks>
        [DllImport("kernel32.dll")]
        private static extern uint GlobalAddAtom(string lpString);

        /// <summary>
        /// 将原子从全局原子表删除
        /// </summary>
        /// <param name="atom">已经存在原子表的原子</param>
        /// <returns>如果原子表存在该原子则引用计数减1，直到引用为0则会删除该原子，执行成功或者删除的是整数原子返回0，失败则使用GetLastError获取错误信息</returns>
        [DllImport("kernel32.dll")]
        private static extern int GlobalDeleteAtom(int atom);

        /// <summary>
        /// 注册全局热键
        /// </summary>
        /// <param name="handle">关联的窗口句柄，如果为null或zero则绑定到调用线程上</param>
        /// <param name="id"></param>
        /// <param name="modifiers"></param>
        /// <param name="virtualCode"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr handle, int id, ModifierKeys modifiers, Keys virtualCode);

        /// <summary>
        /// 注销全局热键
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr handle, int id);



    }
}
