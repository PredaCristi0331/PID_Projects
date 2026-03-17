using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading.Tasks;

namespace Algorithms.Utilities
{
    public class Utils
    {
        #region Compute histogram
        public static int[] ComputeHistogram(Image<Gray, byte> inputImage)
        {
            int[] histogram = new int[256];

            Parallel.For(0, inputImage.Height, (int y) =>
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    ++histogram[inputImage.Data[y, x, 0]];
                }
            });

            return histogram;
        }
        #endregion

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

        public static byte Clamp(double s)
        {
            if (s < 0) return 0;
            if (s > 255) return 255;
            s += 0.5;
            return (byte)s;
        }
    }
}