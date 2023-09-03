using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOpenGL.Physics
{
    public class CollisionData
    {
        public bool IsCollide { get; private set; }
        public float PenetrationDistance { get; private set; }

        public CollisionData(bool isCollide, float penetrationDistance)
        {
        IsCollide = isCollide;
        PenetrationDistance = penetrationDistance;
        }
    }
}
