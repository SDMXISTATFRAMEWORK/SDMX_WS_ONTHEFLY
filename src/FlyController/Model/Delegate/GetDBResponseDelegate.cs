using FlyController.Model.Streaming;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FlyController.Model.Delegate
{
    /// <summary>
    /// Delegate for callback of Database response
    /// </summary>
    /// <param name="rea">SqlDataReader response</param>
    /// <param name="builder"> Delegate for callback of Write response</param>
    /// <param name="writer"> Contains the object of transport used for transmitting data in streaming</param>
    public delegate void GetDBResponseDelegate(SqlDataReader rea, WriteResponseDelegate builder, IFlyWriter writer);

}
