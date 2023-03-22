using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
using System.IO;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class database
    {
        //Ket noi file acess
        string conStr = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source =" + Application.StartupPath + @"\Database.mdb";

        public bool GetConnection()
        {            
            OleDbConnection cnn = new OleDbConnection(conStr);
            try
            {
                cnn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool GetConnection2()
        {
            OleDbConnection cnn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source =" + Application.StartupPath + @"\MesMsg.mdb");
            try
            {
                cnn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DataTable GetData(string str)
        {
            DataTable dta1 = new DataTable();
            OleDbDataAdapter da1 = new OleDbDataAdapter(str, conStr);
            da1.Fill(dta1);
            return dta1;
        }

        public DataTable GetData2(string str)
        {
            DataTable dta1 = new DataTable();
            OleDbDataAdapter da1 = new OleDbDataAdapter(str, @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source =" + Application.StartupPath + @"\MesMsg.mdb");
            da1.Fill(dta1);
            return dta1;
        }

        public void AddModel(string stt, string molCode, string molNam, string kty, string format, string cdai)
        {
            OleDbConnection cnn = new OleDbConnection(conStr);
            cnn.Open();
            string str = "Insert Into Model values('" + stt + "','" 
                                                      + molCode + "','" 
                                                      + molNam + "','"
                                                      + kty + "','"
                                                      + format + "','"         
                                                      + cdai + "')";
            OleDbCommand cmd = new OleDbCommand(str, cnn);
            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public void DelModel(string molCode)
        {
            OleDbConnection cnn = new OleDbConnection(conStr);
            cnn.Open();
            string str = "Delete * From Model Where Ma_model ='" + molCode + "'";                                                      
            OleDbCommand cmd = new OleDbCommand(str, cnn);
            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public string GetMolCod(string productCode)
        {
            string str = string.Empty;
            string strSle = "Select Ky_tu_dinh_danh from Model where Ma_Model = '" + productCode + "'";

            DataTable dt = new DataTable();
            dt = GetData(strSle);

            foreach (DataRow dtr in dt.Rows)
            {
                str = dtr.ItemArray[0].ToString();
            }
            return str;
        }

        public string GetMesErr(string errCode)
        {
            string str = string.Empty;
            string strSle = "Select Info_en from Message where Msg_ID = '" + errCode + "'";

            DataTable dt = new DataTable();
            dt = GetData2(strSle);

            foreach (DataRow dtr in dt.Rows)
            {
                str = dtr.ItemArray[0].ToString();
            }
            return str;
        }

        public string GetModel(ComboBox cbx)
        {
            string str = string.Empty;
            string strSle = "Select Ten_Model from Model";

            DataTable dt = new DataTable();
            dt = GetData(strSle);

            cbx.Items.Clear();
            foreach (DataRow dtr in dt.Rows)
            {
                cbx.Items.Add(dtr.ItemArray[0].ToString());
            }
            return str;
        }

        public string[] GetModelName(string modelNam)
        {
            string[] arr = new string[2];
            string str = string.Empty;
            string strSle = "Select Ma_Model, Ky_tu_dinh_danh from Model where Ten_Model='" + modelNam + "'";

            DataTable dt = new DataTable();
            dt = GetData(strSle);

            int i = 0;
            foreach (DataRow dtr in dt.Rows)
            {
                 arr[i] = dtr.ItemArray[i].ToString();
                 arr[i + 1] = dtr.ItemArray[i + 1].ToString();
                 i++;
            }
            return arr;
        }

        public string GetRight(string id)
        {
            string str = string.Empty;
            if(string.IsNullOrEmpty(id) == false)//có data
            {                              
                string strSle = "Select Ho_ten from SubLeader where Ma_nhan_vien='" + id + "'";

                DataTable dt = new DataTable();
                dt = GetData(strSle);

                foreach (DataRow dtr in dt.Rows)
                {
                    str = dtr.ItemArray[0].ToString();
                }

                return str;
            }
            else
            {
                return str;
            }            
        }

        public void SaveModel(DataGridView dgv)
        {
            OleDbConnection cnn = new OleDbConnection(conStr);
            cnn.Open();

            string strDel = "Delete * From Setup_Model";
            OleDbCommand cmdDel = new OleDbCommand(strDel, cnn);
            cmdDel.ExecuteNonQuery();

            string strDel2 = "Delete * From Model";
            OleDbCommand cmdDel2 = new OleDbCommand(strDel2, cnn);
            cmdDel2.ExecuteNonQuery();

            for (int j = 0; j < dgv.RowCount - 1; j++)
            {
                string strIns = "Insert Into Setup_Model values('" + dgv.Rows[j].Cells["No"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Ma_Model"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Model"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Start_Date"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Len_Date"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Start_Serial"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Len_Serial"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Min_Serial"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Max_Serial"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Start_ModelCode"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Len_ModelCode"].Value.ToString() + "','"
                                                                   + dgv.Rows[j].Cells["Value_ModelCode"].Value.ToString() + "')";
                OleDbCommand cmdIns = new OleDbCommand(strIns, cnn);
                cmdIns.ExecuteNonQuery();

                int subLength = int.Parse(dgv.Rows[j].Cells["Start_ModelCode"].Value.ToString());
                int lenSpecicalChar = int.Parse(dgv.Rows[j].Cells["Len_ModelCode"].Value.ToString());
                int total = subLength + lenSpecicalChar;
                string strIns2 = "Insert Into Model values('" + dgv.Rows[j].Cells["No"].Value.ToString() + "','"
                                                              + dgv.Rows[j].Cells["Ma_Model"].Value.ToString() + "','"
                                                              + dgv.Rows[j].Cells["Model"].Value.ToString() + "','"
                                                              + dgv.Rows[j].Cells["Value_ModelCode"].Value.ToString() + "','"
                                                              + WriteData(total, dgv.Rows[j].Cells["Value_ModelCode"].Value.ToString()) + "','"
                                                              + (total).ToString() + "')";
                OleDbCommand cmdIns2 = new OleDbCommand(strIns2, cnn);
                cmdIns2.ExecuteNonQuery();
            }
            cnn.Close();
        }  
      
        public string WriteData(int dem, string specicalChar)
        {
            string str = string.Empty;
            for(int i = 0; i < dem; i++)
            {
                str = str + "*";
                if(i == dem - specicalChar.Length)
                {
                    str = str + specicalChar;
                }
            }
            return str;
        }
    }
}
