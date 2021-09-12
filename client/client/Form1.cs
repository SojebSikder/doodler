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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using System.Net;
using MySql.Data.MySqlClient;
using System.Threading;

namespace client
{
    public partial class Form1 : Form
    {
        MySqlConnection con = new MySqlConnection("server=localhost;user id=root;database=test;allowuservariables=True");
        
        //painting variable
        Image File;
        public Point current = new Point();
        public Point old = new Point();
        public Pen p = new Pen(Color.White, 5);
        public Pen pe = new Pen(Color.Black, 5);

        public Graphics g;
        public int width;
        Bitmap surface;
        Graphics graph;

        string s = "pic";
        int i = 1;

        private readonly Thread SendThread;


        //</> 

        // remote desktop variable


        private readonly TcpClient client = new TcpClient();
        private NetworkStream mainStream;
        private int portNumber;
        //</>
        private Image GrabDesktop()
        {
           // Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Rectangle bounds = panel1.Bounds;

            var screenshot = new Bitmap(panel1.Width, panel1.Height);
            panel1.DrawToBitmap(screenshot, new Rectangle(0, 0, panel1.Width, panel1.Height));           


            //Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            //Graphics graphics = Graphics.FromImage(screenshot);
            //graphics.CopyFromScreen(bounds.X,bounds.Y,0,0,bounds.Size, CopyPixelOperation.SourceCopy);
           
            return screenshot;
        }

        private void SendDesktopImage()
        {
            try
            {

                BinaryFormatter binFormatter = new BinaryFormatter();
                mainStream = client.GetStream();
                binFormatter.Serialize(mainStream, GrabDesktop());
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
         
        }

        public Form1()
        {
            SendThread = new Thread(GetMsg);

            InitializeComponent();

            //drawing
            panel1.AllowDrop = true;
            g = panel1.CreateGraphics();
            p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            pe.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            //surface = new Bitmap(panel1.Width, panel1.Height);
            surface = new Bitmap(panel1.Width+1000, panel1.Height+1000);
            graph = Graphics.FromImage(surface);
            panel1.BackgroundImage = surface;
            panel1.BackgroundImageLayout = ImageLayout.None;

           

          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //TCP networking code
            btnShare.Enabled = false;

            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach(IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily.ToString()=="InterNetwork")
                {
                    localIP = ip.ToString();
                    txtIP.Text=localIP;

                }
            }


            //MYSQL Database Code
            sendTimer.Start();
            try
            {
                con.Open();
                //Display query  
                string Query = "select * from chat;";
                
                MySqlCommand MyCommand2 = new MySqlCommand(Query, con);
                MySqlDataReader mdr;

                mdr = MyCommand2.ExecuteReader();

                while(mdr.Read())
                {
                   // String com = (dr["text"].ToString());
                   // String msg = (mdr["msg"].ToString());                 
                    var name = mdr["username"].ToString();
                    var msg = mdr["msg"].ToString();
                    RTBMsg.Text +=name+" : "+msg+"\n";                                    
                }
                mdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            
            
            portNumber = int.Parse(txtPort.Text);
            try
            {
                client.Connect(txtIP.Text,portNumber);
                btnConnect.Text = "Connected";
               // btnConnect.Enabled = false;
               // btnConnect.ForeColor = Color.White;
              //  MessageBox.Show("Connected");
                btnConnect.Enabled = false;
                btnShare.Enabled = true;
            }
            catch(Exception)
            {
                MessageBox.Show("Failed to connect..");
                btnConnect.Text = "Not Connected";
            }
        }

        private void btnShare_Click(object sender, EventArgs e)
        {
            if(btnShare.Text.StartsWith("Share"))
            {
                timer1.Start();
                btnShare.Text = "Stop Sharing";
            }
            else{
                timer1.Stop();
                btnShare.Text = "Share My Screen";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendDesktopImage();
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This software created by sojeb sikder. (c)SojebSoft");
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            old = e.Location;
            if (radioButton1.Checked)
                width = 1;
            else if (radioButton2.Checked)
                width = 5;
            else if (radioButton3.Checked)
                width = 10;
            else if (radioButton4.Checked)
                width = 15;
            else if (radioButton5.Checked)
                width = 30;
            else if (radioButton6.Checked)
                width = 35;
            else if (radioButton12.Checked)
                width = 70;
            else if (radioButton24.Checked)
                width = 140;

            p.Width = width;
            pe.Width = width;



           
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                current = e.Location;
                graph.DrawLine(p, old, current);
                old = current;

                panel1.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                current = e.Location;
                graph.DrawLine(pe, old, current);
                old = current;
                panel1.Invalidate();

            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
           this.panel1.Invalidate();
          // panel1.Invalidate();
           panel1.Refresh();
           panel1.ResetText();
        }
        private void ClearColor(PaintEventArgs e)
        {
            // Clear screen with teal background.
            e.Graphics.Clear(Color.Black);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                p.Color = cd.Color;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG(*.JPG)|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File = panel1.BackgroundImage;

                File.Save(saveFileDialog.FileName);             
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
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


     


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //SendThread.Start();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            //SendThread.Abort();
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
                    RTBMsg.Text += name + " : " + msg + "\n";
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RTBMsg.Clear();
            GetMsg();

            RTBMsg.SelectionStart = RTBMsg.TextLength;
            RTBMsg.ScrollToCaret();
        }

        private void sendTimer_Tick(object sender, EventArgs e)
        {
            RTBMsg.Clear();
            GetMsg();

            RTBMsg.SelectionStart = RTBMsg.TextLength;
            RTBMsg.ScrollToCaret();
        }

        private void txtMsg_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyData == Keys.Enter) //&& txtMsg.TextLength > 0
            {
              //  RTBMsg.Clear();
                SendMsg();               

                RTBMsg.SelectionStart = RTBMsg.TextLength;
                RTBMsg.ScrollToCaret();

                txtMsg.Clear();
            }
             
        }

      

       
        
       
    }

    public class TPanel : Panel
    {
        public TPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);

        }
    }
}
