

using OpenTK.Mathematics;
using GameOpenGL.Physics.GJK;

namespace GameOpenGL.Physics
{
    public class RigidBody
    {
        Model.Model _model;
        Vector3 _position;

        Vector3 _direction;
        float _speed;

        Vector3 _rotationAxis = Vector3.One;
        float _rotationAngle = 0.0f;
        float _scale;
        Shader _shader;

        public GJKCollider Collider { get ; private set ; }
        public Vector3 Direction { get { return _direction; } set { _direction = value.Normalized(); }  }
        public float SpeedKmPerHour { get { return _speed * 3.6f; } set { _speed = value / 3.6f; } } // convert to m/s
        public float Speed { get => _speed; set => _speed = value; }

        public float Mass { get; set; }

        public RigidBody(Model.Model model, GJKCollider collider, Shader shader, Vector3 position, float scale = 1.0f)
        {
            _shader = shader;
            _scale = scale;
            _model = model;
            Collider = collider;

            _position = position;
            Collider.Move(position);
        }

        public void Draw()
        {
            _shader.Use();

            Matrix4 model = Matrix4.CreateScale(_scale);
            model *= Matrix4.CreateTranslation(_position);
            model *= Matrix4.CreateFromAxisAngle(_rotationAxis, _rotationAngle);

            _shader.SetMat4("model", model);

            _model.Draw(_shader);

        }

        public bool CheckCollision(RigidBody other)
        {
            return Collider.CheckCollision(other.Collider);
        }

        public void UpdatePos(float deltaTime)
        {
            Vector3 distance_traveled = _direction * _speed * deltaTime * 0.001f; //convert milliseconds to seconds
            _position += distance_traveled;
            Collider.Move(distance_traveled);
            
        }

        public void CollisionResponse(RigidBody other)
        {
            if (Collider.CheckCollision(other.Collider))
            {
                float old_speed = Speed;
                Vector3 old_direction = Direction;

                UpdateDirAndSpeedAfterCollision(other.Mass, other.Speed, other.Direction);
                other.UpdateDirAndSpeedAfterCollision(Mass, old_speed, old_direction);
            }
        }

        public void UpdateDirAndSpeedAfterCollision(float otherMass, float otherSpeed, Vector3 otherDirection)
        {
            Vector3 pulse;

            pulse.X = ((2 * otherMass) / (Mass + otherMass)) * otherSpeed * otherDirection.X + ((Mass - otherMass) / (Mass + otherMass)) * Speed * Direction.X;
            pulse.Y = ((2 * otherMass) / (Mass + otherMass)) * otherSpeed * otherDirection.Y + ((Mass - otherMass) / (Mass + otherMass)) * Speed * Direction.Y;
            pulse.Z = ((2 * otherMass) / (Mass + otherMass)) * otherSpeed * otherDirection.Z + ((Mass - otherMass) / (Mass + otherMass)) * Speed * Direction.Z;

            _speed = pulse.Length;
            Direction = Vector3.Normalize(pulse);
        }
    }
}
