using Atalasoft.Annotate.UI;
using Atalasoft.Imaging;
using Atalasoft.Imaging.Codec;
using Atalasoft.Imaging.Codec.Pdf;
using Atalasoft.Imaging.Drawing;
using Atalasoft.Imaging.ImageProcessing;
using Atalasoft.Imaging.ImageProcessing.Channels;
using Atalasoft.Imaging.ImageProcessing.Document;
using Atalasoft.Imaging.ImageProcessing.Effects;
using Atalasoft.Imaging.ImageProcessing.Fft;
using Atalasoft.Imaging.ImageProcessing.Filters;
using Atalasoft.Imaging.ImageProcessing.Transforms;
using Atalasoft.Imaging.Metadata;
using Atalasoft.Imaging.WinControls;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace DotImageDemoImproved
{
	/// <summary>
	/// Main form for the Atalasoft dotImage Demo.
	/// </summary>
	public class FormMain : System.Windows.Forms.Form
	{	
		#region Private Vars
		// Image Information.
		private ArrayList _tempFiles = new ArrayList();
		private ArrayList _images = new ArrayList();
		private int _currentIndex;
		private bool _firstLoad = true;
		private string currentFile         = "";
		private JpegMarkerCollection jpegMarkers = null;
		private IptcCollection iptcItems = null;
		private ExifCollection exifItems = null;
		private ComTextCollection comItems = null;
		private TransformChainCommand transformChain = null;
		private System.Windows.Forms.ToolBarButton tbSelectRectangle;
		private System.Windows.Forms.ToolBarButton tbSelectEllipse;
		private Histogram histogram = null;
		private bool chainTransforms = false;
		private bool performingOpen = false;
		private bool isProcessError;
		
		private enum DrawMenuMode
		{
			None      = 0,
			Line      = 1,
			Lines     = 2,
			Ellipse   = 3,
			Rectangle = 4,
			FloodFill = 5,
			Freehand  = 6,
			Polygon   = 7,
			Text      = 8
		}

        bool _shift = false;
        bool _ctrl = false;
        #endregion

        #region Designer Vars
        private System.ComponentModel.IContainer components;
		private System.Windows.Forms.StatusBarPanel statusBarPosition;
		private System.Windows.Forms.StatusBarPanel statusBarMessage;
		private System.Windows.Forms.StatusBar statusInfo;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton tbArrow;
		private System.Windows.Forms.ToolBarButton tbPan;
		private System.Windows.Forms.StatusBarPanel statusBarProgress;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.ToolBarButton tbOpen;
		private System.Windows.Forms.ToolBarButton tbSave;
		private System.Windows.Forms.ToolBarButton tbSep;
		private System.Windows.Forms.ToolBarButton tbUndo;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton tbMagnifier;
		private System.Windows.Forms.ToolBarButton tbZoom;
		private System.Windows.Forms.ToolBarButton tbZoomSelection;
		private System.Windows.Forms.ToolBarButton tbRedo;
				
		// Various State Data
		private bool is_Disposed = false;
		private Color currentTextBackColor = Color.Transparent;
		private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
		private System.Windows.Forms.PrintDialog printDialog1;
		private Atalasoft.Imaging.WinControls.ImagePrintDocument imagePrintDocument1;
		private Atalasoft.Imaging.WinControls.EllipseRubberband ellipseRubberband;
		private Atalasoft.Imaging.WinControls.RectangleSelection rectangleSelection;
		private Atalasoft.Imaging.WinControls.WorkspaceViewer Viewer;
		private Atalasoft.Imaging.WinControls.RectangleRubberband rectangleDraw;
		private Atalasoft.Imaging.WinControls.EllipseRubberband ellipseDraw;
		private Atalasoft.Imaging.WinControls.LineRubberband lineDraw;
		private DrawMenuMode drawMode = DrawMenuMode.None;
		private double canvasSmoothing = 0;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuFileNew;
		private System.Windows.Forms.MenuItem menuFileOpen;
		private System.Windows.Forms.MenuItem menuFileOpenFromURL;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuFileNoise;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuFileSaveAs;
		private System.Windows.Forms.MenuItem menuFileSaveFTP;
		private System.Windows.Forms.MenuItem menuFilePageSetup;
		private System.Windows.Forms.MenuItem menuFilePrintImage;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuFileExit;
		private System.Windows.Forms.MenuItem menuEditUndo;
		private System.Windows.Forms.MenuItem menuEditRedo;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuEditCut;
		private System.Windows.Forms.MenuItem menuEditCopy;
		private System.Windows.Forms.MenuItem menuEditPaste;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuEditOptions;
		private System.Windows.Forms.MenuItem menuImageInformation;
		private System.Windows.Forms.MenuItem menuImageChangePixelFormat;
		private System.Windows.Forms.MenuItem menuImageShowHistogram;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuImageExif;
		private System.Windows.Forms.MenuItem menuImageZoom;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuImageIptc;
		private System.Windows.Forms.MenuItem menuImageZoom25;
		private System.Windows.Forms.MenuItem menuImageZoom50;
		private System.Windows.Forms.MenuItem menuImageZoom100;
		private System.Windows.Forms.MenuItem menuImageZoom200;
		private System.Windows.Forms.MenuItem menuImageZoom500;
		private System.Windows.Forms.MenuItem menuImageZoom1000;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuEdit;
		private System.Windows.Forms.MenuItem menuImage;
		private System.Windows.Forms.MenuItem menuDraw;
		private System.Windows.Forms.MenuItem menuCommands;
		private System.Windows.Forms.MenuItem menuChannels;
		private System.Windows.Forms.MenuItem menuEffects;
		private System.Windows.Forms.MenuItem menuFilters;
		private System.Windows.Forms.MenuItem menuTransforms;
		private System.Windows.Forms.MenuItem menuDocument;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.MenuItem menuFlip;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuPush;
		private System.Windows.Forms.MenuItem menuQuadrilateralWarp;
		private System.Windows.Forms.MenuItem menuSkew;
		private System.Windows.Forms.MenuItem menuItem23;
		private System.Windows.Forms.MenuItem menuRotate;
		private System.Windows.Forms.MenuItem menuResample;
		private System.Windows.Forms.MenuItem menuCrop;
		private System.Windows.Forms.MenuItem menuAutoCrop;
		private System.Windows.Forms.MenuItem menuItem28;
		private System.Windows.Forms.MenuItem menuOverlay;
		private System.Windows.Forms.MenuItem menuResizeCanvas;
		private System.Windows.Forms.MenuItem menuFlipHorizontal;
		private System.Windows.Forms.MenuItem menuFlipVertical;
		private System.Windows.Forms.MenuItem menuChannelsCombine;
		private System.Windows.Forms.MenuItem menuChannelsReplace;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem24;
		private System.Windows.Forms.MenuItem menuItem46;
		private System.Windows.Forms.MenuItem menuItem58;
		private System.Windows.Forms.MenuItem menuItem70;
		private System.Windows.Forms.MenuItem menuItem73;
		private System.Windows.Forms.MenuItem menuItem77;
		private System.Windows.Forms.MenuItem menuItem94;
		private System.Windows.Forms.MenuItem menuItem103;
		private System.Windows.Forms.MenuItem menuChannelsSplit;
		private System.Windows.Forms.MenuItem menuChannelsAdjust;
		private System.Windows.Forms.MenuItem menuChannelsAdjustHsl;
		private System.Windows.Forms.MenuItem menuChannelsApplyLut;
		private System.Windows.Forms.MenuItem menuChannelsInvert;
		private System.Windows.Forms.MenuItem menuChannelsShift;
		private System.Windows.Forms.MenuItem menuChannelsSwap;
		private System.Windows.Forms.MenuItem menuChannelsFlattenAlpha;
		private System.Windows.Forms.MenuItem menuChannelsAphaColor;
		private System.Windows.Forms.MenuItem menuChannelsAlphaMask;
		private System.Windows.Forms.MenuItem menuChannelsAlphaValue;
		private System.Windows.Forms.MenuItem menuEffectsAdjustTint;
		private System.Windows.Forms.MenuItem menuEffectsBevelEdge;
		private System.Windows.Forms.MenuItem menuEffectsCrackle;
		private System.Windows.Forms.MenuItem menuEffectsDropShadow;
		private System.Windows.Forms.MenuItem menuEffectsFingerPrint;
		private System.Windows.Forms.MenuItem menuEffectsFloodFill;
		private System.Windows.Forms.MenuItem menuEffectsGamma;
		private System.Windows.Forms.MenuItem menuEffectsGauzy;
		private System.Windows.Forms.MenuItem menuEffectsHalftone;
		private System.Windows.Forms.MenuItem menuEffectsMosaic;
		private System.Windows.Forms.MenuItem menuEffectsPosterize;
		private System.Windows.Forms.MenuItem menuReduceColors;
		private System.Windows.Forms.MenuItem menuEffectsReplaceColor;
		private System.Windows.Forms.MenuItem menuEffectsSolarize;
		private System.Windows.Forms.MenuItem menuEffectsStipple;
		private System.Windows.Forms.MenuItem menuEffectsTintGrayscale;
		private System.Windows.Forms.MenuItem menuEffectsBrightnessHistogramEqualize;
		private System.Windows.Forms.MenuItem menuEffectsBrightnessHistogramStretch;
		private System.Windows.Forms.MenuItem menuHistogramEqualize;
		private System.Windows.Forms.MenuItem menuEffectsHistogramStretch;
		private System.Windows.Forms.MenuItem menuFiltersBrightnessContrast;
		private System.Windows.Forms.MenuItem menuFiltersSaturation;
		private System.Windows.Forms.MenuItem menuFiltersBlur;
		private System.Windows.Forms.MenuItem menuFiltersGaussianBlur;
		private System.Windows.Forms.MenuItem menuAdaptiveUnsharpMask;
		private System.Windows.Forms.MenuItem menuEffectsUnsharpMask;
		private System.Windows.Forms.MenuItem menuFiltersSharpen;
		private System.Windows.Forms.MenuItem menuFiltersAddNoise;
		private System.Windows.Forms.MenuItem menuFiltersEmboss;
		private System.Windows.Forms.MenuItem menuFiltersIntensify;
		private System.Windows.Forms.MenuItem menuFiltersHighPass;
		private System.Windows.Forms.MenuItem menuFiltersMaximum;
		private System.Windows.Forms.MenuItem menuFiltersMean;
		private System.Windows.Forms.MenuItem menuFiltersMedian;
		private System.Windows.Forms.MenuItem menuFiltersMidpoint;
		private System.Windows.Forms.MenuItem menuFiltersMinimum;
		private System.Windows.Forms.MenuItem menuFiltersMorphological;
		private System.Windows.Forms.MenuItem menuMorphoErosion;
		private System.Windows.Forms.MenuItem menuMorphoDilation;
		private System.Windows.Forms.MenuItem menuMorphoOpen;
		private System.Windows.Forms.MenuItem menuMorphoClose;
		private System.Windows.Forms.MenuItem menuMorphoGradient;
		private System.Windows.Forms.MenuItem menuFiltersThreshold;
		private System.Windows.Forms.MenuItem menuMorphoTophat;
		private System.Windows.Forms.MenuItem menuFiltersConvolutionFilter;
		private System.Windows.Forms.MenuItem menuFiltersConvolutionMatrix;
		private System.Windows.Forms.MenuItem menuFiltersCannyEdgeDetector;
		private System.Windows.Forms.MenuItem menuTransformsChain;
		private System.Windows.Forms.MenuItem menuTransformsApply;
		private System.Windows.Forms.MenuItem menuTransformsBumpMap;
		private System.Windows.Forms.MenuItem menuTransformsElliptical;
		private System.Windows.Forms.MenuItem menuTransformsLens;
		private System.Windows.Forms.MenuItem menuLineSlice;
		private System.Windows.Forms.MenuItem menuTransformsMarble;
		private System.Windows.Forms.MenuItem menuTransformsOffset;
		private System.Windows.Forms.MenuItem menuTransformsPerlin;
		private System.Windows.Forms.MenuItem menuTransformsPinch;
		private System.Windows.Forms.MenuItem menuTransformsPolygon;
		private System.Windows.Forms.MenuItem menuTransformsRandom;
		private System.Windows.Forms.MenuItem menuTransformsRipple;
		private System.Windows.Forms.MenuItem menuTransformsSpin;
		private System.Windows.Forms.MenuItem menuTransformSpinWave;
		private System.Windows.Forms.MenuItem menuTransformsWave;
		private System.Windows.Forms.MenuItem menuTransformsWow;
		private System.Windows.Forms.MenuItem menuTransformsZigZag;
		private System.Windows.Forms.MenuItem menuTransformsUser;
		private System.Windows.Forms.MenuItem menuDocumentAutoDeskew;
		private System.Windows.Forms.MenuItem menuDocumentMedian;
		private System.Windows.Forms.MenuItem menuDocumentMorphological;
		private System.Windows.Forms.MenuItem menuBinaryErosion;
		private System.Windows.Forms.MenuItem menuBinaryDilation;
		private System.Windows.Forms.MenuItem menuBinaryOpen;
		private System.Windows.Forms.MenuItem menuBinaryClose;
		private System.Windows.Forms.MenuItem menuBinaryBoundary;
		private System.Windows.Forms.MenuItem menuDocumentThinning;
		private System.Windows.Forms.MenuItem menuDocumentHitOrMiss;
		private System.Windows.Forms.MenuItem menuOverlayNormal;
		private System.Windows.Forms.MenuItem menuOverlayMasked;
		private System.Windows.Forms.MenuItem menuOverlayMerged;
		private System.Windows.Forms.MenuItem menuDrawLine;
		private System.Windows.Forms.MenuItem menuDrawLines;
		private System.Windows.Forms.MenuItem menuDrawRectangle;
		private System.Windows.Forms.MenuItem menuItem21;
		private System.Windows.Forms.MenuItem menuDrawEllipse;
		private System.Windows.Forms.MenuItem menuDrawFreehand;
		private System.Windows.Forms.MenuItem menuDrawPolygon;
		private System.Windows.Forms.MenuItem menuDrawText;
		private System.Windows.Forms.MenuItem menuDrawSetBackcolor;
		private System.Windows.Forms.MenuItem menuDrawClearBackcolor;
		private System.Windows.Forms.MenuItem menuFileDecoders;
		private System.Windows.Forms.MenuItem menuFileJPEG;
		private System.Windows.Forms.MenuItem menuFileTLA;
		private System.Windows.Forms.MenuItem menuFilePNG;
		private System.Windows.Forms.MenuItem menuFileWMF;
		private System.Windows.Forms.MenuItem menuFiltersDespeckle;
		private System.Windows.Forms.MenuItem menuFiltersEdge;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuEffectsDeInterlace;
		private System.Windows.Forms.MenuItem menuFiltersDustScratchRemoval;
		private System.Windows.Forms.MenuItem menuEffectsOilPaint;
		private System.Windows.Forms.MenuItem menuEffectsWatercolorTint;
		private System.Windows.Forms.MenuItem menuDocumentDespeckle;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuFftBandPass;
		private System.Windows.Forms.MenuItem menuFftButterworthHighBoost;
		private System.Windows.Forms.MenuItem menuFftButterworthHighPass;
		private System.Windows.Forms.MenuItem menuFftButterworthLowPass;
		private System.Windows.Forms.MenuItem menuFftGaussianHighBoost;
		private System.Windows.Forms.MenuItem menuFftGaussianHighPass;
		private System.Windows.Forms.MenuItem menuFftGaussianLowPass;
		private System.Windows.Forms.MenuItem menuFftIdealHighPass;
		private System.Windows.Forms.MenuItem menuFftIdealLowPass;
		private System.Windows.Forms.MenuItem menuFftInversePower;
		private System.Windows.Forms.MenuItem menuEffectsRedEyeRemoval;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuEffectsLevels;
		private System.Windows.Forms.MenuItem menuEffectsAutoLevels;
		private System.Windows.Forms.MenuItem menuEffectsAutoContrast;
		private System.Windows.Forms.MenuItem menuEffectsAutoColor;
		private System.Windows.Forms.MenuItem menuEffectsAutoWhiteBalance;
		private System.Windows.Forms.MenuItem menuImageZoom132;
		private System.Windows.Forms.MenuItem menuImageZoom116;
		private System.Windows.Forms.MenuItem menuImageZoom18;
		private System.Windows.Forms.MenuItem menuImageZoom31;
		private System.Windows.Forms.MenuItem menuThresholding;
		private System.Windows.Forms.MenuItem menuThresholdAdaptive;
		private System.Windows.Forms.MenuItem menuThresholdGlobal;
		private System.Windows.Forms.MenuItem menuBorderRemoval;
		private System.Windows.Forms.MenuItem menuHelp;
		private System.Windows.Forms.MenuItem menuItem15;
		private Point[] polygonPoints;
		private System.Windows.Forms.StatusBarPanel statusBarLoadTime;
		private System.Windows.Forms.MenuItem menuFilePDF;
		private System.Windows.Forms.MenuItem menuEffectsRoundedBevel;
		private System.Windows.Forms.MenuItem menuEffectsSaturation;
		private System.Windows.Forms.ComboBox cboFrameIndex;
        private MenuItem menuItem10;
        private MenuItem menuItemAutomaticDocumentNegation;
        private MenuItem menuItemBinarySegmentation;
        private MenuItem menuItemBlackBorderCrop;
        private MenuItem menuItemBlankPageDetection;
        private MenuItem menuItemBlobRemoval;
        private MenuItem menuItemBorderRemoval;
        private MenuItem menuItemDeskew;
        private MenuItem menuItemDespeckle;
        private MenuItem menuItemHalftoneRemoval;
        private MenuItem menuItemHolePunchRemoval;
        private MenuItem menuItemInvertedTextCorrection;
        private MenuItem menuItemLineRemoval;
        private MenuItem menuItemSpeckRemoval;
        private MenuItem menuItemWhiteMarginCrop;
        private MenuItem menuThresholdDynamic;
        private MenuItem menuDithering;
        private MenuItem menuView;
        private MenuItem menuViewWidth;
        private MenuItem menuViewHeight;
        private MenuItem menuViewBest;
        private MenuItem menuView100;
        private MenuItem menuView50;
        private MenuItem menuView150;
        private int _startTick;

		#endregion

		public FormMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			//hook up event code to show a message when the pixel format changes
			AtalaImage.ChangePixelFormat += new PixelFormatChangeEventHandler(AtalaImage_ChangePixelFormat);

			AtalaDemos.HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders);

			if (AtalaImage.Edition != LicenseEdition.Document)
				this.menuDocument.Enabled = false;


            // the next five items are specifically to enable mouse wheel scrolling and allow us to do horizontal scrolling oerride
            // ====================================================================
            KeyPreview = true;
            MouseWheel += FormMain_MouseWheel;
            KeyDown += FormMain_KeyDown;
            KeyUp += FormMain_KeyUp;
            Viewer.MouseWheelScrolling = true;
            // ====================================================================

            // Scroll with the mouse wheel.
            //this.Viewer.MouseWheel += new MouseEventHandler(Viewer_MouseWheel);
		}

        #region MouseScrolling Modifiers
        /// <summary>
        /// This is going to be ONLY for horizontal scrolling
        /// we have had to carefully enable.disable native mouse wheel scrolling in the viewer becasue that scrolling will
        /// override/mess with horizontal if its on
        /// 
        /// so we have disabled the built in mouse wheel scrolling and now we can manually horizontally scroll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormMain_MouseWheel(object sender, MouseEventArgs e)
        {
            // we could maybe be more elegant.. 
            // Unfortunately, there's no documentAnnotationViewer1.Iamgecontrol.Focused property 
            // so if we wanted to be more tight with this, we'd have to get the area of the image control
            // and make our own decision about whether the mouse was inside or outside of it
            // still it works
            if (_shift)
            {
                int oldX = Viewer.ScrollPosition.X;
                int oldY = Viewer.ScrollPosition.Y;

                // Ok, we've got the old values - calculate the new X
                int newX = oldX + e.Delta;

                // this is the "safety dance" to prevent us from scrolling beyond the image
                if (newX > 0)
                {
                    newX = 0;
                }
                else if (newX > Viewer.Image.Width)
                {
                    newX = Viewer.Image.Width;
                }

                // finally we set the new X (and keep the old Y as we are horizontal scrolling only)
                Viewer.ScrollPosition = new Point(newX, oldY);
            }
            if (_ctrl)
            {
                if (Viewer.AutoZoom != AutoZoomMode.None)
                {
                    Viewer.AutoZoom = AutoZoomMode.None;
                }

                int delta = e.Delta;
                double modifier = 1;
                if (e.Delta > 0)
                {
                    modifier = Math.Abs((double)e.Delta / (double)100);
                }
                else
                {
                    modifier = Math.Abs((double)100 / (double)e.Delta);
                }

                if (modifier == 0)
                {
                    modifier = 1.0;
                }
                Viewer.Zoom = (Viewer.Zoom * modifier);
            }
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Shift)
            {
                //Console.WriteLine("SHIFT UP!");
                _shift = false;
                // we need to toggle the mouse wheel scrolling of the image control on
                // when shift isn't being pressed
                Viewer.MouseWheelScrolling = true;
            }
            if (!e.Control)
            {
                _ctrl = false;
                Viewer.MouseWheelScrolling = true;
            }
            if (e.KeyCode == Keys.Space)
            {
                ClearMouseTools();
                tbArrow.Pushed = true;
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                //Console.WriteLine("SHIFT DOWN!");
                _shift = true;
                // need this to prevent our horizontal scrolling from interfering with defualt
                Viewer.MouseWheelScrolling = false;
            }
            if (e.Control)
            {
                _ctrl = true;
                Viewer.MouseWheelScrolling = false;
            }
            if (e.KeyCode == Keys.Space)
            {
                if (!tbPan.Pushed)
                {
                    ClearMouseTools();
                    tbPan.Pushed = true;
                    Viewer.MouseTool = MouseToolType.Pan;
                }
            }
        }
        #endregion MouseScrolling Modifiers

        #region Windows Form Designer generated code
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
		{
			foreach (string file in _tempFiles)
				File.Delete(file);
			
			_tempFiles.Clear();

			if (disposing)
			{
				if (!is_Disposed) 
				{
					is_Disposed = true;
				}
				
				if (components != null) 
				{
					components.Dispose();
				}
			}

			base.Dispose( disposing );
		}

		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.statusInfo = new System.Windows.Forms.StatusBar();
            this.statusBarPosition = new System.Windows.Forms.StatusBarPanel();
            this.statusBarLoadTime = new System.Windows.Forms.StatusBarPanel();
            this.statusBarMessage = new System.Windows.Forms.StatusBarPanel();
            this.statusBarProgress = new System.Windows.Forms.StatusBarPanel();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.tbOpen = new System.Windows.Forms.ToolBarButton();
            this.tbSave = new System.Windows.Forms.ToolBarButton();
            this.tbSep = new System.Windows.Forms.ToolBarButton();
            this.tbUndo = new System.Windows.Forms.ToolBarButton();
            this.tbRedo = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.tbArrow = new System.Windows.Forms.ToolBarButton();
            this.tbSelectRectangle = new System.Windows.Forms.ToolBarButton();
            this.tbSelectEllipse = new System.Windows.Forms.ToolBarButton();
            this.tbPan = new System.Windows.Forms.ToolBarButton();
            this.tbMagnifier = new System.Windows.Forms.ToolBarButton();
            this.tbZoom = new System.Windows.Forms.ToolBarButton();
            this.tbZoomSelection = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.rectangleSelection = new Atalasoft.Imaging.WinControls.RectangleSelection();
            this.Viewer = new Atalasoft.Imaging.WinControls.WorkspaceViewer();
            this.imagePrintDocument1 = new Atalasoft.Imaging.WinControls.ImagePrintDocument();
            this.ellipseRubberband = new Atalasoft.Imaging.WinControls.EllipseRubberband();
            this.rectangleDraw = new Atalasoft.Imaging.WinControls.RectangleRubberband();
            this.ellipseDraw = new Atalasoft.Imaging.WinControls.EllipseRubberband();
            this.lineDraw = new Atalasoft.Imaging.WinControls.LineRubberband();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuFileNew = new System.Windows.Forms.MenuItem();
            this.menuFileOpen = new System.Windows.Forms.MenuItem();
            this.menuFileOpenFromURL = new System.Windows.Forms.MenuItem();
            this.menuFileDecoders = new System.Windows.Forms.MenuItem();
            this.menuFileJPEG = new System.Windows.Forms.MenuItem();
            this.menuFilePDF = new System.Windows.Forms.MenuItem();
            this.menuFilePNG = new System.Windows.Forms.MenuItem();
            this.menuFileTLA = new System.Windows.Forms.MenuItem();
            this.menuFileWMF = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuFileNoise = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.MenuItem();
            this.menuFileSaveFTP = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuFilePageSetup = new System.Windows.Forms.MenuItem();
            this.menuFilePrintImage = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuFileExit = new System.Windows.Forms.MenuItem();
            this.menuView = new System.Windows.Forms.MenuItem();
            this.menuViewWidth = new System.Windows.Forms.MenuItem();
            this.menuViewBest = new System.Windows.Forms.MenuItem();
            this.menuViewHeight = new System.Windows.Forms.MenuItem();
            this.menuView100 = new System.Windows.Forms.MenuItem();
            this.menuView50 = new System.Windows.Forms.MenuItem();
            this.menuView150 = new System.Windows.Forms.MenuItem();
            this.menuEdit = new System.Windows.Forms.MenuItem();
            this.menuEditUndo = new System.Windows.Forms.MenuItem();
            this.menuEditRedo = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuEditCut = new System.Windows.Forms.MenuItem();
            this.menuEditCopy = new System.Windows.Forms.MenuItem();
            this.menuEditPaste = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuEditOptions = new System.Windows.Forms.MenuItem();
            this.menuImage = new System.Windows.Forms.MenuItem();
            this.menuImageInformation = new System.Windows.Forms.MenuItem();
            this.menuImageChangePixelFormat = new System.Windows.Forms.MenuItem();
            this.menuImageShowHistogram = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuImageExif = new System.Windows.Forms.MenuItem();
            this.menuImageIptc = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom = new System.Windows.Forms.MenuItem();
            this.menuImageZoom132 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom116 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom18 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom25 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom50 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom100 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom200 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom500 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom1000 = new System.Windows.Forms.MenuItem();
            this.menuImageZoom31 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuDraw = new System.Windows.Forms.MenuItem();
            this.menuDrawLine = new System.Windows.Forms.MenuItem();
            this.menuDrawLines = new System.Windows.Forms.MenuItem();
            this.menuDrawRectangle = new System.Windows.Forms.MenuItem();
            this.menuDrawEllipse = new System.Windows.Forms.MenuItem();
            this.menuDrawPolygon = new System.Windows.Forms.MenuItem();
            this.menuDrawFreehand = new System.Windows.Forms.MenuItem();
            this.menuItem21 = new System.Windows.Forms.MenuItem();
            this.menuDrawText = new System.Windows.Forms.MenuItem();
            this.menuDrawSetBackcolor = new System.Windows.Forms.MenuItem();
            this.menuDrawClearBackcolor = new System.Windows.Forms.MenuItem();
            this.menuCommands = new System.Windows.Forms.MenuItem();
            this.menuChannels = new System.Windows.Forms.MenuItem();
            this.menuChannelsCombine = new System.Windows.Forms.MenuItem();
            this.menuChannelsReplace = new System.Windows.Forms.MenuItem();
            this.menuChannelsSplit = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuChannelsAdjust = new System.Windows.Forms.MenuItem();
            this.menuChannelsAdjustHsl = new System.Windows.Forms.MenuItem();
            this.menuChannelsApplyLut = new System.Windows.Forms.MenuItem();
            this.menuChannelsInvert = new System.Windows.Forms.MenuItem();
            this.menuChannelsShift = new System.Windows.Forms.MenuItem();
            this.menuChannelsSwap = new System.Windows.Forms.MenuItem();
            this.menuItem24 = new System.Windows.Forms.MenuItem();
            this.menuChannelsFlattenAlpha = new System.Windows.Forms.MenuItem();
            this.menuChannelsAphaColor = new System.Windows.Forms.MenuItem();
            this.menuChannelsAlphaMask = new System.Windows.Forms.MenuItem();
            this.menuChannelsAlphaValue = new System.Windows.Forms.MenuItem();
            this.menuEffects = new System.Windows.Forms.MenuItem();
            this.menuEffectsAdjustTint = new System.Windows.Forms.MenuItem();
            this.menuEffectsBevelEdge = new System.Windows.Forms.MenuItem();
            this.menuEffectsCrackle = new System.Windows.Forms.MenuItem();
            this.menuEffectsDeInterlace = new System.Windows.Forms.MenuItem();
            this.menuEffectsDropShadow = new System.Windows.Forms.MenuItem();
            this.menuEffectsFingerPrint = new System.Windows.Forms.MenuItem();
            this.menuEffectsFloodFill = new System.Windows.Forms.MenuItem();
            this.menuEffectsGamma = new System.Windows.Forms.MenuItem();
            this.menuEffectsGauzy = new System.Windows.Forms.MenuItem();
            this.menuEffectsHalftone = new System.Windows.Forms.MenuItem();
            this.menuEffectsMosaic = new System.Windows.Forms.MenuItem();
            this.menuEffectsOilPaint = new System.Windows.Forms.MenuItem();
            this.menuEffectsPosterize = new System.Windows.Forms.MenuItem();
            this.menuReduceColors = new System.Windows.Forms.MenuItem();
            this.menuEffectsReplaceColor = new System.Windows.Forms.MenuItem();
            this.menuEffectsRoundedBevel = new System.Windows.Forms.MenuItem();
            this.menuEffectsSaturation = new System.Windows.Forms.MenuItem();
            this.menuEffectsSolarize = new System.Windows.Forms.MenuItem();
            this.menuEffectsStipple = new System.Windows.Forms.MenuItem();
            this.menuEffectsTintGrayscale = new System.Windows.Forms.MenuItem();
            this.menuEffectsWatercolorTint = new System.Windows.Forms.MenuItem();
            this.menuItem46 = new System.Windows.Forms.MenuItem();
            this.menuEffectsBrightnessHistogramEqualize = new System.Windows.Forms.MenuItem();
            this.menuEffectsBrightnessHistogramStretch = new System.Windows.Forms.MenuItem();
            this.menuHistogramEqualize = new System.Windows.Forms.MenuItem();
            this.menuEffectsHistogramStretch = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuEffectsRedEyeRemoval = new System.Windows.Forms.MenuItem();
            this.menuEffectsLevels = new System.Windows.Forms.MenuItem();
            this.menuEffectsAutoLevels = new System.Windows.Forms.MenuItem();
            this.menuEffectsAutoContrast = new System.Windows.Forms.MenuItem();
            this.menuEffectsAutoColor = new System.Windows.Forms.MenuItem();
            this.menuEffectsAutoWhiteBalance = new System.Windows.Forms.MenuItem();
            this.menuFilters = new System.Windows.Forms.MenuItem();
            this.menuFiltersBrightnessContrast = new System.Windows.Forms.MenuItem();
            this.menuFiltersSaturation = new System.Windows.Forms.MenuItem();
            this.menuFiltersBlur = new System.Windows.Forms.MenuItem();
            this.menuFiltersGaussianBlur = new System.Windows.Forms.MenuItem();
            this.menuAdaptiveUnsharpMask = new System.Windows.Forms.MenuItem();
            this.menuEffectsUnsharpMask = new System.Windows.Forms.MenuItem();
            this.menuFiltersSharpen = new System.Windows.Forms.MenuItem();
            this.menuItem58 = new System.Windows.Forms.MenuItem();
            this.menuFiltersAddNoise = new System.Windows.Forms.MenuItem();
            this.menuFiltersDespeckle = new System.Windows.Forms.MenuItem();
            this.menuFiltersEmboss = new System.Windows.Forms.MenuItem();
            this.menuFiltersIntensify = new System.Windows.Forms.MenuItem();
            this.menuFiltersHighPass = new System.Windows.Forms.MenuItem();
            this.menuFiltersMaximum = new System.Windows.Forms.MenuItem();
            this.menuFiltersMean = new System.Windows.Forms.MenuItem();
            this.menuFiltersMedian = new System.Windows.Forms.MenuItem();
            this.menuFiltersMidpoint = new System.Windows.Forms.MenuItem();
            this.menuFiltersMinimum = new System.Windows.Forms.MenuItem();
            this.menuFiltersMorphological = new System.Windows.Forms.MenuItem();
            this.menuMorphoDilation = new System.Windows.Forms.MenuItem();
            this.menuMorphoErosion = new System.Windows.Forms.MenuItem();
            this.menuMorphoOpen = new System.Windows.Forms.MenuItem();
            this.menuMorphoClose = new System.Windows.Forms.MenuItem();
            this.menuMorphoTophat = new System.Windows.Forms.MenuItem();
            this.menuMorphoGradient = new System.Windows.Forms.MenuItem();
            this.menuFiltersThreshold = new System.Windows.Forms.MenuItem();
            this.menuItem70 = new System.Windows.Forms.MenuItem();
            this.menuFiltersConvolutionFilter = new System.Windows.Forms.MenuItem();
            this.menuFiltersConvolutionMatrix = new System.Windows.Forms.MenuItem();
            this.menuItem73 = new System.Windows.Forms.MenuItem();
            this.menuFiltersEdge = new System.Windows.Forms.MenuItem();
            this.menuFiltersCannyEdgeDetector = new System.Windows.Forms.MenuItem();
            this.menuFiltersDustScratchRemoval = new System.Windows.Forms.MenuItem();
            this.menuTransforms = new System.Windows.Forms.MenuItem();
            this.menuTransformsChain = new System.Windows.Forms.MenuItem();
            this.menuTransformsApply = new System.Windows.Forms.MenuItem();
            this.menuItem77 = new System.Windows.Forms.MenuItem();
            this.menuTransformsBumpMap = new System.Windows.Forms.MenuItem();
            this.menuTransformsElliptical = new System.Windows.Forms.MenuItem();
            this.menuTransformsLens = new System.Windows.Forms.MenuItem();
            this.menuLineSlice = new System.Windows.Forms.MenuItem();
            this.menuTransformsMarble = new System.Windows.Forms.MenuItem();
            this.menuTransformsOffset = new System.Windows.Forms.MenuItem();
            this.menuTransformsPerlin = new System.Windows.Forms.MenuItem();
            this.menuTransformsPinch = new System.Windows.Forms.MenuItem();
            this.menuTransformsPolygon = new System.Windows.Forms.MenuItem();
            this.menuTransformsRandom = new System.Windows.Forms.MenuItem();
            this.menuTransformsRipple = new System.Windows.Forms.MenuItem();
            this.menuTransformsSpin = new System.Windows.Forms.MenuItem();
            this.menuTransformSpinWave = new System.Windows.Forms.MenuItem();
            this.menuTransformsWave = new System.Windows.Forms.MenuItem();
            this.menuTransformsWow = new System.Windows.Forms.MenuItem();
            this.menuTransformsZigZag = new System.Windows.Forms.MenuItem();
            this.menuItem94 = new System.Windows.Forms.MenuItem();
            this.menuTransformsUser = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItemAutomaticDocumentNegation = new System.Windows.Forms.MenuItem();
            this.menuItemBinarySegmentation = new System.Windows.Forms.MenuItem();
            this.menuItemBlackBorderCrop = new System.Windows.Forms.MenuItem();
            this.menuItemBlankPageDetection = new System.Windows.Forms.MenuItem();
            this.menuItemBlobRemoval = new System.Windows.Forms.MenuItem();
            this.menuItemBorderRemoval = new System.Windows.Forms.MenuItem();
            this.menuItemDeskew = new System.Windows.Forms.MenuItem();
            this.menuItemDespeckle = new System.Windows.Forms.MenuItem();
            this.menuItemHalftoneRemoval = new System.Windows.Forms.MenuItem();
            this.menuItemHolePunchRemoval = new System.Windows.Forms.MenuItem();
            this.menuItemInvertedTextCorrection = new System.Windows.Forms.MenuItem();
            this.menuItemLineRemoval = new System.Windows.Forms.MenuItem();
            this.menuItemSpeckRemoval = new System.Windows.Forms.MenuItem();
            this.menuItemWhiteMarginCrop = new System.Windows.Forms.MenuItem();
            this.menuDocument = new System.Windows.Forms.MenuItem();
            this.menuDocumentAutoDeskew = new System.Windows.Forms.MenuItem();
            this.menuDocumentMedian = new System.Windows.Forms.MenuItem();
            this.menuDocumentDespeckle = new System.Windows.Forms.MenuItem();
            this.menuItem103 = new System.Windows.Forms.MenuItem();
            this.menuDocumentMorphological = new System.Windows.Forms.MenuItem();
            this.menuBinaryDilation = new System.Windows.Forms.MenuItem();
            this.menuBinaryErosion = new System.Windows.Forms.MenuItem();
            this.menuBinaryOpen = new System.Windows.Forms.MenuItem();
            this.menuBinaryClose = new System.Windows.Forms.MenuItem();
            this.menuBinaryBoundary = new System.Windows.Forms.MenuItem();
            this.menuDocumentThinning = new System.Windows.Forms.MenuItem();
            this.menuDocumentHitOrMiss = new System.Windows.Forms.MenuItem();
            this.menuThresholding = new System.Windows.Forms.MenuItem();
            this.menuThresholdAdaptive = new System.Windows.Forms.MenuItem();
            this.menuThresholdGlobal = new System.Windows.Forms.MenuItem();
            this.menuThresholdDynamic = new System.Windows.Forms.MenuItem();
            this.menuBorderRemoval = new System.Windows.Forms.MenuItem();
            this.menuDithering = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuFftBandPass = new System.Windows.Forms.MenuItem();
            this.menuFftButterworthHighBoost = new System.Windows.Forms.MenuItem();
            this.menuFftButterworthHighPass = new System.Windows.Forms.MenuItem();
            this.menuFftButterworthLowPass = new System.Windows.Forms.MenuItem();
            this.menuFftGaussianHighBoost = new System.Windows.Forms.MenuItem();
            this.menuFftGaussianHighPass = new System.Windows.Forms.MenuItem();
            this.menuFftGaussianLowPass = new System.Windows.Forms.MenuItem();
            this.menuFftIdealHighPass = new System.Windows.Forms.MenuItem();
            this.menuFftIdealLowPass = new System.Windows.Forms.MenuItem();
            this.menuFftInversePower = new System.Windows.Forms.MenuItem();
            this.menuItem16 = new System.Windows.Forms.MenuItem();
            this.menuFlip = new System.Windows.Forms.MenuItem();
            this.menuFlipHorizontal = new System.Windows.Forms.MenuItem();
            this.menuFlipVertical = new System.Windows.Forms.MenuItem();
            this.menuItem23 = new System.Windows.Forms.MenuItem();
            this.menuRotate = new System.Windows.Forms.MenuItem();
            this.menuResizeCanvas = new System.Windows.Forms.MenuItem();
            this.menuResample = new System.Windows.Forms.MenuItem();
            this.menuCrop = new System.Windows.Forms.MenuItem();
            this.menuAutoCrop = new System.Windows.Forms.MenuItem();
            this.menuSkew = new System.Windows.Forms.MenuItem();
            this.menuQuadrilateralWarp = new System.Windows.Forms.MenuItem();
            this.menuPush = new System.Windows.Forms.MenuItem();
            this.menuItem28 = new System.Windows.Forms.MenuItem();
            this.menuOverlay = new System.Windows.Forms.MenuItem();
            this.menuOverlayNormal = new System.Windows.Forms.MenuItem();
            this.menuOverlayMasked = new System.Windows.Forms.MenuItem();
            this.menuOverlayMerged = new System.Windows.Forms.MenuItem();
            this.menuItem19 = new System.Windows.Forms.MenuItem();
            this.menuHelp = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.cboFrameIndex = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarLoadTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // statusInfo
            // 
            this.statusInfo.Location = new System.Drawing.Point(0, 451);
            this.statusInfo.Name = "statusInfo";
            this.statusInfo.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPosition,
            this.statusBarLoadTime,
            this.statusBarMessage,
            this.statusBarProgress});
            this.statusInfo.ShowPanels = true;
            this.statusInfo.Size = new System.Drawing.Size(857, 22);
            this.statusInfo.TabIndex = 1;
            this.statusInfo.Text = "statusInfo";
            // 
            // statusBarPosition
            // 
            this.statusBarPosition.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBarPosition.MinWidth = 125;
            this.statusBarPosition.Name = "statusBarPosition";
            this.statusBarPosition.Text = "-- x --";
            this.statusBarPosition.Width = 200;
            // 
            // statusBarLoadTime
            // 
            this.statusBarLoadTime.Name = "statusBarLoadTime";
            this.statusBarLoadTime.Width = 150;
            // 
            // statusBarMessage
            // 
            this.statusBarMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarMessage.Name = "statusBarMessage";
            this.statusBarMessage.Text = "Atalasoft dotImage";
            this.statusBarMessage.Width = 390;
            // 
            // statusBarProgress
            // 
            this.statusBarProgress.MinWidth = 100;
            this.statusBarProgress.Name = "statusBarProgress";
            // 
            // toolBar1
            // 
            this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar1.AutoSize = false;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbOpen,
            this.tbSave,
            this.tbSep,
            this.tbUndo,
            this.tbRedo,
            this.toolBarButton1,
            this.tbArrow,
            this.tbSelectRectangle,
            this.tbSelectEllipse,
            this.tbPan,
            this.tbMagnifier,
            this.tbZoom,
            this.tbZoomSelection});
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(857, 24);
            this.toolBar1.TabIndex = 2;
            this.toolBar1.Wrappable = false;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // tbOpen
            // 
            this.tbOpen.ImageIndex = 3;
            this.tbOpen.Name = "tbOpen";
            this.tbOpen.ToolTipText = "Open";
            // 
            // tbSave
            // 
            this.tbSave.Enabled = false;
            this.tbSave.ImageIndex = 4;
            this.tbSave.Name = "tbSave";
            this.tbSave.ToolTipText = "Save";
            // 
            // tbSep
            // 
            this.tbSep.Name = "tbSep";
            this.tbSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbUndo
            // 
            this.tbUndo.Enabled = false;
            this.tbUndo.ImageIndex = 5;
            this.tbUndo.Name = "tbUndo";
            this.tbUndo.ToolTipText = "Undo";
            // 
            // tbRedo
            // 
            this.tbRedo.Enabled = false;
            this.tbRedo.ImageIndex = 9;
            this.tbRedo.Name = "tbRedo";
            this.tbRedo.ToolTipText = "Redo";
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbArrow
            // 
            this.tbArrow.ImageIndex = 0;
            this.tbArrow.Name = "tbArrow";
            this.tbArrow.Pushed = true;
            this.tbArrow.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.tbArrow.ToolTipText = "Arrow";
            // 
            // tbSelectRectangle
            // 
            this.tbSelectRectangle.ImageIndex = 1;
            this.tbSelectRectangle.Name = "tbSelectRectangle";
            this.tbSelectRectangle.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.tbSelectRectangle.ToolTipText = "Rectangle Selection";
            // 
            // tbSelectEllipse
            // 
            this.tbSelectEllipse.ImageIndex = 10;
            this.tbSelectEllipse.Name = "tbSelectEllipse";
            this.tbSelectEllipse.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.tbSelectEllipse.ToolTipText = "Ellipse Selection";
            // 
            // tbPan
            // 
            this.tbPan.ImageIndex = 2;
            this.tbPan.Name = "tbPan";
            this.tbPan.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.tbPan.ToolTipText = "Pan";
            // 
            // tbMagnifier
            // 
            this.tbMagnifier.ImageIndex = 6;
            this.tbMagnifier.Name = "tbMagnifier";
            this.tbMagnifier.ToolTipText = "Magnifier";
            // 
            // tbZoom
            // 
            this.tbZoom.ImageIndex = 7;
            this.tbZoom.Name = "tbZoom";
            this.tbZoom.ToolTipText = "Zoom";
            // 
            // tbZoomSelection
            // 
            this.tbZoomSelection.ImageIndex = 8;
            this.tbZoomSelection.Name = "tbZoomSelection";
            this.tbZoomSelection.ToolTipText = "Zoom Selection";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(745, 455);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 16);
            this.progressBar1.TabIndex = 4;
            // 
            // rectangleSelection
            // 
            this.rectangleSelection.ActiveButtons = System.Windows.Forms.MouseButtons.Left;
            this.rectangleSelection.Animated = true;
            this.rectangleSelection.AspectRatio = 0F;
            this.rectangleSelection.BackgroundColor = System.Drawing.Color.White;
            this.rectangleSelection.ClickLock = false;
            this.rectangleSelection.Inverted = false;
            this.rectangleSelection.MoveCursor = System.Windows.Forms.Cursors.SizeAll;
            this.rectangleSelection.Parent = this.Viewer;
            this.rectangleSelection.Pen.Color = System.Drawing.Color.Black;
            this.rectangleSelection.Pen.CustomDashPattern = new int[] {
        8,
        8};
            this.rectangleSelection.Pen.LineStyle = Atalasoft.Imaging.Drawing.LineStyle.Custom;
            this.rectangleSelection.Persist = true;
            this.rectangleSelection.SelectionNESWCursor = System.Windows.Forms.Cursors.SizeNESW;
            this.rectangleSelection.SelectionNSCursor = System.Windows.Forms.Cursors.SizeNS;
            this.rectangleSelection.SelectionNWSECursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.rectangleSelection.SelectionWECursor = System.Windows.Forms.Cursors.SizeWE;
            // 
            // Viewer
            // 
            this.Viewer.AllowDrop = true;
            this.Viewer.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.ScaleToGray;
            this.Viewer.Asynchronous = true;
            this.Viewer.AutoZoom = Atalasoft.Imaging.WinControls.AutoZoomMode.FitToWidth;
            this.Viewer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Viewer.BackgroundImage")));
            this.Viewer.Centered = true;
            this.Viewer.DisplayProfile = null;
            this.Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Viewer.Location = new System.Drawing.Point(0, 24);
            this.Viewer.Magnifier.BackColor = System.Drawing.Color.White;
            this.Viewer.Magnifier.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("resource.BackgroundImage")));
            this.Viewer.Magnifier.BorderColor = System.Drawing.Color.White;
            this.Viewer.Magnifier.Size = new System.Drawing.Size(100, 100);
            this.Viewer.Name = "Viewer";
            this.Viewer.OutputProfile = null;
            this.Viewer.Selection = this.rectangleSelection;
            this.Viewer.Size = new System.Drawing.Size(857, 427);
            this.Viewer.TabIndex = 5;
            this.Viewer.Text = "workspaceViewer1";
            this.Viewer.UndoLevels = 5;
            this.Viewer.ProcessError += new Atalasoft.Imaging.ExceptionEventHandler(this.Viewer_ProcessError);
            this.Viewer.Progress += new Atalasoft.Imaging.ProgressEventHandler(this.Viewer_Progress);
            this.Viewer.ImageStreamCompleted += new Atalasoft.Imaging.ImageFileIOEventHandler(this.Viewer_ImageStreamCompleted);
            this.Viewer.ProcessCompleted += new Atalasoft.Imaging.ImageEventHandler(this.Viewer_ProcessCompleted);
            this.Viewer.ImageChanged += new Atalasoft.Imaging.ImageEventHandler(this.Viewer_ChangedImage);
            this.Viewer.MouseMovePixel += new System.Windows.Forms.MouseEventHandler(this.Viewer_MouseMovePixel);
            this.Viewer.DragDrop += new System.Windows.Forms.DragEventHandler(this.Viewer_DragDrop);
            this.Viewer.DragOver += new System.Windows.Forms.DragEventHandler(this.Viewer_DragOver);
            this.Viewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Viewer_KeyDown);
            this.Viewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Viewer_MouseDown);
            // 
            // imagePrintDocument1
            // 
            this.imagePrintDocument1.ScaleMode = Atalasoft.Imaging.WinControls.PrintScaleMode.None;
            // 
            // ellipseRubberband
            // 
            this.ellipseRubberband.ActiveButtons = System.Windows.Forms.MouseButtons.Left;
            this.ellipseRubberband.AspectRatio = 0F;
            this.ellipseRubberband.BackgroundColor = System.Drawing.Color.Transparent;
            this.ellipseRubberband.ClickLock = false;
            this.ellipseRubberband.ConstrainPosition = false;
            this.ellipseRubberband.Fill = null;
            this.ellipseRubberband.MoveCursor = System.Windows.Forms.Cursors.SizeAll;
            this.ellipseRubberband.Parent = this.Viewer;
            this.ellipseRubberband.Pen.Color = System.Drawing.Color.Black;
            this.ellipseRubberband.Pen.CustomDashPattern = new int[] {
        8,
        8};
            this.ellipseRubberband.Persist = true;
            this.ellipseRubberband.SnapToPixelGrid = false;
            // 
            // rectangleDraw
            // 
            this.rectangleDraw.ActiveButtons = System.Windows.Forms.MouseButtons.Left;
            this.rectangleDraw.AspectRatio = 0F;
            this.rectangleDraw.BackgroundColor = System.Drawing.Color.Transparent;
            this.rectangleDraw.CornerRadius = new System.Drawing.Size(0, 0);
            this.rectangleDraw.Fill = null;
            this.rectangleDraw.Inverted = false;
            this.rectangleDraw.MoveCursor = System.Windows.Forms.Cursors.SizeAll;
            this.rectangleDraw.Parent = this.Viewer;
            this.rectangleDraw.Pen.Color = System.Drawing.Color.Black;
            this.rectangleDraw.Pen.CustomDashPattern = new int[] {
        8,
        8};
            this.rectangleDraw.Changed += new Atalasoft.Imaging.WinControls.RubberbandEventHandler(this.rectangleDraw_Changed);
            // 
            // ellipseDraw
            // 
            this.ellipseDraw.ActiveButtons = System.Windows.Forms.MouseButtons.Left;
            this.ellipseDraw.AspectRatio = 0F;
            this.ellipseDraw.BackgroundColor = System.Drawing.Color.Transparent;
            this.ellipseDraw.ConstrainPosition = false;
            this.ellipseDraw.Fill = null;
            this.ellipseDraw.Inverted = false;
            this.ellipseDraw.MoveCursor = System.Windows.Forms.Cursors.SizeAll;
            this.ellipseDraw.Parent = this.Viewer;
            this.ellipseDraw.Pen.Color = System.Drawing.Color.Black;
            this.ellipseDraw.Pen.CustomDashPattern = new int[] {
        8,
        8};
            this.ellipseDraw.Persist = true;
            this.ellipseDraw.SnapToPixelGrid = false;
            this.ellipseDraw.Changed += new Atalasoft.Imaging.WinControls.RubberbandEventHandler(this.ellipseDraw_Changed);
            // 
            // lineDraw
            // 
            this.lineDraw.ActiveButtons = System.Windows.Forms.MouseButtons.Left;
            this.lineDraw.AspectRatio = 0F;
            this.lineDraw.BackgroundColor = System.Drawing.Color.Transparent;
            this.lineDraw.Inverted = false;
            this.lineDraw.MoveCursor = System.Windows.Forms.Cursors.SizeAll;
            this.lineDraw.Parent = this.Viewer;
            this.lineDraw.Pen.Color = System.Drawing.Color.Black;
            this.lineDraw.Pen.CustomDashPattern = new int[] {
        8,
        8};
            this.lineDraw.Changed += new Atalasoft.Imaging.WinControls.RubberbandEventHandler(this.lineDraw_Changed);
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuView,
            this.menuEdit,
            this.menuImage,
            this.menuDraw,
            this.menuCommands,
            this.menuHelp});
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFileNew,
            this.menuFileOpen,
            this.menuFileOpenFromURL,
            this.menuFileDecoders,
            this.menuItem1,
            this.menuFileNoise,
            this.menuItem2,
            this.menuFileSaveAs,
            this.menuFileSaveFTP,
            this.menuItem5,
            this.menuFilePageSetup,
            this.menuFilePrintImage,
            this.menuItem6,
            this.menuFileExit});
            this.menuFile.Text = "&File";
            // 
            // menuFileNew
            // 
            this.menuFileNew.Index = 0;
            this.menuFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.menuFileNew.Text = "&New";
            this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Index = 1;
            this.menuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.menuFileOpen.Text = "&Open";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            // 
            // menuFileOpenFromURL
            // 
            this.menuFileOpenFromURL.Index = 2;
            this.menuFileOpenFromURL.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftO;
            this.menuFileOpenFromURL.Text = "Open from &URL";
            this.menuFileOpenFromURL.Click += new System.EventHandler(this.menuFileOpenFromURL_Click);
            // 
            // menuFileDecoders
            // 
            this.menuFileDecoders.Index = 3;
            this.menuFileDecoders.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFileJPEG,
            this.menuFilePDF,
            this.menuFilePNG,
            this.menuFileTLA,
            this.menuFileWMF});
            this.menuFileDecoders.Text = "Decoder Settings";
            this.menuFileDecoders.Click += new System.EventHandler(this.menuFileDecoders_Click);
            // 
            // menuFileJPEG
            // 
            this.menuFileJPEG.Index = 0;
            this.menuFileJPEG.Text = "JPEG";
            this.menuFileJPEG.Click += new System.EventHandler(this.menuFileDecoders_Click);
            // 
            // menuFilePDF
            // 
            this.menuFilePDF.Index = 1;
            this.menuFilePDF.Text = "PDF";
            this.menuFilePDF.Click += new System.EventHandler(this.menuFileDecoders_Click);
            // 
            // menuFilePNG
            // 
            this.menuFilePNG.Index = 2;
            this.menuFilePNG.Text = "PNG";
            this.menuFilePNG.Click += new System.EventHandler(this.menuFileDecoders_Click);
            // 
            // menuFileTLA
            // 
            this.menuFileTLA.Index = 3;
            this.menuFileTLA.Text = "TLA";
            this.menuFileTLA.Click += new System.EventHandler(this.menuFileDecoders_Click);
            // 
            // menuFileWMF
            // 
            this.menuFileWMF.Index = 4;
            this.menuFileWMF.Text = "WMF";
            this.menuFileWMF.Click += new System.EventHandler(this.menuFileDecoders_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 4;
            this.menuItem1.Text = "-";
            // 
            // menuFileNoise
            // 
            this.menuFileNoise.Index = 5;
            this.menuFileNoise.Text = "Generate Noise Image";
            this.menuFileNoise.Click += new System.EventHandler(this.menuFileNoise_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 6;
            this.menuItem2.Text = "-";
            // 
            // menuFileSaveAs
            // 
            this.menuFileSaveAs.Enabled = false;
            this.menuFileSaveAs.Index = 7;
            this.menuFileSaveAs.Text = "Save &As";
            this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
            // 
            // menuFileSaveFTP
            // 
            this.menuFileSaveFTP.Enabled = false;
            this.menuFileSaveFTP.Index = 8;
            this.menuFileSaveFTP.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftS;
            this.menuFileSaveFTP.Text = "Save to &FTP";
            this.menuFileSaveFTP.Click += new System.EventHandler(this.menuFileSaveFTP_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 9;
            this.menuItem5.Text = "-";
            // 
            // menuFilePageSetup
            // 
            this.menuFilePageSetup.Enabled = false;
            this.menuFilePageSetup.Index = 10;
            this.menuFilePageSetup.Text = "Page Setup";
            this.menuFilePageSetup.Click += new System.EventHandler(this.menuFilePageSetup_Click);
            // 
            // menuFilePrintImage
            // 
            this.menuFilePrintImage.Enabled = false;
            this.menuFilePrintImage.Index = 11;
            this.menuFilePrintImage.Text = "Print Image";
            this.menuFilePrintImage.Click += new System.EventHandler(this.menuFilePrintImage_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 12;
            this.menuItem6.Text = "-";
            // 
            // menuFileExit
            // 
            this.menuFileExit.Index = 13;
            this.menuFileExit.Text = "Exit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuView
            // 
            this.menuView.Index = 1;
            this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuViewWidth,
            this.menuViewBest,
            this.menuViewHeight,
            this.menuView100,
            this.menuView50,
            this.menuView150});
            this.menuView.Text = "&View";
            // 
            // menuViewWidth
            // 
            this.menuViewWidth.Index = 0;
            this.menuViewWidth.Text = "Fit to &Width";
            this.menuViewWidth.Click += new System.EventHandler(this.menuViewWidth_Click);
            // 
            // menuViewBest
            // 
            this.menuViewBest.Index = 1;
            this.menuViewBest.Text = "&Best Fit (Shrink Only)";
            this.menuViewBest.Click += new System.EventHandler(this.menuViewBest_Click);
            // 
            // menuViewHeight
            // 
            this.menuViewHeight.Index = 2;
            this.menuViewHeight.Text = "Fit to &Height";
            this.menuViewHeight.Click += new System.EventHandler(this.menuViewHeight_Click);
            // 
            // menuView100
            // 
            this.menuView100.Index = 3;
            this.menuView100.Text = "&100%";
            this.menuView100.Click += new System.EventHandler(this.menuView100_Click);
            // 
            // menuView50
            // 
            this.menuView50.Index = 4;
            this.menuView50.Text = "&50%";
            this.menuView50.Click += new System.EventHandler(this.menuView50_Click);
            // 
            // menuView150
            // 
            this.menuView150.Index = 5;
            this.menuView150.Text = "150%";
            this.menuView150.Click += new System.EventHandler(this.menuView150_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.Index = 2;
            this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuEditUndo,
            this.menuEditRedo,
            this.menuItem8,
            this.menuEditCut,
            this.menuEditCopy,
            this.menuEditPaste,
            this.menuItem12,
            this.menuEditOptions});
            this.menuEdit.Text = "&Edit";
            // 
            // menuEditUndo
            // 
            this.menuEditUndo.Enabled = false;
            this.menuEditUndo.Index = 0;
            this.menuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.menuEditUndo.Text = "&Undo";
            this.menuEditUndo.Click += new System.EventHandler(this.menuEditUndo_Click);
            // 
            // menuEditRedo
            // 
            this.menuEditRedo.Enabled = false;
            this.menuEditRedo.Index = 1;
            this.menuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
            this.menuEditRedo.Text = "&Redo";
            this.menuEditRedo.Click += new System.EventHandler(this.menuEditRedo_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 2;
            this.menuItem8.Text = "-";
            // 
            // menuEditCut
            // 
            this.menuEditCut.Enabled = false;
            this.menuEditCut.Index = 3;
            this.menuEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.menuEditCut.Text = "Cut";
            this.menuEditCut.Click += new System.EventHandler(this.menuEditCut_Click);
            // 
            // menuEditCopy
            // 
            this.menuEditCopy.Enabled = false;
            this.menuEditCopy.Index = 4;
            this.menuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuEditCopy.Text = "Copy";
            this.menuEditCopy.Click += new System.EventHandler(this.menuEditCopy_Click);
            // 
            // menuEditPaste
            // 
            this.menuEditPaste.Enabled = false;
            this.menuEditPaste.Index = 5;
            this.menuEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.menuEditPaste.Text = "Paste";
            this.menuEditPaste.Click += new System.EventHandler(this.menuEditPaste_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 6;
            this.menuItem12.Text = "-";
            // 
            // menuEditOptions
            // 
            this.menuEditOptions.Index = 7;
            this.menuEditOptions.Text = "Options";
            this.menuEditOptions.Click += new System.EventHandler(this.menuEditOptions_Click);
            // 
            // menuImage
            // 
            this.menuImage.Enabled = false;
            this.menuImage.Index = 3;
            this.menuImage.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuImageInformation,
            this.menuImageChangePixelFormat,
            this.menuImageShowHistogram,
            this.menuItem11,
            this.menuImageExif,
            this.menuImageIptc,
            this.menuItem7,
            this.menuImageZoom,
            this.menuItem9});
            this.menuImage.Text = "&Image";
            // 
            // menuImageInformation
            // 
            this.menuImageInformation.Index = 0;
            this.menuImageInformation.Text = "Information";
            this.menuImageInformation.Click += new System.EventHandler(this.menuImageInformation_Click);
            // 
            // menuImageChangePixelFormat
            // 
            this.menuImageChangePixelFormat.Index = 1;
            this.menuImageChangePixelFormat.Text = "Change Pixel Format";
            this.menuImageChangePixelFormat.Click += new System.EventHandler(this.menuImageChangePixelFormat_Click);
            // 
            // menuImageShowHistogram
            // 
            this.menuImageShowHistogram.Index = 2;
            this.menuImageShowHistogram.Text = "Show Histogram";
            this.menuImageShowHistogram.Click += new System.EventHandler(this.menuImageShowHistogram_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 3;
            this.menuItem11.Text = "-";
            // 
            // menuImageExif
            // 
            this.menuImageExif.Index = 4;
            this.menuImageExif.Text = "&EXIF Data";
            this.menuImageExif.Click += new System.EventHandler(this.menuImageExif_Click);
            // 
            // menuImageIptc
            // 
            this.menuImageIptc.Index = 5;
            this.menuImageIptc.Text = "I&PTC Data";
            this.menuImageIptc.Click += new System.EventHandler(this.menuImageIptc_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 6;
            this.menuItem7.Text = "-";
            // 
            // menuImageZoom
            // 
            this.menuImageZoom.Index = 7;
            this.menuImageZoom.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuImageZoom132,
            this.menuImageZoom116,
            this.menuImageZoom18,
            this.menuImageZoom25,
            this.menuImageZoom50,
            this.menuImageZoom100,
            this.menuImageZoom200,
            this.menuImageZoom500,
            this.menuImageZoom1000,
            this.menuImageZoom31});
            this.menuImageZoom.Text = "Zoom";
            // 
            // menuImageZoom132
            // 
            this.menuImageZoom132.Index = 0;
            this.menuImageZoom132.Text = "1/32";
            this.menuImageZoom132.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom116
            // 
            this.menuImageZoom116.Index = 1;
            this.menuImageZoom116.Text = "1/16";
            this.menuImageZoom116.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom18
            // 
            this.menuImageZoom18.Index = 2;
            this.menuImageZoom18.Text = "1/8";
            this.menuImageZoom18.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom25
            // 
            this.menuImageZoom25.Index = 3;
            this.menuImageZoom25.Text = "25%";
            this.menuImageZoom25.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom50
            // 
            this.menuImageZoom50.Index = 4;
            this.menuImageZoom50.Text = "50%";
            this.menuImageZoom50.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom100
            // 
            this.menuImageZoom100.Index = 5;
            this.menuImageZoom100.Text = "100%";
            this.menuImageZoom100.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom200
            // 
            this.menuImageZoom200.Index = 6;
            this.menuImageZoom200.Text = "200%";
            this.menuImageZoom200.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom500
            // 
            this.menuImageZoom500.Index = 7;
            this.menuImageZoom500.Text = "500%";
            this.menuImageZoom500.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom1000
            // 
            this.menuImageZoom1000.Index = 8;
            this.menuImageZoom1000.Text = "1000%";
            this.menuImageZoom1000.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuImageZoom31
            // 
            this.menuImageZoom31.Index = 9;
            this.menuImageZoom31.Text = "1/31";
            this.menuImageZoom31.Click += new System.EventHandler(this.menuImageZoom_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 8;
            this.menuItem9.Text = "-";
            // 
            // menuDraw
            // 
            this.menuDraw.Enabled = false;
            this.menuDraw.Index = 4;
            this.menuDraw.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDrawLine,
            this.menuDrawLines,
            this.menuDrawRectangle,
            this.menuDrawEllipse,
            this.menuDrawPolygon,
            this.menuDrawFreehand,
            this.menuItem21,
            this.menuDrawText,
            this.menuDrawSetBackcolor,
            this.menuDrawClearBackcolor});
            this.menuDraw.Text = "&Draw";
            // 
            // menuDrawLine
            // 
            this.menuDrawLine.Index = 0;
            this.menuDrawLine.MergeType = System.Windows.Forms.MenuMerge.Replace;
            this.menuDrawLine.Text = "Line";
            this.menuDrawLine.Click += new System.EventHandler(this.menuDrawLine_Click);
            // 
            // menuDrawLines
            // 
            this.menuDrawLines.Index = 1;
            this.menuDrawLines.Text = "Lines";
            this.menuDrawLines.Click += new System.EventHandler(this.menuDrawLines_Click);
            // 
            // menuDrawRectangle
            // 
            this.menuDrawRectangle.Index = 2;
            this.menuDrawRectangle.Text = "Rectangle";
            this.menuDrawRectangle.Click += new System.EventHandler(this.menuDrawRectangle_Click);
            // 
            // menuDrawEllipse
            // 
            this.menuDrawEllipse.Index = 3;
            this.menuDrawEllipse.Text = "Ellipse";
            this.menuDrawEllipse.Click += new System.EventHandler(this.menuDrawEllipse_Click);
            // 
            // menuDrawPolygon
            // 
            this.menuDrawPolygon.Index = 4;
            this.menuDrawPolygon.Text = "Polygon";
            this.menuDrawPolygon.Click += new System.EventHandler(this.menuDrawPolygon_Click);
            // 
            // menuDrawFreehand
            // 
            this.menuDrawFreehand.Index = 5;
            this.menuDrawFreehand.Text = "Freehand";
            this.menuDrawFreehand.Click += new System.EventHandler(this.menuDrawFreehand_Click);
            // 
            // menuItem21
            // 
            this.menuItem21.Index = 6;
            this.menuItem21.Text = "-";
            // 
            // menuDrawText
            // 
            this.menuDrawText.Index = 7;
            this.menuDrawText.Text = "Text";
            this.menuDrawText.Click += new System.EventHandler(this.menuDrawText_Click);
            // 
            // menuDrawSetBackcolor
            // 
            this.menuDrawSetBackcolor.Index = 8;
            this.menuDrawSetBackcolor.Text = "Set Text Backcolor...";
            this.menuDrawSetBackcolor.Click += new System.EventHandler(this.menuDrawSetBackcolor_Click);
            // 
            // menuDrawClearBackcolor
            // 
            this.menuDrawClearBackcolor.Index = 9;
            this.menuDrawClearBackcolor.Text = "Clear Text Backcolor";
            this.menuDrawClearBackcolor.Click += new System.EventHandler(this.menuDrawClearBackcolor_Click);
            // 
            // menuCommands
            // 
            this.menuCommands.Enabled = false;
            this.menuCommands.Index = 5;
            this.menuCommands.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuChannels,
            this.menuEffects,
            this.menuFilters,
            this.menuTransforms,
            this.menuItem10,
            this.menuDocument,
            this.menuItem3,
            this.menuItem16,
            this.menuFlip,
            this.menuItem23,
            this.menuRotate,
            this.menuResizeCanvas,
            this.menuResample,
            this.menuCrop,
            this.menuAutoCrop,
            this.menuSkew,
            this.menuQuadrilateralWarp,
            this.menuPush,
            this.menuItem28,
            this.menuOverlay,
            this.menuItem19});
            this.menuCommands.Text = "&Commands";
            // 
            // menuChannels
            // 
            this.menuChannels.Index = 0;
            this.menuChannels.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuChannelsCombine,
            this.menuChannelsReplace,
            this.menuChannelsSplit,
            this.menuItem13,
            this.menuChannelsAdjust,
            this.menuChannelsAdjustHsl,
            this.menuChannelsApplyLut,
            this.menuChannelsInvert,
            this.menuChannelsShift,
            this.menuChannelsSwap,
            this.menuItem24,
            this.menuChannelsFlattenAlpha,
            this.menuChannelsAphaColor,
            this.menuChannelsAlphaMask,
            this.menuChannelsAlphaValue});
            this.menuChannels.Text = "Channels";
            // 
            // menuChannelsCombine
            // 
            this.menuChannelsCombine.Index = 0;
            this.menuChannelsCombine.Text = "Combine";
            this.menuChannelsCombine.Click += new System.EventHandler(this.menuChannelsCombine_Click);
            // 
            // menuChannelsReplace
            // 
            this.menuChannelsReplace.Index = 1;
            this.menuChannelsReplace.Text = "Replace";
            this.menuChannelsReplace.Click += new System.EventHandler(this.menuChannelsReplace_Click);
            // 
            // menuChannelsSplit
            // 
            this.menuChannelsSplit.Index = 2;
            this.menuChannelsSplit.Text = "Split";
            this.menuChannelsSplit.Click += new System.EventHandler(this.menuChannelsSplit_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 3;
            this.menuItem13.Text = "-";
            // 
            // menuChannelsAdjust
            // 
            this.menuChannelsAdjust.Index = 4;
            this.menuChannelsAdjust.Text = "Adjust";
            this.menuChannelsAdjust.Click += new System.EventHandler(this.menuChannelsAdjust_Click);
            // 
            // menuChannelsAdjustHsl
            // 
            this.menuChannelsAdjustHsl.Index = 5;
            this.menuChannelsAdjustHsl.Text = "Adjust HSL";
            this.menuChannelsAdjustHsl.Click += new System.EventHandler(this.menuChannelsAdjustHsl_Click);
            // 
            // menuChannelsApplyLut
            // 
            this.menuChannelsApplyLut.Index = 6;
            this.menuChannelsApplyLut.Text = "Apply LUT Demo (invert)";
            this.menuChannelsApplyLut.Click += new System.EventHandler(this.menuChannelsApplyLut_Click);
            // 
            // menuChannelsInvert
            // 
            this.menuChannelsInvert.Index = 7;
            this.menuChannelsInvert.Text = "Invert";
            this.menuChannelsInvert.Click += new System.EventHandler(this.menuChannelsInvert_Click);
            // 
            // menuChannelsShift
            // 
            this.menuChannelsShift.Index = 8;
            this.menuChannelsShift.Text = "Shift";
            this.menuChannelsShift.Click += new System.EventHandler(this.menuChannelsShift_Click);
            // 
            // menuChannelsSwap
            // 
            this.menuChannelsSwap.Index = 9;
            this.menuChannelsSwap.Text = "Swap";
            this.menuChannelsSwap.Click += new System.EventHandler(this.menuChannelsSwap_Click);
            // 
            // menuItem24
            // 
            this.menuItem24.Index = 10;
            this.menuItem24.Text = "-";
            // 
            // menuChannelsFlattenAlpha
            // 
            this.menuChannelsFlattenAlpha.Index = 11;
            this.menuChannelsFlattenAlpha.Text = "Flatten Alpha";
            this.menuChannelsFlattenAlpha.Click += new System.EventHandler(this.menuChannelsFlattenAlpha_Click);
            // 
            // menuChannelsAphaColor
            // 
            this.menuChannelsAphaColor.Index = 12;
            this.menuChannelsAphaColor.Text = "Set Alpha By Color";
            this.menuChannelsAphaColor.Click += new System.EventHandler(this.menuChannelsAphaColor_Click);
            // 
            // menuChannelsAlphaMask
            // 
            this.menuChannelsAlphaMask.Index = 13;
            this.menuChannelsAlphaMask.Text = "Set Alpha From Mask";
            this.menuChannelsAlphaMask.Click += new System.EventHandler(this.menuChannelsAlphaMask_Click);
            // 
            // menuChannelsAlphaValue
            // 
            this.menuChannelsAlphaValue.Index = 14;
            this.menuChannelsAlphaValue.Text = "Set Alpha Value";
            this.menuChannelsAlphaValue.Click += new System.EventHandler(this.menuChannelsAlphaValue_Click);
            // 
            // menuEffects
            // 
            this.menuEffects.Index = 1;
            this.menuEffects.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuEffectsAdjustTint,
            this.menuEffectsBevelEdge,
            this.menuEffectsCrackle,
            this.menuEffectsDeInterlace,
            this.menuEffectsDropShadow,
            this.menuEffectsFingerPrint,
            this.menuEffectsFloodFill,
            this.menuEffectsGamma,
            this.menuEffectsGauzy,
            this.menuEffectsHalftone,
            this.menuEffectsMosaic,
            this.menuEffectsOilPaint,
            this.menuEffectsPosterize,
            this.menuReduceColors,
            this.menuEffectsReplaceColor,
            this.menuEffectsRoundedBevel,
            this.menuEffectsSaturation,
            this.menuEffectsSolarize,
            this.menuEffectsStipple,
            this.menuEffectsTintGrayscale,
            this.menuEffectsWatercolorTint,
            this.menuItem46,
            this.menuEffectsBrightnessHistogramEqualize,
            this.menuEffectsBrightnessHistogramStretch,
            this.menuHistogramEqualize,
            this.menuEffectsHistogramStretch,
            this.menuItem4,
            this.menuEffectsRedEyeRemoval,
            this.menuEffectsLevels,
            this.menuEffectsAutoLevels,
            this.menuEffectsAutoContrast,
            this.menuEffectsAutoColor,
            this.menuEffectsAutoWhiteBalance});
            this.menuEffects.Text = "Effects";
            // 
            // menuEffectsAdjustTint
            // 
            this.menuEffectsAdjustTint.Index = 0;
            this.menuEffectsAdjustTint.Text = "Adjust Tint";
            this.menuEffectsAdjustTint.Click += new System.EventHandler(this.menuEffectsAdjustTint_Click);
            // 
            // menuEffectsBevelEdge
            // 
            this.menuEffectsBevelEdge.Index = 1;
            this.menuEffectsBevelEdge.Text = "Bevel Edge";
            this.menuEffectsBevelEdge.Click += new System.EventHandler(this.menuEffectsBevelEdge_Click);
            // 
            // menuEffectsCrackle
            // 
            this.menuEffectsCrackle.Index = 2;
            this.menuEffectsCrackle.Text = "Crackle";
            this.menuEffectsCrackle.Click += new System.EventHandler(this.menuEffectsCrackle_Click);
            // 
            // menuEffectsDeInterlace
            // 
            this.menuEffectsDeInterlace.Index = 3;
            this.menuEffectsDeInterlace.Text = "De-Interlace";
            this.menuEffectsDeInterlace.Click += new System.EventHandler(this.menuEffectsDeInterlace_Click);
            // 
            // menuEffectsDropShadow
            // 
            this.menuEffectsDropShadow.Index = 4;
            this.menuEffectsDropShadow.Text = "Drop Shadow";
            this.menuEffectsDropShadow.Click += new System.EventHandler(this.menuEffectsDropShadow_Click);
            // 
            // menuEffectsFingerPrint
            // 
            this.menuEffectsFingerPrint.Index = 5;
            this.menuEffectsFingerPrint.Text = "Finger Print";
            this.menuEffectsFingerPrint.Click += new System.EventHandler(this.menuEffectsFingerPrint_Click);
            // 
            // menuEffectsFloodFill
            // 
            this.menuEffectsFloodFill.Index = 6;
            this.menuEffectsFloodFill.Text = "Flood Fill";
            this.menuEffectsFloodFill.Click += new System.EventHandler(this.menuEffectsFloodFill_Click);
            // 
            // menuEffectsGamma
            // 
            this.menuEffectsGamma.Index = 7;
            this.menuEffectsGamma.Text = "Gamma";
            this.menuEffectsGamma.Click += new System.EventHandler(this.menuEffectsGamma_Click);
            // 
            // menuEffectsGauzy
            // 
            this.menuEffectsGauzy.Index = 8;
            this.menuEffectsGauzy.Text = "Gauzy";
            this.menuEffectsGauzy.Click += new System.EventHandler(this.menuEffectsGauzy_Click);
            // 
            // menuEffectsHalftone
            // 
            this.menuEffectsHalftone.Index = 9;
            this.menuEffectsHalftone.Text = "Halftone";
            this.menuEffectsHalftone.Click += new System.EventHandler(this.menuEffectsHalftone_Click);
            // 
            // menuEffectsMosaic
            // 
            this.menuEffectsMosaic.Index = 10;
            this.menuEffectsMosaic.Text = "Mosaic";
            this.menuEffectsMosaic.Click += new System.EventHandler(this.menuEffectsMosaic_Click);
            // 
            // menuEffectsOilPaint
            // 
            this.menuEffectsOilPaint.Index = 11;
            this.menuEffectsOilPaint.Text = "Oil Paint";
            this.menuEffectsOilPaint.Click += new System.EventHandler(this.menuEffectsOilPaint_Click);
            // 
            // menuEffectsPosterize
            // 
            this.menuEffectsPosterize.Index = 12;
            this.menuEffectsPosterize.Text = "Posterize";
            this.menuEffectsPosterize.Click += new System.EventHandler(this.menuEffectsPosterize_Click);
            // 
            // menuReduceColors
            // 
            this.menuReduceColors.Index = 13;
            this.menuReduceColors.Text = "Reduce Colors";
            this.menuReduceColors.Click += new System.EventHandler(this.menuReduceColors_Click);
            // 
            // menuEffectsReplaceColor
            // 
            this.menuEffectsReplaceColor.Index = 14;
            this.menuEffectsReplaceColor.Text = "Replace Color";
            this.menuEffectsReplaceColor.Click += new System.EventHandler(this.menuEffectsReplaceColor_Click);
            // 
            // menuEffectsRoundedBevel
            // 
            this.menuEffectsRoundedBevel.Index = 15;
            this.menuEffectsRoundedBevel.Text = "Rounded Bevel";
            this.menuEffectsRoundedBevel.Click += new System.EventHandler(this.menuEffectsRoundedBevel_Click);
            // 
            // menuEffectsSaturation
            // 
            this.menuEffectsSaturation.Index = 16;
            this.menuEffectsSaturation.Text = "Saturation";
            this.menuEffectsSaturation.Click += new System.EventHandler(this.menuEffectsSaturation_Click);
            // 
            // menuEffectsSolarize
            // 
            this.menuEffectsSolarize.Index = 17;
            this.menuEffectsSolarize.Text = "Solarize";
            this.menuEffectsSolarize.Click += new System.EventHandler(this.menuEffectsSolarize_Click);
            // 
            // menuEffectsStipple
            // 
            this.menuEffectsStipple.Index = 18;
            this.menuEffectsStipple.Text = "Stipple";
            this.menuEffectsStipple.Click += new System.EventHandler(this.menuEffectsStipple_Click);
            // 
            // menuEffectsTintGrayscale
            // 
            this.menuEffectsTintGrayscale.Index = 19;
            this.menuEffectsTintGrayscale.Text = "Tint Grayscale";
            this.menuEffectsTintGrayscale.Click += new System.EventHandler(this.menuEffectsTintGrayscale_Click);
            // 
            // menuEffectsWatercolorTint
            // 
            this.menuEffectsWatercolorTint.Index = 20;
            this.menuEffectsWatercolorTint.Text = "Watercolor Tint";
            this.menuEffectsWatercolorTint.Click += new System.EventHandler(this.menuEffectsWatercolorTint_Click);
            // 
            // menuItem46
            // 
            this.menuItem46.Index = 21;
            this.menuItem46.Text = "-";
            // 
            // menuEffectsBrightnessHistogramEqualize
            // 
            this.menuEffectsBrightnessHistogramEqualize.Index = 22;
            this.menuEffectsBrightnessHistogramEqualize.Text = "Brightness Histogram Equalize";
            this.menuEffectsBrightnessHistogramEqualize.Click += new System.EventHandler(this.menuEffectsBrightnessHistogramEqualize_Click);
            // 
            // menuEffectsBrightnessHistogramStretch
            // 
            this.menuEffectsBrightnessHistogramStretch.Index = 23;
            this.menuEffectsBrightnessHistogramStretch.Text = "Brightness Histogram Stretch";
            this.menuEffectsBrightnessHistogramStretch.Click += new System.EventHandler(this.menuEffectsBrightnessHistogramStretch_Click);
            // 
            // menuHistogramEqualize
            // 
            this.menuHistogramEqualize.Index = 24;
            this.menuHistogramEqualize.Text = "Histogram Equalize";
            this.menuHistogramEqualize.Click += new System.EventHandler(this.menuHistogramEqualize_Click);
            // 
            // menuEffectsHistogramStretch
            // 
            this.menuEffectsHistogramStretch.Index = 25;
            this.menuEffectsHistogramStretch.Text = "Histogram Stretch";
            this.menuEffectsHistogramStretch.Click += new System.EventHandler(this.menuEffectsHistogramStretch_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 26;
            this.menuItem4.Text = "-";
            // 
            // menuEffectsRedEyeRemoval
            // 
            this.menuEffectsRedEyeRemoval.Index = 27;
            this.menuEffectsRedEyeRemoval.Text = "Red Eye Removal";
            this.menuEffectsRedEyeRemoval.Click += new System.EventHandler(this.menuEffectsRedEyeRemoval_Click);
            // 
            // menuEffectsLevels
            // 
            this.menuEffectsLevels.Index = 28;
            this.menuEffectsLevels.Text = "Levels";
            this.menuEffectsLevels.Click += new System.EventHandler(this.menuEffectsLevels_Click);
            // 
            // menuEffectsAutoLevels
            // 
            this.menuEffectsAutoLevels.Index = 29;
            this.menuEffectsAutoLevels.Text = "Auto Levels";
            this.menuEffectsAutoLevels.Click += new System.EventHandler(this.menuEffectsAutoLevels_Click);
            // 
            // menuEffectsAutoContrast
            // 
            this.menuEffectsAutoContrast.Index = 30;
            this.menuEffectsAutoContrast.Text = "Auto Contrast";
            this.menuEffectsAutoContrast.Click += new System.EventHandler(this.menuEffectsAutoContrast_Click);
            // 
            // menuEffectsAutoColor
            // 
            this.menuEffectsAutoColor.Index = 31;
            this.menuEffectsAutoColor.Text = "Auto Color";
            this.menuEffectsAutoColor.Click += new System.EventHandler(this.menuEffectsAutoColor_Click);
            // 
            // menuEffectsAutoWhiteBalance
            // 
            this.menuEffectsAutoWhiteBalance.Index = 32;
            this.menuEffectsAutoWhiteBalance.Text = "Auto White Balance";
            this.menuEffectsAutoWhiteBalance.Click += new System.EventHandler(this.menuEffectsAutoWhiteBalance_Click);
            // 
            // menuFilters
            // 
            this.menuFilters.Index = 2;
            this.menuFilters.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFiltersBrightnessContrast,
            this.menuFiltersSaturation,
            this.menuFiltersBlur,
            this.menuFiltersGaussianBlur,
            this.menuAdaptiveUnsharpMask,
            this.menuEffectsUnsharpMask,
            this.menuFiltersSharpen,
            this.menuItem58,
            this.menuFiltersAddNoise,
            this.menuFiltersDespeckle,
            this.menuFiltersEmboss,
            this.menuFiltersIntensify,
            this.menuFiltersHighPass,
            this.menuFiltersMaximum,
            this.menuFiltersMean,
            this.menuFiltersMedian,
            this.menuFiltersMidpoint,
            this.menuFiltersMinimum,
            this.menuFiltersMorphological,
            this.menuFiltersThreshold,
            this.menuItem70,
            this.menuFiltersConvolutionFilter,
            this.menuFiltersConvolutionMatrix,
            this.menuItem73,
            this.menuFiltersEdge,
            this.menuFiltersCannyEdgeDetector,
            this.menuFiltersDustScratchRemoval});
            this.menuFilters.Text = "Filters";
            // 
            // menuFiltersBrightnessContrast
            // 
            this.menuFiltersBrightnessContrast.Index = 0;
            this.menuFiltersBrightnessContrast.Text = "Brightness and Contrast";
            this.menuFiltersBrightnessContrast.Click += new System.EventHandler(this.menuFiltersBrightnessContrast_Click);
            // 
            // menuFiltersSaturation
            // 
            this.menuFiltersSaturation.Index = 1;
            this.menuFiltersSaturation.Text = "Saturation";
            this.menuFiltersSaturation.Click += new System.EventHandler(this.menuFiltersSaturation_Click);
            // 
            // menuFiltersBlur
            // 
            this.menuFiltersBlur.Index = 2;
            this.menuFiltersBlur.Text = "Blur";
            this.menuFiltersBlur.Click += new System.EventHandler(this.menuFiltersBlur_Click);
            // 
            // menuFiltersGaussianBlur
            // 
            this.menuFiltersGaussianBlur.Index = 3;
            this.menuFiltersGaussianBlur.Text = "Gaussian Blur";
            this.menuFiltersGaussianBlur.Click += new System.EventHandler(this.menuFiltersGaussianBlur_Click);
            // 
            // menuAdaptiveUnsharpMask
            // 
            this.menuAdaptiveUnsharpMask.Index = 4;
            this.menuAdaptiveUnsharpMask.Text = "Adaptive Unsharp Mask";
            this.menuAdaptiveUnsharpMask.Click += new System.EventHandler(this.menuAdaptiveUnsharpMask_Click);
            // 
            // menuEffectsUnsharpMask
            // 
            this.menuEffectsUnsharpMask.Index = 5;
            this.menuEffectsUnsharpMask.Text = "Unsharp Mask";
            this.menuEffectsUnsharpMask.Click += new System.EventHandler(this.menuEffectsUnsharpMask_Click);
            // 
            // menuFiltersSharpen
            // 
            this.menuFiltersSharpen.Index = 6;
            this.menuFiltersSharpen.Text = "Sharpen";
            this.menuFiltersSharpen.Click += new System.EventHandler(this.menuFiltersSharpen_Click);
            // 
            // menuItem58
            // 
            this.menuItem58.Index = 7;
            this.menuItem58.Text = "-";
            // 
            // menuFiltersAddNoise
            // 
            this.menuFiltersAddNoise.Index = 8;
            this.menuFiltersAddNoise.Text = "Add Noise";
            this.menuFiltersAddNoise.Click += new System.EventHandler(this.menuFiltersAddNoise_Click);
            // 
            // menuFiltersDespeckle
            // 
            this.menuFiltersDespeckle.Index = 9;
            this.menuFiltersDespeckle.Text = "Despeckle";
            this.menuFiltersDespeckle.Click += new System.EventHandler(this.menuFiltersDespeckle_Click);
            // 
            // menuFiltersEmboss
            // 
            this.menuFiltersEmboss.Index = 10;
            this.menuFiltersEmboss.Text = "Emboss";
            this.menuFiltersEmboss.Click += new System.EventHandler(this.menuFiltersEmboss_Click);
            // 
            // menuFiltersIntensify
            // 
            this.menuFiltersIntensify.Index = 11;
            this.menuFiltersIntensify.Text = "Intensify";
            this.menuFiltersIntensify.Click += new System.EventHandler(this.menuFiltersIntensify_Click);
            // 
            // menuFiltersHighPass
            // 
            this.menuFiltersHighPass.Index = 12;
            this.menuFiltersHighPass.Text = "High Pass";
            this.menuFiltersHighPass.Click += new System.EventHandler(this.menuFiltersHighPass_Click);
            // 
            // menuFiltersMaximum
            // 
            this.menuFiltersMaximum.Index = 13;
            this.menuFiltersMaximum.Text = "Maximum";
            this.menuFiltersMaximum.Click += new System.EventHandler(this.menuFiltersMaximum_Click);
            // 
            // menuFiltersMean
            // 
            this.menuFiltersMean.Index = 14;
            this.menuFiltersMean.Text = "Mean";
            this.menuFiltersMean.Click += new System.EventHandler(this.menuFiltersMean_Click);
            // 
            // menuFiltersMedian
            // 
            this.menuFiltersMedian.Index = 15;
            this.menuFiltersMedian.Text = "Median";
            this.menuFiltersMedian.Click += new System.EventHandler(this.menuFiltersMedian_Click);
            // 
            // menuFiltersMidpoint
            // 
            this.menuFiltersMidpoint.Index = 16;
            this.menuFiltersMidpoint.Text = "Midpoint";
            this.menuFiltersMidpoint.Click += new System.EventHandler(this.menuFiltersMidpoint_Click);
            // 
            // menuFiltersMinimum
            // 
            this.menuFiltersMinimum.Index = 17;
            this.menuFiltersMinimum.Text = "Minimum";
            this.menuFiltersMinimum.Click += new System.EventHandler(this.menuFiltersMinimum_Click);
            // 
            // menuFiltersMorphological
            // 
            this.menuFiltersMorphological.Index = 18;
            this.menuFiltersMorphological.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuMorphoDilation,
            this.menuMorphoErosion,
            this.menuMorphoOpen,
            this.menuMorphoClose,
            this.menuMorphoTophat,
            this.menuMorphoGradient});
            this.menuFiltersMorphological.Text = "Morphological";
            this.menuFiltersMorphological.Click += new System.EventHandler(this.menuFiltersMorphological_Click);
            // 
            // menuMorphoDilation
            // 
            this.menuMorphoDilation.Index = 0;
            this.menuMorphoDilation.Text = "Dilation";
            this.menuMorphoDilation.Click += new System.EventHandler(this.menuFiltersMorphological_Click);
            // 
            // menuMorphoErosion
            // 
            this.menuMorphoErosion.Index = 1;
            this.menuMorphoErosion.Text = "Erosion";
            this.menuMorphoErosion.Click += new System.EventHandler(this.menuFiltersMorphological_Click);
            // 
            // menuMorphoOpen
            // 
            this.menuMorphoOpen.Index = 2;
            this.menuMorphoOpen.Text = "Open";
            this.menuMorphoOpen.Click += new System.EventHandler(this.menuFiltersMorphological_Click);
            // 
            // menuMorphoClose
            // 
            this.menuMorphoClose.Index = 3;
            this.menuMorphoClose.Text = "Close";
            this.menuMorphoClose.Click += new System.EventHandler(this.menuFiltersMorphological_Click);
            // 
            // menuMorphoTophat
            // 
            this.menuMorphoTophat.Index = 4;
            this.menuMorphoTophat.Text = "Tophat";
            this.menuMorphoTophat.Click += new System.EventHandler(this.menuFiltersMorphological_Click);
            // 
            // menuMorphoGradient
            // 
            this.menuMorphoGradient.Index = 5;
            this.menuMorphoGradient.Text = "Gradient";
            this.menuMorphoGradient.Click += new System.EventHandler(this.menuFiltersMorphological_Click);
            // 
            // menuFiltersThreshold
            // 
            this.menuFiltersThreshold.Index = 19;
            this.menuFiltersThreshold.Text = "Threshold";
            this.menuFiltersThreshold.Click += new System.EventHandler(this.menuFiltersThreshold_Click);
            // 
            // menuItem70
            // 
            this.menuItem70.Index = 20;
            this.menuItem70.Text = "-";
            // 
            // menuFiltersConvolutionFilter
            // 
            this.menuFiltersConvolutionFilter.Index = 21;
            this.menuFiltersConvolutionFilter.Text = "Convolution Filter";
            this.menuFiltersConvolutionFilter.Click += new System.EventHandler(this.menuFiltersConvolutionFilter_Click);
            // 
            // menuFiltersConvolutionMatrix
            // 
            this.menuFiltersConvolutionMatrix.Index = 22;
            this.menuFiltersConvolutionMatrix.Text = "Convolution Matrix";
            this.menuFiltersConvolutionMatrix.Click += new System.EventHandler(this.menuFiltersConvolutionMatrix_Click);
            // 
            // menuItem73
            // 
            this.menuItem73.Index = 23;
            this.menuItem73.Text = "-";
            // 
            // menuFiltersEdge
            // 
            this.menuFiltersEdge.Index = 24;
            this.menuFiltersEdge.Text = "Edge Detection";
            this.menuFiltersEdge.Click += new System.EventHandler(this.menuFiltersEdge_Click);
            // 
            // menuFiltersCannyEdgeDetector
            // 
            this.menuFiltersCannyEdgeDetector.Index = 25;
            this.menuFiltersCannyEdgeDetector.Text = "Canny Edge Detector";
            this.menuFiltersCannyEdgeDetector.Click += new System.EventHandler(this.menuFiltersCannyEdgeDetector_Click);
            // 
            // menuFiltersDustScratchRemoval
            // 
            this.menuFiltersDustScratchRemoval.Index = 26;
            this.menuFiltersDustScratchRemoval.Text = "Dust && Scratch Removal";
            this.menuFiltersDustScratchRemoval.Click += new System.EventHandler(this.menuFiltersDustScratchRemoval_Click);
            // 
            // menuTransforms
            // 
            this.menuTransforms.Index = 3;
            this.menuTransforms.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuTransformsChain,
            this.menuTransformsApply,
            this.menuItem77,
            this.menuTransformsBumpMap,
            this.menuTransformsElliptical,
            this.menuTransformsLens,
            this.menuLineSlice,
            this.menuTransformsMarble,
            this.menuTransformsOffset,
            this.menuTransformsPerlin,
            this.menuTransformsPinch,
            this.menuTransformsPolygon,
            this.menuTransformsRandom,
            this.menuTransformsRipple,
            this.menuTransformsSpin,
            this.menuTransformSpinWave,
            this.menuTransformsWave,
            this.menuTransformsWow,
            this.menuTransformsZigZag,
            this.menuItem94,
            this.menuTransformsUser});
            this.menuTransforms.Text = "Transforms";
            // 
            // menuTransformsChain
            // 
            this.menuTransformsChain.Index = 0;
            this.menuTransformsChain.Text = "Chain Transforms Together";
            this.menuTransformsChain.Click += new System.EventHandler(this.menuTransformsChain_Click);
            // 
            // menuTransformsApply
            // 
            this.menuTransformsApply.Enabled = false;
            this.menuTransformsApply.Index = 1;
            this.menuTransformsApply.Text = "Apply Transformation Chain";
            this.menuTransformsApply.Click += new System.EventHandler(this.menuTransformsApply_Click);
            // 
            // menuItem77
            // 
            this.menuItem77.Index = 2;
            this.menuItem77.Text = "-";
            // 
            // menuTransformsBumpMap
            // 
            this.menuTransformsBumpMap.Index = 3;
            this.menuTransformsBumpMap.Text = "Bump Map";
            this.menuTransformsBumpMap.Click += new System.EventHandler(this.menuTransformsBumpMap_Click);
            // 
            // menuTransformsElliptical
            // 
            this.menuTransformsElliptical.Index = 4;
            this.menuTransformsElliptical.Text = "Elliptical";
            this.menuTransformsElliptical.Click += new System.EventHandler(this.menuTransformsElliptical_Click);
            // 
            // menuTransformsLens
            // 
            this.menuTransformsLens.Index = 5;
            this.menuTransformsLens.Text = "Lens";
            this.menuTransformsLens.Click += new System.EventHandler(this.menuTransformsLens_Click);
            // 
            // menuLineSlice
            // 
            this.menuLineSlice.Index = 6;
            this.menuLineSlice.Text = "Line Slice";
            this.menuLineSlice.Click += new System.EventHandler(this.menuLineSlice_Click);
            // 
            // menuTransformsMarble
            // 
            this.menuTransformsMarble.Index = 7;
            this.menuTransformsMarble.Text = "Marble";
            this.menuTransformsMarble.Click += new System.EventHandler(this.menuTransformsMarble_Click);
            // 
            // menuTransformsOffset
            // 
            this.menuTransformsOffset.Index = 8;
            this.menuTransformsOffset.Text = "Offset";
            this.menuTransformsOffset.Click += new System.EventHandler(this.menuTransformsOffset_Click);
            // 
            // menuTransformsPerlin
            // 
            this.menuTransformsPerlin.Index = 9;
            this.menuTransformsPerlin.Text = "Perlin";
            this.menuTransformsPerlin.Click += new System.EventHandler(this.menuTransformsPerlin_Click);
            // 
            // menuTransformsPinch
            // 
            this.menuTransformsPinch.Index = 10;
            this.menuTransformsPinch.Text = "Pinch";
            this.menuTransformsPinch.Click += new System.EventHandler(this.menuTransformsPinch_Click);
            // 
            // menuTransformsPolygon
            // 
            this.menuTransformsPolygon.Index = 11;
            this.menuTransformsPolygon.Text = "Polygon";
            this.menuTransformsPolygon.Click += new System.EventHandler(this.menuTransformsPolygon_Click);
            // 
            // menuTransformsRandom
            // 
            this.menuTransformsRandom.Index = 12;
            this.menuTransformsRandom.Text = "Random";
            this.menuTransformsRandom.Click += new System.EventHandler(this.menuTransformsRandom_Click);
            // 
            // menuTransformsRipple
            // 
            this.menuTransformsRipple.Index = 13;
            this.menuTransformsRipple.Text = "Ripple";
            this.menuTransformsRipple.Click += new System.EventHandler(this.menuTransformsRipple_Click);
            // 
            // menuTransformsSpin
            // 
            this.menuTransformsSpin.Index = 14;
            this.menuTransformsSpin.Text = "Spin";
            this.menuTransformsSpin.Click += new System.EventHandler(this.menuTransformsSpin_Click);
            // 
            // menuTransformSpinWave
            // 
            this.menuTransformSpinWave.Index = 15;
            this.menuTransformSpinWave.Text = "Spin Wave";
            this.menuTransformSpinWave.Click += new System.EventHandler(this.menuTransformSpinWave_Click);
            // 
            // menuTransformsWave
            // 
            this.menuTransformsWave.Index = 16;
            this.menuTransformsWave.Text = "Wave";
            this.menuTransformsWave.Click += new System.EventHandler(this.menuTransformsWave_Click);
            // 
            // menuTransformsWow
            // 
            this.menuTransformsWow.Index = 17;
            this.menuTransformsWow.Text = "Wow";
            this.menuTransformsWow.Click += new System.EventHandler(this.menuTransformsWow_Click);
            // 
            // menuTransformsZigZag
            // 
            this.menuTransformsZigZag.Index = 18;
            this.menuTransformsZigZag.Text = "Zig Zag";
            this.menuTransformsZigZag.Click += new System.EventHandler(this.menuTransformsZigZag_Click);
            // 
            // menuItem94
            // 
            this.menuItem94.Index = 19;
            this.menuItem94.Text = "-";
            // 
            // menuTransformsUser
            // 
            this.menuTransformsUser.Index = 20;
            this.menuTransformsUser.Text = "User Transform Demo";
            this.menuTransformsUser.Click += new System.EventHandler(this.menuTransformsUser_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 4;
            this.menuItem10.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAutomaticDocumentNegation,
            this.menuItemBinarySegmentation,
            this.menuItemBlackBorderCrop,
            this.menuItemBlankPageDetection,
            this.menuItemBlobRemoval,
            this.menuItemBorderRemoval,
            this.menuItemDeskew,
            this.menuItemDespeckle,
            this.menuItemHalftoneRemoval,
            this.menuItemHolePunchRemoval,
            this.menuItemInvertedTextCorrection,
            this.menuItemLineRemoval,
            this.menuItemSpeckRemoval,
            this.menuItemWhiteMarginCrop});
            this.menuItem10.Text = "ADC";
            // 
            // menuItemAutomaticDocumentNegation
            // 
            this.menuItemAutomaticDocumentNegation.Index = 0;
            this.menuItemAutomaticDocumentNegation.Text = "Automatic Document Negation";
            this.menuItemAutomaticDocumentNegation.Click += new System.EventHandler(this.menuItemAutomaticDocumentNegation_Click);
            // 
            // menuItemBinarySegmentation
            // 
            this.menuItemBinarySegmentation.Index = 1;
            this.menuItemBinarySegmentation.Text = "Binary Segmentation";
            this.menuItemBinarySegmentation.Click += new System.EventHandler(this.menuItemBinarySegmentation_Click);
            // 
            // menuItemBlackBorderCrop
            // 
            this.menuItemBlackBorderCrop.Index = 2;
            this.menuItemBlackBorderCrop.Text = "Black Border Crop";
            this.menuItemBlackBorderCrop.Click += new System.EventHandler(this.menuItemBlackBorderCrop_Click);
            // 
            // menuItemBlankPageDetection
            // 
            this.menuItemBlankPageDetection.Index = 3;
            this.menuItemBlankPageDetection.Text = "Blank Page Detection";
            this.menuItemBlankPageDetection.Click += new System.EventHandler(this.menuItemBlankPageDetection_Click);
            // 
            // menuItemBlobRemoval
            // 
            this.menuItemBlobRemoval.Index = 4;
            this.menuItemBlobRemoval.Text = "Blob Removal";
            this.menuItemBlobRemoval.Click += new System.EventHandler(this.menuItemBlobRemoval_Click);
            // 
            // menuItemBorderRemoval
            // 
            this.menuItemBorderRemoval.Index = 5;
            this.menuItemBorderRemoval.Text = "Border Removal";
            this.menuItemBorderRemoval.Click += new System.EventHandler(this.menuItemBorderRemoval_Click);
            // 
            // menuItemDeskew
            // 
            this.menuItemDeskew.Index = 6;
            this.menuItemDeskew.Text = "Deskew";
            this.menuItemDeskew.Click += new System.EventHandler(this.menuItemDeskew_Click);
            // 
            // menuItemDespeckle
            // 
            this.menuItemDespeckle.Index = 7;
            this.menuItemDespeckle.Text = "Despeckle";
            this.menuItemDespeckle.Click += new System.EventHandler(this.menuItemDespeckle_Click);
            // 
            // menuItemHalftoneRemoval
            // 
            this.menuItemHalftoneRemoval.Index = 8;
            this.menuItemHalftoneRemoval.Text = "Halftone Removal";
            this.menuItemHalftoneRemoval.Click += new System.EventHandler(this.menuItemHalftoneRemoval_Click);
            // 
            // menuItemHolePunchRemoval
            // 
            this.menuItemHolePunchRemoval.Index = 9;
            this.menuItemHolePunchRemoval.Text = "Hole Punch Removal";
            this.menuItemHolePunchRemoval.Click += new System.EventHandler(this.menuItemHolePunchRemoval_Click);
            // 
            // menuItemInvertedTextCorrection
            // 
            this.menuItemInvertedTextCorrection.Index = 10;
            this.menuItemInvertedTextCorrection.Text = "Inverted Text Correction";
            this.menuItemInvertedTextCorrection.Click += new System.EventHandler(this.menuItemInvertedTextCorrection_Click);
            // 
            // menuItemLineRemoval
            // 
            this.menuItemLineRemoval.Index = 11;
            this.menuItemLineRemoval.Text = "Line Removal";
            this.menuItemLineRemoval.Click += new System.EventHandler(this.menuItemLineRemoval_Click);
            // 
            // menuItemSpeckRemoval
            // 
            this.menuItemSpeckRemoval.Index = 12;
            this.menuItemSpeckRemoval.Text = "Speck Removal";
            this.menuItemSpeckRemoval.Click += new System.EventHandler(this.menuItemSpeckRemoval_Click);
            // 
            // menuItemWhiteMarginCrop
            // 
            this.menuItemWhiteMarginCrop.Index = 13;
            this.menuItemWhiteMarginCrop.Text = "White Margin Crop";
            this.menuItemWhiteMarginCrop.Click += new System.EventHandler(this.menuItemWhiteMarginCrop_Click);
            // 
            // menuDocument
            // 
            this.menuDocument.Index = 5;
            this.menuDocument.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDocumentAutoDeskew,
            this.menuDocumentMedian,
            this.menuDocumentDespeckle,
            this.menuItem103,
            this.menuDocumentMorphological,
            this.menuDocumentThinning,
            this.menuDocumentHitOrMiss,
            this.menuThresholding,
            this.menuBorderRemoval,
            this.menuDithering});
            this.menuDocument.Text = "Document";
            // 
            // menuDocumentAutoDeskew
            // 
            this.menuDocumentAutoDeskew.Index = 0;
            this.menuDocumentAutoDeskew.Text = "Auto Deskew";
            this.menuDocumentAutoDeskew.Click += new System.EventHandler(this.menuDocumentAutoDeskew_Click);
            // 
            // menuDocumentMedian
            // 
            this.menuDocumentMedian.Index = 1;
            this.menuDocumentMedian.Text = "Remove Noise (Median)";
            this.menuDocumentMedian.Click += new System.EventHandler(this.menuDocumentMedian_Click);
            // 
            // menuDocumentDespeckle
            // 
            this.menuDocumentDespeckle.Index = 2;
            this.menuDocumentDespeckle.Text = "Despeckle";
            this.menuDocumentDespeckle.Click += new System.EventHandler(this.menuDocumentDespeckle_Click);
            // 
            // menuItem103
            // 
            this.menuItem103.Index = 3;
            this.menuItem103.Text = "-";
            // 
            // menuDocumentMorphological
            // 
            this.menuDocumentMorphological.Index = 4;
            this.menuDocumentMorphological.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuBinaryDilation,
            this.menuBinaryErosion,
            this.menuBinaryOpen,
            this.menuBinaryClose,
            this.menuBinaryBoundary});
            this.menuDocumentMorphological.Text = "Morphological";
            this.menuDocumentMorphological.Click += new System.EventHandler(this.menuDocumentMorphological_Click);
            // 
            // menuBinaryDilation
            // 
            this.menuBinaryDilation.Index = 0;
            this.menuBinaryDilation.Text = "Dilation";
            this.menuBinaryDilation.Click += new System.EventHandler(this.menuDocumentMorphological_Click);
            // 
            // menuBinaryErosion
            // 
            this.menuBinaryErosion.Index = 1;
            this.menuBinaryErosion.Text = "Erosion";
            this.menuBinaryErosion.Click += new System.EventHandler(this.menuDocumentMorphological_Click);
            // 
            // menuBinaryOpen
            // 
            this.menuBinaryOpen.Index = 2;
            this.menuBinaryOpen.Text = "Open";
            this.menuBinaryOpen.Click += new System.EventHandler(this.menuDocumentMorphological_Click);
            // 
            // menuBinaryClose
            // 
            this.menuBinaryClose.Index = 3;
            this.menuBinaryClose.Text = "Close";
            this.menuBinaryClose.Click += new System.EventHandler(this.menuDocumentMorphological_Click);
            // 
            // menuBinaryBoundary
            // 
            this.menuBinaryBoundary.Index = 4;
            this.menuBinaryBoundary.Text = "Boundary Extraction";
            this.menuBinaryBoundary.Click += new System.EventHandler(this.menuDocumentMorphological_Click);
            // 
            // menuDocumentThinning
            // 
            this.menuDocumentThinning.Index = 5;
            this.menuDocumentThinning.Text = "Thinning";
            this.menuDocumentThinning.Click += new System.EventHandler(this.menuDocumentThinning_Click);
            // 
            // menuDocumentHitOrMiss
            // 
            this.menuDocumentHitOrMiss.Index = 6;
            this.menuDocumentHitOrMiss.Text = "Hit or Miss";
            this.menuDocumentHitOrMiss.Click += new System.EventHandler(this.menuDocumentHitOrMiss_Click);
            // 
            // menuThresholding
            // 
            this.menuThresholding.Index = 7;
            this.menuThresholding.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuThresholdAdaptive,
            this.menuThresholdGlobal,
            this.menuThresholdDynamic});
            this.menuThresholding.Text = "Thresholding";
            // 
            // menuThresholdAdaptive
            // 
            this.menuThresholdAdaptive.Index = 0;
            this.menuThresholdAdaptive.Text = "Adaptive";
            this.menuThresholdAdaptive.Click += new System.EventHandler(this.menuThresholdAdaptive_Click);
            // 
            // menuThresholdGlobal
            // 
            this.menuThresholdGlobal.Index = 1;
            this.menuThresholdGlobal.Text = "Global";
            this.menuThresholdGlobal.Click += new System.EventHandler(this.menuThresholdGlobal_Click);
            // 
            // menuThresholdDynamic
            // 
            this.menuThresholdDynamic.Index = 2;
            this.menuThresholdDynamic.Text = "Dynamic";
            this.menuThresholdDynamic.Click += new System.EventHandler(this.menuThresholdDynamic_Click);
            // 
            // menuBorderRemoval
            // 
            this.menuBorderRemoval.Index = 8;
            this.menuBorderRemoval.Text = "Border Removal";
            this.menuBorderRemoval.Click += new System.EventHandler(this.menuBorderRemoval_Click);
            // 
            // menuDithering
            // 
            this.menuDithering.Index = 9;
            this.menuDithering.Text = "Dithering";
            this.menuDithering.Click += new System.EventHandler(this.menuDithering_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 6;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFftBandPass,
            this.menuFftButterworthHighBoost,
            this.menuFftButterworthHighPass,
            this.menuFftButterworthLowPass,
            this.menuFftGaussianHighBoost,
            this.menuFftGaussianHighPass,
            this.menuFftGaussianLowPass,
            this.menuFftIdealHighPass,
            this.menuFftIdealLowPass,
            this.menuFftInversePower});
            this.menuItem3.Text = "FFT";
            // 
            // menuFftBandPass
            // 
            this.menuFftBandPass.Index = 0;
            this.menuFftBandPass.Text = "Band Pass Filter";
            this.menuFftBandPass.Click += new System.EventHandler(this.menuFftBandPass_Click);
            // 
            // menuFftButterworthHighBoost
            // 
            this.menuFftButterworthHighBoost.Index = 1;
            this.menuFftButterworthHighBoost.Text = "Butterworth High Boost";
            this.menuFftButterworthHighBoost.Click += new System.EventHandler(this.menuFftButterworthHighBoost_Click);
            // 
            // menuFftButterworthHighPass
            // 
            this.menuFftButterworthHighPass.Index = 2;
            this.menuFftButterworthHighPass.Text = "Butterworth High Pass";
            this.menuFftButterworthHighPass.Click += new System.EventHandler(this.menuFftButterworthHighPass_Click);
            // 
            // menuFftButterworthLowPass
            // 
            this.menuFftButterworthLowPass.Index = 3;
            this.menuFftButterworthLowPass.Text = "Butterworth Low Pass ";
            this.menuFftButterworthLowPass.Click += new System.EventHandler(this.menuFftButterworthLowPass_Click);
            // 
            // menuFftGaussianHighBoost
            // 
            this.menuFftGaussianHighBoost.Index = 4;
            this.menuFftGaussianHighBoost.Text = "Gaussian High Boost";
            this.menuFftGaussianHighBoost.Click += new System.EventHandler(this.menuFftGaussianHighBoost_Click);
            // 
            // menuFftGaussianHighPass
            // 
            this.menuFftGaussianHighPass.Index = 5;
            this.menuFftGaussianHighPass.Text = "Gaussian High Pass";
            this.menuFftGaussianHighPass.Click += new System.EventHandler(this.menuFftGaussianHighPass_Click);
            // 
            // menuFftGaussianLowPass
            // 
            this.menuFftGaussianLowPass.Index = 6;
            this.menuFftGaussianLowPass.Text = "Gaussian Low Pass";
            this.menuFftGaussianLowPass.Click += new System.EventHandler(this.menuFftGaussianLowPass_Click);
            // 
            // menuFftIdealHighPass
            // 
            this.menuFftIdealHighPass.Index = 7;
            this.menuFftIdealHighPass.Text = "Ideal High Pass";
            this.menuFftIdealHighPass.Click += new System.EventHandler(this.menuFftIdealHighPass_Click);
            // 
            // menuFftIdealLowPass
            // 
            this.menuFftIdealLowPass.Index = 8;
            this.menuFftIdealLowPass.Text = "Ideal Low Pass";
            this.menuFftIdealLowPass.Click += new System.EventHandler(this.menuFftIdealLowPass_Click);
            // 
            // menuFftInversePower
            // 
            this.menuFftInversePower.Index = 9;
            this.menuFftInversePower.Text = "Inverse Power";
            this.menuFftInversePower.Click += new System.EventHandler(this.menuFftInversePower_Click);
            // 
            // menuItem16
            // 
            this.menuItem16.Index = 7;
            this.menuItem16.Text = "-";
            // 
            // menuFlip
            // 
            this.menuFlip.Index = 8;
            this.menuFlip.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFlipHorizontal,
            this.menuFlipVertical});
            this.menuFlip.Text = "Flip";
            // 
            // menuFlipHorizontal
            // 
            this.menuFlipHorizontal.Index = 0;
            this.menuFlipHorizontal.Text = "Horizontal";
            this.menuFlipHorizontal.Click += new System.EventHandler(this.menuFlipHorizontal_Click);
            // 
            // menuFlipVertical
            // 
            this.menuFlipVertical.Index = 1;
            this.menuFlipVertical.Text = "Vertical";
            this.menuFlipVertical.Click += new System.EventHandler(this.menuFlipVertical_Click);
            // 
            // menuItem23
            // 
            this.menuItem23.Index = 9;
            this.menuItem23.Text = "-";
            // 
            // menuRotate
            // 
            this.menuRotate.Index = 10;
            this.menuRotate.Text = "Rotate";
            this.menuRotate.Click += new System.EventHandler(this.menuRotate_Click);
            // 
            // menuResizeCanvas
            // 
            this.menuResizeCanvas.Index = 11;
            this.menuResizeCanvas.Text = "Resize Canvas";
            this.menuResizeCanvas.Click += new System.EventHandler(this.menuResizeCanvas_Click);
            // 
            // menuResample
            // 
            this.menuResample.Index = 12;
            this.menuResample.Text = "Resample";
            this.menuResample.Click += new System.EventHandler(this.menuResample_Click);
            // 
            // menuCrop
            // 
            this.menuCrop.Index = 13;
            this.menuCrop.Text = "Crop";
            this.menuCrop.Click += new System.EventHandler(this.menuCrop_Click);
            // 
            // menuAutoCrop
            // 
            this.menuAutoCrop.Index = 14;
            this.menuAutoCrop.Text = "Auto Crop";
            this.menuAutoCrop.Click += new System.EventHandler(this.menuAutoCrop_Click);
            // 
            // menuSkew
            // 
            this.menuSkew.Index = 15;
            this.menuSkew.Text = "Skew";
            this.menuSkew.Click += new System.EventHandler(this.menuSkew_Click);
            // 
            // menuQuadrilateralWarp
            // 
            this.menuQuadrilateralWarp.Index = 16;
            this.menuQuadrilateralWarp.Text = "Quadrilateral Warp";
            this.menuQuadrilateralWarp.Click += new System.EventHandler(this.menuQuadrilateralWarp_Click);
            // 
            // menuPush
            // 
            this.menuPush.Index = 17;
            this.menuPush.Text = "Push";
            this.menuPush.Click += new System.EventHandler(this.menuPush_Click);
            // 
            // menuItem28
            // 
            this.menuItem28.Index = 18;
            this.menuItem28.Text = "-";
            // 
            // menuOverlay
            // 
            this.menuOverlay.Index = 19;
            this.menuOverlay.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuOverlayNormal,
            this.menuOverlayMasked,
            this.menuOverlayMerged});
            this.menuOverlay.Text = "Overlay";
            this.menuOverlay.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // menuOverlayNormal
            // 
            this.menuOverlayNormal.Index = 0;
            this.menuOverlayNormal.Text = "Normal";
            this.menuOverlayNormal.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // menuOverlayMasked
            // 
            this.menuOverlayMasked.Index = 1;
            this.menuOverlayMasked.Text = "Masked";
            this.menuOverlayMasked.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // menuOverlayMerged
            // 
            this.menuOverlayMerged.Index = 2;
            this.menuOverlayMerged.Text = "Merge Options";
            this.menuOverlayMerged.Click += new System.EventHandler(this.menuOverlay_Click);
            // 
            // menuItem19
            // 
            this.menuItem19.Index = 20;
            this.menuItem19.Text = "-";
            // 
            // menuHelp
            // 
            this.menuHelp.Index = 6;
            this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem15});
            this.menuHelp.Text = "&Help";
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 0;
            this.menuItem15.Text = "About ...";
            this.menuItem15.Click += new System.EventHandler(this.menuItem15_Click);
            // 
            // cboFrameIndex
            // 
            this.cboFrameIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFrameIndex.Location = new System.Drawing.Point(284, 2);
            this.cboFrameIndex.Name = "cboFrameIndex";
            this.cboFrameIndex.Size = new System.Drawing.Size(132, 21);
            this.cboFrameIndex.TabIndex = 6;
            this.cboFrameIndex.SelectedIndexChanged += new System.EventHandler(this.cboFrameIndex_SelectedIndexChanged);
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(857, 473);
            this.Controls.Add(this.cboFrameIndex);
            this.Controls.Add(this.Viewer);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.statusInfo);
            this.Menu = this.mainMenu;
            this.Name = "FormMain";
            this.Text = "Atalasoft dotImage Demo";
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarLoadTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarProgress)).EndInit();
            this.ResumeLayout(false);

		}
		
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FormMain());
		}
		#endregion


		#region Viewer Events
		
		private void Viewer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (Viewer.Selection.Active)
					Viewer.Selection.Visible = false;
				this.lineDraw.Cancel();
				this.rectangleDraw.Cancel();
				this.ellipseDraw.Cancel();

				if (this.drawMode == DrawMenuMode.Polygon)
				{
					// We undo so the lines will not draw over eachother.
					// Another option is to simply draw the last connection line,
					// but then we couldn't fill the polygon directly.
					this.Viewer.Undos.Undo();
					Viewer.Undos.Add("Draw Polygon", true);

					Canvas myCanvas = new Canvas(this.Viewer.Image);
					if (this.polygonPoints != null && this.polygonPoints.Length > 1)
					{
						myCanvas.DrawPolygon(this.polygonPoints, this.lineDraw.Pen, this.rectangleDraw.Fill);
					}
					this.polygonPoints = null;
					this.Viewer.Refresh();
				}

				this.drawMode = DrawMenuMode.None;
				this.lineDraw.Active = false;
				this.rectangleDraw.Active = false;
				this.ellipseDraw.Active = false;
			}
			else if ((e.Button == MouseButtons.Left) && (this.drawMode == DrawMenuMode.Freehand))
				//start the freehand drawing
				this.polygonPoints = new Point[1] {new Point((int)((e.X / Viewer.Zoom) - (Viewer.ImagePosition.X / Viewer.Zoom)), (int)((e.Y / Viewer.Zoom) - (Viewer.ImagePosition.Y / Viewer.Zoom)))};
		}

		private void Viewer_MouseMovePixel(object sender, MouseEventArgs e)
		{
			if (Viewer.Selection.Visible) 
			{
				Rectangle rc = Viewer.Selection.Bounds;
				statusBarPosition.Text = "Selection:  " + rc.Left + ", " + rc.Top + ", " + rc.Width + ", " + rc.Height;
			} 
			else 
			{
				string pos = "Position:  " + e?.X.ToString() + " x " + e?.Y.ToString();
				statusBarPosition.Text = pos;
			}
			statusBarPosition.ToolTipText = statusBarPosition.Text;

			if ((this.drawMode == DrawMenuMode.Freehand) && (e.Button == MouseButtons.Left))
			{
				Canvas myCanvas = new Canvas(Viewer.Image);
				myCanvas.DrawLine(this.polygonPoints[0], new Point(e.X, e.Y), this.lineDraw.Pen);
				this.polygonPoints[0] = new Point(e.X, e.Y);
				Viewer.Refresh();
			}
		}
		
		private void Viewer_Progress(object sender, ProgressEventArgs e)
		{
			if (e.Total == 0) 
				e.Total = 1;
			progressBar1.Value = e.Current * 100 / e.Total;
			if (progressBar1.Value == 100)
				progressBar1.Value = 0;

		}

		//handle this event to close the memory stream when complete when opening and saving to/from memory
		private void Viewer_ImageStreamCompleted(object sender, Atalasoft.Imaging.ImageStreamEventArgs e)
		{
			e.Stream.Close();
		}
		
		private void Viewer_ProcessError(object sender, Atalasoft.Imaging.ExceptionEventArgs e)
		{
			MessageBox.Show(this, e.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			this.isProcessError = true;
		}
		
		private void Viewer_ChangedImage(object sender, Atalasoft.Imaging.ImageEventArgs e)
		{
			if (e.Image == null) return;
			statusBarMessage.Text = e.Image.ToString();
			Viewer.Update();
			
			EnableMenuItems();
			if (e.Image != null) statusBarMessage.Text = e.Image.ToString();

			UpdateUndoRedoInfo();

			if (this.histogram != null)
				this.histogram.SetHistogram(e.Image);
		}

		private void Viewer_ProcessCompleted(object sender, ImageEventArgs e)
		{
			if (this.isProcessError)
			{
				this.isProcessError = false;
				return;
			}
			if (this.performingOpen)
			{
				DisplayLoadTime();
				this.performingOpen = false;
				ReadMetadata();
			}
		}

        // replaced with MainForm events
		//private void Viewer_MouseWheel(object sender, MouseEventArgs e)
		//{
		//	if (this.Viewer.Image == null) return;

		//	Point pt = this.Viewer.ScrollPosition;
		//	int jump = (this.Viewer.ClientSize.Height / 10);

		//	if (e.Delta < 0)
		//		pt.Y -= jump;
		//	else
		//		pt.Y += jump;

		//	if (pt.Y > 0) pt.Y = 0;
		//	this.Viewer.ScrollPosition = pt;
		//}

		private void AtalaImage_ChangePixelFormat(object sender, PixelFormatChangeEventArgs e)
		{
			// Make sure this event is raised in the UI thread.
			if (this.InvokeRequired)
			{
				this.Invoke(new PixelFormatChangeEventHandler(AtalaImage_ChangePixelFormat), new object[] { sender, e });
			}
			else
			{
				// Disable this dialog if using the magnifier with 4-bit images.
				if (this.Viewer.Magnifier.Active && this.Viewer.Image.PixelFormat == PixelFormat.Pixel4bppIndexed) return;
			
				// Ask if it's ok to change the pixel format.
				e.Cancel = (bool)(MessageBox.Show(this, "This action requires the pixel format to be changed:\n\nCurrent: \t" + e.CurrentPixelFormat.ToString() + "\nNew: \t" + e.NewPixelFormat.ToString() + "\n\nDo you want to continue?", "Change Pixel Format", MessageBoxButtons.YesNo) == DialogResult.No);
			}
		}
		
		#endregion
		
		#region Form Events

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch(e.Button.ToolTipText)
			{
				case "Open":
					OpenAndLoadImage();
					break;
					
				case "Save":
					SaveCurrentImage(null);
					break;
				
				case "Undo":
					Viewer.Undos.Undo();
					UpdateUndoRedoInfo();
					break;
				
				case "Redo":
					Viewer.Undos.Redo();
					UpdateUndoRedoInfo();
					break;
					
				case "Arrow":
					ClearMouseTools();
					tbArrow.Pushed = true;
					break;
				case "Rectangle Selection":
					ClearMouseTools();
					tbSelectRectangle.Pushed = true;
					Viewer.MouseTool = MouseToolType.Selection;
					Viewer.Selection = this.rectangleSelection;
					break;
				case "Ellipse Selection":
					ClearMouseTools();
					tbSelectEllipse.Pushed = true;
					Viewer.MouseTool = MouseToolType.Selection;
					Viewer.Selection = this.ellipseRubberband;
					break;
				case "Pan":
					ClearMouseTools();
					tbPan.Pushed = true;
					Viewer.MouseTool = MouseToolType.Pan;
					break;
				case "Magnifier":
					ClearMouseTools();
					tbMagnifier.Pushed = true;
                    Viewer.AutoZoom = AutoZoomMode.None;
                    Viewer.MouseTool = MouseToolType.Magnifier;
					break;
				case "Zoom":
					ClearMouseTools();
					tbZoom.Pushed = true;
                    Viewer.AutoZoom = AutoZoomMode.None;
                    Viewer.MouseTool = MouseToolType.Zoom;
					break;
				case "Zoom Selection":
					ClearMouseTools();
					tbZoomSelection.Pushed = true;
                    Viewer.AutoZoom = AutoZoomMode.None;
                    Viewer.MouseTool = MouseToolType.ZoomArea;
					break;
			}
		}
		
		private void cboFrameIndex_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.cboFrameIndex.SelectedIndex == _currentIndex) return;
			_currentIndex = this.cboFrameIndex.SelectedIndex;

			ImageFileLoadData info = (ImageFileLoadData)_images[_currentIndex];
			try
			{
				this.Cursor = Cursors.WaitCursor;
				AtalaImage img = new AtalaImage(info.FileName, info.FrameIndex, null);
				Viewer.Images.Clear();
				Viewer.Images.Add(img);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		#endregion
		
		#region Private Methods

		// helper used to clear all of the mouse tools
		private void ClearMouseTools()
		{
			Viewer.MouseTool = MouseToolType.None;
			tbSelectRectangle.Pushed = false;
			tbSelectEllipse.Pushed = false;
			tbPan.Pushed = false;
			tbArrow.Pushed = false;
			tbMagnifier.Pushed = false;
			tbZoom.Pushed = false;
			tbZoomSelection.Pushed = false;
			tbArrow.Pushed = true;

			this.lineDraw.Active = false;
			this.rectangleDraw.Active = false;
			this.ellipseDraw.Active = false;
		}

		//used for most commands to show the Property Grid Editor and apply the command to the Workspace
		public void ShowCommand(string caption, ImageCommand command)
		{
			// Display the parameter for this command.
			ImageRegionCommand rCommand = command as ImageRegionCommand;
			if (rCommand != null)
				rCommand.RegionOfInterest = this.GetRegionOfInterest();
			Parameters frm = new Parameters(caption, command);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				Viewer.ApplyCommand(command, caption);
				UpdateUndoRedoInfo();
			}
		}

		//used for transform commands to add the command to the transform chain, then show the editor
		public void ShowTransformCommand(string caption, Transform command)
		{
			
			// Handle Transform chains.
			if (this.transformChain == null)
				transformChain = new TransformChainCommand();
				
			if (!this.chainTransforms)
			{
				ShowCommand(caption, command);
			} 
			else 
			{
				Parameters frm = new Parameters(caption, command);
				if (frm.ShowDialog() == DialogResult.OK) 
				{
					transformChain.Add(command);
				}
				
			}
		}

		//returns the region of interest occupied by the selection
		private RegionOfInterest GetRegionOfInterest()
		{
			if (Viewer.Selection.Visible) 
			{
				return new RegionOfInterest(Viewer.Selection.GetRegion());
			} 
			else 
			{
				return null;
			}
		}
		
		// Updates the Undo and Redo menu and toolbar items.
		private void UpdateUndoRedoInfo()
		{
			this.Cursor = Cursors.WaitCursor;

			// Update the undo menu and toolbar item.
			MenuItem item = this.menuEditUndo;
			if (item != null) 
			{
				item.Enabled = (bool)(Viewer.Undos.NumUndos > 0);
				item.Text = (item.Enabled ? "&Undo " + Viewer.Undos[Viewer.Undos.Count - Viewer.Undos.NumUndos].Description : "&Undo");
				tbUndo.Enabled = item.Enabled;
			}
			
			// Update the redo menu and toolbar item.
			item = this.menuEditRedo;
			if (item != null) 
			{
				item.Enabled = (bool)(Viewer.Undos.NumRedos > 0);
				item.Text = (item.Enabled ? "&Redo " + Viewer.Undos[Viewer.Undos.Count - Viewer.Undos.NumUndos - 1].Description : "&Redo");
				tbRedo.Enabled = item.Enabled;
			}
			
			// Update the Frame Index combobox.
			if (this.cboFrameIndex.Items.Count != _images.Count) 
			{
				this.cboFrameIndex.Items.Clear();
				
				int c = _images.Count;
				for (int i = 0; i < c; i++)  
				{
					this.cboFrameIndex.Items.Add("Frame Number " + i.ToString());
					if (i == _currentIndex) this.cboFrameIndex.SelectedIndex = i;
				}
			}
			else if (_images.Count > 0)
				this.cboFrameIndex.SelectedIndex = _currentIndex;
			
			// Update the statusbar.
			if (Viewer.Image != null)
				statusBarMessage.Text = Viewer.Image.ToString();
			else
				statusBarMessage.Text = "No Image";

			this.Cursor = Cursors.Default;
		}
		
		// This is called during the user transform process.
		private bool UserTransformPixel(UserTransformData data)
		{
			// This will move the image to the right."
			data.FromPixel = new PointF((float)(data.CurrentPixel.X - 10.5), (float)data.CurrentPixel.Y);
			
			// Return true to continue.
			return true;
		}
		
		private void OpenAndLoadImage()
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);

			if (_firstLoad)
			{
				// try to locate images folder
				string imagesFolder = Application.ExecutablePath;
				// we assume we are running under the DotImage install folder
				int pos = imagesFolder.IndexOf("DotImage ");
				if (pos != -1)
				{
					imagesFolder = imagesFolder.Substring(0,imagesFolder.IndexOf(@"\",pos)) + @"\Images";
				}

				//use this folder as starting point			
				openFile.InitialDirectory = imagesFolder;
			}

			if (openFile.ShowDialog() == DialogResult.OK)
			{
				try 
				{
					this.currentFile = openFile.FileName;
					this.performingOpen = true;
					
					_startTick = System.Environment.TickCount;
					FileStream fs = new FileStream(this.currentFile, FileMode.Open, FileAccess.Read, FileShare.Read);
					ImageDecoder dec = RegisteredDecoders.GetDecoder(fs);
					fs.Seek(0, SeekOrigin.Begin);
					
					Parameters frm = new Parameters("Open Image", dec);

					if (frm.ShowDialog() == DialogResult.OK) 
					{
						this.Cursor = Cursors.WaitCursor;

						// This demo only keeps one image loaded at a time.
						_images.Clear();
						_currentIndex = 0;
						Viewer.Images.Clear();

						int frameCount = dec.GetImageInfo(fs).FrameCount;
						fs.Seek(0, SeekOrigin.Begin);

						for (int i = 0; i < frameCount; i++)
						{
							_images.Add(new ImageFileLoadData(openFile.FileName, i));
						}

						Viewer.Open(fs, 0);
					}
					// If it's not asynchronous, get the metadata here.
					if (!this.Viewer.Asynchronous)
					{
						DisplayLoadTime();
						this.performingOpen = false;
						ReadMetadata();
					}
				} 
				catch (Exception ex) 
				{
					// Show the exception.
					MessageBox.Show(this, ex.Message);
					this.performingOpen = false;
				}
				finally
				{
					this.Cursor = Cursors.Default;
				}

				if (openFile != null)
					openFile.Dispose();
			}
		}

		public void DisplayLoadTime()
		{
			double time = ((double)System.Environment.TickCount - _startTick) / 1000;
			FileStream fs = File.OpenRead(this.currentFile);
			ImageDecoder dec = RegisteredDecoders.GetDecoder(fs);
			fs.Close();
			this.statusBarLoadTime.Text = time.ToString("0.##") + " sec with " + dec.GetType().Name;
		}

		//used prior to saving an image
		private void PrepareMetadataToSave(ImageEncoder encoder)
		{
			if (encoder is JpegEncoder)
			{
				JpegEncoder jpg = (JpegEncoder)encoder;
				jpg.IptcTags = this.iptcItems;
				jpg.ComText = this.comItems;
				jpg.AppMarkers = this.jpegMarkers;
			}
			else if (encoder is PngEncoder)
			{
				PngEncoder png = (PngEncoder)encoder;
				png.ComText = this.comItems;
			}
			else if (encoder is TiffEncoder)
			{
				TiffEncoder tif = (TiffEncoder)encoder;
				tif.IptcTags = this.iptcItems;
			}
			
		}
		
		private void SaveCurrentImage(UrlParameter url)
		{
			try 
			{
				//show the save dialog
				SaveFileDialog saveFile = new SaveFileDialog();
				saveFile.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(false);

				if (this.currentFile.Length > 3)
					saveFile.DefaultExt = this.currentFile.Substring(this.currentFile.Length - 3);
				
				ImageEncoder encoder = null;
				if (url == null)
				{
					if (saveFile.ShowDialog() == DialogResult.OK) 
					{
						this.currentFile = saveFile.FileName;
						encoder = AtalaDemos.HelperMethods.GetImageEncoder(saveFile.FilterIndex);
					}
					else
					{
						saveFile.Dispose();
						return;
					}
				}
				else
				{
					switch (url.FileFormat)
					{
						case ImageFileFormats.Bmp:
							encoder = new BmpEncoder();
							break;
						case ImageFileFormats.Gif:
							encoder = new GifEncoder();
							break;
						case ImageFileFormats.Jpeg:
							encoder = new JpegEncoder();
							break;
						case ImageFileFormats.Png:
							encoder = new PngEncoder();
							break;
					}
				}
				
				// Custom saving code for animated GIF, TIFF and PDF
				if (encoder == null && saveFile.FileName.EndsWith(".gif"))
				{
					SaveAnimatedGif();
				}
				else if (encoder is PdfEncoder)
				{
					encoder = null;
					SavePdf();
				}

				saveFile.Dispose();

				if (encoder == null) 
					return;

				// Save the metadata with the image
				this.PrepareMetadataToSave(encoder);
				
				//show encoder properties and save
				Form saveFrm = new Parameters("Encoder Settings", encoder);
				if (saveFrm.ShowDialog() == DialogResult.OK)
				{
					// Save it.
					string file = (url == null ? this.currentFile : url.ToString());
					if (encoder is TiffEncoder && _images.Count > 1)
						SaveMultiPageTiff(file, (TiffEncoder)encoder);
					else
						Viewer.Save(file, encoder);
				}
				saveFrm.Dispose();
			} 
			catch (Exception ex) 
			{
				MessageBox.Show(this, ex.Message);
			}
		}

		private void EnableMenuItems()
		{
			this.tbSave.Enabled = true;
			this.menuFileSaveAs.Enabled = true;
			this.menuFileSaveFTP.Enabled = true;
			this.menuEditCut.Enabled = true;
			this.menuEditCopy.Enabled = true;
			this.menuEditPaste.Enabled = true;
			this.menuImage.Enabled = true;
			this.menuDraw.Enabled = true;
			this.menuCommands.Enabled = true;
			this.menuFilePageSetup.Enabled = true;
			this.menuFilePrintImage.Enabled = true;
			
			this.menuImageExif.Enabled = (this.exifItems != null && this.exifItems.Count > 0);
			this.menuImageIptc.Enabled = (this.iptcItems != null && this.iptcItems.Count > 0);
		}

		private void SaveMultiPageTiff(string file, TiffEncoder encoder)
		{
			// This method is required because we only keep one image in memory.
			using (FileStream fs = new FileStream(file, (encoder.Append && File.Exists(file) ? FileMode.Open : FileMode.Create), FileAccess.ReadWrite))
			{
				for (int i = 0; i < _images.Count; i++)
				{
					ImageFileLoadData data = (ImageFileLoadData)_images[i];
					AtalaImage image = (i == _currentIndex ? Viewer.Image : new AtalaImage(data.FileName, data.FrameIndex, null));
					if (i > 0) encoder.Append = true;
					fs.Seek(0, SeekOrigin.Begin);
					encoder.Save(fs, image, null);
					if (i != _currentIndex) image.Dispose();
				}
			}
		}
		
		private void SaveAnimatedGif()
		{
			GifFrameCollection col = new GifFrameCollection();
			GifFrame frame = null;
			MultiSave frm = new MultiSave();
			GifEncoder gif = new GifEncoder();
			
			// Make the encoder the first item.
			frm.listImages.Items.Add("GifEncoder");
			frm.images.Add(gif);
			
			// Add each image as a frame.
			for (int i = 0; i < _images.Count; i++)
			{
				ImageFileLoadData data = (ImageFileLoadData)_images[i];
				AtalaImage image = (i == _currentIndex ? Viewer.Image : new AtalaImage(data.FileName, data.FrameIndex, null));
				frm.listImages.Items.Add(image.ToString());
				frame = new GifFrame(image);
				col.Add(frame);
				frm.images.Add(frame);
			}
			
			// Select the first image.
			frm.listImages.SelectedIndex = 0;
			
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
			
				this.Cursor = Cursors.WaitCursor;
				
				FileStream fs = new FileStream(this.currentFile, FileMode.Create, FileAccess.Write);
				gif.Save(fs, col, null);
				fs.Close();
				
				this.Cursor = Cursors.Default;
			}

			// Dispose the temporary images.
			for (int i = 1; i <= _images.Count; i++)
			{
				if (i - 1 != _currentIndex)
					((GifFrame)frm.images[i]).Image.Dispose();
			}

			frm.Dispose();

		}
		
		private void SavePdf()
		{
			PdfImageCollection col = new PdfImageCollection();
			PdfImage page = null;
			MultiSave frm = new MultiSave();
			PdfEncoder pdf = new PdfEncoder();
			
			// Make the encoder the first item.
			frm.listImages.Items.Add("PdfEncoder");
			frm.images.Add(pdf);
			
			// Add each image.
			for (int i = 0; i < _images.Count; i++)
			{
				ImageFileLoadData data = (ImageFileLoadData)_images[i];
				AtalaImage image = (i == _currentIndex ? Viewer.Image : new AtalaImage(data.FileName, data.FrameIndex, null));
				frm.listImages.Items.Add(image.ToString());
				page = new PdfImage(image, PdfCompressionType.Auto);
				col.Add(page);
				frm.images.Add(page);
			}
			
			// Select the first image.
			frm.listImages.SelectedIndex = 0;
			
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				this.Cursor = Cursors.WaitCursor;
				
				FileStream fs = new FileStream(this.currentFile, FileMode.Create, FileAccess.Write);
				pdf.Save(fs, col, null);
				fs.Close();
				
				this.Cursor = Cursors.Default;
			}

			// Dispose the temporary images.
			for (int i = 1; i <= _images.Count; i++)
			{
				if (i - 1 != _currentIndex)
					((PdfImage)frm.images[i]).Image.Dispose();
			}

			frm.Dispose();
		}

		private void ReadMetadata()
		{
			// A Photo Pro or Document license is required for metadata.
			if (AtalaImage.Edition == LicenseEdition.Photo || AtalaImage.Edition == LicenseEdition.Standard)
				return;

			if (this.currentFile.Length == 0 || this.currentFile.StartsWith("http")) return;

			FileStream fs = new FileStream(this.currentFile, FileMode.Open, FileAccess.Read);
			ExifParser exif = new ExifParser();
			this.exifItems = exif.ParseFromImage(fs, 0);
			this.menuImageExif.Enabled = (this.exifItems != null && this.exifItems.Count > 0);

			fs.Seek(0, SeekOrigin.Begin);
			IptcParser iptc = new IptcParser();
			this.iptcItems = iptc.ParseFromImage(fs, 0);
			this.menuImageIptc.Enabled = (this.iptcItems != null && this.iptcItems.Count > 0);

			fs.Seek(0, SeekOrigin.Begin);
			this.jpegMarkers = new JpegMarkerCollection(fs);

			fs.Seek(0, SeekOrigin.Begin);
			ComTextParser com = new ComTextParser();
			this.comItems = com.ParseFromImage(fs);

			fs.Close();
		}

		private void LoadDirect(AtalaImage image)
		{
			Viewer.Images.Clear();
			Viewer.Images.Add(image);

			_images.Clear();
			_currentIndex = 0;

			string tmp = Path.GetTempFileName();
			image.Save(tmp, new PngEncoder(), null);
			_tempFiles.Add(tmp);
			_images.Add(new ImageFileLoadData(tmp, 0));

			UpdateUndoRedoInfo();
			EnableMenuItems();
		}
		
		#endregion

		#region Rubberband Drawing Event Handlers
		private void lineDraw_Changed(object sender, Atalasoft.Imaging.WinControls.RubberbandEventArgs e)
		{
			Canvas myCanvas = new Canvas(this.Viewer.Image);
			myCanvas.SmoothingLevel = this.canvasSmoothing;
			myCanvas.DrawLine(new Point((int)e.StartPoint.X, (int)e.StartPoint.Y), new Point((int)e.EndPoint.X, (int)e.EndPoint.Y), lineDraw.Pen);
			Viewer.Refresh();

			if (this.drawMode == DrawMenuMode.Lines)
				lineDraw.Start(e.EndPoint, true);
			else if (this.drawMode == DrawMenuMode.Polygon)
			{
				if (this.polygonPoints == null)
				{
					this.polygonPoints = new Point[2];
					this.polygonPoints[0] = new Point((int)e.StartPoint.X, (int)e.StartPoint.Y);
					this.polygonPoints[1] = new Point((int)e.EndPoint.X, (int)e.EndPoint.Y);
				}
				else
				{
					Point[] tmp = this.polygonPoints;
					this.polygonPoints = new Point[tmp.Length + 1];
					tmp.CopyTo(this.polygonPoints, 0);
					this.polygonPoints[tmp.Length] = new Point((int)e.EndPoint.X, (int)e.EndPoint.Y);
				}

				lineDraw.Start(e.EndPoint, true);
			}
			
			UpdateUndoRedoInfo();

		}
		private void rectangleDraw_Changed(object sender, Atalasoft.Imaging.WinControls.RubberbandEventArgs e)
		{
			Canvas myCanvas = new Canvas(this.Viewer.Image);
			myCanvas.SmoothingLevel = this.canvasSmoothing;
			myCanvas.DrawRectangle(e.GetBounds(), this.rectangleDraw.Pen, this.rectangleDraw.Fill, this.rectangleDraw.CornerRadius);
			Viewer.Refresh();
			UpdateUndoRedoInfo();
		}

		private void ellipseDraw_Changed(object sender, Atalasoft.Imaging.WinControls.RubberbandEventArgs e)
		{
			Canvas myCanvas = new Canvas(this.Viewer.Image);
			myCanvas.SmoothingLevel = this.canvasSmoothing;
			myCanvas.DrawEllipse(e.GetBounds(), this.ellipseDraw.Pen, this.ellipseDraw.Fill);
			Viewer.Refresh();
			this.ellipseDraw.Visible = false;
			UpdateUndoRedoInfo();
		}
		#endregion

		#region File Menu
		//create a new image
		private void menuFileNew_Click(object sender, System.EventArgs e)
		{
			// Start a new image.
			NewImageParameter newImage = new NewImageParameter();
			Form frm = new Parameters("New Image", newImage);
					
			if (frm.ShowDialog(this) == DialogResult.OK) 
			{
				// Check for a valid entry.
				if (newImage.Width < 2 || newImage.Height < 2) 
				{
					MessageBox.Show("You have provided an invalid width or height.");
					frm.Dispose();
					return;
				}
						
				// Pass the new image.
				AtalaImage image = new AtalaImage(newImage.Width, newImage.Height, newImage.ImageFormat, newImage.BackColor);
				Viewer.Undos.Add("New Image", false);
				LoadDirect(image);
			}
		}

		//open an image from a file
		private void menuFileOpen_Click(object sender, System.EventArgs e)
		{
			OpenAndLoadImage();
		}

		//open an image from a URL
		private void menuFileOpenFromURL_Click(object sender, System.EventArgs e)
		{
			UrlParameter url = new UrlParameter();
			Form frm = new Parameters("Open from URL", url);
			if (frm.ShowDialog(this) == DialogResult.OK) 
				Viewer.Open(url.Url);
		}

		private void menuFileDecoders_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			ImageDecoder decoder = null;
			switch (item.Text)
			{
				case "JPEG":
					decoder = RegisteredDecoders.GetDecoderFromType(typeof(JpegDecoder));
					break;
				case "TLA":
					decoder = RegisteredDecoders.GetDecoderFromType(typeof(TlaDecoder));
					break;
#if PDFRasterizer
				case "PDF":
					decoder = RegisteredDecoders.GetDecoderFromType(typeof(PdfDecoder));
					break;
#endif
				case "PNG":
					decoder = RegisteredDecoders.GetDecoderFromType(typeof(PngDecoder));
					break;
				case "WMF":
					decoder = RegisteredDecoders.GetDecoderFromType(typeof(WmfDecoder));
					break;
			}

			//show properties.  Changed Settings will be saved to the KnownDecoders class
			Parameters frm = new Parameters(decoder.ToString() + " Settings", decoder);
			frm.ShowDialog(this);

		}

		//generate a new image containing noise
		private void menuFileNoise_Click(object sender, System.EventArgs e)
		{
			NoiseGenerator noise = new NoiseGenerator(new Size(400, 300), 0.5);
			Parameters noiseForm = new Parameters("Noise Generator", noise);
			AtalaImage image = null;
			if (noiseForm.ShowDialog() == DialogResult.OK) 
			{
				switch(noise.Mode) 
				{
					case NoiseGeneratorMode.Hugo:
						image = noise.GenerateImage(2, 2.5);
						break;
					case NoiseGeneratorMode.DimensionalSlice:
						image = noise.GenerateImage(10);
						break;
					default:
						image = noise.GenerateImage();
						break;
				}
			}
			noiseForm.Dispose();
			LoadDirect(image);
		}

		private void menuFileSaveAs_Click(object sender, System.EventArgs e)
		{
			this.SaveCurrentImage(null);
		}

		private void menuFileSaveFTP_Click(object sender, System.EventArgs e)
		{
			UrlParameter ftp = new UrlParameter();
			Form frm = new Parameters("Save to FTP", ftp);
			if (frm.ShowDialog(this) == DialogResult.OK) 
			{
				this.SaveCurrentImage(ftp);
			}
			frm.Dispose();
			
		}

		//shows the page setup dialog to set the page settings prior to printing
		private void menuFilePageSetup_Click(object sender, System.EventArgs e)
		{
			this.pageSetupDialog1.Document = this.imagePrintDocument1;
			this.pageSetupDialog1.ShowDialog(this);
		}

		//show the print dialog, and uses the ImagePrintDocument component to print images
		private void menuFilePrintImage_Click(object sender, System.EventArgs e)
		{
			this.printDialog1.Document = this.imagePrintDocument1;
			if (this.printDialog1.ShowDialog(this) == DialogResult.OK)
			{
				this.Viewer.Image.Resolution = new Dpi(96, 96, Atalasoft.Imaging.ResolutionUnit.DotsPerInch);
				this.imagePrintDocument1.Image = this.Viewer.Image;
				Parameters printParams = new Parameters("Print Options", this.imagePrintDocument1);
				if (printParams.ShowDialog() == DialogResult.OK)
					this.imagePrintDocument1.Print();
				printParams.Dispose();
			}
		}

		//exit the application
		private void menuFileExit_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
        #endregion

        #region View Menu
        private void menuViewWidth_Click(object sender, EventArgs e)
        {
            ClearMouseTools();
            Viewer.AutoZoom = AutoZoomMode.FitToWidth;
        }

        private void menuViewBest_Click(object sender, EventArgs e)
        {
            ClearMouseTools();
            Viewer.AutoZoom = AutoZoomMode.BestFitShrinkOnly;
        }

        private void menuViewHeight_Click(object sender, EventArgs e)
        {
            ClearMouseTools();
            Viewer.AutoZoom = AutoZoomMode.FitToHeight;
        }

        private void menuView100_Click(object sender, EventArgs e)
        {
            Viewer.AutoZoom = AutoZoomMode.None;
            Viewer.Zoom = 1.0;
        }

        private void menuView50_Click(object sender, EventArgs e)
        {
            Viewer.AutoZoom = AutoZoomMode.None;
            Viewer.Zoom = 0.5;
        }

        private void menuView150_Click(object sender, EventArgs e)
        {
            Viewer.AutoZoom = AutoZoomMode.None;
            Viewer.Zoom = 1.5;
        }
        #endregion View Menu

        #region Edit Menu
        //undo the last image change
        private void menuEditUndo_Click(object sender, System.EventArgs e)
		{
			Viewer.Undos.Undo();
			UpdateUndoRedoInfo();
		}

		//redo the previous undo
		private void menuEditRedo_Click(object sender, System.EventArgs e)
		{
			Viewer.Undos.Redo();
			UpdateUndoRedoInfo();
		}

		private void CopyImageToClipboard(bool cutArea)
		{
			if (_images.Count > 0)
			{
				// Copy to the clipboard and erase that area.
				if (Viewer.Selection != null && Viewer.Selection.Visible && Viewer.Selection.Bounds.Width > 0 && Viewer.Selection.Bounds.Height > 0) 
				{
					// Crop to the selection.
					CropCommand copy = new CropCommand(Viewer.Selection.Bounds);
					AtalaImage copyImage = copy.Apply(Viewer.Image).Image;
					copyImage.CopyToClipboard(this.Handle);
						
					// A copy of the image is sent to the clipboard,
					// so go ahead and distroy this image.
					copyImage.Dispose();
					
					if (cutArea)
					{
						// Erase it by simply drawing a solid region.
						Canvas myCanvas = new Canvas(Viewer.Image);
						myCanvas.DrawRegion(Viewer.Selection.GetRegion(), new SolidFill(Color.White));
						Viewer.Refresh();
					}
				} 
				else 
				{
					Viewer.Image.CopyToClipboard(this.Handle);
				}
			}
		}

		//cut the selected porting of the image
		private void menuEditCut_Click(object sender, System.EventArgs e)
		{
			CopyImageToClipboard(true);
		}

		private void menuEditCopy_Click(object sender, System.EventArgs e)
		{
			CopyImageToClipboard(false);
		}

		private void menuEditPaste_Click(object sender, System.EventArgs e)
		{
			AtalaImage pasteImage = AtalaImage.ImageFromClipboard(this.Handle);
			if (pasteImage == null) 
				return;
					
			if (_images.Count == 0) 
			{
				Viewer.Images.Add(pasteImage);
				Viewer.Update();
				return;
			}
					
			// You cannot overlay onto a 4-bit image.
			if (Viewer.Image.PixelFormat == PixelFormat.Pixel4bppIndexed) 
			{
				ChangePixelFormatCommand cp = new ChangePixelFormatCommand(PixelFormat.Pixel8bppIndexed);
				Viewer.ApplyCommand(cp);
			}
					
			// You also cannot overlay an 16-bit image onto an 8-bit indexed.
					
			// Ask where the image should be placed.
			PointParameter pt = new PointParameter();
			if (Viewer.Selection != null && Viewer.Selection.Visible)
				pt.Position = Viewer.Selection.Bounds.Location;

			Form frm = new Parameters("Paste Image", pt);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				// Turn off asynchrouous so the paste image can be disposed.
				bool async = Viewer.Asynchronous;
				Viewer.Asynchronous = false;
				OverlayCommand ovr = new OverlayCommand(pasteImage, pt.Position, (double)1);
				Viewer.ApplyCommand(ovr, "Paste");
				Viewer.Asynchronous = async;
			}
			frm.Dispose();
			pasteImage.Dispose();
		}

		private void menuEditOptions_Click(object sender, System.EventArgs e)
		{
			ProgramOptions opts = new ProgramOptions();
			opts.Asynchronous = Viewer.Asynchronous;
			opts.UndoLevels = Viewer.Undos.Levels;
			opts.AntialiasDisplay = Viewer.AntialiasDisplay;
			opts.AutoZoom = Viewer.AutoZoom;
			opts.ScrollBarStyle = Viewer.ScrollBarStyle;
					
			Form frm = new Parameters("Program Options", opts);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				// Set the modified options.
				Viewer.Asynchronous = opts.Asynchronous;
				Viewer.Undos.Levels = opts.UndoLevels;
				Viewer.AntialiasDisplay = opts.AntialiasDisplay;
				Viewer.AutoZoom = opts.AutoZoom;
				Viewer.ScrollBarStyle = opts.ScrollBarStyle;
			}
			frm.Dispose();
		}
		#endregion

		#region Image Menu
		//show image information
		private void menuImageInformation_Click(object sender, System.EventArgs e)
		{
			Form frm = new Parameters("Image Information", Viewer.Image);
			frm.ShowDialog();
			frm.Dispose();
		}

		private void menuImageChangePixelFormat_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Change Pixel Format", new ChangePixelFormatCommand(Viewer.Image.PixelFormat));
		}

		private void menuImageShowHistogram_Click(object sender, System.EventArgs e)
		{
			this.histogram = new Histogram();
			try
			{
				histogram.SetHistogram(Viewer.Image);
				histogram.Show();
			}
			catch(Exception err)
			{
				MessageBox.Show(this, err.Message);
				this.histogram = null;
			}
		}

		private void menuImageExif_Click(object sender, System.EventArgs e)
		{
			// TODO: Setup a custom class so the values can be used.
			ExifTag[] eTags = new ExifTag[this.exifItems.Count];
			this.exifItems.CopyTo(eTags, 0);
			Form frm = new Parameters("EXIF Data", eTags);
			frm.ShowDialog();
			frm.Dispose();
		}

		private void menuImageIptc_Click(object sender, System.EventArgs e)
		{
			IptcTag[] iTags = new IptcTag[this.iptcItems.Count];
			this.iptcItems.CopyTo(iTags, 0);
			Form frm = new Parameters("IPTC Data", iTags);
			frm.ShowDialog();	
			frm.Dispose();
		}
				
		//set the viewer zoom
		private void menuImageZoom_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			switch (item.Index)
			{
				case 0:
					Viewer.Zoom = (double)1/32;
					break;
				case 1:
					Viewer.Zoom = (double)1/16;
					break;
				case 2:
					Viewer.Zoom = (double)1/8;
					break;
				case 3:
					Viewer.Zoom = 0.25;
					break;
				case 4:
					Viewer.Zoom = 0.50;
					break;
				case 5:
					Viewer.Zoom = 1.0;
					break;
				case 6:
					Viewer.Zoom = 2.0;
					break;
				case 7:
					Viewer.Zoom = 5.0;
					break;
				case 8:
					Viewer.Zoom = 10.0;
					break;
				case 9:
					Viewer.Zoom = (double)1/32;
					break;
			}
			
			this.Viewer.Magnifier.Zoom = Viewer.Zoom * 4;
		}
		#endregion

		#region Channels Commands
		private void menuChannelsCombine_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("This command will combine multiple 8-bit images into a single image.\nEach image represents one channel of the image.\n\nDo you want to continue?", "Combine", MessageBoxButtons.YesNo) == DialogResult.Yes) 
			{
				OpenFileDialog combineFile = new OpenFileDialog();
				combineFile.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);

				AtalaImage[] images = new AtalaImage[4];
						
				// channel 1
				combineFile.Title = "Channel 1";
				if (combineFile.ShowDialog() != DialogResult.OK) return;
				images[0] = new AtalaImage(combineFile.FileName);
						
				// channel 2
				combineFile.Title = "Channel 2";
				if (combineFile.ShowDialog() != DialogResult.OK) 
				{
					images[0].Dispose();
					return;
				}
				images[1] = new AtalaImage(combineFile.FileName);
						
				// channel 3 (optional)
				combineFile.Title = "Channel 3 (optional)";
				if (combineFile.ShowDialog() == DialogResult.OK)
					images[2] = new AtalaImage(combineFile.FileName);
						
				// channel 4 (optional)
				combineFile.Title = "Channel 4 (optional)";
				if (combineFile.ShowDialog() == DialogResult.OK)
					images[3] = new AtalaImage(combineFile.FileName);
						
				AtalaImage combine = null;
						
				if (images[3] != null) 
				{
					combine = AtalaImage.CombineChannels(PixelFormat.Pixel32bppBgra, images[0], images[1], images[2], images[3]);
				} 
				else if (images[2] != null) 
				{
					combine = AtalaImage.CombineChannels(PixelFormat.Pixel24bppBgr, images[0], images[1], images[2]);
				} 
				else 
				{
					combine = AtalaImage.CombineChannels(PixelFormat.Pixel16bppGrayscaleAlpha, images[0], images[1]);
				}
				foreach (AtalaImage image in images)
				{
					if (image != null)
						image.Dispose();
				}

				Viewer.Undos.Add("Combine Image Channels", false);
				Viewer.Image = combine;
				this.UpdateUndoRedoInfo();
			}
				
		}

		private void menuChannelsReplace_Click(object sender, System.EventArgs e)
		{
			// Make sure there are at least two channels.
			int channels = 0;
			switch (this.Viewer.Image.PixelFormat)
			{
				case PixelFormat.Pixel16bppGrayscale:
				case PixelFormat.Pixel1bppIndexed:
				case PixelFormat.Pixel4bppIndexed:
				case PixelFormat.Pixel8bppGrayscale:
				case PixelFormat.Pixel8bppIndexed:
					MessageBox.Show("Replace channels only works on images with at least two color channels.", "Invalid Pixel Format", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				case PixelFormat.Pixel16bppGrayscaleAlpha:
					channels = 2;
					break;
				case PixelFormat.Pixel24bppBgr:
				case PixelFormat.Pixel48bppBgr:
					channels = 3;
					break;
				default:
					channels = 4;
					break;
			}

			if (MessageBox.Show("This command will replace any or all channels in an image with an 8-bit image.\nTo skip a channel, simply click the 'Cancel' button in the file dialog.\n\nDo you want to continue?", "Replace", MessageBoxButtons.YesNo) == DialogResult.Yes) 
			{
				OpenFileDialog replaceFile = new OpenFileDialog();
				replaceFile.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);

				AtalaImage[] images = new AtalaImage[4];
						
				// Channel 1
				replaceFile.Title = "Channel 1";
                if (replaceFile.ShowDialog() == DialogResult.OK)
                {
                    images[0] = new AtalaImage(replaceFile.FileName);
                    if (images[0].PixelFormat != PixelFormat.Pixel8bppGrayscale)
                    {
                        MessageBox.Show(this, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
						
				// Channel 2
				replaceFile.Title = "Channel 2";
                if (replaceFile.ShowDialog() == DialogResult.OK)
                {
                    images[1] = new AtalaImage(replaceFile.FileName);
                    if (images[1].PixelFormat != PixelFormat.Pixel8bppGrayscale)
                    {
                        MessageBox.Show(this, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
						
				// Channel 3
				if (channels > 2)
				{
					replaceFile.Title = "Channel 3";
                    if (replaceFile.ShowDialog() == DialogResult.OK)
                    {
                        images[2] = new AtalaImage(replaceFile.FileName);
                        if (images[2].PixelFormat != PixelFormat.Pixel8bppGrayscale)
                        {
                            MessageBox.Show(this, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
				}
						
				// Channel 4
				if (channels > 3)
				{
					replaceFile.Title = "Channel 4";
                    if (replaceFile.ShowDialog() == DialogResult.OK)
                    {
                        images[3] = new AtalaImage(replaceFile.FileName);
                        if (images[3].PixelFormat != PixelFormat.Pixel8bppGrayscale)
                        {
                            MessageBox.Show(this, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
				}              


				ReplaceChannelCommand replaceChannel = new ReplaceChannelCommand(images[0], images[1], images[2], images[3]);
				Viewer.ApplyCommand(replaceChannel, "Replace Channels");
				UpdateUndoRedoInfo();
			}
		}

		// Split the image into 8-bit channel images.
		private void menuChannelsSplit_Click(object sender, System.EventArgs e)
		{
			if (this.Viewer.Image.ColorDepth < 16) 
				return;

			AtalaImage[] images = this.Viewer.Image.SplitChannels(ChannelFlags.AllChannels);
			if (images.Length == 0) 
				return;
					
			this.Viewer.Images.Clear();
			_images.Clear();
			this.Viewer.Images.Add(images[0]);

			for (int i = 0; i < images.Length; i++)
			{
				string tmp = Path.GetTempFileName();
				images[i].Save(tmp, new PngEncoder(), null);
				if (i > 0) images[i].Dispose();
				_tempFiles.Add(tmp);
				_images.Add(new ImageFileLoadData(tmp, 0));
			}

			UpdateUndoRedoInfo();
		}

		private void menuChannelsAdjust_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Adjust Channels", new AdjustChannelCommand(false, 0, 0, 0));
		}

		private void menuChannelsAdjustHsl_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Adjust HSL", new AdjustHslCommand(false, 0, 0, 0));
		}

		private void menuChannelsApplyLut_Click(object sender, System.EventArgs e)
		{
			// Use ApplyLut to create a negative image.
			byte[] ch1 = new byte[256];
			byte[] ch2 = new byte[256];
			byte[] ch3 = new byte[256];
			byte[] ch4 = new byte[256];
					
			for (int i = 0; i < 256; i++) 
			{
				ch1[i] = (byte)(255 - i);
				ch2[i] = (byte)(255 - i);
				ch3[i] = (byte)(255 - i);
				ch4[i] = (byte)(255 - i);
			}
					
			ShowCommand("Apply LUT (invert)", new ApplyLutCommand(ch1, ch2, ch3, ch4));
		}

		private void menuChannelsInvert_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Invert", new InvertCommand());
		}

		private void menuChannelsShift_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Shift Channels",  new ShiftChannelsCommand(20, 20, ChannelFlags.Channel1, 255));
		}

		private void menuChannelsSwap_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Swap Channels", new SwapChannelsCommand(1, 2, 3, 4));
		}

		private void menuChannelsFlattenAlpha_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Flatten Alpha", new FlattenAlphaCommand(Color.White));
		}

		private void menuChannelsAphaColor_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Set Alpha By Color", new SetAlphaColorCommand(Color.White));
		}

		private void menuChannelsAlphaMask_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog alphaFile = new OpenFileDialog();
			alphaFile.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);
			AtalaImage mask = null;
			if (alphaFile.ShowDialog() == DialogResult.OK) 
			{
				mask = new AtalaImage(alphaFile.FileName);
			}
			alphaFile.Dispose();
			if (mask == null) return;

			ShowCommand("Set Alpha From Mask", new SetAlphaFromMaskCommand(mask, true, AlphaMergeType.UseMostTransparent));
		}

		private void menuChannelsAlphaValue_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Set Alpha Value", new SetAlphaValueCommand(128, AlphaMergeType.UseMostTransparent));
		}
		#endregion

		#region Effects Commands

		
		private void menuEffectsSaturation_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Saturation", new SaturationCommand());
		}

		private void menuEffectsAdjustTint_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Adjust Tint", new AdjustTintCommand(60));
		}

		private void menuEffectsBevelEdge_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Bevel Edge", new BevelEdgeCommand(12, 150, 150, 60, 60, 100));
		}

		private void menuEffectsCrackle_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Crackle", new CrackleCommand(CrackleMode.Erosion, 20));
		}

		private void menuEffectsDeInterlace_Click(object sender, System.EventArgs e)
		{
			ShowCommand("De-Interlace", new DeInterlaceCommand());
		}

		private void menuEffectsDropShadow_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Drop Shadow", new DropShadowCommand(10));
		}

		private void menuEffectsFingerPrint_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Fingerprint", new FingerprintCommand(50, 5, true));
		}

		private void menuEffectsFloodFill_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Flood Fill", new FloodFillCommand(new Point(0,0), Color.Black, 0, ColorMatchMode.Surface));
		}

		private void menuEffectsGamma_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Gamma", new GammaCommand(1));
		}

		private void menuEffectsGauzy_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Gauzy", new GauzyCommand(50, 20, 60, GauzyMode.Max));
		}

		private void menuEffectsHalftone_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Halftone", new HalftoneCommand(2, false));
		}

		private void menuEffectsMosaic_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Mosaic", new MosaicCommand(20));
		}

		private void menuEffectsOilPaint_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Oil Paint", new OilPaintCommand());
		}

		private void menuEffectsPosterize_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Posterize", new PosterizeCommand());
		}

		private void menuReduceColors_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Reduce Colors", new ReduceColorsCommand(256));
		}

		private void menuEffectsReplaceColor_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Replace Color", new ReplaceColorCommand(Color.White, Color.Gray, 0));
		}
		
		private void menuEffectsRoundedBevel_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Rounded Bevel", new RoundedBevelCommand());
		}

		private void menuEffectsSolarize_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Solarize", new SolarizeCommand());
		}

		private void menuEffectsStipple_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Stipple", new StippleCommand(40, StippleFilterType.GeometricMean, StippleMode.FilterFirst));
		}

		private void menuEffectsTintGrayscale_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Tint Grayscale", new TintGrayscaleCommand(Color.Azure));
		}

		private void menuEffectsWatercolorTint_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Watercolor Tint", new WatercolorTintCommand());
		}

		private void menuEffectsBrightnessHistogramEqualize_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Brightness Histogram Equalize", new BrightnessHistogramEqualizeCommand(64, 200));
		}

		private void menuEffectsBrightnessHistogramStretch_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Brightness Histogram Stretch", new BrightnessHistogramStretchCommand(10, 10));
		}

		private void menuHistogramEqualize_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Histogram Equalize", new HistogramEqualizeCommand(10, 200));
		}

		private void menuEffectsHistogramStretch_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Histogram Stretch", new HistogramStretchCommand(10, 10));
		}

		private void menuEffectsRedEyeRemoval_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Red Eye Removal", new RedEyeRemovalCommand());		
		}

		private void menuEffectsLevels_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Levels", new LevelsCommand());
		}

		private void menuEffectsAutoLevels_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Auto Levels", new AutoLevelsCommand());
		}

		private void menuEffectsAutoContrast_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Auto Contrast", new AutoContrastCommand());
		}

		private void menuEffectsAutoColor_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Auto Color", new AutoColorCommand());
		}

		private void menuEffectsAutoWhiteBalance_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Auto White Balance", new AutoWhiteBalanceCommand());
		}
		#endregion

		#region Filters Commands
		private void menuFiltersBrightnessContrast_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Brightness / Contrast", new BrightnessContrastCommand(0, 0));
		}

		private void menuFiltersSaturation_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Saturation", new SaturationCommand(1));
		}

		private void menuFiltersBlur_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Blur", new BlurCommand(60, 3));
		}

		private void menuFiltersGaussianBlur_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Gaussian Blur", new BlurGaussianCommand(2.4));
		}

		private void menuAdaptiveUnsharpMask_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Adaptive Unsharp Mask", new AdaptiveUnsharpMaskCommand(0, 2.5, AdaptiveUnsharpQuality.Middle));
		}

		private void menuEffectsUnsharpMask_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Unsharp Mask", new UnsharpMaskCommand(0, 1, 2.4));
		}

		private void menuFiltersSharpen_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Sharpen", new SharpenCommand(80, 3));
		}

		private void menuFiltersAddNoise_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Add Noise", new AddNoiseCommand(AddNoiseFilterType.Negative, 600, false));
		}

		private void menuFiltersDespeckle_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Despeckle", new DespeckleCommand());
		}

		private void menuFiltersEmboss_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Emboss", new EmbossCommand(135, 10, 135, false));
		}

		private void menuFiltersIntensify_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Intensify", new IntensifyCommand(50));
		}

		private void menuFiltersHighPass_Click(object sender, System.EventArgs e)
		{
			ShowCommand("High Pass Filters", new HighPassCommand(4, 50));
		}

		private void menuFiltersMaximum_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Maximum Filter", new MaximumCommand(3));
		}

		private void menuFiltersMean_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Mean Filters", new MeanCommand(MeanFilterType.Arithmetic, 3, 2));
		}

		private void menuFiltersMedian_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Median Filter", new MedianCommand());
		}

		private void menuFiltersMidpoint_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Midpoint Filter", new MidpointCommand(3));
		}

		private void menuFiltersMinimum_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Minimum Filter", new MinimumCommand(3));
		}

		private void menuFiltersMorphological_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			MorphoGrayCommand command = new MorphoGrayCommand((MorphoGrayMode)item.Index);
			command.RegionOfInterest = GetRegionOfInterest();
			Viewer.ApplyCommand(command, "Morphological " + item.Text);
			UpdateUndoRedoInfo();
		}

		private void menuFiltersThreshold_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Threshold Filter", new ThresholdCommand(10, 200));
		}

		private void menuFiltersConvolutionFilter_Click(object sender, System.EventArgs e)
		{
			DoubleMatrix matrix = new DoubleMatrix(3, 3);
			matrix.SetRow(0, 1, 1, 1);
			matrix.SetRow(1, 1, -8, 1);
			matrix.SetRow(2, 1, 1, 1);

			ShowCommand("Convolution Filter", new ConvolutionFilterCommand(matrix, 0.8));
		}

		private void menuFiltersConvolutionMatrix_Click(object sender, System.EventArgs e)
		{
			DoubleMatrix matrix = new DoubleMatrix(3, 3);
			matrix.SetRow(0, 1, 1, 1);
			matrix.SetRow(1, 1, -8, 1);
			matrix.SetRow(2, 1, 1, 1);

			ShowCommand("Convolution Matrix", new ConvolutionMatrixCommand(matrix, true, 0.8));
		}

		private void menuFiltersEdge_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Edge Detection", new EdgeDetectionCommand(EdgeDetectionType.Gradient, 1));
		}

		private void menuFiltersCannyEdgeDetector_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Canny Edge Detector", new CannyEdgeDetectorCommand(2.4, 30, 70));
		}

		private void menuFiltersDustScratchRemoval_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Dust & Scratch Removal", new DustAndScratchRemovalCommand(9, 50, 100));
		}

		#endregion

		#region Transforms Commands
		
		private void menuTransformsApply_Click(object sender, System.EventArgs e)
		{
			if (transformChain.Count == 0)
				MessageBox.Show("You need to add at least one transform to the chain");
			else
				ShowCommand("Transform Chain", this.transformChain);
		}

		private void menuTransformsBumpMap_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog bumpFile = new OpenFileDialog();
			bumpFile.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);
			AtalaImage image = null;
			if (bumpFile.ShowDialog() == DialogResult.OK) 
			{
				image = new AtalaImage(bumpFile.FileName);
			}
			bumpFile.Dispose();
			if (image == null) 
				return;
					
			ShowTransformCommand("Bump Map Transform", new BumpMapTransform(1.2, image));
		}

		private void menuTransformsChain_Click(object sender, System.EventArgs e)
		{
			this.menuTransformsChain.Checked = !menuTransformsChain.Checked;
			this.chainTransforms = menuTransformsChain.Checked;
			this.menuTransformsApply.Enabled = this.chainTransforms;
		}

		private void menuTransformsElliptical_Click(object sender, System.EventArgs e)
		{
			int elsize;
			if (Viewer.Selection.Visible)
			{
				RectangleF region = this.GetRegionOfInterest().Region.GetBounds(Viewer.Image.GetGraphics());
				elsize = (int)(region.Width > region.Height ? region.Height: region.Width);
			}
			else
				elsize = (int)Math.Max(Viewer.Image.Width, Viewer.Image.Height);

			if (elsize < 1) elsize = 200;
			ShowTransformCommand("Elliptcal Transform", new EllipticalTransform(new Size(elsize, elsize),Point.Empty, CompressTransformMode.Both));
		}

		private void menuTransformsLens_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Lens Transform", new LensTransform(400, Point.Empty));
		}

		private void menuLineSlice_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Line Slice Transform", new LineSliceTransform(10, 300, true));
		}

		private void menuTransformsMarble_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Marble Transform", new MarbleTransform(0.8, new Size(4, 5))); 
		}

		private void menuTransformsOffset_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Offset Transform", new OffsetTransform(new Point(60, 20), OffsetTransformMode.WrapBothEdges));
		}

		private void menuTransformsPerlin_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Perlin Transform", new PerlinTransform(0.8, new Size(4, 5)));
		}

		private void menuTransformsPinch_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Pinch Transform", new PinchTransform(400, 30, Point.Empty));
		}

		private void menuTransformsPolygon_Click(object sender, System.EventArgs e)
		{
			Point[] pts = new Point[4] { new Point(30, 20), new Point(100, 56), new Point(200, 140), new Point(135, 168) };
			ShowTransformCommand("Polygon Transform", new PolygonTransform(pts, CompressTransformMode.Both)); 
		}

		private void menuTransformsRandom_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Random Transform", new RandomTransform(20));
		}

		private void menuTransformsRipple_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Ripple Transform", new RippleTransform(200, 3.5, 15, Point.Empty, RippleTransformMode.Cosine));
		}

		private void menuTransformsSpin_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Spin Transform", new SpinTransform(300,200, Point.Empty, Color.White));
		}

		private void menuTransformSpinWave_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Spin Wave Transform", new SpinWaveTransform(300, 200, 20, new Point(40, 60)));
		}

		private void menuTransformsWave_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Wave Transform", new WaveTransform(30, 10, WaveTransformMode.LeftToRightSine));
		}

		private void menuTransformsWow_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Wow Transform",  new WowTransform(200, true));
		}

		private void menuTransformsZigZag_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("Zig Zag Transform",  new ZigZagTransform(30, 10, false));
		}

		private void menuTransformsUser_Click(object sender, System.EventArgs e)
		{
			ShowTransformCommand("User Transform", new UserTransform(new UserTransformCallback(UserTransformPixel)));
		}
		#endregion

        #region ADC Commands

        private void menuItemAutomaticDocumentNegation_Click(object sender, EventArgs e)
        {
            ShowCommand("AutoNegateCommand", new AutoNegateCommand());
        }

        private void menuItemBinarySegmentation_Click(object sender, EventArgs e)
        {
            ShowCommand("BinarizeCommand", new BinarizeCommand());
        }

        private void menuItemBlackBorderCrop_Click(object sender, EventArgs e)
        {
            ShowCommand("AutoBorderCropCommand", new AutoBorderCropCommand());
        }

        private void menuItemBlankPageDetection_Click(object sender, EventArgs e)
        {
            ShowCommand("BlankPageDetectionCommand", new BlankPageDetectionCommand());
        }

        private void menuItemBlobRemoval_Click(object sender, EventArgs e)
        {
            ShowCommand("BlobRemovalCommand", new BlobRemovalCommand());
        }

        private void menuItemBorderRemoval_Click(object sender, EventArgs e)
        {
            ShowCommand("AdvancedBorderRemovalCommand", new AdvancedBorderRemovalCommand());
        }

        private void menuItemDeskew_Click(object sender, EventArgs e)
        {
            ShowCommand("AutoDeskewCommand", new AutoDeskewCommand());
        }

        private void menuItemDespeckle_Click(object sender, EventArgs e)
        {
            ShowCommand("DocumentDespeckleCommand", new DocumentDespeckleCommand());
        }

        private void menuItemHalftoneRemoval_Click(object sender, EventArgs e)
        {
            ShowCommand("HalftoneRemovalCommand", new HalftoneRemovalCommand());
        }

        private void menuItemHolePunchRemoval_Click(object sender, EventArgs e)
        {
            ShowCommand("HolePunchRemovalCommand", new HolePunchRemovalCommand());
        }

        private void menuItemInvertedTextCorrection_Click(object sender, EventArgs e)
        {
            ShowCommand("AutoInvertTextCommand", new AutoInvertTextCommand());
        }

        private void menuItemLineRemoval_Click(object sender, EventArgs e)
        {
            ShowCommand("LineRemovalCommand", new LineRemovalCommand());
        }

        private void menuItemSpeckRemoval_Click(object sender, EventArgs e)
        {
            ShowCommand("SpeckRemovalCommand", new SpeckRemovalCommand());
        }

        private void menuItemWhiteMarginCrop_Click(object sender, EventArgs e)
        {
            ShowCommand("MarginCropCommand", new MarginCropCommand());
        }

        #endregion 

		#region Document Commands
		private void menuDocumentAutoDeskew_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Auto Deskew", new AutoDeskewCommand());
		}

		private void menuDocumentMedian_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Binary Median Filter", new DocumentMedianCommand());
		}

		private void menuDocumentMorphological_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			MorphoDocumentCommand command = new MorphoDocumentCommand((MorphoDocumentMode)item.Index);
			command.RegionOfInterest = GetRegionOfInterest();
			Viewer.ApplyCommand(command, "Binary Morphological " + item.Text);
			UpdateUndoRedoInfo();
		}

		private void menuDocumentThinning_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Thinning", new DocumentThinningCommand());
		}

		private void menuDocumentHitOrMiss_Click(object sender, System.EventArgs e)
		{
			IntegerMatrix m1 = new IntegerMatrix(3, 3);
			m1.SetRow(0, 0, 0, 0);
			m1.SetRow(1, 0, 1, 1);
			m1.SetRow(2, 0, 0, 0);
			IntegerMatrix m2 = new IntegerMatrix(3, 3);
			m2.SetRow(0, 0, 0, 0);
			m2.SetRow(1, 1, 0, 0);
			m2.SetRow(2, 0, 0, 0);
			ShowCommand("Hit or Miss", new DocumentHitOrMissCommand(m1, m2));
		}

		private void menuDocumentDespeckle_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Despeckle", new DocumentDespeckleCommand());
		}

		private void menuThresholdAdaptive_Click(object sender, System.EventArgs e)
		{
			if (this.Viewer.Image != null)
			{
				AdaptiveThresholdCommand cmd = new AdaptiveThresholdCommand();
				if (this.Viewer.MouseTool == MouseToolType.Selection && this.Viewer.Selection.Visible)
					cmd.RegionOfInterest = new RegionOfInterest(this.Viewer.Selection.Bounds);

				ShowCommand("Adaptive Threshold", cmd);
			}
		}

		private void menuThresholdGlobal_Click(object sender, System.EventArgs e)
		{
			if (this.Viewer.Image != null)
			{
				GlobalThresholdCommand cmd = new GlobalThresholdCommand();
				if (this.Viewer.MouseTool == MouseToolType.Selection && this.Viewer.Selection.Visible)
					cmd.RegionOfInterest = new RegionOfInterest(this.Viewer.Selection.Bounds);

				ShowCommand("Global Threshold", cmd);
			}
		}

        private void menuThresholdDynamic_Click(object sender, EventArgs e)
        {
            if (this.Viewer.Image != null)
            {
                DynamicThresholdCommand cmd = new DynamicThresholdCommand();
                if (this.Viewer.MouseTool == MouseToolType.Selection && this.Viewer.Selection.Visible)
                    cmd.RegionOfInterest = new RegionOfInterest(this.Viewer.Selection.Bounds);

                ShowCommand("Dynamic Threshold", cmd);
            }
        }

		private void menuBorderRemoval_Click(object sender, System.EventArgs e)
		{
			if (this.Viewer.Image != null)
			{
				BorderRemovalCommand cmd = new BorderRemovalCommand(BorderRemovalEdges.AllSides, 5, false);
				ShowCommand("Border Removal", cmd);
			}
		}

        private void menuDithering_Click(object sender, EventArgs e)
        {
            if (this.Viewer.Image != null)
            {
                DitherCommand cmd = new DitherCommand();
                ShowCommand("Dither", cmd);
            }
        }
		#endregion

		#region Fft Commands

		private void menuFftBandPass_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Band Pass Filter", new BandPassFftCommand());
		}

		private void menuFftButterworthHighBoost_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Butterworth High Boost Filter", new ButterworthHighBoostFftCommand());
		}

		private void menuFftButterworthHighPass_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Butterworth High Pass Filter", new ButterworthHighPassFftCommand());
		
		}

		private void menuFftButterworthLowPass_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Butterworth Low Pass Filter", new ButterworthLowPassFftCommand());

		}

		private void menuFftGaussianHighBoost_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Gaussian High Boost Filter", new GaussianHighBoostFftCommand());
		
		}

		private void menuFftGaussianHighPass_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Gaussian High Pass Filter", new GaussianHighPassFftCommand());

		}

		private void menuFftGaussianLowPass_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Gaussian Low Pass Filter", new GaussianLowPassFftCommand());

		}

		private void menuFftIdealHighPass_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Ideal High Pass Filter", new IdealHighPassFftCommand());

		}

		private void menuFftIdealLowPass_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Ideal Low Pass Filter", new IdealLowPassFftCommand());
		
		}

		private void menuFftInversePower_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new ResizeCanvasCommand(new Size(FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(this.Viewer.Image.Height)), Point.Empty, 0));
			ShowCommand("FFT Inverse Power Filter", new InversePowerFftCommand());

		}
		#endregion

		#region Misc Commands
		private void menuFlipHorizontal_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new FlipCommand(FlipDirection.Horizontal), "Flip Horizontal");
		}

		private void menuFlipVertical_Click(object sender, System.EventArgs e)
		{
			this.Viewer.ApplyCommand(new FlipCommand(FlipDirection.Vertical), "Flip Vertical");
		}

		private void menuRotate_Click(object sender, System.EventArgs e)
		{
			// If the image has mixed X and Y resolution values, we need
			// to correct the values and scale the image.
			Dpi resolution = this.Viewer.Image.Resolution;
			if (resolution.X != resolution.Y)
			{
				float ratio = (float)resolution.X / (float)resolution.Y;
				this.Viewer.Image.Resolution = new Dpi(resolution.X, resolution.X, resolution.Units);
  
				ImageCommand cmd = null;
				if (this.Viewer.Image.PixelFormat == PixelFormat.Pixel1bppIndexed && AtalaImage.Edition == LicenseEdition.Document)
					cmd = new ResampleDocumentCommand(Rectangle.Empty, new Size(this.Viewer.Image.Width, Convert.ToInt32(this.Viewer.Image.Height * ratio)), ResampleMethod.Default);
				else
					cmd = new ResampleCommand(new Size(this.Viewer.Image.Width, Convert.ToInt32(this.Viewer.Image.Height * ratio)), ResampleMethod.Default);

				this.Viewer.ApplyCommand(cmd);
			}

			ShowCommand("Rotate", new RotateCommand(90, Color.White));
		}

		private void menuResizeCanvas_Click(object sender, System.EventArgs e)
		{
			ResizeCanvasCommand canvas = new ResizeCanvasCommand(Viewer.Image.Size, new Point(0, 0), Color.White);
			if (Viewer.Image.Palette != null)
				canvas.CanvasPaletteIndex = Viewer.Image.Palette.GetClosestPaletteIndex(Color.White);
			ShowCommand("Resize Canvas", canvas);
		}

		private void menuResample_Click(object sender, System.EventArgs e)
		{
			Rectangle srcRect = new Rectangle(new Point(0, 0), Viewer.Image.Size);
			ImageCommand command = null;
					
			if (Viewer.Image.PixelFormat == PixelFormat.Pixel1bppIndexed) 
			{
				ResampleDocumentCommand docResample = new ResampleDocumentCommand(srcRect, Viewer.Image.Size, ResampleDocumentMethod.AreaAverage);
				command = (ImageCommand)docResample;
			} 
			else 
			{
				ResampleCommand resample = new ResampleCommand(Viewer.Image.Size, ResampleMethod.BiLinear);
				command = (ImageCommand)resample;
			}
			ShowCommand("Resample", command);
		}

		private void menuCrop_Click(object sender, System.EventArgs e)
		{
			// If there is no selection, display the parameters dialog.
			CropCommand crop = null;
			if (Viewer.Selection.Visible) 
			{
				crop = new CropCommand(Viewer.Selection.Bounds);
			} 
			else 
			{
				crop = new CropCommand(new Rectangle(new Point(0, 0), Viewer.Image.Size));
				Form frm = new Parameters("Crop", crop);
				if (frm.ShowDialog() != DialogResult.OK) 
				{
					frm.Dispose();
					return;
				}
				frm.Dispose();
			}
			Viewer.ApplyCommand(crop, "Crop");
			UpdateUndoRedoInfo();
			this.Viewer.Selection.Visible = false;
		}

		private void menuAutoCrop_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Auto Crop", new AutoCropCommand());
		}

		private void menuSkew_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Skew", new SkewCommand(SkewDirection.Horizontal, 30));
		}

		private void menuQuadrilateralWarp_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Quadrilateral Warp", new QuadrilateralWarpCommand(new Point(20, 200), new Point(40, 20), 
				new Point(200, 20), new Point(160, 140), 
				InterpolationMode.BiLinear, Color.White));
		}

		private void menuPush_Click(object sender, System.EventArgs e)
		{
			ShowCommand("Push", new PushCommand(new Point(0, 0), new Point(0, 0)));
		}

		private void menuOverlay_Click(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			string caption = "";

			// We need a file before we can create an OverlayCommand object.
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);
			openFile.Title = "Select a file to overlay";
					
			AtalaImage image = null;
			if (openFile.ShowDialog() == DialogResult.OK)
				image= new AtalaImage(openFile.FileName);
					
			if (image == null) 
				return;

			ImageCommand command = null;
					
			switch(item.Index) 
			{
				case 0:
					command = new OverlayCommand(image, new Point(60, 60), 1);
					caption = "Overlay";
					break;
						
				case 1:
					// We still need the mask image.
					openFile.Title = "Select the alpha mask";
					AtalaImage image2 = null;
					if (openFile.ShowDialog() == DialogResult.OK) 
						image2 = new AtalaImage(openFile.FileName);
					command = new OverlayMaskedCommand(image, image2);
					caption = "Overlay Masked";
					break;
							
				case 2:
					command = new OverlayMergedCommand(image, new Point(60, 60), MergeOption.FastBlend, 1);
					caption = "Overlay Merged";
					break;
			}		
			openFile.Dispose();
			ShowCommand(caption, command);
		}
		#endregion

		#region Draw Menu
		private void menuDrawLine_Click(object sender, System.EventArgs e)
		{
			ClearMouseTools();
			DrawOutlineParameters linePen = new DrawOutlineParameters();
			Form frm = new Parameters("Line", linePen);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				this.drawMode = DrawMenuMode.Line;
				this.lineDraw.Parent = this.Viewer;
				this.lineDraw.Pen = linePen.GetPen();
				this.canvasSmoothing = linePen.SmoothingLevel;
				this.lineDraw.Active = true;
				Viewer.Undos.Add("Draw Line", true);
			}

			frm.Dispose();
		}

		private void menuDrawLines_Click(object sender, System.EventArgs e)
		{
			ClearMouseTools();
			DrawOutlineParameters lines = new DrawOutlineParameters();
			Form frm = new Parameters("Lines", lines);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				this.drawMode = DrawMenuMode.Lines;
				this.lineDraw.Pen = lines.GetPen();
				this.canvasSmoothing = lines.SmoothingLevel;
				this.lineDraw.Active = true;
				Viewer.Undos.Add("Draw Lines", true);
			}
			frm.Dispose();
		}

		private void menuDrawRectangle_Click(object sender, System.EventArgs e)
		{
			ClearMouseTools();
			RectangleParameters rc = new RectangleParameters();
			Form frm = new Parameters("Rectangle", rc);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				Viewer.Undos.Add("Draw Rectangle", true);
				this.canvasSmoothing = rc.SmoothingLevel;
				this.rectangleDraw.Pen = rc.GetPen();
				this.rectangleDraw.CornerRadius = rc.Rounding;
				this.rectangleDraw.Active = true;
				this.rectangleDraw.Fill = rc.GetFill();
				this.drawMode = DrawMenuMode.Rectangle;
			}
			frm.Dispose();
		}

		private void menuDrawEllipse_Click(object sender, System.EventArgs e)
		{
			ClearMouseTools();
			DrawSolidParameters ellipse = new DrawSolidParameters();
			ellipse.Fill.Color = Color.Red;
			Form frm = new Parameters("Ellipse", ellipse);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				Viewer.Undos.Add("Draw Ellipse", true);
				this.canvasSmoothing = ellipse.SmoothingLevel;
				this.ellipseDraw.Pen = ellipse.GetPen();
				this.ellipseDraw.Fill = ellipse.GetFill();
				this.ellipseDraw.Active = true;
				this.drawMode = DrawMenuMode.Ellipse;
			}
			frm.Dispose();
		}

		private void menuDrawPolygon_Click(object sender, System.EventArgs e)
		{
			ClearMouseTools();
			DrawSolidParameters poly = new DrawSolidParameters();
			Form frm = new Parameters("Polygon", poly);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				this.polygonPoints = null;
				Viewer.Undos.Add("Draw Polygon", true);
				this.canvasSmoothing = poly.SmoothingLevel;
				this.lineDraw.Pen = poly.GetPen();
				this.rectangleDraw.Fill = poly.GetFill();
				this.lineDraw.Active = true;
				this.drawMode = DrawMenuMode.Polygon;
			}
			frm.Dispose();
		}

		private void menuDrawText_Click(object sender, System.EventArgs e)
		{
			ClearMouseTools();
			Canvas myCanvas = new Canvas(Viewer.Image);
			TextParameters text = new TextParameters();
			Form frm = new Parameters("Text", text);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				Viewer.Undos.Add("Draw Text", true);
				myCanvas.SmoothingLevel = text.SmoothingLevel;
				myCanvas.FontQuality = text.FontQuality;
				TextFormat tf = text.GetTextFormat();

				if (Viewer.Selection.Visible && (Viewer.Selection.GetType() == typeof(RectangleSelection))) 
				{
					myCanvas.DrawText(text.Text, Viewer.Selection.Bounds, text.Font, new SolidFill(text.Color), new SolidFill(this.currentTextBackColor), tf);
					Viewer.Selection.Visible = false;
				} 
				else 
				{
					myCanvas.DrawText(text.Text, text.Position, text.Font, new SolidFill(text.Color), new SolidFill(this.currentTextBackColor), tf);
				}
						
				Viewer.Refresh();
			}
			frm.Dispose();
		}

		private void menuDrawSetBackcolor_Click(object sender, System.EventArgs e)
		{
			ColorDialog clr = new ColorDialog();
			clr.Color = this.currentTextBackColor;
			if (clr.ShowDialog() == DialogResult.OK)
				this.currentTextBackColor = clr.Color;
			clr.Dispose();
		}

		private void menuDrawClearBackcolor_Click(object sender, System.EventArgs e)
		{
			this.currentTextBackColor = Color.Empty;
		}

		
		private void menuDrawFreehand_Click(object sender, System.EventArgs e)
		{
			ClearMouseTools();
			DrawOutlineParameters freehand = new DrawOutlineParameters();
			Form frm = new Parameters("Freehand", freehand);
			if (frm.ShowDialog() == DialogResult.OK) 
			{
				this.drawMode = DrawMenuMode.Freehand;
				this.lineDraw.Pen = freehand.GetPen();
				this.canvasSmoothing = freehand.SmoothingLevel;
				Viewer.Undos.Add("Freehand", true);
			}
			frm.Dispose();
		}
		#endregion

		#region Help Menu

		private void menuItem15_Click(object sender, System.EventArgs e)
		{
			AtalaDemos.AboutBox.About aboutBox = new AtalaDemos.AboutBox.About("About Atalasoft DotImage WinForms Demo",
				"DotImage WinForms Demo");
			aboutBox.Description = @"The most comprehensive of all the demos demonstrating most of the image processing commands and codecs.  This is a good place to learn about the UI features that dotImage offers, as well as testing of image effects, and transforms.";
			aboutBox.ShowDialog();
		}
		#endregion

		#region Drag Drop Code

		private void Viewer_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// Allow files to be dropped onto the control.
			if (e.Data.GetDataPresent("FileDrop") || e.Data.GetDataPresent("FileNameW") || e.Data.GetDataPresent("FileName"))
				e.Effect = DragDropEffects.Link;
			else
				e.Effect = DragDropEffects.None;
		}

		private void Viewer_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			object dropped = null;

			if (e.Data.GetDataPresent("FileDrop"))
				dropped = e.Data.GetData("FileDrop");
			else if (e.Data.GetDataPresent("FileNameW"))
				dropped = e.Data.GetData("FileNameW");
			else if (e.Data.GetDataPresent("FileName"))
				dropped = e.Data.GetData("FileName");

			if (dropped == null) return;

			string[] files = dropped as string[];
			if (files == null) return;

			bool firstImage = true;

			// This could be extended to load multipage TIFFs.
			foreach (string file in files)
			{
				// Make sure it's a file we can support.
				FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
				try
				{
					ImageDecoder decoder = RegisteredDecoders.GetDecoder(fs);
					if (decoder == null) continue;

					// Wait until we have an image to load before clearing the control.
					if (firstImage)
					{
						firstImage = false;
						this.Viewer.Images.Clear();
						_images.Clear();
					}

					fs.Seek(0, SeekOrigin.Begin);
					MultiFramedImageDecoder multi = decoder as MultiFramedImageDecoder;
					if (multi != null)
					{
						int count = multi.GetFrameCount(fs);
						if (count > 1)
						{
							if (MessageBox.Show(this, "The file '" + Path.GetFileName(file) + "' contains multiple frames.\r\nDo you want to load all of them?", "Multipage File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
							{
								for (int i = 0; i < count; i++)
								{
									_images.Add(new ImageFileLoadData(file, i));
								}
							}
							else
								_images.Add(new ImageFileLoadData(file, 0));
						}
						else
							_images.Add(new ImageFileLoadData(file, 0));
					}
					else
						_images.Add(new ImageFileLoadData(file, 0));
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Error loading " + file + "\r\n" + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					fs.Close();
				}
			}

			if (_images.Count > 0) 
			{
				ImageFileLoadData data = (ImageFileLoadData)_images[0];
				this.Viewer.Images.Add(new AtalaImage(data.FileName, data.FrameIndex, null));
			}

			UpdateUndoRedoInfo();
			EnableMenuItems();
			this.statusBarLoadTime.Text = "";
		}



        #endregion

        private void Viewer_KeyDown(object sender, KeyEventArgs e)
        {
            // IF we are currently in selection mode then we will use keypresses to affect selection size
            // WSAD  with shift for 10px no shift for 1
            // p brings up properties to adjust directly
            if (Viewer.MouseTool == MouseToolType.Selection)
            {
                Rubberband s = Viewer.Selection;
                if (s != null)
                {
                    Rectangle b = s.Bounds;

                    switch (e.KeyCode) 
                    {
                        case Keys.W:
                            // do thing
                            if (e.Shift)
                            {
                                b.Height += 10;
                            } 
                            else if (e.Control)
                            {
                                b.Y -= 1;
                            }
                            else
                            {
                                b.Height += 1;
                            }
                            break;
                        case Keys.S:
                            if (e.Shift)
                            {
                                b.Height -= 10;
                            }
                            else if (e.Control)
                            {
                                b.Y += 1;
                            }
                            else
                            {
                                b.Height -= 1;
                            }
                            break;
                        case Keys.D:
                            if (e.Shift)
                            {
                                b.Width += 10;
                            }
                            else if (e.Control)
                            {
                                b.X += 1;
                            }
                            else
                            {
                                b.Width += 1;
                            }
                            break;
                        case Keys.A:
                            if (e.Shift)
                            {
                                b.Width -= 10;
                            }
                            else if (e.Control)
                            {
                                b.X -= 1;
                            }
                            else
                            {
                                b.Width -= 1;
                            }
                            break;
                        case Keys.P:
                            // do thing
                            MessageBox.Show("properties popup coming soon");
                            break;
                    }

                    s.Bounds = b;
                    Viewer_MouseMovePixel(null, null);
                }
            }
        }
    }

    public struct ImageFileLoadData
	{
		public string FileName;
		public int FrameIndex;

		public ImageFileLoadData(string fileName, int frameIndex)
		{
			this.FileName = fileName;
			this.FrameIndex = frameIndex;
		}
	}
}
