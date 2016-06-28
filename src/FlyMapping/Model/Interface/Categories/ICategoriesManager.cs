using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Categories Manager
    /// CategoriesManager retrieves the data for build  CategoryScheme and CategorisationScheme
    /// </summary>
    public interface ICategoriesManager : IBaseManager
    {
        /// <summary>
        /// Build a CategoryScheme
        /// </summary>
        /// <returns>ICategorySchemeObject for SdmxObject</returns>
        List<ICategorySchemeObject> GetCategoryScheme();

        /// <summary>
        /// Build a Categorisation
        /// </summary>
        /// <returns>list of ICategorisationObject for SdmxObject</returns>
        List<ICategorisationObject> GetCategorisation();


        /// <summary>
        /// Build a Categorisation
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of ICategorisationObject for SdmxObject</returns>
        List<ICategorisationObject> GetCategorisationReferences(IReferencesObject refObj);

        /// <summary>
        /// Build a CategoryScheme
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>ICategorySchemeObject for SdmxObject</returns>
        List<ICategorySchemeObject> GetCategorySchemeReferences(IReferencesObject refObj);

    }
}
