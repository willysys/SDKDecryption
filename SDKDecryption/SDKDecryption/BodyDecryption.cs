using Fiddler;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SDKDecryption
{
    class BodyDecryption
    {
       private static string iv = "";

       public static string decryptSDKBody(String skey,String bodyContent)
        {
            string result;
              //将url_safe_base64编码的数据转换为base64编码的RSA加密的AES密钥
            String skey_base64 = url_safe_base64_decode(skey);
              //将RSA加密的skey进行解密，得到AES私钥
            byte[] aes_key = RSADecryptionWithXMLKey(skey_base64);
            String Result = Convert.ToBase64String(aes_key);           
            result = AESDecryption(bodyContent, Result, iv);
            return result;        
        }

        public static string decryptSDKBody(string skey,byte[] bodyContent)
        {
            string result;
            //将url_safe_base64编码的数据转换为base64编码的RSA加密的AES密钥
            String skey_base64 = url_safe_base64_decode(skey);
            //将RSA加密的skey进行解密，得到AES私钥
            byte[] aes_key = RSADecryptionWithXMLKey(skey_base64);
            String Result = Convert.ToBase64String(aes_key);
            result = AESDecryption(bodyContent, Result, iv);            
            return result;
        }


        public static byte[] RSADecryptionWithXMLKey(string decryptString)
        {
            Assembly assm = Assembly.GetExecutingAssembly();
            Stream istr = assm.GetManifestResourceStream("SDKDecryption.Resources.PrivateKey.xml");
            StreamReader sr = new StreamReader(istr, Encoding.Default);
            StringBuilder buffer = new StringBuilder();
            string strline;
            while ((strline = sr.ReadLine()) != null)
            {
                buffer.Append(strline);
            }
            String xmlPrivateKey = (buffer.ToString());
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            byte[] PlainTextBArray = Convert.FromBase64String(decryptString);
            byte[] DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
            String Result = Convert.ToBase64String(DypherTextBArray);           
            return DypherTextBArray;
        }

        //AES解密
        //解密内容是字符串
        public static string AESDecryption(byte[] text, string AesKey, string AesIV)
        {
            try
            {

                //判断是否是16位 如果不够补0             
                //16进制数据转换成byte
                byte[] encryptedData = text;           
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(AesKey); 
                rijndaelCipher.IV = Encoding.ASCII.GetBytes(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.None;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                byte[] contentText = Decompress(plainText);             
                string result = Encoding.UTF8.GetString(contentText);               
                if (result == null)
                {
                    FiddlerApplication.Log.LogString("aes decryption result is null");
                }
                return result;
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogString(ex.ToString());
                return null;
            }
        }


        //AES解密
        public static string AESDecryption(string text, string AesKey, string AesIV)
        {
            try
            {

                //判断是否是16位 如果不够补0
                //text = tests(text);
                //16进制数据转换成byte
                byte[] encryptedData = Convert.FromBase64String(text);  // strToToHexByte(text);
                Console.WriteLine(encryptedData.Length + "");
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(AesKey); // Encoding.UTF8.GetBytes(AesKey);
                rijndaelCipher.IV = Encoding.ASCII.GetBytes(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.None;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                byte[] contentText = Decompress(plainText);
                string result = Encoding.UTF8.GetString(contentText);
                FiddlerApplication.Log.LogString(result);               
                return result;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                FiddlerApplication.Log.LogString(ex.ToString());
                return null;

            }
        }


        //gzip格式数据解压缩
        public static byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }


        //将url_safe_base64编码格式的字符串转换成base64编码格式的url
        public static String url_safe_base64_decode(String skey)
        {
            String decode_skey = skey.Replace("_", "/").Replace("-", "+");
            int len = skey.Length % 4;
            if (len > 0)
            {
                skey += "====".Substring(0, 4 - len);
            }
            return decode_skey;
        }
    }
}
