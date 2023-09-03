
using OpenTK.Mathematics;

namespace GameOpenGL.Physics
{
    public class PlaneCollider : ICollider
    {
        public Vector3 Normal { get; private set; }
        public float Distance { get; private set; }

        public PlaneCollider(Vector3 normal, float distance)
        {
            Normal = normal;
            Distance = distance;
        }

        public CollisionData CheckCollision(ICollider other)
        {
            return other.CheckCollision(this);
        }

        public CollisionData CheckCollision(SphereCollider other)
        {
            float distance_to_center = Vector3.Dot(Normal, other.Center) - Distance;
            return new CollisionData(distance_to_center <= other.Radius, distance_to_center - other.Radius);
        }

        public CollisionData CheckCollision(PlaneCollider other)
        {
            bool is_parallel = Normal == other.Normal;
            return new CollisionData(!is_parallel, is_parallel ? other.Distance - Distance : 0.0f);
        }

        public void Move(Vector3 distanceTraveled)
        {
            Distance += Vector3.Dot(Normal, distanceTraveled);
        }
    }
}
