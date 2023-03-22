using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class clsScanner
    {
        SerialPort SCANNER;
        Form1 frmmain;

        private string _COMnum;
        private string _data_scanner;
        private bool chk_LodCof = false;

        public string COMnum
        {
            get { return _COMnum; }
            set { _COMnum = value; }
        }

        public clsScanner(Form1 _frmmain)
        {
            frmmain = _frmmain;
            SCANNER = new SerialPort();
        }

        public bool CntScanner(string baurate, string dataBits, string parity, string handShking)
        {
            try
            {
                SCANNER.PortName = _COMnum;
                SCANNER.BaudRate = int.Parse(baurate);
                SCANNER.DataBits = int.Parse(dataBits);
                switch(parity)
                {
                    case "None":
                        SCANNER.Parity = Parity.None;
                        break;
                    case "Odd":
                        SCANNER.Parity = Parity.Odd;
                        break;
                    case "Even":
                        SCANNER.Parity = Parity.Even;
                        break;
                    case "Mark":
                        SCANNER.Parity = Parity.Mark;
                        break;
                    case "Space":
                        SCANNER.Parity = Parity.Space;
                        break;
                    default:
                        break;
                }
                
                switch(handShking)
                {
                    case "None":
                        SCANNER.Handshake = Handshake.None;
                        break;
                    case "RTS":
                        SCANNER.Handshake = Handshake.RequestToSend;
                        break;
                    case "RTS+XOnXOff":
                        SCANNER.Handshake = Handshake.RequestToSendXOnXOff;
                        break;
                    case "XOnXOff":
                        SCANNER.Handshake = Handshake.XOnXOff;
                        break;
                    default:
                        break;
                }
                                
                SCANNER.ReadBufferSize = 1024;
                SCANNER.WriteBufferSize = 1024;
                SCANNER.DtrEnable = true;
                SCANNER.DataReceived += SCANNER_DataReceived;
                SCANNER.Open();
                return true;
            }
            catch (Exception)
            {
                SCANNER.Close();
                return false;
            }
        }

        public void LoadConfig(int config)
        {
            chk_LodCof = true;
            SCANNER.Write("BLOAD," + config + "\r\n");
            //Thread.Sleep(3000);
            //SCANNER.WriteLine("LON");
            //Thread.Sleep(300);
            //SCANNER.WriteLine("LOFF");
        }

        void SCANNER_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (frmmain.chb_usbScn.Checked == false)
            {
                Thread.Sleep(200);
                _data_scanner = SCANNER.ReadExisting();
                Thread.Sleep(200);
                _data_scanner = RemoveLine(_data_scanner);               

                if(chk_LodCof == true)
                {
                    chk_LodCof = false;
                    if(_data_scanner.Contains("ER"))
                    {
                        MessageBox.Show("LOAD CONFIG SCANNER THẤT BẠI - HÃY THỬ LẠI NHÉ!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (_data_scanner == "" || _data_scanner == "\r\n" || _data_scanner == null || _data_scanner == "\\r\\n")
                    {
                        MessageBox.Show("THÔNG BÁO LỖI \r\n\r\n\r\n KHÔNG ĐỌC ĐƯỢC CODE - HÃY THỬ LẠI NHÉ!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        frmmain.textBox1.Text = _data_scanner;
                    }
                }               
            }
        }

        //public string ReadDataSR2000()
        //{
        //    var reader = new StreamReader(stream);
        //    var writer = new StreamWriter(stream);
        //    writer.AutoFlush = true;
        //    writer.WriteLine("LON");
        //    //Thread.Sleep(500);
        //    //writer.WriteLine("LOFF");
        //    string data = string.Empty;
        //    data = reader.ReadLine();
        //    return data;
        //}

        //public void OffScan()
        //{
        //    var reader = new StreamReader(stream);
        //    var writer = new StreamWriter(stream);
        //    writer.AutoFlush = true;
        //    writer.WriteLine("LOFF");
        //}

        //public string SetConfig(int config)
        //{
        //    var reader = new StreamReader(stream);
        //    var writer = new StreamWriter(stream);
        //    writer.AutoFlush = true;
        //    writer.WriteLine("BLOAD," + config.ToString());
        //    Thread.Sleep(3000);
        //    writer.WriteLine("LON");
        //    Thread.Sleep(300);
        //    writer.WriteLine("LOFF");
        //    string data = string.Empty;
        //    data = reader.ReadLine();
        //    return data;

        //}

        public void DisCnt()
        {
            try
            {
                SCANNER.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string RemoveLine(string value)
        {
            string str = "";
            for (int i = 0; i < value.Length; i++)
            {
                if (value.Substring(i, 1) != "\n" && value.Substring(i, 1) != "\r")
                {
                    str += value.Substring(i, 1);
                }
            }
            return str;
        }
    }
}
