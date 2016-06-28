using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using System;
using System.Collections.Generic;
namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetCategoryScheme implementation of SDMXObjectBuilder for creation of CategorisationScheme and CategoryScheme
    /// </summary>
    public interface IGetCategoryScheme : ISDMXObjectBuilder
    {
        /// <summary>
        /// Get and Create All Categorisation
        /// </summary>
        /// <returns>List of <see cref="ICategorisationObject"/></returns>
        List<ICategorisationObject> _GetCategorisation();
        /// <summary>
        /// Get and Create CategoryScheme
        /// </summary>
        /// <returns><see cref="ICategorySchemeObject"/></returns>
        List<ICategorySchemeObject> _GetCategory();
       /// <summary>
        /// Populate a CategoryScheme and list of Categorisation property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        new void Build();
    }
}
