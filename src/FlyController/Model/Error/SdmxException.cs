using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace FlyController.Model.Error
{
    /// <summary>
    /// Object that representing application Exception: contains All information for build a Sdmx standard Error response
    /// </summary>
    public class SdmxException : Exception
    {
        /// <summary>
        /// Fault Code (Standard SDMX)
        /// </summary>
        public int SDMXFaultCode { get; set; }
        /// <summary>
        /// Fault Message (Standard SDMX)
        /// </summary>
        public string SDMXFaultMessage { get; set; }

        /// <summary>
        /// Uri Object that fired Exception
        /// </summary>
        public string ProcSource { get; set; }
        /// <summary>
        /// Exception Detail Text
        /// </summary>
        public string MessageText { get; set; }
        /// <summary>
        /// Exception Detail Object
        /// </summary>
        public FlyMessageError MessageError { get; set; }
        /// <summary>
        /// Internal Intance of FlyExceptionObject for creation all SdmxException
        /// </summary>
        public FlyExceptionObject FlyException;

        /// <summary>
        /// Create Instance of SdmxException
        /// </summary>
        /// <param name="Control">Object that fired Exception (this for all Object, typeof(ClassName) for staticFunction)</param>
        /// <param name="typeErr">Internal Exception type Enumerator</param>
        public SdmxException(object Control, FlyExceptionObject.FlyExceptionTypeEnum typeErr)
            : this(Control, typeErr, null)
        { }

        /// <summary>
        /// Create Instance of SdmxException
        /// </summary>
        /// <param name="Control">Object that fired Exception (this for all Object, typeof(ClassName) for staticFunction)</param>
        /// <param name="typeErr">Internal Exception type Enumerator</param>
        /// <param name="Ex">Additional Exception</param>
        public SdmxException(object Control, FlyExceptionObject.FlyExceptionTypeEnum typeErr, Exception Ex)
        {
            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Error, "Start Writing Error type {0} message {1}", typeErr.ToString(), Ex==null?"":Ex.Message);

            this.FlyException = FlyExceptionObject.Get(typeErr);

            //Fault
            this.SDMXFaultCode = this.FlyException.SDMXException._sdmxErrorCode;
            this.SDMXFaultMessage = this.FlyException.SDMXException._sdmxErrorText;

            //detail
            this.MessageText = string.Format("{0}", FlyException.FlyExceptionText);
            bool IsEmptyDetail = false;
            if (Ex == null)
            {
                IsEmptyDetail = true;
                Ex = new Exception(this.MessageText);
            }
            this.MessageError = new FlyMessageError(Ex, IsEmptyDetail);

            try
            {
                if (Control is string)
                    this.ProcSource = Control as string;
                else if (Control is Type)
                    this.ProcSource = (Control as Type).FullName;
                else
                    this.ProcSource = Control.GetType().FullName;
            }
            catch (Exception)
            {
                this.ProcSource = Ex.Source;
            }
        }

        /// <summary>
        /// Convert SdmxErrorCodeEnumType of SDMXException in HttpStatusCode
        /// </summary>
        /// <returns>Return a HttpStatusCode of Error for a REST call </returns>
        public HttpStatusCode GetRESTStatusCode()
        {
            switch (this.FlyException.SDMXException._sdmxErrorCodeEnum)
            {
                case SdmxErrorCodeEnumType.InternalServerError:
                    return HttpStatusCode.InternalServerError;
                case SdmxErrorCodeEnumType.NoResultsFound:
                    return HttpStatusCode.NotFound;
                case SdmxErrorCodeEnumType.NotImplemented:
                    return HttpStatusCode.NotImplemented;
                case SdmxErrorCodeEnumType.Null:
                    return HttpStatusCode.OK;
                case SdmxErrorCodeEnumType.ResponseSizeExceedsServiceLimit:
                    return HttpStatusCode.RequestEntityTooLarge;
                case SdmxErrorCodeEnumType.ResponseTooLarge:
                    return HttpStatusCode.RequestEntityTooLarge;
                case SdmxErrorCodeEnumType.SemanticError:
                    return HttpStatusCode.BadRequest;
                case SdmxErrorCodeEnumType.ServiceUnavailable:
                    return HttpStatusCode.ServiceUnavailable;
                case SdmxErrorCodeEnumType.SyntaxError:
                    return HttpStatusCode.BadRequest;
                case SdmxErrorCodeEnumType.Unauthorised:
                    return HttpStatusCode.Unauthorized;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }

   
}
