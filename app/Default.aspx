<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OnTheFly.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>On The Fly Web Services</title>
</head>
<body style="font-family: Arial;font-size:15px;">
    <form id="form1" runat="server">
        <div id="page-content">
            <div id="page-content-inner">
                <div id="page-header">
                    <div id="page-header-left">
                    </div>
                    <div id="page-header-right">
                    </div>
                </div>
                <div id="page-body">
                    <div id="pb-workarea-pages">

                        <div id="pb-endpoints">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 20%">&nbsp;</td>
                                    <td style="width: 15%">
                                        <img title="On The Fly WebServices" alt="On The Fly WebServices" src="Images/logoistat.jpg" height="50px" /></td>
                                    <td style="width: 45%">
                                        <h1>On The Fly Web Services</h1>
                                    </td>
                                    <td style="width: 20%">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="4">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width: 20%">&nbsp;</td>
                                    <td style="width: 80%" colspan="2">
                                        <table style="width: 100%; border: 1px solid #3399CC; border-collapse: collapse;">
                                            <thead>
                                                <tr>
                                                    <th>Service Name</th>
                                                    <th>Endpoint path</th>
                                                    <th>WSDL link</th>
                                                    <th>XML Schema path</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td style="border: 1px solid #3399CC">Standard SDMX v2.0</td>
                                                    <td style="border: 1px solid #3399CC"><a href="<%= this.EndPointSdmx20%>"><%= this.EndPointSdmx20%></a></td>
                                                    <td style="border: 1px solid #3399CC"><a class="page-links" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}SoapSdmx20?wsdl",this.UrlPath)%>">WSDL</a></td>
                                                    <td style="border: 1px solid #3399CC"><a class="page-links" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}sdmxv20/SDMXMessage.xsd",this.UrlPath)%>">SDMXMessage.xsd</a></td>
                                                </tr>
                                                <tr>
                                                    <td style="border: 1px solid #3399CC">Standard SDMX v2.1</td>
                                                    <td style="border: 1px solid #3399CC"><a href="<%= this.EndPointSdmx21%>"><%= this.EndPointSdmx21%></a></td>
                                                    <td style="border: 1px solid #3399CC"><a class="page-links" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}SoapSdmx21?wsdl",this.UrlPath)%>">WSDL</a></td>
                                                    <td style="border: 1px solid #3399CC"><a class="page-links" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}sdmxv21/SDMXMessage.xsd",this.UrlPath)%>">SDMXMessage.xsd</a></td>
                                                </tr>
                                                <tr>
                                                    <td style="border: 1px solid #3399CC">Rest Metadata</td>
                                                    <td style="border: 1px solid #3399CC"><a href="<%= this.EndPointRestMetadata%>"><%= this.EndPointRestMetadataPath%></a></td>
                                                    <td style="border: 1px solid #3399CC"><a class="page-links" href="<%= this.EndPointRestMetadata%>">Help</a></td>
                                                    <td style="border: 1px solid #3399CC"></td>
                                                </tr>
                                                 <tr>
                                                    <td style="border: 1px solid #3399CC">Rest Data</td>
                                                    <td style="border: 1px solid #3399CC"><a href="<%= this.EndPointRestData%>"><%= this.EndPointRestDataPath%></a></td>
                                                    <td style="border: 1px solid #3399CC"><a class="page-links" href="<%= this.EndPointRestData%>">Help</a></td>
                                                    <td style="border: 1px solid #3399CC"></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td style="width: 20%">&nbsp;</td>
                                </tr>

                                <tr>
                                    <td colspan="4" style="height:50px;">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width: 20%">&nbsp;</td>
                                    <td colspan="2" style="width: 60%">
                                        <table style="width: 100%;" border="1">
                                            <tr>
                                                <td style="width: 50%">
                                                    <span style="font-weight: bold;">Test Client</span>
                                                </td>
                                                <td style="width: 50%">
                                                    <span style="font-weight: bold;">Web Services Documentation</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 50%">
                                                    <a style="font-size:13px;" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}TestClient/xcopy.zip",this.UrlPath)%>"">Test Client setup</a>
                                                </td>
                                                <td style="width: 50%">
                                                    <a style="font-size:13px;" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}doc/OnTheFlyUserGuide.docx",this.UrlPath)%>">On The Fly User Guide</a>
                                                </td>
                                                
                                            </tr>
                                            <tr>
                                                <td style="width: 50%">
                                                    <a style="font-size:13px;" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}doc/OnTheFlyClient.docx",this.UrlPath)%>">Test Client documentation</a>
                                                </td>
                                                <td style="width: 50%">
                                                    <a style="font-size:13px;" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}doc/OnTheFlyTechnicalGuide.docx",this.UrlPath)%>">On The Fly Techical Guide</a>
                                                </td>
                                                
                                            </tr>
                                              <tr>
                                                <td style="width: 50%">
                                                </td>
                                                <td style="width: 50%">
                                                    <a style="font-size:13px;" href="<%= string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}Help/Documentation.chm",this.UrlPath)%>">On The Fly Documentation.chm</a>
                                                </td>
                                                
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 20%">&nbsp;</td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
