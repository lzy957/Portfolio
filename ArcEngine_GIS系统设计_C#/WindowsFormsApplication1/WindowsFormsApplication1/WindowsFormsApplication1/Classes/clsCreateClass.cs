using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Windows.Forms;
//using ESRI.ArcGIS.NetworkAnalysis;
/*
namespace WindowsFormsApplication1.Classes
{
    public class clsCreateClass
        {
            public IFeatureDataset CreateDataset(IWorkspace pWorkspace)
            {
                try
                {
                    if (pWorkspace == null) return null;
                    IFeatureWorkspace aFeaWorkspace = pWorkspace as IFeatureWorkspace;
                    if (aFeaWorkspace == null) return null;
                    DatasetPropertiesForm aForm = new DatasetPropertiesForm();
                    aForm.HignPrecision = LSGISHelper.WorkspaceHelper.HighPrecision(pWorkspace);
                    if (aForm.ShowDialog() == DialogResult.OK)
                    {
                        string dsName = aForm.FeatureDatasetName;
                        ISpatialReference aSR = aForm.SpatialReference;
                        IFeatureDataset aDS = aFeaWorkspace.CreateFeatureDataset(dsName, aSR);
                        return aDS;
                    }
                }
                catch (Exception ex) { }
                return null;
            }
            public IRasterDataset CreateRasterDataset(IWorkspace pWorkspace, string sName
               )
            {
                try
                {
                    IRasterWorkspaceEx pRWEx = pWorkspace as IRasterWorkspaceEx;
                    IGeometryDef pGDef = new GeometryDefClass();

                    IRasterDataset pRD = pRWEx.CreateRasterDataset(
                        sName, 3, rstPixelType.PT_CHAR, null, null, null, null);
                }
                catch { }
                return null;
            }
            public IFeatureClass CreateFeatureClass(IWorkspace pWorkspace)
            {
                if (pWorkspace == null) return null;
                IFeatureWorkspace aFeaWorkspace = pWorkspace as IFeatureWorkspace;
                if (aFeaWorkspace == null) return null;
                IFeatureClass aClass = null;
                FeatureClassWizard aForm = new FeatureClassWizard();
                aForm.Workspace = pWorkspace;
                if (aForm.ShowDialog() == DialogResult.OK)
                {
                    while (true)
                    {
                        string className = aForm.FeatureClassName;
                        string aliasName = aForm.FeatureClassAliasName;
                        IFields flds = aForm.Fields;
                        try
                        {
                            aClass = aFeaWorkspace.CreateFeatureClass(className, flds
                                , null, null, esriFeatureType.esriFTSimple, "SHAPE", null);
                            if (!aliasName.Equals(""))
                            {
                                IClassSchemaEdit aClassEdit = aClass as IClassSchemaEdit;
                                if (aClassEdit != null) aClassEdit.AlterAliasName(aliasName);
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show ("错误:\n"+ex.Message ,"新建特性类",
                            //    MessageBoxButtons.OK ,MessageBoxIcon.Error );
                            LSCommonHelper.MessageBoxHelper.ShowErrorMessageBox(ex, "");
                        }
                        aForm = new FeatureClassWizard();
                        aForm.Workspace = pWorkspace;
                        aForm.FeatureClassName = className;
                        aForm.FeatureClassAliasName = aliasName;
                        aForm.Fields = flds;
                        if (aForm.ShowDialog() == DialogResult.Cancel) break;
                    }
                }
                return aClass;
            }
            public IFeatureClass CreateFeatureClass(IFeatureDataset pDS)
            {
                if (pDS == null) return null;
                IFeatureClass aClass = null;

                FeatureClassWizard aForm = new FeatureClassWizard();
                aForm.Workspace = (pDS as IDataset).Workspace;
                IGeoDataset pGDS = pDS as IGeoDataset;
                if (pGDS != null)
                {
                    aForm.SpatialReference = pGDS.SpatialReference;
                }
                if (aForm.ShowDialog() == DialogResult.OK)
                {
                    while (true)
                    {
                        string className = aForm.FeatureClassName;
                        string aliasName = aForm.FeatureClassAliasName;
                        IFields flds = aForm.Fields;

                        try
                        {
                            aClass = pDS.CreateFeatureClass(className, flds
                                , null, null, esriFeatureType.esriFTSimple, "SHAPE", null);
                            if (!aliasName.Equals(""))
                            {
                                IClassSchemaEdit aClassEdit = aClass as IClassSchemaEdit;
                                if (aClassEdit != null) aClassEdit.AlterAliasName(aliasName);
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            LSCommonHelper.MessageBoxHelper.ShowErrorMessageBox(ex, "请选择高精度坐标系");
                        }
                        aForm = new FeatureClassWizard();
                        aForm.Workspace = (pDS as IDataset).Workspace;
                        aForm.FeatureClassName = className;
                        aForm.FeatureClassAliasName = aliasName;


                        aForm.Fields = flds;
                        if (aForm.ShowDialog() == DialogResult.Cancel) break;
                    }
                }
                return aClass;
            }
            public ITable CreateTable(IWorkspace pWorkspace)
            {
                if (pWorkspace == null) return null;
                IFeatureWorkspace aFeaWorkspace = pWorkspace as IFeatureWorkspace;
                if (aFeaWorkspace == null) return null;
                ITable aTable = null;
                DataTableWizard aWizard = new DataTableWizard();
                aWizard.Workspace = pWorkspace;
                if (aWizard.ShowDialog() == DialogResult.OK)
                {
                    while (true)
                    {
                        string tableName = aWizard.TableName;
                        string aliasName = aWizard.TableAliasName;
                        IFields flds = aWizard.Fields;
                        try
                        {
                            aTable = aFeaWorkspace.CreateTable(tableName, flds
                                , null, null, null);

                            if (!aliasName.Equals(""))
                            {
                                IClassSchemaEdit aClassEdit = aTable as IClassSchemaEdit;
                                aClassEdit.RegisterAsObjectClass("OBJECTID", null);
                                if (aClassEdit != null) aClassEdit.AlterAliasName(aliasName);
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show ("错误:\n"+ex.Message ,"新建表",
                            //    MessageBoxButtons.OK ,MessageBoxIcon.Error );
                            LSCommonHelper.MessageBoxHelper.ShowErrorMessageBox(ex, "");
                        }
                        aWizard = new DataTableWizard();
                        aWizard.Workspace = pWorkspace;
                        aWizard.TableName = tableName;
                        aWizard.TableAliasName = aliasName;
                        aWizard.Fields = flds;
                        if (aWizard.ShowDialog() == DialogResult.Cancel) break;
                    }
                }
                return aTable;
            }
        }

    }

*/