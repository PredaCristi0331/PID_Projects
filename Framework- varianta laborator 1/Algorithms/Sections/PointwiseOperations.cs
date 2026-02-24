using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {

        #region LUT Generators
        public static byte[] GetLinearLUT(double alpha, double beta)
        {
            byte[] lut = new byte[256];
            for (int r = 0; r < 256; r++)
            {
                double val = alpha * r + beta;

                if (val < 0) val = 0;
                if (val > 255) val = 255;

                lut[r] = (byte)val;
            } 
            return lut;
        }

        public static byte[] GetGammaLUT(double gamma)
        {
            byte[] lut = new byte[256];
            double c = Math.Pow(255.0, 1 - gamma);
            for (int r = 0; r < 256; r++)
            {

                double val = c * Math.Pow(r / 255.0, 1-gamma);

                if (val < 0) val = 0;
                if (val > 255) val = 255;

                lut[r] = (byte)val;
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