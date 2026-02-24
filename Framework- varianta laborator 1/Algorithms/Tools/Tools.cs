using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion

        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, byte T)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (inputImage.Data[y, x, 0] >= T)
                        result.Data[y, x, 0] = 255;

                }
            }
            return result;
        }
        #endregion


        #region Mirror image
        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - 1 - x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - 1 - x, 0];
                    result.Data[y, x, 1] = inputImage.Data[y, inputImage.Width - 1 - x, 1];
                    result.Data[y, x, 2] = inputImage.Data[y, inputImage.Width - 1 - x, 2];
                }
            }
            return result;
        }
        #endregion

        #region Rotate image
        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
           
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                    result.Data[x, inputImage.Height - 1 - y, 1] = inputImage.Data[y, x, 1];
                    result.Data[x, inputImage.Height - 1 - y, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }

        public static Image<Gray, byte> RotateAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                    result.Data[inputImage.Width - 1 - x, y, 1] = inputImage.Data[y, x, 1];
                    result.Data[inputImage.Width - 1 - x, y, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }
        #endregion

        #region Crop image
        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage, Rectangle roi, out double mean, out double stdDev)
        {
            inputImage.ROI = roi;
            Image<Gray, byte> result = inputImage.Copy();
            inputImage.ROI = Rectangle.Empty; 

            CalculateStats(result, out mean, out stdDev);
            return result;
        }

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, Rectangle roi, out double mean, out double stdDev)
        {
            inputImage.ROI = roi;
            Image<Bgr, byte> result = inputImage.Copy();
            inputImage.ROI = Rectangle.Empty;

            
            Image<Gray, byte> grayTemp = result.Convert<Gray, byte>();
            CalculateStats(grayTemp, out mean, out stdDev);
            return result;
        }

        private static void CalculateStats(Image<Gray, byte> img, out double mean, out double stdDev)
        {
            double sum = 0;
            int totalPixels = img.Width * img.Height;

            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    sum += img.Data[y, x, 0];

            mean = sum / totalPixels;

            double varianceSum = 0;
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    varianceSum += Math.Pow(img.Data[y, x, 0] - mean, 2);

            stdDev = Math.Sqrt(varianceSum / totalPixels);
        }
        #endregion

        

        #region LUT Generators
        public static byte[] GetLinearLUT(double alpha, double beta)
        {
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double val = alpha * i + beta;

                if (val < 0) val = 0;
                if (val > 255) val = 255;

                lut[i] = (byte)val;
            }
            return lut;
        }

        public static byte[] GetGammaLUT(double gamma)
        {
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                
                double val = 255.0 * Math.Pow(i / 255.0, gamma);

                if (val < 0) val = 0;
                if (val > 255) val = 255;

                lut[i] = (byte)val;
            }
            return lut;
        }
        #endregion

        #region Apply LUT
        public static Image<Gray, byte> ApplyLUT(Image<Gray, byte> inputImage, byte[] lut)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    
                    result.Data[y, x, 0] = lut[inputImage.Data[y, x, 0]];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> ApplyLUT(Image<Bgr, byte> inputImage, byte[] lut)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    
                    result.Data[y, x, 0] = lut[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = lut[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = lut[inputImage.Data[y, x, 2]];
                }
            }
            return result;
        }
        #endregion
    }
}