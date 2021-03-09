using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1.Forms
{
    public partial class FrmMeasureResult : DevComponents.DotNetBar.Office2007Form
    {
        //声明运行结果关闭事件
        public delegate void FrmClosedEventHandler();
        public event FrmClosedEventHandler frmClosed = null;

        public FrmMeasureResult()
        {
            InitializeComponent();
        }

        //窗口关闭时引发委托事件
        private void FrmMeasureResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (frmClosed != null)
            {
                frmClosed();
            }
        }
    }
}
