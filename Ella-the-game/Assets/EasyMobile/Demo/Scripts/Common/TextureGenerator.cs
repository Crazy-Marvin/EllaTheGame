using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Demo
{
    public static class TextureGenerator
    {
        public static Texture2D GenerateRandomTexture2D(int width, int height, Color[] randomColors)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] colors = texture.GetPixels();

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    int index = x + y * texture.width;
                    Color color = randomColors[UnityEngine.Random.Range(0, randomColors.Length)];
                    colors[index] = color;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    }
}
