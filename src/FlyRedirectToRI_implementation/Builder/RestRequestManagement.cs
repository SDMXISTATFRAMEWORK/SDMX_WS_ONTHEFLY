using FlyController;
using FlyController.Model;
using FlyRedirectToRI_implementation.Interfaces;
using FlyRedirectToRI_implementation.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FlyRedirectToRI_implementation.Builder
{
    /// <summary>
    /// Build <see cref="IFlyWriterBody"/> response Message for REST request and contains method for parse response
    /// </summary>
    public class RestRequestManagement : IRequestManagement
    {
        /// <summary>
        /// Contains all object and methods for writing a response <see cref="IRestStructureQuery"/>
        /// </summary>
        public string RestQuery { get; set; }
        /// <summary>
        /// entire request string
        /// </summary>
        public string RequestAccept { get; set; }

        /// <summary>
        /// Build <see cref="IFlyWriterBody"/> response Message for REST request
        /// </summary>
        /// <returns>the <see cref="IFlyWriterBody"/></returns>
        public IFlyWriterBody CreateResponseMessage()
        {
            string EndPoint = FlyConfiguration.RIWebServices.RestUri + RestQuery;
            RIRequestManagement request = new RIRequestManagement(EndPoint, null, RequestAccept);
            HttpWebRequest RestReq = request.InvokeRESTMethod();
            return new FlyRIWriterBody(RestReq);
        }
    }
}
