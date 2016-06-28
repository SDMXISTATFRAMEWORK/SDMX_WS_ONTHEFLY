using FlyController.Model;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Builder
{
    /// <summary>
    /// Static Function used in some Codelist Builder
    /// </summary>
    public class CodelistItemBuilder
    {
        /// <summary>
        /// Create a Single Mutable Code Object
        /// </summary>
        /// <param name="codeId">Code Object Code</param>
        /// <param name="names">Code Object Names Description</param>
        /// <param name="ParentCode">Code Object Parent code (or Null if not have a parent)</param>
        /// <returns>Mutable Code Object ICodeMutableObject</returns>
        public static ICodeMutableObject BuildCodeObjects(string codeId, List<SdmxObjectNameDescription> names, string ParentCode)
        {
            try
            {
                ICodeMutableObject code = new CodeMutableCore();
                code.Id = codeId;
                foreach (SdmxObjectNameDescription item in names)
                    code.AddName(item.Lingua, item.Name);

                if (!string.IsNullOrEmpty(ParentCode))
                    code.ParentCode = ParentCode;

                return code;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(CodelistItemBuilder), FlyExceptionObject.FlyExceptionTypeEnum.CreateICodeMutableObject, ex);
            }
        }

    }
}
