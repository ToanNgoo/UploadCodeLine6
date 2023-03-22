using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class Excel
    {
        public void SaveLog(string inp_Cod, string modelNam, string mes)
        {
            string dt = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            FileStream fs = new FileStream(@Application.StartupPath + "\\Result\\" + modelNam + "\\" + dt + "-" + mes + ".log", FileMode.Append);            
            StreamWriter sw = new StreamWriter(fs);            
            sw.WriteLine(inp_Cod);            
            sw.Close();            
            fs.Close();
        }
    }
}
