using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Atalasoft.Imaging.ImageProcessing;
using Atalasoft.Imaging.Drawing;
using Atalasoft.Imaging;

namespace DotImageDemoImproved
{
	/// <summary>
	/// Summary description for Histogram.
	/// </summary>
	public class Histogram : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private int[] hist;

		public Histogram()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			base.SetStyle(ControlStyles.AllPaintingInWmPaint | 
				ControlStyles.DoubleBuffer | 
				ControlStyles.UserPaint, true);

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
			// 
			// Histogram
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(248, 66);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "Histogram";
			this.Text = "Histogram";
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.Histogram_Layout);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Histogram_Paint);

		}
		#endregion

		public void SetHistogram(AtalaImage image)
		{
			Atalasoft.Imaging.ImageProcessing.Histogram getHist = new Atalasoft.Imaging.ImageProcessing.Histogram(image);

			switch (image.PixelFormat)
			{
				case PixelFormat.Pixel1bppIndexed:
					if (AtalaImage.Edition == LicenseEdition.Document)
						this.hist = getHist.GetDocumentHistogram();
					else
						this.hist = new int[256];
					break;
				case PixelFormat.Pixel8bppGrayscale:
				case PixelFormat.Pixel16bppGrayscaleAlpha:
				case PixelFormat.Pixel16bppGrayscale:
					this.hist = getHist.GetChannelHistogram(0);
					break;
				case PixelFormat.Pixel24bppBgr:
				case PixelFormat.Pixel32bppBgr:
				case PixelFormat.Pixel32bppBgra:
				case PixelFormat.Pixel48bppBgr:
				case PixelFormat.Pixel64bppBgra:
					this.hist = getHist.GetBrightnessHistogram();
					break;
				default:  //incompatible
					this.hist = new int[256];
					break;
			}

			//draw histogram
			this.Invalidate();

		}

		private void Histogram_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (hist != null)
			{
				//get max value in histogram
				int max = 0;
				foreach (int val in hist)
					if (val > max)
						max = val;

				// This will happen if the pixel format is not compatible.
				if (max == 0) max = 1;

				//clear display
				e.Graphics.FillRectangle(new SolidBrush(Color.White), this.ClientRectangle);

				int w = this.Width / hist.Length;
				if (w == 0)
					w = 1;

				//draw histogram
				for (int i = 0; i < hist.Length; i++)
				{
					e.Graphics.DrawLine(new Pen(Color.Black, w), new Point(i * w, this.ClientSize.Height), new Point(i * w, this.ClientSize.Height - ((int)(((double)hist[i] / max) * this.ClientSize.Height))));
				}
			}
		}

		private void Histogram_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			this.Invalidate();
		}
	}

	public enum HistogramType
	{
		Brightness,
		Channel,
		Document
	}

	public class HistogramOptions
	{
		private HistogramType type = HistogramType.Brightness;
		private Atalasoft.Imaging.ImageProcessing.ChannelFlags channel = Atalasoft.Imaging.ImageProcessing.ChannelFlags.Channel1;

		public HistogramOptions()
		{
			
		}

		public HistogramType Type
		{
			get { return this.type; }
			set { this.type = value; }
		}

		public Atalasoft.Imaging.ImageProcessing.ChannelFlags Channel
		{
			get { return this.channel; }
			set { this.channel = value; }
		}

	}
	

}
