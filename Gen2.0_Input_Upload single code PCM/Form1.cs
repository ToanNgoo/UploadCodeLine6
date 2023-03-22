using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.OleDb;
using System.IO;
using System.IO.Ports;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    public partial class Form1 : Form
    {
        database dtb = new database();
        Display ds = new Display();
        ReadData rData = new ReadData();
        MES ms = new MES();
        Excel ex = new Excel();
        clsSocket socket;
        clsScanner Scn;
        Thread Display_NhapNhay;
        Thread upStkMes;

        public bool Oracle_Connect = false;
        public string MES_Connecting = "CANT";
        public string lineNo = string.Empty;
        public DataTable dtStk, dtUpl, dtDel;
        public string mol = string.Empty;
        public DataTable dtSetMol;
        public int startSerial, lenSerial, startModCode, lenModCode, minSerial, maxSerial;
        public string dShift = string.Empty;

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Unable
            timer_retStk.Stop();
            textBox1.Visible = false;
            pl_Main.Visible = false;
            pl_Main.Enabled = false;
            pl_Cell.Visible = false;
            pl_Cell.Enabled = false;
            pl_Sub.Visible = false;
            pl_Sub.Enabled = false;
            dShift = find_shift();
            txt_qty.ReadOnly = true;
            rchtxt_guider.SelectionStart = 0;
            rchtxt_guider.SelectionLength = 22;
            rchtxt_guider.SelectionFont = new System.Drawing.Font(rchtxt_guider.SelectionFont, FontStyle.Bold);
            //Enable 
            cbx_modelName.Visible = false;
            //qty
            txt_qty.Text = rData.ReadQty();
            //set screen
            this.Icon = Properties.Resources.Mail_ru_Cloud_icon_icons_com_76713;
            //this.Width = Screen.PrimaryScreen.Bounds.Width - 200;
            //this.Height = Screen.PrimaryScreen.Bounds.Height - 200;
            //this.Location = new Point(Screen.PrimaryScreen.Bounds.Width/2 - this.Width/2, Screen.PrimaryScreen.Bounds.Height/2 - this.Height/2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            //unable txt
            txt_IPmes.Enabled = false;
            txt_lnomes.Enabled = false;
            txt_mcidmes.Enabled = false;
            txt_stdidmes.Enabled = false;
            txt_portmes.Enabled = false;
            //Initial setup          
            lbl_ver.Text = "Version 02";
            cbx_shift.Items.Add("Ca-A");
            cbx_shift.Items.Add("Ca-B");
            cbx_shift.Items.Add("Kíp-A");
            cbx_shift.Items.Add("Kíp-B");
            cbx_shift.Items.Add("Kíp-C");
            cbx_shift.Items.Add("Hành chính");
            //CLs Scanner
            Scn = new clsScanner(this);
            //Scanner Baudrate
            cbx_bRat.Items.Add("2400");
            cbx_bRat.Items.Add("4800");
            cbx_bRat.Items.Add("9600");
            cbx_bRat.Items.Add("19200");
            cbx_bRat.Items.Add("38400");
            cbx_bRat.Items.Add("57600");
            cbx_bRat.Items.Add("115200");
            //Scanner Data Bits
            cbx_dtBits.Items.Add("5");
            cbx_dtBits.Items.Add("6");
            cbx_dtBits.Items.Add("7");
            cbx_dtBits.Items.Add("8");
            //Scanner Parity
            cbx_parity.Items.Add("None");
            cbx_parity.Items.Add("Odd");
            cbx_parity.Items.Add("Even");
            cbx_parity.Items.Add("Mark");
            cbx_parity.Items.Add("Space");
            //Scanner Handshaking
            cbx_hdSkh.Items.Add("None");
            cbx_hdSkh.Items.Add("RTS");
            cbx_hdSkh.Items.Add("RTS+XOnXOff");
            cbx_hdSkh.Items.Add("XOnXOff");
            //COM available
            string[] ports = SerialPort.GetPortNames();
            cbx_COM.Items.AddRange(ports);
            rData.ReadConfigScn(cbx_COM, cbx_bRat, cbx_dtBits, cbx_parity, cbx_hdSkh);
            //Load model
            dgv_infModel.Columns.Clear();
            DataTable dtMol = dtb.GetData("Select * From Model Order by STT");
            ds.ShowModel(dgv_infModel, dtMol);
            //Database
            bool cnn = dtb.GetConnection();
            bool cnn2 = dtb.GetConnection2();
            if (cnn == true && cnn2 == true)
            {
                lbl_dtb.BackColor = Color.Green;
            }
            else
            {
                lbl_dtb.BackColor = Color.Red;
            }
            //thong so mes
            rData.ReadConfigMES(txt_IPmes, txt_lnomes, txt_mcidmes, txt_stdidmes, txt_portmes);
            //Mes-Auto   
            chb_rnStk.Checked = false;
            chb_ato.Checked = true;
            //Connect Mes
            Connect_MES();
            //Oracle connect
            ConntOracle();
            //Scanner
            if (chb_usbScn.Checked == true)
            {
                // Kết nối USB
            }
            else
            {
                ConnctScanner(cbx_COM);
            }
            //Folder history
            if (!System.IO.Directory.Exists(@Application.StartupPath + "\\Result"))
            {
                System.IO.Directory.CreateDirectory(@Application.StartupPath + "\\Result");
            } 
            //set model
            dgv_setup.Enabled = false;
            btn_addModel.Enabled = false;
            btn_delModel.Enabled = false;
            btn_savModel.Enabled = false;
            btn_lgin.Image = new Bitmap(@Application.StartupPath + "\\Picture\\lock.png");
            btn_lgin.SizeMode = PictureBoxSizeMode.StretchImage;
            ShowupHistoryNVL(); 
            dgv_setup.Columns.Clear();                     
            dtSetMol = dtb.GetData("Select * From Setup_Model Order by No DESC");
            dgv_setup.DataSource = dtSetMol;
            for (int j = 0; j < dgv_setup.ColumnCount; j++)
            {
                if(j == 0)
                {
                    dgv_setup.Columns[j].Width = 50;
                }
                else if (j == 1 || j == 2)
                {
                    dgv_setup.Columns[j].Width = 120;
                }
                else if(j >= 3 && j <= 8)
                {
                    dgv_setup.Columns[j].Width = 80;
                }
                else
                {
                    dgv_setup.Columns[j].Width = 120;
                }
            }
            
            Display_NhapNhay = new Thread(new ThreadStart(NhapNhayBTN));
            Display_NhapNhay.IsBackground = true;
            Display_NhapNhay.Start();            
        }        

        public class Stock
        {
            public string Barcode { set; get; }
            public string dateTime { set; get; }
            public string idCode { set; get; }
            public string namPer { set; get; }
        }

        public DataTable CreateTable(bool upl, bool del)
        {
            DataTable TableExcel = new DataTable();
            DataColumn column;

            column = new DataColumn();
            column.DataType = typeof(String);
            column.ColumnName = "STT";
            TableExcel.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(String);
            column.ColumnName = "Barcode";
            TableExcel.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(String);
            column.ColumnName = "DateTime";
            TableExcel.Columns.Add(column); 

            if(upl == true)
            {
                column = new DataColumn();
                column.DataType = typeof(String);
                column.ColumnName = "ID";
                TableExcel.Columns.Add(column);

                column = new DataColumn();
                column.DataType = typeof(String);
                column.ColumnName = "Name";
                TableExcel.Columns.Add(column); 
            }

            if(del == true)
            {
                column = new DataColumn();
                column.DataType = typeof(String);
                column.ColumnName = "ID";
                TableExcel.Columns.Add(column);

                column = new DataColumn();
                column.DataType = typeof(String);
                column.ColumnName = "Name";
                TableExcel.Columns.Add(column); 
            }

            return TableExcel;
        }

        public DataTable assignTable(DataTable TableExcel, string stt, string barcod, string dateTime, bool upl, bool del, string id, string namePer)
        {
            DataRow Row;

            Row = TableExcel.NewRow();
            Row["STT"] = stt;
            Row["Barcode"] = barcod;
            Row["DateTime"] = dateTime;

            if(upl == true)
            {
                Row["ID"] = id;
                Row["Name"] = namePer;
            }

            if(del == true)
            {
                Row["ID"] = id;
                Row["Name"] = namePer;
            }

            TableExcel.Rows.Add(Row);
            return TableExcel;
        }

        public void LoadStock()
        {
            dgv_stk.Columns.Clear();
            dtStk = new DataTable();
            var stks = new List<Stock>() { };
            StreamReader srStk = new StreamReader(@Application.StartupPath + "\\" + mol + "_Stock.txt");
            while(srStk.EndOfStream == false)
            {
                string[] arStr = srStk.ReadLine().Split('|');
                if(arStr.Length == 2 && arStr[0] != "Barcode")
                {
                    stks.Add(new Stock
                    {
                        Barcode = arStr[0],
                        dateTime = arStr[1]
                    });
                }                                    
            }
            srStk.Close();

            dtStk = CreateTable(false, false);
            //var strr = (from stk in stks select stk).OrderBy(n => n.dateTime).ToArray();
            var strr = stks.Select(n => new { n.Barcode, n.dateTime}).OrderBy(n => n.dateTime).ToArray();
            for(int i = 0; i < strr.Length; i++)
            {
                assignTable(dtStk, (i + 1).ToString(), strr[i].Barcode, strr[i].dateTime, false, false, "", "");               
            }
            dgv_stk.DataSource = dtStk;
            dgv_stk.Columns["STT"].ReadOnly = true;
            dgv_stk.Columns["Barcode"].ReadOnly = true;
            dgv_stk.Columns["DateTime"].ReadOnly = true;
            for (int j = 0; j < dgv_stk.ColumnCount; j++)
            {
                if(j == 0)
                {
                    dgv_stk.Columns[j].Width = 50;
                }
                else
                {
                    dgv_stk.Columns[j].Width = 200;
                }                
            }
        }

        public void LoadUpload()
        {
            dgv_upload.Columns.Clear();
            dtUpl = new DataTable();
            var stkUpl = new List<Stock>() { };
            StreamReader srUpl = new StreamReader(@Application.StartupPath + "\\History\\Upload\\UploadStock.txt");
            while(srUpl.EndOfStream == false)
            {
                string[] arrStr = srUpl.ReadLine().Split('|');
                if(arrStr.Length == 4)
                {
                    stkUpl.Add(new Stock
                    {
                        Barcode = arrStr[0],
                        dateTime = arrStr[1],
                        idCode = arrStr[2],
                        namPer = arrStr[3]
                    });
                }
            }
            srUpl.Close();

            dtUpl = CreateTable(true, false);
            var strr = stkUpl.Select(n => new { n.Barcode, n.dateTime, n.idCode, n.namPer }).OrderBy(n => n.dateTime).ToArray();
            for(int i = 0; i < strr.Length; i++)
            {
                assignTable(dtUpl, (i + 1).ToString(), strr[i].Barcode, strr[i].dateTime, true, false, strr[i].idCode, strr[i].namPer);
            }
            dgv_upload.DataSource = dtUpl;
            dgv_upload.Columns["STT"].ReadOnly = true;
            dgv_upload.Columns["Barcode"].ReadOnly = true;
            dgv_upload.Columns["DateTime"].ReadOnly = true;
            dgv_upload.Columns["ID"].ReadOnly = true;
            dgv_upload.Columns["Name"].ReadOnly = true;
            for (int j = 0; j < dgv_upload.ColumnCount; j++)
            {
                if (j == 0)
                {
                    dgv_upload.Columns[j].Width = 50;
                }
                else
                {
                    dgv_upload.Columns[j].Width = 150;
                }
            }
        }

        public void LoadDel()
        {
            dgv_del.Columns.Clear();
            dtDel = new DataTable();
            var stkDel = new List<Stock>() { };
            StreamReader srDel = new StreamReader(@Application.StartupPath + "\\History\\Delete\\DeleteStock.txt");
            while(srDel.EndOfStream == false)
            {
                string[] arStr = srDel.ReadLine().Split('|');
                if(arStr.Length == 4)
                {
                    stkDel.Add(new Stock
                    {
                        Barcode = arStr[0],
                        dateTime = arStr[1],
                        idCode = arStr[2],
                        namPer = arStr[3]
                    });
                }
            }
            srDel.Close();

            dtDel = CreateTable(false, true);
            var strr = stkDel.Select(n => new { n.Barcode, n.dateTime, n.idCode, n.namPer }).OrderBy(n => n.dateTime).ToArray();
            for(int i = 0; i < strr.Length; i++)
            {
                assignTable(dtDel, (i + 1).ToString(), strr[i].Barcode, strr[i].dateTime, false, true, strr[i].idCode, strr[i].namPer);               
            }
            dgv_del.DataSource = dtDel;
            dgv_del.Columns["STT"].ReadOnly = true;
            dgv_del.Columns["Barcode"].ReadOnly = true;
            dgv_del.Columns["DateTime"].ReadOnly = true;
            dgv_del.Columns["ID"].ReadOnly = true;
            dgv_del.Columns["Name"].ReadOnly = true;
            for (int j = 0; j < dgv_del.ColumnCount; j++)
            {
                if (j == 0)
                {
                    dgv_del.Columns[j].Width = 50;
                }
                else
                {
                    dgv_del.Columns[j].Width = 150;
                }
            }
        }

        public void Connect_MES()
        {
            socket = new clsSocket(this);
            socket.Ip = txt_IPmes.Text;
            socket.Lineno = txt_lnomes.Text;
            socket.Mcid = txt_mcidmes.Text;
            socket.Stnid1 = txt_stdidmes.Text;
            socket.Stnid2 = "";
            socket.Stnid3 = "";
            socket.Port = int.Parse(txt_portmes.Text);
            socket.Portprocess = "001";
            socket.Workerid = "20603021";
            socket.start(txt_IPmes.Text);
        }

        public void ConntOracle()
        {
            //MES.Dbsource = "VNMES";
            //MES.User = "vncmesadm";
            //MES.Pass = "thgkr007~";

            StreamReader srOracle = new StreamReader(@Application.StartupPath + "\\Data\\OracleSetting.ini");
            while(srOracle.EndOfStream == false)
            {
                string str = srOracle.ReadLine();
                string[] arrstr = str.Split('=');
                switch(arrstr[0])
                {
                    case "Dbsource":
                        ms.Dbsource = arrstr[1];
                        break;
                    case "User":
                        ms.User = arrstr[1];
                        break;
                    case "Pass":
                        ms.Pass = arrstr[1];
                        break;
                    case "LineNo":
                        lineNo = arrstr[1];
                        break;
                    default:
                        break;
                }
            }
            srOracle.Close();

            if (ms.checkconnection())
            {
                lbl_oracle.BackColor = Color.Green;
                //Tải tên line vào combobox
                DataSet dsLine = new DataSet();
                dsLine = ms.LoadLineNo();//show linename in MIH
                foreach (DataRow myrow in dsLine.Tables["tbllinename"].Rows)
                {
                    if (myrow.ItemArray[0].ToString().Contains(lineNo))
                    {
                        txt_lineNo.Text = myrow.ItemArray[0].ToString();
                    }                   
                }
                Oracle_Connect = true;
            }
            else
            {
                lbl_oracle.BackColor = Color.Red;
                Oracle_Connect = false;
            }
        }       

        public void ConnctScanner(ComboBox cbx)
        {
            if(btn_offCOM.Text == "Ngắt kết nối")
            {
                Scn.COMnum = cbx.Text;
                if (Scn.CntScanner(cbx_bRat.Text, cbx_dtBits.Text, cbx_parity.Text, cbx_hdSkh.Text) == true)
                {
                    btn_offCOM.BackColor = Color.Lime;
                    btn_offCOM.Text = "Kết nối";
                    lbl_scn.BackColor = Color.Green;
                }  
                else
                {
                    Scn.DisCnt();
                    btn_offCOM.BackColor = Color.Red;
                    btn_offCOM.Text = "Ngắt kết nối";
                    lbl_scn.BackColor = Color.Red; 
                }
            }
            else
            {
                Scn.DisCnt();
                btn_offCOM.BackColor = Color.Red;
                btn_offCOM.Text = "Ngắt kết nối";
                lbl_scn.BackColor = Color.Red;                   
            }           
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            
        }

        private void btn_reset_MouseHover(object sender, EventArgs e)
        {
            btn_reset.BackColor = Color.YellowGreen;
        }

        private void btn_reset_MouseLeave(object sender, EventArgs e)
        {
            btn_reset.BackColor = Color.FromArgb(255, 192, 128);
        }        

        private void btn_load_MouseHover(object sender, EventArgs e)
        {
            btn_load.BackColor = Color.YellowGreen;
        }

        private void btn_load_MouseLeave(object sender, EventArgs e)
        {
            btn_load.BackColor = Color.FromArgb(255, 255, 128);
        }               

        private void btn_offCOM_MouseHover(object sender, EventArgs e)
        {
            btn_offCOM.BackColor = Color.Yellow;
        }

        private void btn_offCOM_MouseLeave(object sender, EventArgs e)
        {
            if (btn_offCOM.Text == "Ngắt kết nối")
            {
                if (chb_usbScn.Checked == true)
                {
                    btn_offCOM.BackColor = Color.White;
                }
                else
                {
                    btn_offCOM.BackColor = Color.Red;
                }              
            }
            else
            {
                if (chb_usbScn.Checked == true)
                {
                    btn_offCOM.BackColor = Color.White;
                }
                else
                {
                    btn_offCOM.BackColor = Color.Lime;
                }                
            }
        }             

        private void cbx_shift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbx_shift.Text))
            {
                cbx_shift.BackColor = Color.Red;
            }
            else
            {
                cbx_shift.BackColor = Color.Green;
            }
        }

        private void txt_modelCode_TextChanged(object sender, EventArgs e)
        {            
            if (string.IsNullOrEmpty(txt_modelName.Text))
            {
                txt_modelName.BackColor = Color.Red;
            }
            else
            {
                txt_modelName.BackColor = Color.Green;
            }
        }

        public DateTime dt = DateTime.Now;
        public DateTime startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0);
        public DateTime endDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, 12, 0, 0);
        private void btn_load_Click(object sender, EventArgs e)
        {       
            if(txt_qtyPO.Text != "")
            {
                if (chb_rnStk.Checked == false)
                {
                    if((DateTime.Compare(startDT, dt) < 0) && (DateTime.Compare(dt, endDT) < 0) && (dShift == "Đêm"))
                    {
                        dt = dateTimePickerDaySelect.Value.AddDays(1);
                    }
                    
                    btn_load.BackColor = Color.FromArgb(255, 255, 128);
                    //lay ky tu code model, model name
                    LoadPoRun(txt_modelName, dgvOracleData);
                    txt_charModelCode.Text = dtb.GetMolCod(lbl_modelCode.Text);
                    //assign model
                    if (lbl_modelCode.Text.Contains("P01P-00226A"))
                    {
                        mol = "Main";                        
                        pl_Main.Visible = true;
                        pl_Main.Enabled = true;
                        pl_Main.BringToFront();
                        lbl_Main.Text = txt_modelName.Text;
                        picBox_Main.Image = new Bitmap(@Application.StartupPath + "\\Picture\\Main.png");
                        picBox_Main.SizeMode = PictureBoxSizeMode.StretchImage;
                        txt1_CodeMain.Text = "";
                        txt1_MesMain.Text = "";
                        txt2_CodeMain.Text = "";
                        txt2_MesMain.Text = "";
                        pl_Cell.Visible = false;
                        pl_Cell.Enabled = false;
                        pl_Sub.Visible = false;
                        pl_Sub.Enabled = false;
                        //load config Scanner
                        Scn.LoadConfig(2);
                    }
                    else if (lbl_modelCode.Text.Contains("P01P-00225A"))
                    {
                        mol = "Cell";
                        pl_Cell.Visible = true;
                        pl_Cell.Enabled = true;
                        pl_Cell.BringToFront();
                        lbl_Cell.Text = txt_modelName.Text;
                        picBox_Cell.Image = new Bitmap(@Application.StartupPath + "\\Picture\\Cell.png");
                        picBox_Cell.SizeMode = PictureBoxSizeMode.StretchImage;
                        txt1_CodeCell.Text = "";
                        txt1_MesCell.Text = "";
                        txt2_CodeCell.Text = "";
                        txt2_MesCell.Text = "";
                        txt3_CodeCell.Text = "";
                        txt3_MesCell.Text = "";
                        pl_Main.Visible = false;
                        pl_Main.Enabled = false;
                        pl_Sub.Visible = false;
                        pl_Sub.Enabled = false;
                        //load config Scanner
                        Scn.LoadConfig(3);
                    }
                    else if (lbl_modelCode.Text.Contains("P01P-00227A"))
                    {
                        mol = "Sub";
                        pl_Sub.Visible = true;
                        pl_Sub.Enabled = true;
                        pl_Sub.BringToFront();
                        lbl_Sub.Text = txt_modelName.Text;
                        picBox_Sub.Image = new Bitmap(@Application.StartupPath + "\\Picture\\Sub.png");
                        picBox_Sub.SizeMode = PictureBoxSizeMode.StretchImage;
                        txt1_CodeSub.Text = "";
                        txt1_MesSub.Text = "";
                        txt2_CodeSub.Text = "";
                        txt2_MesSub.Text = "";
                        txt3_CodeSub.Text = "";
                        txt3_MesSub.Text = "";
                        txt4_CodeSub.Text = "";
                        txt4_MesSub.Text = "";
                        pl_Main.Visible = false;
                        pl_Main.Enabled = false;
                        pl_Cell.Visible = false;
                        pl_Cell.Enabled = false;
                        //load config Scanner
                        Scn.LoadConfig(4);
                    }
                    //Load setup barcode
                    LoadBarodeSetup(mol);
                    //Load Stock
                    LoadStock();
                    //Folder history
                    if (!System.IO.Directory.Exists(@Application.StartupPath + "\\Result\\" + txt_modelName.Text))
                    {
                        System.IO.Directory.CreateDirectory(@Application.StartupPath + "\\Result\\" + txt_modelName.Text);
                    }
                    txt_qtyPO.Enabled = false;
                }     
            }
            else
            {
                MessageBox.Show("Bạn chưa điền sản lượng của PO ca/kíp đang chạy!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_qtyPO.Focus();
            }                   
        }

        public void LoadPoRun(TextBox txtMol, DataGridView dgvData)
        {
            txtMol.Text = "";
            if (Oracle_Connect == true)
            {
                try
                {
                    //Datetime
                    ms.SelectTime = dt.ToString("yyyyMMdd");
                    //Line CODE
                    ms.SelectLineCode = txt_lineNo.Text;
                    //Tai tat ca cac thong tin tu MES/CMES vao dgv
                    DataSet ds = new DataSet();
                    ds = ms.LoadData();
                    dgvOracleData.Columns.Clear();
                    dgvOracleData.DataSource = ds.Tables["tblallinfo"];

                    for (int i = 0; i < dgvData.RowCount - 1; i++)
                    {
                        if (dgvData.Rows[i].Cells["PO_STATUS"].Value.ToString() == "In Use") // PO đang sử dụng
                        {
                            txtMol.Text = dgvData.Rows[i].Cells["MODEL_NAME"].Value.ToString();
                            lbl_modelCode.Text = dgvData.Rows[i].Cells["PRODUCT_CODE"].Value.ToString();
                            i = dgvData.RowCount + 1;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Tải thất bại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Chưa kết nối Oracle!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //private void btn_enter_Click(object sender, EventArgs e)
        //{
        //    btn_enter.FlatStyle = FlatStyle.Standard;
        //    Thread.Sleep(1000);

        //    if (cbx_shift.Text == "" || (txt_modelName.Text == "" && cbx_modelName.Text == "")
        //            || (chb_ato.Checked == false && chb_mal.Checked == false && chb_rnStk.Checked == false))
        //    {
        //        MessageBox.Show("Bạn điền thiếu thông tin!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    else
        //    {
        //        if (chb_mal.Checked == true && chb_ato.Checked == false && chb_rnStk.Checked == false)
        //        {
        //            switch (mol)
        //            {
        //                case "Main":
        //                foreach (var textBox in pl_Main.Controls.OfType<TextBox>())
        //                {
        //                    if (textBox.Name.Contains("Code"))
        //                    {
        //                        //upload Mes
        //                        UploadMes(chb_mal, textBox.Text);
        //                        //Save code
        //                        rData.SaveCode(textBox.Text, mol);
        //                    }
        //                }
        //                break;
        //                case "Cell":
        //                foreach (var textBox in pl_Cell.Controls.OfType<TextBox>())
        //                {
        //                    if (textBox.Name.Contains("Code"))
        //                    {
        //                        //upload Mes
        //                        UploadMes(chb_mal, textBox.Text);
        //                        //Save code
        //                        rData.SaveCode(textBox.Text, mol);
        //                    }
        //                }
        //                break;
        //                case "Sub":
        //                foreach (var textBox in pl_Sub.Controls.OfType<TextBox>())
        //                {
        //                    if (textBox.Name.Contains("Code"))
        //                    {
        //                        //upload Mes
        //                        UploadMes(chb_mal, textBox.Text);
        //                        //Save code
        //                        rData.SaveCode(textBox.Text, mol);
        //                    }
        //                }
        //                break;
        //                default:
        //                break;
        //            }
        //        }
        //    }                                       
        //}

        //private void picB_del_Click(object sender, EventArgs e)
        //{
        //    picB_del.BorderStyle = BorderStyle.Fixed3D;
        //    if (lbl_modelCode.Text.Contains("P01P-00226A"))
        //    {
        //        txt1_CodeMain.Text = "";
        //        txt1_MesMain.Text = "";
        //        txt2_CodeMain.Text = "";
        //        txt2_MesMain.Text = "";
        //    }
        //    else if (lbl_modelCode.Text.Contains("P01P-00225A"))
        //    {
        //        txt1_CodeCell.Text = "";
        //        txt1_MesCell.Text = "";
        //        txt2_CodeCell.Text = "";
        //        txt2_MesCell.Text = "";
        //        txt3_CodeCell.Text = "";
        //        txt3_MesCell.Text = "";
        //    }
        //    else if (lbl_modelCode.Text.Contains("P01P-00227A"))
        //    {
        //        txt1_CodeSub.Text = "";
        //        txt1_MesSub.Text = "";
        //        txt2_CodeSub.Text = "";
        //        txt2_MesSub.Text = "";
        //        txt3_CodeSub.Text = "";
        //        txt3_MesSub.Text = "";
        //        txt4_CodeSub.Text = "";
        //        txt4_MesSub.Text = "";
        //    }
        //}

        private void btn_reset_Click(object sender, EventArgs e)
        {
            btn_reset.BackColor = Color.FromArgb(255, 192, 128);
            DialogResult rel = MessageBox.Show("Reset số lượng?", "Input Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(rel == DialogResult.Yes)
            {
                txt_qty.Text = "0";
                FileStream fs = new FileStream(@Application.StartupPath + "\\QtyCurrent.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(txt_qty.Text);
                sw.Close();
                fs.Close();
            }
        }

        //private async void txt_input_TextChanged(object sender, EventArgs e)
        //{
            //await Task.Delay(1000);

            //if (txt_inputAuto.Text == "" || txt_inputAuto.Text == null)
            //{
            //    return;
            //}
            //else //(txt_inputAuto.Text.Length == 14 && txt_inputAuto.Text != "")
            //{
            //    if (cbx_shift.Text == "" || (txt_modelName.Text == "" && cbx_modelName.Text == "")
            //        || (chb_ato.Checked == false && chb_mal.Checked == false && chb_rnStk.Checked == false))
            //    {
            //        txt_inputAuto.ResetText();
            //        MessageBox.Show("Bạn điền thiếu thông tin!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //    else
            //    {
            //        if (rData.CheckFormatCode(txt_inputAuto.Text, txt_charModelCode.Text, startSerial, lenSerial, startModCode, lenModCode, minSerial, maxSerial) == false)
            //        {
            //            txt_inputAuto.ResetText();
            //            MessageBox.Show("Sai format code đã input. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            txt_inputAuto.Focus();//Trỏ chuột tại textBox Input
            //        }
            //        else if (rData.ChekdoubleCode(txt_inputAuto.Text) == false)
            //        {
            //            txt_inputAuto.ResetText();
            //            MessageBox.Show("Trùng code đã input. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            txt_inputAuto.Focus();//Trỏ chuột tại textBox Input
            //        }
            //        else
            //        {
            //            if (chb_ato.Checked == true && chb_mal.Checked == false && chb_rnStk.Checked == false)//Chạy MP
            //            {
            //                if (dgv_stk.Rows.Count > 1)
            //                {
            //                    string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
            //                    DataRow dtrw = dtStk.NewRow();
            //                    if (dgv_stk.Rows.Count == 1)
            //                    {
            //                        dtrw["STT"] = "1";
            //                    }
            //                    else
            //                    {
            //                        dtrw["STT"] = (int.Parse(dgv_stk.Rows[dgv_stk.Rows.Count - 2].Cells["STT"].Value.ToString()) + 1).ToString();
            //                    }
            //                    dtrw["Barcode"] = txt_inputAuto.Text;
            //                    dtrw["DateTime"] = dtime;
            //                    dtStk.Rows.Add(dtrw);
            //                    dtStk.AcceptChanges();

            //                    //upload Mes
            //                    if (rData.CheckFormatCode(dgv_stk.Rows[0].Cells["Barcode"].Value.ToString(), txt_charModelCode.Text, startSerial, lenSerial, startModCode, lenModCode, minSerial, maxSerial) == false)
            //                    {
            //                        MessageBox.Show("Sai format code. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                    }
            //                    else
            //                    {
            //                        UploadMes(chb_ato, dgv_stk.Rows[0].Cells["Barcode"].Value.ToString());
            //                    }                                
            //                }
            //                else
            //                {
            //                    //upload Mes
            //                    UploadMes(chb_ato, txt_inputAuto.Text);                             
            //                }
                           
            //                rData.SaveCode(txt_inputAuto.Text);

            //                //Reset 
            //                txt_inputAuto.Text = "";
            //                txt_inputAuto.Focus();
            //            }

            //            if (chb_ato.Checked == false && chb_mal.Checked == false && chb_rnStk.Checked == true)//Chạy stock
            //            {
            //                string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
            //                DataRow dtrw = dtStk.NewRow();
            //                if (dgv_stk.Rows.Count == 1)
            //                {
            //                    dtrw["STT"] = "1";
            //                }
            //                else
            //                {
            //                    dtrw["STT"] = (int.Parse(dgv_stk.Rows[dgv_stk.Rows.Count - 2].Cells["STT"].Value.ToString()) + 1).ToString();
            //                }
            //                dtrw["Barcode"] = txt_inputAuto.Text;
            //                dtrw["DateTime"] = dtime;
            //                dtStk.Rows.Add(dtrw);
            //                dtStk.AcceptChanges();

            //                rData.SaveCode(txt_inputAuto.Text);
            //                ex.SaveLog(txt_inputAuto.Text, cbx_modelName.Text, "OffMes");

            //                int sQty = new int();
            //                sQty = int.Parse(txt_qty.Text);
            //                txt_qty.Text = (sQty + 1).ToString();

            //                //Lưu stock
            //                if (dgv_stk.Rows.Count > 1)
            //                {
            //                    FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + mol + "_Stock.txt", FileMode.Create);
            //                    StreamWriter sw_stk = new StreamWriter(fs_stk);
            //                    sw_stk.WriteLine(dgv_stk.Columns[1].HeaderText + "|" + dgv_stk.Columns[2].HeaderText);
            //                    for (int i = 0; i < dgv_stk.Rows.Count - 1; i++)
            //                    {
            //                        if (string.IsNullOrEmpty(dgv_stk.Rows[i].Cells["STT"].Value.ToString()) == false)
            //                        {
            //                            sw_stk.WriteLine(dgv_stk.Rows[i].Cells["Barcode"].Value.ToString() + "|" + dgv_stk.Rows[i].Cells["DateTime"].Value.ToString());
            //                        }
            //                    }
            //                    sw_stk.Close();
            //                    fs_stk.Close();
            //                }

            //                //Reset 
            //                txt_inputAuto.Text = "";
            //                txt_inputAuto.Focus();
            //            }                        
            //        }                   
            //    }
            //}           
        //}

        public void UploadMes(CheckBox chb, string code)
        {
            if(MES_Connecting == "CAN" && chb.Checked == true)
            {
                if (rData.ChekdoubleCode(code, mol) == false)
                {
                    MessageBox.Show("Trùng code đã input. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    socket.Sent_Input_Gen2(code, lstBx_PcMes);
                    Thread.Sleep(500);
                }                     
            }
        }                       

        private void chb_nouse_CheckedChanged(object sender, EventArgs e)
        {
            if(chb_nouse.Checked == true)
            {
                //Ẩn Mes
                chb_rnStk.Checked = true;
            }
            else
            {
                //Hiện Mes
                chb_rnStk.Checked = false;
            }
        }

        private void timer_MES_Tick(object sender, EventArgs e)
        {
            //Quét MES connect
            if (MES_Connecting == "CAN")
            {
                lbl_mesCon.BackColor = Color.Green;
            }
            else
            {
                lbl_mesCon.BackColor = Color.Red;
            }                
        }        

        private void btn_offCOM_Click(object sender, EventArgs e)
        {
            ConnctScanner(cbx_COM);
        }        

        private void chb_usbScn_CheckedChanged(object sender, EventArgs e)
        {
            if(chb_usbScn.Checked == true)
            {
                Scn.DisCnt();
                btn_offCOM.BackColor = Color.Gray;
                btn_offCOM.Text = "Ngắt kết nối";
                lbl_scn.BackColor = Color.Gray;
            }
            else
            {
                ConnctScanner(cbx_COM);
            }
        }

        private void cbx_modelNam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txt_qtyPO.Text != "")
            {
                if (string.IsNullOrEmpty(cbx_modelName.Text))
                {
                    cbx_modelName.BackColor = Color.Red;
                }
                else
                {
                    cbx_modelName.BackColor = Color.Green;
                    string[] arrStr = dtb.GetModelName(cbx_modelName.Text);
                    lbl_modelCode.Text = arrStr[0];
                    txt_charModelCode.Text = arrStr[1];
                    //assign model
                    if (lbl_modelCode.Text.Contains("P01P-00226A"))
                    {
                        mol = "Main";
                        pl_Main.Visible = true;
                        pl_Main.Enabled = true;
                        pl_Main.BringToFront();
                        lbl_Main.Text = cbx_modelName.Text;
                        picBox_Main.Image = new Bitmap(@Application.StartupPath + "\\Picture\\Main.png");
                        picBox_Main.SizeMode = PictureBoxSizeMode.StretchImage;
                        txt1_CodeMain.Text = "";
                        txt1_MesMain.Text = "";
                        txt2_CodeMain.Text = "";
                        txt2_MesMain.Text = "";
                        pl_Cell.Visible = false;
                        pl_Cell.Enabled = false;
                        pl_Sub.Visible = false;
                        pl_Sub.Enabled = false;
                        //load config Scanner
                        Scn.LoadConfig(2);
                    }
                    else if (lbl_modelCode.Text.Contains("P01P-00225A"))
                    {
                        mol = "Cell";
                        pl_Cell.Visible = true;
                        pl_Cell.Enabled = true;
                        pl_Cell.BringToFront();
                        lbl_Cell.Text = cbx_modelName.Text;
                        picBox_Cell.Image = new Bitmap(@Application.StartupPath + "\\Picture\\Cell.png");
                        picBox_Cell.SizeMode = PictureBoxSizeMode.StretchImage;
                        txt1_CodeCell.Text = "";
                        txt1_MesCell.Text = "";
                        txt2_CodeCell.Text = "";
                        txt2_MesCell.Text = "";
                        txt3_CodeCell.Text = "";
                        txt3_MesCell.Text = "";
                        pl_Main.Visible = false;
                        pl_Main.Enabled = false;
                        pl_Sub.Visible = false;
                        pl_Sub.Enabled = false;
                        //load config Scanner
                        Scn.LoadConfig(3);
                    }
                    else if (lbl_modelCode.Text.Contains("P01P-00227A"))
                    {
                        mol = "Sub";
                        pl_Sub.Visible = true;
                        pl_Sub.Enabled = true;
                        pl_Sub.BringToFront();
                        lbl_Sub.Text = cbx_modelName.Text;
                        picBox_Sub.Image = new Bitmap(@Application.StartupPath + "\\Picture\\Sub.png");
                        picBox_Sub.SizeMode = PictureBoxSizeMode.StretchImage;
                        txt1_CodeSub.Text = "";
                        txt1_MesSub.Text = "";
                        txt2_CodeSub.Text = "";
                        txt2_MesSub.Text = "";
                        txt3_CodeSub.Text = "";
                        txt3_MesSub.Text = "";
                        txt4_CodeSub.Text = "";
                        txt4_MesSub.Text = "";
                        pl_Main.Visible = false;
                        pl_Main.Enabled = false;
                        pl_Cell.Visible = false;
                        pl_Cell.Enabled = false;
                        //load config Scanner
                        Scn.LoadConfig(4);
                    }
                    //Load setup barcode
                    LoadBarodeSetup(mol);
                    //Load Stock
                    LoadStock();
                    //Folder history
                    if (!System.IO.Directory.Exists(@Application.StartupPath + "\\Result\\" + cbx_modelName.Text))
                    {
                        System.IO.Directory.CreateDirectory(@Application.StartupPath + "\\Result\\" + cbx_modelName.Text);
                    }
                    txt_qtyPO.Enabled = false;
                }
            }  
            else
            {
                cbx_modelName.SelectedIndex = -1;
                MessageBox.Show("Bạn chưa điền sản lượng của PO ca/kíp đang chạy!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_qtyPO.Focus();                
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileStream fs = new FileStream(@Application.StartupPath + "\\QtyCurrent.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(txt_qty.Text);
            sw.Close();
            fs.Close();            
        }

        private void cbx_dayCod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbx_dayCod.Text == cbx_dayCod.Items[0].ToString())
            {
                txt_dayCod.Text = "";
            }
            else
            {
                txt_dayCod.Text = cbx_dayCod.Items[1].ToString();
            }
        }

        public bool clickButn = false;
        public int qtyUpld = 0;
        public string namInp = string.Empty, idInp = string.Empty;
        private void btn_dayCod_Click(object sender, EventArgs e)
        {
            if ((dtb.GetRight(txt_idsubLder.Text) == txt_subLder.Text) && txt_idsubLder.Text != "" && txt_subLder.Text != "")
            {
                namInp = txt_subLder.Text;
                idInp = txt_idsubLder.Text;
                //check so luong
                bool chk = false;                
                if(txt_dayCod.Text == "")
                {
                    qtyUpld = 1;
                    chk = true;
                }
                else if (txt_dayCod.Text == cbx_dayCod.Items[1].ToString())
                {
                    qtyUpld = dgv_stk.RowCount - 1;
                    chk = true;
                }
                else
                {
                    chk = int.TryParse(txt_dayCod.Text, out qtyUpld);
                }
                //upload code MES
                if(chk == true)
                {
                    if ((txt_modelName.Text != "" || cbx_modelName.Text != "") && txt_charModelCode.Text != "")
                    {
                        if(dgv_stk.Rows.Count > 1)
                        {
                            if (qtyUpld <= dgv_stk.Rows.Count - 1)
                            {
                                timer_retStk.Start();
                                clickButn = true;
                                upStkMes = new Thread(new ThreadStart(upStk));
                                upStkMes.IsBackground = true;
                                upStkMes.Start();                               
                                txt_subLder.Text = "";
                                txt_idsubLder.Text = "";
                                txt_dayCod.Text = "";
                            }
                            else
                            {
                                DialogResult rel = MessageBox.Show("Số lượng bạn nhập > số lượng code đang có stock!\nBạn muốn đẩy hết code không?", "Input Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (rel == DialogResult.Yes)
                                {
                                    timer_retStk.Start();
                                    clickButn = true;
                                    qtyUpld = dgv_stk.RowCount - 1;
                                    upStkMes = new Thread(new ThreadStart(upStk));
                                    upStkMes.IsBackground = true;
                                    upStkMes.Start();   
                                    txt_subLder.Text = "";
                                    txt_idsubLder.Text = "";
                                    txt_dayCod.Text = "";
                                }
                                else
                                {
                                    txt_dayCod.Text = "";
                                    MessageBox.Show("Nhập lại số lượng code muốn đẩy!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Stock empty!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }                       
                    }
                     else
                    {
                        MessageBox.Show("Chưa có Model Code!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Điền lại số lượng!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Mã nhân viên Sub/Leader sai hoặc trống!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void upStk()
        {
            UploadStock(qtyUpld);
        }

        public bool sentSucc = false, sentFail = false;
        public void UploadStock(int dong)
        {
            if (chb_rnStk.Checked == false && chb_ato.Checked == true && clickButn == true)
            {
                int j = 0;
                for (; j < dong; )
                {
                    if (sentSucc == false)
                    {
                        sentSucc = true;
                        if (rData.CheckFormatCode(dgv_stk.Rows[j].Cells["Barcode"].Value.ToString(), txt_charModelCode.Text, startSerial, lenSerial, startModCode, lenModCode, minSerial, maxSerial) == false)
                        {
                            MessageBox.Show("Sai format code. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            for (int i = 0; i < dgv_stk.Rows.Count - 2; i++)
                            {
                                if (dgv_stk.Rows[i].Cells["Barcode"].Style.BackColor == Color.Green)
                                {
                                    int indRow = dgv_stk.Rows[i].Index;
                                    DataRow drToDelete = dtStk.Rows[indRow];
                                    dtStk.Rows.Remove(drToDelete);
                                    i--;
                                }
                            }
                            if (dgv_stk.Rows.Count > 1)
                            {
                                FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + mol + "_Stock.txt", FileMode.Create);
                                StreamWriter sw_stk = new StreamWriter(fs_stk);
                                sw_stk.WriteLine(dgv_stk.Columns[1].HeaderText + "|" + dgv_stk.Columns[2].HeaderText);
                                for (int m = 0; m < dgv_stk.Rows.Count - 1; m++)
                                {
                                    if (string.IsNullOrEmpty(dgv_stk.Rows[m].Cells["STT"].Value.ToString()) == false)
                                    {
                                        sw_stk.WriteLine(dgv_stk.Rows[m].Cells["Barcode"].Value.ToString() + "|" + dgv_stk.Rows[m].Cells["DateTime"].Value.ToString());
                                    }
                                }
                                sw_stk.Close();
                                fs_stk.Close();
                            }
                            break;
                        }
                        else
                        {
                            if (MES_Connecting == "CAN" && chb_ato.Checked == true)
                            {
                                socket.Sent_Input_Gen2(dgv_stk.Rows[j].Cells["Barcode"].Value.ToString(), lstBx_PcMes);
                                dgv_stk.Rows[j].Cells["Barcode"].Style.BackColor = Color.Green;
                                dgv_stk.FirstDisplayedScrollingRowIndex = dgv_stk.Rows[j].Index;
                                count_Stk = count_Stk + j + 1;
                                j++;
                                Thread.Sleep(200);
                            }
                        }
                    }

                    if (sentFail == true)
                    {
                        sentFail = false;
                        for (int i = 0; i < dgv_stk.Rows.Count - 2; i++)
                        {
                            if (dgv_stk.Rows[i].Cells["Barcode"].Style.BackColor == Color.Green)
                            {
                                int indRow = dgv_stk.Rows[i].Index;
                                DataRow drToDelete = dtStk.Rows[indRow];
                                dtStk.Rows.Remove(drToDelete);
                                i--;
                            }
                        }
                        if (dgv_stk.Rows.Count > 1)
                        {
                            FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + mol + "_Stock.txt", FileMode.Create);
                            StreamWriter sw_stk = new StreamWriter(fs_stk);
                            sw_stk.WriteLine(dgv_stk.Columns[1].HeaderText + "|" + dgv_stk.Columns[2].HeaderText);
                            for (int m = 0; m < dgv_stk.Rows.Count - 1; m++)
                            {
                                if (string.IsNullOrEmpty(dgv_stk.Rows[m].Cells["STT"].Value.ToString()) == false)
                                {
                                    sw_stk.WriteLine(dgv_stk.Rows[m].Cells["Barcode"].Value.ToString() + "|" + dgv_stk.Rows[m].Cells["DateTime"].Value.ToString());
                                }
                            }
                            sw_stk.Close();
                            fs_stk.Close();
                        }
                        break;
                    }

                    if (j == dong)
                    {
                        for (int i = 0; i < dgv_stk.Rows.Count - 2; i++)
                        {
                            if (dgv_stk.Rows[i].Cells["Barcode"].Style.BackColor == Color.Green)
                            {
                                int indRow = dgv_stk.Rows[i].Index;
                                DataRow drToDelete = dtStk.Rows[indRow];
                                dtStk.Rows.Remove(drToDelete);
                                i--;
                            }
                        }
                        clickButn = false;
                        if (dgv_stk.Rows.Count > 1)
                        {
                            FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + mol + "_Stock.txt", FileMode.Create);
                            StreamWriter sw_stk = new StreamWriter(fs_stk);
                            sw_stk.WriteLine(dgv_stk.Columns[1].HeaderText + "|" + dgv_stk.Columns[2].HeaderText);
                            for (int m = 0; m < dgv_stk.Rows.Count - 1; m++)
                            {
                                if (string.IsNullOrEmpty(dgv_stk.Rows[m].Cells["STT"].Value.ToString()) == false)
                                {
                                    sw_stk.WriteLine(dgv_stk.Rows[m].Cells["Barcode"].Value.ToString() + "|" + dgv_stk.Rows[m].Cells["DateTime"].Value.ToString());
                                }
                            }
                            sw_stk.Close();
                            fs_stk.Close();
                        }
                    }
                    lbl_qtyBuStk.Text = count_Stk.ToString();
                }
            }                                                                                              
        }

        private void btn_dayCod_MouseHover(object sender, EventArgs e)
        {
            btn_dayCod.BackColor = Color.YellowGreen;
        }

        private void btn_dayCod_MouseLeave(object sender, EventArgs e)
        {
            btn_dayCod.BackColor = Color.FromArgb(255, 255, 128);
        }                               

        private void btn_delCod_Click(object sender, EventArgs e)
        {
            bool del = true;
            int deled = 0;
            if (dgv_stk.CurrentRow.Index >= 0)
            {
                if ((dtb.GetRight(txt_manv.Text) == txt_hoten.Text) && txt_manv.Text != "" && txt_hoten.Text != "")
                {
                    bool chk = true;
                    if (txt_dayCod.Text == "")
                    {
                        qtyUpld = 1;
                        chk = true;
                    }
                    else if (txt_dayCod.Text == cbx_dayCod.Items[1].ToString())
                    {
                        qtyUpld = dgv_stk.RowCount - 1;
                        chk = true;
                    }
                    else
                    {
                        chk = int.TryParse(txt_dayCod.Text, out qtyUpld);
                    }
                    if (chk == true)
                    {
                        if (qtyUpld < dgv_stk.RowCount - 1)
                        {
                            for (int i = 0; i < qtyUpld; i++)
                            {
                                int indRow = 0;
                                if (del == false)
                                    indRow = dgv_stk.CurrentRow.Index + i;
                                if (del == true)
                                    indRow = dgv_stk.CurrentRow.Index;
                                if (deled > 0 && del == false)
                                    indRow = dgv_stk.CurrentRow.Index + 1;
                                dgv_stk.Rows[indRow].Cells["Barcode"].Style.BackColor = Color.Yellow;
                                dgv_stk.FirstDisplayedScrollingRowIndex = dgv_stk.Rows[indRow].Index;
                                DialogResult rel = MessageBox.Show("Bạn muốn xóa code : " + dgv_stk.Rows[indRow].Cells["Barcode"].Value.ToString() + " ?", "Input Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (rel == DialogResult.Yes)
                                {
                                    del = true;
                                    deled++;
                                    //lưu lich su
                                    rData.SaveData(@Application.StartupPath + "\\History\\Delete\\DeleteStock.txt",
                                                   dgv_stk.Rows[indRow].Cells["Barcode"].Value.ToString() + "|" +
                                                   DateTime.Now.ToString("yyyyMMddHHmmss") + "|" +
                                                   txt_manv.Text + "|" +
                                                   txt_hoten.Text);
                                    //thao tac                       
                                    DataRow drToDelete = dtStk.Rows[indRow];
                                    dtStk.Rows.Remove(drToDelete);
                                    //luu stock
                                    if (dgv_stk.Rows.Count > 1)
                                    {
                                        FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + mol + "_Stock.txt", FileMode.Create);
                                        StreamWriter sw_stk = new StreamWriter(fs_stk);
                                        sw_stk.WriteLine(dgv_stk.Columns[1].HeaderText + "|" + dgv_stk.Columns[2].HeaderText);
                                        for (int m = 0; m < dgv_stk.Rows.Count - 1; m++)
                                        {
                                            if (string.IsNullOrEmpty(dgv_stk.Rows[m].Cells["STT"].Value.ToString()) == false)
                                            {
                                                sw_stk.WriteLine(dgv_stk.Rows[m].Cells["Barcode"].Value.ToString() + "|" + dgv_stk.Rows[m].Cells["DateTime"].Value.ToString());
                                            }
                                        }
                                        sw_stk.Close();
                                        fs_stk.Close();
                                    }
                                }
                                else
                                {
                                    del = false;
                                    dgv_stk.Rows[indRow].Cells["Barcode"].Style.BackColor = Color.White;
                                }
                            }
                        }
                        else
                        {
                            DialogResult rel = MessageBox.Show("Bạn muốn xóa hết code ?", "Input Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (rel == DialogResult.Yes)
                            {
                                for (int i = 0; i < qtyUpld; i++)
                                {
                                    int indRow = dgv_stk.CurrentRow.Index;
                                    //thao tac                       
                                    DataRow drToDelete = dtStk.Rows[indRow];
                                    dtStk.Rows.Remove(drToDelete);
                                }
                            }
                        }
                        txt_hoten.Text = "";
                        txt_manv.Text = "";
                        txt_dayCod.Text = "";
                        //luu stock
                        if (dgv_stk.Rows.Count > 1)
                        {
                            FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + mol + "_Stock.txt", FileMode.Create);
                            StreamWriter sw_stk = new StreamWriter(fs_stk);
                            sw_stk.WriteLine(dgv_stk.Columns[1].HeaderText + "|" + dgv_stk.Columns[2].HeaderText);
                            for (int i = 0; i < dgv_stk.Rows.Count - 1; i++)
                            {
                                if (string.IsNullOrEmpty(dgv_stk.Rows[i].Cells["STT"].Value.ToString()) == false)
                                {
                                    sw_stk.WriteLine(dgv_stk.Rows[i].Cells["Barcode"].Value.ToString() + "|" + dgv_stk.Rows[i].Cells["DateTime"].Value.ToString());
                                }
                            }
                            sw_stk.Close();
                            fs_stk.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Điền lại số lượng!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Kiểm tra Họ tên Sublead và Mã nhân viên!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Chọn code muốn xóa!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_delCod_MouseHover(object sender, EventArgs e)
        {
            btn_delCod.FlatStyle = FlatStyle.Popup;
        }

        private void btn_delCod_MouseLeave(object sender, EventArgs e)
        {
            btn_delCod.FlatStyle = FlatStyle.Flat;
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl2.SelectedTab == tabPage7)
            {
                LoadUpload();
            }

            if(tabControl2.SelectedTab == tabPage8)
            {
                LoadDel();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(chb_rnStk.Checked == true)
            {
                chb_ato.Checked = false;
                DialogResult rel = MessageBox.Show("Line chạy hàng Stock?", "Input Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(rel == DialogResult.Yes)
                {
                    chb_rnStk.ForeColor = Color.Blue;
                    //change off mes
                    cbx_modelName.Visible = true;
                    lbl_modelCode.Text = "Model Code";
                    txt_modelName.Text = "";
                    cbx_modelName.Text = "";
                    cbx_modelName.BackColor = Color.Red;
                    txt_charModelCode.Text = "///";
                    dtb.GetModel(cbx_modelName);
                    MES_Connecting = "CANT";

                    rchtxt_guider.SelectionStart = 76;
                    rchtxt_guider.SelectionLength = 43;
                    rchtxt_guider.SelectionFont = new System.Drawing.Font(rchtxt_guider.SelectionFont, FontStyle.Bold);
                    rchtxt_guider.SelectionColor = Color.Blue;
                }
                else
                {
                    chb_rnStk.Checked = false;
                    chb_nouse.Checked = false;
                }                
            }
            else
            {
                chb_ato.Checked = true;
                chb_rnStk.ForeColor = Color.Black;
                cbx_modelName.Visible = false;
                lbl_modelCode.Text = "Model Code";
                txt_modelName.Text = "";
                cbx_modelName.Text = "";
                cbx_modelName.BackColor = Color.Red;
                txt_charModelCode.Text = "///";

                rchtxt_guider.SelectionStart = 76;
                rchtxt_guider.SelectionLength = 43;
                rchtxt_guider.SelectionFont = new System.Drawing.Font(rchtxt_guider.SelectionFont, FontStyle.Regular);
                rchtxt_guider.SelectionColor = Color.Black;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_ato.Checked == true)
            {
                //Connect Mes
                Connect_MES();
                chb_ato.Checked = true;
                chb_rnStk.Checked = false;
                chb_ato.ForeColor = Color.Blue;
                chb_nouse.Checked = false;

                rchtxt_guider.SelectionStart = 24;
                rchtxt_guider.SelectionLength = 32;
                rchtxt_guider.SelectionFont = new System.Drawing.Font(rchtxt_guider.SelectionFont, FontStyle.Bold);
                rchtxt_guider.SelectionColor = Color.Blue;
            }
            else
            {
                MES_Connecting = "CANT";
                chb_ato.Checked = false;
                chb_ato.ForeColor = Color.Black;

                rchtxt_guider.SelectionStart = 24;
                rchtxt_guider.SelectionLength = 32;
                rchtxt_guider.SelectionFont = new System.Drawing.Font(rchtxt_guider.SelectionFont, FontStyle.Regular);
                rchtxt_guider.SelectionColor = Color.Black;
            }

            if (lbl_modelCode.Text.Contains("P01P-00226A"))
            {                
                txt1_CodeMain.Text = "";
                txt1_MesMain.Text = "";
                txt2_CodeMain.Text = "";
                txt2_MesMain.Text = "";
            }
            else if (lbl_modelCode.Text.Contains("P01P-00225A"))
            {
                txt1_CodeCell.Text = "";
                txt1_MesCell.Text = "";
                txt2_CodeCell.Text = "";
                txt2_MesCell.Text = "";
                txt3_CodeCell.Text = "";
                txt3_MesCell.Text = "";
            }
            else if (lbl_modelCode.Text.Contains("P01P-00227A"))
            {
                txt1_CodeSub.Text = "";
                txt1_MesSub.Text = "";
                txt2_CodeSub.Text = "";
                txt2_MesSub.Text = "";
                txt3_CodeSub.Text = "";
                txt3_MesSub.Text = "";
                txt4_CodeSub.Text = "";
                txt4_MesSub.Text = "";
            }
        }       

        private async void txt_qtyPO_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(3000);
            if(txt_qtyPO.Text.Length > 0)
            {
                int qtPo = 0;
                bool chkQtPo = int.TryParse(txt_qtyPO.Text, out qtPo);
                if(chkQtPo == false)
                {
                    txt_qtyPO.Text = "";
                    MessageBox.Show("Hãy điền số ở mục sản lượng PO!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_qtyPO.Focus();                    
                }  
                else
                {
                    txt_qtyPO.BackColor = Color.White;
                }
            } 
            else
            {
                Display_NhapNhay = new Thread(new ThreadStart(NhapNhayBTN));
                Display_NhapNhay.IsBackground = true;
                Display_NhapNhay.Start(); 
            }
        }

        private async void txt_qty_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(500);
            int qty = int.Parse(txt_qty.Text);
            if (qty > 0 && chb_rnStk.Checked == false && txt_qtyPO.Text != "")
            {
                if(qty >= int.Parse(txt_qtyPO.Text))
                {
                    MessageBox.Show("Line đã đủ sản lượng PO của ca/kíp này!\nChuyển chạy Stock ca/kíp sau!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    chb_rnStk.Checked = true;
                }
            }
        } 
      
        //===========
        public void ShowupHistoryNVL()
        {
            dgv_setup.Columns.Add("No", "No");
            dgv_setup.Columns.Add("Ma_Model", "Ma_Model");
            dgv_setup.Columns.Add("Model", "Model");
            dgv_setup.Columns.Add("Start", "Start");
            dgv_setup.Columns.Add("Len", "Len");
            dgv_setup.Columns.Add("Start", "Start");
            dgv_setup.Columns.Add("Len", "Len");
            dgv_setup.Columns.Add("Min", "Min");
            dgv_setup.Columns.Add("Max", "Max");
            dgv_setup.Columns.Add("Start", "Start");
            dgv_setup.Columns.Add("Len", "Len");
            dgv_setup.Columns.Add("Value", "Value");           

            dgv_setup.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgv_setup.ColumnHeadersHeight = dgv_setup.ColumnHeadersHeight * 2;
            dgv_setup.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgv_setup.CellPainting += new DataGridViewCellPaintingEventHandler(dgv_setup_CellPainting);
            dgv_setup.Paint += new PaintEventHandler(dgv_setup_Paint);
            dgv_setup.Scroll += new ScrollEventHandler(dgv_setup_Scroll);
            dgv_setup.ColumnWidthChanged += new DataGridViewColumnEventHandler(dgv_setup_ColumnWidthChanged);
        }

        private void dgv_setup_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex > -1)
            {
                Rectangle r2 = e.CellBounds;
                r2.Y += e.CellBounds.Height / 2;
                r2.Height = e.CellBounds.Height / 2;
                e.PaintBackground(r2, true);
                e.PaintContent(r2);
                e.Handled = true;
            }
        }

        private void dgv_setup_Paint(object sender, PaintEventArgs e)
        {
            string[] infHeader = { "Information", "Date", "Serial No", "Model Code"};
            for (int j = 0; j < 10; )
            {
                if (j == 0)
                {
                    Rectangle r1 = dgv_setup.GetCellDisplayRectangle(j, -1, true);
                    int w2 = dgv_setup.GetCellDisplayRectangle(j + 1, -1, true).Width;
                    r1.X += 1;
                    r1.Y += 1;
                    r1.Width = r1.Width + (2 * w2) - 2;
                    r1.Height = r1.Height / 2 - 2;
                    e.Graphics.FillRectangle(new SolidBrush(Color.Lime), r1);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(infHeader[0],
                    new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), new SolidBrush(dgv_setup.ColumnHeadersDefaultCellStyle.ForeColor), r1, format);
                    dgv_setup.Columns[0].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[1].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[2].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    j += 3;
                }
                else if (j == 3)
                {
                    Rectangle r1 = dgv_setup.GetCellDisplayRectangle(j, -1, true);
                    int w2 = dgv_setup.GetCellDisplayRectangle(j + 1, -1, true).Width;
                    r1.X += 1;
                    r1.Y += 1;
                    r1.Width = r1.Width + (1 * w2) - 2;
                    r1.Height = r1.Height / 2 - 2;
                    e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), r1);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(infHeader[1],
                    new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), new SolidBrush(dgv_setup.ColumnHeadersDefaultCellStyle.ForeColor), r1, format);
                    dgv_setup.Columns[3].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[4].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    j += 2;
                }
                else if (j == 5)
                {
                    Rectangle r1 = dgv_setup.GetCellDisplayRectangle(j, -1, true);
                    int w2 = dgv_setup.GetCellDisplayRectangle(j + 1, -1, true).Width;
                    r1.X += 1;
                    r1.Y += 1;
                    r1.Width = r1.Width + (3 * w2) - 2;
                    r1.Height = r1.Height / 2 - 2;
                    e.Graphics.FillRectangle(new SolidBrush(Color.Violet), r1);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(infHeader[2],
                    new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), new SolidBrush(dgv_setup.ColumnHeadersDefaultCellStyle.ForeColor), r1, format);
                    dgv_setup.Columns[5].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[6].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[7].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[8].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    j += 4;
                }
                else if (j == 9)
                {
                    Rectangle r1 = dgv_setup.GetCellDisplayRectangle(j, -1, true);
                    int w2 = dgv_setup.GetCellDisplayRectangle(j + 1, -1, true).Width;
                    r1.X += 1;
                    r1.Y += 1;
                    r1.Width = r1.Width + (2 * w2) - 2;
                    r1.Height = r1.Height / 2 - 2;
                    e.Graphics.FillRectangle(new SolidBrush(Color.SkyBlue), r1);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(infHeader[3],
                    new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), new SolidBrush(dgv_setup.ColumnHeadersDefaultCellStyle.ForeColor), r1, format);
                    dgv_setup.Columns[9].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[10].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    dgv_setup.Columns[11].HeaderCell.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    j += 3;
                }               
            }
        }

        private void dgv_setup_Scroll(object sender, ScrollEventArgs e)
        {
            Rectangle rtHeader = dgv_setup.DisplayRectangle;
            rtHeader.Height = dgv_setup.ColumnHeadersHeight / 2;
            dgv_setup.Invalidate(rtHeader);
        }

        private void dgv_setup_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            Rectangle rtHeader = dgv_setup.DisplayRectangle;
            rtHeader.Height = dgv_setup.ColumnHeadersHeight / 2;
            dgv_setup.Invalidate(rtHeader);
        }

        bool chk_lgin = false;
        private void btn_lgin_Click(object sender, EventArgs e)
        {
            btn_lgin.BorderStyle = BorderStyle.Fixed3D;
            if(chk_lgin == false)
            {
                string user = string.Empty, password = string.Empty;
                StreamReader srLgin = new StreamReader(@Application.StartupPath + "\\Account.txt");
                while (srLgin.EndOfStream == false)
                {
                    string str = srLgin.ReadLine();
                    string[] arr = str.Split('=');
                    switch (arr[0])
                    {
                        case "user":
                            user = arr[1];
                            break;
                        case "pass":
                            password = arr[1];
                            break;
                        default:
                            break;
                    }
                }
                srLgin.Close();

                if (user == txt_user.Text && password == txt_passwd.Text)
                {
                    dgv_setup.Enabled = true;
                    btn_addModel.Enabled = true;
                    btn_delModel.Enabled = true;
                    btn_savModel.Enabled = true;
                    txt_passwd.Text = "";
                    btn_lgin.Image = new Bitmap(@Application.StartupPath + "\\Picture\\unlock.png");
                    btn_lgin.SizeMode = PictureBoxSizeMode.StretchImage;
                    chk_lgin = true;
                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_user.Text = "";
                    txt_passwd.Text = "";
                }
            }
            else
            {
                dgv_setup.Enabled = false;
                btn_addModel.Enabled = false;
                btn_delModel.Enabled = false;
                btn_savModel.Enabled = false;
                txt_passwd.Text = "";
                btn_lgin.Image = new Bitmap(@Application.StartupPath + "\\Picture\\lock.png");
                btn_lgin.SizeMode = PictureBoxSizeMode.StretchImage;
                chk_lgin = false;
                txt_user.Text = "";
                txt_user.Focus();
                txt_passwd.Text = "";
            }
        }

        private void btn_addModel_Click(object sender, EventArgs e)
        {
            btn_addModel.BorderStyle = BorderStyle.Fixed3D;
            DataRow dtr = dtSetMol.NewRow();
            dtSetMol.Rows.Add(dtr);
            dtSetMol.AcceptChanges();
            //DataGridViewRow rw = (DataGridViewRow)dgv_setup.Rows[0].Clone();
            //dgv_setup.Rows.Add(rw);
        }

        private void btn_delModel_Click(object sender, EventArgs e)
        {
            btn_delModel.BorderStyle = BorderStyle.Fixed3D;
            foreach (DataGridViewRow dgr in this.dgv_setup.SelectedRows)
            {
                dgv_setup.Rows.RemoveAt(dgr.Index);
            }
        }

        private void btn_savModel_Click(object sender, EventArgs e)
        {
            btn_savModel.BorderStyle = BorderStyle.Fixed3D;
            dtb.SaveModel(dgv_setup);
            dgv_infModel.Columns.Clear();
            DataTable dtMol = dtb.GetData("Select * From Model Order by STT");
            ds.ShowModel(dgv_infModel, dtMol);
        }

        public void LoadBarodeSetup(string modelName)
        {
            for(int i = 0; i < dgv_setup.RowCount - 1; i++)
            {
                if(dgv_setup.Rows[i].Cells["Model"].Value.ToString().Contains(modelName))
                {
                    startSerial = int.Parse(dgv_setup.Rows[i].Cells["Start_Serial"].Value.ToString());
                    lenSerial = int.Parse(dgv_setup.Rows[i].Cells["Len_Serial"].Value.ToString());
                    startModCode = int.Parse(dgv_setup.Rows[i].Cells["Start_ModelCode"].Value.ToString());
                    lenModCode = int.Parse(dgv_setup.Rows[i].Cells["Len_ModelCode"].Value.ToString());
                    minSerial = int.Parse(dgv_setup.Rows[i].Cells["Min_Serial"].Value.ToString());
                    maxSerial = int.Parse(dgv_setup.Rows[i].Cells["Max_Serial"].Value.ToString());
                    break;
                }
            }
        }

        private void btn_lgin_MouseHover(object sender, EventArgs e)
        {
            btn_lgin.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btn_lgin_MouseLeave(object sender, EventArgs e)
        {
            btn_lgin.BorderStyle = BorderStyle.None;
        }

        private void btn_addModel_MouseHover(object sender, EventArgs e)
        {
            btn_addModel.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btn_addModel_MouseLeave(object sender, EventArgs e)
        {
            btn_addModel.BorderStyle = BorderStyle.None;
        }

        private void btn_delModel_MouseHover(object sender, EventArgs e)
        {
            btn_delModel.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btn_delModel_MouseLeave(object sender, EventArgs e)
        {
            btn_delModel.BorderStyle = BorderStyle.None;
        }

        private void btn_savModel_MouseHover(object sender, EventArgs e)
        {
            btn_savModel.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btn_savModel_MouseLeave(object sender, EventArgs e)
        {
            btn_savModel.BorderStyle = BorderStyle.None;
        }

        public bool TimeBetween(DateTime time, DateTime startDateTime, DateTime endDateTime)
        {
            // get TimeSpan
            TimeSpan start = new TimeSpan(startDateTime.Hour, startDateTime.Minute, 0);
            TimeSpan end = new TimeSpan(endDateTime.Hour, endDateTime.Minute, 0);

            // convert datetime to a TimeSpan
            TimeSpan now = time.TimeOfDay;
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }

        public string find_shift()
        {
            string shift;
            DateTime dateTime = DateTime.Now;

            DateTime startDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 8, 0, 0);
            DateTime endDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 20, 0, 0);

            if (TimeBetween(dateTime, startDateTime, endDateTime))
            {
                shift = "Ngày";
            }
            else
            {
                shift = "Đêm";
            }
            return shift;
        }

        public void NhapNhayBTN()
        {
            Hightlight(txt_qtyPO);
        }

        public void Hightlight(TextBox txt)
        {
            while(txt.Text.Length == 0)
            {
                txt.BackColor = Color.White;
                Thread.Sleep(100);
                txt.BackColor = Color.Red;
                Thread.Sleep(100);
                if(txt.Text.Length > 0)
                {
                    break;
                }
            }
        }

        public string[] arrCodeUpload, errCodeMes;
        public int[] status_PCM;
        public void HandleData(string dataScannerRead)
        {
            if (cbx_shift.Text == "" || (txt_modelName.Text == "" && cbx_modelName.Text == "")
                    || (chb_ato.Checked == false && chb_rnStk.Checked == false))
            {
                MessageBox.Show("Bạn điền thiếu thông tin!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string[] arrStr = dataScannerRead.Split(',');
                int err = 0;
                for (int i = 0; i < arrStr.Length; i++)
                {
                    if (rData.CheckFormatCode(arrStr[i], txt_charModelCode.Text, startSerial, lenSerial, startModCode, lenModCode, minSerial, maxSerial) == false)
                    {
                        err++;
                        MessageBox.Show("PCM số " + (i + 1).ToString() + " :\nSai format code đã input. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }                    
                }

                if(err == 0)
                {
                    status_PCM = new int[3];
                    switch (mol)
                    {
                        case "Main":
                            arrCodeUpload = new string[2];
                            errCodeMes = new string[2];                                                
                            break;
                        case "Cell":
                            arrCodeUpload = new string[3];
                            errCodeMes = new string[3];                            
                            break;
                        case "Sub":
                            arrCodeUpload = new string[4];
                            errCodeMes = new string[4];                            
                            break;
                        default:
                            break;
                    }
                    if (chb_ato.Checked == true && chb_rnStk.Checked == false)//Chạy MP
                    {
                        timerShow.Start();
                        WhatCodeUpload_OnStock(arrStr);
                    }
                    if (chb_ato.Checked == false && chb_rnStk.Checked == true)//Chạy stock
                    {
                        timerShow.Stop();
                        #region
                        switch (mol)
                        {
                            case "Main":
                                foreach (var textBox in pl_Main.Controls.OfType<TextBox>())
                                {
                                    if (textBox.Name.Contains("Code"))
                                    {
                                        textBox.BackColor = Color.White;
                                    }
                                }
                                break;
                            case "Cell":
                                foreach (var textBox in pl_Cell.Controls.OfType<TextBox>())
                                {
                                    if (textBox.Name.Contains("Code"))
                                    {
                                        textBox.BackColor = Color.White;
                                    }
                                }
                                break;
                            case "Sub":
                                foreach (var textBox in pl_Sub.Controls.OfType<TextBox>())
                                {
                                    if (textBox.Name.Contains("Code"))
                                    {
                                        textBox.BackColor = Color.White;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        #endregion
                        WhatCodeUpload_Stock(arrStr, mol);
                    }    
                }                    
            }
            textBox1.Text = "";                      
        }

        public int countPcmMes = 0;
        public void WhatCodeUpload_OnStock(string[] arrCode)
        {
            countPcmMes = 0;
            for (int i = 0; i < arrCode.Length; i++)
            {
                if (dgv_stk.Rows.Count > 1)
                {
                    string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    DataRow dtrw = dtStk.NewRow();
                    if (dgv_stk.Rows.Count == 1)
                    {
                        dtrw["STT"] = "1";
                    }
                    else
                    {
                        dtrw["STT"] = (int.Parse(dgv_stk.Rows[dgv_stk.Rows.Count - 2].Cells["STT"].Value.ToString()) + 1).ToString();
                    }
                    dtrw["Barcode"] = arrCode[i];
                    dtrw["DateTime"] = dtime;
                    dtStk.Rows.Add(dtrw);
                    dtStk.AcceptChanges();
                    //upload Mes
                    if (rData.CheckFormatCode(dgv_stk.Rows[0].Cells["Barcode"].Value.ToString(), txt_charModelCode.Text, startSerial, lenSerial, startModCode, lenModCode, minSerial, maxSerial) == false)
                    {
                        MessageBox.Show("Sai format code. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UploadMes(chb_ato, dgv_stk.Rows[i].Cells["Barcode"].Value.ToString());                        
                        arrCodeUpload[i] = dgv_stk.Rows[i].Cells["Barcode"].Value.ToString();
                    }
                }
                else
                {
                    //upload Mes
                    UploadMes(chb_ato, arrCode[i]);
                    arrCodeUpload[i] = arrCode[i];
                }
                rData.SaveCode(arrCode[i], mol);
            }
        }

        public void WhatCodeUpload_Stock(string[] arrCode, string model)
        {
            switch (mol)
            {
                case "Main":
                    txt1_CodeMain.Text = arrCode[0];
                    txt2_CodeMain.Text = arrCode[1];
                    break;
                case "Cell":
                    txt1_CodeCell.Text = arrCode[0];
                    txt2_CodeCell.Text = arrCode[1];
                    txt3_CodeCell.Text = arrCode[2];
                    break;
                case "Sub":
                    txt1_CodeSub.Text = arrCode[0];
                    txt2_CodeSub.Text = arrCode[1];
                    txt3_CodeSub.Text = arrCode[2];
                    txt4_CodeSub.Text = arrCode[3];
                    break;
                default:
                    break;
            }        

            for (int i = 0; i < arrCode.Length; i++)
            {
                if (rData.ChekdoubleCode(arrCode[i], mol) == false)
                {
                    MessageBox.Show("PCM số " + (i + 1).ToString() + " :\nTrùng code đã input. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    #region
                    switch (mol)
                    {
                        case "Main":
                            foreach (var textBox in pl_Main.Controls.OfType<TextBox>())
                            {
                                if (textBox.Name.Contains("Code"))
                                {
                                    if(textBox.Text == arrCode[i])
                                    {
                                        textBox.BackColor = Color.Red;
                                    }
                                }
                            }
                            break;
                        case "Cell":
                            foreach (var textBox in pl_Cell.Controls.OfType<TextBox>())
                            {
                                if (textBox.Name.Contains("Code"))
                                {
                                    if (textBox.Text == arrCode[i])
                                    {
                                        textBox.BackColor = Color.Red;
                                    }
                                }
                            }
                            break;
                        case "Sub":
                            foreach (var textBox in pl_Sub.Controls.OfType<TextBox>())
                            {
                                if (textBox.Name.Contains("Code"))
                                {
                                    if (textBox.Text == arrCode[i])
                                    {
                                        textBox.BackColor = Color.Red;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
                else
                {
                    string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    DataRow dtrw = dtStk.NewRow();
                    if (dgv_stk.Rows.Count == 1)
                    {
                        dtrw["STT"] = "1";
                    }
                    else
                    {
                        dtrw["STT"] = (int.Parse(dgv_stk.Rows[dgv_stk.Rows.Count - 2].Cells["STT"].Value.ToString()) + 1).ToString();
                    }
                    dtrw["Barcode"] = arrCode[i];
                    dtrw["DateTime"] = dtime;
                    dtStk.Rows.Add(dtrw);
                    dtStk.AcceptChanges();
                    if (timer_retStk.Enabled == true)
                    {
                        count_enterStk++;
                    }
                    rData.SaveCode(arrCode[i], mol);
                    ex.SaveLog(arrCode[i], cbx_modelName.Text, "OffMes");
                    txt_qty.Text = (int.Parse(txt_qty.Text) + 1).ToString();
                    //Lưu stock
                    if (dgv_stk.Rows.Count > 1)
                    {
                        FileStream fs_stk = new FileStream(@Application.StartupPath + "\\" + mol + "_Stock.txt", FileMode.Create);
                        StreamWriter sw_stk = new StreamWriter(fs_stk);
                        sw_stk.WriteLine(dgv_stk.Columns[1].HeaderText + "|" + dgv_stk.Columns[2].HeaderText);
                        for (int j = 0; j < dgv_stk.Rows.Count - 1; j++)
                        {
                            if (string.IsNullOrEmpty(dgv_stk.Rows[j].Cells["STT"].Value.ToString()) == false)
                            {
                                sw_stk.WriteLine(dgv_stk.Rows[j].Cells["Barcode"].Value.ToString() + "|" + dgv_stk.Rows[j].Cells["DateTime"].Value.ToString());
                            }
                        }
                        sw_stk.Close();
                        fs_stk.Close();
                    }
                    #region
                    switch (mol)
                    {
                        case "Main":
                            foreach (var textBox in pl_Main.Controls.OfType<TextBox>())
                            {
                                if (textBox.Name.Contains("Code"))
                                {
                                    if (textBox.Text == arrCode[i])
                                    {
                                        textBox.BackColor = Color.Green;
                                    }
                                }
                            }
                            break;
                        case "Cell":
                            foreach (var textBox in pl_Cell.Controls.OfType<TextBox>())
                            {
                                if (textBox.Name.Contains("Code"))
                                {
                                    if (textBox.Text == arrCode[i])
                                    {
                                        textBox.BackColor = Color.Green;
                                    }
                                }
                            }
                            break;
                        case "Sub":
                            foreach (var textBox in pl_Sub.Controls.OfType<TextBox>())
                            {
                                if (textBox.Name.Contains("Code"))
                                {
                                    if (textBox.Text == arrCode[i])
                                    {
                                        textBox.BackColor = Color.Green;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                }               
            }
        }   
   
        private void timerShow_Tick(object sender, EventArgs e)
        {
            int PcmCount = 0;
            switch (mol)
            {
                case "Main":
                    PcmCount = 2;
                    for (int i = 0; i < PcmCount; i++)
                    {
                        Display_Main(i + 1, status_PCM[i]);
                    }
                    break;
                case "Cell":
                    PcmCount = 3;
                    for (int i = 0; i < PcmCount; i++)
                    {
                        Display_Cell(i + 1, status_PCM[i]);
                    }
                    break;
                case "Sub":
                    PcmCount = 4;
                    for (int i = 0; i < PcmCount; i++)
                    {
                        Display_Sub(i + 1, status_PCM[i]);
                    }
                    break;
                default:
                    break;
            }                    
        }
        
        public void Display_Main(int CH, int status)
        {
            Color monitor_default = Color.FromName("DimGray");  // Color hiển thị default
            Color monitor_OK = Color.FromName("Green");  // Color hiển thị lúc OK
            Color monitor_NG = Color.FromName("Red"); // Color hiển thị lúc NG
            switch (CH) // Kiểm tra từng kênh
            {
                case 1: //Kênh 1
                    txt1_CodeMain.Text = arrCodeUpload[0];
                    txt1_MesMain.Text = errCodeMes[0];                    
                    switch (status)
                    {
                        case 1://Default status
                            txt1_CodeMain.BackColor = monitor_default;
                            txt1_MesMain.BackColor = monitor_default;      
                            break;
                        case 2://OK status
                            txt1_CodeMain.BackColor = monitor_OK;
                            txt1_MesMain.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt1_CodeMain.BackColor = monitor_NG;
                            txt1_MesMain.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    txt2_CodeMain.Text = arrCodeUpload[1];
                    txt2_MesMain.Text = errCodeMes[1];                    
                    switch (status)
                    {
                        case 1://Default status
                            txt2_CodeMain.BackColor = monitor_default;
                            txt2_MesMain.BackColor = monitor_default;      
                            break;
                        case 2://OK status
                            txt2_CodeMain.BackColor = monitor_OK;
                            txt2_MesMain.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt2_CodeMain.BackColor = monitor_NG;
                            txt2_MesMain.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;                                  
                default:
                    break;
            }
        }

        public void Display_Cell(int CH, int status)
        {
            Color monitor_default = Color.FromName("DimGray");  // Color hiển thị default
            Color monitor_OK = Color.FromName("Green");  // Color hiển thị lúc OK
            Color monitor_NG = Color.FromName("Red"); // Color hiển thị lúc NG
            switch (CH) // Kiểm tra từng kênh
            {
                case 1: //Kênh 1
                    txt1_CodeCell.Text = arrCodeUpload[0];
                    txt1_MesCell.Text = errCodeMes[0];
                    switch (status)
                    {
                        case 1://Default status
                            txt1_CodeCell.BackColor = monitor_default;
                            txt1_MesCell.BackColor = monitor_default;
                            break;
                        case 2://OK status
                            txt1_CodeCell.BackColor = monitor_OK;
                            txt1_MesCell.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt1_CodeCell.BackColor = monitor_NG;
                            txt1_MesCell.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    txt2_CodeCell.Text = arrCodeUpload[1];
                    txt2_MesCell.Text = errCodeMes[1];
                    switch (status)
                    {
                        case 1://Default status
                            txt2_CodeCell.BackColor = monitor_default;
                            txt2_MesCell.BackColor = monitor_default;
                            break;
                        case 2://OK status
                            txt2_CodeCell.BackColor = monitor_OK;
                            txt2_MesCell.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt2_CodeCell.BackColor = monitor_NG;
                            txt2_MesCell.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    txt3_CodeCell.Text = arrCodeUpload[2];
                    txt3_MesCell.Text = errCodeMes[2];
                    switch (status)
                    {
                        case 1://Default status
                            txt3_CodeCell.BackColor = monitor_default;
                            txt3_MesCell.BackColor = monitor_default;
                            break;
                        case 2://OK status
                            txt3_CodeCell.BackColor = monitor_OK;
                            txt3_MesCell.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt3_CodeCell.BackColor = monitor_NG;
                            txt3_MesCell.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Display_Sub(int CH, int status)
        {
            Color monitor_default = Color.FromName("DimGray");  // Color hiển thị default
            Color monitor_OK = Color.FromName("Green");  // Color hiển thị lúc OK
            Color monitor_NG = Color.FromName("Red"); // Color hiển thị lúc NG
            switch (CH) // Kiểm tra từng kênh
            {
                case 1: //Kênh 1
                    txt1_CodeSub.Text = arrCodeUpload[0];
                    txt1_MesSub.Text = errCodeMes[0];
                    switch (status)
                    {
                        case 1://Default status
                            txt1_CodeSub.BackColor = monitor_default;
                            txt1_MesSub.BackColor = monitor_default;
                            break;
                        case 2://OK status
                            txt1_CodeSub.BackColor = monitor_OK;
                            txt1_MesSub.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt1_CodeSub.BackColor = monitor_NG;
                            txt1_MesSub.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    txt2_CodeSub.Text = arrCodeUpload[1];
                    txt2_MesSub.Text = errCodeMes[1];
                    switch (status)
                    {
                        case 1://Default status
                            txt2_CodeSub.BackColor = monitor_default;
                            txt2_MesSub.BackColor = monitor_default;
                            break;
                        case 2://OK status
                            txt2_CodeSub.BackColor = monitor_OK;
                            txt2_MesSub.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt2_CodeSub.BackColor = monitor_NG;
                            txt2_MesSub.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    txt3_CodeSub.Text = arrCodeUpload[2];
                    txt3_MesSub.Text = errCodeMes[2];
                    switch (status)
                    {
                        case 1://Default status
                            txt3_CodeSub.BackColor = monitor_default;
                            txt3_MesSub.BackColor = monitor_default;
                            break;
                        case 2://OK status
                            txt3_CodeSub.BackColor = monitor_OK;
                            txt3_MesSub.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt3_CodeSub.BackColor = monitor_NG;
                            txt3_MesSub.BackColor = monitor_NG;
                            break;
                        default:
                            break;
                    }
                    break;
                case 4:
                    txt4_CodeSub.Text = arrCodeUpload[3];
                    txt4_MesSub.Text = errCodeMes[3];
                    switch (status)
                    {
                        case 1://Default status
                            txt4_CodeSub.BackColor = monitor_default;
                            txt4_MesSub.BackColor = monitor_default;
                            break;
                        case 2://OK status
                            txt4_CodeSub.BackColor = monitor_OK;
                            txt4_MesSub.BackColor = monitor_OK;
                            break;
                        case 3://NG status
                            txt4_CodeSub.BackColor = monitor_NG;
                            txt4_MesSub.BackColor = monitor_NG;
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void ClearStatus() // Xóa array code hiện tại
        {
            int PcmCount = 0;
            switch (mol)
            {
                case "Main":
                    PcmCount = 2;                   
                    break;
                case "Cell":
                    PcmCount = 3;                    
                    break;
                case "Sub":
                    PcmCount = 4;                    
                    break;
                default:
                    break;
            }
            arrCodeUpload = new string[PcmCount];
            errCodeMes = new string[PcmCount];
            status_PCM = new int[3];
            Array.Clear(arrCodeUpload, 0, PcmCount);
            Array.Clear(errCodeMes, 0, PcmCount); // Reset Error từ mes
            for (int i = 0; i < PcmCount - 1; i++) // Hiển thị status  code jig trên màn hình main
            {
                status_PCM[i] = 1; // Hiển thị [1] Default status
            }
        }

        private void btn_Upload_MouseHover(object sender, EventArgs e)
        {
            btn_Upload.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btn_Upload_MouseLeave(object sender, EventArgs e)
        {
            btn_Upload.BorderStyle = BorderStyle.None;
        }

        public bool newCodeUpl = false;
        private void btn_Upload_Click(object sender, EventArgs e)
        {
            newCodeUpl = true;            
            btn_Upload.BorderStyle = BorderStyle.Fixed3D;
            if (chb_nouse.Checked == false)
            {
                if (cbx_shift.Text == "" || (txt_modelName.Text == "" && cbx_modelName.Text == "")
                    || (chb_ato.Checked == false && chb_rnStk.Checked == false) || txt_NewCode.Text == "")
                {
                    MessageBox.Show("Bạn điền thiếu thông tin!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (rData.CheckFormatCode(txt_NewCode.Text, txt_charModelCode.Text, startSerial, lenSerial, startModCode, lenModCode, minSerial, maxSerial) == false)
                    {
                        MessageBox.Show("Sai format code đã input. Hãy kiểm tra lại!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }                    
                    else
                    {
                        ClearStatus();
                        UploadMes(chb_ato, txt_NewCode.Text);
                        arrCodeUpload[0] = txt_NewCode.Text;
                    }
                }
            }
        }

        private void btn_del_MouseHover(object sender, EventArgs e)
        {
            btn_del.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btn_del_MouseLeave(object sender, EventArgs e)
        {
            btn_del.BorderStyle = BorderStyle.None;
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            btn_del.BorderStyle = BorderStyle.Fixed3D;
            txt_NewCode.Text = "";
        }        

        private async void textBox1_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(2000);
            if(textBox1.Text.Length > 0)
            {
                ClearStatus();
                Thread.Sleep(200);
                HandleData(textBox1.Text);
            }
            else
            {
                return;
            }
        }

        private void btn_loadConfig_Click(object sender, EventArgs e)
        {
            //btn_loadConfig.FlatStyle = FlatStyle.Standard;
            switch (mol)
            {
                case "Main":
                    Scn.LoadConfig(2);
                    break;
                case "Cell":
                    Scn.LoadConfig(3);
                    break;
                case "Sub":
                    Scn.LoadConfig(4);
                    break;
                default:
                    break;
            }
        }

        private void btn_loadConfig_MouseHover(object sender, EventArgs e)
        {
            btn_loadConfig.BackColor = Color.YellowGreen;
        }

        private void btn_loadConfig_MouseLeave(object sender, EventArgs e)
        {
            btn_loadConfig.BackColor = Color.FromArgb(255, 192, 128);
        }

        public int count_enterStk = 0, count_Stk = 0;
        private void timer_retStk_Tick(object sender, EventArgs e)
        {
            if (chb_buStk.Checked == false)
            {
                if (count_Stk == count_enterStk)
                {
                    timer_retStk.Stop();
                    count_Stk = 0;
                    count_enterStk = 0;
                    lbl_qtyBuStk.Text = count_Stk.ToString();
                }
                else
                {
                    MessageBox.Show("Bạn chưa bù đủ số lượng Stock đã đẩy MES!\nSố lượng còn thiếu : " + (count_Stk - count_enterStk).ToString(), "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                count_Stk = 0;
                count_enterStk = 0;
                lbl_qtyBuStk.Text = count_Stk.ToString();
            }
        }

        private void chb_buStk_Click(object sender, EventArgs e)
        {
            if (chb_buStk.Checked == true)
            {
                chb_buStk.Checked = false;
                bool Isopen = false;
                foreach (Form f in Application.OpenForms)
                {
                    if (f.Text == "Admin")
                    {
                        Isopen = true;
                        f.BringToFront();
                        break;
                    }
                }
                if (Isopen == false)
                {
                    Admin ad = new Admin(this);
                    ad.Show();
                }
            }          
        }     
    }
}
