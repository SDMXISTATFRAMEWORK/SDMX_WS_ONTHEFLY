using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace TestOnTheFlyService
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();


        }
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                ChkEnableHTTPAuthenication_CheckedChanged(this, new EventArgs());
                ChkEnableProxy_CheckedChanged(this, new EventArgs());


                txtMainUri.Text = ConfigurationManager.AppSettings["MainUri"];
                txtSoap20.Text = ConfigurationManager.AppSettings["EndPoint20"];
                txtSoap21.Text = ConfigurationManager.AppSettings["EndPoint21"];
                ChkEnableHTTPAuthenication.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableHTTPAuthenication"]);
                txtDomain.Text = ConfigurationManager.AppSettings["Domain"];
                txtUsername.Text = ConfigurationManager.AppSettings["Username"];
                txtPassword.Text = ConfigurationManager.AppSettings["Password"];
                ChkEnableProxy.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProxy"]);
                txtProxyServer.Text = ConfigurationManager.AppSettings["ProxyServer"];
                txtProxyServerPort.Text = ConfigurationManager.AppSettings["ProxyServerPort"];
                txtProxyUsername.Text = ConfigurationManager.AppSettings["ProxyUsername"];
                txtProxyPassword.Text = ConfigurationManager.AppSettings["ProxyPassword"];
                ChkUseSystemProxy.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSystemProxy"]);
                
            }
            catch (Exception)
            {
                MessageBox.Show("Error to load Configuration", "On the Fly");
                this.Close();
            }

            base.OnLoad(e);
        }

        private void btnReboot_Click(object sender, EventArgs e)
        {
            try
            {
                Configuration appConfig = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                if (!txtMainUri.Text.EndsWith("\\") && !txtMainUri.Text.EndsWith("/"))
                    txtMainUri.Text = txtMainUri.Text + "/";
                appConfig.AppSettings.Settings["MainUri"].Value = txtMainUri.Text;
                appConfig.AppSettings.Settings["EndPoint20"].Value = txtSoap20.Text;
                appConfig.AppSettings.Settings["EndPoint21"].Value = txtSoap21.Text ;
                appConfig.AppSettings.Settings["EnableHTTPAuthenication"].Value = ChkEnableHTTPAuthenication.Checked.ToString();
                appConfig.AppSettings.Settings["Domain"].Value = txtDomain.Text;
                appConfig.AppSettings.Settings["Username"].Value = txtUsername.Text;
                appConfig.AppSettings.Settings["Password"].Value = txtPassword.Text;
                appConfig.AppSettings.Settings["EnableProxy"].Value = ChkEnableProxy.Checked.ToString();
                appConfig.AppSettings.Settings["UseSystemProxy"].Value = ChkUseSystemProxy.Checked.ToString();
                appConfig.AppSettings.Settings["ProxyServer"].Value = txtProxyServer.Text;
                appConfig.AppSettings.Settings["ProxyServerPort"].Value = txtProxyServerPort.Text;
                appConfig.AppSettings.Settings["ProxyUsername"].Value = txtProxyUsername.Text;
                appConfig.AppSettings.Settings["ProxyPassword"].Value = txtProxyPassword.Text;

               
                appConfig.Save(ConfigurationSaveMode.Full);
                Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error to Save Configuration: " + ex.Message, "On the Fly");
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChkEnableHTTPAuthenication_CheckedChanged(object sender, EventArgs e)
        {
            if (!ChkEnableHTTPAuthenication.Checked)
            {
                txtDomain.Text = "";
                txtUsername.Text = "";
                txtPassword.Text = "";
               

            }
            txtDomain.Enabled = ChkEnableHTTPAuthenication.Checked;
            txtUsername.Enabled = ChkEnableHTTPAuthenication.Checked;
            txtPassword.Enabled = ChkEnableHTTPAuthenication.Checked;
        }

        private void ChkEnableProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (!ChkEnableProxy.Checked)
            {
                txtProxyServer.Text = "";
                txtProxyServerPort.Text = "";
                txtProxyUsername.Text = "";
                txtProxyPassword.Text = "";
                ChkUseSystemProxy.Checked = false;
            }
            txtProxyServer.Enabled = ChkEnableProxy.Checked;
            txtProxyServerPort.Enabled = ChkEnableProxy.Checked;
            txtProxyUsername.Enabled = ChkEnableProxy.Checked;
            txtProxyPassword.Enabled = ChkEnableProxy.Checked;
            ChkUseSystemProxy.Enabled = ChkEnableProxy.Checked;
        }

    }
}
