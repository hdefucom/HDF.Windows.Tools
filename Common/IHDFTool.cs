using System.Windows.Forms;

namespace HDF.Windows.Tools.Common
{
    /// <summary>
    /// 工具接口
    /// </summary>
    public interface IHDFTool
    {
        /// <summary>
        /// 工具标识，应该具有唯一性，不可重复
        /// </summary>
        string ID { get; }

        /// <summary>
        /// 工具名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 工具运行
        /// </summary>
        void Run();


        void Load();
        void Unload();


        /// <summary>
        /// 是否启用全局热键
        /// </summary>
        bool EnableHotKey { get; }

        /// <summary>
        /// 全局热键
        /// </summary>
        HotKeys[] HotKeys { get; }

        /// <summary>
        /// 处理热键操作
        /// </summary>
        bool HandlerHotKey(HotKeys hotkey);


        void SetContextMenu(ContextMenu menu);


    }
}
