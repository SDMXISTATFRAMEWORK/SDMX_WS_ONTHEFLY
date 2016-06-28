using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTheFly.Model
{


    /// <summary>
    ///     The WSDL registry.
    /// </summary>
    public sealed class WsdlRegistry
    {
        #region Static Fields

        /// <summary>
        ///     The singleton instance
        /// </summary>
        private static readonly WsdlRegistry _instance = new WsdlRegistry();

        #endregion

        #region Fields

        /// <summary>
        ///     The _WSDL map
        /// </summary>
        private readonly IDictionary<string, WsdlInfo> _wsdlMap = new Dictionary<string, WsdlInfo>(StringComparer.Ordinal);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="WsdlRegistry" /> class from being created.
        /// </summary>
        private WsdlRegistry()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the singleton instance
        /// </summary>
        public static WsdlRegistry Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        ///     Gets or sets the WSDL REST URL.
        /// </summary>
        /// <value>
        ///     The WSDL rest URL.
        /// </value>
        public string WsdlRestUrl { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="wsdlInfo">
        /// The WSDL information.
        /// </param>
        public void Add(WsdlInfo wsdlInfo)
        {
            this._wsdlMap[wsdlInfo.Name] = wsdlInfo;
        }

        /// <summary>
        /// Adds the specified WSDL info.
        /// </summary>
        /// <param name="wsdlInfos">The WSDL info.</param>
        public void Add(params WsdlInfo[] wsdlInfos)
        {
            foreach (var wsdlInfo in wsdlInfos)
            {
                this._wsdlMap[wsdlInfo.Name] = wsdlInfo;
            }
        }

        ////public string GetBaseDir(string name)
        ////{
        ////    string baseDir;
        ////    if (this._baseDirMap.TryGetValue(name, out baseDir))
        ////    {
        ////        return baseDir;
        ////    }

        ////    return null;
        ////}

        /// <summary>
        /// Gets the WSDL path.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The WSDL path
        /// </returns>
        public WsdlInfo GetWsdlInfo(string name)
        {
            WsdlInfo wsdlPath;
            if (this._wsdlMap.TryGetValue(name, out wsdlPath))
            {
                return wsdlPath;
            }

            return null;
        }

        #endregion
    }
}