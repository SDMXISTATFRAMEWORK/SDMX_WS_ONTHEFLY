using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyController.Model;
using FlyController;
using FlyController.Builder;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;

namespace FlyController.Builder
{
    /// <summary>
    /// DataflowBuilder Create a ImmutableInstance of Dataflow with DataStructure reference
    /// </summary>
    public class DataflowBuilder : ObjectBuilder
    {
        #region IObjectBuilder Property
        /// <summary>
        ///  Identificable Code
        /// </summary>
        public override string Code { get; set; }
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        public override List<SdmxObjectNameDescription> Names { get; set; }
       
        #endregion
        /// <summary>
        /// DataStructure reference
        /// </summary>
        public IStructureReference DataStrunctureRef { get; set; }

        /// <summary>
        /// Create DataflowBuilder Istance 
        /// </summary>
        /// <param name="id"> Identificable Dataflow Code</param>
        /// <param name="names">Dataflow Descriptions Names</param>
        /// <param name="parsingObject">Parsing Object <see cref="ISdmxParsingObject"/></param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        public DataflowBuilder(string id, List<SdmxObjectNameDescription> names, ISdmxParsingObject parsingObject, SdmxSchemaEnumType versionTypeResp) :
             base(parsingObject,versionTypeResp)
        {
            this.Code = id;
            this.Names = names;
        }

     
        /// <summary>
        /// Create a ImmutableInstance of Dataflow
        /// </summary>
        /// <param name="AgencyId">Agency Id</param>
        /// <param name="Version">Artefact Version</param>
        /// <returns>IDataflowObject</returns>
        public IDataflowObject BuildDataflow(string AgencyId, string Version)
        {
            try
            {

                IDataflowMutableObject df = new DataflowMutableCore();
                df.AgencyId = AgencyId;
                df.Version = Version;
                df.Id = this.Code;
                if (this.Names != null)
                    foreach (SdmxObjectNameDescription nome in this.Names)
                        df.AddName(nome.Lingua, nome.Name);

                if (!this.ParsingObject.ReturnStub)
                {
                    if (this.DataStrunctureRef != null)
                        df.DataStructureRef = this.DataStrunctureRef;
                    else 
                        df.DataStructureRef = ReferenceBuilder.CreateDSDStructureReference(this.Code);//Creo una struttura di riferimento
                }
               
                df.FinalStructure = TertiaryBool.ParseBoolean(true);

                if (this.ParsingObject.isReferenceOf || this.ParsingObject.ReturnStub)
                {
                    df.ExternalReference = TertiaryBool.ParseBoolean(true);
                    df.StructureURL = RetreivalStructureUrl.Get(this, df.Id, df.AgencyId, df.Version);
                }
                return df.ImmutableInstance;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }



    }
}
