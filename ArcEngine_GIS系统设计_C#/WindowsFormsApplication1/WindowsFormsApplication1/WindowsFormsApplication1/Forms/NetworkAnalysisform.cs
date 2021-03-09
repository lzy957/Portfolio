using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalyst;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.NetworkAnalysis;


namespace WindowsFormsApplication1.Forms
{
    public partial class NetworkAnalysisform : Form
    {


    //    ClassPathFinder m_ClassPathFinder;

        private IGeometricNetwork m_ipGeometricNetwork;
        private IMap m_ipMap;
        private IPointCollection m_ipPoints;
        private IPointToEID m_ipPointToEID;
        private double m_dblPathCost = 0;
        private IEnumNetEID m_ipEnumNetEID_Junctions;
        private IEnumNetEID m_ipEnumNetEID_Edges;
        private IPolyline m_ipPolyline;

        //地图数据 
        private AxMapControl mMapControl;
        //选中图层 
        private IFeatureLayer mFeatureLayer;
        //根据所选择的图层查询得到的特征类
        private IFeatureClass pFeatureClass = null;

        private IActiveView m_ipActiveView;
        private bool clicked;
     
        int clickedcount = 0;
        public NetworkAnalysisform()
        {
            InitializeComponent();
            pPointC = pMulPoint as IPointCollection;
            pClickedCount = 0;
            m_ipActiveView = axMapControl1.ActiveView;
            m_ipMap = m_ipActiveView.FocusMap;
            clicked = false;
            pGC = m_ipMap as IGraphicsContainer;
        }
        #region Services

     
        INAContext pNAContext;
        INASolver pNASolveClass;
        INALayer pNALayer;
        IMap pNetMap;
        IGraphicsContainer pGC;
        IPointCollection pPointC;
        IMultipoint pMulPoint=new MultipointClass();
        int pFlag = 0;
        int pClickedCount;
        IWorkspace pFeatureWorkspace;
        INetworkDataset pNetworkDataset;
        private void Initial()
        {
            this.axMapControl1.ActiveView.Clear();
            axMapControl1.ActiveView.Refresh();

            /*
            
            pFeatureWorkspace = OpenWorkspace(ConfigurationManager.ConnectionStrings["MdbPath"].ToString()) as IFeatureWorkspace;
            pNetworkDataset = OpenNetworkDataset_Other(pFeatureWorkspace as IWorkspace, "TestNet_ND", "TestNet");

            pNAContext = CreateNAContext(pNetworkDataset);

            pInputFC = pFeatureWorkspace.OpenFeatureClass("stop");

            pVertexFC = pFeatureWorkspace.OpenFeatureClass("TestNet_ND_Junctions");

            IFeatureLayer pVertexFL = new FeatureLayerClass();
            pVertexFL.FeatureClass = pFeatureWorkspace.OpenFeatureClass("TestNet_ND_Junctions");
            pVertexFL.Name = pVertexFL.FeatureClass.AliasName;
            axMapControl1.AddLayer(pVertexFL, 0);

            IFeatureLayer pRoadFL = new FeatureLayerClass();
            pRoadFL.FeatureClass = pFeatureWorkspace.OpenFeatureClass("道路中心线");
            pRoadFL.Name = pRoadFL.FeatureClass.AliasName;
            axMapControl1.AddLayer(pRoadFL, 0);

            ILayer pLayer;
            INetworkLayer pNetworkLayer = new NetworkLayerClass();
            pNetworkLayer.NetworkDataset = pNetworkDataset;
            pLayer = pNetworkLayer as ILayer;
            pLayer.Name = "Network Dataset";
            axMapControl1.AddLayer(pLayer, 0);

            //Create a Network Analysis Layer and add to ArcMap
            INALayer naLayer = pNAContext.Solver.CreateLayer(pNAContext);
            pLayer = naLayer as ILayer;
            pLayer.Name = pNAContext.Solver.DisplayName;
            axMapControl1.AddLayer(pLayer, 0);

            pActiveView = axMapControl1.ActiveView;
            pMap = pActiveView.FocusMap;
            pGraphicsContainer = pMap as IGraphicsContainer;
             */
             
        }

        /// <summary>
        /// 获取网络数据集
        /// </summary>
        /// <param name="_pFeatureWs"></param>
        /// <param name="_pDatasetName"></param>
        /// <param name="_pNetDatasetName"></param>
        /// <returns></returns>
        INetworkDataset GetNetDataset(IFeatureWorkspace _pFeatureWs, string _pDatasetName, string _pNetDatasetName)
        {

            ESRI.ArcGIS.Geodatabase.IDatasetContainer3 pDatasetContainer = null;

            ESRI.ArcGIS.Geodatabase.IFeatureDataset pFeatureDataset = _pFeatureWs.OpenFeatureDataset(_pDatasetName);
            ESRI.ArcGIS.Geodatabase.IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer = pFeatureDataset as ESRI.ArcGIS.Geodatabase.IFeatureDatasetExtensionContainer; // Dynamic Cast
            ESRI.ArcGIS.Geodatabase.IFeatureDatasetExtension pFeatureDatasetExtension = pFeatureDatasetExtensionContainer.FindExtension(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTNetworkDataset);
            pDatasetContainer = pFeatureDatasetExtension as ESRI.ArcGIS.Geodatabase.IDatasetContainer3; // Dynamic Cast

            ESRI.ArcGIS.Geodatabase.IDataset pNetWorkDataset = pDatasetContainer.get_DatasetByName(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTNetworkDataset, _pNetDatasetName);

            return pNetWorkDataset as ESRI.ArcGIS.Geodatabase.INetworkDataset; // Dynamic Cast
        }

        /// <summary>
        /// 加载无向网络
        /// </summary>
        /// <param name="axMapControl1">地图控件名</param>
        /// <param name="Datasetpath">数据库路径</param>
        /// <param name="Datasetname">数据集名称</param>
        /// <param name="netdatasetname">无向网络名称</param>
        public void LoadNetDataSet(AxMapControl axMapControl1, string Datasetpath, string Datasetname, string netdatasetname)
        {
            IWorkspace pWs = GetMDBWorkspace(Datasetpath);

            IFeatureWorkspace pFtWs = pWs as IFeatureWorkspace;

            INetworkDataset pNetWorkDataset = GetNetDataset(pFtWs, Datasetname, netdatasetname);

            pNASolveClass = new NARouteSolverClass();

            loadNet(axMapControl1.Map, pNetWorkDataset);

            
            pNAContext = GetSolverContext(pNASolveClass, pNetWorkDataset);

            pNALayer = GetNaLayer(pNASolveClass, GetSolverContext(pNASolveClass, pNetWorkDataset));

            axMapControl1.Map.AddLayer(pNALayer as ILayer);
        }

        /// <summary>
        /// 打开个人数据库
        /// </summary>
        /// <param name="_pGDBName"></param>
        /// <returns></returns>
        IWorkspace GetMDBWorkspace(String _pGDBName)
        {
            IWorkspaceFactory pWsFac = new AccessWorkspaceFactoryClass();

            IWorkspace pWs = pWsFac.OpenFromFile(_pGDBName, 0);

            return pWs;
        }

        /// <summary>
        /// _pFtClass参数为Stops的要素类，_pPointC是用鼠标点的点生成的点的集合，最后一个参数是捕捉距离
        /// </summary>
        /// <param name="_pNaContext"></param>
        /// <param name="_pFtClass"></param>
        /// <param name="_pPointC"></param>
        /// <param name="_pDist"></param>
        public void NASolve(INAContext _pNaContext, IFeatureClass _pFtClass, IPointCollection _pPointC, double _pDist)
        {
            INALocator pNAlocator = _pNaContext.Locator;
            for (int i = 0; i < _pPointC.PointCount; i++)
            {
                IFeature pFt = _pFtClass.CreateFeature();

                IRowSubtypes pRowSubtypes = pFt as IRowSubtypes;

                pRowSubtypes.InitDefaultValues();

                pFt.Shape = _pPointC.get_Point(i) as IGeometry;

                IPoint pPoint = null;

                INALocation pNalocation = null;

                pNAlocator.QueryLocationByPoint(_pPointC.get_Point(i), ref pNalocation, ref pPoint, ref _pDist);

                INALocationObject pNAobject = pFt as INALocationObject;

                pNAobject.NALocation = pNalocation;

                int pNameFieldIndex = _pFtClass.FindField("Name");

                pFt.set_Value(pNameFieldIndex, pPoint.X.ToString() + "," + pPoint.Y.ToString());

                int pStatusFieldIndex = _pFtClass.FindField("Status");

                pFt.set_Value(pStatusFieldIndex, esriNAObjectStatus.esriNAObjectStatusOK);

                int pSequenceFieldIndex = _pFtClass.FindField("Sequence");

                pFt.set_Value(_pFtClass.FindField("Sequence"), ((ITable)_pFtClass).RowCount(null));

                pFt.Store();

            }
        }

        /// <summary>
        /// 获取网络分析上下文，这个接口是网络分析中很重要的一个
        /// </summary>
        /// <param name="_pNaSolver"></param>
        /// <param name="_pNetworkDataset"></param>
        /// <returns></returns>
        public INAContext GetSolverContext(INASolver _pNaSolver, INetworkDataset _pNetworkDataset)
        {
            //Get the Data Element

            IDatasetComponent pDataComponent = _pNetworkDataset as IDatasetComponent;

            IDEDataset pDeDataset = pDataComponent.DataElement;

            INAContextEdit pContextEdit = _pNaSolver.CreateContext(pDeDataset as IDENetworkDataset, _pNaSolver.Name) as INAContextEdit;

            //Prepare the context for analysis based upon the current network dataset schema.
            pContextEdit.Bind(_pNetworkDataset, new GPMessagesClass());
            return pContextEdit as INAContext;
        }

        /// <summary>
        /// 加载NetworkDataset到Map中
        /// </summary>
        /// <param name="_pMap"></param>
        /// <param name="_pNetworkDataset"></param>
        void loadNet(IMap _pMap, INetworkDataset _pNetworkDataset)
        {
            INetworkLayer pNetLayer = new NetworkLayerClass();

            pNetLayer.NetworkDataset = _pNetworkDataset;

            _pMap.AddLayer(pNetLayer as ILayer);
        }

        //打开工作空间
        private IWorkspace OpenWorkspace(string strMDBName)
        {
            IWorkspaceFactory pWorkspaceFactory = new AccessWorkspaceFactoryClass();
            return pWorkspaceFactory.OpenFromFile(strMDBName, 0);
        }

        //打开网络数据集
        private INetworkDataset OpenNetworkDataset(IWorkspace workspace, string strNDName)
        {
            IWorkspaceExtensionManager pWorkspaceExtensionManager;
            IWorkspaceExtension pWorkspaceExtension;
            IDatasetContainer2 pDatasetContainer2;

            pWorkspaceExtensionManager = workspace as IWorkspaceExtensionManager;
            int iCount = pWorkspaceExtensionManager.ExtensionCount;
            for (int i = 0; i < iCount; i++)
            {
                pWorkspaceExtension = pWorkspaceExtensionManager.get_Extension(i);
                if (pWorkspaceExtension.Name.Equals("Network Dataset"))
                {
                    pDatasetContainer2 = pWorkspaceExtension as IDatasetContainer2;
                    return pDatasetContainer2.get_DatasetByName(esriDatasetType.esriDTNetworkDataset, strNDName) as INetworkDataset;
                }
            }
            return null;

        }

        /// <summary>
        /// 获取NALayer
        /// </summary>
        /// <param name="_pNaSover"></param>
        /// <param name="_pNaContext"></param>
        /// <returns></returns>

        INALayer GetNaLayer(INASolver _pNaSover, INAContext _pNaContext)
        {
            return _pNaSover.CreateLayer(_pNaContext);
        }

        /*
        private INetworkDataset OpenNetworkDataset_Other(IWorkspace workspace, string strNDName, string strRoadFeatureDataset)
        {
            IDatasetContainer3 pDatasetContainer3;
            IFeatureWorkspace pFeatureWorkspace = workspace as IFeatureWorkspace;
            pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(strRoadFeatureDataset);
            IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer = pFeatureDataset as IFeatureDatasetExtensionContainer;
            IFeatureDatasetExtension pFeatureDatasetExtension = pFeatureDatasetExtensionContainer.FindExtension(esriDatasetType.esriDTNetworkDataset);
            pDatasetContainer3 = pFeatureDatasetExtension as IDatasetContainer3;

            if (pDatasetContainer3 == null)
                return null;
            IDataset pDataset = pDatasetContainer3.get_DatasetByName(esriDatasetType.esriDTNetworkDataset, strNDName);
            return pDataset as INetworkDataset;
        }
         
       

        //创建网络分析上下文
        private INAContext CreateNAContext(INetworkDataset networkDataset)
        {
            IDENetworkDataset pDENetworkDataset = GetDENetworkDataset(networkDataset);
            INASolver pNASolver = new NARouteSolverClass();
            INAContextEdit pNAContextEdit = pNASolver.CreateContext(pDENetworkDataset, pNASolver.Name) as INAContextEdit;
            pNAContextEdit.Bind(networkDataset, new GPMessagesClass());
            return pNAContextEdit as INAContext;
        }
         * */

        //根据点图层确定最短路径所用经历的点
        //private void LoadNANetWorkLocations(string strNAClassName, IFeatureClass inputFC, double dSnapTolerance)
        //{
        //    INAClass pNAClass;
        //    INamedSet pNamedSet;
        //    pNamedSet = pNAContext.NAClasses;
        //    pNAClass = pNamedSet.get_ItemByName(strNAClassName) as INAClass;

        //    //删除已存在的位置点
        //    pNAClass.DeleteAllRows();

        //    //创建NAClassLoader，设置捕捉容限值
        //    INAClassLoader pNAClassLoader = new NAClassLoaderClass();
        //    pNAClassLoader.Locator = pNAContext.Locator;
        //    if (dSnapTolerance > 0)
        //        pNAClassLoader.Locator.SnapTolerance = dSnapTolerance;
        //    pNAClassLoader.NAClass = pNAClass;

        //    //字段匹配
        //    INAClassFieldMap pNAClassFieldMap = new NAClassFieldMapClass();
        //    pNAClassFieldMap.CreateMapping(pNAClass.ClassDefinition, inputFC.Fields);
        //    pNAClassLoader.FieldMap = pNAClassFieldMap;

        //    //pNAClassFieldMap.set_MappedField("OBJECTID", "OBJECTID");
        //    //pNAClassLoader.FieldMap = pNAClassFieldMap;

        //    //加载网络位置点数据
        //    int iRows = 0;
        //    int iRowsLocated = 0;
        //    IFeatureCursor pFeatureCursor = pInputFC.Search(null, true);
        //    pNAClassLoader.Load((ICursor)pFeatureCursor, null, ref iRows, ref iRowsLocated);
        //    ((INAContextEdit)pNAContext).ContextChanged();
        //}

        #endregion

        private string OpenMxd()
        {
            string MxdPath = "";
          //  Microsoft.Win32.OpenFileDialog OpenMXD = new Microsoft.Win32.OpenFileDialog();
            OpenFileDialog OpenMXD = new OpenFileDialog(); 
            OpenMXD.Title = "打开地图";
            OpenMXD.InitialDirectory = "C:\\Users\\Administrator\\Desktop\\作业\\空间分析\\实习walkscore";
            OpenMXD.Filter = "Map Documents (*.mxd)|*.mxd";
            if (OpenMXD.ShowDialog() == DialogResult.OK)
            {
                MxdPath = OpenMXD.FileName;
            }
            return MxdPath;
        }

        private string OpenMDB()
        {
            string MxdPath = "";
            OpenFileDialog OpenMXD = new OpenFileDialog();
            OpenMXD.Title = "打开个人数据库";
            OpenMXD.InitialDirectory = "C:\\Users\\Administrator\\Desktop\\作业\\空间分析\\实习walkscore";
            OpenMXD.Filter = "Personl GeoMDB (*.mdb)|*.mdb";
            if (OpenMXD.ShowDialog() == DialogResult.OK)
            {
                MxdPath = OpenMXD.FileName;
            }
            return MxdPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string mapfile = OpenMxd();
            try
            {
                this.axMapControl1.LoadMxFile(mapfile);
            }
            catch (Exception)
            {
                
                throw;
            }

            //MapControl中没有图层时返回 
            if (axMapControl1.LayerCount <= 0)
                return;
            //获取MapControl中的全部图层名称，并加入ComboBox 
            //图层 
            ILayer pLayer;
            //图层名称 
            string strLayerName;
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                pLayer = axMapControl1.get_Layer(i);
                strLayerName = pLayer.Name;
                //图层名称加入cboLayer 
                this.comboBox1.Items.Add(strLayerName);
            }
            //默认显示第一个选项 
            this.comboBox1.SelectedIndex = 0;
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string datanetname = OpenMDB();
            if (datanetname!="")
            {
                LoadNetDataSet(axMapControl1, datanetname, "geometric", "street");
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            pFlag = 2;
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {

            /*
            if (pFlag == 2)
            {
                pNetMap = axMapControl1.Map;

                pGC = pNetMap as IGraphicsContainer;

                IActiveView pActView = pNetMap as IActiveView;

                IPoint pPoint = pActView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

                object o = Type.Missing;
                object o1 = Type.Missing;

                pPointC.AddPoint(pPoint, ref o, ref o1);


                IElement Element;

                ITextElement Textelement = new TextElementClass();

                Element = Textelement as IElement;

                pClickedCount++;

                Textelement.Text = pClickedCount.ToString();

                Element.Geometry = pActView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

                pGC.AddElement(Element, 0);

                pActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);


                if (pNAContext != null)
                {
                    IFeatureClass pFeatureClass = pNAContext.NAClasses.get_ItemByName("Stops") as IFeatureClass;

                    if (pFeatureClass != null)
                    {
                        NASolve(pNAContext, pFeatureClass, pPointC, 500);

                        IGPMessages gpMessages = new GPMessagesClass();
                        try
                        {
                            bool pBool = pNASolveClass.Solve(pNAContext, gpMessages, null);
                        }
                        catch (Exception)
                        {
                            
                            throw;
                        }

                       

                    }
                

                }

            }
             */

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

        private void NetworkAnalysis_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //if (axMapControl1 == null)
            //{
            //  //  m_ClassPathFinder = new ClassPathFinder(axMapControl1);

            //    ILayer ipLayer = axMapControl1.get_Layer(0);
            //    IFeatureLayer ipFeatureLayer = ipLayer as IFeatureLayer;



            //}
            if (axMapControl1.LayerCount == 0)  
                return;
            else
            {
                m_ipActiveView = axMapControl1.ActiveView;
                m_ipMap = m_ipActiveView.FocusMap;
            }
            ILayer ipLayer = m_ipMap.get_Layer(0);
            //IFeatureLayer ipFeatureLayer = ipLayer as IFeatureLayer;
            IFeatureLayer ipFeatureLayer = mFeatureLayer;
            IFeatureDataset ipFDS = ipFeatureLayer.FeatureClass.FeatureDataset;
            this.OpenFeatureDatasetNetworks(ipFDS);// OpenFeatureDatasetNetwork(ipFDS);
            clicked = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SolvePath("Weight");// SolvePath("Weight");//先解析路径
            IPolyline ipPolyResult = PathPolyLine();// PathPolyLine();//最后返回最短路径
            clicked = false;


            IRgbColor color = new RgbColorClass();
            color.Red = 255;
            IElement element = new LineElementClass();
            ILineSymbol linesymbol = new SimpleLineSymbolClass();
            linesymbol.Color = color as IColor;
            linesymbol.Width = 100;
            element.Geometry = m_ipPolyline;
            pGC.AddElement(element, 2);
            m_ipActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            CloseWorkspace();
        }
        //1
        #region MyRegion
        public void OpenFeatureDatasetNetworks(IFeatureDataset FeatureDataset)
        {
            CloseWorkspace();
            if (!InitializeNetworkAndMap(FeatureDataset))
                Console.WriteLine("打开network出错");
        }


        #region Public Function
        //返回和设置当前地图
        public IMap SetOrGetMap
        {
            set { m_ipMap = value; }
            get { return m_ipMap; }
        }
        //打开网络
       /* public void OpenFeatureDatasetNetwork(IFeatureDataset FeatureDataset)
        {
            CloseWorkspace();
            if (!InitializeNetworkAndMap(FeatureDataset))
                Console.WriteLine("打开出错");
        }*/
        //输入点的集合
        public IPointCollection StopPoints
        {
            set { m_ipPoints = value; }
            get { return m_ipPoints; }
        }

        //路径成本
        public double PathCost
        {
            get { return m_dblPathCost; }
        }

        //返回路径
        public IPolyline PathPolyLine()
        {
            IEIDInfo ipEIDInfo;
            IGeometry ipGeometry;
            if (m_ipPolyline != null) return m_ipPolyline;

            m_ipPolyline = new PolylineClass();
            IGeometryCollection ipNewGeometryColl = m_ipPolyline as IGeometryCollection;

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
                /*C#中使用
                 *ITraceFlowSolverGEN替代ITraceFlowSolver
                 */
                ITraceFlowSolverGEN ipTraceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                INetSolver ipNetSolver = ipTraceFlowSolver as INetSolver;
                INetwork ipNetwork = m_ipGeometricNetwork.Network;
                ipNetSolver.SourceNetwork = ipNetwork;
                INetElements ipNetElements = ipNetwork as INetElements;
                int intCount = m_ipPoints.PointCount;
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
                //计算成本
                m_dblPathCost = 0;
                for (int i = 0; i < vaRes.Length; i++)
                {
                    double m_Va = (double)vaRes[i];
                    m_dblPathCost = m_dblPathCost + m_Va;
                }
                m_ipPolyline = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
        #region Private Function
        //初始化
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
            //获取几何网络工作空间
            m_ipGeometricNetwork = ipNetworkCollection.get_GeometricNetwork(0);

            INetwork ipNetwork = m_ipGeometricNetwork.Network;



            if (m_ipMap != null)
            {
                m_ipMap = new MapClass();
                ipFeatureClassContainer = m_ipGeometricNetwork as IFeatureClassContainer;
                count = ipFeatureClassContainer.ClassCount;
                for (int i = 0; i < count; i++)
                {
                    ipFeatureClass = ipFeatureClassContainer.get_Class(i);
                    ipFeatureLayer = new FeatureLayerClass();
                    ipFeatureLayer.FeatureClass = ipFeatureClass;
                    m_ipMap.AddLayer(ipFeatureLayer);
                }
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            //获取cboLayer中选中的图层 
            mFeatureLayer = axMapControl1.get_Layer(comboBox1.SelectedIndex) as IFeatureLayer;
            pFeatureClass = mFeatureLayer.FeatureClass;

        }
        #endregion
    }
}
