#### HDF.Windows.Tools

一个简单的Windows工具集合框架，第三方可实现`IHDFTool`接口进行自定义工具开发，生成的dll放在exe同级的tools文件夹下可自动反射加载。

##### 目前内置工具：
> 1. 翻译工具（百度翻译），需自己申请翻译api账户，配置到编译后的config文件即可。  
> 2. 鼠标获取Windows窗口信息（包括Winform控件）

框架主要提供了全局热键及开机自启的快捷使用。