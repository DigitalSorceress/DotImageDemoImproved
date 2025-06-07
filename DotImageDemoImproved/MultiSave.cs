using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DotImageDemoImproved
{
	/// <summary>
	/// Summary description for MultiSave.
	/// </summary>
	public class MultiSave : System.Windows.Forms.Form
	{
		internal ArrayList images = new ArrayList();
		
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		internal System.Windows.Forms.ListBox listImages;
		internal System.Windows.Forms.PropertyGrid propertyGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MultiSave()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.listImages = new System.Windows.Forms.ListBox();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.btnCancel,
																				 this.btnSave});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 212);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(427, 54);
			this.panel1.TabIndex = 0;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(258, 11);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(96, 30);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// btnSave
			// 
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.Location = new System.Drawing.Point(72, 11);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(96, 30);
			this.btnSave.TabIndex = 0;
			this.btnSave.Text = "Save";
			// 
			// listImages
			// 
			this.listImages.Dock = System.Windows.Forms.DockStyle.Left;
			this.listImages.Name = "listImages";
			this.listImages.Size = new System.Drawing.Size(158, 212);
			this.listImages.TabIndex = 1;
			this.listImages.SelectedIndexChanged += new System.EventHandler(this.listImages_SelectedIndexChanged);
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(158, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid1.Size = new System.Drawing.Size(269, 212);
			this.propertyGrid1.TabIndex = 2;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// MultiSave
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(427, 266);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.propertyGrid1,
																		  this.listImages,
																		  this.panel1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "MultiSave";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "MultiSave";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void listImages_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listImages.SelectedIndex == -1) return;
			this.propertyGrid1.SelectedObject = this.images[listImages.SelectedIndex];
		}
	}
}
