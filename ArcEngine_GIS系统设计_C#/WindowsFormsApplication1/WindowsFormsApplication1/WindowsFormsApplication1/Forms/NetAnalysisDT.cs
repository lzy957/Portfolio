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
using ESRI.ArcGIS.esriSystem;

namespace WindowsFormsApplication1.Forms
{
    public partial class NetAnalysisDT : Form
    {
        private IFeatureWorkspace pFeatureWorkspace;
        private INetworkDataset pNetworkDataset;
        private INAContext pNAContext;
        private INASolver pNASolveClass;
        private INALayer pNALayer;
        private IFeatureClass pInputFC;
        private IFeatureClass pVertexFC;
        private IActiveView pActiveView;
        private IMap pMap;
        private IGraphicsContainer pGraphicsContainer;
        private IFeatureDataset pFeatureDataset;
       
        private bool clicked;
        private IPointCollection m_ipPoints;
        int clickedcount = 0;

        public NetAnalysisDT()
        {
            InitializeComponent();
            Initial();
        }

        //初始化地图、网络数据集
        private void Initial()
        {
            this.axMapControl1.ActiveView.Clear();
            axMapControl1.ActiveView.Refresh();

            pFeatureWorkspace = OpenWorkspace("C:\\Users\\Administrator\\Desktop\\作业\\空间分析\\实习walkscore\\walkscoredata\\test.mdb") as IFeatureWorkspace;
            pNetworkDataset = OpenNetworkDataset_Other(pFeatureWorkspace as IWorkspace, "test_ND", "test");

            pNAContext = CreateNAContext(pNetworkDataset);

            pInputFC = pFeatureWorkspace.OpenFeatureClass("stop");

            pVertexFC = pFeatureWorkspace.OpenFeatureClass("test_ND_Junctions");

            IFeatureLayer pVertexFL = new FeatureLayerClass();
            pVertexFL.FeatureClass = pFeatureWorkspace.OpenFeatureClass("test_ND_Junctions");
            pVertexFL.Name = pVertexFL.FeatureClass.AliasName;
            axMapControl1.AddLayer(pVertexFL, 0);

            IFeatureLayer pRoadFL = new FeatureLayerClass();
            pRoadFL.FeatureClass = pFeatureWorkspace.OpenFeatureClass("streetf");
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

        //得到创建网络分析上下文所需的IDENetworkDataset类型参数 
        public IDENetworkDataset GetDENetworkDataset(INetworkDataset networkDataset)
        {
            //QI from the Network Dataset to the DatasetComponent
            IDatasetComponent dsComponent;
            dsComponent = networkDataset as IDatasetComponent;
                        //Get the Data Element
            return dsComponent.DataElement as IDENetworkDataset;
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

        //根据点图层确定最短路径所用经历的点
        private void LoadNANetWorkLocations(string strNAClassName, IFeatureClass inputFC, double dSnapTolerance)
        {
            INAClass pNAClass;
            INamedSet pNamedSet;
            pNamedSet = pNAContext.NAClasses;
            pNAClass = pNamedSet.get_ItemByName(strNAClassName) as INAClass;

            //删除已存在的位置点
            pNAClass.DeleteAllRows();

            //创建NAClassLoader，设置捕捉容限值
            INAClassLoader pNAClassLoader = new NAClassLoaderClass();
            pNAClassLoader.Locator = pNAContext.Locator;
            if (dSnapTolerance > 0)
                pNAClassLoader.Locator.SnapTolerance = dSnapTolerance;
            pNAClassLoader.NAClass = pNAClass;

            //字段匹配
            INAClassFieldMap pNAClassFieldMap = new NAClassFieldMapClass();
            pNAClassFieldMap.CreateMapping(pNAClass.ClassDefinition, inputFC.Fields);
            pNAClassLoader.FieldMap = pNAClassFieldMap;

            //pNAClassFieldMap.set_MappedField("OBJECTID", "OBJECTID");
            //pNAClassLoader.FieldMap = pNAClassFieldMap;

            //加载网络位置点数据
            int iRows = 0;
            int iRowsLocated = 0;
            IFeatureCursor pFeatureCursor = pInputFC.Search(null, true);
            pNAClassLoader.Load((ICursor)pFeatureCursor, null, ref iRows, ref iRowsLocated);
            ((INAContextEdit)pNAContext).ContextChanged();
        }
        //窗口获取点
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
                    }                             }            }
             */

            if (clicked != true)
                return;
            IPoint ipNew;
            if (m_ipPoints == null)
            {
                m_ipPoints = new MultipointClass();
            }
            ipNew = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            object o = Type.Missing;
            m_ipPoints.AddPoint(ipNew, ref o, ref o);

            IFeature fea = pInputFC.CreateFeature();
            fea.Shape = m_ipPoints as IPoint;
            fea.Store();

            IElement element;
            ITextElement textelement = new TextElementClass();
            element = textelement as IElement;
            clickedcount++;
            textelement.Text = clickedcount.ToString();
            element.Geometry = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            pGraphicsContainer.AddElement(element, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        ////解决路径
        //public void SolvePath(string WeightName)
        //{
        //    try
        //    {
        //        int intEdgeUserClassID;
        //        int intEdgeUserID;
        //        int intEdgeUserSubID;
        //        int intEdgeID;
        //        IPoint ipFoundEdgePoint;
        //        double dblEdgePercent;
        //        /*C#中使用
        //         *ITraceFlowSolverGEN替代ITraceFlowSolver
        //         */
        //        ITraceFlowSolverGEN ipTraceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
        //        INetSolver ipNetSolver = ipTraceFlowSolver as INetSolver;
        //        //INetwork ipNetwork = m_ipGeometricNetwork.Network;
        //        ipNetSolver.SourceNetwork = pNetworkDataset as INetwork;
        //        //ipNetSolver.SourceNetwork = ipNetwork;
        //        INetElements ipNetElements = pNetworkDataset as INetElements;
        //        int intCount = m_ipPoints.PointCount;
        //        IPointToEID m_ipPointToEID;
        //        m_ipPointToEID = new PointToEIDClass();
        //        m_ipPointToEID.SourceMap = pMap;
        //        m_ipPointToEID.GeometricNetwork = pNetworkDataset as IGeometricNetwork;

        //        //定义一个边线旗数组
        //        IEdgeFlag[] pEdgeFlagList = new EdgeFlagClass[intCount];
        //        for (int i = 0; i < intCount; i++)
        //        {

        //            INetFlag ipNetFlag = new EdgeFlagClass() as INetFlag;
        //            IPoint ipEdgePoint = m_ipPoints.get_Point(i);
        //            //查找输入点的最近的边线
        //            m_ipPointToEID.GetNearestEdge(ipEdgePoint, out intEdgeID, out ipFoundEdgePoint, out dblEdgePercent);
        //            ipNetElements.QueryIDs(intEdgeID, esriElementType.esriETEdge, out intEdgeUserClassID, out intEdgeUserID, out intEdgeUserSubID);
        //            ipNetFlag.UserClassID = intEdgeUserClassID;
        //            ipNetFlag.UserID = intEdgeUserID;
        //            ipNetFlag.UserSubID = intEdgeUserSubID;
        //            IEdgeFlag pTemp = (IEdgeFlag)(ipNetFlag as IEdgeFlag);
        //            pEdgeFlagList[i] = pTemp;
        //        }
        //        ipTraceFlowSolver.PutEdgeOrigins(ref pEdgeFlagList);
        //        INetSchema ipNetSchema = ipNetwork as INetSchema;
        //        INetWeight ipNetWeight = ipNetSchema.get_WeightByName(WeightName);



        //        INetSolverWeights ipNetSolverWeights = ipTraceFlowSolver as INetSolverWeights;
        //        ipNetSolverWeights.FromToEdgeWeight = ipNetWeight;//开始边线的权重
        //        ipNetSolverWeights.ToFromEdgeWeight = ipNetWeight;//终止边线的权重
        //        object[] vaRes = new object[intCount - 1];
        //        //通过findpath得到边线和交汇点的集合
        //        ipTraceFlowSolver.FindPath(esriFlowMethod.esriFMConnected,
        //         esriShortestPathObjFn.esriSPObjFnMinSum,
        //         out m_ipEnumNetEID_Junctions, out m_ipEnumNetEID_Edges, intCount - 1, ref vaRes);
        //        //计算成本
        //        m_dblPathCost = 0;
        //        for (int i = 0; i < vaRes.Length; i++)
        //        {
        //            double m_Va = (double)vaRes[i];
        //            m_dblPathCost = m_dblPathCost + m_Va;
        //        }
        //        m_ipPolyline = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        private void SetSolverSettings()
        {
            //Set Route specific Settings
            INASolver naSolver = pNAContext.Solver;
            INARouteSolver cfSolver = naSolver as INARouteSolver;
            cfSolver.OutputLines = esriNAOutputLineType.esriNAOutputLineTrueShapeWithMeasure;
            // Set generic solver settings
            // Set the impedance attribute
            INASolverSettings naSolverSettings;
            naSolverSettings = naSolver as INASolverSettings;
            // Set the OneWay Restriction if necessary
            IStringArray restrictions;
            restrictions = naSolverSettings.RestrictionAttributeNames;
            restrictions.RemoveAll();
            restrictions.Add("oneway");
            naSolverSettings.RestrictionAttributeNames = restrictions;
            ////Restrict UTurns
            //naSolverSettings.RestrictUTurns = esriNetworkForwardStarBacktrack.esriNFSBNoBacktrack;
            //naSolverSettings.IgnoreInvalidLocations = true;
            // Do not forget to update the context after you set your impedance
            naSolver.UpdateContext(pNAContext, GetDENetworkDataset(pNAContext.NetworkDataset), new GPMessagesClass());
        }


        //路径分析
        private void btnSolver_Click(object sender, EventArgs e)
        {
            IAoInitialize m_AoInitialize = new AoInitializeClass();

            esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;

            licenseStatus = m_AoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
        
            clicked = false;
            this.Cursor = Cursors.WaitCursor;
            lstOutput.Items.Clear();
            lstOutput.Items.Add("分析中...");
            LoadNANetWorkLocations("Stops", pInputFC, 80);
            IGPMessages gpMessages = new GPMessagesClass();
            INASolver naSolver = pNAContext.Solver;
            SetSolverSettings();
            pNAContext.Solver.Solve(pNAContext, gpMessages, new CancelTrackerClass());

            if (gpMessages != null)
            {
                for (int i = 0; i < gpMessages.Count; i++)
                {
                    switch (gpMessages.GetMessage(i).Type)
                    {

                        case esriGPMessageType.esriGPMessageTypeError:
                            lstOutput.Items.Add("错误 " + gpMessages.GetMessage(i).ErrorCode.ToString() + " " + gpMessages.GetMessage(i).Description);
                            break;
                        case esriGPMessageType.esriGPMessageTypeWarning:
                            lstOutput.Items.Add("警告 " + gpMessages.GetMessage(i).Description);
                            break;
                        default:
                            lstOutput.Items.Add("信息 " + gpMessages.GetMessage(i).Description);
                            break;
                    }
                }
            }

            axMapControl1.Refresh();
            lstOutput.Items.Add("Successful");
            this.Cursor = Cursors.Default;
            CloseWorkspace();
        }

        private void CloseWorkspace()
        {
            m_ipPoints = null;

        }


        private void FindPath_Click(object sender, EventArgs e)
        {
            clicked = true;
        }

        //*********************************************************************************
        // Set Solver Settings设置阻抗属性等
        //*********************************************************************************
        /*
        public void SetSolverSettings()
        {
            //Set Route specific Settings
            INASolver naSolver = m_NAContext.Solver;

            INARouteSolver cfSolver = naSolver as INARouteSolver;
            //if (txtCutOff.Text.Length > 0 && IsNumeric(txtCutOff.Text.Trim()))
            //    cfSolver.DefaultCutoff = txtCutOff.Text;
            //else
            //    cfSolver.DefaultCutoff = null;

            //if (txtTargetFacility.Text.Length > 0 && IsNumeric(txtTargetFacility.Text))
            //    cfSolver.DefaultTargetFacilityCount = int.Parse(txtTargetFacility.Text);
            //else
            //    cfSolver.DefaultTargetFacilityCount = 1;

            cfSolver.OutputLines = esriNAOutputLineType.esriNAOutputLineTrueShapeWithMeasure;
            cfSolver.CreateTraversalResult = true;
            cfSolver.UseTimeWindows = false;
            cfSolver.FindBestSequence = false;
            cfSolver.PreserveFirstStop = false;
            cfSolver.PreserveLastStop = false;

            // Set generic solver settings设置Solver属性，设置路径分析阻抗属性
            // Set the impedance attribute
            INASolverSettings naSolverSettings;
            naSolverSettings = naSolver as INASolverSettings;
            naSolverSettings.ImpedanceAttributeName = cboCostAttribute.Text;

            // Set the OneWay Restriction if necessary设置单行限制
            IStringArray restrictions;
            restrictions = naSolverSettings.RestrictionAttributeNames;
            restrictions.RemoveAll();
            if (chkUseRestriction.Checked)
                restrictions.Add("oneway");

            naSolverSettings.RestrictionAttributeNames = restrictions;

            //Restrict UTurns限制U型转向限制
            naSolverSettings.RestrictUTurns = esriNetworkForwardStarBacktrack.esriNFSBNoBacktrack;
            naSolverSettings.IgnoreInvalidLocations = true;

            // Set the Hierachy attribute设置层次属性
            naSolverSettings.UseHierarchy = chkUseHierarchy.Checked;
            if (naSolverSettings.UseHierarchy)
            {
                naSolverSettings.HierarchyAttributeName = "hierarchy";
                naSolverSettings.HierarchyLevelCount = 3;
                naSolverSettings.set_MaxValueForHierarchy(1, 1);
                naSolverSettings.set_NumTransitionToHierarchy(1, 9);

                naSolverSettings.set_MaxValueForHierarchy(2, 2);
                naSolverSettings.set_NumTransitionToHierarchy(2, 9);
            }

            // Do not forget to update the context after you set your impedance
            naSolver.UpdateContext(m_NAContext, GetDENetworkDataset(m_NAContext.NetworkDataset), new GPMessagesClass());
        }

        //*********************************************************************************
        // Solve the problem 进行最短路径分析
        //*********************************************************************************
        public string Solve(INAContext naContext, IGPMessages gpMessages)
        {
            string errStr = "";
            try
            {
                //Solving the Problem
                errStr = "Error when solving";
                bool isPartialSolution = naContext.Solver.Solve(naContext, gpMessages, null);

                if (!isPartialSolution)
                    errStr = "OK";
                else
                    errStr = "Partial Solution";
            }
            catch (Exception e)
            {
                errStr += " Error Description " + e.Message;
            }
            return errStr;
        } */

    }
}
