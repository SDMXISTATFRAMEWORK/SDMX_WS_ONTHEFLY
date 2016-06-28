using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OnTheFly.Model
{

    /// <summary>
    ///     The static <c>WSDL</c> service
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IStaticWsdlService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the WSDL.
        /// </summary>
        /// <param name="name">
        /// The service name.
        /// </param>
        /// <returns>
        /// The stream to the WSDL.
        /// </returns>
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{name}")]
        Stream GetWsdl(string name);

        #endregion
    }
}
