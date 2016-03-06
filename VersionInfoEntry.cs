using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Finding_Framework_Version_from_Solution_Files
{
	public partial class VersionInfoEntry : Form
	{
		public string DevExpressVersion { get; set; }

		public VersionInfoEntry()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.DevExpressVersion = this.textBox1.Text.Trim();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
