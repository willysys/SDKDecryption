using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Standard;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SDKDecryption
{
    public sealed class Decryption : Inspector2, IRequestInspector2, IBaseInspector2
    {

        private DecryptionViewer myControl = new DecryptionViewer();
        private bool m_bDirty;
        private bool m_bReadOnly;
        private byte[] m_entityBody;
        private HTTPRequestHeaders m_Headers;
        private JSONRequestViewer jsonRequestViewer;
       

       public Decryption()
        {
            this.jsonRequestViewer = new JSONRequestViewer();
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
                return this.m_entityBody;        
            }

            set
            {                
                this.m_entityBody = value;
                this.DoDecryption();
            }
        }

        
        public void DoDecryption()
        {          
            //如果需要解密
            String path = this.m_Headers.RequestPath;
            if (this.m_entityBody == null || this.m_entityBody.Length == 0)
            {
                this.myControl.clearText();
            }
            else
            {
                if (path.Contains("skey"))
                {
                    String skey = Regex.Split(path, "skey=")[1];                  
                    //此种方式才能将byte[]转换成常见的base64编码的字符串
                    String base64Body = System.Text.Encoding.Default.GetString(this.m_entityBody);
                    //此种方式转不了base64编码格式的字符串                   
                    String decryptionBody = BodyDecryption.decryptSDKBody(skey, base64Body);                    
                    myControl.setText(decryptionBody);
                }
                else
                {
                    String decodeBody = System.Text.Encoding.Default.GetString(this.m_entityBody);
                    myControl.setText(decodeBody);
                }
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
            }
        }

        public HTTPRequestHeaders headers
        {
            get
            {
                FiddlerApplication.Log.LogString("headers get function.");
                return m_Headers;
            }  
            set
             {
                this.m_Headers = value;           

            }

        }

        public override void AddToTab(TabPage o)
        {
            o.Text = "Decryption";
            o.Controls.Add(this.myControl);
            o.Controls[0].Dock = DockStyle.Fill;
        }

        public  void Clear()
        {
            this.m_entityBody = null;
            this.myControl.clearText();         
        }


        public override int GetOrder() =>
       100;


    }
}
