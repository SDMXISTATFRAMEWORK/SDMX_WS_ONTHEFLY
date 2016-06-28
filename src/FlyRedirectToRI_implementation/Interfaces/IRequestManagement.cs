using FlyController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyRedirectToRI_implementation.Interfaces
{
    /// <summary>
    /// Interface for provide methods to Rest and Soap Request
    /// </summary>
    public interface IRequestManagement
    {
        /// <summary>
        /// Build <see cref="IFlyWriterBody"/> response message
        /// </summary>
        /// <returns>the <see cref="IFlyWriterBody"/></returns>
        IFlyWriterBody CreateResponseMessage();
    }
}
