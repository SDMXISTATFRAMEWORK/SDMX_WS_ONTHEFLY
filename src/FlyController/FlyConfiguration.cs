using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyController
{
    /// <summary>
    /// FlyConfiguration is a Application Configuration File
    /// Contains all static property and costants property  of Application
    /// Read a XML File Config and populate Configuration Property
    /// </summary>
    public class FlyConfiguration
    {
        /// <summary>
        /// Read a XML File Config and populate FlyConfiguration property
        /// </summary>
        /// <param name="_ExecutionPath">Path where is the XML File Config </param>
        public static void InitConfig(string _ExecutionPath)
        {
            try
            {
                FlyConfiguration.ExecutionPath = _ExecutionPath;

                XmlDocument configuration = new XmlDocument();
                configuration.Load(FlyConfiguration.ConfigurationFile);
                foreach (XmlNode item in configuration.ChildNodes[0].ChildNodes)
                {
                    if (item.Name == "GlobalSettings")
                    {
                        GetGlobalConf(item);
                        continue;
                    }
                    if (item.Name == "MappingSettings")
                    {
                        if (item.Attributes == null || item.Attributes["UsedSettings"] == null)
                            throw new Exception("The Element MappingSettings don't have attribute UsedSettings");
                        string MappingSettingKey = item.Attributes["UsedSettings"].Value;
                        bool foundSetting = false;
                        foreach (XmlNode MappingSetting in item.ChildNodes)
                        {
                            if (MappingSetting.Name == "MappingSetting" && MappingSetting.Attributes != null && MappingSetting.Attributes["id"] != null && MappingSettingKey.Trim().ToLower() == MappingSetting.Attributes["id"].Value.Trim().ToLower())
                            {
                                foundSetting = true;
                                GetMappingSettingsConf(MappingSetting);
                                break;
                            }
                        }
                        if (!foundSetting)
                            throw new Exception(string.Format("MappingSetting whit id {0} not found", MappingSettingKey));
                    }
                }

                //Codifica i codici dei messaggi SDMX CommonAPI in descrizione
                Org.Sdmxsource.Sdmx.Api.Exception.SdmxException.SetMessageResolver(new Org.Sdmxsource.Util.ResourceBundle.MessageDecoder());
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.FlyConfiguration), FlyExceptionObject.FlyExceptionTypeEnum.InitConfigError, ex);
            }
        }

        private static void GetGlobalConf(XmlNode GlobalSettingsNode)
        {
            try
            {
                foreach (XmlNode item in GlobalSettingsNode.ChildNodes)
                {
                    if (item.Name == "HeaderSettings")
                    {
                        FlyConfiguration.HeaderSettings = new HeaderBuilder(item);
                        continue;
                    }

                    if (item.Name != "add" || item.Attributes == null || item.Attributes["key"] == null || item.Attributes["value"] == null)
                        continue;
                    switch (item.Attributes["key"].Value)
                    {
                        case "UserName":
                            FlyConfiguration.UserName = item.Attributes["value"].Value;
                            break;
                        case "Domain":
                            FlyConfiguration.Domain = item.Attributes["value"].Value;
                            break;
                        case "DatasetTitle":
                            bool _datasetTitle;
                            bool.TryParse(item.Attributes["value"].Value, out _datasetTitle);
                            FlyConfiguration.DatasetTitle = _datasetTitle;
                            break;
                        case "OrganisationScheme":
                            FlyConfiguration.AgencyOrganisationId = item.Attributes["value"].Value;
                            FlyConfiguration.AgencyOrganisationDescription = SdmxObjectNameDescription.GetNameDescriptions(item);
                            break;
                        case "MainAgencyId":
                            FlyConfiguration.MainAgencyId = item.Attributes["value"].Value;
                            FlyConfiguration.MainAgencyDescription = SdmxObjectNameDescription.GetNameDescriptions(item);
                            break;
                        case "Version":
                            FlyConfiguration.Version = item.Attributes["value"].Value;
                            break;
                    }
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.FlyConfiguration), FlyExceptionObject.FlyExceptionTypeEnum.InitConfigError, ex);
            }
        }

        private static void GetMappingSettingsConf(XmlNode MappingSettings)
        {
            try
            {
                foreach (XmlNode item in MappingSettings.ChildNodes)
                {
                    if (item.Name == "StoreProcedureSettings")
                    {
                        FlyConfiguration.StoreProcedureSettings = DatabaseStoreProcedureSettings.GetDbStoreSetting(item);
                        continue;
                    }
                    if (item.Name == "RIWebService")
                    {
                        FlyConfiguration.RIWebServices = new RIWebServiceConfiguration();
                        FlyConfiguration.RIWebServices.Configure(item);
                        continue;
                    }
                    if (item.Name == "CategorySettings")
                    {
                        foreach (XmlNode cats in item.ChildNodes)
                        {
                            if (cats.Name != "add" || cats.Attributes == null || cats.Attributes["key"] == null || cats.Attributes["value"] == null)
                                continue;
                            switch (cats.Attributes["key"].Value)
                            {
                                case "ConnectionStringCategory":
                                    FlyConfiguration.ConnectionStringCategory = cats.Attributes["value"].Value;
                                    break;
                                case "CategoryName":
                                    FlyConfiguration.CategoryName = cats.Attributes["value"].Value;
                                    break;
                            }
                        }
                    }

                    if (item.Name != "add" || item.Attributes == null || item.Attributes["key"] == null || item.Attributes["value"] == null)
                        continue;
                    switch (item.Attributes["key"].Value)
                    {
                        case "ConnectionSTAT":
                            DBStatExtra = false;
                            FlyConfiguration.ConnectionString = item.Attributes["value"].Value;
                            break;
                        case "ConnectionDDB":
                            DBStatExtra = true;
                            FlyConfiguration.ConnectionString = item.Attributes["value"].Value;
                            break;
                        case "MsConnectionString":
                            FlyConfiguration.ConnectionStringMR = item.Attributes["value"].Value;
                            break;
                        case "DsdFormat":
                            FlyConfiguration.DsdFormat = item.Attributes["value"].Value;
                            break;
                        case "ConceptSchemeFormat":
                            FlyConfiguration.ConceptSchemeFormat = item.Attributes["value"].Value;
                            break;
                        case "CategorySchemeFormat":
                            FlyConfiguration.CategorySchemeFormat = item.Attributes["value"].Value;
                            break;
                        case "CodelistFormat":
                            FlyConfiguration.CodelistFormat = item.Attributes["value"].Value;
                            break;
                        case "CategorisationFormat":
                            FlyConfiguration.CategorisationFormat = item.Attributes["value"].Value;
                            break;
                        case "CodelistWhitoutConstrain":
                            FlyConfiguration.CodelistWhitoutConstrain = bool.Parse(item.Attributes["value"].Value);
                            break;
                        case "ConceptObservationFlag":
                            try
                            {
                                if (!string.IsNullOrEmpty(item.Attributes["value"].Value))
                                {
                                    FlyConfiguration.ConceptObservationAttribute = new AttributeConcept(item.Attributes["value"].Value, SdmxObjectNameDescription.GetNameDescriptions(item));
                                    FlyConfiguration.ConceptObservationAttribute.AttributeAttachmentLevelType = (AttributeAttachmentLevel)Enum.Parse(typeof(AttributeAttachmentLevel), item.Attributes["attachmentLevel"].Value, true);
                                    FlyConfiguration.ConceptObservationAttribute.AssignmentStatusType = (AssignmentStatusTypeEnum)Enum.Parse(typeof(AssignmentStatusTypeEnum), item.Attributes["assignmentStatus"].Value, true);
                                    FlyConfiguration.ConceptObservationAttribute.IsFlagAttribute = true;
                                }
                            }
                            catch (Exception)
                            {
                                FlyConfiguration.ConceptObservationAttribute = null;
                            }
                            break;
                    }
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.FlyConfiguration), FlyExceptionObject.FlyExceptionTypeEnum.InitConfigError, ex);
            }
        }

        private static bool DBStatExtra = false;
        /// <summary>
        /// OnTheFly Version onlyget return 1.0 if CoonectionStringMr is empty also return version 2.0
        /// </summary>
        public static OnTheFlyVersionEnum OnTheFlyVersion
        {
            get
            {
                if (string.IsNullOrEmpty(ConnectionStringMR))
                    if (!DBStatExtra)
                        return OnTheFlyVersionEnum.OnTheFly1;
                    else
                        return OnTheFlyVersionEnum.OnTheFly15;
                else
                    return OnTheFlyVersionEnum.OnTheFly2;
            }
        }

        //---------------DB---------------
        /// <summary>
        /// ConnectionString Datawarehouse
        /// </summary>
        public static string ConnectionString { get; set; }
        /// <summary>
        /// ConnectionString from where to get the data relative of Category
        /// </summary>
        public static string ConnectionStringCategory { get; set; }
        /// <summary>
        /// ConnectionString to MetadataRepository from where to get the Metadata 
        /// If this is Null the OnTheFly Version is setted on 1.0 also will be set OnTheFly Version on 2.0
        /// </summary>
        public static string ConnectionStringMR { get; set; }
        /// <summary>
        /// "UserName" StoreProcedure Parameter
        /// </summary>
        public static string UserName { get; set; }
        /// <summary>
        /// "Domain" StoreProcedure Parameter
        /// </summary>
        public static string Domain { get; set; }


        //---------------Log e Utils---------------
        /// <summary>
        /// Path where is the XML File Config
        /// </summary>
        public static string ExecutionPath { get; set; }
        /// <summary>
        /// [const] Configuration File Name (only Get)
        /// </summary>
        public static string ConfigurationFile { get { return FlyConfiguration.ExecutionPath + "\\ServiceConfiguration.xml"; } }
        /// <summary>
        /// [const] Attribute File Name (only Get) (File where is a list of Application Attribute and relative codelist)
        /// </summary>
        public static string AttributeFile { get { return FlyConfiguration.ExecutionPath + "\\ConfigurationXml\\AttributeConcepts.xml"; } }
        /// <summary>
        /// [const] FrequencyCodelist File Name (only Get) (File where is a codelist of Frequency dimension)
        /// </summary>
        public static string FrequencyCodelistFile { get { return FlyConfiguration.ExecutionPath + "\\ConfigurationXml\\FrequencyCodelist.xml"; } }
        /// <summary>
        /// [const] Error Description File Name (only Get) (File where is a list of Errors Descriptions)
        /// </summary>
        public static string ErrorDescriptionFile { get { return FlyConfiguration.ExecutionPath + "\\ConfigurationXml\\ErrorDescription.xml"; } }

        //---------------Application Utils---------------
        /// <summary>
        /// OrganisationScheme Code
        /// </summary>
        public static string AgencyOrganisationId { get; set; }
        /// <summary>
        /// OrganisationScheme Description Names
        /// </summary>
        public static List<SdmxObjectNameDescription> AgencyOrganisationDescription { get; set; }
        /// <summary>
        /// Main Agency Id (AgencyScheme Code) Names
        /// </summary>
        public static string MainAgencyId { get; set; }
        /// <summary>
        /// AgencyScheme Description Names
        /// </summary>
        public static List<SdmxObjectNameDescription> MainAgencyDescription { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public static string Version { get; set; }

        //---------------Object Format---------------
        /// <summary>
        /// Format Code DSD Actualy {0}_DSD, {0} will be replaced with Dataset Code
        /// </summary>
        public static string DsdFormat { get; set; }
        /// <summary>
        /// Format Code ConceptScheme Actualy {0}_CS, {0} will be replaced with Dataset Code
        /// </summary>
        public static string ConceptSchemeFormat { get; set; }
        /// <summary>
        /// Format Code CategoryScheme Actualy {0}_CategoryScheme, {0} will be replaced with MainAgencyId
        /// </summary>
        public static string CategorySchemeFormat { get; set; }
        /// <summary>
        /// Format Code ConceptScheme Actualy CL_{0}, {0} will be replaced with Concept Code
        /// </summary>
        public static string CodelistFormat { get; set; }
        /// <summary>
        /// Format Code Categorisation Actualy {0}_Categorisation, {0} will be replaced with dataflow
        /// </summary>
        public static string CategorisationFormat { get; set; }

        /// <summary>
        /// If arrive Query Codelist Whitout Constrain (Dataflow reference) 
        /// true: Merge codelist in All Dataflow (low performance)
        /// false: Generate Exception
        /// </summary>
        public static bool CodelistWhitoutConstrain { get; set; }

        /// <summary>
        /// FLAGDimension (OBS_STATUS)
        /// </summary>
        public static IAttributeConcept ConceptObservationAttribute { get; set; }

        /// <summary>
        /// The Store Procedure Settings <see cref="DatabaseStoreProcedureSettings"/>
        /// </summary>
        public static List<DatabaseStoreProcedureSettings> StoreProcedureSettings { get; set; }

        /// <summary>
        /// HeaderBuilder Object with Header already created 
        /// </summary>
        public static HeaderBuilder HeaderSettings { get; set; }
        /// <summary>
        /// Dataset Title used in data message response
        /// </summary>
        public static bool DatasetTitle { get; set; }

        /// <summary>
        /// Attribute TIME_FORMAT Code (Used to filter the query with this attribute)
        /// </summary>
        public static string Time_Format_Id = "TIME_FORMAT";

        /// <summary>
        /// Name of Category in DB, if empty get the first Category founded
        /// </summary>
        public static string CategoryName { get; set; }

        /// <summary>
        /// Service Uri, used for build StructureUrl <see cref="RetreivalStructureUrl"/>
        /// </summary>
        public string ServiceUri { get; set; }

        /// <summary>
        /// Uri to Connect to WebServices RI
        /// </summary>
        public static RIWebServiceConfiguration RIWebServices { get; set; }

    }
}
