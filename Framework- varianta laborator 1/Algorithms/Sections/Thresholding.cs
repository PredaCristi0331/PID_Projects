using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        #region Otsu Threshold
        public static byte GetOtsuThreshold(Image<Gray, byte> inputImage)
        {
            
            int[] H = new int[256];
            int n = inputImage.Width * inputImage.Height;

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    H[inputImage.Data[y, x, 0]]++;
                }
            }

            
            double[] p = new double[256];
            for (int k = 0; k < 256; k++)
            {
                p[k] = (double)H[k] / n;
            }

            byte T = 0;
            double maxInterVariance = 0;

            
            for (int t = 1; t < 254; t++)
            {
                double P1 = 0;
                for (int k = 0; k <= t; k++)
                {
                    P1 += p[k];
                }

                double P2 = 1.0 - P1;

                
                if (P1 == 0 || P2 == 0) continue;

                double mu1 = 0;
                for (int k = 0; k <= t; k++)
                {
                    mu1 += (k * p[k]);
                }
                mu1 /= P1;

                double mu2 = 0;
                for (int k = t + 1; k < 256; k++)
                {
                    mu2 += (k * p[k]);
                }
                mu2 /= P2;

                
                double interVariance = P1 * P2 * (mu1 - mu2) * (mu1 - mu2);

                if (interVariance > maxInterVariance)
                {
                    maxInterVariance = interVariance;
                    T = (byte)t;
                }
            }

            return T;
        }
        #endregion
    }
}