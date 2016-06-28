using FlyController.Model.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FlyController
{
    /// <summary>
    /// CommonFunction contains all static Function used by all object in all project
    /// </summary>
    public class CommonFunction
    {
        /// <summary>
        /// Find Xml Sub Nodo
        /// </summary>
        /// <param name="nodo">XmlNode where to search subNode</param>
        /// <param name="Name">Name of SubNode</param>
        /// <returns>XmlNode fouded or Null</returns>
        public static XmlNode FindSubNodo(XmlNode nodo, string Name)
        {
            try
            {
                foreach (XmlNode findnodo in nodo.ChildNodes)
                    if (findnodo.Name == Name)
                        return findnodo;
                return null;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.CommonFunction), FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }
        /// <summary>
        /// For Sdmx 2.1 find a "id" (Attribute) of SubNode named "Ref"
        /// </summary>
        /// <param name="element">XmlNode where to search id</param>
        /// <returns>id ref element</returns>
        public static string FindRefIDNodo21(XElement element)
        {
            try
            {
                return element.Elements().FirstOrDefault(el => el.Name.LocalName == "Ref").Attribute("id").Value;
            }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.CommonFunction), FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        /// <summary>
        /// Find in Header Xml Structure an Element
        /// </summary>
        /// <param name="query">Xml Structure of Header</param>
        /// <param name="ElementToFind">Element name to find</param>
        /// <returns>the element value request or Null if it not exist</returns>
        public static string GetHeaderElement(XmlElement query, string ElementToFind)
        {
            try
            {
                foreach (XmlNode HeaderFind in query.ChildNodes)
                {
                    if (HeaderFind.LocalName.Trim().ToLower() == "header")
                    {
                        foreach (XmlNode child in HeaderFind.ChildNodes)
                        {
                            if (child.LocalName.Trim().ToLower() == ElementToFind.Trim().ToLower())
                                return child.InnerText;
                        }
                        break;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.CommonFunction), FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }
    }
}
