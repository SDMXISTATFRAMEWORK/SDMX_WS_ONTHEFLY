using FlyController.Model.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Determinate if REST query must be redirect in non-standard function for redirect a codelist constrained
    /// </summary>
    public class RedirectForCodelistConstrain
    {
        #region Property & Init

        /// <summary>
        /// Structure request type
        /// </summary>
        public string _structure { get; set; }
        /// <summary>
        /// Structure Agency Code
        /// </summary>
        public string _agencyId { get; set; }
        /// <summary>
        /// Identificable of maintenable request, if null is ALL
        /// </summary>
        public string _resourceId { get; set; }
        /// <summary>
        /// Structure Version
        /// </summary>
        public string _version { get; set; }
        /// <summary>
        /// Other query parameter
        /// </summary>
        public System.Collections.Specialized.NameValueCollection _queryParameters { get; set; }
       
        #endregion

        /// <summary>
        /// Checks whether the query is a request for codelist constrained
        /// </summary>
        /// <param name="ContrainOption"></param>
        /// <returns></returns>
        public System.Collections.Specialized.NameValueCollection CheckExistReference(out string ContrainOption)
        {
            ContrainOption = null;
            if (_structure.Trim().ToLower() != "codelist" || string.IsNullOrEmpty(_resourceId))
                return _queryParameters;
            foreach (string item in _queryParameters)
            {
                if (item.Trim().ToLower() == "references")
                {
                    string val = _queryParameters.Get(item);
                    if (val.Trim().ToLower().StartsWith("dataflowid("))
                    {
                        ContrainOption = val;
                        _queryParameters.Set(item, "Dataflow");
                        return _queryParameters;
                    }
                }
            }
            return _queryParameters;
        }

        /// <summary>
        /// Parse a parameter "references" of query for retreival codelist constrained
        /// </summary>
        /// <param name="ConstrainData">a references parameter value</param>
        /// <param name="ConstrainDataFlowCode">ref DataFlowID</param>
        /// <param name="ConstrainDataFlowAgency">ref DataFlowAgency</param>
        /// <param name="ConstrainDataFlowVersion">ref DataFlowVersion</param>
        /// <param name="ConstrainConceptCode">ref conceptID</param>
        /// <returns>a boolean indicating whether the parsing is unsuccessful</returns>
        public static bool ParseContrainReferences(string ConstrainData, ref string ConstrainDataFlowCode, ref string ConstrainDataFlowAgency, ref string ConstrainDataFlowVersion, ref string ConstrainConceptCode)
        {
            try
            {
                string DataFlowIdKey = "DataflowID";
                string DataFlowAgencyKey = "DFAgency";
                string DataFlowVersionKey = "DFVersion";
                string DimensionIdKey = "DimensionId";

                string[] Queries = ConstrainData.Split(new string[1] { ")." }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var Query in Queries)
                {
                    if (Query.ToLower().StartsWith(DataFlowIdKey.ToLower()))
                        ConstrainDataFlowCode = Query;
                    else if (Query.ToLower().StartsWith(DataFlowAgencyKey.ToLower()))
                        ConstrainDataFlowAgency = Query;
                    else if (Query.ToLower().StartsWith(DataFlowVersionKey.ToLower()))
                        ConstrainDataFlowVersion = Query;
                    else if (Query.ToLower().StartsWith(DimensionIdKey.ToLower()))
                        ConstrainConceptCode = Query;
                }
               
                //Controlli dei Dati
                if (string.IsNullOrEmpty(ConstrainDataFlowCode))
                    throw new Exception("DataFlow Id is required");
                if (string.IsNullOrEmpty(ConstrainDataFlowAgency))
                    throw new Exception("DataFlow Agency is required");
                if (string.IsNullOrEmpty(ConstrainDataFlowVersion))
                    throw new Exception("DataFlow Version is required");
                if (string.IsNullOrEmpty(ConstrainConceptCode))
                    throw new Exception("Dimension Id is required");

                if (ConstrainDataFlowCode.Length > DataFlowIdKey.Length)
                    ConstrainDataFlowCode = ConstrainDataFlowCode.Substring(DataFlowIdKey.Length + 1, ConstrainDataFlowCode.Length - (DataFlowIdKey.Length + (ConstrainDataFlowCode.EndsWith(")") ? 2 : 1)));
                if (ConstrainDataFlowAgency.Length > DataFlowAgencyKey.Length)
                    ConstrainDataFlowAgency = ConstrainDataFlowAgency.Substring(DataFlowAgencyKey.Length + 1, ConstrainDataFlowAgency.Length - (DataFlowAgencyKey.Length + (ConstrainDataFlowAgency.EndsWith(")") ? 2 : 1)));
                if (ConstrainDataFlowVersion.Length > DataFlowVersionKey.Length)
                    ConstrainDataFlowVersion = ConstrainDataFlowVersion.Substring(DataFlowVersionKey.Length + 1, ConstrainDataFlowVersion.Length - (DataFlowVersionKey.Length + (ConstrainDataFlowVersion.EndsWith(")") ? 2 : 1)));
                if (ConstrainConceptCode.Length > DimensionIdKey.Length)
                    ConstrainConceptCode = ConstrainConceptCode.Substring(DimensionIdKey.Length + 1, ConstrainConceptCode.Length - (DimensionIdKey.Length + (ConstrainConceptCode.EndsWith(")") ? 2 : 1)));

                //Controlli dei Dati
                if (string.IsNullOrEmpty(ConstrainDataFlowCode))
                    throw new Exception("DataFlow Id is required");
                if (string.IsNullOrEmpty(ConstrainDataFlowAgency))
                    throw new Exception("DataFlow Agency is required");
                if (string.IsNullOrEmpty(ConstrainDataFlowVersion))
                    throw new Exception("DataFlow Version is required");
                if (string.IsNullOrEmpty(ConstrainConceptCode))
                    throw new Exception("Dimension Id is required");

              
                return true;

            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(typeof(RedirectForCodelistConstrain), FlyExceptionObject.FlyExceptionTypeEnum.ConstrainParsingError, e);
            }
        }
    }
}
