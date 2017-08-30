using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Standard;
using System.Text.RegularExpressions;

namespace SDKDecryption
{
    public class DecryptionFormatJson : Inspector2, IRequestInspector2, IBaseInspector2
    {
        private bool m_bDirty;
        private bool m_bReadOnly;
        private byte[] m_entityBody;
        private HTTPRequestHeaders m_Headers;
        private JSONRequestViewer jsonRequestViewer;

        public DecryptionFormatJson()
        {
            
        }

        public bool bDirty
        {
            get
            {
                return this.m_bDirty;
            }
        }

        public byte[] body
        {
            get
            {
                return jsonRequestViewer.body;
            }

            set
            {
                this.m_entityBody = value;
                byte[] decodedBody = this.DoDecryption();
                if (decodedBody != null)
                {
                    jsonRequestViewer.body = decodedBody;
                }
                else
                {
                    jsonRequestViewer.body = value;
                }
            }
        }

        public byte[] DoDecryption()
        {           
            //如果需要解密
            String path = this.m_Headers.RequestPath;
            if (path.Contains("skey"))
            {
                String skey = Regex.Split(path, "skey=")[1];               
                //此种方式才能将byte[]转换成常见的base64编码的字符串
                String base64Body = System.Text.Encoding.Default.GetString(this.m_entityBody);
                //此种方式转不了base64编码格式的字符串
                // String bodytext= Convert.ToBase64String(this.m_entityBody);
                String decryptionBody = BodyDecryption.decryptSDKBody(skey, base64Body);
                byte[] decodeBody = System.Text.Encoding.UTF8.GetBytes(decryptionBody);
                return decodeBody;
            }
            else
            {             
                this.Clear();
                return null;
            }
        }


        public bool bReadOnly
        {
            get
            {
                return this.m_bReadOnly;              
            }

            set
            {
                this.m_bReadOnly = value;
                jsonRequestViewer.bReadOnly = value;              
            }
        }

        public HTTPRequestHeaders headers
        {
            get
            {
                return this.m_Headers;              
            }

            set
            {

                this.m_Headers = value;
                jsonRequestViewer.headers = value;               
            }
        }

        public override void AddToTab(TabPage o)
        {
            jsonRequestViewer = new JSONRequestViewer();
            jsonRequestViewer.AddToTab(o);
            o.Text = "DecryptionFormatJson";         
        }

        public void Clear()
        {
            this.m_entityBody = null;
            jsonRequestViewer.Clear();           
        }

        public override int GetOrder()
        {
            return jsonRequestViewer.GetOrder();          
        }

        public override int ScoreForContentType(string sMIMEType)
        {
            return jsonRequestViewer.ScoreForContentType(sMIMEType);
        }

        public override void SetFontSize(float flSizeInPoints)
        {
            jsonRequestViewer.SetFontSize(flSizeInPoints);
        }

    }
}
