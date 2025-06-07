using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Atalasoft.Imaging;
using Atalasoft.Imaging.Codec;
using Atalasoft.Imaging.Drawing;
using Atalasoft.Imaging.WinControls;

namespace DotImageDemoImproved
{
	
	#region NewImageParameter
	
	/// <summary>
	/// This class is used for the Property Grid when a new image is requested.
	/// </summary>
	public class NewImageParameter
	{
		private int width               = 400;
		private int height              = 400;
		private Color backColor         = Color.White;
		private PixelFormat imageFormat = PixelFormat.Pixel24bppBgr;
		
		public NewImageParameter(){}
		
		public NewImageParameter(int width, int height, Color backColor, PixelFormat imageFormat)
		{
			this.Width = width;
			this.Height = height;
			this.BackColor = backColor;
			this.imageFormat = imageFormat;
		}
		
		[Description("The width of the image in pixels."), DefaultValue(400)]
		public int Width
		{
			get { return width; }
			set { width = value; }
		}
		
		[Description("The height of the image in pixels."), DefaultValue(400)]
		public int Height
		{
			get { return height; }
			set { height = value; }
		}
		
		[Description("The background color of the image.")]
		public Color BackColor
		{
			get { return backColor; }
			set { backColor = value; }
		}
		
		[Description("The pixel format of the image."), DefaultValue(PixelFormat.Pixel24bppBgr)]
		public PixelFormat ImageFormat
		{
			get { return imageFormat; }
			set { imageFormat = value; }
		}
	}
	
	/// <summary>
	/// This class is used with the Property Grid when a URL is requested.
	/// </summary>
	public class UrlParameter
	{
		private string _url = "";
		private string _username = "";
		private string _password = "";
		private ImageFileFormats _fileFormat;
		
		public UrlParameter(){}
		
		[Description("The HTTP or FTP address of the image.")]
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		public ImageFileFormats FileFormat
		{
			get { return _fileFormat; }
			set { _fileFormat = value; }
		}

		public override string ToString()
		{
			if (_url != null && _url.Length > 0)
			{
				if (_username.Length > 0 && _password.Length > 0)
					return "ftp://" + _username + ":" + _password + "@" + _url;
				else
					return _url;
			}
			else
				return "";
		}

	}
	
	#endregion
	
	#region Program Options
	
	public class ProgramOptions
	{
		bool asynchronous = true;
		int  undoLevels   = 0;
		AntialiasDisplayMode antialias = AntialiasDisplayMode.None;
		AutoZoomMode autoZoom = AutoZoomMode.None;
		ScrollBarVisibility scrollbars = ScrollBarVisibility.Dynamic;
		
		public ProgramOptions(){}
		
		[Description("Runs the Workspace in asynchronous mode."), DefaultValue(true)]
		public bool Asynchronous
		{
			get { return asynchronous; }
			set { asynchronous = value; }
		}
		
		[Description("The number of undo levels to keep in memory."), DefaultValue(5)]
		public int UndoLevels
		{
			get { return undoLevels; }
			set { undoLevels = value; }
		}
		
		[Description("The antialiasing mode, controling how the image is scaled at different zoom levels."), DefaultValue(AntialiasDisplayMode.None)]
		public AntialiasDisplayMode AntialiasDisplay
		{
			get { return antialias; }
			set { antialias = value; }
		}
		
		[Description("A value indicating how the image should be zoomed as the control is resized."), DefaultValue(AutoZoomMode.None)]
		public AutoZoomMode AutoZoom
		{
			get { return autoZoom; }
			set { autoZoom = value; }
		}
		
		[Description("Sets the type of scrollbars to use, if any."), DefaultValue(ScrollBarVisibility.Dynamic)]
		public ScrollBarVisibility ScrollBarStyle
		{
			get { return scrollbars; }
			set { scrollbars = value; }
		}
		
	}
	
	#endregion
	
	public enum FillMode
	{
		Solid,
		Hatch
	}

	public enum ImageFileFormats
	{
		Bmp,
		Gif,
		Jpeg,
		Png,
	}

	public class DrawOutlineParameters : AtalaPen
	{
		private double smoothingLevel = 0;
		public DrawOutlineParameters(){}
		
		public AtalaPen GetPen()
		{
			return (AtalaPen)base.Clone();
		}

		public double SmoothingLevel
		{
			get { return this.smoothingLevel; }
			set { this.smoothingLevel = value; }
		}
	}

	public class DrawSolidParameters : DrawOutlineParameters
	{
		private Color fillColor = Color.Transparent;
		private FillMode fillMode = FillMode.Solid;
		private Hatch hatch = Hatch.Cross;
		
		public DrawSolidParameters(){}
		
		public FillMode FillMode
		{
			get { return fillMode; }
			set { fillMode = value; }
		}

		public Hatch Hatch
		{
			get { return hatch; }
			set { hatch = value; }
		}

		public Color FillColor
		{
			get { return fillColor; }
			set { fillColor = value; }
		}

		public Fill GetFill()
		{
			switch (fillMode)
			{
				case FillMode.Solid:
					return new SolidFill(fillColor);
				case FillMode.Hatch:
					return new HatchedFill(hatch, fillColor);
				default:
					return null;
			}
		}
	}
	
	#region Line Parameters
	
	public class LineParameters : DrawOutlineParameters
	{
		private Point startPos = new Point(10, 10);
		private Point endPos = new Point(100, 100);
		
		public LineParameters(){}
		
		public Point StartPosition
		{
			get { return startPos; }
			set { startPos = value; }
		}
		
		public Point EndPosition
		{
			get { return endPos; }
			set { endPos = value; }
		}
				
	}
	
	#endregion
	
	#region Lines Parameters
	
	public class LinesParameters : DrawOutlineParameters
	{
		private Point[] points = new Point[3] { new Point(10, 10), new Point(100, 250), new Point(10, 250) };
		
		public LinesParameters(){}
		
		public Point[] Points
		{
			get { return points; }
			set { points = value; }
		}
		
	}
	
	#endregion
	
	#region Ellipse Parameters
	
	public class EllipseParameters : DrawSolidParameters
	{
		private Rectangle rectangle = new Rectangle(10, 10, 100, 160);
		
		public EllipseParameters(){}
		
		public Rectangle Rectangle
		{
			get { return rectangle; }
			set { rectangle = value; }
		}

	}
	
	#endregion
	
	#region Rectangle Parameters
	
	public class RectangleParameters : DrawSolidParameters
	{
		private Size rounding = new Size(0,0);
		
		public RectangleParameters(){}

		public Size Rounding
		{
			get { return rounding; }
			set { rounding = value; }
		}
	}
	
	#endregion
	
	#region FloodFill Parameters
	
	public class FloodFillParameters
	{
		private Point position = new Point(50, 50);
		private int tolerance = 0;
		private Color color = Color.Red;
		private bool surface = false;
		private Color fillColor = Color.Transparent;
		
		public FloodFillParameters(){}
		
		public Color Color
		{
			get { return color; }
			set { color = value; }
		}
		
		public Point Position
		{
			get { return position; }
			set { position = value; }
		}
		
		public int Tolerance
		{
			get { return tolerance; }
			set { tolerance = value; }
		}
		
		public bool SurfaceFill
		{
			get { return surface; }
			set { surface = value; }
		}
		
		public Color FillColor
		{
			get { return fillColor; }
			set { fillColor = value; }
		}
	}
	
	#endregion
	
	#region Polygon Parameters
	
	public class PolygonParameters : DrawSolidParameters
	{
		private Point[] points = new Point[3] { new Point(10, 10), new Point(100, 250), new Point(10, 250) };
		
		public PolygonParameters(){}
		
		public Point[] Points
		{
			get { return points; }
			set { points = value; }
		}
	}
	
	#endregion
	
	#region Text Parameters
	
	public class TextParameters : TextFormat
	{
		private string text = "Atalasoft dotImage";
		private Font font = new Font("Verdana", 32);
		private Color fillColor = Color.Black;
		private double smoothingLevel = 0;
		private Point position = Point.Empty;
		private FontQuality _quality = FontQuality.Default;
		
		public TextParameters(){}
		
		public string Text
		{
			get { return text; }
			set { text = value; }
		}
		
		public Font Font
		{
			get { return font; }
			set { font = value; }
		}
		
		[Description("The color of the text.")]
		public Color Color
		{
			get { return fillColor; }
			set { fillColor = value; }
		}

		public double SmoothingLevel
		{
			get { return this.smoothingLevel; }
			set { this.smoothingLevel = value; }
		}

		public TextFormat GetTextFormat()
		{
			TextFormat tf = new TextFormat(base.Alignment, base.Angle, base.InterCharacterSpace);
			return tf;
		}
		
		[Description("The top/left position to place the text.\nThis is ignored if a selection is visible.")]
		public Point Position
		{
			get { return this.position; }
			set { this.position = value; }
		}

		public FontQuality FontQuality
		{
			get { return this._quality; }
			set { this._quality = value; }
		}

	}
	
	#endregion
	
	public class PointParameter
	{
		private Point pt = new Point(0, 0);
		
		public PointParameter(){}
		
		public Point Position
		{
			get { return pt; }
			set { pt = value; }
		}
	}
	
	public class TlaPassword
	{
		private string password = "";
		
		public TlaPassword(){}
		
		[Description("The password for this TLA file.  Leave black if there is no password.")]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}
	}
	
	public class ImageFileFormat
	{
		private ImageType saveType = ImageType.Jpeg;

		public ImageFileFormat(){}
		
		public ImageType ImageFormat
		{
			get { return this.saveType; }
			set { this.saveType = value; }
		}

	}

}
