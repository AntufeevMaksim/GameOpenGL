using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

using Assimp;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ObjLoader.Loader.Data;

namespace Model
{
    /// <summary>
    /// set of meshes with textures which constitute a complete graphical representation of object 
    /// </summary>
    public class Model
    {
        private Shader _shader;
        private List<Mesh> _meshes = new List<Mesh>();
        private Scene _model;
        private List<Texture> _textures_loaded = new List<Texture>();
        private string _loadingPath;
        public Model(string loadingPath, string fileName, Shader shader)
        {
            _shader = shader;
            _loadingPath = loadingPath;
            AssimpContext importer = new AssimpContext();
            _model = importer.ImportFile(loadingPath + fileName, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
            ProcessNode(_model.RootNode);
            _shader = shader;
        }
        public void Draw(Matrix4 transformations)
        {
            _shader.Use();
            _shader.SetMat4("model", transformations);
            foreach (var mesh in _meshes)
            {
                mesh.Draw(_shader);
            }
        }

        private void ProcessNode(Node node)
        {
            foreach(int index in node.MeshIndices)
            {
                Assimp.Mesh mesh = _model.Meshes[index];
                _meshes.Add(ProcessMesh(mesh));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i]);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i <  mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex();
                Vector3 vector3;

                vector3.X = mesh.Vertices[i].X;
                vector3.Z = mesh.Vertices[i].Y;
                vector3.Y = mesh.Vertices[i].Z;
                vertex.position = vector3;

                vector3.X = mesh.Normals[i].X;
                vector3.Z = mesh.Normals[i].Y;
                vector3.Y = mesh.Normals[i].Z;
                vertex.normal = vector3;

                if (mesh.TextureCoordinateChannelCount != 0)    //if mesh has texture coordinates
                {
                    Vector2 tex_coords;

                    tex_coords.X = mesh.TextureCoordinateChannels[0][i].X;
                    tex_coords.Y = mesh.TextureCoordinateChannels[0][i].Y;
                    vertex.TexCoords = tex_coords;
                }
                else
                {
                    vertex.TexCoords = new Vector2(0.0f);
                }
                vertices.Add(vertex);
            }

            indices = new List<uint>(mesh.GetUnsignedIndices());

            if (mesh.MaterialIndex >= 0)    //if mesh has material
            {
                Assimp.Material material = _model.Materials[mesh.MaterialIndex];

                textures.AddRange(LoadMaterialTextures(material));

                Vector4 color = new Vector4( material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B, material.ColorDiffuse.A );
                return new Mesh(vertices, indices, textures, color);
            }

            return new Mesh(vertices, indices, textures, new Vector4(0));


        }

        private List<Texture> LoadMaterialTextures(Assimp.Material material)
        {
            List<Texture> textures = new List<Texture>();
            int c = material.GetMaterialTextureCount(Assimp.TextureType.Diffuse);
            TextureSlot[] assimp_textures = material.GetAllMaterialTextures();

            float[] border_color = {0.0f, 0.0f, 0.0f, 0.0f};
            bool skip_loading = false;
            foreach(TextureSlot texture_slot in assimp_textures)
            {
                Texture loaded_texture = _textures_loaded.FirstOrDefault(t => t.Path == texture_slot.FilePath);

                if (loaded_texture is null)
                {
                    border_color =  new float[] {material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B, material.ColorDiffuse.A};

                    Texture texture = null;
                    switch (texture_slot.TextureType)
                    {
                        case Assimp.TextureType.Diffuse:
                            texture = new Texture(_loadingPath + texture_slot.FilePath, TextureType.Diffuse, texture_slot.WrapModeU, texture_slot.WrapModeV, border_color);
                            break;
                        case Assimp.TextureType.Specular:
                            texture = new Texture(_loadingPath + texture_slot.FilePath, TextureType.Specular, texture_slot.WrapModeU, texture_slot.WrapModeV, border_color);
                            break;
                    }

                    textures.Add(texture);
                    _textures_loaded.Add(texture);
                }
                else
                {
                textures.Add(loaded_texture);
                }
            }

            return textures;
        }
    }
}

