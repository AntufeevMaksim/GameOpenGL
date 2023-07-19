using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

using Assimp;
using System.IO;

namespace Model
{
    public class Model
    {
        private List<Mesh> _meshes = new List<Mesh>();
        private Scene _model;
        private List<Texture> _textures_loaded = new List<Texture>();
        private string _loadingPath;
        public Model(string loadingPath, string fileName)
        {
            _loadingPath = loadingPath;
            AssimpContext importer = new AssimpContext();
            _model = importer.ImportFile(loadingPath + fileName, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
            ProcessNode(_model.RootNode);
        }
        public void Draw(Shader shader, int _EBO, int _VAO, int _VBO)
        {
            foreach (var mesh in _meshes)
            {
                mesh.Draw(shader, _EBO, _VAO, _VBO);
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
                vector3.Y = mesh.Vertices[i].Y;
                vector3.Z = mesh.Vertices[i].Z;
                vertex.position = vector3;

                vector3.X = mesh.Normals[i].X;
                vector3.Y = mesh.Normals[i].Y;
                vector3.Z = mesh.Normals[i].Z;
                vertex.normal = vector3;
                vertices.Add(vertex);

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
            }

            indices = new List<uint>(mesh.GetUnsignedIndices());

            if (mesh.MaterialIndex >= 0)    //if mesh has material
            {
                Material material = _model.Materials[mesh.MaterialIndex];

                textures.AddRange(LoadMaterialTextures(material));
                //textures.AddRange(LoadMaterialTextures(material));
            }

            return new Mesh(vertices, indices, textures);

        }

        private List<Texture> LoadMaterialTextures(Material material)
        {
            List<Texture> textures = new List<Texture>();

            TextureSlot[] assimp_textures = material.GetAllMaterialTextures();

            bool skip_loading = false;
            foreach(TextureSlot texture_slot in assimp_textures)
            {
                Texture loaded_texture = _textures_loaded.FirstOrDefault(t => t.Path == texture_slot.FilePath);

                if (loaded_texture is null)
                {

                    Texture texture = null;
                    switch (texture_slot.TextureType)
                    {
                        case Assimp.TextureType.Diffuse:
                            texture = new Texture(_loadingPath + texture_slot.FilePath, TextureType.Diffuse);
                            break;
                        case Assimp.TextureType.Specular:
                            texture = new Texture(_loadingPath + texture_slot.FilePath, TextureType.Specular);
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

