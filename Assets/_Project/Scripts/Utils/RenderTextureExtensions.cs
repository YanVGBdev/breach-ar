using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// RenderTexture extension methods
    /// </summary>
    public static class RenderTextureExtensions
    {
        /// <summary>
        /// Convert to Texture2D
        /// </summary>
        public static Texture2D ToTexture2D(this RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            return texture;
        }

        /// <summary>
        /// Convert to PNG bytes
        /// </summary>
        public static byte[] ToPNG(this RenderTexture renderTexture)
        {
            Texture2D texture = renderTexture.ToTexture2D();
            byte[] bytes = texture.EncodeToPNG();
            Object.Destroy(texture);
            return bytes;
        }

        /// <summary>
        /// Convert to JPG bytes
        /// </summary>
        public static byte[] ToJPG(this RenderTexture renderTexture, int quality = 75)
        {
            Texture2D texture = renderTexture.ToTexture2D();
            byte[] bytes = texture.EncodeToJPG(quality);
            Object.Destroy(texture);
            return bytes;
        }

        /// <summary>
        /// Save as PNG file
        /// </summary>
        public static void SaveAsPNG(this RenderTexture renderTexture, string path)
        {
            byte[] bytes = renderTexture.ToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
        }

        /// <summary>
        /// Save as JPG file
        /// </summary>
        public static void SaveAsJPG(this RenderTexture renderTexture, string path, int quality = 75)
        {
            byte[] bytes = renderTexture.ToJPG(quality);
            System.IO.File.WriteAllBytes(path, bytes);
        }
    }
}
