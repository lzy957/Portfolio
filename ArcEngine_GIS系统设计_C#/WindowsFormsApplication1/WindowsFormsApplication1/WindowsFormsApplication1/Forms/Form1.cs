using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.SystemUI; 
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.ADF;
using WindowsFormsApplication1.Classes;
using WindowsFormsApplication1.Forms;
using WindowsFormsApplication1;
using WindowsFormsApplication1.Statistics;
using stdole;
using BaseLibs;


namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        private string pMapUnits;
        private ILayer pLayer;
        private Pan pan = null;
        private bool identify = false;
        private bool highlight = false;
        private bool idClick = false;
        private bool idBox = false;
        private bool drawcircle = false;
        private bool drawshape = false;
        private bool isBorderOpen = false;
        private bool isBackGroudOpen = false;
        private bool isGridOpen = false;
        private bool isShadowOpen = false;
        private bool ispathanalysis = false;
        //RoadAnalysis m_ipPathFinder;
        //MultipointClass m_ipPoints;
        //vs2015
        private IGeometricNetwork m_ipGeometricNetwork;
        private IMap m_ipMap;
        private IPointCollection m_ipPoints;//输入点集合
        private IPointToEID m_ipPointToEID;
        private double m_dblPathCost = 0;
        private IEnumNetEID m_ipEnumNetEID_Junctions;
        private IEnumNetEID m_ipEnumNetEID_Edges;
        private IPolyline m_ipPolyline;
        private IActiveView m_ipActiveView;
        private bool clicked = false;
        IGraphicsContainer pGC;
        int clickedcount = 0;
        //private IHookHelper m_hookHelper = null;
        IMap pMap;
        private IActiveView pActiveView;
        private IEnvelope pEnv;
        private ISelectionEnvironment pSelectionEnv;
        private IEnumFeature pEnumFeature;
        private IGraphicsContainer pGraphicsContainer;
        private IFeature pFeature;
        private IGeometry pGeometry;
        private IEnvelope pEnvClip;
        private IPolyline pLineCut;
        private IPolygon pFirstPolygon;
        private bool isClip = false;
        private bool isCut = false;
        private bool isFirstIn = false;
        private bool isbufferclick = false;
        private bool isspatialana = false;
        private bool isfirstcontainer = false;
        private bool issecondcontainer = false;
        private bool isMeasure = false;
        private IGeometry pGeometryA;
        private IGeometry pGeometryB;

        private IPoint m_PointPt = null;
        private IPoint m_MovePt = null;
        private INewEnvelopeFeedback pNewEnvelopeFeedback;
        private IStyleGalleryItem pStyleGalleryItem;   //比例尺
        private EnumMapSurroundType _enumMapSurType = EnumMapSurroundType.None;  //图例
        private frmSymbol frmSym = null;
        private OperatePageLayout m_OperatePageLayout = new OperatePageLayout();
        string pMouseOperate = null;                //鼠标点击区分的switch（可用在其他部分）
        private FrmMeasureResult frmMeasureResult = null;    //量算结果窗体
        private INewLineFeedback pNewLineFeedback;           //追踪线对象
        private INewPolygonFeedback pNewPolygonFeedback;     //追踪面对象
        private IPoint pPointPt = null;                      //鼠标点击点
        private IPoint pMovePt = null;                       //鼠标移动时的当前点
        private double dToltalLength = 0;                    //量测总长度
        private double dSegmentLength = 0;                   //片段距离
        private IPointCollection pAreaPointCol = new MultipointClass();  //面积量算时画的点进行存储
        private object missing = Type.Missing;

        #region 空间查询变量
        //查询方式  
        public int mQueryModel;
        //图层索引  
        public int mLayerIndex;
        private string mTool;
        private bool issqform = false;
        #endregion

        public Form1()
        {
            InitializeComponent();
            pMapUnits = "Unknown";
            axMapControl2_initial();
            //vs2015
            m_ipActiveView = axMapControl1.ActiveView;
            m_ipMap = m_ipActiveView.FocusMap;
            pMap = axMapControl1.Map;
            //pEnumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
            clicked = false;
            pGC = m_ipMap as IGraphicsContainer;
        }

        //vs2015
        public void OpenFeatureDatasetNetwork(IFeatureDataset FeatureDataset)
        {
            CloseWorkspace();
            if (!InitializeNetworkAndMap(FeatureDataset))
                Console.WriteLine("打开network出错");
        }
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
            //m_ipGeometricNetwork = ipNetworkCollection.get_GeometricNetwork(0);
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
        public void OpenAccessNetwork(string AccessFileName, string FeatureDatasetName)
        {
            IWorkspaceFactory ipWorkspaceFactory;
            IWorkspace ipWorkspace;
            IFeatureWorkspace ipFeatureWorkspace;
            IFeatureDataset ipFeatureDataset;
            CloseWorkspace();

            //open the mdb
            ipWorkspaceFactory = new AccessWorkspaceFactory();
            ipWorkspace = ipWorkspaceFactory.OpenFromFile(AccessFileName, 0);

            //et the FeatureWorkspace
            ipFeatureWorkspace = ipWorkspace as IFeatureWorkspace;

            //open the FeatureDataset
            ipFeatureDataset = ipFeatureWorkspace.OpenFeatureDataset(FeatureDatasetName);

            //initialize Network and Map (m_ipNetwork, m_ipMap)
            if (InitializeNetworkAndMap(ipFeatureDataset))
            {
                MessageBox.Show("Error!");
            }
        }

        //vs2015 shijian
        private void FindPath_Click(object sender, EventArgs e)
        {
            if (m_ipMap.LayerCount == 0)
                return;
            ILayer ipLayer = m_ipMap.get_Layer(0);
            IFeatureLayer ipFeatureLayer = ipLayer as IFeatureLayer;
            IFeatureDataset ipFDS = ipFeatureLayer.FeatureClass.FeatureDataset;
            OpenFeatureDatasetNetwork(ipFDS);
            clicked = true;
        }

        private void PathSolve_Click(object sender, EventArgs e)
        {

        }


        //2010
        private void CopyMapFromMapControlToPageLayoutControl()
        {
            //获得IObjectCopy接口
            IObjectCopy pObjectCopy = new ObjectCopyClass();
            //获得要拷贝的图层 
            System.Object pSourceMap = axMapControl1.Map;
            //获得拷贝图层
            System.Object pCopiedMap = pObjectCopy.Copy(pSourceMap);
            //获得要重绘的地图 
            System.Object pOverwritedMap = axPageLayoutControl1.ActiveView.FocusMap;
            //重绘pagelayout地图
            pObjectCopy.Overwrite(pCopiedMap, ref pOverwritedMap);
        }


        private void openMxd_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenMXD = new OpenFileDialog(); //可实例化类
            // Gets or sets the file dialog box title. (Inherited from FileDialog.)
            OpenMXD.Title = "打开地图"; // OpenFileDialog类的属性Title
            // Gets or sets the initial directory displayed by the file dialog box. 
            OpenMXD.InitialDirectory = @"C:\Users\Administrator\Documents\arcgis\201812";
            // Gets or sets the current file name filter string ,Save as file type
            OpenMXD.Filter = "Map Documents (*.mxd)|*.mxd";
            if (OpenMXD.ShowDialog() == DialogResult.OK) //ShowDialog是类的方法
            {
                //FileName:Gets or sets a string containing the file name selected in the file dialog box
                string MxdPath = OpenMXD.FileName;
                axMapControl1.LoadMxFile(MxdPath);
                axMapControl2_initial();
                //IMapControl2的方法
            }
        }

        private void OpenSHP_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenShpFile = new OpenFileDialog();
            OpenShpFile.Title = "打开Shape文件";
            OpenShpFile.InitialDirectory = @"C:\Users\Administrator\Desktop\作业\讲义\USA";
            OpenShpFile.Filter = "Shape文件(*.shp)|*.shp";
            if (OpenShpFile.ShowDialog() == DialogResult.OK)
            {
                string ShapPath = OpenShpFile.FileName;
                int Position = ShapPath.LastIndexOf("\\"); //利用"\\"将文件路径分成两部分
                string FilePath = ShapPath.Substring(0, Position);
                string ShpName = ShapPath.Substring(Position + 1);
                axMapControl1.AddShapeFile(FilePath, ShpName);
                axMapControl2_initial();
                //axMapControl2.AddShapeFile(FilePath, ShpName);
            }

        }

        private void OpenLyr_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenLyrFile = new OpenFileDialog();
            OpenLyrFile.Title = "打开Lyr";
            OpenLyrFile.InitialDirectory = @"C:\Users\Administrator\Desktop\作业\讲义\USA";
            OpenLyrFile.Filter = "lyr文件(*.lyr)|*.lyr";
            if (OpenLyrFile.ShowDialog() == DialogResult.OK)
            {
                string LayPath = OpenLyrFile.FileName;
                axMapControl1.AddLayerFromFile(LayPath);
                axMapControl2_initial();
                //axMapControl2.AddLayerFromFile(LayPath);
            }

        }

        private void axMapControl2_initial()
        {
            if (axMapControl1.LayerCount > 0)
            {
                axMapControl2.Map = new MapClass();
                for (int i = axMapControl1.Map.LayerCount - 1; i >= 0; i--)
                {
                    axMapControl2.AddLayer(axMapControl1.get_Layer(i));
                }
                axMapControl2.Extent = axMapControl1.Extent;
                axMapControl2.Refresh();
            }
            CopyMapFromMapControlToPageLayoutControl();//调用地图复制函数

            #region 坐标单位替换
            esriUnits mapUnits = axMapControl1.Map.MapUnits;
            switch (mapUnits)
            {
                case esriUnits.esriCentimeters:
                    pMapUnits = "Centimeters";
                    break;
                case esriUnits.esriDecimalDegrees:
                    pMapUnits = "Decimal Degrees";
                    break;
                case esriUnits.esriDecimeters:
                    pMapUnits = "Decimeters";
                    break;
                case esriUnits.esriFeet:
                    pMapUnits = "Feet";
                    break;
                case esriUnits.esriInches:
                    pMapUnits = "Inches";
                    break;
                case esriUnits.esriKilometers:
                    pMapUnits = "Kilometers";
                    break;
                case esriUnits.esriMeters:
                    pMapUnits = "Meters";
                    break;
                case esriUnits.esriMiles:
                    pMapUnits = "Miles";
                    break;
                case esriUnits.esriMillimeters:
                    pMapUnits = "Millimeters";
                    break;
                case esriUnits.esriNauticalMiles:
                    pMapUnits = "NauticalMiles";
                    break;
                case esriUnits.esriPoints:
                    pMapUnits = "Points";
                    break;
                case esriUnits.esriUnknownUnits:
                    pMapUnits = "Unknown";
                    break;
                case esriUnits.esriYards:
                    pMapUnits = "Yards";
                    break;
            }
            #endregion
        }

        private void axMapControl1_OnMapReplaced(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMapReplacedEvent e)
        {
            if (axMapControl1.LayerCount > 0)
            {
                axMapControl2.Map = new MapClass();
                for (int i = axMapControl1.Map.LayerCount - 1; i >= 0; i--)
                {
                    axMapControl2.AddLayer(axMapControl1.get_Layer(i));
                }
                axMapControl2.Extent = axMapControl1.Extent;
                axMapControl2.Refresh();
            }
            CopyMapFromMapControlToPageLayoutControl();//调用地图复制函数

            #region 坐标单位替换
            esriUnits mapUnits = axMapControl1.MapUnits;
            switch (mapUnits)
            {
                case esriUnits.esriCentimeters:
                    pMapUnits = "Centimeters";
                    break;
                case esriUnits.esriDecimalDegrees:
                    pMapUnits = "Decimal Degrees";
                    break;
                case esriUnits.esriDecimeters:
                    pMapUnits = "Decimeters";
                    break;
                case esriUnits.esriFeet:
                    pMapUnits = "Feet";
                    break;
                case esriUnits.esriInches:
                    pMapUnits = "Inches";
                    break;
                case esriUnits.esriKilometers:
                    pMapUnits = "Kilometers";
                    break;
                case esriUnits.esriMeters:
                    pMapUnits = "Meters";
                    break;
                case esriUnits.esriMiles:
                    pMapUnits = "Miles";
                    break;
                case esriUnits.esriMillimeters:
                    pMapUnits = "Millimeters";
                    break;
                case esriUnits.esriNauticalMiles:
                    pMapUnits = "NauticalMiles";
                    break;
                case esriUnits.esriPoints:
                    pMapUnits = "Points";
                    break;
                case esriUnits.esriUnknownUnits:
                    pMapUnits = "Unknown";
                    break;
                case esriUnits.esriYards:
                    pMapUnits = "Yards";
                    break;
            }
            #endregion
        }

        private void axMapControl1_OnExtentUpdated(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnExtentUpdatedEvent e)
        {

            // 得到新范围
            IEnvelope pEnvelope = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGraphicsContainer = axMapControl2.Map as IGraphicsContainer;
            IActiveView pActiveView = pGraphicsContainer as IActiveView;
            //在绘制前，清除axMapControl2中的任何图形元素
            pGraphicsContainer.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pElement = pRectangleEle as IElement;
            pElement.Geometry = pEnvelope;
            //设置鹰眼图中的红线框
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255; pColor.Green = 0; pColor.Blue = 0; pColor.Transparency = 255;
            //产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 3; pOutline.Color = pColor;
            //设置颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255; pColor.Green = 0; pColor.Blue = 0; pColor.Transparency = 0;
            //设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor; pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pElement as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGraphicsContainer.AddElement((IElement)pFillShapeEle, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//部分刷新

        }

        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button == 1)
            {
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(e.mapX, e.mapY);
                axMapControl1.CenterAt(pPoint);
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }

        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (axMapControl2.Map.LayerCount > 0)
            {
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    axMapControl1.CenterAt(pPoint);
                    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                else if (e.button == 2)
                {
                    IEnvelope pEnv = axMapControl2.TrackRectangle();
                    axMapControl1.Extent = pEnv;
                    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }

        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            if (axMapControl1.LayerCount > 0)
            {
                esriTOCControlItem pItem = new esriTOCControlItem();
                pLayer = new FeatureLayerClass();
                IBasicMap pBasicMap = new MapClass();
                object pOther = new object();
                object pIndex = new object();
                // Returns the item in the TOCControl at the specified coordinates.
                axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pBasicMap, ref pLayer, ref pOther, ref pIndex);
            }//TOCControl类的ITOCControl接口的HitTest方法
            if (e.button == 2)
            {
                contextMenuStrip1.Show(axTOCControl1, e.x, e.y);
            }

        }

        private void openAttToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //传入图层，在右击事件里返回的图层
            FrmAttribute frm1 = new FrmAttribute(pLayer);
            frm1.Show();
        }



        private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            //获得IActiveView接口
            IActiveView pPageLayoutView = (IActiveView)axPageLayoutControl1.ActiveView.FocusMap;
            //获得IDisplayTransformation接口
            IDisplayTransformation pDisplayTransformation = pPageLayoutView.ScreenDisplay.DisplayTransformation;
            //设置可视范围
            pDisplayTransformation.VisibleBounds = axMapControl1.Extent;
            axPageLayoutControl1.ActiveView.Refresh(); //刷新地图
            //根据MapControl的视图范围，确定PageLayoutControl的视图范围
            CopyMapFromMapControlToPageLayoutControl();
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 取得鼠标所在工具的索引号  
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                // 取得鼠标所在工具的 ToolbarItem  
                IToolbarItem toolbarItem = axToolbarControl1.GetItem(index);
                // 设置状态栏信息  
                MessageLabel.Text = toolbarItem.Command.Message;
            }
            else
            {
                MessageLabel.Text = " 就绪 ";
            }
            // 显示当前比例尺
            ScaleLabel.Text = " 比例尺 1:" + ((long)this.axMapControl1.MapScale).ToString();
            // 显示当前坐标
            CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits;
            CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + pMapUnits.ToString();
            if (isMeasure)
            {
                pMovePt = (axMapControl1.Map as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                #region 长度量算
                if (pMouseOperate == "MeasureLength")
                {
                    if (pNewLineFeedback != null)
                    {
                        pNewLineFeedback.MoveTo(pMovePt);
                    }
                    double deltaX = 0; //两点之间X差值
                    double deltaY = 0; //两点之间Y差值

                    if ((pPointPt != null) && (pNewLineFeedback != null))
                    {
                        deltaX = pMovePt.X - pPointPt.X;
                        deltaY = pMovePt.Y - pPointPt.Y;
                        dSegmentLength = Math.Round(Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY)), 3);
                        dToltalLength = dToltalLength + dSegmentLength;
                        if (frmMeasureResult != null)
                        {
                            frmMeasureResult.lblMeasureResult.Text = String.Format(
                                "当前线段长度：{0:.###}{1};\r\n总长度为: {2:.###}{1}",
                                dSegmentLength, pMapUnits, dToltalLength);
                            dToltalLength = dToltalLength - dSegmentLength; //鼠标移动到新点重新开始计算
                        }
                        frmMeasureResult.frmClosed += new FrmMeasureResult.FrmClosedEventHandler(frmMeasureResult_frmColsed);
                    }
                }
                #endregion
                #region 面积量算
                if (pMouseOperate == "MeasureArea")
                {
                    if (pNewPolygonFeedback != null)
                    {
                        pNewPolygonFeedback.MoveTo(pMovePt);
                    }

                    IPointCollection pPointCol = new Polygon();
                    IPolygon pPolygon = new PolygonClass();
                    IGeometry pGeo = null;

                    ITopologicalOperator pTopo = null;
                    for (int i = 0; i <= pAreaPointCol.PointCount - 1; i++)
                    {
                        pPointCol.AddPoint(pAreaPointCol.get_Point(i), ref missing, ref missing);
                    }
                    pPointCol.AddPoint(pMovePt, ref missing, ref missing);

                    if (pPointCol.PointCount < 3) return;
                    pPolygon = pPointCol as IPolygon;

                    if ((pPolygon != null))
                    {
                        pPolygon.Close();
                        pGeo = pPolygon as IGeometry;
                        pTopo = pGeo as ITopologicalOperator;
                        //使几何图形的拓扑正确
                        pTopo.Simplify();
                        pGeo.Project(axMapControl1.Map.SpatialReference);
                        IArea pArea = pGeo as IArea;

                        frmMeasureResult.lblMeasureResult.Text = String.Format(
                            "总面积为：{0:.####}平方{1};\r\n总长度为：{2:.####}{1}",
                            pArea.Area, pMapUnits, pPolygon.Length);
                        pPolygon = null;
                    }
                }
                #endregion
            }
            //漫游（BaseTool方法）
            if (pan != null)
                pan.OnMouseMove(e.button, e.shift, e.x, e.y);

            axMapControl1.ShowMapTips = true;
            IFeatureLayer pFeatureLayer = axMapControl1.Map.get_Layer(0) as IFeatureLayer;
            pFeatureLayer.DisplayField = "Name";
            pFeatureLayer.ShowTips = true;

            if (axMapControl1.LayerCount == 0) return;
            pMapUnits = GetMapUnit(axMapControl1.Map.MapUnits);  //求量测所需的单位

            
 
        }

        private void 中心放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //声明与初始化 
            FixedZoomIn fixedZoomin = new FixedZoomIn();
            //与MapControl关联 
            fixedZoomin.OnCreate(this.axMapControl1.Object);
            fixedZoomin.OnClick();
        }

        private void 中心缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand command = new ControlsMapZoomOutFixedCommandClass();
            command.OnCreate(this.axMapControl1.Object);
            command.OnClick();
        }

        private void 漫游ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //声明并初始化
            pan = new Pan();
            //关联MapControl
            pan.OnCreate(this.axMapControl1.Object);
            //设置鼠标形状 
            //this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerPan;
            this.axMapControl1.CurrentTool = pan;

        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            DrawCircle();
            DrawShape();
            //清空上次选择的结果  
            this.axMapControl1.Map.ClearSelection();
            if (issqform)
                switch (mTool)
                {
                    case "SpatialQuery":
                        //获取当前视图  
                        ESRI.ArcGIS.Carto.IActiveView pActiveView = this.axMapControl1.ActiveView;
                        //获取鼠标点  
                        ESRI.ArcGIS.Geometry.IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                        panel1.Visible = true;
                        ESRI.ArcGIS.Geometry.IGeometry pGeometry = null;
                        switch (this.mQueryModel)
                        {
                            case 0:         //矩形查询  
                                pGeometry = this.axMapControl1.TrackRectangle();
                                break;
                            case 1:         //线查询  
                                pGeometry = this.axMapControl1.TrackLine();
                                break;
                            case 2:         //点查询  
                                ESRI.ArcGIS.Geometry.ITopologicalOperator pTopo;
                                ESRI.ArcGIS.Geometry.IGeometry pBuffer;
                                pGeometry = pPoint;
                                pTopo = pGeometry as ESRI.ArcGIS.Geometry.ITopologicalOperator;
                                //根据点位创建缓冲区，缓冲半径设为0.1，可自行修改  
                                pBuffer = pTopo.Buffer(0.1);
                                pGeometry = pBuffer.Envelope;
                                break;
                            case 3:         //圆查询  
                                pGeometry = this.axMapControl1.TrackCircle();
                                break;
                        }
                        ESRI.ArcGIS.Carto.IFeatureLayer pFeatureLayer = this.axMapControl1.Map.get_Layer(this.mLayerIndex) as ESRI.ArcGIS.Carto.IFeatureLayer;
                        DataTable pDataTable = this.LoadQueryResult(axMapControl1, pFeatureLayer, pGeometry);
                        this.dataGridView1.DataSource = pDataTable.DefaultView;
                        this.dataGridView1.Refresh();
                        break;
                    default:
                        break;

                }
            //漫游
            if (pan != null)
                pan.OnMouseDown(e.button, e.shift, e.x, e.y);
            if (isfirstcontainer)
            {
                m_ipMap = axMapControl1.Map;
                pActiveView = m_ipMap as IActiveView;
                pEnv = axMapControl1.TrackRectangle();

                pSelectionEnv = new SelectionEnvironment();
                pSelectionEnv.DefaultColor = getRGB(100, 100, 100);
                m_ipMap.SelectByShape(pEnv, pSelectionEnv, false);
                pActiveView.Refresh();
                pEnumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
                pFeature = pEnumFeature.Next() as IFeature;     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    MessageBox.Show("No GeometryA!");
                pGeometryA = pFeature.Shape;     //获取要素的Geometry
                isfirstcontainer = false;

            }
            if (issecondcontainer)
            {
                m_ipMap = axMapControl1.Map;
                pActiveView = m_ipMap as IActiveView;
                pEnv = axMapControl1.TrackRectangle();

                pSelectionEnv = new SelectionEnvironment();
                pSelectionEnv.DefaultColor = getRGB(200, 200, 200);
                m_ipMap.SelectByShape(pEnv, pSelectionEnv, false);
                pActiveView.Refresh();
                pEnumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
                pFeature = pEnumFeature.Next();     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    MessageBox.Show("No GeometryB!");
                pGeometryB = pFeature.Shape;     //获取要素的Geometry
                issecondcontainer = false;
            }
            //点集要素信息box
            if (idClick)
            {
                IIdentify pIdentify = axMapControl1.Map.get_Layer(0) as IIdentify; //通过图层获取 IIdentify 实例
                IPoint pPoint = new ESRI.ArcGIS.Geometry.Point(); //新建点来选择
                IArray pIDArray;
                IIdentifyObj pIdObj;

                pPoint.PutCoords(e.mapX, e.mapY);      //定义点
                pIDArray = pIdentify.Identify(pPoint);       //通过点获取数组，用点一般只能选择一个元素
                if (pIDArray != null)
                {
                    pIdObj = pIDArray.get_Element(0) as IIdentifyObj; //取得要素
                    pIdObj.Flash(axMapControl1.ActiveView.ScreenDisplay);       //闪烁效果
                    MessageBox.Show("Layer: " + pIdObj.Layer.Name + "\n" + "Feature: " + pIdObj.Name); //输出信息
                }
                else
                {
                    MessageBox.Show("Nothing!");
                }
                //高亮框选要素
                idClick = false;
            }
            if (highlight)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
                pMap = axMapControl1.Map;
                pMap.ClearSelection();
                pSelectionEnv = new SelectionEnvironment(); //新建选择环境
                axMapControl1.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                pEnumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
                IGeometry pGeometry = axMapControl1.TrackRectangle();       //获取框选几何
                IRgbColor pColor = new RgbColor();
                pColor.Red = 255;
                pSelectionEnv.DefaultColor = pColor;         //设置高亮显示的颜色！

                pMap.SelectByShape(pGeometry, pSelectionEnv, false); //选择图形！

                axMapControl1.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                highlight = false;
            }
            else
            {
                pMap = axMapControl1.Map;
                pMap.ClearSelection();
                axMapControl1.ActiveView.Refresh();
            }
            if (isspatialana)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                if (isClip) //此时要拉Clip框
                {
                    pEnvClip = axMapControl1.TrackRectangle();
                    isClip = false;
                }
                else if (isCut)
                {
                    pLineCut = axMapControl1.TrackLine() as IPolyline;
                    isCut = false;
                }
                else if (isFirstIn)
                {
                    m_ipMap = axMapControl1.Map;
                    pActiveView = m_ipMap as IActiveView;
                    pEnv = axMapControl1.TrackRectangle();

                    pSelectionEnv = new SelectionEnvironment();
                    pSelectionEnv.DefaultColor = getRGB(0, 255, 0);
                    m_ipMap.SelectByShape(pEnv, pSelectionEnv, false);
                    pActiveView.Refresh();
                    pEnumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
                    isbufferclick = true;
                }
                else
                {
                    m_ipMap = axMapControl1.Map;
                    pActiveView = m_ipMap as IActiveView;
                    pEnv = axMapControl1.TrackRectangle();

                    pSelectionEnv = new SelectionEnvironment();
                    pSelectionEnv.DefaultColor = getRGB(255, 255, 0);
                    m_ipMap.SelectByShape(pEnv, pSelectionEnv, false);
                    pActiveView.Refresh();
                    pEnumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
                }

            }
            //框选要素信息box
            if (idBox)
            {
                IIdentify pIdentify = axMapControl1.Map.get_Layer(0) as IIdentify;
                IGeometry pGeo = axMapControl1.TrackRectangle() as IGeometry;
                IArray pIDArray;
                IIdentifyObj pIdObj;

                pIDArray = pIdentify.Identify(pGeo);
                if (pIDArray != null)
                {
                    string str = "\n";
                    string lyrName = "";
                    for (int i = 0; i < pIDArray.Count; i++)
                    {
                        pIdObj = pIDArray.get_Element(i) as IIdentifyObj;
                        pIdObj.Flash(axMapControl1.ActiveView.ScreenDisplay);
                        str += pIdObj.Name + "\n";
                        lyrName = pIdObj.Layer.Name;
                    }
                    MessageBox.Show("Layer: " + lyrName + "\n" + "Feature: " + str);
                }
                else
                {
                    MessageBox.Show("Nothing!");
                }
                idBox = false;
                /*
                if (ispathanalysis)
                {
                    IPoint ipNew;
                    if (m_ipPoints == null)
                    {
                        m_ipPoints = new MultipointClass();
                        m_ipPathFinder.StopPoints = m_ipPoints;
                    }
                    ipNew = axMapControl1.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                    object o = Type.Missing;
                    m_ipPoints.AddPoint(ipNew, ref o, ref o);
                    ispathanalysis = false;
                }*/
                //2015
            }
            if (isMeasure)
            {
                //屏幕坐标点转化为地图坐标点
                pPointPt = (axMapControl1.Map as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                if (e.button == 1)   //左键
                {
                    IActiveView pActiveView = axMapControl1.ActiveView;
                    IEnvelope pEnvelope = new EnvelopeClass();
                    switch (pMouseOperate)
                    {
                        #region 距离量算
                        case "MeasureLength":
                            //判断追踪线对象是否为空，若是则实例化并设置当前鼠标点为起始点
                            if (pNewLineFeedback == null)
                            {
                                //实例化追踪线对象
                                pNewLineFeedback = new NewLineFeedbackClass();
                                pNewLineFeedback.Display = (axMapControl1.Map as IActiveView).ScreenDisplay;
                                //设置起点，开始动态线绘制
                                pNewLineFeedback.Start(pPointPt);
                                dToltalLength = 0;
                            }
                            else //如果追踪线对象不为空，则添加当前鼠标点
                            {
                                pNewLineFeedback.AddPoint(pPointPt);
                            }
                            if (dSegmentLength != 0)
                            {
                                dToltalLength = dToltalLength + dSegmentLength;
                            }
                            break;
                        #endregion
                        #region 面积量算
                        case "MeasureArea":
                            if (pNewPolygonFeedback == null)
                            {
                                //实例化追踪面对象
                                pNewPolygonFeedback = new NewPolygonFeedback();
                                pNewPolygonFeedback.Display = (axMapControl1.Map as IActiveView).ScreenDisplay;
                                pAreaPointCol.RemovePoints(0, pAreaPointCol.PointCount);
                                //开始绘制多边形
                                pNewPolygonFeedback.Start(pPointPt);
                                pAreaPointCol.AddPoint(pPointPt, ref missing, ref missing);
                            }
                            else
                            {
                                pNewPolygonFeedback.AddPoint(pPointPt);
                                pAreaPointCol.AddPoint(pPointPt, ref missing, ref missing);
                            }
                            break;
                        #endregion
                        #region 选择要素
                        case "SelFeature":
                            IEnvelope pEnv = axMapControl1.TrackRectangle();
                            IGeometry pGeo = pEnv as IGeometry;
                            //矩形框若为空，即为点选时，对点范围进行扩展
                            if (pEnv.IsEmpty == true)
                            {
                                tagRECT r;
                                r.left = e.x;
                                r.top = e.y;
                                r.right = e.x;
                                r.bottom = e.y;
                                pActiveView.ScreenDisplay.DisplayTransformation.TransformRect(pEnv, ref r, 4);
                                pEnv.SpatialReference = pActiveView.FocusMap.SpatialReference;
                            }
                            pGeo = pEnv as IGeometry;
                            axMapControl1.Map.SelectByShape(pGeo, null, false);
                            axMapControl1.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                            break;
                        #endregion
                        default:
                            break;
                    }
                }
                else if (e.button == 2) //右键
                {
                }
            }
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

        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            //漫游（BaseTool方法）
            if (pan != null)
                pan.OnMouseUp(e.button, e.shift, e.x, e.y);

        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Tool的定义和初始化 
            ITool tool = new ControlsMapZoomInToolClass();
            //查询接口获取ICommand 
            ICommand command = tool as ICommand;
            //Tool通过ICommand与MapControl的关联 
            command.OnCreate(this.axMapControl1.Object);
            command.OnClick();
            //MapControl的当前工具设定为tool 
            this.axMapControl1.CurrentTool = tool;

        }

        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Tool的定义和初始化 
            ITool tool = new ControlsMapZoomOutToolClass();
            //查询接口获取ICommand 
            ICommand command = tool as ICommand;
            //Tool通过ICommand与MapControl的关联 
            command.OnCreate(this.axMapControl1.Object);
            command.OnClick();
            //MapControl的当前工具设定为tool 
            this.axMapControl1.CurrentTool = tool;

        }

        private void 全图显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand Full = new ControlsMapFullExtentCommandClass();
            Full.OnCreate(axMapControl1.Object);
            Full.OnClick();

        }

        private void RoadAnalysisToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NetworkAnalysisform fnetworkAnalysis = new NetworkAnalysisform();
            fnetworkAnalysis.Show();

        }

        private void attrToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AttributeQueryForm frmattributequery = new AttributeQueryForm(this.axMapControl1);
            frmattributequery.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axTOCControl1.SetBuddyControl(axMapControl1);//手动设置控件绑定
        }

        private void attrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (axMapControl1.Map.LayerCount > 0)
                axMapControl1.DeleteLayer(0);
            if (axMapControl2.Map.LayerCount > 0)
                axMapControl2.DeleteLayer(0);
        }

        private void identifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (identify)
                identify = false;
            else
                identify = true;
        }

        private void highlightChoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (highlight)
                highlight = false;
            else
                highlight = true;
        }

        private void clickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!idClick)
                idClick = true;
        }

        private void boxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!idBox)
                idBox = true;
        }

        private void attributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //声明与初始化 
            Attr AttrLayer = new Attr(pLayer);
            //与MapControl关联 
            AttrLayer.OnCreate(this.axMapControl1.Object);
            AttrLayer.OnClick();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!isBorderOpen)
            {
                IActiveView pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                IMap pMap = pActiveView.FocusMap;
                IGraphicsContainer pGraphicsContainer = pActiveView as IGraphicsContainer;
                IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
                IStyleSelector pStyleSelector = new BorderSelector();
                if (pStyleSelector.DoModal(axPageLayoutControl1.hWnd))
                {
                    IBorder PBorder = pStyleSelector.GetStyle(0) as IBorder;
                    pMapFrame.Border = PBorder;
                }
                axPageLayoutControl1.Refresh(esriViewDrawPhase.esriViewBackground, null, null);
                isBorderOpen = true;
            }
            else
                isBorderOpen = false;
        }

        private void backGroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isBackGroudOpen)
            {
                IActiveView pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                IMap pMap = pActiveView.FocusMap;
                IGraphicsContainer pGraphicsContainer = pActiveView as IGraphicsContainer;
                IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
                IStyleSelector pStyleSelector = new BackgroundSelector();
                if (pStyleSelector.DoModal(axPageLayoutControl1.hWnd))
                {
                    IBackground pBackground = pStyleSelector.GetStyle(0) as IBackground;
                    pMapFrame.Background = pBackground;
                }
                pActiveView.Refresh();
                isBackGroudOpen = true;
            }
            else
                isBackGroudOpen = false;
        }

        private void shadowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isShadowOpen)
            {
                IActiveView pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                IMap pMap = pActiveView.FocusMap;
                IGraphicsContainer pGraphicsContainer = pActiveView as IGraphicsContainer;
                IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
                IStyleSelector pStyleSelector = new ShadowSelector();
                if (pStyleSelector.DoModal(axPageLayoutControl1.hWnd))
                {
                    IShadow pShadow = pStyleSelector.GetStyle(0) as IShadow;
                    IFrameProperties pFrameProperties = pMapFrame as IFrameProperties;
                    pFrameProperties.Shadow = pShadow;
                }
                pActiveView.Refresh();
                isShadowOpen = true;
            }
            else
                isShadowOpen = false;
        }

        private void mapGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isGridOpen)
            {
                IActiveView pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                IMap pMap = pActiveView.FocusMap;
                IGraphicsContainer pGraphicsContainer = pActiveView as IGraphicsContainer;
                IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
                IStyleSelector pStyleSelector = new MapGridSelector();
                if (pStyleSelector.DoModal(axPageLayoutControl1.hWnd))
                {
                    IMapGrid pMapGrid = pStyleSelector.GetStyle(0) as IMapGrid;
                    IMapGrids pMapGrids = pMapFrame as IMapGrids;
                    if (pMapGrid == null)
                    {
                        return;
                    }
                    pMapGrids.AddMapGrid(pMapGrid);
                }
                pActiveView.Refresh();
                isGridOpen = true;
            }
            else
                isGridOpen = false;
        }

        private IColor getRGB(int R, int G, int B)
        {
            IRgbColor pColor = new RgbColor();
            pColor.Red = R;
            pColor.Green = G;
            pColor.Blue = B;
            return pColor as IColor;
        }

        private void DrawShape()
        {
            if (drawshape)
            {
                //m_ipActiveView = m_hookHelper.ActiveView;
                //m_ipMap = m_hookHelper.FocusMap;
                m_ipActiveView = axMapControl1.ActiveView;
                m_ipMap = axMapControl1.ActiveView.FocusMap;
                IScreenDisplay pScreenDisplay = m_ipActiveView.ScreenDisplay;
                IRubberBand pRubberPolygon = new RubberPolygonClass();
                ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = getRGB(255, 255, 0);
                IPolygon pPolygon = pRubberPolygon.TrackNew(pScreenDisplay, (ISymbol)pFillSymbol) as IPolygon;
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                pFillSymbol.Color = getRGB(0, 255, 255);
                IFillShapeElement pPolygonEle = new PolygonElementClass();
                pPolygonEle.Symbol = pFillSymbol;
                IElement pEle = pPolygonEle as IElement;
                pEle.Geometry = pPolygon;
                pGraphicsContainer = m_ipMap as IGraphicsContainer;
                pGraphicsContainer.AddElement(pEle, 0);
                m_ipActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                drawshape = false;
            }
        }
        private void shapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawshape = true;
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            if (!isBorderOpen)
            {
                IActiveView pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                IMap pMap = pActiveView.FocusMap;
                IGraphicsContainer pGraphicsContainer = pActiveView as IGraphicsContainer;
                IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
                IStyleSelector pStyleSelector = new BorderSelector();
                if (pStyleSelector.DoModal(axPageLayoutControl1.hWnd))
                {
                    IBorder PBorder = pStyleSelector.GetStyle(0) as IBorder;
                    pMapFrame.Border = PBorder;
                }
                axPageLayoutControl1.Refresh(esriViewDrawPhase.esriViewBackground, null, null);
                isBorderOpen = true;
            }
            else
                isBorderOpen = false;
        }
        /*
                private void RoadAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    if(m_ipPathFinder==null)//打开几何网络工作空间
                     {
                        m_ipPathFinder = new RoadAnalysis();
                        IActiveView pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                        IMap pMap = pActiveView.FocusMap;
                        pLayer = pMap.get_Layer(0);
                        IFeatureLayer ipFeatureLayer = pLayer as IFeatureLayer;
                        IFeatureDataset ipFDB = ipFeatureLayer.FeatureClass.FeatureDataset;
                        m_ipPathFinder.SetOrGetMap = pMap;
                        m_ipPathFinder.OpenFeatureDatasetNetwork(ipFDB);
                    }
                    ispathanalysis = true;
                    //m_ipPathFinder.SolvePath("Weight");//先解析路径
                    //IPolyline ipPolyResult = m_ipPathFinder.PathPolyLine();//最后返回最短路径
                }*/

        //dataset

        /*
        /// <summary>

        /// 创建网络数据集对象

        /// </summary>

        /// <param name="featureDataset">包含网络数据集的空间要素集</param>

        /// <param name="NetworkName">网络数据集名称</param>

        /// <returns>边线网络数据集</returns>

        public IDENetworkDataset CreateNetworkDataset(IFeatureDataset featureDataset, string NetworkName)
        {
            if (string.IsNullOrEmpty(NetworkName) || null == featureDataset)
            {return null;}
            //定义边线网络数据集对象
            IDENetworkDataset deNetworkDataset = new DENetworkDatasetClass();
            // 转换为 IGeoDataset 接口
            IGeoDataset geoDataset = (IGeoDataset)featureDataset;
            // 设置数据集的空间参考和空间范围
            IDEGeoDataset deGeoDataset = (IDEGeoDataset)deNetworkDataset;
            deGeoDataset.Extent = geoDataset.Extent;
            deGeoDataset.SpatialReference = geoDataset.SpatialReference;
            // 设置名称
            IDataElement dataElement = (IDataElement)deNetworkDataset;
            dataElement.Name = NetworkName;
            // 设置为可创建
            deNetworkDataset.Buildable = true;
            //设置数据集类型
            deNetworkDataset.NetworkType = esriNetworkDatasetType.esriNDTGeodatabase;
            return deNetworkDataset;
        }
        
        public IDENetworkDataset CreateNetworkDataset(ILayer pLayer, string NetworkName)
        {
            if (string.IsNullOrEmpty(NetworkName) || null == pLayer)
            { return null; }
            //定义边线网络数据集对象
            IDENetworkDataset deNetworkDataset = new DENetworkDatasetClass();
            // 转换为 IGeoDataset 接口
            IGeoDataset geoDataset = (IGeoDataset)pLayer;
            // 设置数据集的空间参考和空间范围
            IDEGeoDataset deGeoDataset = (IDEGeoDataset)deNetworkDataset;
            deGeoDataset.Extent = geoDataset.Extent;
            deGeoDataset.SpatialReference = geoDataset.SpatialReference;
            // 设置名称
            IDataElement dataElement = (IDataElement)deNetworkDataset;
            dataElement.Name = NetworkName;
            // 设置为可创建
            deNetworkDataset.Buildable = true;
            //设置数据集类型
            deNetworkDataset.NetworkType = esriNetworkDatasetType.esriNDTGeodatabase;
            return deNetworkDataset;
        }

        /// <summary>

        /// 创建网络源对象

        /// </summary>

        /// <param name="FeatureClassName">参与网络数据集的空间要素类名称</param>

        /// <returns>源</returns>

        public INetworkSource CreateEdgeFeatureNetworkSource(string FeatureClassName)
        {
            INetworkSource pEdgeNetworkSource = new EdgeFeatureSourceClass();
            pEdgeNetworkSource.Name = FeatureClassName;
            //设置类型
            pEdgeNetworkSource.ElementType = esriNetworkElementType.esriNETEdge;
            return pEdgeNetworkSource;
        }

        /// <summary>

        /// 设置源的连通性,不使用字段值设置

        /// </summary>

        /// <param name="pEdgeNetworkSource">源对象</param>

        public void SetNetworkSourcewithoutSubtypes(INetworkSource pEdgeNetworkSource)
        {

            // 源的连通性

            IEdgeFeatureSource pEdgeFeatureSource = (IEdgeFeatureSource)pEdgeNetworkSource;

            //不使用子类

            pEdgeFeatureSource.UsesSubtypes = false;

            //分组

            pEdgeFeatureSource.ClassConnectivityGroup = 1;

            //使用节点参与

            pEdgeFeatureSource.ClassConnectivityPolicy = esriNetworkEdgeConnectivityPolicy.esriNECPEndVertex;

        }



        /// <summary>

        /// 设置源对象的方向

        /// </summary>

        /// <param name="StreetFieldName">道路属性名</param>

        /// <param name="EdgeNetworkSource">源对象</param>

        private void SetNetworkSourceDirections(string StreetFieldName, INetworkSource EdgeNetworkSource)
        {

            // 创建道路名字段类对象
            IStreetNameFields streetNameFields = new StreetNameFieldsClass();
            streetNameFields.Priority = 1;
            // 设置名称
            streetNameFields.StreetNameFieldName = StreetFieldName;
            //添加到集合中
            IArray nsdArray = new ArrayClass();
            nsdArray.Add(streetNameFields);
            //创建网络方向对象
            INetworkSourceDirections nsDirections = new NetworkSourceDirectionsClass();
            nsDirections.StreetNameFields = nsdArray;
            //设置源对象的网络方向
            EdgeNetworkSource.NetworkSourceDirections = nsDirections;
        }


        /// <summary>

        /// 网络权重属性设置，多个源参与同一个网络数据集属性的设置

        /// </summary>

        /// <param name=" SourceLst ">参与的所有源对象</param>

        /// <param name="AttributeName">属性名称</param>

        /// <param name="Expression">设置表达式</param>

        /// <param name="PreLogic">设置逻辑表达式，可空</param>

        /// <returns></returns>

        private IEvaluatedNetworkAttribute CreateNetworkSourceAttribute(List<INetworkSource> SourceLst, string AttributeName, string Expression, string PreLogic)
        {

            //定义变量
            IEvaluatedNetworkAttribute pEvalNetAttr;
            INetworkAttribute2 pNetAttr2;
            INetworkFieldEvaluator pNetFieldEval;
            INetworkConstantEvaluator pNetConstEval;
            pEvalNetAttr = new EvaluatedNetworkAttributeClass();
            pNetAttr2 = (INetworkAttribute2)pEvalNetAttr;
            pNetAttr2.Name = AttributeName;
            //计算类型
            pNetAttr2.UsageType = esriNetworkAttributeUsageType.esriNAUTCost;
            //数值类型     
            pNetAttr2.DataType = esriNetworkAttributeDataType.esriNADTDouble;
            //单位类型
            pNetAttr2.Units = esriNetworkAttributeUnits.esriNAUMeters;
            pNetAttr2.UseByDefault = true;
            //计算表达式
            pNetFieldEval = new NetworkFieldEvaluatorClass();
            pNetFieldEval.SetExpression(Expression, PreLogic);
            //参与的每个源的计算表达式设置
            SourceLst.ForEach(pEdgeNetworkSource =>
            {
                //正向计算表达式
                pEvalNetAttr.set_Evaluator(pEdgeNetworkSource, esriNetworkEdgeDirection.esriNEDAlongDigitized, (INetworkEvaluator)pNetFieldEval);
                //反向计算表达式
                pEvalNetAttr.set_Evaluator(pEdgeNetworkSource, esriNetworkEdgeDirection.esriNEDAgainstDigitized, (INetworkEvaluator)pNetFieldEval);
            });
            pNetConstEval = new NetworkConstantEvaluatorClass();
            pNetConstEval.ConstantValue = 0;
            //设置边，交汇点，转弯的默认值为常数
            pEvalNetAttr.set_DefaultEvaluator(esriNetworkElementType.esriNETEdge,
            (INetworkEvaluator)pNetConstEval);
            pEvalNetAttr.set_DefaultEvaluator(esriNetworkElementType.esriNETJunction,
            (INetworkEvaluator)pNetConstEval);
            pEvalNetAttr.set_DefaultEvaluator(esriNetworkElementType.esriNETTurn,
            (INetworkEvaluator)pNetConstEval);
            return pEvalNetAttr;
        }

        /// <summary>

        /// 指定网络数据集的方向属性

        /// </summary>

        /// <param name="deNetworkDataset">网络数据集</param>

        /// <param name="UnitsType">单位类型</param>

        /// <param name="LengthAttribute"> 创建的长度属性的名称</param>

        /// <param name="TimeAttribute"> 创建的时间属性名称，可空</param>

        /// <param name="RoadClassAttribute">创建的道路类型属性名称，可空</param>

        public void SetNetworkDirction(IDENetworkDataset deNetworkDataset, esriNetworkAttributeUnits UnitsType, string LengthAttribute, string TimeAttribute, string RoadClassAttribute)
        {

            // 创建网络方向对象

            INetworkDirections networkDirections = new NetworkDirectionsClass();
            networkDirections.DefaultOutputLengthUnits = UnitsType;
            //设置长度属性

            if (!string.IsNullOrEmpty(LengthAttribute))
            {
                networkDirections.LengthAttributeName = LengthAttribute;
            }

            //设置时间属性

            if (!string.IsNullOrEmpty(TimeAttribute))
            {
                networkDirections.TimeAttributeName = TimeAttribute;
            }
            //设置道路类型属性
            if (!string.IsNullOrEmpty(RoadClassAttribute))
            {
                networkDirections.RoadClassAttributeName = RoadClassAttribute;
            }
            // 设置网络数据集的方向属性
            deNetworkDataset.Directions = networkDirections;
        }

        /// <summary>

　　/// 根据网络节点信息,创建网络数据集对象

　　/// </summary>

　　/// <param name="_pFeatureDataset">包含网络数据集的空间数据集</param>

　　/// <param name="_pDENetDataset">源网络</param>

　　/// <returns></returns>

　　public INetworkDataset CreateBuildingDataset(IFeatureDataset _pFeatureDataset, IDENetworkDataset2 _pDENetDataset)

　　{

                   IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer =  (IFeatureDatasetExtensionContainer)_pFeatureDataset;

                   IFeatureDatasetExtension pFeatureDatasetExtension =  pFeatureDatasetExtensionContainer.FindExtension(esriDatasetType.esriDTNetworkDataset);

                   IDatasetContainer2 pDatasetContainer2 =  (IDatasetContainer2)pFeatureDatasetExtension;

                   IDEDataset pDENetDataset = (IDEDataset)_pDENetDataset;
                   //创建网络数据集

                   INetworkDataset pNetworkDataset =  (INetworkDataset)pDatasetContainer2.CreateDataset(pDENetDataset);
                   return pNetworkDataset;

　　}

 

　　/// <summary>

　　/// 生成网络数据集

　　/// </summary>

　　/// <param name="networkDataset">网络数据集</param>

　　/// <param name="geoDataset">空间数据集</param>

　　public bool BuildNetwork(INetworkDataset networkDataset, IFeatureDataset featureDataset)

　　{

             // 空间数据集转换为IGeoDataset 接口

            IGeoDataset geoDataset = (IGeoDataset)featureDataset;

             if (null==geoDataset)

              {return false;                }
            INetworkBuild networkBuild = (INetworkBuild)networkDataset;

           //构建网络数据集

           networkBuild.BuildNetwork(geoDataset.Extent);
      return true;

　　}*/

        /// <summary>
        /// 创建网络数据集
        /// </summary>
        /// <param name="featureClassPath">gdb文件的featureclass全路径</param>
        /// <param name="netName">生成的网络数据集名称，可以为中文</param>
        /// <returns></returns>
        private void buildDatasetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
             string layer = "";
                //  Microsoft.Win32.OpenFileDialog OpenMXD = new Microsoft.Win32.OpenFileDialog();
                  OpenFileDialog OpenWS = new OpenFileDialog(); 
                  OpenWS.Title = "打开图层";
                  OpenWS.InitialDirectory = "C:\\Users\\Administrator\\Desktop\\作业\\空间分析\\实习walkscore\\walkscoredata";
                  OpenWS.Filter = "Map Documents (*.shp)|*.shp";
                  if (OpenWS.ShowDialog() == DialogResult.OK)
                  {
                      layer = OpenWS.FileName;
                  }
            //FileGDBWorkspaceFactory pWSF = new FileGDBWorkspaceFactory();
            //IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)pWSF.OpenFromFile(Workspace,0);
          //  IFeatureDataset featureDataset = featureWorkspace.OpenFeatureDataset("street");
                  IDENetworkDataset pDENetworkDataset = CreateNetworkDataset(pLayer, "streetnetds");
            //IDENetworkDataset =CreateEdgeFeatureNetworkSource("test");
            pDENetworkDataset.SupportsTurns = true;//支持turn
            INetworkSource networkSource;*/
            string featureClassPath = "C:\\Users\\Administrator\\Desktop\\作业\\空间分析\\实习walkscore\\walkscoredata\\walkability.gdb\\test\\streetf";
            string netName = "street_test";
            string featureClassName = featureClassPath.Substring(featureClassPath.LastIndexOf('\\') + 1);
            string datasetPath = featureClassPath.Substring(0, featureClassPath.LastIndexOf('\\'));
            string datasetName = datasetPath.Substring(datasetPath.LastIndexOf('\\') + 1);
            string gdbPath = datasetPath.Substring(0, datasetPath.LastIndexOf('\\'));

            IWorkspaceFactory pWorkspceFac = new FileGDBWorkspaceFactory();
            IWorkspace pWorkspace = pWorkspceFac.OpenFromFile(gdbPath, 0);
            IFeatureWorkspace featureWorkspace = pWorkspace as IFeatureWorkspace;

            IFeatureDataset pFeatureDataset;//数据源数据集
            IFeatureClass pFeatureClass = null;//数据源要素类

            pFeatureDataset = featureWorkspace.OpenFeatureDataset(datasetName);
            IFeatureClassContainer container = pFeatureDataset as IFeatureClassContainer;
            pFeatureClass = container.get_ClassByName(featureClassName);//不能再打开数据的情况下get
            if (pFeatureDataset != null)
            {
                IDENetworkDataset pDENetworkDataset = CreateNetworkDataset(pFeatureDataset, netName);
                pDENetworkDataset.SupportsTurns = true;//支持turn
                INetworkSource networkSource;
                if (pFeatureClass != null)
                {
                    networkSource = CreateEdgeFeatureNetworkSource(((IDataset)pFeatureClass).Name);
                    SetNetworkSourceWithoutSubtype(networkSource);
                    SetNetworkDatasetDirections("NAME", networkSource);

                    List<INetworkSource> sourceList = new List<INetworkSource>();
                    sourceList.Add(networkSource);
                    IEvaluatedNetworkAttribute networkAttribute = CreateNetworkSourceAttribute(sourceList, "Length", "[Shape]", "");

                    SetNetworkDirection(pDENetworkDataset, esriNetworkAttributeUnits.esriNAUMiles, "Length", "", "");

                    IArray array = new ArrayClass();
                    array.Add(networkAttribute);
                    pDENetworkDataset.Attributes = array;

                    IArray arraySource = new ArrayClass();
                    arraySource.Add(networkSource);
                    pDENetworkDataset.Sources = arraySource;

                    bool success = CreateBuildingDataset(pFeatureDataset, pDENetworkDataset as IDENetworkDataset2);
                }
            }
        }

        /// <summary>
        /// 创建DE网络数据集
        /// </summary>
        /// <param name="pFeatureDataset">源数据集</param>
        /// <param name="strNetWorkName">输出网络数据集名称</param>
        /// <returns>网络数据集</returns>
        private IDENetworkDataset CreateNetworkDataset(IFeatureDataset pFeatureDataset, string strNetWorkName)
        {
            if (string.IsNullOrEmpty(strNetWorkName) || pFeatureDataset == null)
            {
                return null;
            }
            //设置范围和空间参考
            IDENetworkDataset deNetworkDataset = new DENetworkDatasetClass();
            IGeoDataset geoDataset = pFeatureDataset as IGeoDataset;
            IDEGeoDataset deGeoDataset = deNetworkDataset as IDEGeoDataset;

            deGeoDataset.Extent = geoDataset.Extent;
            deGeoDataset.SpatialReference = geoDataset.SpatialReference;

            //设置名称
            IDataElement dataElement = deNetworkDataset as IDataElement;
            dataElement.Name = strNetWorkName;
            //设置可创建
            deNetworkDataset.Buildable = true;
            //设置源类型
            deNetworkDataset.NetworkType = esriNetworkDatasetType.esriNDTGeodatabase;

            return deNetworkDataset;
        }

        /// <summary>
        /// 创建网络源对象
        /// </summary>
        /// <param name="strFeatureClassName"></param>
        /// <returns></returns>
        private INetworkSource CreateEdgeFeatureNetworkSource(string strFeatureClassName)
        {
            INetworkSource edgeNetworkSource = new EdgeFeatureSourceClass();

            edgeNetworkSource.Name = strFeatureClassName;

            //类型
            edgeNetworkSource.ElementType = esriNetworkElementType.esriNETEdge;

            return edgeNetworkSource;
        }

        /// <summary>
        /// 设置源连通性，不使用字段设置
        /// </summary>
        /// <param name="pNetworkSource"></param>
        private void SetNetworkSourceWithoutSubtype(INetworkSource pNetworkSource)
        {
            IEdgeFeatureSource edgeFeatureSource = pNetworkSource as IEdgeFeatureSource;
            //不使用子类
            edgeFeatureSource.UsesSubtypes = false;
            //分组
            edgeFeatureSource.ClassConnectivityGroup = 1;
            //使用终节点参与
            edgeFeatureSource.ClassConnectivityPolicy = esriNetworkEdgeConnectivityPolicy.esriNECPEndVertex;
        }


        /// <summary>
        /// 设置源Direcition
        /// </summary>
        /// <param name="pStreetNameFieldName">道路属性名(默认为NAME)</param>
        /// <param name="pNetworkSource"></param>
        private void SetNetworkDatasetDirections(string pStreetNameFieldName, INetworkSource pNetworkSource)
        {
            IStreetNameFields streetNameFields = new StreetNameFieldsClass();
            streetNameFields.Priority = 1;
            //设置名称
            streetNameFields.StreetNameFieldName = pStreetNameFieldName;
            //添加到集合中
            IArray nsdArray = new ArrayClass();
            nsdArray.Add(streetNameFields);
            //创建网络方向对象
            INetworkSourceDirections nsDirection = new NetworkSourceDirectionsClass();
            nsDirection.StreetNameFields = nsdArray;

            pNetworkSource.NetworkSourceDirections = nsDirection;
        }

        /// <summary>
        /// 网络权重属性设置，多个源参与同一个网络数据集属性设置
        /// </summary>
        /// <param name="sourceList">源对象集合</param>
        /// <param name="strAttributeName">属性名称</param>
        /// <param name="strExpression">表达式</param>
        /// <param name="preLogic">逻辑表达式，可空</param>
        /// <returns></returns>
        private IEvaluatedNetworkAttribute CreateNetworkSourceAttribute(List<INetworkSource> sourceList, string strAttributeName, string strExpression, string preLogic)
        {
            IEvaluatedNetworkAttribute pEvaluateNetworkAttribute;
            INetworkAttribute2 pNetworkAttribute;
            INetworkFieldEvaluator pNetFieldEval;
            INetworkConstantEvaluator pNetConsEval;

            pEvaluateNetworkAttribute = new EvaluatedNetworkAttributeClass();
            pNetworkAttribute = pEvaluateNetworkAttribute as INetworkAttribute2;
            pNetworkAttribute.Name = strAttributeName;
            //设置属性
            pNetworkAttribute.UsageType = esriNetworkAttributeUsageType.esriNAUTCost;
            pNetworkAttribute.DataType = esriNetworkAttributeDataType.esriNADTDouble;
            pNetworkAttribute.Units = esriNetworkAttributeUnits.esriNAUMeters;
            pNetworkAttribute.UseByDefault = true;

            //计算表达式
            pNetFieldEval = new NetworkFieldEvaluatorClass();
            //INetworkEvaluator networkEvaluator = pNetFieldEval as INetworkEvaluator;
            pNetFieldEval.SetExpression(strExpression, preLogic);

            //参与的每个源的计算表达式设置
            sourceList.ForEach(pEdgeNetworkSource =>
            {
                //正向计算表达式
                pEvaluateNetworkAttribute.set_Evaluator(pEdgeNetworkSource, esriNetworkEdgeDirection.esriNEDAlongDigitized, (INetworkEvaluator)pNetFieldEval);
                //反向计算表达式pEvaluateNetworkAttribute.set_Evaluator(pEdgeNetworkSource, esriNetworkEdgeDirection.esriNEDAgainstDigitized, (INetworkEvaluator)pNetFieldEval);
            });

            pNetConsEval = new NetworkConstantEvaluatorClass();
            pNetConsEval.ConstantValue = 0;

            //设置边，交汇点和转弯默认值为常数
            pEvaluateNetworkAttribute.set_DefaultEvaluator(esriNetworkElementType.esriNETEdge, (INetworkEvaluator)pNetConsEval);
            pEvaluateNetworkAttribute.set_DefaultEvaluator(esriNetworkElementType.esriNETJunction, (INetworkEvaluator)pNetConsEval);
            pEvaluateNetworkAttribute.set_DefaultEvaluator(esriNetworkElementType.esriNETTurn, (INetworkEvaluator)pNetConsEval);

            return pEvaluateNetworkAttribute;
        }


        /// <summary>
        /// 指定网络数据集的方向属性
        /// </summary>
        /// <param name="deNetworkDataset">源</param>
        /// <param name="unitType">单位类型</param>
        /// <param name="strLengthAttribute">长度属性名称</param>
        /// <param name="strTimeAttribute">时间属性名称，可空</param>
        /// <param name="strRoadClassAttribute">创建道路类型属性名称，可空</param>
        private void SetNetworkDirection(IDENetworkDataset deNetworkDataset, esriNetworkAttributeUnits unitType, string strLengthAttribute, string strTimeAttribute, string strRoadClassAttribute)
        {
            //创建网络方向对象
            INetworkDirections netDirections = new NetworkDirectionsClass();
            netDirections.DefaultOutputLengthUnits = unitType;

            if (!string.IsNullOrEmpty(strLengthAttribute))
            {
                netDirections.LengthAttributeName = strLengthAttribute;
            }
            if (!string.IsNullOrEmpty(strTimeAttribute))
            {
                netDirections.TimeAttributeName = strTimeAttribute;
            }
            if (!string.IsNullOrEmpty(strRoadClassAttribute))
            {
                netDirections.RoadClassAttributeName = strRoadClassAttribute;
            }

            deNetworkDataset.Directions = netDirections;
        }

        /// <summary>
        /// 根据网络节点信息，创建网络数据集对象
        /// </summary>
        /// <param name="pFeatureDataset">源数据集</param>
        /// <param name="pDENetworkDataset2">源网络</param>
        /// <returns></returns>
        private bool CreateBuildingDataset(IFeatureDataset pFeatureDataset, IDENetworkDataset2 pDENetworkDataset2)
        {
            IFeatureDatasetExtensionContainer pExtensionContainer = pFeatureDataset as IFeatureDatasetExtensionContainer;
            IFeatureDatasetExtension pExtention = pExtensionContainer.FindExtension(esriDatasetType.esriDTNetworkDataset);

            IDatasetContainer2 pContainer2 = pExtention as IDatasetContainer2;
            IDEDataset pDEDataset = pDENetworkDataset2 as IDEDataset;

            //创建
            INetworkDataset pNetworkDataset = null;
            try
            {
                pNetworkDataset = pContainer2.CreateDataset(pDEDataset) as INetworkDataset;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("0x80040301"))
                {
                    MessageBox.Show("没有找到数据集！");
                }
                else if (ex.Message.Contains("0x80042267"))
                {
                    MessageBox.Show("已经存在网络数据集！");
                }
                else
                    MessageBox.Show(ex.Message);
                return false;
            }

            INetworkBuild build = pNetworkDataset as INetworkBuild;
            IEnvelope envelope = build.BuildNetwork(((IGeoDataset)pFeatureDataset).Extent);
            return true;
        }

        private void roadNetAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetAnalysisDT dtnetanalysis = new NetAnalysisDT();
            dtnetanalysis.Show();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formtest test1 = new Formtest();
            test1.Show();
        }

        private void coloTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloloTest ctest = new CloloTest();
            ctest.Show();
        }

        private void mapsTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapsTest maptest = new MapsTest();
            maptest.Show();
        }

        private void DrawCircle()
        {
            if (drawcircle)
            {
                m_ipActiveView = axMapControl1.ActiveView;
                m_ipMap = axMapControl1.ActiveView.FocusMap;
                IScreenDisplay pScreenDisplay = m_ipActiveView.ScreenDisplay;
                IRubberBand pRubberCircle = new RubberCircleClass();
                ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = getRGB(255, 255, 0);
                IGeometry pCircle = pRubberCircle.TrackNew(pScreenDisplay, (ISymbol)pFillSymbol) as IGeometry;

                IPolygon pPolygon = new PolygonClass();　　　　//空的多边形
                ISegmentCollection pSegmentCollection = pPolygon as ISegmentCollection;　　//段集合
                ISegment pSegment = pCircle as ISegment;　　//将圆赋值给段
                object missing = Type.Missing;　　//显示默认值
                pSegmentCollection.AddSegment(pSegment, ref missing, ref missing);　　//给空多边形加入圆
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                pFillSymbol.Color = getRGB(0, 255, 255);
                IFillShapeElement pPolygonEle = new PolygonElementClass();
                pPolygonEle.Symbol = pFillSymbol;
                IElement pEle = pPolygonEle as IElement;
                pEle.Geometry = pPolygon;
                pGraphicsContainer = m_ipMap as IGraphicsContainer;
                pGraphicsContainer.AddElement(pEle, 0);
                m_ipActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                drawcircle = false;
            }
        }
        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawcircle = true;

        }

        private void bufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (true)
            {
                pGraphicsContainer = pMap as IGraphicsContainer;    //定义容器
                pFeature = pEnumFeature.Next();     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    break;
                pGeometry = pFeature.Shape;     //获取要素的Geometry
                ITopologicalOperator pTopoOperator = pGeometry as ITopologicalOperator; //QI到拓扑操作
                IGeometry pBufferGeo = pTopoOperator.Buffer(10);     //缓冲区分析

                IElement pElement = new PolygonElement();
                pElement.Geometry = pBufferGeo;     //获取得到的缓冲区

                pGraphicsContainer.AddElement(pElement, 0); //显示缓冲区
                axMapControl1.ActiveView.Refresh();
            }
            isspatialana = false;
        }

        private void boundaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (true)
            {
                pGraphicsContainer = pMap as IGraphicsContainer;    //定义容器
                pFeature = pEnumFeature.Next();     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    break;
                pGeometry = pFeature.Shape;     //获取要素的Geometry
                ITopologicalOperator pTopoOperator = pGeometry as ITopologicalOperator; //QI到拓扑操作
                IGeometry pBoundary = pTopoOperator.Boundary;  //获取边界

                ILineElement pLineEle = new LineElementClass();
                ISimpleLineSymbol pSLS = new SimpleLineSymbol();
                IRgbColor pColor = getRGB(0, 255, 0) as IRgbColor;
                pSLS.Color = pColor;
                pSLS.Width = 5;
                pLineEle.Symbol = pSLS;

                IElement pElement = pLineEle as IElement;
                pElement.Geometry = pBoundary;

                pGraphicsContainer.AddElement(pElement, 0); //显示边界
                pActiveView.Refresh();
            }
            isspatialana = false;
        }

        private void convexHullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (true)
            {
                pGraphicsContainer = pMap as IGraphicsContainer;    //定义容器
                pFeature = pEnumFeature.Next();     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    break;
                pGeometry = pFeature.Shape;     //获取要素的Geometry
                ITopologicalOperator pTopoOperator = pGeometry as ITopologicalOperator; //QI到拓扑操作
                IGeometry pBufferGeo = pTopoOperator.ConvexHull();

                IElement pElement = new PolygonElement();
                pElement.Geometry = pBufferGeo;     //获取得到的缓冲区

                pGraphicsContainer.AddElement(pElement, 0); //显示缓冲区
                pActiveView.Refresh();
            }
            isspatialana = false;

        }

        private void isClipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClip = true;
        }

        private void clipToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            while (true)
            {
                pGraphicsContainer = pMap as IGraphicsContainer;    //定义容器
                pFeature = pEnumFeature.Next();     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    break;
                pGeometry = pFeature.Shape;     //获取要素的Geometry
                ITopologicalOperator pTopoOperator = pGeometry as ITopologicalOperator; //QI到拓扑操作
                pTopoOperator.Clip(pEnvClip);

                IElement pElement = new PolygonElement();
                pElement.Geometry = pGeometry;     //获取得到的缓冲区

                pGraphicsContainer.AddElement(pElement, 0); //显示缓冲区
                pActiveView.Refresh();
            }
            isspatialana = false;

        }

        private void isCutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCut = true;
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            while (true)
            {
                pGraphicsContainer = pMap as IGraphicsContainer;    //定义容器
                pFeature = pEnumFeature.Next();     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    break;
                pGeometry = pFeature.Shape;     //获取要素的Geometry
                ITopologicalOperator pTopoOperator = pGeometry as ITopologicalOperator; //QI到拓扑操作ry
                IGeometry pGeoRight = new PolygonClass();
                IGeometry pGeoLeft = new PolygonClass();
                pTopoOperator.Cut(pLineCut, out pGeoLeft, out pGeoRight);

                IElement pElement = new PolygonElement();

                IFillShapeElement pFillEle = pElement as IFillShapeElement;
                ISimpleFillSymbol pSFS = new SimpleFillSymbol();
                pSFS.Color = getRGB(255, 255, 0);
                pFillEle.Symbol = pSFS;

                pElement.Geometry = pGeoLeft;     //获取得到的缓冲区
                pGraphicsContainer.AddElement(pElement, 0); //显示缓冲区

                pSFS = new SimpleFillSymbol();
                pSFS.Color = getRGB(180, 0, 180);
                pFillEle.Symbol = pSFS;

                pElement = new PolygonElement();
                pElement.Geometry = pGeoRight;     //获取得到的缓冲区
                pGraphicsContainer.AddElement(pElement, 0); //显示缓冲区

                m_ipActiveView.Refresh();
            }
            isspatialana = false;
        }

        private void unionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGeometry pUnionGeo = new PolygonClass();
            pGraphicsContainer = pMap as IGraphicsContainer;    //定义容器

            while (true)
            {
                pFeature = pEnumFeature.Next();     //遍历要素
                if (pFeature == null)            //若不存在要素，则推出循环
                    break;
                pGeometry = pFeature.Shape;     //获取要素的Geometry
                ITopologicalOperator pTopoOperator = pUnionGeo as ITopologicalOperator; //QI到拓扑操作ry
                pUnionGeo = pTopoOperator.Union(pGeometry);
            }

            IElement pElement = new PolygonElement();

            IFillShapeElement pFillEle = pElement as IFillShapeElement;
            ISimpleFillSymbol pSFS = new SimpleFillSymbol();
            pSFS.Color = getRGB(255, 255, 0);
            pFillEle.Symbol = pSFS;

            pElement.Geometry = pUnionGeo;     //获取得到的缓冲区
            pGraphicsContainer.AddElement(pElement, 0); //显示缓冲区

            pActiveView.Refresh();
            isspatialana = false;

        }

        private void isFirstInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isFirstIn = true;
        }

        private void intersectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isFirstIn = false;
            pFirstPolygon = new PolygonClass();
            while (true)
            {
                pFeature = pEnumFeature.Next();
                if (pFeature == null)
                    break;
                pGeometry = pFeature.Shape;
                ITopologicalOperator pTopoOperator = pFirstPolygon as ITopologicalOperator;
                pFirstPolygon = pTopoOperator.Union(pGeometry) as IPolygon;
            }
        }

        private void whatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGeometry pIntersectGeo = new PolygonClass();
            IGeometry pSecondPolygon = new PolygonClass();
            pGraphicsContainer = pMap as IGraphicsContainer;

            while (true)
            {
                pFeature = pEnumFeature.Next();
                if (pFeature == null)
                    break;
                pGeometry = pFeature.Shape;
                ITopologicalOperator pTopoOperator = pSecondPolygon as ITopologicalOperator;
                pSecondPolygon = pTopoOperator.Union(pGeometry) as IPolygon;
            }

            ITopologicalOperator pTopo = pSecondPolygon as ITopologicalOperator;
            pIntersectGeo = pTopo.Intersect(pFirstPolygon, esriGeometryDimension.esriGeometry2Dimension) as IPolygon;
            IElement pElement = new PolygonElementClass();
            pElement.Geometry = pIntersectGeo;
            pGraphicsContainer.AddElement(pElement, 0);
            IFeatureLayer pFeatureLayer = pMap.get_Layer(0) as IFeatureLayer;
            pFeatureLayer.Visible = false;
            pFeatureLayer = pMap.get_Layer(1) as IFeatureLayer;
            pFeatureLayer.Visible = false;
            pActiveView.Refresh();
            isspatialana = false;

        }

        private void differenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGeometry pIntersectGeo = new PolygonClass();
            IGeometry pSecondPolygon = new PolygonClass();
            pGraphicsContainer = pMap as IGraphicsContainer;

            while (true)
            {
                pFeature = pEnumFeature.Next();
                if (pFeature == null)
                    break;
                pGeometry = pFeature.Shape;
                ITopologicalOperator pTopoOperator = pSecondPolygon as ITopologicalOperator;
                pSecondPolygon = pTopoOperator.Union(pGeometry) as IPolygon;
            }

            ITopologicalOperator pTopo = pSecondPolygon as ITopologicalOperator;
            pIntersectGeo = pTopo.Difference(pFirstPolygon) as IPolygon;
            IElement pElement = new PolygonElementClass();
            pElement.Geometry = pIntersectGeo;
            pGraphicsContainer.AddElement(pElement, 0);
            IFeatureLayer pFeatureLayer = pMap.get_Layer(0) as IFeatureLayer;
            pFeatureLayer.Visible = false;
            pFeatureLayer = pMap.get_Layer(1) as IFeatureLayer;
            pFeatureLayer.Visible = false;
            pActiveView.Refresh();
            isspatialana = false;
        }

        private void spatialAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isspatialana = true;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pMap.ClearSelection();
            pGraphicsContainer.DeleteAllElements();
            pActiveView.Refresh();
            isspatialana = false;

        }

        ///
        /// 检测几何图形A是否包含几何图形B
        ///
        /// 几何图形A
        /// 几何图形B
        /// True为包含，False为不包含
        private bool CheckGeometryContain(IGeometry pGeometryA, IGeometry pGeometryB)
        {
            IRelationalOperator pRelOperator = pGeometryA as IRelationalOperator;
            if (pRelOperator.Contains(pGeometryB))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// 在pGeometry上返回一个离pInputPoint最近的point
        ///
        /// 给定的点对象
        /// 要查询的几何图形
        /// the nearest Point
        private IPoint NearestPoint(IPoint pInputPoint, IGeometry pGeometry)
        {
            try
            {
                IProximityOperator pProximity = (IProximityOperator)pGeometry;
                IPoint pNearestPoint = pProximity.ReturnNearestPoint(pInputPoint, esriSegmentExtension.esriNoExtension);
                return pNearestPoint;
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }

        ///
        /// 获取两个几何图形的距离
        ///
        /// 几何图形A
        /// 几何图形B
        /// 两个几何图形的距离
        private double GetTwoGeometryDistance(IGeometry pGeometryA, IGeometry pGeometryB)
        {
            IProximityOperator pProOperator = pGeometryA as IProximityOperator;
            if (pGeometryA != null || pGeometryB != null)
            {
                double distance = pProOperator.ReturnDistance(pGeometryB);
                return distance;
            }
            else
            {
                return 0;
            }
        }

        private void containerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckGeometryContain(pGeometryA, pGeometryB))
                MessageBox.Show("Contain"); //输出信息
            else
                MessageBox.Show("NotContain"); //输出信息
        }

        private void firstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isfirstcontainer = true;
        }

        private void secondToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            issecondcontainer = true;
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pMap.ClearSelection();
            pGraphicsContainer.DeleteAllElements();
            pActiveView.Refresh();
        }

        private void spatialQueryFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //初始化空间查询窗体  
            spatialqueryForm pspatialQueryForm = new spatialqueryForm(axMapControl1);
            if (pspatialQueryForm.ShowDialog() == DialogResult.OK)
            {
                this.mTool = "SpatialQuery";
                //获取查询方式和图层信息  
                this.mQueryModel = pspatialQueryForm.mQueryModel;
                this.mLayerIndex = pspatialQueryForm.mLayerIndex;
                this.issqform = pspatialQueryForm.issqform;
                //设置鼠标形状  
                this.axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerCrosshair;
            }
        }
        private DataTable LoadQueryResult(ESRI.ArcGIS.Controls.AxMapControl mapControl, ESRI.ArcGIS.Carto.IFeatureLayer featureLayer, ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            ESRI.ArcGIS.Geodatabase.IFeatureClass pFeatureClass = featureLayer.FeatureClass;
            //根据图层属性字段初始化DataTable  
            ESRI.ArcGIS.Geodatabase.IFields pFields = pFeatureClass.Fields;
            DataTable pDataTable = new DataTable();
            for (int i = 0; i < pFields.FieldCount; ++i)
            {
                pDataTable.Columns.Add(pFields.get_Field(i).AliasName);
            }
            //空间过滤器  
            ESRI.ArcGIS.Geodatabase.ISpatialFilter pSpatialFilter = new ESRI.ArcGIS.Geodatabase.SpatialFilterClass();
            pSpatialFilter.Geometry = geometry;
            //根据图层类型选择缓冲方式  
            switch (pFeatureClass.ShapeType)
            {
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint:
                    pSpatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    pSpatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    pSpatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }
            //定义空间过滤器的空间字段  
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
            ESRI.ArcGIS.Geodatabase.IQueryFilter pQueryFilter;
            ESRI.ArcGIS.Geodatabase.IFeatureCursor pFeatureCursor;
            ESRI.ArcGIS.Geodatabase.IFeature pFeature;
            //利用要素过滤器查询要素  
            pQueryFilter = pSpatialFilter as ESRI.ArcGIS.Geodatabase.IQueryFilter;
            pFeatureCursor = featureLayer.Search(pQueryFilter, true);
            pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                string strFldValue = null;
                DataRow dr = pDataTable.NewRow();
                //遍历图层属性表字段值，并加入pDataTable  
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    string strFldName = pFields.get_Field(i).Name;
                    if (strFldName == "Shape")
                    {
                        strFldValue = Convert.ToString(pFeature.Shape.GeometryType);
                    }
                    else
                        strFldValue = Convert.ToString(pFeature.get_Value(i));
                    dr[i] = strFldValue;
                }
                pDataTable.Rows.Add(dr);
                //高亮选择要素  
                mapControl.Map.SelectFeature((ESRI.ArcGIS.Carto.ILayer)featureLayer, pFeature);
                mapControl.ActiveView.Refresh();
                pFeature = pFeatureCursor.NextFeature();
            }

            return pDataTable;

        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = false;
            this.axMapControl1.ActiveView.FocusMap.ClearSelection();
            this.axMapControl1.Refresh();
            issqform = false;
        }

        private void stopPanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pan = null;
        }

        private void distanceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double tempt;
            tempt = GetTwoGeometryDistance(pGeometryA, pGeometryB);
            MessageBox.Show(tempt.ToString());
        }

        private void nearestToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IPoint tempt = NearestPoint(pGeometryA as IPoint, pGeometryB);
            IArray pIDArray;
            IIdentifyObj pIdObj;
            IIdentify pIdentify = axMapControl1.Map.get_Layer(0) as IIdentify; //通过图层获取 IIdentify 实例


            //    pIDArray = pIdentify.Identify(tempt);       //通过点获取数组，用点一般只能选择一个元素
            //    string str = "\n";
            //    string lyrName = "";
            //    if (pIDArray != null)
            //    {
            //        pIdObj = pIDArray.get_Element(0) as IIdentifyObj; //取得要素
            //        pIdObj.Flash(axMapControl1.ActiveView.ScreenDisplay);       //闪烁效果
            //        str += pIdObj.Name + "\n";
            //        lyrName = pIdObj.Layer.Name;

            //        pSelectionEnv = new SelectionEnvironment(); //新建选择环境
            //        axMapControl1.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            //        pEnumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
            //        //IGeometry pGeometry = axMapControl1.TrackRectangle();       //获取框选几何
            //        IRgbColor pColor = new RgbColor();
            //        pColor.Red = 255;
            //        pSelectionEnv.DefaultColor = pColor;         //设置高亮显示的颜色！

            //        pMap.SelectByShape(tempt, pSelectionEnv, false); //选择图形！

            //        axMapControl1.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            //    }
            //            MessageBox.Show("Layer: " + lyrName + "\n" + "Feature: " + str);

            //}


        }

        #region 添加制图要素1
        //图例
        private void addLegendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enumMapSurType = EnumMapSurroundType.Legend;
        }
        //指北针
        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _enumMapSurType = EnumMapSurroundType.NorthArrow;
            if (frmSym == null || frmSym.IsDisposed)
            {
                frmSym = new frmSymbol();
                frmSym.GetSelSymbolItem += new frmSymbol.GetSelSymbolItemEventHandler(frmSym_GetSelSymbolItem);
            }
            frmSym.EnumMapSurType = _enumMapSurType;
            frmSym.InitUI();
            frmSym.ShowDialog();
        }
        private void frmSym_GetSelSymbolItem(ref IStyleGalleryItem pStyleItem)
        {
            pStyleGalleryItem = pStyleItem;
        }
        //比例尺
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enumMapSurType = EnumMapSurroundType.ScaleBar;
            if (frmSym == null || frmSym.IsDisposed)
            {
                frmSym = new frmSymbol();
                frmSym.GetSelSymbolItem += new frmSymbol.GetSelSymbolItemEventHandler(frmSym_GetSelSymbolItem);
            }
            frmSym.EnumMapSurType = _enumMapSurType;
            frmSym.InitUI();
            frmSym.ShowDialog();
        }
        #endregion

        #region 公共方法（其他）
        private string GetMapUnit(esriUnits _esriMapUnit)   //地图单位获取 
        {
            string sMapUnits = string.Empty;
            switch (_esriMapUnit)
            {
                case esriUnits.esriCentimeters:
                    sMapUnits = "厘米";
                    break;
                case esriUnits.esriDecimalDegrees:
                    sMapUnits = "十进制";
                    break;
                case esriUnits.esriDecimeters:
                    sMapUnits = "分米";
                    break;
                case esriUnits.esriFeet:
                    sMapUnits = "尺";
                    break;
                case esriUnits.esriInches:
                    sMapUnits = "英寸";
                    break;
                case esriUnits.esriKilometers:
                    sMapUnits = "千米";
                    break;
                case esriUnits.esriMeters:
                    sMapUnits = "米";
                    break;
                case esriUnits.esriMiles:
                    sMapUnits = "英里";
                    break;
                case esriUnits.esriMillimeters:
                    sMapUnits = "毫米";
                    break;
                case esriUnits.esriNauticalMiles:
                    sMapUnits = "海里";
                    break;
                case esriUnits.esriPoints:
                    sMapUnits = "点";
                    break;
                case esriUnits.esriUnitsLast:
                    sMapUnits = "UnitsLast";
                    break;
                case esriUnits.esriUnknownUnits:
                    sMapUnits = "未知单位";
                    break;
                case esriUnits.esriYards:
                    sMapUnits = "码";
                    break;
                default:
                    break;
            }
            return sMapUnits;
        }
        #endregion


        #region 添加制图要素2 布局视图点击
        private void axPageLayoutControl1_OnMouseDown(object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {
            if (_enumMapSurType != EnumMapSurroundType.None)
            {
                IActiveView pActiveView = null;
                pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                m_PointPt = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                if (pNewEnvelopeFeedback == null)
                {
                    pNewEnvelopeFeedback = new NewEnvelopeFeedbackClass();
                    pNewEnvelopeFeedback.Display = pActiveView.ScreenDisplay;
                    pNewEnvelopeFeedback.Start(m_PointPt);
                }
                else
                {
                    pNewEnvelopeFeedback.MoveTo(m_PointPt);
                }
            }
        }

        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            if (_enumMapSurType != EnumMapSurroundType.None)
            {
                if (pNewEnvelopeFeedback != null)
                {
                    m_MovePt = (axPageLayoutControl1.PageLayout as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                    pNewEnvelopeFeedback.MoveTo(m_MovePt);
                }
            }

        }

        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)  //地图双击 
        {
            #region 长度量算
            if (pMouseOperate == "MeasureLength")
            {
                if (frmMeasureResult != null)
                {
                    frmMeasureResult.lblMeasureResult.Text = "线段总长度为：" + dToltalLength + pMapUnits;
                }
                if (pNewLineFeedback != null)
                {
                    pNewLineFeedback.Stop();
                    pNewLineFeedback = null;
                    //清空所画的线对象
                    (axMapControl1.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                }
                dToltalLength = 0;
                dSegmentLength = 0;
            }
            #endregion
            #region 面积量算
            if (pMouseOperate == "MeasureArea")
            {
                if (pNewPolygonFeedback != null)
                {
                    pNewPolygonFeedback.Stop();
                    pNewPolygonFeedback = null;
                    //清空所画的线对象
                    (axMapControl1.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                }
                pAreaPointCol.RemovePoints(0, pAreaPointCol.PointCount); //清空点集中所有点
            }
            #endregion
        }

        private void axPageLayoutControl1_OnMouseUp(object sender, IPageLayoutControlEvents_OnMouseUpEvent e)
        {
            if (_enumMapSurType != EnumMapSurroundType.None)
            {
                if (pNewEnvelopeFeedback != null)
                {
                    IActiveView pActiveView = null;
                    pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                    IEnvelope pEnvelope = pNewEnvelopeFeedback.Stop();
                    AddMapSurround(pActiveView, _enumMapSurType, pEnvelope);
                    pNewEnvelopeFeedback = null;
                    _enumMapSurType = EnumMapSurroundType.None;
                }
            }
        }

        //添加整饰要素
        private void AddMapSurround(IActiveView pAV, EnumMapSurroundType _enumMapSurroundType, IEnvelope pEnvelope)
        {
            try
            {
                switch (_enumMapSurroundType)
                {
                    case EnumMapSurroundType.NorthArrow:
                        addNorthArrow(axPageLayoutControl1.PageLayout, pEnvelope, pAV);
                        break;
                    case EnumMapSurroundType.ScaleBar:
                        makeScaleBar(pAV, axPageLayoutControl1.PageLayout, pEnvelope);
                        break;
                    case EnumMapSurroundType.Legend:
                        MakeLegend(pAV, axPageLayoutControl1.PageLayout, pEnvelope);
                        break;
                }
            }
            catch
            {
            }
        }
        //添加图例
        private void MakeLegend(IActiveView pActiveView, IPageLayout pPageLayout, IEnvelope pEnv)
        {
            UID pID = new UID();
            pID.Value = "esriCarto.Legend";
            IGraphicsContainer pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pActiveView.FocusMap) as IMapFrame;
            IMapSurroundFrame pMapSurroundFrame = pMapFrame.CreateSurroundFrame(pID, null);//根据唯一标示符，创建与之对应MapSurroundFrame
            IElement pDeletElement = axPageLayoutControl1.FindElementByName("Legend");//获取PageLayout中的图例元素
            if (pDeletElement != null)
            {
                pGraphicsContainer.DeleteElement(pDeletElement);  //如果已经存在图例，删除已经存在的图例
            }
            //设置MapSurroundFrame背景
            ISymbolBackground pSymbolBackground = new SymbolBackgroundClass();
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
            pLineSymbol.Color = m_OperatePageLayout.GetRgbColor(240, 240, 240);
            pFillSymbol.Color = m_OperatePageLayout.GetRgbColor(240, 240, 240);
            pFillSymbol.Outline = pLineSymbol;
            pSymbolBackground.FillSymbol = pFillSymbol;
            pMapSurroundFrame.Background = pSymbolBackground;
            //添加图例
            IElement pElement = pMapSurroundFrame as IElement;
            pElement.Geometry = pEnv as IGeometry;
            IMapSurround pMapSurround = pMapSurroundFrame.MapSurround;
            ILegend pLegend = pMapSurround as ILegend;
            pLegend.ClearItems();
            pLegend.Title = "图例";
            for (int i = 0; i < pActiveView.FocusMap.LayerCount; i++)
            {
                ILegendItem pLegendItem = new HorizontalLegendItemClass();
                pLegendItem.Layer = pActiveView.FocusMap.get_Layer(i);//获取添加图例关联图层             
                pLegendItem.ShowDescriptions = false;
                pLegendItem.Columns = 1;
                pLegendItem.ShowHeading = true;
                pLegendItem.ShowLabels = true;
                pLegend.AddItem(pLegendItem);//添加图例内容
            }
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        //添加指北针
        void addNorthArrow(IPageLayout pPageLayout, IEnvelope pEnv, IActiveView pActiveView)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pStyleGalleryItem == null) return;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            INorthArrow pNorthArrow = new MarkerNorthArrowClass();
            pNorthArrow = pStyleGalleryItem.Item as INorthArrow;
            pNorthArrow.Size = pEnv.Width * 50;
            pMapSurroundFrame.MapSurround = (IMapSurround)pNorthArrow;//根据用户的选取，获取相应的MapSurround            
            IElement pElement = axPageLayoutControl1.FindElementByName("NorthArrows");//获取PageLayout中的指北针元素
            if (pElement != null)
            {
                pGraphicsContainer.DeleteElement(pElement);  //如果存在指北针，删除已经存在的指北针
            }
            IElementProperties pElePro = null;
            pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = (IGeometry)pEnv;
            pElePro = pElement as IElementProperties;
            pElePro.Name = "NorthArrows";
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        //添加比例尺
        public void makeScaleBar(IActiveView pActiveView, IPageLayout pPageLayout, IEnvelope pEnv)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pStyleGalleryItem == null) return;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            pMapSurroundFrame.MapSurround = (IMapSurround)pStyleGalleryItem.Item;
            IElement pElement = axPageLayoutControl1.FindElementByName("ScaleBar");
            if (pElement != null)
            {
                pGraphicsContainer.DeleteElement(pElement);  //删除已经存在的比例尺
            }
            IElementProperties pElePro = null;
            pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = (IGeometry)pEnv;
            pElePro = pElement as IElementProperties;
            pElePro.Name = "ScaleBar";
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        #endregion

        /*private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
            pMap = pActiveView.FocusMap as IMap;
            pGraphicsContainer = pActiveView as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            //设置比例尺样式
            IMapSurround pMapSurround;
            IScaleBar pScaleBar;
            pScaleBar = new ScaleLineClass();
            pScaleBar.Units = pMap.MapUnits;
            pScaleBar.Divisions = 2;
            pScaleBar.Subdivisions = 4;
            pScaleBar.DivisionsBeforeZero = 0;
            //pScaleBar.UnitLabel = ChangeMapUniteToChinese(pMap.MapUnits);
            pScaleBar.LabelPosition = esriVertPosEnum.esriBelow;
            pScaleBar.LabelGap = 3.6;
            pScaleBar.LabelFrequency = esriScaleBarFrequency.esriScaleBarDivisionsAndFirstMidpoint;


            pMapSurround = pScaleBar;
            pMapSurround.Name = "ScaleBar";
            //定义UID
            UID uid = new UIDClass();
            uid.Value = "esriCarto.ScaleLine";
            //定义MapSurroundFrame对象
            IMapSurroundFrame pMapSurroundFrame = pMapFrame.CreateSurroundFrame(uid, null);
            pMapSurroundFrame.MapSurround = pMapSurround;
            //定义Envelope设置Element摆放的位置
            IEnvelope pEnvelope = new EnvelopeClass();
            pEnvelope.PutCoords(16, 2, 25, 3);
            IElement pElement = pMapSurroundFrame as IElement;
            pElement.Geometry = pEnvelope;
            pGraphicsContainer.AddElement(pElement, 0);
           
            IMap pMap = this.axPageLayoutControl1.ActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = axPageLayoutControl1.PageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            UID pUID = new UIDClass();
            pUID.Value = "{6589F146-F7F7-11d2-B872-00600802E603}";
            IMapSurroundFrame pScaleLine = pMapFrame.CreateSurroundFrame(pUID, null);
            IEnvelope pMarkEnvelope = new EnvelopeClass();
            double xMin, yMin, xMax, yMax;
            IEnvelope pMapFrameEnvelope;

            pMapFrame.ExtentType = esriExtentTypeEnum.esriExtentScale;
            IElement pMapFrameElement = pMapFrame as IElement;
            pMapFrameEnvelope = pMapFrameElement.Geometry.Envelope;
            pMapFrameEnvelope.QueryCoords(out xMin, out yMin, out xMax, out yMax);
            pMarkEnvelope.PutCoords(xMax - 6, yMin + 1, xMax, yMin + 3);

            IStyleSelector pStyleselector;
            pStyleselector = new ScaleBarSelectorClass();
            bool ok;
            ok = pStyleselector.DoModal(0);
            if (!ok) return;

            IScaleBar pScarbar = pStyleselector.GetStyle(0) as IScaleBar;
            pScarbar.BarHeight = 1;
            pScarbar.Division = 0;
            pScarbar.Subdivisions = 0;
            pScarbar.UnitLabelGap = 0;
            pScarbar.UnitLabel = "KM";
            pScarbar.Units = esriUnits.esriMeters;
            pScarbar.LabelPosition = esriVertPosEnum.esriBelow;

            ITextSymbol pTextsymbol = new TextSymbolClass();
            pTextsymbol.Size = 1;
            stdole.StdFont pFont = new stdole.StdFontClass();
            pFont.Size = 1;
            pFont.Name = "Arial";
            pTextsymbol.Font = pFont as stdole.IFontDisp;


            pTextsymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHALeft;
            pScarbar.UnitLabelSymbol = pTextsymbol;
            pScarbar.LabelSymbol = pTextsymbol;

            INumericFormat pNumericFormat = new NumericFormatClass();
            pNumericFormat.AlignmentWidth = 0;
            pNumericFormat.RoundingOption = esriRoundingOptionEnum.esriRoundNumberOfSignificantDigits;
            pNumericFormat.RoundingValue = 0;
            pNumericFormat.UseSeparator = true;
            pNumericFormat.ShowPlusSign = false;

            INumberFormat pNumberFormat = pNumericFormat as INumberFormat;
            pScarbar.NumberFormat = pNumberFormat;

            IMapSurround pMapSurround = pScarbar as IMapSurround;
            pScaleLine.MapSurround = pMapSurround;


            this.axPageLayoutControl1.AddElement(pScaleLine as IElement, pMarkEnvelope, Type.Missing, Type.Missing, 0);
            this.axPageLayoutControl1.Refresh();
        }*/

        /*private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
            pMap = pActiveView.FocusMap as IMap;
            pGraphicsContainer = pActiveView as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IMapSurround pMapSurround;
            INorthArrow pNorthArrow;
            pNorthArrow = new MarkerNorthArrowClass();
            pMapSurround = pNorthArrow;
            pMapSurround.Name = "NorthArrow";
            //定义UID
            UID uid = new UIDClass();
            uid.Value = "esriCarto.MarkerNorthArrow";
            //定义MapSurroundFrame对象
            IMapSurroundFrame pMapSurroundFrame = pMapFrame.CreateSurroundFrame(uid, null);
            pMapSurroundFrame.MapSurround = pMapSurround;
            //定义Envelope设置Element摆放的位置
            IEnvelope pEnvelope = new EnvelopeClass();
            pEnvelope.PutCoords(3, 25, 3, 25);


            IElement pElement = pMapSurroundFrame as IElement;
            pElement.Geometry = pEnvelope;
            pGraphicsContainer.AddElement(pElement, 0);

           
        }*/

        /*private void addLegendToolStripMenuItem_Click(object sender, EventArgs e)
        {
             //获取axPageLayoutControl1的图形容器
            IGraphicsContainer graphicsContainer =
            axPageLayoutControl1.GraphicsContainer;
            //获取axPageLayoutControl1空间里面显示的地图图层
            IMapFrame mapFrame =
            (IMapFrame)graphicsContainer.FindFrame(axPageLayoutControl1.ActiveView.FocusMap);
            if (mapFrame == null) return;
            //创建图例
            UID uID = new UIDClass();//创建UID作为该图例的唯一标识符，方便创建之后进行删除、移动等操作
            uID.Value = "esriCarto.Legend";
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null); 
            if (mapSurroundFrame == null) return;
            if (mapSurroundFrame.MapSurround == null) return;
            mapSurroundFrame.MapSurround.Name = "Legend";
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(16, 3, 18, 5);//设置图例摆放位置（原点在axPageLayoutControl左下角）
            IElement element = (IElement)mapSurroundFrame;
            element.Geometry = envelope;
            //将图例转化为几何要素添加到axPageLayoutControl1,并刷新页面显示
            axPageLayoutControl1.AddElement(element, Type.Missing, Type.Missing,
            "Legend", 0);
            axPageLayoutControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null,
            null);
        }*/

        #region 添加制图要素3 格网
        //Graticule格网
        private void buttonItem21_Click(object sender, EventArgs e)
        {
            IActiveView pActiveView = axPageLayoutControl1.ActiveView;
            IPageLayout pPageLayout = axPageLayoutControl1.PageLayout;
            DeleteMapGrid(pActiveView, pPageLayout);
            CreateGraticuleMapGrid(pActiveView, pPageLayout);
        }
        public void CreateGraticuleMapGrid(IActiveView pActiveView, IPageLayout pPageLayout)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraticule pGraticule = new GraticuleClass();//看这个改动是否争取
            pGraticule.Name = "Map Grid";
            //设置网格线的符号样式
            ICartographicLineSymbol pLineSymbol;
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 1;
            pLineSymbol.Color = m_OperatePageLayout.GetRgbColor(166, 187, 208);
            pGraticule.LineSymbol = pLineSymbol;
            //设置网格的边框样式           
            ISimpleMapGridBorder simpleMapGridBorder = new SimpleMapGridBorderClass();
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Color = m_OperatePageLayout.GetRgbColor(100, 255, 0);
            simpleLineSymbol.Width = 2;
            simpleMapGridBorder.LineSymbol = simpleLineSymbol as ILineSymbol;
            pGraticule.Border = simpleMapGridBorder as IMapGridBorder;
            pGraticule.SetTickVisibility(true, true, true, true);
            //设置网格的主刻度的样式和可见性
            pGraticule.TickLength = 15;
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 1;
            pLineSymbol.Color = m_OperatePageLayout.GetRgbColor(255, 187, 208);
            pGraticule.TickMarkSymbol = null;
            pGraticule.TickLineSymbol = pLineSymbol;
            pGraticule.SetTickVisibility(true, true, true, true);
            //设置网格的次级刻度的样式和可见性
            pGraticule.SubTickCount = 5;
            pGraticule.SubTickLength = 10;
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 0.1;
            pLineSymbol.Color = m_OperatePageLayout.GetRgbColor(166, 187, 208);
            pGraticule.SubTickLineSymbol = pLineSymbol;
            pGraticule.SetSubTickVisibility(true, true, true, true);
            //设置网格的标签的样式和可见性
            IGridLabel pGridLabel;
            pGridLabel = pGraticule.LabelFormat;
            pGridLabel.LabelOffset = 15;
            stdole.StdFont pFont = new stdole.StdFont();
            pFont.Name = "Arial";
            pFont.Size = 16;
            pGraticule.LabelFormat.Font = pFont as stdole.IFontDisp;
            pGraticule.Visible = true;
            //创建IMeasuredGrid对象
            IMeasuredGrid pMeasuredGrid = new MeasuredGridClass();
            IProjectedGrid pProjectedGrid = pMeasuredGrid as IProjectedGrid;
            pProjectedGrid.SpatialReference = pMap.SpatialReference;
            pMeasuredGrid = pGraticule as IMeasuredGrid;
            //获取坐标范围，设置网格的起始点和间隔
            double MaxX, MaxY, MinX, MinY;
            pProjectedGrid.SpatialReference.GetDomain(out MinX, out MaxX, out MinY, out MaxY);
            pMeasuredGrid.FixedOrigin = true;
            pMeasuredGrid.Units = pMap.MapUnits;
            pMeasuredGrid.XIntervalSize = (MaxX - MinX) / 1000000000;//纬度间隔
            pMeasuredGrid.XOrigin = MinX;
            pMeasuredGrid.YIntervalSize = (MaxY - MinY) / 1000000000;//经度间隔.
            pMeasuredGrid.YOrigin = MinY;
            //将网格对象添加到地图控件中                              
            IGraphicsContainer pGraphicsContainer = pActiveView as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IMapGrids pMapGrids = pMapFrame as IMapGrids;
            pMapGrids.AddMapGrid(pGraticule);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
        }
        public void DeleteMapGrid(IActiveView pActiveView, IPageLayout pPageLayout)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer graphicsContainer = pPageLayout as IGraphicsContainer;
            IFrameElement frameElement = graphicsContainer.FindFrame(pMap);
            IMapFrame mapFrame = frameElement as IMapFrame;
            IMapGrids mapGrids = null;
            mapGrids = mapFrame as IMapGrids;
            if (mapGrids.MapGridCount > 0)
            {
                IMapGrid pMapGrid = mapGrids.get_MapGrid(0);
                mapGrids.DeleteMapGrid(pMapGrid);
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
        }
        //MeasuredGrid格网
        private void addGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IActiveView pActiveView = axPageLayoutControl1.ActiveView;
            IPageLayout pPageLayout = axPageLayoutControl1.PageLayout;
            DeleteMapGrid(pActiveView, pPageLayout);//删除已存在格网
            CreateMeasuredGrid(pActiveView, pPageLayout);
        }
        public void CreateMeasuredGrid(IActiveView pActiveView, IPageLayout pPageLayout)
        {
            IMap map = pActiveView.FocusMap;
            IMeasuredGrid pMeasuredGrid = new MeasuredGridClass();
            //设置格网基本属性           
            pMeasuredGrid.FixedOrigin = false;
            pMeasuredGrid.Units = map.MapUnits;
            pMeasuredGrid.XIntervalSize = 100000000000;//纬度间隔           
            pMeasuredGrid.YIntervalSize = 100000000000;//经度间隔.             
            //设置GridLabel格式
            IGridLabel pGridLabel = new FormattedGridLabelClass();
            IFormattedGridLabel pFormattedGridLabel = new FormattedGridLabelClass();
            INumericFormat pNumericFormat = new NumericFormatClass();
            pNumericFormat.AlignmentOption = esriNumericAlignmentEnum.esriAlignLeft;
            pNumericFormat.RoundingOption = esriRoundingOptionEnum.esriRoundNumberOfDecimals;
            pNumericFormat.RoundingValue = 0;
            pNumericFormat.ZeroPad = true;
            pFormattedGridLabel.Format = pNumericFormat as INumberFormat;
            pGridLabel = pFormattedGridLabel as IGridLabel;
            StdFont myFont = new stdole.StdFontClass();
            myFont.Name = "宋体";
            myFont.Size = 15;
            pGridLabel.Font = myFont as IFontDisp;
            IMapGrid pMapGrid = new MeasuredGridClass();
            pMapGrid = pMeasuredGrid as IMapGrid;
            pMapGrid.LabelFormat = pGridLabel;
            //将格网添加到地图上           
            IGraphicsContainer graphicsContainer = pPageLayout as IGraphicsContainer;
            IFrameElement frameElement = graphicsContainer.FindFrame(map);
            IMapFrame mapFrame = frameElement as IMapFrame;
            IMapGrids mapGrids = null;
            mapGrids = mapFrame as IMapGrids;
            mapGrids.AddMapGrid(pMapGrid);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
        }
        #endregion

        #region 量测
        private void btnMeasureLength_Click(object sender, EventArgs e)  //距离量测
        {
            isMeasure = true;
            axMapControl1.CurrentTool = null;
            pMouseOperate = "MeasureLength";
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            if (frmMeasureResult == null || frmMeasureResult.IsDisposed)
            {
                frmMeasureResult = new FrmMeasureResult();
                frmMeasureResult.frmClosed += new FrmMeasureResult.FrmClosedEventHandler(frmMeasureResult_frmColsed);
                frmMeasureResult.lblMeasureResult.Text = "";
                frmMeasureResult.Text = "距离量测";
                frmMeasureResult.Show();
            }
            else
            {
                frmMeasureResult.Activate();
            }
        }
        private void btnMeasureArea_Click(object sender, EventArgs e)   //面积量测
        {
            isMeasure = true;
            axMapControl1.CurrentTool = null;
            pMouseOperate = "MeasureArea";
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            if (frmMeasureResult == null || frmMeasureResult.IsDisposed)
            {
                frmMeasureResult = new FrmMeasureResult();
                frmMeasureResult.frmClosed += new FrmMeasureResult.FrmClosedEventHandler(frmMeasureResult_frmColsed);
                frmMeasureResult.lblMeasureResult.Text = "";
                frmMeasureResult.Text = "面积量测";
                frmMeasureResult.Show();
            }
            else
            {
                frmMeasureResult.Activate();
            }
        }
        private void frmMeasureResult_frmColsed()                    //测量窗口关闭响应(原窗口的委托)  
        {
            //清空线对象
            if (pNewLineFeedback != null)
            {
                pNewLineFeedback.Stop();
                pNewLineFeedback = null;
            }
            //清空面对象
            if (pNewPolygonFeedback != null)
            {
                pNewPolygonFeedback.Stop();
                pNewPolygonFeedback = null;
                pAreaPointCol.RemovePoints(0, pAreaPointCol.PointCount); //清空点集中所有点
            }
            //清空量算画的线、面对象
            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
            //结束量算功能
            pMouseOperate = string.Empty;
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
        }
        #endregion

        #region 统计图表
        //柱状图
        private void barChartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand cmd;
            cmd = new BarChart();
            cmd.OnCreate(axMapControl1.GetOcx());
            GeoBaseLib gb = new GeoBaseLib(axMapControl1);
            string sLayer = "交通通达度";
            ILayer m_pLayer = gb.GetLayer(sLayer);
            ((BarChart)cmd).pLayer = m_pLayer as IFeatureLayer;
            if (cmd.Enabled)
            {
                cmd.OnClick();
                this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }
        #endregion
    }
}
