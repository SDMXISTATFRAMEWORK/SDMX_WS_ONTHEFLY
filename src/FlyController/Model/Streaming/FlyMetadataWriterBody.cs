using FlyController.Model.Error;
using OnTheFlyLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyController.Model.Streaming
{
    /// <summary>
    ///  class that  return the Metatdata result of the request in streaming mode
    /// </summary>
    public class FlyMetadataWriterBody : FlyWriterBody
    {
        #region Metadata property
        /// <summary>
        /// Sdmx Structure Format
        /// </summary>
        public Org.Sdmxsource.Sdmx.SdmxObjects.Model.SdmxStructureFormat StructureFormat { get; set; }
        /// <summary>
        /// Sdmx Object contains all metadata info to write into response
        /// </summary>
        public Org.Sdmxsource.Sdmx.Api.Model.Objects.ISdmxObjects SdmxObject { get; set; }
        #endregion

        /// <summary>
        /// Write a metadata body content into FlyWriter
        /// </summary>
        /// <param name="writer">object of transport used for transmitting data in streaming <see cref="IFlyWriter"/></param>
        public override void WriterBody(IFlyWriter writer)
        {
            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Writing Metadata Result");
            switch (writer.FlyMediaType)
            {
                case FlyMediaEnum.Sdmx:
                    Org.Sdmxsource.Sdmx.Structureparser.Manager.StructureWriterManager Struwriter = new Org.Sdmxsource.Sdmx.Structureparser.Manager.StructureWriterManager(new Org.Sdmxsource.Sdmx.Structureparser.Factory.SdmxStructureWriterFactory(writer.__SdmxXml));
                    Struwriter.WriteStructures(SdmxObject, StructureFormat, null);
                    break;
                case FlyMediaEnum.Rdf:
                    RDFProvider.Structure.Manager.StructureRDFWritingManager RDFStruwriter = new RDFProvider.Structure.Manager.StructureRDFWritingManager(new RDFProvider.Structure.Factory.RDFStructureWriterFactory(writer.__SdmxXml));
                    RDFStruwriter.RDFWriteStructures(SdmxObject, null);
                    break;
                case FlyMediaEnum.Dspl:
                case FlyMediaEnum.Json:
                default:
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.NotImplemented, new Exception("MediaType=" + writer.FlyMediaType.ToString()));
            }

            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Writing the result successfully completed");
        }
    }
}
