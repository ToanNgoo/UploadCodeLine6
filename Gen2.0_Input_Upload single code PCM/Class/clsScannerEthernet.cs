using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class clsScannerEthernet
    {
        private const Int32 BUFFER_SIZE = 1024;
        private const Int32 PORT_NUMBER = 5025;
        static ASCIIEncoding encoding = new ASCIIEncoding();
        TcpClient client = new TcpClient();
        Stream stream;

        public bool Connect(string IP, Int32 PORT_NUM, Button bt)
        {
            try
            {
                client.Connect(IP, PORT_NUM);
                stream = client.GetStream();
                if (client.Connected)
                {
                    bt.BackColor = Color.Green;
                    return true;
                }
                else
                {
                    bt.BackColor = Color.Red;
                    return false;
                }
            }
            catch (Exception)
            {
                bt.BackColor = Color.Red;
                return false;
            }
        }

        public void Disconnect(string IP, Int32 PORT_NUM, Button bt)
        {
            if (client.Connected)
            {
                client.Close();
                bt.BackColor = Color.Red;
            }
            else
            {
                client.Connect(IP, PORT_NUM);
                stream = client.GetStream();
                bt.BackColor = Color.Green;
            }
        }

        public void Setup_Ethernet()
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
        }

        public string ReadDataSR2000()
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            writer.WriteLine("LON");
            //Thread.Sleep(500);
            //writer.WriteLine("LOFF");
            string data = string.Empty;
            data = reader.ReadLine();
            return data;
        }

        public void OffScan()
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            writer.WriteLine("LOFF");
        }

        public string SetConfig(int config)
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            writer.WriteLine("BLOAD," + config.ToString());
            Thread.Sleep(3000);
            writer.WriteLine("LON");
            Thread.Sleep(300);
            writer.WriteLine("LOFF");
            string data = string.Empty;
            data = reader.ReadLine();
            return data;
        }

        public string ReadConfig()
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            string data = string.Empty;
            data = reader.ReadLine();
            return data;
        }

        public string ReadData()
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            writer.WriteLine("LON");
            string data = string.Empty;
            data = reader.ReadLine();
            return data;
        }
    }
}
