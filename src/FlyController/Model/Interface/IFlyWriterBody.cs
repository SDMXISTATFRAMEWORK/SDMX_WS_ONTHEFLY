using FlyController.Model.Streaming;
using System;
namespace FlyController.Model
{
    /// <summary>
    /// interface that allows you to return the result of the request in streaming mode
    /// </summary>
    public interface IFlyWriterBody
    {
        /// <summary>
        /// Write a data body content into FlyWriter
        /// </summary>
        /// <param name="writer">object of transport used for transmitting data in streaming <see cref="IFlyWriter"/></param>
        void WriterBody(IFlyWriter writer);
    }
}
