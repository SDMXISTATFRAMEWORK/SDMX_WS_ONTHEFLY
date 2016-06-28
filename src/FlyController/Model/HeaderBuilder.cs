using Estat.Sri.SdmxXmlConstants;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Header;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyController.Model
{
    /// <summary>
    /// HeaderBuilder Create a Header in all Sdmx Structure 
    /// </summary>
    public class HeaderBuilder
    {
        /// <summary>
        /// Create a instance of HeaderBuilder
        /// and create a _header_setting reading settingNode
        /// </summary>
        /// <param name="settingNode">a node representing a header configuration (in file Config)</param>
        public HeaderBuilder(XmlNode settingNode)
        {
            PopulateHeaderSetting(settingNode);
            InitialiseHeader();
        }
        /// <summary>
        /// Header
        /// </summary>
        private IHeader _header;
        /// <summary>
        /// Dictionary with all configuration for header creation 
        /// </summary>
        private Dictionary<string, string> _header_setting = new Dictionary<string, string>();
       
        /// <summary>
        /// create a _header_setting reading settingNode
        /// </summary>
        /// <param name="settingNode"></param>
        private void PopulateHeaderSetting(XmlNode settingNode)
        {
            foreach (XmlNode set in settingNode.ChildNodes)
            {
                if (set.Name == "setting" && set.Attributes != null && set.Attributes["name"] != null && set.ChildNodes.Count == 1 && set.ChildNodes[0].Name == "value")
                {
                    _header_setting.Add(set.Attributes["name"].Value, set.ChildNodes[0].InnerText);
                }
            }
        }
       
        /// <summary>
        ///Create a Header
        /// </summary>
        private void InitialiseHeader()
        {
            try
            {

                IList<ITextTypeWrapper> name = new List<ITextTypeWrapper>();
                name.Add(new TextTypeWrapperImpl(FindHeaderSetting("lang"), FindHeaderSetting("name"), null));

                IList<ITextTypeWrapper> textTypeWrapperSender = new List<ITextTypeWrapper>();
                textTypeWrapperSender.Add(new TextTypeWrapperImpl(FindHeaderSetting("lang"), FindHeaderSetting("sendername"), null));

                IContactMutableObject senderContact = new ContactMutableObjectCore();
                senderContact.AddName(new TextTypeWrapperMutableCore(FindHeaderSetting("lang"), FindHeaderSetting("sendercontactname")));
                senderContact.AddDepartment(new TextTypeWrapperMutableCore(FindHeaderSetting("lang"), FindHeaderSetting("sendercontactdepartment")));
                senderContact.AddRole(new TextTypeWrapperMutableCore(FindHeaderSetting("lang"), FindHeaderSetting("sendercontactrole")));

                if (!string.IsNullOrEmpty(FindHeaderSetting("sendercontacttelephone")))
                {
                    senderContact.AddTelephone(FindHeaderSetting("sendercontacttelephone"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("sendercontactfax")))
                {
                    senderContact.AddFax(FindHeaderSetting("sendercontactfax"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("sendercontactx400")))
                {
                    senderContact.AddX400(FindHeaderSetting("sendercontactx400"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("sendercontacturi")))
                {
                    senderContact.AddUri(FindHeaderSetting("sendercontacturi"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("sendercontactemail")))
                {
                    senderContact.AddEmail(FindHeaderSetting("sendercontactemail"));
                }

                // SENDER
                IContact contactImmutableSender = new ContactCore(senderContact);
                IList<IContact> contactsSender = new List<IContact>();
                contactsSender.Add(contactImmutableSender);
                IParty sender = new PartyCore(textTypeWrapperSender, FindHeaderSetting("senderid"), contactsSender, null);

                IList<ITextTypeWrapper> textTypeWrapperReceiver = new List<ITextTypeWrapper>();
                textTypeWrapperReceiver.Add(new TextTypeWrapperImpl(FindHeaderSetting("lang"), FindHeaderSetting("receivername"), null));

                IContactMutableObject receiverContact = new ContactMutableObjectCore();

                receiverContact.AddName(new TextTypeWrapperMutableCore(FindHeaderSetting("lang"), FindHeaderSetting("receivercontactname")));
                receiverContact.AddDepartment(new TextTypeWrapperMutableCore(FindHeaderSetting("lang"), FindHeaderSetting("receivercontactdepartment")));
                receiverContact.AddRole(new TextTypeWrapperMutableCore(FindHeaderSetting("lang"), FindHeaderSetting("receivercontactrole")));

                if (!string.IsNullOrEmpty(FindHeaderSetting("receivercontacttelephone")))
                {
                    receiverContact.AddTelephone(FindHeaderSetting("receivercontacttelephone"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("receivercontactfax")))
                {
                    receiverContact.AddFax(FindHeaderSetting("receivercontactfax"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("receivercontactx400")))
                {
                    receiverContact.AddX400(FindHeaderSetting("receivercontactx400"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("receivercontacturi")))
                {
                    receiverContact.AddUri(FindHeaderSetting("receivercontacturi"));
                }

                if (!string.IsNullOrEmpty(FindHeaderSetting("receivercontactemail")))
                {
                    receiverContact.AddEmail(FindHeaderSetting("receivercontactemail"));
                }

                // RECEIVER
                IContact contactImmutableReceiver = new ContactCore(receiverContact);
                IList<IContact> contactsReceiver = new List<IContact>();
                contactsReceiver.Add(contactImmutableReceiver);
                IParty receiver = new PartyCore(textTypeWrapperReceiver, FindHeaderSetting("receiverid"), contactsReceiver, null);
                IList<IParty> receiverList = new List<IParty>();
                receiverList.Add(receiver);

                IDictionary<string, string> additionalAttributes = new Dictionary<string, string>();
                additionalAttributes.Add(NameTableCache.GetElementName(ElementNameTable.KeyFamilyRef), FindHeaderSetting("keyfamilyref"));
                additionalAttributes.Add(NameTableCache.GetElementName(ElementNameTable.KeyFamilyAgency), FindHeaderSetting("keyfamilyagency"));
                additionalAttributes.Add(NameTableCache.GetElementName(ElementNameTable.DataSetAgency), FindHeaderSetting("datasetagency"));

                DateTime extracted, prepared, reportingBegin, reportingEnd;
                bool isValid = DateTime.TryParse(FindHeaderSetting("extracted"), out extracted);
                if (!isValid)
                {
                    extracted = DateTime.Now;
                }

                isValid = DateTime.TryParse(FindHeaderSetting("reportingbegin"), out reportingBegin);
                if (!isValid)
                {
                    reportingBegin = DateTime.Now;
                }

                isValid = DateTime.TryParse(FindHeaderSetting("reportingend"), out reportingEnd);
                if (!isValid)
                {
                    reportingEnd = DateTime.Now;
                }

                isValid = DateTime.TryParse(FindHeaderSetting("prepared"), out prepared);
                if (!isValid)
                {
                    prepared = DateTime.Now;
                }

                IList<ITextTypeWrapper> source = new List<ITextTypeWrapper>();
                if (!string.IsNullOrEmpty(FindHeaderSetting("source")))
                {
                    source.Add(new TextTypeWrapperImpl(FindHeaderSetting("lang"), FindHeaderSetting("source"), null));
                }

                this._header = new HeaderImpl(
                    additionalAttributes,
                    null,
                    null,
                    DatasetAction.GetAction(FindHeaderSetting("datasetaction")),
                    FindHeaderSetting("id"),
                    FindHeaderSetting("datasetid"),
                    null,
                    extracted,
                    prepared,
                    reportingBegin,
                    reportingEnd,
                    name,
                    source,
                    receiverList,
                    sender,
                    bool.Parse(FindHeaderSetting("test")));
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateHeader, ex);
            }
        }

        /// <summary>
        /// Search Header Setting in _header_setting
        /// </summary>
        /// <param name="settingName">Setting Name</param>
        /// <returns>Configured Setting Value</returns>
        private string FindHeaderSetting(string settingName)
        {
            if (_header_setting.ContainsKey(settingName))
                return _header_setting[settingName];
            return null;
        }


        /// <summary>
        /// Get Configured Header
        /// </summary>
        /// <returns>IHeader already created</returns>
        public IHeader GetHeader()
        {
            return _header;
        }
    }
}
