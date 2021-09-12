using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;


using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using MySql.Data.MySqlClient;

namespace server
{

    public partial class Form2 : Form
    {
        MySqlConnection con = new MySqlConnection("server=localhost;user id=root;database=test;allowuservariables=True");

        private int port;
        private IPAddress localAddr;
        private TcpClient client;
        private TcpListener server;
        private NetworkStream mainStream;
     

        private Thread Listening;
        private Thread GetImage;

        public Form2()
        {

           // port = Port;
            //localAddr = IPAddress.Parse(local);
           //int Port = int.Parse(txtPort.Text);
           //string local = txtIP.Text;


          

            InitializeComponent();

            
        }

       

        private void StartListening()
        {
            try
            {
                while (!client.Connected)
                {

                    server.Start();
                    client = server.AcceptTcpClient();
                }
                GetImage.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopListening()
        {
            server.Stop();
            client = null;
            if (Listening.IsAlive) Listening.Abort();
            if (GetImage.IsAlive) Listening.Abort();
        }

        private void ReceiveImage()
        {
            try
            {
                BinaryFormatter binFormatter = new BinaryFormatter();
                while (client.Connected)
                {
                    mainStream = client.GetStream();
                    pictureBox1.Image = (Image)binFormatter.Deserialize(mainStream);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
           // IPAddress localAddr = IPAddress.Parse("192.168.0.106");

            //server = new TcpListener(IPAddress.Any,port);            
            server = new TcpListener(localAddr, port);
            Listening.Start();
            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopListening();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {        
            Image File;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG(*.JPG)|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File = pictureBox1.Image;

                    File.Save(saveFileDialog.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                
            }

           
        }
       

        private void Form2_Load(object sender, EventArgs e)
        {
            //Connection
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

            int Port = int.Parse(txtPort.Text);
            string local = txtIP.Text;

            port = Port;
            localAddr = IPAddress.Parse(local);


            client = new TcpClient();
            Listening = new Thread(StartListening);
            GetImage = new Thread(ReceiveImage);



            //Mysql Database Program
            receiveTimer.Start();
            try
            {
                con.Open();
                //Display query  
                string Query = "select * from chat;";

                MySqlCommand MyCommand2 = new MySqlCommand(Query, con);
                MySqlDataReader mdr;

                mdr = MyCommand2.ExecuteReader();

                while (mdr.Read())
                {
                    // String com = (dr["text"].ToString());
                    // String msg = (mdr["msg"].ToString());                 
                    var name = mdr["username"].ToString();
                    var msg = mdr["msg"].ToString();
                    rtbMsg.Text += name + " : " + msg + "\n";
                }
                mdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void receiveTimer_Tick(object sender, EventArgs e)
        {
            rtbMsg.Clear();
            GetMsg();

            rtbMsg.SelectionStart = rtbMsg.TextLength;
            rtbMsg.ScrollToCaret();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            // Send Message
            try
            {
                con.Open();
                //This is my insert query in which i am taking input from the user through windows forms  
                string Query = "insert into chat(username,msg,email) values('" + txtUser.Text + "','" + txtMsg.Text + "','" + txtEmail.Text + "');";
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(Query, con);
                MySqlDataReader MyReader2;
                // con.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  

                //play sound
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"sound.wav");
                player.Play();
                // MessageBox.Show("Insert Data Successfully");
                while (MyReader2.Read())
                {
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            txtMsg.Clear();
        }

        private void GetMsg()
        {
            try
            {
                con.Open();
                //Display query  
                string Query = "select * from chat;";

                MySqlCommand MyCommand2 = new MySqlCommand(Query, con);
                MySqlDataReader mdr;

                mdr = MyCommand2.ExecuteReader();

                while (mdr.Read())
                {
                    // String com = (dr["text"].ToString());
                    // String msg = (mdr["msg"].ToString());                 
                    var name = mdr["username"].ToString();
                    var msg = mdr["msg"].ToString();
                    rtbMsg.Text += name + " : " + msg + "\n";
                }

                mdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SendMsg()
        {
            // Send Message
            try
            {
                con.Open();
                //This is my insert query in which i am taking input from the user through windows forms  
                string Query = "insert into chat(username,msg,email) values('" + txtUser.Text + "','" + txtMsg.Text + "','" + txtEmail.Text + "');";
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(Query, con);
                MySqlDataReader MyReader2;
                // con.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  

                //play sound
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"sound.wav");
                player.Play();
                // MessageBox.Show("Insert Data Successfully");
                while (MyReader2.Read())
                {
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            IPAddress local = IPAddress.Parse(txtIP.Text);

           // new Form2(int.Parse(txtPort.Text), txtIP.Text).Show();

            btnListen.Enabled = false;
        }

        private void txtMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) //&& txtMsg.TextLength > 0
            {
                //  RTBMsg.Clear();
                SendMsg();

                rtbMsg.SelectionStart = rtbMsg.TextLength;
                rtbMsg.ScrollToCaret();

                txtMsg.Clear();
            }
        }
    }
}
