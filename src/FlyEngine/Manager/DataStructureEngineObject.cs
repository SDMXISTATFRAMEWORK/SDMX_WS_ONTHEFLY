using FlyMapping.Model;
using FlyEngine.Engine;
using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Structureparser.Workspace;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyMapping.Build;
using FlyController;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyEngine.Engine.Metadata;
using System.Xml;
using System.IO;
using FlyController.Model;
using OnTheFlyLog;

namespace FlyEngine.Manager
{
    /// <summary>
    /// EntryPoint Class for request Metadata Message
    /// </summary>
    public class DataStructureEngineObject : DataEngineObject
    {

        /// <summary>
        /// Metadata builder is base for all Metadata request
        /// </summary>
        private ISDMXObjectBuilder Builder = null;




        /// <summary>
        /// Entrypoint of class FlyEngine that processes the request
        /// and produces a response or an error
        /// </summary>
        /// <param name="_parsingObject">Processed request</param>
        /// <param name="_versionType">Sdmx Version</param>
        public void Engine(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionType)
        {
            try
            {
                this.VersionTypeResp = _versionType;

                //IDBAccess DbAccess = new DWHAccess(FlyConfiguration.ConnectionString);


                _parsingObject.PreliminarCheck();
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Query Preliminar Check passed. Start creation of SDMXObject");

                Builder = CreateBuilder(_parsingObject, _versionType);

                if (Builder == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.NotSupportedRegistryType, new Exception(_parsingObject.SdmxStructureType.ToString()));
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Parsing Metadata: {0}", _parsingObject.SdmxStructureType.ToString());

                Builder.Build();

                ParseOtherRegistry(_parsingObject);

                Builder.AddReferences();

            }
            catch (SdmxException sdmxerr)
            {
                HaveError = true;
                ErrorMessage = sdmxerr;
            }
            catch (Exception err)
            {
                HaveError = true;
                ErrorMessage = new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, err);
            }
        }

        private void ParseOtherRegistry(ISdmxParsingObject _parsingObject)
        {
            if (_parsingObject.OtherRegistry != null && _parsingObject.OtherRegistry.Count > 0)
            {
                foreach (var _parsingOther in _parsingObject.OtherRegistry)
                {
                    ISDMXObjectBuilder OtherRegistry = CreateBuilder(_parsingOther, this.VersionTypeResp);
                    if (OtherRegistry == null)
                    {
                        FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Error, "NotSupportedRegistryType (Other Registry) SdmxStructureType {0}", _parsingOther.SdmxStructureType.ToString());
                        continue;
                    }
                    FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Parsing Metadata (Other Registry): {0}", _parsingOther.SdmxStructureType.ToString());
                    OtherRegistry.Build();
                    switch (_parsingOther.SdmxStructureType)
                    {
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dataflow:
                            if (Builder._Dataflows == null || Builder._Dataflows.Count == 0)
                                Builder._Dataflows = OtherRegistry._Dataflows;
                            else
                                Builder._Dataflows.AddRange(OtherRegistry._Dataflows);
                            break;
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ConceptScheme:
                            if (Builder._Conceptscheme == null || Builder._Conceptscheme.Count == 0)
                                Builder._Conceptscheme = OtherRegistry._Conceptscheme;
                            else
                                Builder._Conceptscheme.AddRange(OtherRegistry._Conceptscheme);
                            break;
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CodeList:
                            if (Builder._Codelists == null || Builder._Codelists.Count == 0)
                                Builder._Codelists = OtherRegistry._Codelists;
                            else
                                Builder._Codelists.AddRange(OtherRegistry._Codelists);
                            break;
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.AgencyScheme:
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationScheme:
                            Builder._AgencyScheme = OtherRegistry._AgencyScheme;
                            break;
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CategoryScheme:
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Categorisation:
                            if (Builder._CategorisationObject == null || Builder._CategorisationObject.Count == 0)
                                Builder._CategorisationObject = OtherRegistry._CategorisationObject;
                            else
                                Builder._CategorisationObject.AddRange(OtherRegistry._CategorisationObject);    
                            break;
                        case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dsd:
                            if (Builder._KeyFamily == null || Builder._KeyFamily.Count == 0)
                                Builder._KeyFamily = OtherRegistry._KeyFamily;
                            else
                                Builder._KeyFamily.AddRange(OtherRegistry._KeyFamily);    
                            break;
                    }
                }
            }
        }



        /// <summary>
        /// Get Processed result of request
        /// </summary>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public override IFlyWriterBody GetResult()
        {
            try
            {
                return Builder.WriteDSD();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateDSDObject, ex);
            }
            finally
            {
                DestroyObjects();
            }
        }

        /// <summary>
        /// Implement correct Metadata Builder to process the request
        /// identifies the correct implementation with the SdmxStructureType property of SdmxParsingObject
        /// </summary>
        /// <param name="_parsingObject">Processed request</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns>Implementation of SDMXObjectBuilder</returns>
        public ISDMXObjectBuilder CreateBuilder(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            try
            {
                this.VersionTypeResp = _versionTypeResp;
                ISDMXObjectBuilder _builder = null;
                EngineChooser chooser = new EngineChooser(_parsingObject, _versionTypeResp);
                #region Creazione dei Builder
                switch (_parsingObject.SdmxStructureType)
                {
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dataflow:
                        _builder = chooser.GetDataflows();
                        break;
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ConceptScheme:
                        _builder = chooser.GetConcepts();
                        break;
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CodeList:
                        _builder = chooser.GetCodelists();

                        break;
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.AgencyScheme:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationScheme:
                        _builder = chooser.GetAgencyScheme();
                        break;
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CategoryScheme:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Categorisation:
                        _builder = chooser.GetCategoryScheme();
                        break;
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dsd:
                        _builder = chooser.GetStructure();

                        break;

                    //qui sotto ancora non implementate
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Annotation:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Any:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.AttachmentConstraint:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.AttachmentConstraintAttachment:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.AttributeDescriptor:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CategoryId:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CategoryMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CategorySchemeMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Code:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CodeListMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CodeListRef:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CodeMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Component:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ComponentMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Computation:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ConceptMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ConceptSchemeMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ConstrainedDataKey:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ConstrainedDataKeyset:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Constraint:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ConstraintContentTarget:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Contact:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ContentConstraint:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ContentConstraintAttachment:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CrossSectionalMeasure:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CubeRegion:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataAttribute:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataConsumer:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataConsumerScheme:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataProvider:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataProviderScheme:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dataset:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DatasetReference:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DatasetTarget:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Datasource:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dimension:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DimensionDescriptor:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DimensionDescriptorValuesTarget:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Group:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.HierarchicalCode:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.HierarchicalCodelist:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Hierarchy:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.HybridCode:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.HybridCodelistMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.IdentifiableObjectTarget:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.InputOutput:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ItemMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.KeyValues:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Level:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.LocalRepresentation:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MeasureDescriptor:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MeasureDimension:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataAttribute:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataDocument:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataFlow:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataReferenceValue:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataReport:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataReportAttribute:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataReportTarget:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataSet:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataTarget:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.MetadataTargetRegion:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Msd:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Null:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationSchemeMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationUnit:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationUnitScheme:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.PrimaryMeasure:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Process:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ProcessStep:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ProvisionAgreement:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ReferencePeriod:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Registration:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.RelatedStructures:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ReleaseCalendar:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ReportPeriodTarget:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ReportStructure:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ReportingCategory:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ReportingTaxonomy:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.ReportingTaxonomyMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.RepresentationMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.StructureMap:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.StructureSet:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Subscription:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.TextFormat:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.TextType:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.TimeDimension:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.TimeRange:
                    case Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Transition:
                    default:
                        break;
                }
                #endregion


                return _builder;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateBuilder, ex);
            }
        }



        /// <summary>
        /// Dispose all Object used for retreial data
        /// </summary>
        public override void DestroyObjects()
        {
            try
            {
                this.Builder = null;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DestroyObjects, ex);
            }
        }
    }
}
