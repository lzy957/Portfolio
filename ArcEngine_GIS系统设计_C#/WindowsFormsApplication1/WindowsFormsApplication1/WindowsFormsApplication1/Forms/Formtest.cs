using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;

namespace WindowsFormsApplication1
{
    /// <summary>
    /// 几何网络的最优路径分析
    /// </summary>
    public partial class Formtest : Form
    {

        private IGeometricNetwork m_ipGeometricNetwork;
        private IMap m_ipMap;
        private ILayer m_iplayer;
        private IPointCollection m_ipPoints;//输入点集合
        private IPointToEID m_ipPointToEID;
        private double m_dblPathCost = 0;
        private IEnumNetEID m_ipEnumNetEID_Junctions;
        private IEnumNetEID m_ipEnumNetEID_Edges;
        private IPolyline m_ipPolyline;
        private IActiveView m_ipActiveView;
        private bool clicked;
        IGraphicsContainer pGC;
        int clickedcount = 0;

        public Formtest()
        {
            InitializeComponent();
            m_ipActiveView = axMapControl1.ActiveView;
            m_ipMap = m_ipActiveView.FocusMap;
            clicked = false;
            pGC = m_ipMap as IGraphicsContainer;
        }

        public void OpenFeatureDatasetNetwork(IFeatureDataset FeatureDataset)
        {
            CloseWorkspace();
            if (!InitializeNetworkAndMap(FeatureDataset))
                Console.WriteLine("打开network出错");
        }        

        private void test_Load(object sender, EventArgs e)
        {

        }

        #region MyRegion
        //路径成本
        public double PathCost
        {
            get { return m_dblPathCost; }
        }
        //返回路径的几何体
        public IPolyline PathPolyLine()
        {
            IEIDInfo ipEIDInfo;
            IGeometry ipGeometry;
            if (m_ipPolyline != null) return m_ipPolyline;

            m_ipPolyline = new PolylineClass();
            IGeometryCollection ipNewGeometryColl = m_ipPolyline as IGeometryCollection;//引用传递

            ISpatialReference ipSpatialReference = m_ipMap.SpatialReference;
            IEIDHelper ipEIDHelper = new EIDHelperClass();
            ipEIDHelper.GeometricNetwork = m_ipGeometricNetwork;
            ipEIDHelper.OutputSpatialReference = ipSpatialReference;
            ipEIDHelper.ReturnGeometries = true;
            IEnumEIDInfo ipEnumEIDInfo = ipEIDHelper.CreateEnumEIDInfo(m_ipEnumNetEID_Edges);
            int count = ipEnumEIDInfo.Count;
            ipEnumEIDInfo.Reset();
            for (int i = 0; i < count; i++)
            {
                ipEIDInfo = ipEnumEIDInfo.Next();
                ipGeometry = ipEIDInfo.Geometry;
                ipNewGeometryColl.AddGeometryCollection(ipGeometry as IGeometryCollection);
            }
            return m_ipPolyline;
        }
        //解决路径
        public void SolvePath(string WeightName)
        {
            try
            {
                int intEdgeUserClassID;
                int intEdgeUserID;
                int intEdgeUserSubID;
                int intEdgeID;
                IPoint ipFoundEdgePoint;
                double dblEdgePercent;
                ITraceFlowSolverGEN ipTraceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                INetSolver ipNetSolver = ipTraceFlowSolver as INetSolver;
                INetwork ipNetwork = m_ipGeometricNetwork.Network;
                ipNetSolver.SourceNetwork = ipNetwork;
                INetElements ipNetElements = ipNetwork as INetElements;
                int intCount = m_ipPoints.PointCount;//这里的points有值吗？
                //定义一个边线旗数组
                IEdgeFlag[] pEdgeFlagList = new EdgeFlagClass[intCount];
                for (int i = 0; i < intCount; i++)
                {
                    INetFlag ipNetFlag = new EdgeFlagClass() as INetFlag;
                    IPoint ipEdgePoint = m_ipPoints.get_Point(i);
                    //查找输入点的最近的边线
                    m_ipPointToEID.GetNearestEdge(ipEdgePoint, out intEdgeID, out ipFoundEdgePoint, out dblEdgePercent);
                    ipNetElements.QueryIDs(intEdgeID, esriElementType.esriETEdge, out intEdgeUserClassID, out intEdgeUserID, out intEdgeUserSubID);
                    ipNetFlag.UserClassID = intEdgeUserClassID;
                    ipNetFlag.UserID = intEdgeUserID;
                    ipNetFlag.UserSubID = intEdgeUserSubID;
                    IEdgeFlag pTemp = (IEdgeFlag)(ipNetFlag as IEdgeFlag);
                    pEdgeFlagList[i] = pTemp;
                }
                ipTraceFlowSolver.PutEdgeOrigins(ref pEdgeFlagList);
                INetSchema ipNetSchema = ipNetwork as INetSchema;
                INetWeight ipNetWeight = ipNetSchema.get_WeightByName(WeightName);
                INetSolverWeights ipNetSolverWeights = ipTraceFlowSolver as INetSolverWeights;
                ipNetSolverWeights.FromToEdgeWeight = ipNetWeight;//开始边线的权重
                ipNetSolverWeights.ToFromEdgeWeight = ipNetWeight;//终止边线的权重
                object[] vaRes = new object[intCount - 1];
                //通过findpath得到边线和交汇点的集合
                ipTraceFlowSolver.FindPath(esriFlowMethod.esriFMConnected,
                 esriShortestPathObjFn.esriSPObjFnMinSum,
                 out m_ipEnumNetEID_Junctions, out m_ipEnumNetEID_Edges, intCount - 1, ref vaRes);
                //计算元素成本
                m_dblPathCost = 0;
                for (int i = 0; i < vaRes.Length; i++)
                {
                    double m_Va = (double)vaRes[i];//我修改过
                    m_dblPathCost = m_dblPathCost + m_Va;
                }
                m_ipPolyline = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //初始化几何网络和地图 
        private bool InitializeNetworkAndMap(IFeatureDataset FeatureDataset)
        {
            IFeatureClassContainer ipFeatureClassContainer;
            IFeatureClass ipFeatureClass;
            IGeoDataset ipGeoDataset;
            ILayer ipLayer;
            IFeatureLayer ipFeatureLayer;
            IEnvelope ipEnvelope, ipMaxEnvelope;
            double dblSearchTol;
            INetworkCollection ipNetworkCollection = FeatureDataset as INetworkCollection;
            int count = ipNetworkCollection.GeometricNetworkCount;
            //获取第一个几何网络工作空间
            m_ipGeometricNetwork = ipNetworkCollection.get_GeometricNetwork(0);
            INetwork ipNetwork = m_ipGeometricNetwork.Network;
            if (m_ipMap != null)
            {
                //m_ipMap = new MapClass();
                ipFeatureClassContainer = m_ipGeometricNetwork as IFeatureClassContainer;
                count = ipFeatureClassContainer.ClassCount;
                for (int i = 0; i < count; i++)
                {
                    ipFeatureClass = ipFeatureClassContainer.get_Class(i);
                    ipFeatureLayer = new FeatureLayerClass();
                    ipFeatureLayer.FeatureClass = ipFeatureClass;
                    for (int j = 0; j < m_ipMap.LayerCount; j++)
                    {
                        if (m_ipMap.get_Layer(j).Name.ToUpper() == ipFeatureLayer.Name.ToUpper())
                        {
                            continue;
                        }
                    }
                    m_ipMap.AddLayer(ipFeatureLayer);
                }
                m_ipActiveView.Refresh();
            }
            count = m_ipMap.LayerCount;
            ipMaxEnvelope = new EnvelopeClass();
            for (int i = 0; i < count; i++)
            {
                ipLayer = m_ipMap.get_Layer(i);
                ipFeatureLayer = ipLayer as IFeatureLayer;
                ipGeoDataset = ipFeatureLayer as IGeoDataset;
                ipEnvelope = ipGeoDataset.Extent;
                ipMaxEnvelope.Union(ipEnvelope);
            }
            m_ipPointToEID = new PointToEIDClass();
            m_ipPointToEID.SourceMap = m_ipMap;
            m_ipPointToEID.GeometricNetwork = m_ipGeometricNetwork;
            double dblWidth = ipMaxEnvelope.Width;
            double dblHeight = ipMaxEnvelope.Height;
            if (dblWidth > dblHeight)
                dblSearchTol = dblWidth / 100;
            else
                dblSearchTol = dblHeight / 100;
            m_ipPointToEID.SnapTolerance = dblSearchTol;
            return true;
        }
        //关闭工作空间            
        private void CloseWorkspace()
        {
            m_ipGeometricNetwork = null;
            m_ipPoints = null;
            m_ipPointToEID = null;
            m_ipEnumNetEID_Junctions = null;
            m_ipEnumNetEID_Edges = null;
            m_ipPolyline = null;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            ControlsAddDataCommand adddata = new ControlsAddDataCommandClass();
            adddata.OnCreate(axMapControl1.Object);
            adddata.OnClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_ipMap.LayerCount == 0)
                return;
            ILayer ipLayer = m_ipMap.get_Layer(0);
            IFeatureLayer ipFeatureLayer = ipLayer as IFeatureLayer;
            IFeatureDataset ipFDS = ipFeatureLayer.FeatureClass.FeatureDataset;
            OpenFeatureDatasetNetwork(ipFDS);
            clicked = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SolvePath("Weight");//先解析路径
            IPolyline ipPolyResult = PathPolyLine();//最后返回最短路径
            clicked = false;


            IRgbColor color = new RgbColorClass();
            color.Red = 255;
            IElement element = new LineElementClass();
            ILineSymbol linesymbol = new SimpleLineSymbolClass();
            linesymbol.Color = color as IColor;
            linesymbol.Width = 800;
            element.Geometry = m_ipPolyline;
            pGC.AddElement(element, 2);
            m_ipActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            CloseWorkspace();
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (clicked != true)
                return;
            IPoint ipNew;
            if (m_ipPoints == null)
            {
                m_ipPoints = new MultipointClass();
            }
            ipNew = m_ipActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            object o = Type.Missing;
            m_ipPoints.AddPoint(ipNew, ref o, ref o);

            IElement element;
            ITextElement textelement = new TextElementClass();
            element = textelement as IElement;
            clickedcount++;
            textelement.Text = clickedcount.ToString();
            element.Geometry = m_ipActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            pGC.AddElement(element, 0);
            m_ipActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void clear_Click(object sender, EventArgs e)
        {
            clickedcount = 0;
            pGC.DeleteAllElements();
            m_ipActiveView.Refresh();
            axMapControl1.Refresh();
        }



        //private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //获取cboLayer中选中的图层 
        //    IFeatureLayer mFeaturelayer = axMapControl1.get_Layer(comboBox1.SelectedIndex) as IFeatureLayer;
        //    IFeatureClass ipFeatureLayer = mFeaturelayer.FeatureClass;
        //}

    }
}
