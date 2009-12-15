using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Examples.ProgressBar
{
    public partial class ProgressBar : Form
    {
        public ProgressBar()
        {
            InitializeComponent();



        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            var account = Authenticate.Connection("foobar", "fdaslfkj");
            var container = account.GetContainer("foobarcontainer");
            var so = container.GetStorageObject("myfoo.txt");

            Action<long, long> progress = delegate(long amt, long total)
            {
                float percentage = amt / total;
                var nicepercentage = Convert.ToInt32(percentage * 100);
                this.progressBar1.Value = nicepercentage;
            };


            so.SaveToDisk(File.OpenRead("c:\\myfoo.txt"), progress);
        }
    }
}
