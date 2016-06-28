using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace XMLViewer
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : UserControl
    {
        private XmlDocument _xmldocument;
        public Viewer()
        {
            InitializeComponent();
        }

        public XmlDocument xmlDocument
        {
            get { return _xmldocument; }
            set
            {
                _xmldocument = value;
                BindXMLDocument();
            }
        }

        private void BindXMLDocument()
        {
            if (_xmldocument == null)
            {
                xmlTree.ItemsSource = null;
                return;
            }

            XmlDataProvider provider = new XmlDataProvider();
            provider.Document = _xmldocument;
            Binding binding = new Binding();
            binding.Source = provider;
            binding.XPath = "child::node()";
            xmlTree.SetBinding(TreeView.ItemsSourceProperty, binding);
        }

        public string FileSavedPath { get; set; }

        private string _testo = "";
        public string Testo
        {
            get { return _testo; }
            set { _testo = value;
            FileSavedPath = null;
            xmlDocument = null;
                try
                {
                    if (string.IsNullOrEmpty(_testo) || !_testo.StartsWith("<"))
                    {
                         _testo= string.Format("<{0}>{1}</{0}>", "ApplicationInfo", Testo);
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_testo);
                    xmlDocument = doc;
                }
                catch (System.Exception)
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(string.Format("<{0}>{1}</{0}>", "ApplicationError", _testo));
                        xmlDocument = doc;
                    }
                    catch (System.Exception)
                    {
                        
                        
                    }
                }

            }
        }
    }
}
