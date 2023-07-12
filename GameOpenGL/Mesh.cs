using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
namespace Mesh
{
    public enum TextureType
    {
        Diffuse,
        Specular
    }
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 color;
    }

    public struct Texture
    {
        public uint id;
        public TextureType type;
    }

    public class Mesh
    {

        public List<Vertex> vertices;
        public List<uint> indices;
        public List<Texture> textures;


        Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;

            setupMesh();
        }

        public void Draw(Shader shader)
        {
            uint diffuse_texture_nr = 0;
            uint specular_texture_nr = 0;

            for (int i = 0; i < textures.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);

                switch (textures[i].type)
                {
                    case TextureType.Diffuse:
                        shader.SetInt(String.Format("material.texture_diffuse{0}", diffuse_texture_nr), i);
                        diffuse_texture_nr++;
                        break;

                    case TextureType.Specular:
                        shader.SetInt(String.Format("material.texture_specular{0}", specular_texture_nr), i);
                        specular_texture_nr++;
                        break;

                    default:
                        break;
                }
            }
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

        }

        private int VAO;
        private int VBO;
        private int EBO;

        private void setupMesh()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, vertices.Count * Marshal.SizeOf(vertices[0]), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(vertices[0]), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(vertices[0]), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf(vertices[0]), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);


            GL.BindVertexArray(0);
        }
    }

}
