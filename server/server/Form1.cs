using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            //int port = int.Parse(txtPort.Text);
            //Form2 f2 = new Form2(port);
            //f2.Show();
           IPAddress local = IPAddress.Parse(txtIP.Text);
        //    new Form2(int.Parse(txtPort.Text),txtIP.Text).Show();
            btnListen.Enabled=false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    txtIP.Text = localIP;

                }
            }
        }
    }
}
