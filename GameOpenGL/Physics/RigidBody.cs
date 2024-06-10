

using OpenTK.Mathematics;
using GameOpenGL.Physics.GJK;

namespace GameOpenGL.Physics
{
    /// <summary>
    /// physical and graphical representation of object
    /// </summary>
    public class RigidBody
    {
        Model.Model _model;
        Vector3 _position;

        Vector3 _direction;
        float _speed;

        Vector3 _rotationAxis = Vector3.One;
        float _rotationAngle = 0.0f;
        float _scale;
        float _elasticity;

        public GJKCollider Collider { get ; private set ; }
        public Vector3 Direction { get { return _direction; } set { _direction = value.Normalized(); }  }
        public float SpeedKmPerHour { get { return _speed * 3.6f; } set { _speed = value / 3.6f; } } // convert to m/s
        public float Speed { get => _speed; set => _speed = value; }

        public float Mass { get; set; }

        /// <summary>
        /// valid values ​​are from 0 to 1
        /// </summary>
        public float Elasticity { get => _elasticity; set { _elasticity = Math.Abs(value) > 1.0f ?  1.0f : Math.Abs(value); } }
        public RigidBody(Model.Model model, GJKCollider collider, Vector3 position, float scale = 1.0f)
        {
            _scale = scale;
            _model = model;
            Collider = collider;

            _position = position;
            Collider.Move(position);
        }

        public void Draw()
        {

            Matrix4 model = Matrix4.CreateScale(_scale);
            model *= Matrix4.CreateTranslation(_position);
            model *= Matrix4.CreateFromAxisAngle(_rotationAxis, _rotationAngle);


            _model.Draw(model);

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

                UpdateDirAndSpeedAfterCollision(other.Mass, other.Speed, other.Direction, other.Elasticity);
                other.UpdateDirAndSpeedAfterCollision(Mass, old_speed, old_direction, other.Elasticity);
            }
        }

        public void UpdateDirAndSpeedAfterCollision(float otherMass, float otherSpeed, Vector3 otherDirection, float otherElasticity)
        {
            Vector3 directed_speed;
            float collision_elasticity = otherElasticity * Elasticity;

            directed_speed = (_speed * Direction) - (1 + collision_elasticity) * (otherMass / (Mass + otherMass)) * ((_speed * Direction) - (otherSpeed * otherDirection));

            _speed = directed_speed.Length;
            Direction = Vector3.Normalize(directed_speed);
        }
    }
}
