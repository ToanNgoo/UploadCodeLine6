using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class MES
    {
        public string SelectTime = "";
        public string SelectLineCode = "";
        private string _user;
        private string _pass;
        private string _dbsource;

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
        
        public string Pass
        {
            get { return _pass; }
            set { _pass = value; }
        }
        
        public string Dbsource
        {
            get { return _dbsource; }
            set { _dbsource = value; }
        }

        public DataSet LoadData()
        {
            try
            {
                string ngaythangnam = DateTime.Now.ToString("yyyyMMdd");
                string str = "";
                DataSet ds = new DataSet();
                OleDbDataAdapter da;
                string constr = @"Provider=MSDAORA.1;Password=" + _pass + @";User ID=" + _user + ";Data Source=" + _dbsource + ";Persist Security Info=True";
                OleDbConnection cnn = new OleDbConnection(constr);
                cnn.Open();
                //PO 1 day
                str = @"SELECT A.PLAN_DATE, A.PO_NO, FN_GET_PO_TYPE_NAME('E', A.PO_TYPE) PO_TYPE, FN_GET_LINE_NAME('E', A.LINE_CD) LINE_NAME, A.PRD_CD PRODUCT_CODE, FN_GET_PRODUCT_NAME(A.PRD_CD) MODEL_NAME, B.BCR_CD MODEL_BAR, DECODE(A.USE_STATUS,'C' ,'Closed','N' ,'Available' ,'A' ,'Available' ,'D' ,'Delete','In Use') PO_STATUS"
                         + @" FROM VW_PO_MST A, VNPCMADMIN.PMPRDSTD B, VW_LINE C"
                         + @" WHERE A.PLAN_DATE= '" + SelectTime + "'"
                         + @" AND A.PRD_CD = B.PRD_CD"
                         + @" AND A.LINE_CD = C.LINE_CD"
                         + @" AND C.LINE_NAME_EN = '" + SelectLineCode + "'"
                         + @" ORDER BY A.PLAN_DATE ASC";
                da = new OleDbDataAdapter(str, constr);
                da.Fill(ds, "tblallinfo");

                cnn.Close();
                da.Dispose();
                return ds;
            }
            catch (Exception)
            {
                MessageBox.Show("Không load được PO đang chạy!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }
        }

        public DataSet LoadLineNo()
        {
            try
            {
                string ngaythangnam = DateTime.Now.ToString("yyyyMMdd");
                string str = "";
                DataSet ds = new DataSet();
                OleDbDataAdapter da;
                string constr = @"Provider=MSDAORA.1;Password=" + _pass + @";User ID=" + _user + ";Data Source=" + _dbsource + ";Persist Security Info=True";
                OleDbConnection cnn = new OleDbConnection(constr);
                cnn.Open();
                str = @"SELECT LINE_NAME_EN, LINE_CD FROM VW_LINE"
                        + @" WHERE BIG_CODE = 'PCM'"
                        + @" ORDER BY LINE_NAME_EN ASC";
                da = new OleDbDataAdapter(str, constr);
                da.Fill(ds, "tbllinename");
                cnn.Close();
                da.Dispose();
                return ds;
            }
            catch (Exception)
            {
                MessageBox.Show("Không load được Line No!", "Input Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw;
            }
        }

        public bool checkconnection()
        {
            string constr = @"Provider=MSDAORA.1;Password=" + _pass + @";User ID=" + _user + ";Data Source=" + _dbsource + ";Persist Security Info=True";
            OleDbConnection cnn = new OleDbConnection(constr);
            try
            {
                cnn.Open();
                cnn.Close();
                return true;
            }
            catch (Exception)
            {
                cnn.Close();
                return false;
            }
        }
    }
}
