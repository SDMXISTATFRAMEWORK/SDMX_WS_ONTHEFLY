using System;
using System.Collections.Generic;
using System.Xml;

namespace FlyController.Streaming
{  
    /// <summary>
    /// The Stream controller
    /// </summary>
    /// <typeparam name="T">
    /// The type of the output writer
    /// </typeparam>
    public interface IStreamController<in T>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Stream XML output to <paramref name="writer"/>
        /// </summary>
        /// <param name="writer">
        ///     The writer to write the output to
        /// </param>
        /// <param name="actions"></param>
        void StreamTo(T writer, Queue<Action> actions);

        #endregion

       
    }
}