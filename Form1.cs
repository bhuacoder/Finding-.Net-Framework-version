using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Finding_Framework_Version_from_Solution_Files
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			List<string> solutionsPaths = GetSolutionPaths();
			List<string> projectPaths = GetProjectPaths(solutionsPaths);

			DataSet ds = new DataSet();
			DataTable dt =  new DataTable();
			dt.Columns.Add("Project Name", typeof(string));
			dt.Columns.Add("Version", typeof(string));
			dt.Columns.Add("Full Path", typeof(string));
			ds.Tables.Add(dt);

			foreach (string projectPath in projectPaths)
			{
				using (StreamReader r = new StreamReader(projectPath))
				{
					string line;
					while ((line = r.ReadLine()) != null)
					{
						if(line.Contains("TargetFrameworkVersion"))
						{
							DataRow dr = dt.NewRow();
							dr[0] = Path.GetFileNameWithoutExtension(projectPath);
							dr[1] = line.Replace("<TargetFrameworkVersion>v", string.Empty).Replace("</TargetFrameworkVersion>", string.Empty);
							dr[2] = projectPath;
							ds.Tables[0].Rows.Add(dr);
						}
					}
				}
			}

			this.dataGridView1.DataSource = ds.Tables[0];
			this.dataGridView1.Sort(this.dataGridView1.Columns[1], ListSortDirection.Ascending);
			MessageBox.Show(ds.Tables[0].Rows.Count + "");
			
		}

		private List<string> GetProjectPaths(List<string> solutionsPaths)
		{
			List<string> projectPaths = new List<string>();

			foreach (string solutionPath in solutionsPaths)
			{
				using (StreamReader r = new StreamReader(solutionPath))
				{
					string line;
					while ((line = r.ReadLine()) != null)
					{
						if (line.Contains(".csproj"))
						{
							string[] portions = line.Split(new string[] { "," }, StringSplitOptions.None);
							foreach (string part in portions)
							{
								if (!part.Contains("SccProjectUniqueName") && part.Contains(".csproj"))
								{
									projectPaths.Add(string.Format("{0}\\{1}", Path.GetDirectoryName(solutionPath), part.Replace("\"", string.Empty).Trim()));
								}
							}
						}
					}
				}
			}

			return projectPaths;
		}

		private List<string> GetSolutionPaths()
		{
			List<string> solutions = new List<string>();
			string currentPath = @"D:\PROJECTS\iTrak\TFS\Current";
			using (StreamReader r = new StreamReader(@"D:\PROJECTS\iTrak\TFS\Current\TeamBuildTypes\All Source Code\TFSBuild.proj"))
			{
				string line;
				while ((line = r.ReadLine()) != null)
				{
					string x = "<SolutionToBuild Include=\"$(BuildProjectFolderPath)/../../";
					if(line.Contains(x))
					{
						solutions.Add(string.Format("{0}\\{1}", currentPath, line.Replace(x, string.Empty).Replace("\">", string.Empty).Replace("\t", string.Empty).Trim()));
					}
				}
			}

			return solutions;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string val = string.Empty;

			using (var form = new VersionInfoEntry())
			{
				var result = form.ShowDialog();
				if (result == DialogResult.OK)
				{
					val = form.DevExpressVersion;            //values preserved after close
				}
			}

			DisplayDevExpressInfo(val);
		}

		private void DisplayDevExpressInfo(string version)
		{
			List<string> solutionsPaths = GetSolutionPaths();
			List<string> projectPaths = GetProjectPaths(solutionsPaths);

			DataSet ds = new DataSet();
			DataTable dt = new DataTable();
			dt.Columns.Add("Project Name", typeof(string));
			dt.Columns.Add("Has Expected Version", typeof(bool));
			dt.Columns.Add("Full Path", typeof(string));
			ds.Tables.Add(dt);
			dt.Rows.Clear();

			foreach (string projectPath in projectPaths)
			{
				using (StreamReader r = new StreamReader(projectPath))
				{
					string line;
					bool found = false;
					while ((line = r.ReadLine()) != null)
					{
						if (line.Contains(string.Format("Version={0}", version)))
						{
							found = true;
							break;
						}
					}

					DataRow dr = dt.NewRow();
					dr[0] = Path.GetFileNameWithoutExtension(projectPath);
					dr[1] = found;
					dr[2] = projectPath;
					ds.Tables[0].Rows.Add(dr);
				}
			}

			this.dataGridView1.DataSource = null;
			this.dataGridView1.DataSource = ds.Tables[0];
			this.dataGridView1.Sort(this.dataGridView1.Columns[1], ListSortDirection.Ascending);
			MessageBox.Show(ds.Tables[0].Rows.Count + "");
			
		}
	}
}
