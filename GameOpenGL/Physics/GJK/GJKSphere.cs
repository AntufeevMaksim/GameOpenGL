
using OpenTK.Mathematics;


namespace GameOpenGL.Physics.GJK
{
    public class GJKSphere : GJKCollider
    {
        private float _radius;
        private Vector3 _position;

        public GJKSphere(float radius, Vector3 position)
        {
            _radius = radius;
            _position = position;
        }

        public override Vector3 SupportFunction(Vector3 direction)
        {
            return _position + _radius * direction;
        }

        public override void Move(Vector3 traveledPath)
        {
            _position += traveledPath; 
        }
    }
}
