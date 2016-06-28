using FlyController.Model.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model.Delegate
{
    /// <summary>
    /// Delegate for callback of Write response
    /// </summary>
    /// <param name="dt">Headers response</param>
    /// <param name="writer"> Contains the object of transport used for transmitting data in streaming</param>
    public delegate void WriteResponseDelegate(IDataTableMessageObject dt, IFlyWriter writer);

}
