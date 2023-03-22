using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class clsConvertData
    {
        public string summake(string str)
        {
            int str_num;
            string hex_string;
            string kq;
            long sum_val_long;
            byte sum_val_byte;
            str_num = str.Length;
            sum_val_long = 0;

            for (int i = 0; i < str.Length; i++)
            {
                sum_val_long += Convert.ToChar((str.Substring(i, 1)));
            }

            sum_val_byte = Convert.ToByte(sum_val_long % 256);

            if (sum_val_byte != 0)
            {
                sum_val_byte = Convert.ToByte(256 - Convert.ToInt32(sum_val_byte));
            };

            hex_string = hex_str2(sum_val_byte);

            kq = hex_string;
            return kq;
        }

        public string hex_str2(byte num)
        {
            string str;
            str = String.Format("{0:X}", num);
            if (str.Length == 1)
            {
                str = "0" + str;
            }
            return str;
        }

        public string HHLL_LLHH(string str)
        {
            return str.Substring(2, 2) + str.Substring(0, 2);
        }

        public int comparedate(string datestr)
        {
            int nam = int.Parse(datestr.Substring(0, 4));
            int thang = int.Parse(datestr.Substring(4, 2));
            int ngay = int.Parse(datestr.Substring(6, 2));
            DateTime ngaydulieu = new DateTime(nam, thang, ngay);
            TimeSpan timesspan = DateTime.Now - ngaydulieu;
            return timesspan.Days / 30;
            //return timesspan.Days;
        }

        public int comparedatelog(string datestr)
        {
            int nam = int.Parse(datestr.Substring(0, 4));
            int thang = int.Parse(datestr.Substring(4, 2));
            int ngay = int.Parse(datestr.Substring(6, 2));
            DateTime ngaydulieu = new DateTime(nam, thang, ngay);
            TimeSpan timesspan = DateTime.Now - ngaydulieu;
            return timesspan.Days;
            //return timesspan.Days;
        }

        public string hex(int value)
        {
            return string.Format("0x{0:X}", value);
        }

        public int FromHex(string value)
        {
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }
            return Int32.Parse(value, NumberStyles.HexNumber);
        }

        public long H2D(string hexstr)
        {
            long tempvalue = FromHex(hexstr);
            long gt = 0;
            if (tempvalue >= 0)
            {
                return tempvalue;
            }
            else
            {
                gt = FromHex(hexstr) - FromHex("7FFF");
                return gt + 32767;
            }
        }

        public string D2B(long value)
        {
            string tempvalue = "";
            if (value == 0) return "0";
            while (value != 0)
            {
                if ((value % 2) == 0)
                {
                    tempvalue = "0" + tempvalue;
                }
                else
                {
                    tempvalue = "1" + tempvalue;
                }
                value = value / 2;
            }
            return insert_0_left(tempvalue, 8);
        }

        public string H2B(string value)
        {
            return D2B(H2D(value));
        }

        public string PCMDate(string datehex)
        {
            string nam = ((H2D(datehex) & 65024) / 512 + 1980).ToString();
            string thang = ((H2D(datehex) & 480) / 32).ToString();
            string ngay = (H2D(datehex) & 31).ToString();

            nam = insert_0_left(nam, 4);
            thang = insert_0_left(thang, 2);
            ngay = insert_0_left(ngay, 2);

            return nam + thang + ngay;
        }

        public string PackDate(string datehex)
        {
            string nam = (2000 + H2D(datehex.Substring(0, 2))).ToString();
            string thang = (H2D(datehex.Substring(2, 2))).ToString();
            string ngay = (H2D(datehex.Substring(4, 2))).ToString();

            nam = insert_0_left(nam, 4);
            thang = insert_0_left(thang, 2);
            ngay = insert_0_left(ngay, 2);

            return nam + thang + ngay;
        }

        public string hex2str(string value)
        {
            string tempvalue = "";
            for (int i = 0; i < (value.Length / 2); i++)
            {
                if (value.Substring(i * 2, 2) != "00")
                {
                    tempvalue = tempvalue + Convert.ToChar(H2D(value.Substring(i * 2, 2)));
                }
            }
            return tempvalue;
        }

        public float calc(string str, string[] V)
        {
            string bien = "";
            string type = "";
            float result = 0;
            str = str + "+V2";
            for (int i = 0; i < str.Length; i++)
            {
                if (str.Substring(i, 1) == "+")
                {
                    if (bien.Substring(0, 1) == "V")
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "+":
                                result = result + float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "-":
                                result = result - float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "*":
                                result = result * float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "/":
                                result = result / float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case "":
                                result = int.Parse(bien);
                                break;

                            case "+":
                                result = result + float.Parse(bien);
                                break;

                            case "-":
                                result = result - float.Parse(bien);
                                break;

                            case "*":
                                result = result * float.Parse(bien);
                                break;

                            case "/":
                                result = result / float.Parse(bien);
                                break;
                        }
                    }
                    bien = "";
                    type = "+";
                }
                else if (str.Substring(i, 1) == "-")
                {
                    if (bien.Substring(0, 1) == "V")
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "+":
                                result = result + float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "-":
                                result = result - float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "*":
                                result = result * float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "/":
                                result = result / float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(bien);
                                break;

                            case "+":
                                result = result + float.Parse(bien);
                                break;

                            case "-":
                                result = result - float.Parse(bien);
                                break;

                            case "*":
                                result = result * float.Parse(bien);
                                break;

                            case "/":
                                result = result / float.Parse(bien);
                                break;
                        }
                    }
                    bien = "";
                    type = "-";
                }
                else if (str.Substring(i, 1) == "*")
                {
                    if (bien.Substring(0, 1) == "V")
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "+":
                                result = result + float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "-":
                                result = result - float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "*":
                                result = result * float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "/":
                                result = result / float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(bien);
                                break;

                            case "+":
                                result = result + float.Parse(bien);
                                break;

                            case "-":
                                result = result - float.Parse(bien);
                                break;

                            case "*":
                                result = result * float.Parse(bien);
                                break;

                            case "/":
                                result = result / float.Parse(bien);
                                break;
                        }
                    }
                    bien = "";
                    type = "*";
                }
                else if (str.Substring(i, 1) == "/")
                {
                    if (bien.Substring(0, 1) == "V")
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "+":
                                result = result + float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "-":
                                result = result - float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "*":
                                result = result * float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "/":
                                result = result / float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(bien);
                                break;

                            case "+":
                                result = result + float.Parse(bien);
                                break;

                            case "-":
                                result = result - float.Parse(bien);
                                break;

                            case "*":
                                result = result * float.Parse(bien);
                                break;

                            case "/":
                                result = result / float.Parse(bien);
                                break;
                        }
                    }
                    bien = "";
                    type = "/";
                }
                else if (str.Substring(i, 1) == "&")
                {
                    if (bien.Substring(0, 1) == "V")
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "+":
                                result = result + float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "-":
                                result = result - float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "*":
                                result = result * float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;

                            case "/":
                                result = result / float.Parse(V[int.Parse(bien.Substring(1, bien.Length - 1))]);
                                break;
                            case "&":
                                string a = result.ToString() + V[int.Parse(bien.Substring(1, bien.Length - 1))];
                                result = int.Parse(a);
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case "":
                                result = float.Parse(bien);
                                break;

                            case "+":
                                result = result + float.Parse(bien);
                                break;

                            case "-":
                                result = result - float.Parse(bien);
                                break;

                            case "*":
                                result = result * float.Parse(bien);
                                break;

                            case "/":
                                result = result / float.Parse(bien);
                                break;
                            case "&":
                                result = int.Parse(result.ToString() + bien);
                                break;
                        }
                    }
                    bien = "";
                    type = "*";
                }
                else
                {
                    bien = bien + str.Substring(i, 1);
                }
            }//end for

            return result;
        }

        public string hex_str4(int num)
        {
            return insert_0_left(hex(num), 4);
        }

        public string date2hex(string value)
        {
            return hex_str4((int.Parse(value.Substring(0, 4)) - 1980) * 512 + int.Parse(value.Substring(4, 2)) * 32 + int.Parse(value.Substring(6, 2)));
        }

        public string removeline(string value)
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

        public string ascii2hex(string ascii)
        {
            StringBuilder sb = new StringBuilder();
            byte[] inputbytes = Encoding.UTF8.GetBytes(ascii);
            foreach (byte b in inputbytes)
            {
                sb.Append(string.Format("{0:x2}", b));
            }
            return sb.ToString();
        }

        public string hex2ascii(string value)
        {
            string tempvalue = "";
            for (int i = 0; i < (value.Length / 2); i++)
            {
                if (value.Substring(i * 2, 2) != "00")
                {
                    tempvalue = tempvalue + Convert.ToChar(H2D(value.Substring(i * 2, 2)));
                }
            }
            return tempvalue;
        }

        public string insert_0_left(string str, int len)
        {
            if (str.Length >= len) return str;
            while (str.Length != len)
            {
                str = "0" + str;
            }
            return str;
        }

        public string insert_0_right(string str, int len)
        {
            if (str.Length >= len) return str;
            while (str.Length != len)
            {
                str = str + "0";
            }
            return str;
        }

        public string insert_Blank_Left(string str, int len)
        {
            if (str.Length >= len) return str;
            while (str.Length != len)
            {
                str = " " + str;
            }
            return str;
        }

        public string insert_Blank_Right(string str, int len)
        {
            if (str.Length >= len) return str;
            while (str.Length != len)
            {
                str = str + " ";
            }
            return str;
        }

        public string removenull(string str)
        {
            if (str.Length == 0) return "";
            string data = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str.Substring(i, 1) != "\0")
                {
                    data = data + str.Substring(i, 1);
                }
            }
            return data;
        }

        public double str2num(string str)
        {
            return float.Parse(str) * 1000;
        }
        public double str2numdvm(string str)
        {
            //float so = Convert.ToInt64(str.Substring(0, 6));
            //int mu = Convert.ToInt32(str.Substring(12, 3));
            return double.Parse(str) * 1000;
            //return float.Parse(str);
        }
        public double str2numdap(string str)
        {
            //float so = Convert.ToInt64(str.Substring(0, 6));
            //int mu = Convert.ToInt32(str.Substring(12, 3));
            return double.Parse(str);
            //return float.Parse(str)/100000000;
        }

        public double str2num_Hioki(string str)
        {
            string[] data = str.Split('E');
            string so = data[0];
            string mu;
            if (data[1] == "")
            {
                mu = "0";
            }
            else
            {
                mu = data[1];
            }

            return float.Parse(so + "E" + mu);
        }
    }
}
