using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApplication1.Classes;

namespace WindowsFormsApplication1.Forms
{
    public partial class frmSymbol : Form
    {
        public ISymbol pSelSymbol;
        private IStyleGalleryItem pStyleGalleryItem;
        private ISymbologyStyleClass pSymStyleClass;

        public delegate void GetSelSymbolItemEventHandler(ref IStyleGalleryItem pStyleItem);
        public event GetSelSymbolItemEventHandler GetSelSymbolItem = null;

        string filepath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;


        private EnumMapSurroundType _enumMapSurType = EnumMapSurroundType.None;
        public EnumMapSurroundType EnumMapSurType
        {
            get { return _enumMapSurType; }
            set { _enumMapSurType = value; }
        }
        public frmSymbol()
        {
            InitializeComponent();
        }
        public void InitUI()
        {
            axSymbologyControl1.Clear();
            //string StyleFilePath = OperatePageLayout.getPath(filepath) + "\\data\\Symbol\\ESRI.ServerStyle";//载入系统符号库
            string StyleFilePath = @"C:\Users\Administrator\Desktop\作业\WindowsFormsApplication1\Symbol\ESRI.ServerStyle";
            axSymbologyControl1.LoadStyleFile(StyleFilePath);
            switch (_enumMapSurType)
            {
                case Classes.EnumMapSurroundType.NorthArrow://根据选择，载入系统指北针符号库
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassNorthArrows;
                    pSymStyleClass = axSymbologyControl1.GetStyleClass(esriSymbologyStyleClass.esriStyleClassNorthArrows);
                    break;
                case Classes.EnumMapSurroundType.ScaleBar://根据选择，载入系统比例尺符号库
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;
                    pSymStyleClass = axSymbologyControl1.GetStyleClass(esriSymbologyStyleClass.esriStyleClassScaleBars);
                    break;
            }
            pSymStyleClass.UnselectItem();
        }

        private void axSymbologyControl1_OnMouseDown(object sender, ISymbologyControlEvents_OnMouseDownEvent e)
        {
            pStyleGalleryItem = axSymbologyControl1.HitTest(e.x, e.y);//用户选择需要符号  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetSelSymbolItem(ref pStyleGalleryItem);//传递用户选择的值
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetSelSymbolItem(ref pStyleGalleryItem);//传递用户选择的值
            this.Close();
        }
    }
}
