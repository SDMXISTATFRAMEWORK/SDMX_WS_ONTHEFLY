using FlyEngine.Model;
using FlyController;
using FlyController.Builder;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
using Org.Sdmxsource.Sdmx.Structureparser.Manager;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Manager.Output;
using Org.Sdmxsource.Sdmx.Structureparser.Factory;
using FlyController.Model;
using FlyController.Model.Streaming;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;

namespace FlyEngine.Model
{
    /// <summary>
    /// Object Model base for develop Metadata Builder
    /// </summary>
    public abstract class SDMXObjectBuilder : ISDMXObjectBuilder
    {
        /// <summary>
        /// create a SDMXObjectBuilder instance
        /// and requires all Classbuilder to request this information
        /// </summary>
        /// <param name="_parsingObject">Processed request</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public SDMXObjectBuilder(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            this.ParsingObject = _parsingObject;
            this.VersionTypeResp = _versionTypeResp;
        }

        /// <summary>
        /// List of immutable instance of Categorisation
        /// </summary>
        public List<ICategorisationObject> _CategorisationObject { get; set; }
        /// <summary>
        /// Immutable instance of CategoryScheme
        /// </summary>
        public List<ICategorySchemeObject> _CategorySchemeObject { get; set; }
        /// <summary>
        /// Immutable instance of AgencyScheme
        /// </summary>
        public IAgencyScheme _AgencyScheme { get; set; }
        /// <summary>
        /// List of immutable instance of Dataflow
        /// currently is only one
        /// </summary>
        public List<IDataflowObject> _Dataflows { get; set; }
        /// <summary>
        ///  Immutable instance of ConceptsScheme
        /// </summary>
        public List<IConceptSchemeObject> _Conceptscheme { get; set; }
        /// <summary>
        /// List of immutable instance of Codelist
        /// </summary>
        public List<ICodelistMutableObject> _Codelists { get; set; }
        /// <summary>
        /// List of immutable instance of DataStructure (KeyFamily for Sdmx 2.0, Structure for Sdmx 2.1)
        /// </summary>
        public List<DataStructureObjectImpl> _KeyFamily { get; set; }

        /// <summary>
        /// Builder of DataStructure
        /// </summary>
        public DataStructureBuilder _DataStructureBuilder { get; set; }
        /// <summary>
        /// Processed request
        /// </summary>
        public ISdmxParsingObject ParsingObject { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public SdmxSchemaEnumType VersionTypeResp { get; set; }
        

        /// <summary>
        /// Abstract void for build this property with correct parameter
        /// </summary>
        public abstract void Build();

        /// <summary>
        /// Abstract void for build the External References
        /// </summary>
        public abstract void AddReferences();

        /// <summary>
        /// Call CreateDSD and Write SdmxObject in XElement Streaming to return with processed metadata result
        /// </summary>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public virtual IFlyWriterBody WriteDSD()
        {
            try
            {

                ISdmxObjects sdmxObject = CreateDSD();
                sdmxObject.Action = DatasetAction.GetFromEnum(DatasetActionEnumType.Append);

                //Oggetto che crea l'output
                StructureWriterManager swm = new StructureWriterManager();

                StructureOutputFormat sofType = null;
                if (VersionTypeResp == SdmxSchemaEnumType.VersionTwo)
                    sofType = StructureOutputFormat.GetFromEnum(StructureOutputFormatEnumType.SdmxV2RegistryQueryResponseDocument);
                else if (VersionTypeResp == SdmxSchemaEnumType.VersionTwoPointOne)
                    sofType = StructureOutputFormat.GetFromEnum(StructureOutputFormatEnumType.SdmxV21StructureDocument);
                else
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.UnrecognizedVersion, new Exception("Version: " + VersionTypeResp.ToString()));
                SdmxStructureFormat sof = new SdmxStructureFormat(sofType);

                //IStructureFormat

                //Dove metto il risultato Stream
                IFlyWriterBody WriterBody = new FlyMetadataWriterBody()
                {
                    StructureFormat = sof,
                    SdmxObject = sdmxObject
                };
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Create Callback for Writing SDMXObject");
                return WriterBody;
                //MemoryStream ms = new MemoryStream();
                //swm.WriteStructures(sdmxObject, sof, ms);
                //ms.Position = 0;
                //StreamReader rdr = new System.IO.StreamReader(ms);
                //ms.Position = 0;
                //string DSDris = rdr.ReadToEnd();
                //return XElement.Parse(DSDris);

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateSdmxObjectError, ex);
            }
        }



        /// <summary>
        /// Create a SdmxObjects from all parameter configured
        /// </summary>
        /// <returns>SdmxObject for write message</returns>
        public ISdmxObjects CreateDSD()
        {
            try
            {
                StringBuilder sdmxObjectDescription = new StringBuilder();
                sdmxObjectDescription.AppendLine();
                ISdmxObjects sdmxObject = new SdmxObjectsImpl();
                sdmxObject.Header = FlyConfiguration.HeaderSettings.GetHeader();
                //sdmxObject.AddRegistration(


                if (_CategorySchemeObject != null)
                {
                    foreach (ICategorySchemeObject CSObj in _CategorySchemeObject)
                    {
                        sdmxObject.AddCategoryScheme(CSObj);
                        sdmxObjectDescription.AppendLine(string.Format("CategoryScheme: {0} category", CSObj.Items == null ? 0 : CSObj.Items.Count));
                    }
                }
                if (_CategorisationObject != null)
                {
                    foreach (var categorisation in _CategorisationObject)
                        sdmxObject.AddCategorisation(categorisation);
                    sdmxObjectDescription.AppendLine(string.Format("Categorisation: {0} categorisation", _CategorisationObject.Count));

                }


                if (_AgencyScheme != null)
                {
                    sdmxObject.AddAgencyScheme(_AgencyScheme);
                    sdmxObjectDescription.AppendLine(string.Format("AgencyScheme"));

                }

                if (_Codelists != null)
                {
                    try
                    {
                        int totalcode = 0;
                        _Codelists.ForEach(cl =>
                        {
                            sdmxObject.AddCodelist(cl.ImmutableInstance);
                            totalcode += cl.Items.Count;
                        });
                        sdmxObjectDescription.AppendLine(string.Format("Codelists: {0} codelists, {1} code_objects", _Codelists.Count, totalcode));
                    }
                    catch (SdmxException) { throw; }
                    catch (Exception ex)
                    {
                        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
                    }
                }

                if (_Conceptscheme != null)
                {
                    foreach (var conceptscheme in _Conceptscheme)
                    {
                        sdmxObject.AddConceptScheme(conceptscheme);
                        sdmxObjectDescription.AppendLine(string.Format("Conceptscheme: {0} concepts -> {1}", (conceptscheme.Items == null ? 0 : conceptscheme.Items.Count), conceptscheme.Id));
                    }

                }

                if (_KeyFamily != null)
                {
                    foreach (DataStructureObjectImpl keyFamily in _KeyFamily)
                    {
                        sdmxObject.AddDataStructure(keyFamily.Immutated);
                        sdmxObjectDescription.AppendLine(string.Format("Structure: {0} dimensions, {1} attributes  -> {2}",
                            keyFamily.Immutated.GetDimensions() == null ? 0 : keyFamily.Immutated.GetDimensions().Count,
                            keyFamily.Attributes == null ? 0 : keyFamily.Attributes.Count, keyFamily.Id));
                    }
                    
                }

                if (_Dataflows != null)
                {
                    _Dataflows.ForEach(df => sdmxObject.AddDataflow(df));
                    sdmxObjectDescription.AppendLine(string.Format("Dataflows: {0} dataflows", _Dataflows.Count));
                }

                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"Writing sdmxObject complete succesfully. the Sdmx Contains: {0}", sdmxObjectDescription.ToString().Trim());


                return sdmxObject;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateDSDObject, ex);
            }
        }

    }
}
