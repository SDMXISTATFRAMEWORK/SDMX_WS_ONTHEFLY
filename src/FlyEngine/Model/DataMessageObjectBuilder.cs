using Org.Sdmxsource.Sdmx.Api.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FlyEngine.Manager;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System.Xml;
using System.IO;
using Org.Sdmxsource.Sdmx.Api.Manager.Retrieval;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using FlyController.Model;
using FlyController;
using FlyController.Model.Error;
using FlyMapping.Model;
using FlyEngine.Engine.Data;
using System.Data.SqlClient;
using OnTheFlyLog;
using FlyController.Model.Streaming;

namespace FlyEngine.Model
{
    /// <summary>
    /// Object Model contains information for Data Message
    /// </summary>
    public class DataMessageObjectBuilder
    {
        /// <summary>
        /// Data builder object Contains data to return and function for write return streaming data
        /// </summary>
        public IDataTableMessageObject _tableResponse { get; set; }
        /// <summary>
        /// Type of request, both SDMX 2.0 and Sdmx 2.1
        /// </summary>
        public MessageTypeEnum MessageType { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public SdmxSchemaEnumType _versionTypeResp { get; set; }
        /// <summary>
        /// Retrieval Manager contains DataStructure information
        /// </summary>
        public RetrievalManager _retrievalManager { get; set; }
        /// <summary>
        /// Dimension At Observation, describe if query is TimeSeries or CrossSectional and contains "AllDimensions" for FLAT result
        /// </summary>
        public string DimensionAtObservation { get; set; }



        /// <summary>
        ///Start Write data message and Call Write20DataMessage or Write21DataMessage
        /// Create a XElement to return back
        /// </summary>
        /// <param name="TableResponse">Headers of response</param>
        /// <param name="writer"> Contains the object of transport used for transmitting data in streaming</param>
        internal void WriteDataMessage(IDataTableMessageObject TableResponse, IFlyWriter writer)
        {


            try
            {
                this._tableResponse = TableResponse;
                WriterDataBase writerData = null;
                if (writer.FlyMediaType==FlyMediaEnum.Rdf)
                    writerData = new WriteRDFDataMessage();
                else if (writer.FlyMediaType == FlyMediaEnum.Dspl)
                    writerData = new WriteDSPLDataMessage();
                else if (writer.FlyMediaType == FlyMediaEnum.Json)
                    writerData = new WriteJsonDataMessage();
                else if (_versionTypeResp == SdmxSchemaEnumType.VersionTwo)
                    writerData = new Write20DataMessage();
                else
                    writerData = new Write21DataMessage();

                writerData.MessageType = this.MessageType;
                writerData._tableResponse = this._tableResponse;
                writerData._retrievalManager = this._retrievalManager;
                writerData.DimensionAtObservation = this.DimensionAtObservation;

                writerData.WriteDataMessage(writer);
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Total Rows return from Database {0}", this._tableResponse.RowsCounter);



                //StreamReader rdr = new System.IO.StreamReader(ms);
                //ms.Position = 0;
                //string DSDris = rdr.ReadToEnd();
                ////return XElement.Parse(DSDris);
                //ms.Position = 0;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.WriteDataMessage, ex);
            }
        }


    }
}
