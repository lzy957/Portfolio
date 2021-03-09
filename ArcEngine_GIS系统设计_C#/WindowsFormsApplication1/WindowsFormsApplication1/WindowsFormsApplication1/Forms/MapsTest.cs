using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Controls;




namespace WindowsFormsApplication1
{
    public partial class MapsTest : Form
    {
        public MapsTest()
        {
            InitializeComponent();
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
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            //产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 3;
            pOutline.Color = pColor;
            //设置颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            //设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pElement as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGraphicsContainer.AddElement((IElement)pFillShapeEle, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            
        }

        private void axMapControl1_OnMapReplaced(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMapReplacedEvent e)
        {
            if (axMapControl1.LayerCount > 0)
            {
                axMapControl2.Map = new MapClass();
                for (int i = 0; i <= axMapControl1.Map.LayerCount - 1; i++)
                {
                    axMapControl2.AddLayer(axMapControl1.get_Layer(i));
                }
                axMapControl2.Extent = axMapControl1.Extent;
                axMapControl2.Refresh();
            }
        }

        private void axMapControl2_OnMouseMove(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button == 1)
            {
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(e.mapX, e.mapY);
                axMapControl1.CenterAt(pPoint);
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }

        //进行命中测试

        private void button1_Click(object sender, EventArgs e)
        {
            IGeometry pGeo = axMapControl1.TrackPolygon();
            axMapControl1.Map.SelectByShape(pGeo, null, false);
            axMapControl1.ActiveView.Refresh();
        }
        #region 
        //打开GeoDataBase
        
        
        public IWorkspace GetMDBWorkspace(String _pGDBName)
        {
            IWorkspaceFactory pWsFac = new AccessWorkspaceFactoryClass();
            IWorkspace pWs = pWsFac.OpenFromFile(_pGDBName, 0);
            return pWs;
        }
         
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            string WsName = WSPath();
            if (WsName != "")
            {
                IWorkspaceFactory pWsFt = new AccessWorkspaceFactoryClass();
                IWorkspace pWs = pWsFt.OpenFromFile(WsName, 0);
                IEnumDataset pEDataset = pWs.get_Datasets(esriDatasetType.esriDTAny);
                IDataset pDataset = pEDataset.Next();
                while (pDataset != null)
                {
                    if (pDataset.Type == esriDatasetType.esriDTFeatureClass)
                    {
                        FeatureClassBox.Items.Add(pDataset.Name);
                    }
                    //如果是数据集
                    else if (pDataset.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        IEnumDataset pESubDataset = pDataset.Subsets;
                        IDataset pSubDataset = pESubDataset.Next();
                        while (pSubDataset != null)
                        {
                            FeatureClassBox.Items.Add(pSubDataset.Name);
                            pSubDataset = pESubDataset.Next();
                        }
                    }
                    pDataset = pEDataset.Next();
                }
 
            }
            FeatureClassBox.Text = FeatureClassBox.Items[0].ToString();
        }

        public string WSPath()
        {
            string WsFileName = "";
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "个人数据库(MDB)|*.mdb";
            DialogResult DialogR = OpenFile.ShowDialog();
            if (DialogR == DialogResult.Cancel)
            {
            }
            else
            {
                WsFileName = OpenFile.FileName;
            }
            return WsFileName;
        }

        //其中GetLayer函数是我们写的一个根据图层的名称获取图层的方法，代码如下图：
        private ILayer GetLayer(IMap pMap, string LayerName)
        {
            IEnumLayer pEnunLayer;
            pEnunLayer = pMap.get_Layers(null, false);
            pEnunLayer.Reset();
            ILayer pRetureLayer;
            pRetureLayer = pEnunLayer.Next();
            while (pRetureLayer != null)
            {
                if (pRetureLayer.Name == LayerName)
                {
                    break;
                }
                pRetureLayer = pEnunLayer.Next();
            }
            return pRetureLayer;
        }


        private void FeatureSelection()
        {
            IMap pMap = axMapControl1.Map;
            IFeatureLayer pFeaturelayer = GetLayer(pMap, "street") as IFeatureLayer;
            IFeatureSelection pFeatureSelection = pFeaturelayer as IFeatureSelection;
            IQueryFilter pQuery = new QueryFilterClass();
            pQuery.WhereClause = "TYPE=" + "'paved'";
            pFeatureSelection.SelectFeatures(pQuery, esriSelectionResultEnum.esriSelectionResultNew, false);
            axMapControl1.ActiveView.Refresh();
        }

        //查询矩形范围内的点要素
        public ESRI.ArcGIS.Geodatabase.IFeatureCursor GetAllFeaturesFromPointSearchInGeoFeatureLayer(ESRI.ArcGIS.Geometry.IEnvelope pEnvelope, IPoint pPoint, IFeatureClass pFeatureClass)
        {
            if (pPoint == null || pFeatureClass == null)
            {
                return null;
            }
            System.String pShapeFieldName = pFeatureClass.ShapeFieldName;
            ESRI.ArcGIS.Geodatabase.ISpatialFilter pSpatialFilter = new ESRI.ArcGIS.Geodatabase.SpatialFilterClass();
            pSpatialFilter.Geometry = pEnvelope;
            pSpatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelEnvelopeIntersects;
            pSpatialFilter.GeometryField = pShapeFieldName;
            ESRI.ArcGIS.Geodatabase.IFeatureCursor pFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);
            return pFeatureCursor;
 
        }

        //开启附件功能，并添加一个附件
        private void CreateAttachTable(IFeatureClass pFeatureClass, int pID, string pFilePath, string pFileType)
        {
            //要素表是否有附件表,数据库只能是10版本的
            ITableAttachments pTableAtt = pFeatureClass as ITableAttachments;
            if (pTableAtt.HasAttachments == false)
            {
                pTableAtt.AddAttachments();
            }
            //获取附件管理器
            IAttachmentManager pAttachmentManager = pTableAtt.AttachmentManager;
            //用二进制流读取数据
            IMemoryBlobStream pMemoryBlobStream = new MemoryBlobStreamClass();
            pMemoryBlobStream.LoadFromFile(pFilePath);
            //创建一个附件
            IAttachment pAttachment = new AttachmentClass();
            pAttachment.ContentType = pFileType;
            pAttachment.Name = System.IO.Path.GetFileName(pFilePath);
            pAttachment.Data = pMemoryBlobStream;
            //添加到表中
            pAttachmentManager.AddAttachment(pID, pAttachment);
        }



        /// <summary>
        /// 输出结果为一个张表，这张表有3个字段，其中面ID为面要素数据的FID
        /// 个数用于记录这个面包含的点的个数
        /// </summary>
        /// <param name="_TablePath "></param>
        /// <param name="_TableName "></param>
        /// <returns></returns>
        public ITable CreateTable(string _TablePath, string _TableName)
        {
            IWorkspaceFactory pWks = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFwk = pWks.OpenFromFile(_TablePath, 0) as IFeatureWorkspace;
            //用于记录面中的ID;
            IField pFieldID = new FieldClass();
            IFieldEdit pFieldIID = pFieldID as IFieldEdit;
            pFieldIID.Type_2 = esriFieldType.esriFieldTypeInteger;
            pFieldIID.Name_2 = "面ID";
            //用于记录个数的;
            IField pFieldCount = new FieldClass();
            IFieldEdit pFieldICount = pFieldCount as IFieldEdit;
            pFieldICount.Type_2 = esriFieldType.esriFieldTypeInteger;
            pFieldICount.Name_2 = "个数";
            //用于添加表中的必要字段
            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription = new ESRI.ArcGIS.Geodatabase.ObjectClassDescriptionClass();
            IFields pTableFields = objectClassDescription.RequiredFields;
            IFieldsEdit pTableFieldsEdit = pTableFields as IFieldsEdit;
            pTableFieldsEdit.AddField(pFieldID);
            pTableFieldsEdit.AddField(pFieldCount);
            ITable pTable = pFwk.CreateTable(_TableName, pTableFields, null, null, "");
            return pTable;
        }

        /// <summary>
        /// 第一个参数为面数据，第二个参数为点数据，第三个为输出的表
        /// </summary>
        /// <param name="_pPolygonFClass"></param>
        /// <param name="_pPointFClass"></param>
        /// <param name="_pTable"></param>
        public void StatisticPointCount(IFeatureClass _pPolygonFClass, IFeatureClass _pPointFClass, ITable _pTable)
        {
            IFeatureCursor pPolyCursor = _pPolygonFClass.Search(null, false);
            IFeature pPolyFeature = pPolyCursor.NextFeature();
            while (pPolyFeature != null)
            {
                IGeometry pPolGeo = pPolyFeature.Shape;
                int Count = 0;
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = pPolGeo;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                IFeatureCursor pPointCur = _pPointFClass.Search(spatialFilter, false);
                if (pPointCur != null)
                {
                    IFeature pPointFeature = pPointCur.NextFeature();
                    while (pPointFeature != null)
                    {
                        pPointFeature = pPointCur.NextFeature();
                        Count++;
                    }
                }
                if (Count != 0)
                {
                    IRow pRow = _pTable.CreateRow();
                    pRow.set_Value(1, pPolyFeature.get_Value(0));
                    pRow.set_Value(2, Count);
                    pRow.Store();
                }
                pPolyFeature = pPolyCursor.NextFeature();
            }
        }

        //数据转换
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_pSWorkspaceFactory"></param>
        /// <param name="_pSWs"></param>
        /// <param name="_pSName"></param>
        /// <param name="_pTWorkspaceFactory"></param>
        /// <param name="_pTWs"></param>
        /// <param name="_pTName"></param>
        public void ConvertFeatureClass(IWorkspaceFactory _pSWorkspaceFactory, String _pSWs, string _pSName, IWorkspaceFactory _pTWorkspaceFactory, String _pTWs, string _pTName)
        {
            // Open the source and target workspaces.
            IWorkspace pSWorkspace = _pSWorkspaceFactory.OpenFromFile(_pSWs, 0);
            IWorkspace pTWorkspace = _pTWorkspaceFactory.OpenFromFile(_pTWs, 0);
            IFeatureWorkspace pFtWs = pSWorkspace as IFeatureWorkspace;
            IFeatureClass pSourceFeatureClass = pFtWs.OpenFeatureClass(_pSName);
            IDataset pSDataset = pSourceFeatureClass as IDataset;
            IFeatureClassName pSourceFeatureClassName = pSDataset.FullName as IFeatureClassName;
            IDataset pTDataset = (IDataset)pTWorkspace;
            IName pTDatasetName = pTDataset.FullName;
            IWorkspaceName pTargetWorkspaceName = (IWorkspaceName)pTDatasetName;
            IFeatureClassName pTargetFeatureClassName = new FeatureClassNameClass();
            IDatasetName pTargetDatasetName = (IDatasetName)pTargetFeatureClassName;
            pTargetDatasetName.Name = _pTName;
            pTargetDatasetName.WorkspaceName = pTargetWorkspaceName;

            // 创建字段检查对象
            IFieldChecker pFieldChecker = new FieldCheckerClass();
            IFields sourceFields = pSourceFeatureClass.Fields;
            IFields pTargetFields = null;
            IEnumFieldError pEnumFieldError = null;
            pFieldChecker.InputWorkspace = pSWorkspace;
            pFieldChecker.ValidateWorkspace = pTWorkspace;
            // 验证字段
            pFieldChecker.Validate(sourceFields, out pEnumFieldError, out pTargetFields);
            if (pEnumFieldError != null)
            {
                // Handle the errors in a way appropriate to your application.
                Console.WriteLine("Errors were encountered during field validation.");
            }
            String pShapeFieldName = pSourceFeatureClass.ShapeFieldName;
            int pFieldIndex = pSourceFeatureClass.FindField(pShapeFieldName);
            IField pShapeField = sourceFields.get_Field(pFieldIndex);
            IGeometryDef pTargetGeometryDef = pShapeField.GeometryDef;
            // 创建要素转换对象
            IFeatureDataConverter pFDConverter = new FeatureDataConverterClass();
            IEnumInvalidObject pEnumInvalidObject = pFDConverter.ConvertFeatureClass
            (pSourceFeatureClassName, null, null, pTargetFeatureClassName,
            pTargetGeometryDef, pTargetFields, "", 1000, 0);
            // Check for errors.
            IInvalidObjectInfo pInvalidInfo = null;
            pEnumInvalidObject.Reset();
            while ((pInvalidInfo = pEnumInvalidObject.Next()) != null)
            {
                // Handle the errors in a way appropriate to the application.
                Console.WriteLine("Errors occurred for the following feature: {0}",
                pInvalidInfo.InvalidObjectID);
            }
        }


        //创建经纬度点
        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private IPoint ConstructPoint(double x, double y)
        {
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(x, y);
            return pPoint;
        }


        //创建MulitPoint 多点集
        private object pMissing = Type.Missing;
        public IGeometry GetMultipointGeometry()
        {
            const double MultipointPointCount = 25;
            IPointCollection pPointCollection = new MultipointClass();
            for (int i = 0; i < MultipointPointCount; i++)
            {
                pPointCollection.AddPoint(GetPoint(), ref PMissing, ref PMissing);
            }
            return pPointCollection as IGeometry;
        }
        private IPoint GetPoint()
        {
            const double Min = -10;
            const double Max = 10;
            Random pRandom = new Random();
            double x = Min + (Max - Min) * pRandom.NextDouble();
            double y = Min + (Max - Min) * pRandom.NextDouble();
            return ConstructPoint(x, y);
        }


        //PloyLine的创建
        private object PMissing = Type.Missing;
        public IGeometry GetPolylineGeometry()
        {
            const double PathCount = 3;
            const double PathVertexCount = 3;
            IGeometryCollection pGeometryCollection = new PolylineClass();
            for (int i = 0; i < PathCount; i++)
            {
                IPointCollection pPointCollection = new PathClass();
                for (int j = 0; j < PathVertexCount; j++)
                {
                    pPointCollection.AddPoint(GetPoint(), ref PMissing, ref PMissing);
                }
                pGeometryCollection.AddGeometry(pPointCollection as IGeometry, ref PMissing, ref PMissing);
            }
            return pGeometryCollection as IGeometry;
        }



        //Polygon多边形的构造
        /// <summary>
        /// 通过多个点构造面
        /// </summary>
        /// <param name="pPointCollection"></param>
        /// <returns></returns>
        public IPolygon CreatePolygonByPoints(IPointCollection pPointCollection)
        {
            return null;
 
        }


        //平头缓冲
        private IPolygon FlatBuffer(IPolyline pLline1, double pBufferDis)
        {
            object o = System.Type.Missing;
            //分别对输入的线平移两次（正方向和负方向）
            IConstructCurve pCurve1 = new PolylineClass();
            pCurve1.ConstructOffset(pLline1, pBufferDis, ref o, ref o);
            IPointCollection pCol = pCurve1 as IPointCollection;
            IConstructCurve pCurve2 = new PolylineClass();
            pCurve2.ConstructOffset(pLline1, -1 * pBufferDis, ref o, ref o);
            //把第二次平移的线的所有节点翻转
            IPolyline pline2 = pCurve2 as IPolyline;
            pline2.ReverseOrientation();
            //把第二条的所有节点放到第一条线的IPointCollection里面
            IPointCollection pCol2 = pline2 as IPointCollection;
            pCol.AddPointCollection(pCol2);
            //用面去初始化一个IPointCollection
            IPointCollection pPointCol = new PolygonClass();
            pPointCol.AddPointCollection(pCol);
            //把IPointCollection转换为面
            IPolygon pPolygon = pPointCol as IPolygon;
            //简化节点次序
            pPolygon.SimplifyPreserveFromTo();
            return pPolygon;
        }

        //通过UGeometryCollection接口创建一个Polygon对象
        /// <summary>
        /// 通过IGeometryCollection创建一个Polygon对象的代码片段如下：
        /// </summary>
        /// <param name="pRingList"></param>
        /// <returns></returns>
        private IPolygon ConstructorPolygon(List<IRing> pRingList)
        {
            try
            {
                IGeometryCollection pGCollection = new PolygonClass();
                object o = Type.Missing;
                for (int i = 0; i < pRingList.Count; i++)
                {
                    //通过IGeometryCollection接口的AddGeometry方法向Polygon对象中添加Ring子对象
                    pGCollection.AddGeometry(pRingList[i], ref o, ref o);
                }
                //QI至ITopologicalOperator
                ITopologicalOperator pTopological = pGCollection as ITopologicalOperator;
                //执行Simplify操作
                pTopological.Simplify();
                IPolygon pPolygon = pGCollection as IPolygon;
                //返回Polygon对象
                return pPolygon;
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.ToString());
                return null;
            }
        }


        private IPolygon MergePolygons(IPolygon firstPolygon, IPolygon SecondPolygon)
        {
            try
            {
                //创建一个Polygon对象
                IGeometryCollection pGCollection1 = new PolygonClass();
                IGeometryCollection pGCollection2 = firstPolygon as IGeometryCollection;
                IGeometryCollection pGCollection3 = SecondPolygon as IGeometryCollection;
                //添加firstPolygon
                pGCollection1.AddGeometryCollection(pGCollection2);
                //添加SecondPolygon
                pGCollection1.AddGeometryCollection(pGCollection3);
                //QI至ITopologicalOperator
                ITopologicalOperator pTopological = pGCollection1 as ITopologicalOperator;
                //执行Simplify操作
                pTopological.Simplify();
                IPolygon pPolygon = pGCollection1 as IPolygon;
                //返回Polygon对象
                return pPolygon;
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.ToString());
                return null;
            }
        }

        //空间参考（Spatial Reference）是GIS数据的骨骼框架，能够将我们的数据定位到相应的位置，为地图中的每一点提供准确的坐标。 在同一个地图上显示的地图数据的空间参考必须是一致的，如果两个图层的空间参考不一致，往往会导致两幅地图无法正确拼合，因此开发一个GIS系统时，为数据选择正确的空间参考非常重要。




        //拓扑关系查询，比如：查找距离超市1000米内有多少居民。这些居民中有多少潜在顾客。这也是一个典型的缓冲区分析，实际上就是给超市做了个1000米得缓冲区，然后用这个缓冲区和居民数据叠加，进而挖掘潜在顾客。
        private void TopologicualQuery(ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            IMap pMap = axMapControl1.Map;
            IActiveView pActView = pMap as IActiveView;
            IPoint pt = pActView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            ITopologicalOperator pTopo = pt as ITopologicalOperator;
            IGeometry pGeo = pTopo.Buffer(500);
            ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
            rgbColor.Red = 255;
            ESRI.ArcGIS.Display.IColor color = rgbColor; // Implicit Cast
            ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
            simpleFillSymbol.Color = color;
            ESRI.ArcGIS.Display.ISymbol symbol = simpleFillSymbol as ESRI.ArcGIS.Display.ISymbol;
            pActView.ScreenDisplay.SetSymbol(symbol);
            pActView.ScreenDisplay.DrawPolygon(pGeo);
            pMap.SelectByShape(pGeo, null, false);
            //闪动1000次
            axMapControl1.FlashShape(pGeo, 10, 2, symbol);
            axMapControl1.ActiveView.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           // TopologicualQuery(e);
        }

        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            TopologicualQuery(e);
        }


        //矢量格式的地图输出
        /// <summary>
        /// 示例：输出EMF格式：
        /// </summary>
        private void ExportEMF()
        {
            IActiveView pActiveView;
            pActiveView = axPageLayoutControl1.ActiveView;
            IExport pExport;
            pExport = new ExportEMFClass();
            pExport.ExportFileName = @"C:\Users\Administrator\Desktop\作业\空间分析\实习walkscore\ExportEMF.emf";
            pExport.Resolution = 300;
            tagRECT exportRECT;
            exportRECT = pActiveView.ExportFrame;
            IEnvelope pPixelBoundsEnv;
            pPixelBoundsEnv = new EnvelopeClass();
            pPixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.top,
            exportRECT.right, exportRECT.bottom);
            pExport.PixelBounds = pPixelBoundsEnv;
            int hDC;
            hDC = pExport.StartExporting();
            pActiveView.Output(hDC, (int)pExport.Resolution, ref exportRECT, null, null);
            pExport.FinishExporting();
            pExport.Cleanup();
        }
        /// <summary>
        /// 
        /// </summary>
        private void ExportPDF()
        {
            IActiveView pActiveView;
            pActiveView = axPageLayoutControl1.ActiveView;
            IEnvelope pEnv;
            pEnv = pActiveView.Extent;
            IExport pExport;
            pExport = new ExportPDFClass();
            pExport.ExportFileName = @"C:\Users\Administrator\Desktop\ae\WindowsFormsApplication3\ExportPDF.pdf";
            pExport.Resolution = 30;
            tagRECT exportRECT;
            exportRECT.top = 0;
            exportRECT.left = 0;
            exportRECT.right = (int)pEnv.Width;
            exportRECT.bottom = (int)pEnv.Height;
            IEnvelope pPixelBoundsEnv;
            pPixelBoundsEnv = new EnvelopeClass();
            pPixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.bottom, exportRECT.right, exportRECT.top);
            pExport.PixelBounds = pPixelBoundsEnv;
            int hDC;
            hDC = pExport.StartExporting();
            pActiveView.Output(hDC, (int)pExport.Resolution, ref exportRECT, null, null);
            pExport.FinishExporting();
            pExport.Cleanup();

        }


        //坐标点生成
        /// <summary>
        /// 模拟Addxy
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="pSpatialReference"></param>
        /// <returns></returns>
        public IFeatureClass CreateXYEventSource(ITable pTable, ISpatialReference pSpatialReference)
        {
            IXYEvent2FieldsProperties pEvent2FieldsProperties = new XYEvent2FieldsPropertiesClass();
            pEvent2FieldsProperties.XFieldName = "X";
            pEvent2FieldsProperties.YFieldName = "Y";
            IDataset pSourceDataset = (IDataset)pTable;
            IName sourceName = pSourceDataset.FullName;
            IXYEventSourceName pEventSourceName = new XYEventSourceNameClass();
            pEventSourceName.EventProperties = pEvent2FieldsProperties; pEventSourceName.EventTableName = sourceName; pEventSourceName.SpatialReference = pSpatialReference;
            IName pName = (IName)pEventSourceName;
            IXYEventSource pEventSource = (IXYEventSource)pName.Open();
            IFeatureClass pFeatureClass = (IFeatureClass)pEventSource; return pFeatureClass;
 
        }

        //排序
        /// <summary>
/// true 表示升序，false 表示降序
/// </summary>
/// <param name="_pTable"></param>
/// <param name="_FieldName"></param>
/// <param name="_Bool"></param>
        void Sort(ITable _pTable, string _FieldName, bool _Bool)
        {
            ITableSort pTableSort = new TableSortClass();
            pTableSort.Table = _pTable;
            pTableSort.Fields = _FieldName;
            pTableSort.set_Ascending(_FieldName, _Bool);
            pTableSort.Sort(null);
            ICursor pSortCursor = pTableSort.Rows;
            IRow pSortRow = pSortCursor.NextRow();
            IDataset plSortDataset = _pTable as IDataset;
            IFeatureWorkspace pFWs = plSortDataset.Workspace as IFeatureWorkspace;
            ITable plStable = pFWs.CreateTable("NewSort", _pTable.Fields, null, null, null);
            while (pSortRow != null)
            {
                IRow pRow = plStable.CreateRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    if (pRow.Fields.get_Field(i).Type != esriFieldType.esriFieldTypeOID)
                    {
                        pRow.set_Value(i, pSortRow.get_Value(i));
                    }
                }
                pRow.Store();
                pSortRow = pSortCursor.NextRow();
            }
        }
        /*代码创建几个网络代码创建几个网络代码创建几个网络代码创建几个网络代码创创建几个网代码创建几个网络代码创建几个网络代码创建几个网络代码创建几个网络代码创建几个网络*/
        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void MapsTest_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 打开个人地理信息数据库
        /// </summary>
        /// <param name="_pGDBName"></param>
        /// <returns></returns>
        /// 
       
        public IWorkspace GetWorkspace(String _pGDBName)
        {
            IWorkspaceFactory pWsFac = new AccessWorkspaceFactoryClass();
            IWorkspace pWs = pWsFac.OpenFromFile(_pGDBName, 0);
            return pWs;
        }
        public void CreateGeometricNetwork(IWorkspace _pWorkspace, IFeatureDatasetName _pFeatureDatasetName, String _pGeometricName)
        {
            INetworkLoader2 pNetworkLoader = new NetworkLoaderClass();
            // 网络的名称
            pNetworkLoader.NetworkName = _pGeometricName;
            // 网络的类型
            pNetworkLoader.NetworkType = esriNetworkType.esriNTUtilityNetwork;
            // Set the containing feature dataset.
            pNetworkLoader.FeatureDatasetName = (IDatasetName)_pFeatureDatasetName;
            // 检查要建立几何网络的数据，每一个要素只能参与一个网络
            if (pNetworkLoader.CanUseFeatureClass("PrimaryLine") ==
            esriNetworkLoaderFeatureClassCheck.esriNLFCCValid)
            {
                pNetworkLoader.AddFeatureClass("PrimaryLine",
                esriFeatureType.esriFTComplexEdge, null, false);
            }
            if (pNetworkLoader.CanUseFeatureClass("Feeder") ==
            esriNetworkLoaderFeatureClassCheck.esriNLFCCValid)
            {
                pNetworkLoader.AddFeatureClass("Feeder", esriFeatureType.esriFTSimpleJunction,
                null, false);
            }
            // 我的数据中没有enable字段，所以，用了false，如果用true的话，就要进行相关的设置
            INetworkLoaderProps pNetworkLoaderProps = (INetworkLoaderProps)pNetworkLoader;
            pNetworkLoader.PreserveEnabledValues = false;
            // Set the ancillary role field for the Feeder class.
            String defaultAncillaryRoleFieldName =
            pNetworkLoaderProps.DefaultAncillaryRoleField;
            esriNetworkLoaderFieldCheck ancillaryRoleFieldCheck =
            pNetworkLoader.CheckAncillaryRoleField("Feeder",
            defaultAncillaryRoleFieldName);
            switch (ancillaryRoleFieldCheck)
            {
                case esriNetworkLoaderFieldCheck.esriNLFCValid:
                case esriNetworkLoaderFieldCheck.esriNLFCNotFound:
                    pNetworkLoader.PutAncillaryRole("Feeder",
                    esriNetworkClassAncillaryRole.esriNCARSourceSink,
                    defaultAncillaryRoleFieldName);
                    break;
                default:
                    Console.WriteLine(
                    "The field {0} could not be used as an ancillary role field.",
                    defaultAncillaryRoleFieldName);
                    break;
            }
            pNetworkLoader.SnapTolerance = 0.02;
            // 给几何网络添加权重
            pNetworkLoader.AddWeight("Weight", esriWeightType.esriWTDouble, 0);
            // 将权重和PrimaryLine数据中的SHAPE_Length字段关联
            pNetworkLoader.AddWeightAssociation("Weight", "PrimaryLine", "SHAPE_Length");
            // 构建网络
            pNetworkLoader.LoadNetwork();
        }

        private void GetGepmetryNetWork()
        {
            IWorkspace pWs = GetWorkspace(@"C:\Users\Administrator\Desktop\作业\空间分析\实习walkscore\walkscoredata\geo.mdb");
            IFeatureWorkspace pFtWs = pWs as IFeatureWorkspace;
            IFeatureDataset pFtDataset = pFtWs.OpenFeatureDataset("geometric");
            IDataset pDataset = pFtDataset as IDataset;
            IFeatureDatasetName pFtDatasetName = pDataset.FullName as IFeatureDatasetName;
            CreateGeometricNetwork(pWs, pFtDatasetName, "geometric");

        }
         
        //代码实现几何网络的最短路径分析
     
        /// 最短路径分析
        /// </summary>
        /// <param name="_pMap"></param>
        /// <param name="_pGeometricNetwork"></param>
        /// <param name="_pWeightName"></param>
        /// <param name="_pPoints"></param>
        /// <param name="_pDist"></param>
        /// <param name="_pPolyline"></param>
        /// <param name="_pPathCost"></param>
        public void SolvePath(IMap _pMap, IGeometricNetwork _pGeometricNetwork, string _pWeightName, IPointCollection _pPoints, double _pDist, ref IPolyline _pPolyline, ref double _pPathCost)
        {
            try
            { // 这4个参数其实就是一个定位Element的指标
                int intEdgeUserClassID;
                int intEdgeUserID;
                int intEdgeUserSubID;
                int intEdgeID;
                IPoint pFoundEdgePoint;
                double dblEdgePercent;
                ITraceFlowSolverGEN pTraceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
                INetSolver pNetSolver = pTraceFlowSolver as INetSolver;
                //操作是针对逻辑网络的,INetwork是逻辑网络
                INetwork pNetwork = _pGeometricNetwork.Network;
                pNetSolver.SourceNetwork = pNetwork;
                INetElements pNetElements = pNetwork as INetElements;
                int pCount = _pPoints.PointCount;
                //定义一个边线旗数组
                IEdgeFlag[] pEdgeFlagList = new EdgeFlagClass[pCount];
                IPointToEID pPointToEID = new PointToEIDClass();
                pPointToEID.SourceMap = _pMap;
                pPointToEID.GeometricNetwork = _pGeometricNetwork;
                pPointToEID.SnapTolerance = _pDist;
                for (int i = 0; i < pCount; i++)
                {
                    INetFlag pNetFlag = new EdgeFlagClass() as INetFlag;
                    IPoint pEdgePoint = _pPoints.get_Point(i);
                    //查找输入点的最近的边线
                    pPointToEID.GetNearestEdge(pEdgePoint, out intEdgeID, out pFoundEdgePoint, out dblEdgePercent);
                    pNetElements.QueryIDs(intEdgeID, esriElementType.esriETEdge, out intEdgeUserClassID, out intEdgeUserID, out intEdgeUserSubID);
                    pNetFlag.UserClassID = intEdgeUserClassID;
                    pNetFlag.UserID = intEdgeUserID;
                    pNetFlag.UserSubID = intEdgeUserSubID;
                    IEdgeFlag pTemp = (IEdgeFlag)(pNetFlag as IEdgeFlag);
                    pEdgeFlagList[i] = pTemp;
                }
                pTraceFlowSolver.PutEdgeOrigins(ref pEdgeFlagList);
                INetSchema pNetSchema = pNetwork as INetSchema;
                INetWeight pNetWeight = pNetSchema.get_WeightByName(_pWeightName);
                INetSolverWeightsGEN pNetSolverWeights = pTraceFlowSolver as INetSolverWeightsGEN;
                pNetSolverWeights.FromToEdgeWeight = pNetWeight;//开始边线的权重
                pNetSolverWeights.ToFromEdgeWeight = pNetWeight;//终止边线的权重
                object[] pRes = new object[pCount - 1];
                //通过FindPath得到边线和交汇点的集合
                IEnumNetEID pEnumNetEID_Junctions;
                IEnumNetEID pEnumNetEID_Edges;
                pTraceFlowSolver.FindPath(esriFlowMethod.esriFMConnected,
                esriShortestPathObjFn.esriSPObjFnMinSum,
                out pEnumNetEID_Junctions, out pEnumNetEID_Edges, pCount - 1, ref pRes);
                //计算元素成本
                _pPathCost = 0;
                for (int i = 0; i < pRes.Length; i++)
                {
                    double m_Va = (double)pRes[i];
                    _pPathCost = _pPathCost + m_Va;
                }
                IGeometryCollection pNewGeometryColl = _pPolyline as IGeometryCollection;//QI
                ISpatialReference pSpatialReference = _pMap.SpatialReference;
                IEIDHelper pEIDHelper = new EIDHelperClass();
                pEIDHelper.GeometricNetwork = _pGeometricNetwork;
                pEIDHelper.OutputSpatialReference = pSpatialReference;
                pEIDHelper.ReturnGeometries = true;
                IEnumEIDInfo pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(pEnumNetEID_Edges);
                int Count = pEnumEIDInfo.Count;
                pEnumEIDInfo.Reset();
                for (int i = 0; i < Count; i++)
                {
                    IEIDInfo pEIDInfo = pEnumEIDInfo.Next();
                    IGeometry pGeometry = pEIDInfo.Geometry;
                    pNewGeometryColl.AddGeometryCollection(pGeometry as IGeometryCollection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void www()
        {
 
        }
        //添加临时元素到地图上
        public static void AddTempElement(AxMapControl pMapCtrl, IElement pEle, IElementCollection pEleColl)
        {
            try
            {
                IMap pMap = pMapCtrl.Map;
                IGraphicsContainer pGCs = pMap as IGraphicsContainer;
                if (pEle != null)
                    pGCs.AddElement(pEle, 0);

                if (pEleColl != null)
                    if (pEleColl.Count > 0)
                        pGCs.AddElements(pEleColl, 0);
                IActiveView pAV = (IActiveView)pMap;
                //需要刷新才能即时显示
                pAV.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, pAV.Extent);


            }
            catch (Exception Err)
            {

               // throw;
                MessageBox.Show(Err.Message, "提示", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }


        //获取图层符合条件 的要素
        /// /// <summary> 
        /// 获取要素图层符合条件获取选择集
        /// <param name="pFeatureLayer">要素图层</param> 
        /// <param name="WhereClause">过滤条件</param>
        /// <returns>返回选择集</returns>
        private IFeatureSelection SelectLayersFeatures(IFeatureLayer pFeatureLayer, string WhereClause)
        {
            IFeatureSelection pFeatureSelection = pFeatureLayer as IFeatureSelection;

            if (pFeatureSelection == null) return null;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = WhereClause;
            pFeatureSelection.SelectFeatures(pQueryFilter,
            esriSelectionResultEnum.esriSelectionResultNew, false);

            return pFeatureSelection;
        }

        /// <summary>
        /// 按行政区范围创建行政区范围的图层
        /// </summary>
        /// <param name="pFeatureLayer">源数据图层</param>
        /// <param name="pGeometry">行政区范围</param>
        /// <param name="bXZQ">图层是否为行政区</param>
        /// <returns></returns>
        private IFeatureLayer GetSelectionLayer(IFeatureLayer pFeatureLayer, IGeometry pGeometry, bool bXZQ)
        {
            try
            {
                if (pFeatureLayer != null && pGeometry != null)
                {
                    IQueryFilter pQueryFilter;
                    ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                    IFeatureSelection pFeatureSelection = pFeatureLayer as   IFeatureSelection;

                    pSpatialFilter.GeometryField =
                    pFeatureLayer.FeatureClass.ShapeFieldName;

                    pFeatureSelection.Clear();
                    if (!bXZQ)
                    {
                        pSpatialFilter.Geometry = pGeometry;
                        pSpatialFilter.SpatialRel =
                        esriSpatialRelEnum.esriSpatialRelIntersects;

                        pQueryFilter = pSpatialFilter;
                        pFeatureSelection.SelectFeatures(pQueryFilter,
                        esriSelectionResultEnum.esriSelectionResultNew, false);

                    }
                    else
                    {
                        pSpatialFilter.SpatialRel =
                        esriSpatialRelEnum.esriSpatialRelContains;

                        pQueryFilter = pSpatialFilter;
                        if (pGeometry is IGeometryCollection)
                        {
                            for (int i = 0; i < (pGeometry as
                            IGeometryCollection).GeometryCount; i++)
                            {
                                pSpatialFilter.Geometry = (pGeometry as
                                                IGeometryCollection).get_Geometry(i);

                                pFeatureSelection.SelectFeatures(pQueryFilter,
                                esriSelectionResultEnum.esriSelectionResultAdd, false);

                            }
                        }
                    }
                    IFeatureLayerDefinition pFLDefinition = pFeatureLayer as
                    IFeatureLayerDefinition;

                    IFeatureLayer pNewFeatureLayer =
                    pFLDefinition.CreateSelectionLayer(pFeatureLayer.Name, true, null, null);

                    pNewFeatureLayer.MaximumScale = pFeatureLayer.MaximumScale;
                    pNewFeatureLayer.MinimumScale = pFeatureLayer.MinimumScale;
                    pNewFeatureLayer.Selectable = pFeatureLayer.Selectable;
                    pNewFeatureLayer.Visible = pFeatureLayer.Visible;
                    pNewFeatureLayer.ScaleSymbols = pFeatureLayer.ScaleSymbols;
                    return pNewFeatureLayer;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception)
            {

                throw;
            }
 
        }


        /// <summary>
        /// 向pagelayout上的指定位置添加辅助线
        /// </summary>
        /// <param name="pPageLayout">对象</param>
        /// <param name="pPoistion">位置</param>
        /// <param name="bHorizontal">true为水平方向辅助线，False为垂直方向辅助线</param>
        private void AddGuideOnPageLayout(IPageLayout pPageLayout, double pPoistion, bool bHorizontal)
        {
            try
            {
                if (pPageLayout != null)
                {
                    ISnapGuides pSnapGuides = null;
                    //如果是水平辅助线
                    if (bHorizontal)
                    {
                        pSnapGuides = pPageLayout.HorizontalSnapGuides;
                    }
                    //如果是垂直辅助线
                    else
                    {
                        pSnapGuides = pPageLayout.VerticalSnapGuides;
                    }
                    if (pSnapGuides != null)
                    {
                        //向PageLayout上添加辅助线
                        pSnapGuides.AddGuide(pPoistion);
                    }
                }
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message, "提示", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
        }


        /// <summary>
        /// 为PageLayout对象添加图例对象
        /// </summary>
        /// <param name="pPageLayout">PageLayout对象</param>
        /// <param name="pEnvelope">图例添加的位置</param>
        private void AddLegendToPageLayout(IPageLayout pPageLayout, IEnvelope pEnvelope)
        {
            try
            {
                IActiveView pActiveView = pPageLayout as IActiveView;
                IMap pMap = pActiveView.FocusMap;
                IGraphicsContainer pGraphicsContainer = pActiveView as
                IGraphicsContainer;

                IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as
                IMapFrame;

                UID pUID = new UID();
                pUID.Value = "{7A3F91E4‐B9E3‐11d1‐8756‐0000F8751720}";
                ISymbolBackground pSymbolBackground = new SymbolBackgroundClass();
                IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
                pFillSymbol.Color = GetRgbColor(255, 255, 255); 
                pLineSymbol.Color = GetRgbColor(255, 255, 255);
                pFillSymbol.Outline = pLineSymbol;
                pSymbolBackground.FillSymbol = pFillSymbol;
                IMapSurroundFrame pMapSurroundFrame =
                pMapFrame.CreateSurroundFrame(pUID, null);

                pMapSurroundFrame.Background = pSymbolBackground;
                IElement pElement = pMapSurroundFrame as IElement;
                pElement.Geometry = pEnvelope;
                IMapSurround pMapSurround = pMapSurroundFrame.MapSurround;
                ILegend pLegend = pMapSurround as ILegend;
                pLegend.ClearItems();
                pLegend.Title = "图例";
                ITextSymbol pTextSymbol = new TextSymbolClass();
                pTextSymbol.Size = 10;
                pTextSymbol.HorizontalAlignment =
                esriTextHorizontalAlignment.esriTHALeft;

                ILegendItem pLegendItem = null;
                for (int i = 0; i < pActiveView.FocusMap.LayerCount; i++)
                {
                    ILayer pLayer = pActiveView.FocusMap.get_Layer(i);
                    if (pLayer is IFeatureLayer)
                    {
                        IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                        IFeatureClass pFeatureClass = pFLayer.FeatureClass;
                        if (pFeatureClass.FeatureType ==
                        esriFeatureType.esriFTAnnotation)
                        {
                            continue;
                        }
                        else
                        {
                            pLegendItem = new HorizontalLegendItemClass();
                            pLegendItem.Layer = pLayer;
                            pLegendItem.Columns = 1;
                            pLegendItem.ShowDescriptions = false;
                            pLegendItem.ShowHeading = false;
                            pLegendItem.ShowLabels = true;
                            pLegendItem.LayerNameSymbol = pTextSymbol;
                            pLegend.AddItem(pLegendItem);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
 
        }

        private IRgbColor GetRgbColor(int r, int g, int b)
        {
            IRgbColor pRgbColor = new RgbColorClass();//构建一个RgbColorClass
            pRgbColor.Red = r;//设置Red属性
            pRgbColor.Green = g;//设置Green属性
            pRgbColor.Blue = b;//设置Blue属性
            return pRgbColor;
        }

        //Network DataSet分析
    }
}
