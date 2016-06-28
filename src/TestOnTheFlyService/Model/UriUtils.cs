
namespace TestOnTheFlyService
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Various static URI/URL related helper methods.
    /// </summary>
    public static class UriUtils
    {
        /// <summary>
        /// Fixes the system URI dot .NET bug. This is required to be able to parse REST data queries with wildcards.
        /// </summary>
        /// <seealso cref="http://stackoverflow.com/questions/856885/httpwebrequest-to-url-with-dot-at-the-end"/>
        public static void FixSystemUriDotBug()
        {
            // HACK bug in .NET 4.0 URI. It removes any trailing dots in any part of the URL
            // Workaround from http://stackoverflow.com/questions/856885/httpwebrequest-to-url-with-dot-at-the-end
            MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    var parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        var flagsValue = (int)flagsField.GetValue(parser);

                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                        {
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                        }
                    }
                }
            }
        }
    }
}