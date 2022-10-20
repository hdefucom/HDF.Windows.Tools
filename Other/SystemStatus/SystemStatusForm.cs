using HDF.Common.Windows;
using HDF.Windows.Tools.Common;
using System.Windows.Forms;

namespace HDF.Windows.Tools.Other.SystemStatus;

public partial class SystemStatusForm : Form
{
    public SystemStatusForm()
    {
        InitializeComponent();
        this.SetBorderShadows();

        this.SaveFormRectangle("location-系统状态监测工具");
    }








    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);





    }











}
