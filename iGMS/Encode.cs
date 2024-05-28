using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WMS
{
    public class Encode
    {
        public static string ToMD5(string str)
        {
            string result = "";
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                result += buffer[i].ToString("x2");
            }
            return result;
        } 

        public static string EPC(string str)
        {
            Random res = new Random();
            String strs = "abcdefghijklmnopqrstuvwxyz";
            int size = 2;
            String ran = "";
            for (int i = 0; i < size; i++)
            {
                int x = res.Next(26);
                ran = ran + strs[x];
            }
            int strlength = str.Length;
            int startno = strlength - 6;
            int startba = strlength - 11;
            string idepcsno = "";
            string idepcsba = "";
            string idepcs = "";
            if (strlength > 11)
            {
                idepcsno = str.Substring(startno, 6);
                idepcsba = str.Substring(startba, 3);
                idepcs = idepcsba + idepcsno + ran;
                byte[] bytes = Encoding.Default.GetBytes(idepcs);
                string idepc = BitConverter.ToString(bytes);
                idepc = idepc.Replace("-", "");
                return idepc + "EE";
            }
            else
            {
                idepcsno = str;
                byte[] bytes = Encoding.Default.GetBytes(idepcsno);
                string idepc = BitConverter.ToString(bytes);
                idepc = idepc.Replace("-", "");
                var idpeclength = idepc.Length;
                if (idpeclength < 22)
                {
                    var add = 22 - idpeclength;
                    for (int i = 0; i < add; i++)
                    {
                        idepc += "0";
                    }
                }
                return idepc + "EE";
            }
        }
        public async static Task<string> GenerateRandomString(int length)
        {
            Random random = new Random();
            const string characters = "0123456789";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(characters.Length);
                result.Append(characters[index]);
            }

            return result.ToString();
        }
        public static string EpctoUpc(string epc)
        {
            string EPC = epc;
            string EPCIN = "";
            string SGTIN = "";
            string ItemRef = "";
            string Result = "";
            string UPC = "";
            int i, SGTINResult, ItemRefResult, CheckDigit = 0;

            for (i = 0; i < EPC.Length; i++)
            {
                EPCIN += Convert.ToString(Convert.ToInt32(EPC.Substring(i, 1), 16), 2).PadLeft(4, '0');
            }

            EPCIN = EPCIN.Substring(EPCIN.Length - 82);
            SGTIN = EPCIN.Substring(0, 24);
            ItemRef = EPCIN.Substring(24, 20);

            SGTINResult = 0;
            for (i = 1; i < SGTIN.Length; i++)
            {
                SGTINResult += Convert.ToInt32(SGTIN.Substring(i, 1)) * (int)Math.Pow(2, SGTIN.Length - i - 1);
            }

            ItemRefResult = 0;
            for (i = 1; i < ItemRef.Length; i++)
            {
                ItemRefResult += Convert.ToInt32(ItemRef.Substring(i, 1)) * (int)Math.Pow(2, ItemRef.Length - i - 1);
            }

            Result = SGTINResult.ToString() + ("00000" + ItemRefResult).Substring(Math.Max(0, ("00000" + ItemRefResult).Length - 5));

            CheckDigit = 0;
            for (i = 1; i <= 17; i++)
            {
                if (Result.Length > Math.Abs(i - 17))
                {
                    if (i % 2 != 0)
                    {
                        CheckDigit += 3 * Convert.ToInt32(Result.Substring(Result.Length - Math.Abs(i - 17) - 1, 1));
                    }
                    else
                    {
                        CheckDigit += Convert.ToInt32(Result.Substring(Result.Length - Math.Abs(i - 17) - 1, 1));
                    }
                }
            }

            CheckDigit = Convert.ToInt32(Math.Ceiling((double)CheckDigit / 10) * 10) - CheckDigit;
            UPC = Result + CheckDigit;

            return UPC;
        }
    }
}