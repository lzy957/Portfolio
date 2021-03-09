using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using stdole;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace WindowsFormsApplication1.Statistics
{
    class BarChart : BaseCommand
    {
        IHookHelper m_hookHelper = new HookHelperClass();
        //IMap pMap;
        //IActiveView pActiveView;
        IFeatureLayer m_pLayer = null;
        public IFeatureLayer pLayer
        {
            set { m_pLayer = value; }
            get { return m_pLayer; }
        }
        public BarChart()
        {
            base.m_caption = "BarChartRenderer";
            base.m_message = "BarChartRenderer";
            base.m_toolTip = "BarChartRenderer";
            base.m_category = "Symbology";
            base.m_enabled = true;
            //base.m_bitmap = new 313
            //System.Drawing.Bitmap(GetType().Assembly.GetManifestResourceStream(GetType(),"x.bmp"));
        }
        public override void OnCreate(object hook)
        {
            m_hookHelper.Hook = hook;
        }
        public override void OnClick()
        {
            IGeoFeatureLayer pGeoFeatureL;
            //IFeatureLayer pFeatureLayer;
            ITable pTable;
            ICursor pCursor;
            IQueryFilter pQueryFilter;
            IRowBuffer pRowBuffer;
            int numFields = 5;
            int[] fieldIndecies = new int[numFields];
            int lfieldIndex;
            double dmaxValue;
            bool firstValue;
            double dfieldValue;
            IChartRenderer pChartRenderer;
            IRendererFields pRendererFields;
            IFillSymbol pFillSymbol;
            IMarkerSymbol pMarkerSymbol;
            ISymbolArray pSymbolArray;
            IChartSymbol pChartSymbol;
            string strPopField1 = "A";
            string strPopField2 = "renkou_MD";
            string strPopField3 = "Road_MD";
            string strPopField4 = "Shape_Area";
            string strPopField5 = "COUNT_newclass";
            //pActiveView = m_hookHelper.ActiveView;
            //pMap = m_hookHelper.FocusMap;
            //pMap.ReferenceScale = pMap.MapScale ;
            // pFeatureLayer = (IGeoFeatureLayer) pMap.get_Layer(1);//Parameter!!!
            pGeoFeatureL = (IGeoFeatureLayer)m_pLayer;
            pTable = (ITable)pGeoFeatureL;
            pGeoFeatureL.ScaleSymbols = true;
            pChartRenderer = new ChartRendererClass();
            pRendererFields = (IRendererFields)pChartRenderer;
            pRendererFields.AddField(strPopField1, strPopField1);
            pRendererFields.AddField(strPopField2, strPopField2);
            pRendererFields.AddField(strPopField3, strPopField3);
            pRendererFields.AddField(strPopField4, strPopField4);
            pRendererFields.AddField(strPopField5, strPopField5);

            pQueryFilter = new QueryFilterClass();
            pQueryFilter.AddField(strPopField1);
            pQueryFilter.AddField(strPopField2);
            pQueryFilter.AddField(strPopField3);
            pQueryFilter.AddField(strPopField4);
            pQueryFilter.AddField(strPopField5);
            pCursor = pTable.Search(pQueryFilter, true);
            fieldIndecies[0] = pTable.FindField(strPopField1);
            fieldIndecies[1] = pTable.FindField(strPopField2);
            fieldIndecies[2] = pTable.FindField(strPopField3);
            fieldIndecies[3] = pTable.FindField(strPopField4);
            fieldIndecies[4] = pTable.FindField(strPopField5);
            firstValue = true;
            dmaxValue = 0;
            pRowBuffer = pCursor.NextRow();
            while (pRowBuffer != null)
            {
                for (lfieldIndex = 0; lfieldIndex < numFields; lfieldIndex++)
                {
                    dfieldValue = (double)Convert.ToInt32(pRowBuffer.get_Value(fieldIndecies[lfieldIndex]));
                    if (firstValue)
                    {
                        dmaxValue = dfieldValue;
                        firstValue = false;
                    }
                    else
                    {
                        if (dfieldValue > dmaxValue)
                        {
                            dmaxValue = dfieldValue;
                        }
                    }
                }
                pRowBuffer = pCursor.NextRow();
            }
            if (dmaxValue <= 0)
            {
                MessageBox.Show("Failed to gather stats on the feature class");
                Application.Exit();
            }
            IBarChartSymbol pBarChartSymbol;
            pBarChartSymbol = new BarChartSymbolClass();
            pChartSymbol = (IChartSymbol)pBarChartSymbol;
            pBarChartSymbol.Width = 5;
            pMarkerSymbol = (IMarkerSymbol)pBarChartSymbol;
            pChartSymbol.MaxValue = dmaxValue;
            pMarkerSymbol.Size = 50;
            pSymbolArray = (ISymbolArray)pBarChartSymbol;

            pFillSymbol = new SimpleFillSymbolClass();
            IRgbColor pRGB = new RgbColorClass();
            pRGB.Red = 213;
            pRGB.Green = 212;
            pRGB.Blue = 252;
            pFillSymbol.Color = pRGB;
            pSymbolArray.AddSymbol((ISymbol)pFillSymbol);

            pFillSymbol = new SimpleFillSymbolClass();
            pRGB.Red = 193;
            pRGB.Green = 252;
            pRGB.Blue = 179;
            pFillSymbol.Color = pRGB;
            pSymbolArray.AddSymbol((ISymbol)pFillSymbol);

            pFillSymbol = new SimpleFillSymbolClass();
            pRGB.Red = 170;
            pRGB.Green = 152;
            pRGB.Blue = 179;
            pFillSymbol.Color = pRGB;
            pSymbolArray.AddSymbol((ISymbol)pFillSymbol);

            pFillSymbol = new SimpleFillSymbolClass();
            pRGB.Red = 233;
            pRGB.Green = 52;
            pRGB.Blue = 179;
            pFillSymbol.Color = pRGB;
            pSymbolArray.AddSymbol((ISymbol)pFillSymbol);

            pFillSymbol = new SimpleFillSymbolClass();
            pRGB.Red = 93;
            pRGB.Green = 252;
            pRGB.Blue = 159;
            pFillSymbol.Color = pRGB;
            pSymbolArray.AddSymbol((ISymbol)pFillSymbol);

            //面背景
            pChartRenderer.ChartSymbol = (IChartSymbol)pBarChartSymbol;
            pFillSymbol = new SimpleFillSymbolClass();
            pRGB.Red = 239;
            pRGB.Green = 228;
            pRGB.Blue = 190;
            pFillSymbol.Color = pRGB;

            // 设置背景符号
            pChartRenderer.BaseSymbol = (ISymbol)pFillSymbol;
            //让符号处于图形中央（若渲染的图层为点图层，则该句应去掉，否则不显示渲染结果）
            pChartRenderer.UseOverposter = false;
            pChartRenderer.CreateLegend();
            pGeoFeatureL.Renderer = (IFeatureRenderer)pChartRenderer;
        }
    }
}
