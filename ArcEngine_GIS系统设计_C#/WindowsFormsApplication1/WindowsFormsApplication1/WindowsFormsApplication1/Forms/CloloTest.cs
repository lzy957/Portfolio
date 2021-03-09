using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;

namespace WindowsFormsApplication1
{
    public partial class CloloTest : Form
    {
        public CloloTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int R1, R2, G1, G2, B1, B2;
            R1 = int.Parse(this.textR1.Text.ToString());
            R2 = int.Parse(this.textR2.Text.ToString());
            B1 = int.Parse(this.textB1.Text.ToString());
            B2 = int.Parse(this.textB2.Text.ToString());
            G1 = int.Parse(this.textG1.Text.ToString());
            G2 = int.Parse(this.textG2.Text.ToString());
            //创建一个新AlgorithmicColorRampClass对象
            IAlgorithmicColorRamp algColorRamp = new AlgorithmicColorRampClass();
            //创建起始颜色对象
            IRgbColor startColor = new RgbColor();
            startColor.Red = R1;
            startColor.Green = G1;
            startColor.Blue = B1;
            //创建终止颜色对象
            IRgbColor endColor = new RgbColor();
            endColor.Red = R2;
            endColor.Green = G2;
            endColor.Blue = B2;
            //设置AlgorithmicColorRampClass的起止颜色属性
            algColorRamp.ToColor = startColor;
            algColorRamp.FromColor = endColor;
            //设置梯度类型
            algColorRamp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm;
            //设置颜色带颜色数量
            algColorRamp.Size = 5;
            //创建颜色带
            bool bture = true;
            algColorRamp.CreateRamp(out bture);
            //使用IEnumColors获取颜色带
            IEnumColors pEnumColors = null;
            pEnumColors = algColorRamp.Colors;

            IRgbColor tempcolor;
            //设置５个picturebox的背景色为产生颜色带的５个颜色
            this.pictureBox1.BackColor =
            ColorTranslator.FromOle(pEnumColors.Next().RGB);
            tempcolor = new RgbColor();
            tempcolor.RGB = algColorRamp.get_Color(0).RGB;
            this.label1.Text = "R:"+tempcolor.Red.ToString() +" G:"+ tempcolor.Green.ToString() +" B:"+ tempcolor.Blue.ToString();
            //this.label1.Text = algColorRamp.get_Color(0).RGB.ToString();
            this.pictureBox2.BackColor =
            ColorTranslator.FromOle(pEnumColors.Next().RGB);
            tempcolor.RGB = algColorRamp.get_Color(1).RGB;
            this.label2.Text = "R:" + tempcolor.Red.ToString() + " G:" + tempcolor.Green.ToString() + " B:" + tempcolor.Blue.ToString();
            //this.label2.Text = algColorRamp.get_Color(1).RGB.ToString();
            this.pictureBox3.BackColor =
            ColorTranslator.FromOle(pEnumColors.Next().RGB);
            tempcolor.RGB = algColorRamp.get_Color(2).RGB;
            this.label3.Text = "R:" + tempcolor.Red.ToString() + " G:" + tempcolor.Green.ToString() + " B:" + tempcolor.Blue.ToString();
            //this.label3.Text = algColorRamp.get_Color(2).RGB.ToString();
            this.pictureBox4.BackColor =
            ColorTranslator.FromOle(pEnumColors.Next().RGB);
            tempcolor.RGB = algColorRamp.get_Color(3).RGB;
            this.label4.Text = "R:" + tempcolor.Red.ToString() + " G:" + tempcolor.Green.ToString() + " B:" + tempcolor.Blue.ToString();
            //this.label4.Text = algColorRamp.get_Color(3).RGB.ToString();
            this.pictureBox5.BackColor =
            ColorTranslator.FromOle(pEnumColors.Next().RGB);
            tempcolor.RGB = algColorRamp.get_Color(4).RGB;
            this.label5.Text = "R:" + tempcolor.Red.ToString() + " G:" + tempcolor.Green.ToString() + " B:" + tempcolor.Blue.ToString();
            //this.label5.Text = algColorRamp.get_Color(4).RGB.ToString();


        }

        private void CloloTest_Load(object sender, EventArgs e)
        {

        }

    }
}
