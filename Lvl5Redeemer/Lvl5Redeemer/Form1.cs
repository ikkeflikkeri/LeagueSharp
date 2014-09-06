using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lvl5Redeemer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (webBrowser1.Document.GetElementById("error-body") == null)
                MessageBox.Show("Goed");
            else
                MessageBox.Show("Slecht");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                return;

            if (webBrowser1.Document.GetElementById("error-body") == null)
                timer1.Stop();
            else
                webBrowser1.Refresh();
        }
    }
}
