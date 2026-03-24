using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using System;


namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<Gray, byte> ApplyFilter(Image<Gray, byte> inputImage, double[,] kernel)
        {
            int h = kernel.GetLength(0);
            int w = kernel.GetLength(1);
            int hOffset = h / 2;
            int wOffset = w / 2;

            Image<Gray, byte> borderImage = 
                new Image<Gray, byte>(inputImage.Width + 2 * wOffset, inputImage.Height + 2 * hOffset);

            CvInvoke.CopyMakeBorder(inputImage, borderImage, 
                hOffset, hOffset, wOffset, wOffset, Emgu.CV.CvEnum.BorderType.Reflect101);

            Image<Gray, byte> result = inputImage.Clone();

            for (int y = h / 2; y < borderImage.Height - h / 2; y++)
            {
                for (int x = w / 2; x < borderImage.Width - w / 2; x++)
                {
                    double sum = 0;

                    for (int i = -h / 2; i <= h / 2; i++)
                    {
                        for (int j = -w / 2; j <= w / 2; j++)
                        {
                            sum += borderImage.Data[y + i, x + j, 0] * kernel[i + h / 2, j + w / 2];
                        }
                    }
                    result.Data[y-h/2, x-w/2, 0] = Utils.Clamp(sum);
                }
            }
            return result;
        }


        public static Image<Bgr, byte> ApplyFilter(Image<Bgr, byte> inputImage, double[,] kernel)
        {
            int h = kernel.GetLength(0);
            int w = kernel.GetLength(1);
            int hOffset = h / 2;
            int wOffset = w / 2;
            Image<Bgr, byte> result = inputImage.Clone();

            for (int y = h / 2; y < inputImage.Height - h / 2; y++)
            {
                for (int x = w / 2; x < inputImage.Width - w / 2; x++)
                {
                    for (int channel = 0; channel < inputImage.NumberOfChannels; channel++)
                    {
                        double sum = 0;
                        for (int i = -h / 2; i <= h / 2; i++)
                        {
                            for (int j = -w / 2; j <= w / 2; j++)
                            {
                                sum += inputImage.Data[y + i, x + j, channel] * kernel[i + h / 2, j + w / 2];
                            }
                        }
                        result.Data[y, x, channel] = Utils.Clamp(sum);
                    }
                }
            }
            return result;
        }

        #region 

        public static double[,] GaussMask(double sigma, bool isHorizontal)
        {
            
            if (sigma <= 0) return new double[,] { { 1 } };

            
            int l = (int)Math.Ceiling(4 * sigma);
            if (l % 2 == 0) l++;

            int offset = l / 2;

            
            double[,] mask = isHorizontal ? new double[1, l] : new double[l, 1];

            double sum = 0; 
            double constant = 1.0 / (Math.Sqrt(2 * Math.PI) * sigma);
            double denominator = 2 * sigma * sigma;

            
            for (int z = -offset; z <= offset; z++)
            {
                double exponent = -(z * z) / denominator;
                double val = constant * Math.Exp(exponent);

                if (isHorizontal)
                    mask[0, z + offset] = val;
                else
                    mask[z + offset, 0] = val;

                sum += val;
            }

            
            for (int i = 0; i < l; i++)
            {
                if (isHorizontal)
                    mask[0, i] /= sum;
                else
                    mask[i, 0] /= sum;
            }

            return mask;
        }


        public static Image<TColor, TDepth> GaussFiltering<TColor, TDepth>(Image<TColor, TDepth> img, double sigmaX, double sigmaY)
    where TColor : struct, IColor
    where TDepth : new()
        {
            
            double[,] maskX = GaussMask(sigmaX, true);

           
            Image<TColor, TDepth> tempImage = ApplyFilter(img, maskX);

            
            double[,] maskY = GaussMask(sigmaY, false);

            
            Image<TColor, TDepth> finalImage = ApplyFilter(tempImage, maskY);

            return finalImage;
        }

        private static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(Image<TColor, TDepth> tempImage, double[,] maskY)
            where TColor : struct, IColor
            where TDepth : new()
        {
            int h = maskY.GetLength(0);
            int w = maskY.GetLength(1);
            int hOffset = h / 2;
            int wOffset = w / 2;
            Image<TColor,TDepth> result = tempImage.Clone();

            for (int y = h / 2; y < tempImage.Height - h / 2; y++)
            {
                for (int x = w / 2; x < tempImage.Width - w / 2; x++)
                {
                    for (int channel = 0; channel < tempImage.NumberOfChannels; channel++)
                    {
                        double sum = 0;
                        for (int i = -h / 2; i <= h / 2; i++)
                        {
                            for (int j = -w / 2; j <= w / 2; j++)
                            {
                                sum += Convert.ToDouble(tempImage.Data[y + i, x + j, channel]) * maskY[i + h / 2, j + w / 2];
                            }
                        }
                        result.Data[y, x, channel] = (TDepth)(object)Utils.Clamp(sum);
                    }
                }
            }
            return result;
        }





        #endregion

    }
}