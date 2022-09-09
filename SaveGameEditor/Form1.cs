using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveGameEditor
{
    public partial class Form1 : Form
    {
        public static Form1 form1 = null;
        public Form1()
        {
            form1 = this;
            InitializeComponent();
            l = new SaveLoader();
        }
        SaveLoader l = null;
        private void button1_Click(object sender, EventArgs e)
        {
            l.loadAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            l.saveAll();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            l.transferActiveSave();
        }
    }
}
