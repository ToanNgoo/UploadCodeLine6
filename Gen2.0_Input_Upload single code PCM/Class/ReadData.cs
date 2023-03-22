using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class ReadData
    {
        public void ReadConfigMES(TextBox ip, TextBox lineno, TextBox mcid, TextBox stnid, TextBox port)
        {
            StreamReader sr = new StreamReader(@Application.StartupPath + "\\Data\\MesSetting.ini");
            while(sr.EndOfStream == false)
            {
                string[] str = sr.ReadLine().Split('=');
                switch(str[0])
                {
                    case "IP":
                        ip.Text = str[1];
                        break;
                    case "LineNo":
                        lineno.Text = str[1];
                        break;
                    case "MCID":
                        mcid.Text = str[1];
                        break;
                    case "StnID":
                        stnid.Text = str[1];
                        break;
                    case "Port":
                        port.Text = str[1];
                        break;
                    default:
                        break;
                }
            }
            sr.Close();
        }

        public void ReadConfigScn(ComboBox Com, ComboBox brat, ComboBox dtBits, ComboBox prity, ComboBox hndShk)
        {
            StreamReader sr = new StreamReader(@Application.StartupPath + "\\Data\\ScannerSetting.ini");
            while (sr.EndOfStream == false)
            {
                string[] str = sr.ReadLine().Split('=');
                switch (str[0])
                {
                    case "COM":
                        Com.Text = str[1];
                        break;
                    case "BaudRate":
                        brat.Text = str[1];
                        break;
                    case "DataBits":
                        dtBits.Text = str[1];
                        break;
                    case "Parity":
                        prity.Text = str[1];
                        break;
                    case "HandShaking":
                        hndShk.Text = str[1];
                        break;
                    default:
                        break;
                }
            }
            sr.Close();
        }

        public string ReadQty()
        {
            string str = string.Empty;
            StreamReader sr = new StreamReader(@Application.StartupPath + "\\QtyCurrent.txt");
            while (sr.EndOfStream == false)
            {
                str = sr.ReadLine();               
            }
            sr.Close();
            return str;
        }

        public void SaveCode(string inp_Cod, string mol)
        {
            FileStream fs = new FileStream(@Application.StartupPath + "\\Log\\Duplicate\\" + mol + "_Code_Input.log", FileMode.Append);            
            StreamWriter sw = new StreamWriter(fs);           
            sw.WriteLine(inp_Cod);            
            sw.Close();            
            fs.Close();
        }

        public bool ChekdoubleCode(string inp_Cod, string mol)
        {
            if (File.Exists(@Application.StartupPath + "\\Log\\Duplicate\\" + mol + "_Code_Input.log"))
            {
                int same = 0;
                StreamReader sr = new StreamReader(@Application.StartupPath + "\\Log\\Duplicate\\" + mol + "_Code_Input.log");
                while (sr.EndOfStream == false)
                {
                    string strRead = sr.ReadLine();
                    if (strRead == inp_Cod)
                    {
                        same++;
                    }
                }
                sr.Close();

                if (same > 0)//trung
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public bool CheckFormatCode(string code, string codeMol, int startSerial, int lenSerial, int startModCod, int lenModCod, int minSerial, int maxSerial)
        {
            if(code.Length == 14)
            {
                int err = 0;
                DateTime dat;
                string strY = code.Substring(0, 2);
                string strM = code.Substring(2, 2);
                string strD = code.Substring(4, 2);
                string chr = code.Substring(6, 1);
                string serialNo = code.Substring(startSerial, lenSerial);
                string molCod = code.Substring(startModCod, lenModCod);
                if(DateTime.TryParse(strM + "/" + strD + "/20" + strY, out dat) == false )
                {
                    err++;
                }
                else if (chr != "V")
                {
                    err++;
                }
                else if((int.Parse(serialNo) > maxSerial) && (int.Parse(serialNo) < minSerial))
                {
                    err++;
                }
                else if (molCod != codeMol)
                {
                    err++;
                }
                else
                {
                    err = 0;
                }

                if(err > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void SaveData(string path, string data)
        {
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(data);
            sw.Close();
            fs.Close();
        }
    }
}
