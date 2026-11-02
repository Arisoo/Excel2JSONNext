using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib
{
    public sealed class Cryption
    {
        // 알고리즘 구현 설명
        //  key,iv는 RijnadaelManaged 생성하고 난뒤 GenerateKey/IV를 통해 얻은걸 사용할것 설정에 따라 크기제한이 다르기때문에
        //  RijnadaelManaged 생성시 특별한 옵션을 줄 필요없다.
        //  key, iv 는 base64string으로 변환해서 관리해야한다.
        //  암호화된 문자열도 base64string으로 관리해야한다.

        //the Key and the Inicialization Vector
        private byte[] key;
        public string KeyBase64String
        {
            get { return Convert.ToBase64String(key); }
        }
        public string KEYCodes
        {
            get { return ToCSharpCodes(ref key); }
        }

        private byte[] iv;
        public string IVBase64String
        {
            get { return Convert.ToBase64String(iv); }
        }
        public string IVCodes
        {
            get { return ToCSharpCodes(ref iv); }
        }
        ////password view
        //private string pwd_str;
        //private byte[] pwd_byte;

        /// <summary>
        /// key, iv를 내부에서 자동생성한다.
        /// </summary>
        public Cryption()
        {
            RijndaelManaged gen = new RijndaelManaged();
            gen.GenerateIV();
            gen.GenerateKey();
            string sk = Convert.ToBase64String(gen.Key);
            string siv = Convert.ToBase64String(gen.IV);
            key = Convert.FromBase64String(sk);
            iv = Convert.FromBase64String(siv);
        }

        public Cryption(byte[] key_val, byte[] iv_val)
        {
            key = key_val;
            iv = iv_val;
        }

        //Constructor
        /// <summary>
        /// key와 vector라는 2개의 값으로 RijndaelManaged 알고리즘의 암호화,복호화를 합니다.        /// 
        /// key,iv는 생성하는방법이 무척 까다롭기때문에, Generate??()을 활용하는것을 권장
        /// </summary>
        /// <param name="key_val">영문자숫자_만가능 식별자규칙 @#$%^+| 문자 사용불가능 (유니코드안됨), base64인코딩된 값이어야한다.</param>
        /// <param name="iv_val">영문자숫자만 가능, base64인코딩된 값이어야한다.</param>
        public Cryption(string key_val, string iv_val)
        {
            key = Convert.FromBase64String(key_val);
            iv = Convert.FromBase64String(iv_val);
        }

        public string Encrypt(string s)
        {
            string rs;
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;
            RijndaelManaged aesAlg = null;

            aesAlg = new RijndaelManaged();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            msEncrypt = new MemoryStream();
            csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            swEncrypt = new StreamWriter(csEncrypt);

            swEncrypt.Write(s);

            if (swEncrypt != null) swEncrypt.Close();
            if (csEncrypt != null) csEncrypt.Close();
            if (msEncrypt != null) msEncrypt.Close();
            if (aesAlg != null) aesAlg.Clear();

            rs = Convert.ToBase64String(msEncrypt.ToArray());
            return rs;
        }

        public string Decrypt(string s)
        {
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;
            RijndaelManaged aesAlg = null;
            string plaintext = null;

            aesAlg = new RijndaelManaged();
            aesAlg.Key = key;
            aesAlg.IV = iv;


            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            byte[] cipherText = Convert.FromBase64String(s);
            msDecrypt = new MemoryStream(cipherText);
            csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            srDecrypt = new StreamReader(csDecrypt);

            plaintext = srDecrypt.ReadToEnd();

            if (srDecrypt != null) srDecrypt.Close();
            if (csDecrypt != null) csDecrypt.Close();
            if (msDecrypt != null) msDecrypt.Close();
            if (aesAlg != null) aesAlg.Clear();

            return plaintext;
        }

        public static string ConvertStringToHex(string unicodeString)
        {
            string hex = "";
            byte[] ConvData = System.Text.Encoding.Unicode.GetBytes(unicodeString);
            foreach (byte b in ConvData)
            {
                hex += string.Format("{0:x2}", b);
            }
            return hex;
        }

        public static string ConvertHexToString(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format("The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            string s = System.Text.Encoding.Unicode.GetString(HexAsBytes, 0, HexAsBytes.Length);
            return s;
        }

        public static string ToCSharpCodes(ref byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            for (int i = 0; i < data.Length; ++i)
            {
                if (0 < i) sb.Append(",");
                sb.Append(" ");
                sb.Append("0x");
                sb.Append(BitConverter.ToString(data, i, 1));
            }
            sb.Append(" };");

            return sb.ToString();

        }
    }
}
