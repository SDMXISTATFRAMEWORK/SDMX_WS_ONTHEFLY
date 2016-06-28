using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model.DbSetting
{
    /// <summary>
    /// All Operations Type ... name will then be converted to run from store procedure
    /// </summary>
    public enum DBOperationEnum
    {
        /// <summary>
        /// Get all GetDataflows (Version 2.0)
        /// </summary>
        GetDataflows,
        /// <summary>
        /// Get all GetDatasets
        /// </summary>
        GetDatasets,
        /// <summary>
        /// Get all Dimensions
        /// </summary>
        GetDimensions,
        /// <summary>
        /// Get all Attributes
        /// </summary>
        GetAttributes,
        /// <summary>
        /// Get all Dimension Codelist Constrain
        /// </summary>
        GetDimensionCodelistConstrain,
        /// <summary>
        /// Get all Attribute Codelist Not Constrained
        /// </summary>
        GetAttributeCodelistNOConstrain,
        /// <summary>
        /// Get all Attribute Codelist Constrain
        /// </summary>
        GetAttributeCodelistConstrain,
        /// <summary>
        /// Get all Dimension Codelist Not Constrained
        /// </summary>
        GetDimensionCodelistNOConstrain,
        /// <summary>
        /// Get Category (TreeTheme)
        /// </summary>
        GetCategory,
        /// <summary>
        /// Get Flag codelist
        /// </summary>
        GetFlags,
        /// <summary>
        /// Get Group Information
        /// </summary>
        GetGroups,
        /// <summary>
        /// Get Data values for DataMessage
        /// </summary>
        GetData,
        /// <summary>
        /// Get All CategoryScheme
        /// </summary>
        GetCategorySchemes,
        /// <summary>
        /// Get All Categorisation, Dataflow INFO
        /// </summary>
        GetCategorisationDataflow,

        /// <summary>
        /// Get Codelist no constrain in MS Database
        /// </summary>
        MSGetCodelist,
        /// <summary>
        /// Get ConceptSchemes in MS Database
        /// </summary>
        MSGetConceptScheme,
        /// <summary>
        /// Get DSDs in MS Database
        /// </summary>
        MSGetDSD,
        /// <summary>
        /// Get Dataflows Code in MS Database
        /// </summary>
        MSGetDataflows,
        /// <summary>
        /// Get Category And Categorisation in MS Database
        /// </summary>
        MSGetCategoryAndCategorisation

    }
}
