

using OpenTK.Mathematics;

namespace GameOpenGL.Physics
{
    public interface ICollider
    {
        public CollisionData CheckCollision(ICollider other);
        public CollisionData CheckCollision(SphereCollider other);
        public CollisionData CheckCollision(PlaneCollider other);

        public void Move(Vector3 distanceTraveled);
    }
}
