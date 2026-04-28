using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        #region Dilation
        public static Image<Gray, byte> Dilation(Image<Gray, byte> inputImage, int h, int w, byte T, int option)
        {
            Image<Gray, byte> binary = Tools.Tools.Binary(inputImage, T);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int hOffset = h / 2;
            int wOffset = w / 2;

            for (int y = 0; y < binary.Height; y++)
            {
                for (int x = 0; x < binary.Width; x++)
                {
                    bool found = false;

                    
                    for (int i = -hOffset; i <= hOffset; i++)
                    {
                        for (int j = -wOffset; j <= wOffset; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;

                            
                            if (ny >= 0 && ny < binary.Height && nx >= 0 && nx < binary.Width)
                            {
                                byte pixel = binary.Data[ny, nx, 0];

                                if (option == 1) 
                                {
                                    if (pixel == 255) { found = true; break; }
                                }
                                else 
                                {
                                    if (pixel == 0) { found = true; break; }
                                }
                            }
                        }
                        if (found) break;
                    }

                    if (option == 1)
                        result.Data[y, x, 0] = found ? (byte)255 : (byte)0;
                    else
                        result.Data[y, x, 0] = found ? (byte)0 : (byte)255;
                }
            }
            return result;
        }
        #endregion

        #region Erosion
        public static Image<Gray, byte> Erosion(Image<Gray, byte> inputImage, int h, int w, byte T, int option)
        {
            Image<Gray, byte> binary = Tools.Tools.Binary(inputImage, T);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            int hOffset = h / 2;
            int wOffset = w / 2;

            for (int y = 0; y < binary.Height; y++)
            {
                for (int x = 0; x < binary.Width; x++)
                {
                    bool conditionMet = true;

                    for (int i = -hOffset; i <= hOffset; i++)
                    {
                        for (int j = -wOffset; j <= wOffset; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;

                            if (ny >= 0 && ny < binary.Height && nx >= 0 && nx < binary.Width)
                            {
                                byte pixel = binary.Data[ny, nx, 0];

                                if (option == 1) 
                                {
                                    if (pixel == 0) { conditionMet = false; break; }
                                }
                                else 
                                {
                                    if (pixel == 255) { conditionMet = false; break; }
                                }
                            }
                        }
                        if (!conditionMet) break;
                    }

                    if (option == 1)
                        result.Data[y, x, 0] = conditionMet ? (byte)255 : (byte)0;
                    else
                        result.Data[y, x, 0] = conditionMet ? (byte)0 : (byte)255;
                }
            }
            return result;
        }
        #endregion

        #region Opening
        public static Image<Gray, byte> Opening(Image<Gray, byte> inputImage, int h, int w, byte T, int option)
        {
            
            Image<Gray, byte> eroded = Erosion(inputImage, h, w, T, option);

            
            return Dilation(eroded, h, w, 128, option);
        }
        #endregion

        #region Closing
        public static Image<Gray, byte> Closing(Image<Gray, byte> inputImage, int h, int w, byte T, int option)
        {
            Image<Gray, byte> dilated = Dilation(inputImage, h, w, T, option);

            return Erosion(dilated, h, w, 128, option);
        }
        #endregion
    }
}