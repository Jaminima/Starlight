using System;

namespace StarlightGame.GL
{
    public class Renderer
    {
        public byte[] GetColorArray(int width, int height)
        {
            // Demo: return a simple gradient
            byte[] colors = new byte[width * height * 3];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * 3;
                    colors[index] = (byte)(x * 255 / width); // Red
                    colors[index + 1] = (byte)(y * 255 / height); // Green
                    colors[index + 2] = 128; // Blue
                }
            }
            return colors;
        }
    }
}