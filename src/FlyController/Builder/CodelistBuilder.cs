using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;

namespace FlyController.Builder
{
    /// <summary>
    /// CodelistBuilder Create a ImmutableInstance of Codelist from Dataflow and Concept 
    /// </summary>
    public class CodelistBuilder : ObjectBuilder
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
        /// List of Code
        /// </summary>
        public List<ICodeMutableObject> CodesObjects { get; set; }

        /// <summary>
        /// Create CodelistBuilder Istance 
        /// </summary>
        /// <param name="parsingObject">Parsing Object <see cref="ISdmxParsingObject"/></param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        public CodelistBuilder(ISdmxParsingObject parsingObject, SdmxSchemaEnumType versionTypeResp) :
            base(parsingObject, versionTypeResp)
        {


        }



        /// <summary>
        /// Create a ImmutableInstance of Codelist
        /// </summary>
        /// <param name="AgencyId">Agency Id</param>
        /// <param name="Version">Artefact Version</param>
        /// <returns>ICodelistObject</returns>
        public ICodelistMutableObject BuildCodelist(string AgencyId, string Version)
        {
            try
            {
                ICodelistMutableObject codelist = new CodelistMutableCore();
                codelist.Id = this.Code;
                codelist.AgencyId = AgencyId;
                codelist.Version = Version;
                if (this.Names == null)
                {
                    this.Names = new List<SdmxObjectNameDescription>();
                    this.Names.Add(new SdmxObjectNameDescription() { Lingua = "en", Name = Code });
                }
                else
                    foreach (SdmxObjectNameDescription item in this.Names)
                        codelist.AddName(item.Lingua, item.Name);





                if (!this.ParsingObject.ReturnStub)
                {
                    Dictionary<string, int> Idduples = new Dictionary<string, int>();
                    //List<string> Idduples = new List<string>();
                    //try
                    //{//ci sono i duplicati ID sotto padri diversi e SDMX scoppia....
                    //    if (CodesObjects != null)
                    //        Idduples = (from c in CodesObjects
                    //                    where CodesObjects.Count(comp => comp.Id == c.Id) > 1
                    //                    select c.Id).Distinct().ToList();
                    //}
                    //catch (Exception)
                    //{
                    //    //Errore nella ricerca dei duplicati ma non fa niente
                    //}
                    foreach (ICodeMutableObject cl in CodesObjects)
                    {
                        if (!Idduples.ContainsKey(cl.Id))
                            Idduples[cl.Id] = 1;
                        else
                            Idduples[cl.Id]++;

                        if (Idduples[cl.Id] == 1)// && codelist.Items.Count(presentCode => presentCode.Id == cl.Id) == 0)
                            codelist.AddItem(cl);
                    }

                    codelist.FinalStructure = TertiaryBool.ParseBoolean(true);
                    codelist.IsPartial = true;
                }


                if (this.ParsingObject.isReferenceOf || this.ParsingObject.ReturnStub)
                {
                    codelist.ExternalReference = TertiaryBool.ParseBoolean(true);
                    codelist.StructureURL = RetreivalStructureUrl.Get(this, codelist.Id, codelist.AgencyId, codelist.Version);
                }
                return codelist;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }


        /// <summary>
        /// Add item to codelist whitout generate a duplicate items
        /// </summary>
        /// <param name="codelist">codelist from which to copy the items</param>
        public void AddItemto(ICodelistMutableObject codelist)
        {
            try
            {

                List<string> Idduples = new List<string>();
                try
                {//ci sono i duplicati ID sotto padri diversi e SDMX scoppia....
                    if (CodesObjects != null)
                        Idduples = (from c in CodesObjects
                                    where CodesObjects.Count(comp => comp.Id == c.Id) > 1
                                    select c.Id).Distinct().ToList();
                }
                catch (Exception)
                {
                    //Errore nella ricerca dei duplicati ma non fa niente
                }


                if (!this.ParsingObject.ReturnStub)
                {
                    foreach (ICodeMutableObject cl in CodesObjects)
                    {
                        if (!Idduples.Contains(cl.Id) && codelist.Items.Count(presentCode => presentCode.Id == cl.Id) == 0)
                            codelist.AddItem(cl);
                    }
                    codelist.FinalStructure = TertiaryBool.ParseBoolean(true);
                }


                if (this.ParsingObject.isReferenceOf || this.ParsingObject.ReturnStub)
                {
                    codelist.ExternalReference = TertiaryBool.ParseBoolean(true);
                    codelist.StructureURL = RetreivalStructureUrl.Get(this, codelist.Id, codelist.AgencyId, codelist.Version);
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }
    }
    //public class chi
    //{
    //    public ICodeMutableObject code { get; set; }
    //    public override string ToString()
    //    {
    //        return string.Format("Code {0} - URN {2} - descr {1}", code.Id, code.Descriptions.Count, code.Urn);
    //    }
    //}
}

