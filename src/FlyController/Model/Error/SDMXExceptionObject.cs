using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model.Error
{
    /// <summary>
    /// SDMXExceptionObject representing a Sdmx standard Error
    /// </summary>
    public class SDMXExceptionObject
    {
        /// <summary>
        /// Sdmx Standard Error Type
        /// </summary>
        public SdmxErrorCodeEnumType _sdmxErrorCodeEnum { get; set; }
        /// <summary>
        /// Sdmx Standard Error Code
        /// </summary>
        public int _sdmxErrorCode { get; set; }
        /// <summary>
        /// Sdmx Standard Error Text
        /// </summary>
        public string _sdmxErrorText { get; set; }

        /// <summary>
        /// Static List of this object Instances.
        /// Association of Type, Code, Text
        /// </summary>
        public static List<SDMXExceptionObject> SDMXExceptionList = new List<SDMXExceptionObject>()
        {
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.InternalServerError , _sdmxErrorCode= 500, _sdmxErrorText="Internal Server Error" },
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.NoResultsFound , _sdmxErrorCode= 100 , _sdmxErrorText="No Results Found"},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.NotImplemented , _sdmxErrorCode= 501 , _sdmxErrorText="Not Implemented"},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.Null , _sdmxErrorCode= 1000 , _sdmxErrorText=""},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.ResponseSizeExceedsServiceLimit , _sdmxErrorCode= 510 , _sdmxErrorText="Response Size Exceeds Service Limit"},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.ResponseTooLarge , _sdmxErrorCode= 130 , _sdmxErrorText="Response Too Large"},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.SemanticError , _sdmxErrorCode= 150 , _sdmxErrorText="Semantic Error"},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.ServiceUnavailable , _sdmxErrorCode= 503 , _sdmxErrorText="Service Unavailable"},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.SyntaxError , _sdmxErrorCode= 140 , _sdmxErrorText="Syntax Error"},
             new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.Unauthorised , _sdmxErrorCode= 110 , _sdmxErrorText="Unauthorised"},
        };

        /// <summary>
        /// Find this istance in Static SDMXExceptionList
        /// </summary>
        /// <param name="_sdmxErrorCodeEnumType">Sdmx Standard Error Type</param>
        /// <returns>Founded SDMXExceptionObject</returns>
        public static SDMXExceptionObject Get(SdmxErrorCodeEnumType _sdmxErrorCodeEnumType)
        {
            try
            {
                return SDMXExceptionList.Find(em => em._sdmxErrorCodeEnum == _sdmxErrorCodeEnumType);
            }
            catch (Exception)
            {
                return new SDMXExceptionObject() { _sdmxErrorCodeEnum= SdmxErrorCodeEnumType.InternalServerError , _sdmxErrorCode= 500, _sdmxErrorText="Internal Server Error" };
            }
        }
    }
}
