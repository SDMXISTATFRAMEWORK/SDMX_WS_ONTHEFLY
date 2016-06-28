using FlyController.Model.Delegate;
using OnTheFlyLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model.Streaming
{
    /// <summary>
    /// class that  return the Data result of the request in streaming mode
    /// </summary>
    public class FlyDataWriterBody : FlyWriterBody
    {
        #region Data property
        /// <summary>
        /// Database response
        /// </summary>
        public System.Data.SqlClient.SqlDataReader Rea { get; set; }
        /// <summary>
        /// Sql Connection
        /// </summary>
        public System.Data.SqlClient.SqlConnection Conn { get; set; }
        /// <summary>
        /// Callback for parsing Database response
        /// </summary>
        public GetDBResponseDelegate Parser { get; set; }
        /// <summary>
        /// Callback for Write response
        /// </summary>
        public WriteResponseDelegate Builder { get; set; }


        #endregion

        /// <summary>
        /// Write a data body content into FlyWriter
        /// </summary>
        /// <param name="writer">object of transport used for transmitting data in streaming <see cref="IFlyWriter"/></param>
        public override void WriterBody(IFlyWriter writer)
        {
            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Writing Data Result");
            Parser(Rea, Builder, writer);
            Conn.Close();
            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Writing the result successfully completed");
        }
    }
}
