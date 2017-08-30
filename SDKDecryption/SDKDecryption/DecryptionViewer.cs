using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDKDecryption
{
    public partial class DecryptionViewer : UserControl
    {
        public DecryptionViewer()
        {
            InitializeComponent();
        }

   
     private void DecryptionViewer_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void setText(string content)
        {
            // this.textBox1.Text = content;
            //this.treeView1.
            this.richTextBox1.Text = content;
        }

        public void clearText()
        {
            this.richTextBox1.Clear();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }


}
