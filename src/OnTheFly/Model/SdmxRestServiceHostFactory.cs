﻿using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;

namespace OnTheFly.Model
{
    /// <summary>
    /// The sdmx rest service host factory.
    /// </summary>
    public class SdmxRestServiceHostFactory : WebServiceHostFactory
    {
      

        #region Fields

        /// <summary>
        /// The _type.
        /// </summary>
        private readonly Type _type;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SdmxRestServiceHostFactory"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public SdmxRestServiceHostFactory(Type type)
        {
            this._type = type;
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
                ServiceHost serviceHost = base.CreateServiceHost(serviceType, baseAddresses);

                var webBehavior = new WebHttpBehavior { AutomaticFormatSelectionEnabled = false, HelpEnabled = true, FaultExceptionEnabled = false, DefaultBodyStyle = WebMessageBodyStyle.Bare };
                var binding = new WebHttpBinding { TransferMode = TransferMode.Streamed, ContentTypeMapper = new SdmxContentMapper() };
                var endpoint = serviceHost.AddServiceEndpoint(this._type, binding, baseAddresses[0]);

                endpoint.Behaviors.Add(webBehavior);

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
