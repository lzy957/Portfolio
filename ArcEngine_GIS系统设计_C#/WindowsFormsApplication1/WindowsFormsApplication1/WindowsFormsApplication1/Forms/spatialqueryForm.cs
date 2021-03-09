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
    public partial class spatialqueryForm : Form
    { 
        public spatialqueryForm(ESRI.ArcGIS.Controls.AxMapControl mapControl)
        {
            InitializeComponent();
            this.m_mapControl = mapControl;

        }
        #region Class Numble
        //获取主窗体mapControl对象  
        private ESRI.ArcGIS.Controls.AxMapControl m_mapControl;
        //查询方式  
        public int mQueryModel;
        //图层索引  
        public int mLayerIndex;
        public bool issqform = false;

        #endregion

        private void spatialqueryForm_Load(object sender, EventArgs e)
        {
            //MapControl没有图层返回  
            if (m_mapControl.LayerCount <= 0)
                return;
            //获取MapControl中的全部图层名称，并加入ComboBox  
            for (int i = 0; i < m_mapControl.LayerCount; ++i)
            {
                cobLayer.Items.Add(m_mapControl.get_Layer(i).Name);
            }
            //加载查询方式  
            this.cobSearchStyle.Items.Add("矩形查询");
            this.cobSearchStyle.Items.Add("线查询");
            this.cobSearchStyle.Items.Add("点查询");
            this.cobSearchStyle.Items.Add("圆查询");
            //初始化ComboBox默认值  
            cobLayer.SelectedIndex = 0;
            cobSearchStyle.SelectedIndex = 0; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            //判断图层数量  
            if (this.cobLayer.Items.Count <= 0)
            {
                MessageBox.Show("当前MapControl没有添加图层！", "提示");
                return;
            }
            //获取选中的查询方式和图层索引  
            this.mLayerIndex = cobLayer.SelectedIndex;
            this.mQueryModel = cobSearchStyle.SelectedIndex;
            this.issqform = true;
        }



    }
}

