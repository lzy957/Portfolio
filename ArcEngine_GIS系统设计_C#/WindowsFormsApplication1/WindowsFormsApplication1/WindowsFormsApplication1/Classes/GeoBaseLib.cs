using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesRaster;
//using System.Text.RegularExpressions;

namespace BaseLibs
{
    /// The different layer GUID's and Interface's are:
    /// "{AD88322D-533D-4E36-A5C9-1B109AF7A346}" = IACFeatureLayer
    /// "{74E45211-DFE6-11D3-9FF7-00C04F6BC6A5}" = IACLayer
    /// "{495C0E2C-D51D-4ED4-9FC1-FA04AB93568D}" = IACImageLayer
    /// "{65BD02AC-1CAD-462A-A524-3F17E9D85432}" = IACAcetateLayer
    /// "{4AEDC069-B599-424B-A374-49602ABAD308}" = IAnnotationLayer
    /// "{DBCA59AC-6771-4408-8F48-C7D53389440C}" = IAnnotationSublayer
    /// "{E299ADBC-A5C3-11D2-9B10-00C04FA33299}" = ICadLayer
    /// "{7F1AB670-5CA9-44D1-B42D-12AA868FC757}" = ICadastralFabricLayer
    /// "{BA119BC4-939A-11D2-A2F4-080009B6F22B}" = ICompositeLayer
    /// "{9646BB82-9512-11D2-A2F6-080009B6F22B}" = ICompositeGraphicsLayer
    /// "{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}" = ICoverageAnnotationLayer
    /// "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}" = IDataLayer
    /// "{0737082E-958E-11D4-80ED-00C04F601565}" = IDimensionLayer
    /// "{48E56B3F-EC3A-11D2-9F5C-00C04F6BC6A5}" = IFDOGraphicsLayer
    /// "{40A9E885-5533-11D0-98BE-00805F7CED21}" = IFeatureLayer
    /// "{605BC37A-15E9-40A0-90FB-DE4CC376838C}" = IGdbRasterCatalogLayer
    /// "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}" = IGeoFeatureLayer
    /// "{34B2EF81-F4AC-11D1-A245-080009B6F22B}" = IGraphicsLayer
    /// "{EDAD6644-1810-11D1-86AE-0000F8751720}" = IGroupLayer
    /// "{D090AA89-C2F1-11D3-9FEF-00C04F6BC6A5}" = IIMSSubLayer
    /// "{DC8505FF-D521-11D3-9FF4-00C04F6BC6A5}" = IIMAMapLayer
    /// "{34C20002-4D3C-11D0-92D8-00805F7C28B0}" = ILayer
    /// "{E9B56157-7EB7-4DB3-9958-AFBF3B5E1470}" = IMapServerLayer
    /// "{B059B902-5C7A-4287-982E-EF0BC77C6AAB}" = IMapServerSublayer
    /// "{82870538-E09E-42C0-9228-CBCB244B91BA}" = INetworkLayer
    /// "{D02371C7-35F7-11D2-B1F2-00C04F8EDEFF}" = IRasterLayer
    /// "{AF9930F0-F61E-11D3-8D6C-00C04F5B87B2}" = IRasterCatalogLayer
    /// "{FCEFF094-8E6A-4972-9BB4-429C71B07289}" = ITemporaryLayer
    /// "{5A0F220D-614F-4C72-AFF2-7EA0BE2C8513}" = ITerrainLayer
    /// "{FE308F36-BDCA-11D1-A523-0000F8774F0F}" = ITinLayer
    /// "{FB6337E3-610A-4BC2-9142-760D954C22EB}" = ITopologyLayer
    /// "{005F592A-327B-44A4-AEEB-409D2F866F47}" = IWMSLayer
    /// "{D43D9A73-FF6C-4A19-B36A-D7ECBE61962A}" = IWMSGroupLayer
    /// "{8C19B114-1168-41A3-9E14-FC30CA5A4E9D}" = IWMSMapLayer
    // for example
    //UID uid = new UIDClass();
    //uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
    //IEnumLayer players = axMapControl1.Map.get_Layers(uid, true);
    //ILayer plyr = players.Next();
    //while (plyr != null)
    //{
    //     plyr.Next();
    //}
    public enum baseLayerType
    {
        baseGroupLayer = 0,
        baseFeatureLayer = 1,
        baseRasterLayer = 2,
        basePointLayer = 3,
        basePolylineLayer = 4,
        basePolygonLayer = 5,
    }

    class GeoBaseLib
    {
        private AxMapControl m_axMapControl = null;
        IMapDocument m_pMapDocument = null;

        public GeoBaseLib(AxMapControl mapControl = null, IMapDocument mdoc = null)
        {
            m_axMapControl = mapControl;
            m_pMapDocument = mdoc;
        }

        public static bool IsSysField(string sFieldName)
        {
            if (sFieldName.Equals("FID", StringComparison.OrdinalIgnoreCase) || sFieldName.Equals("OBJECTID", StringComparison.OrdinalIgnoreCase)
                || sFieldName.Equals("SHAPE", StringComparison.OrdinalIgnoreCase) || sFieldName.Equals("SHAPE_Length", StringComparison.OrdinalIgnoreCase)
                || sFieldName.Equals("SHAPE_Area", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public bool IsLayerExisted(string sLayerName)
        {
            if (m_axMapControl == null) return false;
            UID uid = new UIDClass();
            uid.Value = "{34C20002-4D3C-11D0-92D8-00805F7C28B0}";
            IEnumLayer pLayers = m_axMapControl.Map.get_Layers(uid, true);
            ILayer pLayer = pLayers.Next();
            while (pLayer != null)
            {
                if (pLayer != null && pLayer.Name == sLayerName)
                {
                    return true;
                }
                pLayer = pLayers.Next();
            }
            return false;
        }

        public ILayer GetLayer(string sLayerName)   //找到对应名称的图层
        {
            if (m_axMapControl == null) return null;
            UID uid = new UIDClass();
            uid.Value = "{34C20002-4D3C-11D0-92D8-00805F7C28B0}";
            IEnumLayer pLayers = m_axMapControl.Map.get_Layers(uid, true);
            ILayer pLayer = pLayers.Next();
            while (pLayer != null)
            {
                if (pLayer != null && pLayer.Name == sLayerName)
                {
                    return pLayer;
                }
                pLayer = pLayers.Next();
            }
            return null;
        }

        public void RemoveLayer(string sLayerName)
        {
            if (m_axMapControl == null) return;
            for (int iMap = 0; iMap < m_pMapDocument.MapCount; iMap++)
            {
                IMap pMap = m_pMapDocument.get_Map(iMap);
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    ILayer pLayer = pMap.get_Layer(i);
                    if (pLayer != null && pLayer.Name == sLayerName)
                    {
                        pMap.DeleteLayer(pLayer);
                        m_axMapControl.Refresh();
                        return;
                    }
                }
            }
        }

        public static IArray GetSelectedFeatures(AxMapControl mapControl)  //找到选中的要素组
        {
            IArray pFeatureArray = new ArrayClass();
            ISelection pFeatureSelction = mapControl.Map.FeatureSelection;
            IEnumFeature pEnumFeature = pFeatureSelction as IEnumFeature;
            IEnumFeatureSetup pEnumFeatureSetup = pEnumFeature as IEnumFeatureSetup;
            pEnumFeatureSetup.AllFields = true;
            IFeature pFeature = pEnumFeature.Next();
            while (pFeature != null)
            {
                if (!pFeature.Shape.IsEmpty)
                {
                    pFeatureArray.Add(pFeature);
                }
                pFeature = pEnumFeature.Next();
            }
            return pFeatureArray;
        }

        public static IArray SelectFeatureByGeometry(IFeatureLayer pLayer, IGeometry pGeometry)
        {
            if (pLayer == null) return null;
            IArray pFeatureArray = new ArrayClass();
            try
            {
                IFeatureClass pFeatureClass = pLayer.FeatureClass;
                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                pSpatialFilter.Geometry = pGeometry;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    if (pFeature.Shape == null)
                    {
                        pFeature = pFeatureCursor.NextFeature();
                        continue;
                    }
                    if (!pFeature.Shape.IsEmpty)
                    {
                        pFeatureArray.Add(pFeature);
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                return pFeatureArray;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Failed in searching with error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return null;
        }

        public static void InitLabel(IGeoFeatureLayer pGeoFeaturelayer, string sFieldName)  //重新标注整个图层
        {
            /*IAnnotateLayerPropertiesCollection作用于一个要素图层的所有注记设置的集合，控制要素图层的一系列注记对象*/
            IAnnotateLayerPropertiesCollection pAnnoLayerPropsCollection;
            //定义标注类
            pAnnoLayerPropsCollection = pGeoFeaturelayer.AnnotationProperties;
            /*将要素图层注记集合中的所有项都移除*/
            pAnnoLayerPropsCollection.Clear();

            IBasicOverposterLayerProperties pBasicOverposterlayerProps = new BasicOverposterLayerPropertiesClass();
            switch (pGeoFeaturelayer.FeatureClass.ShapeType)
            {
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    pBasicOverposterlayerProps.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolygon;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                    pBasicOverposterlayerProps.FeatureType = esriBasicOverposterFeatureType.esriOverposterPoint;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    pBasicOverposterlayerProps.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
                    break;
            }
            ITextSymbol pTextSymbol = new TextSymbolClass();
            stdole.IFontDisp pFont = (stdole.IFontDisp)new stdole.StdFont();
            IRgbColor pRGB = null;
            pFont.Name = "Arial";
            pFont.Size = 9;
            pFont.Bold = false;
            pFont.Italic = false;
            pFont.Underline = false;
            pTextSymbol.Font = pFont;
            if (pRGB == null)
            {
                pRGB = new RgbColorClass();
                pRGB.Red = 0;
                pRGB.Green = 0;
                pRGB.Blue = 0;
                pTextSymbol.Color = (IColor)pRGB;
            }

            ILabelEngineLayerProperties pLabelEnginelayerProps = new LabelEngineLayerPropertiesClass();
            pLabelEnginelayerProps.Expression = "[" + sFieldName + "]";
            pLabelEnginelayerProps.Symbol = pTextSymbol;
            pLabelEnginelayerProps.BasicOverposterLayerProperties = pBasicOverposterlayerProps;
            /*将一项标注属性(LayerEngineLayerProperties对象)增加到要素图层的注记集合当中*/
            /*IAnnotateLayerProperties接口用于获取和修改要素图层注记类的注记属性，定义要素图层动态注记（文本）的显示*/
            pAnnoLayerPropsCollection.Add((IAnnotateLayerProperties)pLabelEnginelayerProps);

        }

        public IArray Search(IFeatureLayer pLayer, string sWhereClause)  //SQL语句找到某图层的目标
        {
            IArray pFeatureArray = new ArrayClass();
            if (pLayer == null) return pFeatureArray;
            try
            {
                IFeatureClass pFeatureClass = pLayer.FeatureClass;
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = sWhereClause;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(pQueryFilter, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    if (pFeature.Shape == null)
                    {
                        pFeature = pFeatureCursor.NextFeature();
                        continue;
                    }
                    if (!pFeature.Shape.IsEmpty)
                    {
                        pFeatureArray.Add(pFeature);
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                return pFeatureArray;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Failed in searching with error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return pFeatureArray;
        }

        public IFeatureLayer CreateShapefile(string sNewLayerName, IArray pOldFeatures, IArray pNewGeometries, esriGeometryType nNewGeometryType)
        //增加一个新图层（如用于缓冲区等）
        {
            if (m_axMapControl == null || pOldFeatures.Count == 0 || pNewGeometries.Count == 0) return null;
            //public static string GetWorkspacePath()
            //{
            string sPath = "\\WSData";
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            //return sPath;
            //}  
            string MapPath = sPath;     //改
            bool bFileExisted = System.IO.File.Exists(MapPath + "\\" + sNewLayerName + ".shp");
            if (bFileExisted && MessageBox.Show("The file is already existed, and do you want to create new one?", "Select one", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.IO.File.Delete(MapPath + "\\" + sNewLayerName + ".shp");
                System.IO.File.Delete(MapPath + "\\" + sNewLayerName + ".dbf");
                System.IO.File.Delete(MapPath + "\\" + sNewLayerName + ".shx");
                RemoveLayer(sNewLayerName);
            }
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = pWorkspaceFactory.OpenFromFile(MapPath, 0) as IFeatureWorkspace;
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            //If file existed, directly load
            if (System.IO.File.Exists(MapPath + "\\" + sNewLayerName + ".shp"))
            {
                pFeatureLayer.FeatureClass = pFWS.OpenFeatureClass(sNewLayerName);
                pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;
                if (!IsLayerExisted(sNewLayerName))
                {
                    m_axMapControl.AddLayer(pFeatureLayer);
                    m_axMapControl.Refresh();
                }
                return pFeatureLayer;
            }
            //Find fields from old feature
            IFeature pTmpFt = pOldFeatures.get_Element(0) as IFeature;
            IFields pTmpFields = pTmpFt.Fields;
            try
            {
                IFields fields = new FieldsClass();
                IFieldsEdit fieldsEdit = fields as IFieldsEdit;

                //Shape field
                IField fd1 = new FieldClass();
                IFieldEdit fiEdit1 = fd1 as IFieldEdit;
                fiEdit1.Name_2 = "Shape";
                fiEdit1.Type_2 = esriFieldType.esriFieldTypeGeometry;
                IGeometryDef pGeomDef = new GeometryDefClass();
                IGeometryDefEdit pGeomDefEdit = pGeomDef as IGeometryDefEdit;
                ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironment();
                pGeomDefEdit.GeometryType_2 = nNewGeometryType;
                //pGeomDefEdit.SpatialReference_2 = pSpatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984) as ISpatialReference;
                fiEdit1.GeometryDef_2 = pGeomDef;
                fieldsEdit.AddField(fd1);

                for (int index = 0; index < pTmpFields.FieldCount; index++)
                {
                    IField pField = pTmpFields.get_Field(index);
                    if (IsSysField(pField.Name)) continue;
                    IField fd = new FieldClass();
                    IFieldEdit fiEdit2 = fd as IFieldEdit;
                    fiEdit2.Name_2 = pField.Name;
                    fiEdit2.Type_2 = pField.Type;
                    fiEdit2.Length_2 = pField.Length;
                    fiEdit2.AliasName_2 = pField.AliasName;
                    fieldsEdit.AddField(fd);
                }

                //Create feature class
                pFeatureLayer.FeatureClass = pFWS.CreateFeatureClass(sNewLayerName, fields, null, null, esriFeatureType.esriFTSimple, "Shape", "");
                pFeatureLayer.Name = sNewLayerName;
                //start editing
                IFeatureClass fc = pFeatureLayer.FeatureClass;
                IWorkspaceEdit w = (fc as IDataset).Workspace as IWorkspaceEdit;
                //w.StartEditing(true);
                //w.StartEditOperation();
                IFeatureBuffer f = fc.CreateFeatureBuffer();
                IFeatureCursor cur = fc.Insert(true);

                pTmpFields = fc.Fields;
                for (int i = 0; i < pOldFeatures.Count; i++)
                {
                    IFeature pOldFeature = pOldFeatures.get_Element(0) as IFeature;
                    f.Shape = pNewGeometries.get_Element(i) as IGeometry;
                    for (int index = 0; index < pTmpFields.FieldCount; index++)
                    {
                        IField pField = pTmpFields.get_Field(index);
                        if (IsSysField(pField.Name)) continue;
                        int nOldIdx = pOldFeature.Fields.FindField(pField.Name);
                        if (nOldIdx != -1)
                        {
                            object obj = pOldFeature.get_Value(nOldIdx);
                            f.set_Value(index, obj);
                        }
                    }
                    cur.InsertFeature(f);
                    //flush per 1000 loops
                    if (i % 1000 == 0)
                    {
                        cur.Flush();
                    }
                }
                cur.Flush();
                //w.StopEditOperation();
                //w.StopEditing(true);
                m_axMapControl.AddShapeFile(MapPath, sNewLayerName + ".shp");
                m_axMapControl.Refresh();

            }
            catch (Exception exception)
            {
                MessageBox.Show("Create" + sNewLayerName + " shapefile error:" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return pFeatureLayer;
        }

        public static void DrawElement(AxMapControl mapControl, IGeometry pGeometry, IRgbColor pColor, bool bCleanLastDraw = false)
        //空间查询高亮目标
        {
            if (pGeometry == null || mapControl == null) return;
            IElement pElement = null;
            IGraphicsContainer pGra = mapControl.ActiveView as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            if (bCleanLastDraw)
            {
                pGra.DeleteAllElements();
            }
            if (pGeometry.Dimension == esriGeometryDimension.esriGeometry0Dimension)
            {
                IMarkerElement pMarkerElement = new MarkerElementClass();
                pElement = pMarkerElement as IElement;
                pElement.Geometry = pGeometry;

                ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                pSimpleMarkerSymbol.Color = pColor;
                pSimpleMarkerSymbol.Size = 3;
                (pElement as IMarkerElement).Symbol = pSimpleMarkerSymbol;
                pGra.AddElement((IElement)pElement, 0);
            }
            else if (pGeometry.Dimension == esriGeometryDimension.esriGeometry1Dimension)
            {
                ILineElement pLineElement = new LineElementClass();
                pElement = pLineElement as IElement;
                pElement.Geometry = pGeometry;
                ILineSymbol pOutline = new SimpleLineSymbolClass();
                pOutline.Width = 1;
                pOutline.Color = pColor;
                (pElement as ILineElement).Symbol = pOutline;
                pGra.AddElement((IElement)pElement, 0);
            }
            else if (pGeometry.Dimension == esriGeometryDimension.esriGeometry2Dimension)
            {
                IPolygonElement pPolygonElement = new PolygonElementClass();
                pElement = pPolygonElement as IElement;
                pElement.Geometry = pGeometry;

                IRgbColor pOutlineColor = new RgbColorClass();
                pOutlineColor.Red = 0;
                pOutlineColor.Green = 0;
                pOutlineColor.Blue = 0;
                pOutlineColor.Transparency = 40;
                ILineSymbol pOutline = new SimpleLineSymbolClass();
                pOutline.Width = 1;
                pOutline.Color = pOutlineColor;
                IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = pColor;
                pFillSymbol.Outline = pOutline;
                pFillSymbol.Color.Transparency = 60;
                IFillShapeElement pFillShapeEle = pElement as IFillShapeElement;
                pFillShapeEle.Symbol = pFillSymbol;
                pGra.AddElement((IElement)pFillShapeEle, 0);
            }
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
    }
}
