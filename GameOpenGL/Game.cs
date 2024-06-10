
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

using ObjLoader;

using Input;
using Model;

using System.Diagnostics;
using ObjLoader.Loader.Loaders;
using System.Reflection;
using GameOpenGL.Physics;
using GameOpenGL.Physics.GJK;
using System.Collections.Generic;

public class Game : GameWindow
{
    int _VBO;
    int _VAO;
    int _lightVAO;
    int _EBO;

    Shader _shader;
    Shader _lampShader;

    Vector3 _lightPos = (0.0f, 0.0f, 0.0f);

    Texture _textureDiffuse;
    Texture _textureSpecular;
    Texture _textureEmission;
    float _mixCoefficient;

    IObjLoader _objLoader;


    InputCallback _input;


    float _lastFrame = 0.0f;
    float _deltaTime = 0.0f;

    Camera _camera;
    Model.Model model1;
    RigidBody _car;
    RigidBody _cube;
    RigidBody _cube2;
    RigidBody _plane;
    Stopwatch _timer = new Stopwatch();

    List<Vector3> _cubePositions = new List<Vector3>{
    new Vector3( 0.0f,  10.0f,  0.0f),
    new Vector3( 2.0f,  5.0f, -15.0f),
    new Vector3(-1.5f, -2.2f, -2.5f),
    new Vector3(-3.8f, -2.0f, -12.3f),
    new Vector3( 2.4f, -0.4f, -3.5f),
    new Vector3(-1.7f,  3.0f, -7.5f),
    new Vector3( 1.3f, -2.0f, -2.5f),
    new Vector3( 1.5f,  2.0f, -2.5f),
    new Vector3( 1.5f,  0.2f, -1.5f),
    new Vector3(-1.3f,  1.0f, -1.5f),
    new Vector3(2.0f,  1.0f, 10.5f)
    };

    List<Vector3> _pointLightPositions = new List<Vector3>{

    new Vector3( 0.7f,  0.2f,  2.0f),
    new Vector3( 2.3f, -3.3f, -4.0f),
    new Vector3(-4.0f,  2.0f, -12.0f),
    new Vector3( 0.0f,  0.0f, -3.0f)
  };


    uint[] indices = {
    0, 1, 3, // First Triangle
    1, 2, 3  // Second Triangle
    };

    public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        KeyboardState input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        if (input.IsKeyPressed(Keys.Up) & _mixCoefficient < 1)
        {
            _mixCoefficient += 0.1f;
        }
        else if (input.IsKeyPressed(Keys.Down) & _mixCoefficient > 0)
        {
            _mixCoefficient -= 0.1f;
        }
    }
    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.0f, 0.01f, 0.1f, 1.0f);


        


        _shader = new Shader("C:\\Users\\antuf\\source\\repos\\c#\\GameOpenGL\\shader.vert", "C:\\Users\\antuf\\source\\repos\\c#\\GameOpenGL\\shader.frag");

        _lampShader = new Shader("C:\\Users\\antuf\\source\\repos\\c#\\GameOpenGL\\shader.vert", "C:\\Users\\antuf\\source\\repos\\c#\\GameOpenGL\\lamp_shader.frag");

        //_textureDiffuse = new Texture("C:\\Users\\antuf\\source\\resources\\container2.png",);
        //_textureSpecular = new Texture("C:\\Users\\antuf\\source\\resources\\container2_specular.png");
        //_textureEmission = new Texture("C:\\Users\\antuf\\source\\resources\\matrix.jpg");

        _input = new InputCallback(KeyboardState, MouseState);
        _camera = new Camera(new Vector3(0.0f, 0.0f, -3.0f), new Vector3(2.0f, 1.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
        CursorState = CursorState.Grabbed;
        _timer.Start();

        //model1 = new Model.Model("D:\\Models\\survival-guitar-backpack\\source\\", "Survival_BackPack_2.fbx");
        //model1 = new Model.Model("D:\\Models\\Nissan Skyline\\", "r32.obj");
        model1 = new Model.Model("D:\\Models\\Viper\\source\\", "Super car Viper.fbx", _shader);
        Model.Model model2 = new Model.Model("D:\\Models\\", "cube.fbx", _shader);
        Model.Model model3 = new Model.Model("D:\\Models\\", "plane.fbx", _shader);

        List<Vector3> collider_vertieces = new List<Vector3> { new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f)};


        //List<Vector3>  collider_vertieces = new List<Vector3> { new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.5f) };
        //List<Vector3> collider_vertieces2 = new List<Vector3> { new Vector3(-1.3f, 0.2f, 0.0f), new Vector3(-0.4f, -0.7f, 0.0f), new Vector3(0.31f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.8f) };

        _car = new RigidBody(model1, new GJKPolyhedron( collider_vertieces), Vector3.Zero, 0.00001f);
        _cube = new RigidBody(model2, new GJKPolyhedron(collider_vertieces), new Vector3(0.0f, 10.0f, 0.0f));
        _cube.Mass = 1.0f;
        _cube2 = new RigidBody(model2, new GJKPolyhedron(collider_vertieces), new Vector3(0.0f, 0.0f, -10.0f));
        _cube2.Mass = 1.0f;

        _cube.Direction = new Vector3(0.0f, -1.0f, 0.0f);
        _cube.Speed = 1.0f;
        _cube.Elasticity = 0.5f;
        _cube2.Elasticity = 0.5f;
        _cube2.Direction = new Vector3(0.0f, 0.0f, 1.0f);
        _cube2.Speed = 1.0f;
        GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _shader.Dispose();
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        System.Threading.Thread.Sleep(20);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);
        GL.ClearColor(new Color4(0.17f, 0.34f, 0.49f, 0.0f));


        float current_frame = _timer.ElapsedMilliseconds;
        _deltaTime = current_frame - _lastFrame;
        _lastFrame = current_frame;
        InputData input_data = _input.GetInputData();

        //camera
        _camera.Update(input_data, _deltaTime);
        Matrix4 view = _camera.GetLookAt();
        Matrix4 projection = _camera.GetProjection();

        _shader.Use();
        _shader.SetMat4("projection", projection);
        _shader.SetMat4("view", view);


        Vector3 camera_pos = _camera.Pos;
        _shader.SetVec3("viewPos", camera_pos);
        //cube

        //_textureDiffuse.Use(TextureUnit.Texture0);
        //_shader.SetInt("material.diffuse", 0);
        //_textureSpecular.Use(TextureUnit.Texture1);
        //_shader.SetInt("material.specular", 1);
        //_textureEmission.Use(TextureUnit.Texture2);
        //_shader.SetInt("material.emission", 2);

        //_shader.Use();
        //_shader.SetMat4("projection", projection);
        //_shader.SetMat4("view", view);

        //_shader.SetVec3("viewPos", camera_pos);

        //Matrix4 model = Matrix4.Identity;
        //model *= Matrix4.CreateScale(0.00001f);
        ////        model *= Matrix4.CreateTranslation(_cubePositions[0]);
        //model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(270));
        //_shader.SetMat4("model", model);

        _shader.SetFloat("material.shininess", 100.0f);

        float x_pos = (float)Math.Sin(MathHelper.DegreesToRadians(15 * _timer.Elapsed.TotalSeconds));
        float y_pos = (float)Math.Cos(MathHelper.DegreesToRadians(15 * _timer.Elapsed.TotalSeconds));


        for (int i = 0; i < _pointLightPositions.Count; i++)
        {

            _shader.SetVec3(String.Format("pointLights[{0}].ambient", i), new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetVec3(String.Format("pointLights[{0}].diffuse", i), new Vector3(0.9f, 1.0f, 1.0f));
            _shader.SetVec3(String.Format("pointLights[{0}].specular", i), new Vector3(0.9f, 1.0f, 1.0f));
            _shader.SetFloat(String.Format("pointLights[{0}].constant", i), 1.0f);
            _shader.SetFloat(String.Format("pointLights[{0}].linear", i), 0.09f);
            _shader.SetFloat(String.Format("pointLights[{0}].quadratic", i), 0.032f);
            _shader.SetVec3(String.Format("pointLights[{0}].position", i), _pointLightPositions[i]);
        }
        _shader.SetVec3("dirLight.direction", new Vector3(1.0f, 0.0f, 0.5f));
        _shader.SetVec3("dirLight.diffuse", new Vector3(1.0f, 0.0f, 0.0f));

        _shader.SetVec3("spotlight.position", _camera.Pos);
        _shader.SetVec3("spotlight.direction", _camera.Direction);
        _shader.SetFloat("spotlight.inner_corner", (float)Math.Cos(MathHelper.DegreesToRadians(2.5f)));
        _shader.SetFloat("spotlight.outer_corner", (float)Math.Cos(MathHelper.DegreesToRadians(5.5f)));
        _shader.SetVec3("spotlight.ambient", new Vector3(0.1f, 0.1f, 0.1f));
        _shader.SetVec3("spotlight.diffuse", new Vector3(1.0f, 1.0f, 1.0f)); // darken the light a bit to fit the scene
        _shader.SetVec3("spotlight.specular", new Vector3(1.0f, 1.0f, 1.0f));

        GL.BindVertexArray(_VAO);


        //model1.Draw(_shader);
        _car.Draw();
        _cube2.UpdatePos(_deltaTime);
        _cube.UpdatePos(_deltaTime);

        _cube2.CollisionResponse(_cube);

        _cube2.Draw();
        _cube.Draw();

        Matrix4 model = Matrix4.Zero;
        for (int i = 0; i < _cubePositions.Count; i++)
        {
//            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(_cubePositions[i]);
            // We then calculate the angle and rotate the model around an axis
            float angle = 20.0f * i;
            model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
            // Remember to set the model at last so it can be used by opentk
            _shader.SetMat4("model", model);

            // At last we draw all our cubes
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(_VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            //GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }




        //lamp

        _lampShader.Use();
        _lampShader.SetMat4("projection", projection);
        _lampShader.SetMat4("view", view);

        foreach (Vector3 pos in _pointLightPositions)
        {
            Matrix4 lamp_model = Matrix4.CreateScale(0.2f);
            lamp_model *= Matrix4.CreateTranslation(pos);
            _lampShader.SetMat4("model", lamp_model);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        Console.WriteLine(_cube.CheckCollision(_cube2));

        Context.SwapBuffers();

        base.OnRenderFrame(e);

    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, (e.Height - e.Width) / 2, e.Width, e.Width);
    }



}