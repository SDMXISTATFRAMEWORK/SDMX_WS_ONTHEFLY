using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestOnTheFlyService
{
    public partial class WinXmlViewer : UserControl
    {
        public WinXmlViewer()
        {
            InitializeComponent();
            this.richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
        }
        public bool SolaLettura
        {
            get { return this.richTextBox1.ReadOnly; }
            set { } //this.richTextBox1.ReadOnly = value; }
        }


        public string Testo
        {
            get { return this.richTextBox1.Text; }
            set
            {
                try
                {
                    this.richTextBox1.SuspendLayout();
                    this.richTextBox1.Text = value;
                    HighlightRTF(this.richTextBox1, DefSizeText);
                }
                catch (Exception)
                { }
                finally
                {
                    this.richTextBox1.ResumeLayout();
                }
            }
        }


        public string FileSavedPath { get; set; }


        private float DefSizeText = 10;


        public static void HighlightRTF(RichTextBox rtb, float DefSizeText)
        {
            int k = 0;

            string str = rtb.Text;

            int st, en;
            int lasten = -1;
            rtb.Select(0, str.Length);
            rtb.SelectionFont = new Font(FontFamily.GenericSansSerif, DefSizeText);
            rtb.SelectionColor = Color.Black;
            if (str.Length > 100000)
                return;
            while (k < str.Length)
            {
                st = str.IndexOf('<', k);

                if (st < 0)
                    break;
               

                if (lasten > 0)
                {
                    rtb.Select(lasten + 1, st - lasten - 1);
                    rtb.SelectionColor = HighlightColors.HC_INNERTEXT;
                }

                en = str.IndexOf('>', st + 1);
                if (en < 0)
                    break;

                k = en + 1;
                lasten = en;

                if (str[st + 1] == '!')
                {
                    rtb.Select(st + 1, en - st - 1);
                    rtb.SelectionColor = HighlightColors.HC_COMMENT;
                    continue;

                }
                String nodeText = str.Substring(st + 1, en - st - 1);


                bool inString = false;

                int lastSt = -1;
                int state = 0;
                /* 0 = before node name
                 * 1 = in node name
                   2 = after node name
                   3 = in attribute
                   4 = in string
                   */
                int startNodeName = 0, startAtt = 0;
                for (int i = 0; i < nodeText.Length; ++i)
                {
                    if (nodeText[i] == '"')
                        inString = !inString;

                    if (inString && nodeText[i] == '"')
                        lastSt = i;
                    else
                        if (nodeText[i] == '"')
                        {
                            rtb.Select(lastSt + st + 2, i - lastSt - 1);
                            rtb.SelectionColor = HighlightColors.HC_STRING;
                        }

                    switch (state)
                    {
                        case 0:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startNodeName = i;
                                state = 1;
                            }
                            break;
                        case 1:
                            if (Char.IsWhiteSpace(nodeText, i))
                            {
                                rtb.Select(startNodeName + st, i - startNodeName + 1);
                                rtb.SelectionColor = HighlightColors.HC_NODE;
                                rtb.SelectionFont = new Font(rtb.SelectionFont, FontStyle.Bold);
                                state = 2;
                            }
                            break;
                        case 2:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startAtt = i;
                                state = 3;
                            }
                            break;

                        case 3:
                            if (Char.IsWhiteSpace(nodeText, i) || nodeText[i] == '=')
                            {
                                rtb.Select(startAtt + st, i - startAtt + 1);
                                rtb.SelectionColor = HighlightColors.HC_ATTRIBUTE;
                                rtb.SelectionFont = new Font(rtb.SelectionFont, FontStyle.Bold);
                                state = 4;
                            }
                            break;
                        case 4:
                            if (nodeText[i] == '"' && !inString)
                                state = 2;
                            break;


                    }

                }
                if (state == 1)
                {
                    rtb.Select(st + 1, nodeText.Length);
                    rtb.SelectionColor = HighlightColors.HC_NODE;
                    rtb.SelectionFont = new Font(rtb.SelectionFont, FontStyle.Bold);
                }


            }


        }
        public class HighlightColors
        {
            public static Color HC_NODE = Color.Firebrick;
            public static Color HC_STRING = Color.Blue;
            public static Color HC_ATTRIBUTE = Color.Black;
            public static Color HC_COMMENT = Color.GreenYellow;
            public static Color HC_INNERTEXT = Color.Black;
        }

        internal void SetSize(float s)
        {
            DefSizeText = s;
            HighlightRTF(this.richTextBox1, DefSizeText);
        }
    }
}
