using Emgu.CV;
using Emgu.CV.Structure;

using System.Windows;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using Framework.View;
using static Framework.Utilities.DataProvider;
using static Framework.Utilities.DrawingHelper;
using static Framework.Utilities.FileHelper;
using static Framework.Converters.ImageConverter;

using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;

namespace Framework.ViewModel
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;

        public MenuCommands(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private ImageSource InitialImage
        {
            get => _mainVM.InitialImage;
            set => _mainVM.InitialImage = value;
        }

        private ImageSource ProcessedImage
        {
            get => _mainVM.ProcessedImage;
            set => _mainVM.ProcessedImage = value;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayscaleImageCommand;
        public RelayCommand LoadGrayscaleImageCommand
        {
            get
            {
                if (_loadGrayscaleImageCommand == null)
                    _loadGrayscaleImageCommand = new RelayCommand(LoadGrayscaleImage);
                return _loadGrayscaleImageCommand;
            }
        }

        private void LoadGrayscaleImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a grayscale picture");
            if (fileName != null)
            {
                GrayInitialImage = new Image<Gray, byte>(fileName);
                InitialImage = Convert(GrayInitialImage);
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a color picture");
            if (fileName != null)
            {
                ColorInitialImage = new Image<Bgr, byte>(fileName);
                InitialImage = Convert(ColorInitialImage);
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                GrayProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                ColorProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                Process.Start(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes from initial canvas
        private ICommand _removeInitialDrawnShapesCommand;
        public ICommand RemoveInitialDrawnShapesCommand
        {
            get
            {
                if (_removeInitialDrawnShapesCommand == null)
                    _removeInitialDrawnShapesCommand = new RelayCommand(RemoveInitialDrawnShapes);
                return _removeInitialDrawnShapesCommand;
            }
        }

        private void RemoveInitialDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from processed canvas
        private ICommand _removeProcessedDrawnShapesCommand;
        public ICommand RemoveProcessedDrawnShapesCommand
        {
            get
            {
                if (_removeProcessedDrawnShapesCommand == null)
                    _removeProcessedDrawnShapesCommand = new RelayCommand(RemoveProcessedDrawnShapes);
                return _removeProcessedDrawnShapesCommand;
            }
        }

        private void RemoveProcessedDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from both canvases
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            var canvases = (object[])parameter;
            RemoveUiElements(canvases[0] as Canvas);
            RemoveUiElements(canvases[1] as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != Application.Current.MainWindow)
                {
                    window.Close();
                }
            }

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0] as Canvas);
            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (MagnifierOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
        }
        #endregion

        #region Visualize color levels

        #region Row color levels
        private ICommand _rowColorLevelsCommand;
        public ICommand RowColorLevelsCommand
        {
            get
            {
                if (_rowColorLevelsCommand == null)
                    _rowColorLevelsCommand = new RelayCommand(RowColorLevels);
                return _rowColorLevelsCommand;
            }
        }

        private void RowColorLevels(object parameter)
        {
            if (RowColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Row);
            window.Show();
        }
        #endregion

        #region Column color levels
        private ICommand _columnColorLevelsCommand;
        public ICommand ColumnColorLevelsCommand
        {
            get
            {
                if (_columnColorLevelsCommand == null)
                    _columnColorLevelsCommand = new RelayCommand(ColumnColorLevels);
                return _columnColorLevelsCommand;
            }
        }

        private void ColumnColorLevels(object parameter)
        {
            if (ColumnColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Column);
            window.Show();
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (InitialHistogramOn == true) return;
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            HistogramWindow window = null;

            if (ColorInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialColor);
            }
            else if (GrayInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialGray);
            }

            window.Show();
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (ProcessedHistogramOn == true) return;
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            HistogramWindow window = null;

            if (ColorProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedColor);
            }
            else if (GrayProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedGray);
            }

            window.Show();
        }
        #endregion

        #endregion

        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Binary image
        private ICommand _binaryImageCommand;
        public ICommand BinaryImageCommand
        {
            get
            {
                if (_binaryImageCommand == null)
                    _binaryImageCommand = new RelayCommand(BinaryImage);
                return _invertImageCommand;
            }
        }

        private void BinaryImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string> { "Thresholding:" };
            DialogWindow diag = new DialogWindow(_mainVM, labels);
            diag.ShowDialog();

            List<double> values = diag.GetValues();

            if (values[0]< 10 || values[0] > 145)
            {
                MessageBox.Show("Threshold value must be between 10 and 145!");
                return;
            }

            byte T = (byte) values[0];


            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Binary(GrayInitialImage, T);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                GrayProcessedImage = Tools.Binary(GrayProcessedImage, T);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Mirror image
        private ICommand _mirrorImageCommand;
        public ICommand MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                return _mirrorImageCommand;
            }
        }

        private void MirrorImage(object parameter)
        {
            if (InitialImage == null) { MessageBox.Show("Please add an image!"); return; }
            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Mirror(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Mirror(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Rotate image (Clockwise & Anti-Clockwise)
        private ICommand _rotateCWImageCommand;
        public ICommand RotateCWImageCommand
        {
            get { return _rotateCWImageCommand ?? (_rotateCWImageCommand = new RelayCommand(RotateCWImage)); }
        }

        private void RotateCWImage(object parameter)
        {
            if (InitialImage == null) { MessageBox.Show("Please add an image!"); return; }
            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }

        private ICommand _rotateACWImageCommand;
        public ICommand RotateACWImageCommand
        {
            get { return _rotateACWImageCommand ?? (_rotateACWImageCommand = new RelayCommand(RotateACWImage)); }
        }

        private void RotateACWImage(object parameter)
        {
            if (InitialImage == null) { MessageBox.Show("Please add an image!"); return; }
            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateAntiClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateAntiClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Crop image
        private ICommand _enableCropModeCommand;
        public ICommand EnableCropModeCommand
        {
            get { return _enableCropModeCommand ?? (_enableCropModeCommand = new RelayCommand(EnableCropMode)); }
        }

        private void EnableCropMode(object parameter)
        {
            if (InitialImage == null) { MessageBox.Show("Please add an image!"); return; }

            MessageBox.Show("Modul Crop activat. Te rog să desenezi un dreptunghi pe imaginea inițială pentru a decupa.");
 
        }

       
        public void ExecuteCrop(System.Drawing.Rectangle selectionRect)
        {
            if (selectionRect.Width <= 0 || selectionRect.Height <= 0) return;

            double mean, stdDev;

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Crop(GrayInitialImage, selectionRect, out mean, out stdDev);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Crop(ColorInitialImage, selectionRect, out mean, out stdDev);
                ProcessedImage = Convert(ColorProcessedImage);
            }

           
        }
        #endregion

        #region Contrast and Brightness
        private ICommand _contrastBrightnessCommand;
        public ICommand ContrastBrightnessCommand
        {
            get
            {
                if (_contrastBrightnessCommand == null)
                    _contrastBrightnessCommand = new RelayCommand(ContrastBrightness);
                return _contrastBrightnessCommand;
            }
        }

        private void ContrastBrightness(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string> { "Alpha (Contrast > 0):", "Beta (Brightness):" };
            DialogWindow diag = new DialogWindow(_mainVM, labels);
            diag.ShowDialog();

            List<double> values = diag.GetValues();

            if (values == null || values.Count < 2) return; 

            double alpha = values[0];
            double beta = values[1];

            if (alpha <= 0)
            {
                MessageBox.Show("Alpha must be strictly positive (> 0)!");
                return;
            }

            
            byte[] lut = PointwiseOperations.GetLinearLUT(alpha, beta);

            
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.ApplyLUT(GrayInitialImage, lut);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.ApplyLUT(ColorInitialImage, lut);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Gamma Operator
        private ICommand _gammaOperatorCommand;
        public ICommand GammaOperatorCommand
        {
            get
            {
                if (_gammaOperatorCommand == null)
                    _gammaOperatorCommand = new RelayCommand(GammaOperator);
                return _gammaOperatorCommand;
            }
        }

        private void GammaOperator(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string> { "Gamma (> 0):" };
            DialogWindow diag = new DialogWindow(_mainVM, labels);
            diag.ShowDialog();

            List<double> values = diag.GetValues();

            if (values == null || values.Count < 1) return;

            double gamma = values[0];

            if (gamma <= 0)
            {
                MessageBox.Show("Gamma must be strictly positive (> 0)!");
                return;
            }

            
            byte[] lut = PointwiseOperations.GetGammaLUT(gamma);

            
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.ApplyLUT(GrayInitialImage, lut);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.ApplyLUT(ColorInitialImage, lut);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region EM Operator
        private ICommand _emOperatorCommand;
        public ICommand EMOperatorCommand
        {
            get
            {
                if (_emOperatorCommand == null)
                    _emOperatorCommand = new RelayCommand(EMOperator);
                return _emOperatorCommand;
            }
        }

        private void EMOperator(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string> { "m (threshold):", "E (slope):" };
            DialogWindow diag = new DialogWindow(_mainVM, labels);
            diag.ShowDialog();

            List<double> values = diag.GetValues();

            if (values == null || values.Count < 2) return; 

            double m = values[0];
            double E = values[1];

            
            byte[] lut = PointwiseOperations.GetEMOperatorLUT(m, E);

            
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.ApplyLUT(GrayInitialImage, lut);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.ApplyLUT(ColorInitialImage, lut);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("It is possible to convert only color images!");
            }
        }
        #endregion

        #endregion

        #region Pointwise operations


        #endregion

        #region Thresholding
        
        #region Otsu Threshold
        private ICommand _otsuThresholdCommand;
        public ICommand OtsuThresholdCommand
        {
            get
            {
                if (_otsuThresholdCommand == null)
                    _otsuThresholdCommand = new RelayCommand(OtsuThreshold);
                return _otsuThresholdCommand;
            }
        }

        private void OtsuThreshold(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            byte optimalT = 0;

            if (GrayInitialImage != null)
            {
                
                optimalT = Thresholding.GetOtsuThreshold(GrayInitialImage);

                
                GrayProcessedImage = Tools.Binary(GrayInitialImage, optimalT);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                
                Image<Gray, byte> grayTemp = Tools.Convert(ColorInitialImage);

                
                optimalT = Thresholding.GetOtsuThreshold(grayTemp);

                
                GrayProcessedImage = Tools.Binary(grayTemp, optimalT);
                ProcessedImage = Convert(GrayProcessedImage);
            }

  
            MessageBox.Show($"Binarizarea Otsu a fost aplicată cu succes!\nPragul global calculat automat este: {optimalT}", "Otsu Threshold Result");
        }
        #endregion

        #endregion

        #region Filters

        #region Custom filter 1
        private ICommand _customFilter1Command;
        public ICommand CustomFilter1Command
        {
            get
            {
                if (_customFilter1Command == null)
                    _customFilter1Command = new RelayCommand(CustomFilter1);
                return _customFilter1Command;
            }
        }
        private void CustomFilter1(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            double[,] kernel = new double[3, 3]
            {
                { 0.1, 0.1, 0.1 },
                { 0.1, 0.2, 0.1 },
                { 0.1, 0.1, 0.1 },
            };

            if (GrayInitialImage != null)
            {

                GrayProcessedImage = Filters.ApplyFilter(GrayInitialImage, kernel);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Filters.ApplyFilter(ColorInitialImage, kernel);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region
        #region Gaussian Filter
        private ICommand _gaussianFilterCommand;
        public ICommand GaussianFilterCommand => _gaussianFilterCommand ?? (_gaussianFilterCommand = new RelayCommand(ApplyGaussianFilter));

        private void ApplyGaussianFilter(object parameter)
        {
            if (InitialImage == null) { MessageBox.Show("Please add an image!"); return; }
            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string> { "Sigma X:", "Sigma Y:" };
            DialogWindow diag = new DialogWindow(_mainVM, labels);
            diag.ShowDialog();

            List<double> values = diag.GetValues();
            if (values == null || values.Count < 2) return;

            double sigmaX = values[0];
            double sigmaY = values[1];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.GaussFiltering(GrayInitialImage, sigmaX, sigmaY);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Filters.GaussFiltering(ColorInitialImage, sigmaX, sigmaY);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Directional Sobel Edge Detection
        private ICommand _directionalSobelCommand;
        public ICommand DirectionalSobelCommand => _directionalSobelCommand ?? (_directionalSobelCommand = new RelayCommand(ApplyDirectionalSobel));

        private void ApplyDirectionalSobel(object parameter)
        {
            if (InitialImage == null) { MessageBox.Show("Please add an image!"); return; }
            ClearProcessedCanvas(parameter as Canvas);


            List<string> labels = new List<string> {
        "Threshold T:",
        "Target Angle \u03B8 (degrees):",
        "Deviation \u00B1 (degrees):"
    };

            DialogWindow diag = new DialogWindow(_mainVM, labels);
            diag.ShowDialog();

            List<double> values = diag.GetValues();
            if (values == null || values.Count < 3) return;

            double threshold = values[0];
            double targetAngle = values[1];
            double deviation = values[2];


            Image<Gray, byte> processImage;
            if (ColorInitialImage != null)
            {
                processImage = Tools.Convert(ColorInitialImage);
            }
            else
            {
                processImage = GrayInitialImage;
            }


            GrayProcessedImage = Filters.DirectionalSobel(processImage, threshold, targetAngle, deviation);
            ProcessedImage = Convert(GrayProcessedImage);
        }
        #endregion
        #endregion



        #endregion

        #region Morphological operations
        #endregion

        #region Geometric transformations
        #endregion

        #region Segmentation


        #endregion

        #region Use processed image as initial image
        private ICommand _useProcessedImageAsInitialImageCommand;
        public ICommand UseProcessedImageAsInitialImageCommand
        {
            get
            {
                if (_useProcessedImageAsInitialImageCommand == null)
                    _useProcessedImageAsInitialImageCommand = new RelayCommand(UseProcessedImageAsInitialImage);
                return _useProcessedImageAsInitialImageCommand;
            }
        }

        private void UseProcessedImageAsInitialImage(object parameter)
        {
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0] as Canvas);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
                InitialImage = Convert(GrayInitialImage);
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
                InitialImage = Convert(ColorInitialImage);
            }

            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion



    }
}