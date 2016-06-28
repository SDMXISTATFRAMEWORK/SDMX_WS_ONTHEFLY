using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.ConceptScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Abstract Extension of ConceptMutableCore for separate Dimension and Attribute
    /// </summary>
    public abstract class ConceptObjectImpl : ConceptMutableCore, IConceptObjectImpl
    {
        /// <summary>
        ///  Identificable Code
        /// </summary>
        public string ConceptObjectCode { get; set; }
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        public List<SdmxObjectNameDescription> ConceptObjectNames { get; set; }
        /// <summary>
        /// Concept Type (Dimension or Attribute)
        /// </summary>
        public ConceptTypeEnum ConceptType { get; set; }

        /// <summary>
        /// Used only for DotStatExtra implementation specify a code of codelist whitout using configuration settings
        /// </summary>
        public string CodelistCode { get; set; }

        /// <summary>
        /// Support Information on Concept in DSD rappresentation
        /// </summary>
        public ConceptDSDInfoObject ConceptDSDInfo { get; set; }
     
        /// <summary>
        /// Create a instance 
        /// </summary>
        /// <param name="code">Concept Code</param>
        /// <param name="names">Concept Descriptions Names</param>
        /// <param name="conceptType">Concept Type</param>
        public ConceptObjectImpl(string code, List<SdmxObjectNameDescription> names, ConceptTypeEnum conceptType)
            : base()
        {
            this.ConceptObjectCode = code;
            this.ConceptObjectNames = names;
            this.ConceptType = conceptType;

            #region Popolo i dati di base
            base.Id = code;
            foreach (SdmxObjectNameDescription item in names)
                base.AddName(item.Lingua, item.Name);
            #endregion

           
        }
      
       
    }
}
