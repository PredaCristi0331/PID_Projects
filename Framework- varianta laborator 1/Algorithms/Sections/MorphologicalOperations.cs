using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
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
        #region Connected Components (BFS)
        public static Image<Bgr, byte> ConnectedComponents(Image<Gray, byte> inputImage, byte T)
        {
            
            Image<Gray, byte> binary = Tools.Tools.Binary(inputImage, T);
            int width = binary.Width;
            int height = binary.Height;

            
            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);
            bool[,] visited = new bool[height, width];
            Random rand = new Random();

            
            int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
            int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    
                    if (binary.Data[y, x, 0] == 255 && !visited[y, x])
                    {
                        
                        Bgr color = new Bgr(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));

                        
                        Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
                        queue.Enqueue(new Tuple<int, int>(y, x));
                        visited[y, x] = true;

                        while (queue.Count > 0)
                        {
                            Tuple<int, int> current = queue.Dequeue();
                            int curY = current.Item1;
                            int curX = current.Item2;

                            
                            result[curY, curX] = color;

                            
                            for (int i = 0; i < 8; i++)
                            {
                                int ny = curY + dy[i];
                                int nx = curX + dx[i];

                                
                                if (ny >= 0 && ny < height && nx >= 0 && nx < width &&
                                    binary.Data[ny, nx, 0] == 255 && !visited[ny, nx])
                                {
                                    visited[ny, nx] = true;
                                    queue.Enqueue(new Tuple<int, int>(ny, nx));
                                }
                            }
                        }
                    }
                    else if (binary.Data[y, x, 0] == 0)
                    {
                        
                        result[y, x] = new Bgr(0, 0, 0);
                    }
                }
            }
            return result;
        }
        #endregion
    }
}