using UnityEngine;

namespace GUIGUI17F
{
    public class TextureScaler
    {
        private static readonly Rect TargetRect = new Rect(0, 0, 1, 1);
        
        /// <summary>
        /// warning: this method will override the RenderTexture setup
        /// </summary>
        public static void Scale(Texture2D texture, int width, int height, bool useMipmap, FilterMode filterMode)
        {
            texture.filterMode = filterMode;
            texture.Apply(useMipmap);
            
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 32);
            Graphics.SetRenderTarget(renderTexture);
            GL.LoadPixelMatrix(0, 1, 1, 0);
            GL.Clear(true, true, Color.clear);
            Graphics.DrawTexture(TargetRect, texture);
            RenderTexture.ReleaseTemporary(renderTexture);
            
            texture.Reinitialize(width, height);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0, useMipmap);
            texture.Apply(useMipmap); 
        }
    }
}