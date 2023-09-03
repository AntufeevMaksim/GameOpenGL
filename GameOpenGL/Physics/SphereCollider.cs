using ObjLoader.Loader.Data.VertexData;
using OpenTK.Mathematics;


namespace GameOpenGL.Physics
{
    public class SphereCollider : ICollider
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public SphereCollider(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }



        public CollisionData CheckCollision(ICollider other)
        {
            return other.CheckCollision(this);
        }

        public CollisionData CheckCollision(SphereCollider other)
        {
            float center_distance = (Center - other.Center).Length;
            return new CollisionData(center_distance <= Radius + other.Radius, center_distance - Radius - other.Radius);
        }

        public CollisionData CheckCollision(PlaneCollider other)
        {
            float distance_to_center = Vector3.Dot(other.Normal, Center) - other.Distance;
            return new CollisionData(distance_to_center <= Radius, distance_to_center - Radius);
        }

        public void Move(Vector3 direction)
        {
            Center += direction;
        }
    }
}
