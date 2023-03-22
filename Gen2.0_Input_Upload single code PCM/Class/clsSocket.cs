using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SetSystemTime;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Data;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class clsSocket
    {
        private const string MES_SETDATETIME = "0001";                                 //MES -> PC
        private const string MES_HEARTBIT = "0002";                                    //MES -> PC

        private const string MES_INPUTHHP = "1205";                                    //PC -> MES
        private const string MES_INPUTHHP_RES = "0205";                                //MES -> PC

        private const string MES_INPUTPBA = "1300";                                    //PC -> MES  
        private const string MES_SEND_PBA = "0300";                                    //MES -> PC

        private const string MES_REQUEST_SN_NPC = "1203";                              //PC -> MES
        private const string MES_REQUEST_SN_NPC_RES = "0203";                          //MES -> PC

        private const string MES_SENDRESULT_NPC = "1204";                              //PC -> MES
        private const string MES_SENDRESULT_NPC_RES = "0204";                          //MES -> PC

        private const string MES_SENDRESULT_PT = "1222";                               //PC -> MES
        private const string MES_SENDRESULT_PT_RES = "0222";                           //MES -> PC

        private const string MES_SENDRESULT_HHP = "1209";                              //PC -> MES
        private const string MES_SENDRESULT_HHP_RES = "0209";                          //MES -> PC

        private const string MES_SEND_IROCV_HHP = "1236";                              //PC -> MES
        private const string MES_SEND_IROCV_HHP_RES = "1236";                          //MES -> PC

        private const string MES_REQUEST_FUNCTIONRESULT_HHP_1208 = "1208";             //PC -> MES
        private const string MES_REQUEST_FUNCTIONRESULT_HHP_0208_RES = "0208";         //MES -> PC

        private const string MES_SETDATETIME_RES = "1001";                             //PC -> MES
        private const string MES_HEARTBIT_RES = "1002";                                //PC -> MES

        private const string MES_WORKER = "1101";                                      //PC -> MES
        private const string MES_WORKER_RES = "0101";                                  //MES -> PC

        //private const string MES_INPUTHHP_PBA = "1211";                                //PC -> MES
        //private const string MES_INPUTHHP_PBA_RES = "0211";                            //MES -> PC
         
        //private const string MES_INPUTINSPEC_PBA = "1212";                             //PC -> MES
        //private const string MES_INPUTINSPEC_PBA_RES = "0212";                         //PC -> MES

        private const string MES_INPUTGEN2 = "1211";                                   //PC -> MES
        private const string MES_INPUTGEN2_RES = "0211";                               //MES -> PC

        private const string MES_EXTGEN2 = "1212";                                     //PC -> MES
        private const string MES_EXTGEN2_RES = "0212";                                 //MES -> PC

        private const string MES_BOXGEN2 = "1501";                                     //PC -> MES
        private const string MES_BOXGEN2_RES = "0501";                                 //MES -> PC

        Socket server;
        IPEndPoint ipe;
        List<Socket> lstclient;

        DatNgayGioHeThong setsystemtime = new DatNgayGioHeThong();
        clsConvertData dataconvert = new clsConvertData();
        ReadData rDt = new ReadData();
        database dtb = new database();
        Excel ex = new Excel();

        Form1 frmmain;
        Thread socketstart;
        Thread clientProcess;

        private string _myIP;
        private string _ip;
        private string _lineno;
        private string _mcid;
        private string _stnid1;
        private string _stnid2;
        private string _stnid3;
        private string _stnid4;
        private string _stnid5;
        private int _port;
        private string _portprocess;
        private string _workerid;

        public string MyIP
        {
            get { return _myIP; }

        }

        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public string Lineno
        {
            get { return _lineno; }
            set { _lineno = value; }
        }

        public string Mcid
        {
            get { return _mcid; }
            set { _mcid = value; }
        }

        public string Stnid5
        {
            get { return _stnid5; }
            set { _stnid5 = value; }
        }

        public string Stnid4
        {
            get { return _stnid4; }
            set { _stnid4 = value; }
        }

        public string Stnid3
        {
            get { return _stnid3; }
            set { _stnid3 = value; }
        }

        public string Stnid2
        {
            get { return _stnid2; }
            set { _stnid2 = value; }
        }

        public string Stnid1
        {
            get { return _stnid1; }
            set { _stnid1 = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string Portprocess
        {
            get { return _portprocess; }
            set { _portprocess = value; }
        }

        public string Workerid
        {
            get { return _workerid; }
            set { _workerid = value; }
        }

        //public string A { get => _a; set => _a = value; }

        public void start(string IP)
        {
            LayIP(IP);
            socketstart = new Thread(new ThreadStart(listensocket));
            socketstart.IsBackground = true;
            socketstart.Start();
        }

        public clsSocket(Form1 _frmmain)
        {
            frmmain = _frmmain;
            lstclient = new List<Socket>();
            setsystemtime = new DatNgayGioHeThong();
            socketstart = new Thread(new ThreadStart(listensocket));
            clientProcess = new Thread(myThreadClient);
            if (socketstart.IsAlive == true) socketstart.Abort();
            if (clientProcess.IsAlive == true) clientProcess.Abort();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public void LayIP(string IP)
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress diachi in host.AddressList)
            {
                if (diachi.AddressFamily.ToString() == "InterNetwork")
                {
                    _myIP = diachi.ToString();
                }
            }
            ipe = new IPEndPoint(IPAddress.Parse(IP), _port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        private void listensocket()
        {
            try
            {
                server.Bind(ipe);
                server.Listen(100);
                lstclient.Clear();
                while (true)
                {
                    Socket sk = server.Accept();
                    lstclient.Add(sk);
                    clientProcess = new Thread(myThreadClient);
                    clientProcess.IsBackground = true;
                    clientProcess.Start(sk);
                }
            }
            catch (Exception)
            {
                frmmain.MES_Connecting = "CANT";;
                return;
            }
        }

        private void myThreadClient(object obj)
        {
            Socket clientsk = (Socket)obj;
            // Vòng lặp quét tín hiệu mes trả về
            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024];
                    int recv = clientsk.Receive(buff);
                    foreach (Socket sk in lstclient)
                    {
                        string str = dataconvert.removenull(System.Text.Encoding.ASCII.GetString(buff));
                        xulydatasocket(str);
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        public void senddata(string str)
        {
            foreach (Socket sk in lstclient)
            {
                byte[] datasendbyte = new byte[1024];
                datasendbyte = System.Text.Encoding.ASCII.GetBytes(str);
                sk.Send(datasendbyte);
            }
        }

        private void xulydatasocket(string str) // Xử lý data từ mes trả về
        {
            string[] data = new string[1024]; // mảng độ dài 1024 (theo mes)           
            data = str.Split(';');           // tách chuỗi bởi kí tự ; thành các phần tử của mảng
            string commandcode = data[0].Substring(21, 4);       // cắt chuỗi str từ kí tự 21 và dài 4 kí tự 

            switch (commandcode)  // Lọc theo biến 
            {
                case MES_INPUTGEN2_RES:
                    /* Mes return kết quả input*/
                    string code = data[0].Substring(91, 50);
                    int errorcode = int.Parse(data[0].Substring(39, 4));
                    frmmain.countPcmMes++;                   
                    for (int i = 0; i < frmmain.arrCodeUpload.Length; i++)
                    {
                        if(code.Trim() == frmmain.arrCodeUpload[i])
                        {
                            if(errorcode == 0)
                            {
                                frmmain.status_PCM[i] = 2;  // Monitor hiển thị mầu nền xanh
                                frmmain.errCodeMes[i] = errorcode.ToString(); // Hiển thị tên code lỗi ra màn hình tương ứng
                                //Save code
                                ex.SaveLog(code.Substring(0, 14) + "|" + DateTime.Now.ToString("yyyyMMddHHmmss"), frmmain.txt_modelName.Text, "OnMes");
                                //Sum slg
                                frmmain.txt_qty.Text = (int.Parse(frmmain.txt_qty.Text) + 1).ToString();
                                //History
                                if (frmmain.clickButn == true)
                                {
                                    rDt.SaveData(@Application.StartupPath + "\\History\\Upload\\UploadStock.txt", code.Substring(0, 14) + "|" + DateTime.Now.ToString("yyyyMMddHHmmss") + "|" + frmmain.idInp + "|" + frmmain.namInp);
                                    frmmain.sentSucc = false;
                                }
                                else
                                {
                                    if (frmmain.newCodeUpl == false)
                                    {
                                        frmmain.dgv_stk.Rows[i].Cells["Barcode"].Style.BackColor = Color.Green;
                                    }                                    
                                }
                            }
                            else
                            {
                                frmmain.status_PCM[i] = 3;  // Monitor hiển thị mầu nền đỏ
                                frmmain.errCodeMes[i] = errorcode.ToString(); // Hiển thị tên code lỗi ra màn hình tương ứng
                                if (frmmain.clickButn == true)
                                {
                                    frmmain.clickButn = false;
                                    frmmain.sentFail = true;
                                }
                                else
                                {
                                    if(frmmain.newCodeUpl == false)
                                    {
                                        if (frmmain.dgv_stk.Rows.Count > 1)
                                        {
                                            //Xoa dong vua them
                                            DataRow drToDelete = frmmain.dtStk.Rows[frmmain.dgv_stk.Rows.Count - 2];
                                            frmmain.dtStk.Rows.Remove(drToDelete);
                                        }
                                    }   
                                } 
                            }
                        }
                    }

                    if (frmmain.countPcmMes == frmmain.arrCodeUpload.Length - 1)
                    {
                        //Lưu stock
                        if (frmmain.dgv_stk.Rows.Count > 1)
                        {
                            //Xoa dgv
                            for (int i = 0; i < frmmain.dgv_stk.Rows.Count - 2; i++)
                            {
                                if (frmmain.dgv_stk.Rows[i].Cells["Barcode"].Style.BackColor == Color.Green)
                                {
                                    int indRow = frmmain.dgv_stk.Rows[i].Index;
                                    DataRow drToDelete = frmmain.dtStk.Rows[indRow];
                                    frmmain.dtStk.Rows.Remove(drToDelete);
                                    i--;
                                }
                            }

                            FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + frmmain.mol + "_Stock.txt", FileMode.Create);
                            StreamWriter sw_stk = new StreamWriter(fs_stk);
                            sw_stk.WriteLine(frmmain.dgv_stk.Columns[1].HeaderText + "|" + frmmain.dgv_stk.Columns[2].HeaderText);
                            for (int m = 0; m < frmmain.dgv_stk.Rows.Count - 1; m++)
                            {
                                if (string.IsNullOrEmpty(frmmain.dgv_stk.Rows[m].Cells["STT"].Value.ToString()) == false)
                                {
                                    sw_stk.WriteLine(frmmain.dgv_stk.Rows[m].Cells["Barcode"].Value.ToString() + "|" + frmmain.dgv_stk.Rows[m].Cells["DateTime"].Value.ToString());
                                }
                            }
                            sw_stk.Close();
                            fs_stk.Close();
                        }
                    }
                    //MES conersation
                    frmmain.lstBx_PcMes.Items.Add("[Rec]" + data[0]);
                    frmmain.MES_Connecting = "CAN";
                    saveRec(data[0]);
                    frmmain.newCodeUpl = false;
                    break;
                case MES_WORKER_RES:  /*Lấy Worker_name từ Mes*/      //0101              
                    frmmain.MES_Connecting = "CAN";
                    break;
                case MES_SETDATETIME: //0001
                    string thoigian = data[0].Substring(25, 14);
                    string nam = thoigian.Substring(0, 4);
                    string thang = thoigian.Substring(4, 2);
                    string ngay = thoigian.Substring(6, 2);
                    string gio = thoigian.Substring(8, 2);
                    string phut = thoigian.Substring(10, 2);
                    string giay = thoigian.Substring(12, 2);
                    setsystemtime.DatNgay(ngay, thang, nam);
                    Response_DateTimeSet();
                    frmmain.MES_Connecting = "CAN";
                    break;
                case MES_HEARTBIT: // 0002
                    frmmain.MES_Connecting = "CAN";
                    RESPONSE_HEARBIT(data[0].Substring(39, 4));
                    break;
                case MES_HEARTBIT_RES: // 1002
                    frmmain.MES_Connecting = "CAN";
                    RESPONSE_HEARBIT(data[0].Substring(39, 4));
                    break;
                default:
                    break;
            }
        }

        private void HanldeMesResponse(string code, string errCode, string namErr)
        {
            string subTxtCode = string.Empty;
            switch (frmmain.mol)
            {
                case "Main":
                    //lay ra textbox code
                    foreach (var controls in frmmain.pl_Main.Controls)
                    {
                        subTxtCode = string.Empty;
                        if (controls is TextBox)
                        {
                            var textBox = (TextBox)controls;
                            if (textBox.Name.Contains("Code") && textBox.Text == code)
                            {
                                subTxtCode = textBox.Name.Substring(0, 4);
                                if (int.Parse(errCode) > 0)
                                {
                                    textBox.BackColor = Color.Red;
                                }
                                else
                                {
                                    textBox.BackColor = Color.Green;
                                }
                                break;
                            }
                        }
                    }
                    //lay ra textbox error MES
                    foreach (var controls in frmmain.pl_Main.Controls)
                    {
                        if (controls is TextBox)
                        {
                            var textBox = (TextBox)controls;
                            if (textBox.Name.Contains("Mes") && textBox.Name.Contains(subTxtCode))
                            {
                                textBox.Text = errCode + ":" + namErr;
                                if(int.Parse(errCode) > 0)
                                {
                                    textBox.BackColor = Color.Red;
                                }
                                else
                                {
                                    textBox.BackColor = Color.Green;
                                }
                                break;
                            }
                        }
                    }
                    break;
                case "Cell":
                    //lay ra textbox code
                    foreach (var controls in frmmain.pl_Cell.Controls)
                    {
                        subTxtCode = string.Empty;
                        if (controls is TextBox)
                        {
                            var textBox = (TextBox)controls;
                            if (textBox.Name.Contains("Code") && textBox.Text == code)
                            {
                                subTxtCode = textBox.Name.Substring(0, 4);
                                if (int.Parse(errCode) > 0)
                                {
                                    textBox.BackColor = Color.Red;
                                }
                                else
                                {
                                    textBox.BackColor = Color.Green;
                                }
                                break;
                            }
                        }
                    }
                    //lay ra textbox error MES
                    foreach (var controls in frmmain.pl_Cell.Controls)
                    {
                        if (controls is TextBox)
                        {
                            var textBox = (TextBox)controls;
                            if (textBox.Name.Contains("Mes") && textBox.Name.Contains(subTxtCode))
                            {
                                textBox.Text = errCode + ":" + namErr;
                                if (int.Parse(errCode) > 0)
                                {
                                    textBox.BackColor = Color.Red;
                                }
                                else
                                {
                                    textBox.BackColor = Color.Green;
                                }
                                break;
                            }
                        }
                    }
                    break;
                case "Sub":
                    //lay ra textbox code
                    foreach (var controls in frmmain.pl_Sub.Controls)
                    {
                        subTxtCode = string.Empty;
                        if (controls is TextBox)
                        {
                            var textBox = (TextBox)controls;
                            if (textBox.Name.Contains("Code") && textBox.Text == code)
                            {
                                subTxtCode = textBox.Name.Substring(0, 4);
                                if (int.Parse(errCode) > 0)
                                {
                                    textBox.BackColor = Color.Red;
                                }
                                else
                                {
                                    textBox.BackColor = Color.Green;
                                }
                                break;
                            }
                        }
                    }
                    //lay ra textbox error MES
                    foreach (var controls in frmmain.pl_Sub.Controls)
                    {
                        if (controls is TextBox)
                        {
                            var textBox = (TextBox)controls;
                            if (textBox.Name.Contains("Mes") && textBox.Name.Contains(subTxtCode))
                            {
                                textBox.Text = errCode + ":" + namErr;
                                if (int.Parse(errCode) > 0)
                                {
                                    textBox.BackColor = Color.Red;
                                }
                                else
                                {
                                    textBox.BackColor = Color.Green;
                                }
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Response_DateTimeSet()
        {
            string msg;
            msg = "";
            msg = "@";
            msg += _lineno;
            msg += _mcid;
            msg += dataconvert.insert_Blank_Right("COMMSVR", 8);
            msg += MES_SETDATETIME_RES;
            msg += DateTime.Now.ToString("yyyyMMddHHmmss");
            msg += dataconvert.insert_Blank_Left("0", 4);
            msg += dataconvert.insert_Blank_Left("0", 6);
            msg += ":";
            msg += "*;";
            senddata(msg);
        }

        private void RESPONSE_HEARBIT(string Responsebit)
        {
            string msg;
            msg = "";
            msg = "@";
            msg += _lineno;
            msg += _mcid;
            msg += dataconvert.insert_Blank_Right("COMMSVR", 8);
            msg += MES_HEARTBIT_RES;
            msg += DateTime.Now.ToString("yyyyMMddHHmmss");
            switch (int.Parse(Responsebit))
            {
                case 0:
                    msg += dataconvert.insert_Blank_Left("1", 4);
                    break;
                case 1:
                    msg += dataconvert.insert_Blank_Left("0", 4);
                    break;
            }

            msg += dataconvert.insert_Blank_Left("0", 6);
            msg += ":";
            msg += "*;";
            senddata(msg);
        }

        public void Sent_Input_Gen2(string barcode, ListBox lstBx)
        {
            string stnid = "";
            stnid = Stnid1;
            string msg;
            msg = "@"; //
            msg += _lineno;//
            msg += _mcid;//
            msg += dataconvert.insert_Blank_Right("MES", 8);//
            msg += MES_INPUTGEN2; //
            msg += DateTime.Now.ToString("yyyyMMddHHmmss"); //
            msg += dataconvert.insert_Blank_Right("0", 4);   //           
            msg += dataconvert.insert_Blank_Left("109", 6);
            msg += ":";//
            msg += dataconvert.insert_Blank_Left(stnid, 8); //
            msg += dataconvert.insert_Blank_Left(_portprocess, 3); //
            msg += dataconvert.insert_Blank_Right(_workerid, 30); //
            msg += dataconvert.insert_Blank_Right(barcode, 50); //array barcode
            msg += dataconvert.insert_Blank_Right("1", 3); //Array size 
            msg += dataconvert.insert_Blank_Right("1", 4); //PCM no
            msg += "O";
            msg += dataconvert.insert_Blank_Right("", 10); //ItemData
            msg += "*;";
            saveSend(msg);  // Lưu log gửi lên mes
            SendData(msg);
            //Mes conersation
            lstBx.Items.Add("[Send]" + msg);
        }

        public void Sent_Inspection_Gen2(string barcode, TextBox txt)
        {
            //string stnid = "";
            //stnid = Stnid1;
            //string msg;
            //msg = "@"; //
            //msg += _lineno;//
            //msg += _mcid;//
            //msg += dataconvert.insert_Blank_Right("MES", 8);//
            //msg += MES_EXTGEN2; //
            //msg += DateTime.Now.ToString("yyyyMMddHHmmss"); //
            //msg += dataconvert.insert_Blank_Right("0", 4);  //           
            //msg += dataconvert.insert_Blank_Left("119", 6);
            //msg += ":";//
            //msg += dataconvert.insert_Blank_Left(stnid, 8); //
            //msg += dataconvert.insert_Blank_Left(_portprocess, 3);//
            //msg += dataconvert.insert_Blank_Right(_workerid, 30);//
            //msg += dataconvert.insert_Blank_Right(barcode, 50);// array barcode 
            //msg += dataconvert.insert_Blank_Right("1", 3);  // Array size 
            //msg += dataconvert.insert_Blank_Right("1", 4);// PCM no
            //msg += "O";
            //msg += dataconvert.insert_Blank_Right(stnid, 20); // ItemData 
            //msg += "*;";
            //saveSend(msg);
            //SendData(msg);
            ////Mes conersation
            //if (txt.Text == "")
            //{
            //    txt.Text = "[Send]" + msg;
            //}
            //else
            //{
            //    txt.AppendText("\r\n" + "[Send]" + msg);
            //}  
        }

        public void Sent_Box_Gen2(string insLot, string barcodeBox, string lotSize, string PCMboxing, TextBox txt)
        {
            //string clsSw = "";
            //if (int.Parse(PCMboxing) < int.Parse(lotSize))
            //{
            //    clsSw = "S";
            //}
            //else
            //{
            //    clsSw = "C";
            //}
            //string stnid = "";
            //stnid = Stnid1;
            //string msg;
            //msg = "@"; //
            //msg += _lineno;//
            //msg += _mcid;//
            //msg += dataconvert.insert_Blank_Right("MES", 8);//
            //msg += MES_BOXGEN2; //
            //msg += DateTime.Now.ToString("yyyyMMddHHmmss"); //
            //msg += dataconvert.insert_Blank_Right("0", 4);  //           
            //msg += dataconvert.insert_Blank_Left("102", 6);
            //msg += ":";//
            //msg += dataconvert.insert_Blank_Left(stnid, 8); //
            //msg += dataconvert.insert_Blank_Left(_portprocess, 3);//
            //msg += dataconvert.insert_Blank_Right(_workerid, 30);//
            //msg += dataconvert.insert_Blank_Right(insLot, 15);//Inspect Lot
            //msg += clsSw;//Close SW
            //msg += dataconvert.insert_Blank_Right(barcodeBox, 40);// array barcode 
            //msg += dataconvert.insert_Blank_Right(lotSize, 5);  // Lot size 
            //msg += "*;";
            //saveSend(msg);
            //SendData(msg);
            ////Mes conersation
            //txt.Text = txt.Text + "\n" + msg;
        }

        public void Request_WorkerID(string ID)
        {
            string msg;
            msg = "@";
            msg += _lineno;
            msg += _mcid;
            msg += dataconvert.insert_Blank_Right("MES", 8);
            msg += MES_WORKER;
            msg += DateTime.Now.ToString("yyyyMMddHHmmss");
            msg += dataconvert.insert_Blank_Right("0", 4);
            msg += dataconvert.insert_Blank_Left("88", 6);
            msg += ":";
            msg += _stnid1;
            msg += _portprocess;
            msg += dataconvert.insert_Blank_Left("", 30);
            msg += DateTime.Now.ToString("yyyyMMddHHmmss");
            msg += "000";
            msg += dataconvert.insert_Blank_Right(ID, 30);
            msg += "*;";
            senddata(msg);
        }

        public void saveSend(string msg)   // Lưu log gửi lên mes
        {
            string DataReadTime1 = DateTime.Now.ToString("yyyyMMddHHmmss");
            DirectoryInfo DayDataFolder1 = new DirectoryInfo(@Application.StartupPath + "\\Log\\MES\\" + DataReadTime1.Remove(4) + "\\" + DataReadTime1.Substring(4, 2));
            if (DayDataFolder1.Exists == false)
            {
                DayDataFolder1.Create();
            }

            FileStream MESLog = new FileStream(@Application.StartupPath + "\\Log\\MES\\" + DataReadTime1.Remove(4) + "\\" + DataReadTime1.Substring(4, 2) + "\\" + DataReadTime1.Substring(4, 4) + "LOG(Send).txt", FileMode.Append);

            using (StreamWriter MESLogWrite = new StreamWriter(MESLog))
            {
                MESLogWrite.Write("[SEND]" + DateTime.Now.ToString("HHmmss.ff") + " " + msg + "\r\n");
                MESLogWrite.Close();
            }
            MESLog.Close();
        }

        public void saveRec(string msg)  // Lưu log RECEIVE nhận từ mes
        {
            string DataReadTime1 = DateTime.Now.ToString("yyyyMMddHHmmss");
            DirectoryInfo DayDataFolder1 = new DirectoryInfo(@Application.StartupPath + "\\Log\\MES\\" + DataReadTime1.Remove(4) + "\\" + DataReadTime1.Substring(4, 2));
            if (DayDataFolder1.Exists == false)
            {
                DayDataFolder1.Create();
            }

            FileStream MESLog = new FileStream(@Application.StartupPath + "\\Log\\MES\\" + DataReadTime1.Remove(4) + "\\" + DataReadTime1.Substring(4, 2) + "\\" + DataReadTime1.Substring(4, 4) + "LOG(Rec).txt", FileMode.Append);

            using (StreamWriter MESLogWrite = new StreamWriter(MESLog))
            {
                MESLogWrite.Write("[RECEIVE]" + DateTime.Now.ToString("HHmmss.ff") + " " + msg + "\r\n");
                MESLogWrite.Close();
            }
            MESLog.Close();
        }

        public void SendData(string str)
        {
            foreach (Socket sk in lstclient)
            {
                byte[] datasendbyte = new byte[1024];
                datasendbyte = System.Text.Encoding.ASCII.GetBytes(str);
                sk.Send(datasendbyte);
            }
        }

        public void Stop()
        {
            if (socketstart.IsAlive == true) socketstart.Abort();
            if (clientProcess.IsAlive == true) clientProcess.Abort();
            frmmain.MES_Connecting = "CANT";
        }
    }
}
