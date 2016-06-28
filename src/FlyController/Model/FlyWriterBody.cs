using FlyController.Model.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// class that allows you to return the result of the request in streaming mode
    /// </summary>
    public abstract class FlyWriterBody : IFlyWriterBody
    {
        /// <summary>
        /// Write a data body content into FlyWriter
        /// </summary>
        /// <param name="writer">object of transport used for transmitting data in streaming <see cref="IFlyWriter"/></param>
        public abstract void WriterBody(IFlyWriter writer);
    }
}
