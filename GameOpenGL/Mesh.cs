using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;


namespace Model
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
        public Vector2 TexCoords;


        public static readonly int Stride = Marshal.SizeOf(default(Vertex));
        public float[] ToArray()
        {
            return new float[] { position.X, position.Y, position.Z, normal.X, normal.Y, normal.Z, TexCoords.X, TexCoords.Y };
        }
}

    public class Mesh
    {

        public List<Vertex> _vertices;
        public List<uint> _indices = new List<uint> {    0, 1, 3,  1, 2, 3   };
        public List<Texture> _textures;


        public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures)
        {
            _vertices = vertices;
            _indices = indices;
            _textures = textures;

            setupMesh();
        }

        public void Draw(Shader shader, int _EBO, int _VAO, int _VBO)
        {
            uint diffuse_texture_nr = 0;
            uint specular_texture_nr = 0;

            for (int i = 0; i < _textures.Count; i++)
            {
                _textures[i].Use(TextureUnit.Texture0 + i);

                switch (_textures[i].Type)
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

            //uint[] indices = {
            //0, 1, 3, // First Triangle
            //1, 2, 3  // Second Triangle
            //};

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

 
            GL.DrawElements(BeginMode.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

        }

        private int VAO;
        private int VBO;
        private int EBO;

        private void setupMesh()
        {

            float[] vertices = {
        // vertexes             // normals              //texture coords
        -0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,     0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,     1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,     1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,     1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,     0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,     0.0f, 0.0f,

        -0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,     0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,     1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,     1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,     1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,     0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,     0.0f, 0.0f,

        -0.5f,  0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,     1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,     1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,     0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,     0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,     0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,     1.0f, 0.0f,

         0.5f,  0.5f,  0.5f,    1.0f,  0.0f,  0.0f,     1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,    1.0f,  0.0f,  0.0f,     1.0f, 1.0f,
         0.5f, -0.5f, -0.5f,    1.0f,  0.0f,  0.0f,     0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,    1.0f,  0.0f,  0.0f,     0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,    1.0f,  0.0f,  0.0f,     0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,    1.0f,  0.0f,  0.0f,     1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,     0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,     1.0f, 1.0f,
         0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,     1.0f, 0.0f,
         0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,     1.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,     0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,     0.0f, 1.0f,

        -0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,     0.0f, 1.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,     1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,     1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,     1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,     0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,     0.0f, 1.0f
    };


            //VBO1
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(_vertices[0]) * vertices.Length/*vertices.Length * sizeof(float)*/, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            //            GL.BufferData(BufferTarget.ArrayBuffer, (_vertices.Count * 8 * sizeof(float)), (from v in _vertices select v.ToArray()), BufferUsageHint.StaticDraw);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Count * Vertex.Stride), _vertices.ToArray(), BufferUsageHint.StaticDraw);


            //VAO1
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);



            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }

}
