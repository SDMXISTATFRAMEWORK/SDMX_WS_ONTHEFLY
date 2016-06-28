using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

namespace OnTheFly.Model
{
    /// <summary>
    /// The soap service host factory.
    /// </summary>
    public class SoapServiceHostFactory : ServiceHostFactory
    {
       
        #region Fields

         /// <summary>
        /// The _type.
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// The WSDL location.
        /// </summary>
        private readonly string _wsdlLocation;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapServiceHostFactory"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="wsdlLocation">
        /// The WSDL location.
        /// </param>
        public SoapServiceHostFactory(Type type, string wsdlLocation)
        {
            this._type = type;
            this._wsdlLocation = wsdlLocation;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create service host.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="baseAddresses">
        /// The base addresses.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceHost"/>.
        /// </returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            try
            {
                // System.Diagnostics.Debugger.Break();
                ServiceHost serviceHost = base.CreateServiceHost(serviceType, baseAddresses);

                var binding = new BasicHttpBinding { TransferMode = TransferMode.Streamed, MessageEncoding = WSMessageEncoding.Text };

                var baseAddress = baseAddresses[0];
                serviceHost.AddServiceEndpoint(this._type, binding, baseAddress);

                serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
                //serviceHost.Description.Behaviors.Add(new SdmxErrorServiceBehaviour(exception => this._faultSoapv20Builder.BuildException(exception, "Unknown")));

                //// serviceHost.Description.Behaviors.Add(new SdmxInspectorBehaviour());
                return serviceHost;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}