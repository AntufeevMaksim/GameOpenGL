using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;


namespace Model
{
    public class Texture
    {
        int Handle;


        public string Path { get; private set; }
        public TextureType Type { get; private set; }
        public Texture(string path, TextureType type, Assimp.TextureWrapMode wrapModeU, Assimp.TextureWrapMode wrapModeV, float[] borderColor)
        {
            Path = path;
            Type = type;

            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GetWrapMode(wrapModeU));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GetWrapMode(wrapModeV));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            Type = type;


        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        private TextureWrapMode GetWrapMode(Assimp.TextureWrapMode mode)
        {
            switch (mode)
            {
                case Assimp.TextureWrapMode.Mirror:
                    return TextureWrapMode.MirroredRepeat;

                case Assimp.TextureWrapMode.Clamp:
                    return TextureWrapMode.ClampToBorder;

                case Assimp.TextureWrapMode.Decal:
                    return TextureWrapMode.ClampToEdge;

                case Assimp.TextureWrapMode.Wrap:
                    return TextureWrapMode.Repeat;

                default:
                    return TextureWrapMode.MirroredRepeat;


            }
        }
    }
}