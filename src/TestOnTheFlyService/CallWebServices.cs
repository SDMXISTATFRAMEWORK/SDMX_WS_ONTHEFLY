using FlyCallWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TestOnTheFlyService
{
    public class CallWebServices
    {
        public  const int MAX_OUTPUT_LENGTH = 10485760; // 10MB
        private static WsConfigurationSettings wsConfigSettings = null;

        public static WsConfigurationSettings GetSettings(string _Endpoint, string Operation)
        {
            if (wsConfigSettings == null)
            {
                wsConfigSettings = new WsConfigurationSettings()
                {
                    //WebService
                    RestEndPoint = "",
                    Prefix = ConfigurationManager.AppSettings["Prefix"],


                    //Credenziali Http
                    EnableHTTPAuthenication = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableHTTPAuthenication"]),
                    Domain = ConfigurationManager.AppSettings["Domain"],
                    Username = ConfigurationManager.AppSettings["Username"],
                    Password = ConfigurationManager.AppSettings["Password"],

                    //Proxy
                    EnableProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProxy"]),
                    UseSystemProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSystemProxy"]),
                    ProxyServer = ConfigurationManager.AppSettings["ProxyServer"],
                    ProxyServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ProxyServerPort"]),
                    ProxyUsername = ConfigurationManager.AppSettings["ProxyUsername"],
                    ProxyPassword = ConfigurationManager.AppSettings["ProxyPassword"],
                };
            }

            wsConfigSettings.EndPoint = _Endpoint;
            if (string.IsNullOrEmpty(Operation))
            {
                wsConfigSettings.WSDL = "";
                wsConfigSettings.Operation = "";
            }
            else
            {
                wsConfigSettings.WSDL = _Endpoint + "?wsdl";
                wsConfigSettings.Operation = Operation;
            }
            return wsConfigSettings;
        }

    }
}
