using FlyController.Builder;
using FlyController.Utils;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Class that calculates the value of StructureUrl depending on the type of artifact
    /// </summary>
    public class RetreivalStructureUrl
    {
        /// <summary>
        /// calculates the value of StructureUrl (<see cref="RetreivalStructureUrl"/>)
        /// </summary>
        /// <param name="obj">Type of artifact</param>
        /// <param name="MaintenableId">Artefact Code</param>
        /// <param name="Agencyid">Artefact AgencyId</param>
        /// <param name="Version">Artefact Version</param>
        /// <returns>Parsed StructureUrl Uri</returns>
        public static Uri Get(ObjectBuilder obj, string MaintenableId, string Agencyid, string Version)
        {
            try
            {
                string Uri = System.ServiceModel.OperationContext.Current.Host.BaseAddresses[0].ToString();
                int Fromindex = 0;
                if (Uri.Contains("/" + Constant.ContractNamespaceSdmx20))
                    Fromindex = Uri.IndexOf("/" + Constant.ContractNamespaceSdmx20);
                if (Uri.Contains("/" + Constant.ContractNamespaceSdmx21))
                    Fromindex = Uri.IndexOf("/" + Constant.ContractNamespaceSdmx21);
                if (Uri.Contains("/rest"))
                    Fromindex = Uri.IndexOf("/rest");

                if (Fromindex > 0)
                {
                    string restV = "/rest/";

                    if (obj is CategorySchemeBuilder)
                    {
                        if (obj.ParsingObject.SdmxStructureType != SdmxStructureEnumType.Categorisation)
                            restV += "CategoryScheme/";
                        else
                            restV += "Categorisation/";
                    }
                    else if (obj is CodelistBuilder)
                        restV += "Codelist/";
                    else if (obj is ConceptBuilder)
                        restV += "ConceptScheme/";
                    else if (obj is DataflowBuilder)
                        restV += "Dataflow/";
                    else if (obj is DataStructureBuilder)
                        restV += "DataStructure/";

                    restV += string.Format("{0}/{1}/{2}/", Agencyid, MaintenableId, Version);

                    return new Uri(Uri.Substring(0, Fromindex) + restV);
                }

            }
            catch (Exception ex)
            {
                FlyLog.WriteLog(typeof(RetreivalStructureUrl), FlyLog.LogTypeEnum.Error, "Error when parsing Uri for StructureUrl err: {0}", ex.Message);
            }
            return new Uri("http://OntheFly/ExternalReference/StructureURL");


        }
    }
}
