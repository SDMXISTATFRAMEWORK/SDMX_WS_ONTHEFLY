using FlyController.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace RESTSdmx
{
    /// <summary>
    /// the StructureResource Interface
    /// </summary>
    [ServiceContract(Namespace = Constant.ServiceNamespace, SessionMode = SessionMode.NotAllowed)]
    public interface  IStructureResource
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get structure.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <param name="agencyId">
        /// The agency id.
        /// </param>
        /// <param name="resourceId">
        /// The resource id.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{structure}/{agencyId}/{resourceId}/{version}/")]
        Message GetStructure(string structure, string agencyId, string resourceId, string version);

        /// <summary>
        /// The get structure all.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{structure}/")]
        Message GetStructureAll(string structure);

        /// <summary>
        /// The get structure all ids latest.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <param name="agencyId">
        /// The agency id.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{structure}/{agencyId}/")]
        Message GetStructureAllIdsLatest(string structure, string agencyId);

        /// <summary>
        /// The get structure latest.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <param name="agencyId">
        /// The agency id.
        /// </param>
        /// <param name="resourceId">
        /// The resource id.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{structure}/{agencyId}/{resourceId}/")]
        Message GetStructureLatest(string structure, string agencyId, string resourceId);

        #endregion
    }
}
