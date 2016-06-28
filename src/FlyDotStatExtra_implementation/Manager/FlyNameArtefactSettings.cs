using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStatExtra_implementation.Manager
{
    /// <summary>
    /// Concert Artefact Code as Configured
    /// </summary>
    public class FlyNameArtefactSettings
    {
        private ISdmxParsingObject sdmxParsingObject;
        /// <summary>
        /// Inizialize a new Instance of <see cref="FlyNameArtefactSettings"/>
        /// </summary>
        /// <param name="_sdmxParsingObject"> Parsing Object <see cref="ISdmxParsingObject"/></param>
        public FlyNameArtefactSettings(ISdmxParsingObject _sdmxParsingObject)
        {
            this.sdmxParsingObject = _sdmxParsingObject;
        }

        #region Settings
        /// <summary>
        /// Get DataFlow Code From ConceptSchema (Using ConceptSchemeFormat specified in File Config)
        /// </summary>
        /// <returns>DataFlow Code</returns>
        public string GetDataFlowCodeFromConceptSchema()
        {
            try
            {
                string man = this.sdmxParsingObject.MaintainableId.Trim().ToUpper();
                string ConceptForm = FlyConfiguration.ConceptSchemeFormat.Replace("{0}", "").ToUpper();
                if (!man.EndsWith(ConceptForm) && !man.StartsWith(ConceptForm))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ConceptSchemaFormatError, new Exception(this.sdmxParsingObject.MaintainableId));

                string DataFlowCode = null;
                if (man.EndsWith(ConceptForm))
                    DataFlowCode = this.sdmxParsingObject.MaintainableId.Trim().Substring(0, this.sdmxParsingObject.MaintainableId.Trim().Length - ConceptForm.Length);
                else
                    DataFlowCode = this.sdmxParsingObject.MaintainableId.Trim().Substring(ConceptForm.Length);
                if (string.IsNullOrEmpty(DataFlowCode))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ConceptSchemaInvalid, new Exception(this.sdmxParsingObject.MaintainableId));

                return DataFlowCode;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetDataFlowCodeFromConceptSchema, ex);
            }
        }
        /// <summary>
        /// Get Concept Code From Codelist (Using CodelistFormat specified in File Config)
        /// </summary>
        /// <returns>Concept Code</returns>
        public string GetConceptCodeFromCodelist()
        {
            try
            {
                //Controllo se il Codelist inizia o finisce con CodelistFormat cosi da prendermi il nome del Concept
                string man = this.sdmxParsingObject.MaintainableId.Trim().ToUpper();
                string CodelistForm = FlyConfiguration.CodelistFormat.Replace("{0}", "").ToUpper();
                if (!man.EndsWith(CodelistForm) && !man.StartsWith(CodelistForm))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CodelistFormatError, new Exception(this.sdmxParsingObject.MaintainableId));
                string ConceptCode = null;
                if (man.EndsWith(CodelistForm))
                    ConceptCode = this.sdmxParsingObject.MaintainableId.Trim().Substring(0, this.sdmxParsingObject.MaintainableId.Trim().Length - CodelistForm.Length);
                else
                    ConceptCode = this.sdmxParsingObject.MaintainableId.Trim().Substring(CodelistForm.Length);

                if (string.IsNullOrEmpty(ConceptCode))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CodelistInvalid, new Exception(this.sdmxParsingObject.MaintainableId));

                return ConceptCode;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetConceptCodeFromCodelist, ex);
            }
        }
        /// <summary>
        /// Get DataFlow Code From KeyFamily (Using DsdFormat specified in File Config)
        /// </summary>
        /// <returns>DataFlow Code</returns>
        public string GetDataFlowCodeFromKeyFamily()
        {
            try
            {
                //Controllo se il KeyFamily è formattato correttamente (finisce o inizia per DsdFormat) e la parte iniziale o finale è il codice del DataFlow
                string man = this.sdmxParsingObject.MaintainableId.Trim().ToUpper();
                string dsdForm = FlyConfiguration.DsdFormat.Replace("{0}", "").ToUpper();
                if (!man.EndsWith(dsdForm) && !man.StartsWith(dsdForm))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.KeyFamilyFormatError, new Exception(this.sdmxParsingObject.MaintainableId));
                string DataflowId = null;
                if (man.EndsWith(dsdForm))
                    DataflowId = this.sdmxParsingObject.MaintainableId.Trim().Substring(0, this.sdmxParsingObject.MaintainableId.Trim().Length - dsdForm.Length);
                else
                    DataflowId = this.sdmxParsingObject.MaintainableId.Trim().Substring(dsdForm.Length);

                if (string.IsNullOrEmpty(DataflowId))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.KeyFamilyInvalid, new Exception(this.sdmxParsingObject.MaintainableId));

                return DataflowId;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetDataFlowCodeFromKeyFamily, ex);
            }
        }


        #endregion
    }
}
