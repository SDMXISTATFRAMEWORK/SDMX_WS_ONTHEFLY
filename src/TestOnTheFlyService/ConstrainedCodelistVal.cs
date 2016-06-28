using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestOnTheFlyService
{
    public class ConstrainedCodelistVal
    {
        public static string GetStringVal(SdmxObject DF, SdmxObject Concept)
        {
            //OCCHIO ALL'ORDINE
            List<string> vals = new List<string>()
            {
                DF.Code,
                DF.DataFlowAgency,
                DF.DataFlowVersion,
                Concept.isTimeConcept?"CL_TIME_PERIOD": Concept.CodelistId,
                Concept.isTimeConcept?"MA":Concept.CodelistAgency,
                Concept.isTimeConcept?"1.0":Concept.CodelistVersion,
                Concept.isTimeConcept?"TIME_PERIOD":Concept.Code
            };
            return string.Join(",",vals);
        }

        public static string RestExtraRef(string val, out string CodelistId, out string CodelistAgencyId, out string CodelistVersion)
        {
            string[] vals = val.Split(',');
            string DFId = vals[0];
            string DFAgencyId = vals[1];
            string DFVersion = vals[2];
            CodelistId = vals[3];
            CodelistAgencyId = vals[4];
            CodelistVersion = vals[5];
            string ConceptId = vals[6];

            return string.Format("DataflowID({0}).DimensionId({1}).DFAgency({2}).DFVersion({3})", DFId, ConceptId, DFAgencyId, DFVersion);
        }

        public static string ReplaceSoapVals(string val, string xmlQuery)
        {
            string[] vals = val.Split(',');
            string DFId = vals[0];
            string DFAgencyId = vals[1];
            string DFVersion = vals[2];
            string CodelistId = vals[3];
            string CodelistAgencyId = vals[4];
            string CodelistVersion = vals[5];
            string ConceptId = vals[6];

            xmlQuery = xmlQuery.Replace("###AGENCYID###", CodelistAgencyId);
            xmlQuery = xmlQuery.Replace("###RESOURCE###", CodelistId);
            xmlQuery = xmlQuery.Replace("###VERSION###", CodelistVersion);
            xmlQuery = xmlQuery.Replace("###DFAGENCYID###", DFAgencyId);
            xmlQuery = xmlQuery.Replace("###DATAFLOWID###", DFId);
            xmlQuery = xmlQuery.Replace("###DFVERSION###", DFVersion);
            xmlQuery = xmlQuery.Replace("###DIMENSION###", ConceptId);

            return xmlQuery;

        }
    }
}
