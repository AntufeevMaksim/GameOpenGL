
using OpenTK.Mathematics;

namespace GameOpenGL.Physics.GJK
{

    public abstract class GJKCollider
    {
        public abstract Vector3 SupportFunction(Vector3 direction);
        public abstract void Move(Vector3 traveledPath);

        public bool CheckCollision(GJKCollider other)
        {
            List<Vector3> simplex = new List<Vector3>();
            Vector3 direction = new Vector3(1.0f, 0.0f, 0.0f);

            simplex.Add(SupportFunction(direction) - other.SupportFunction(-direction));
            direction = Vector3.Normalize(-simplex[0]);
            simplex.Add(SupportFunction(direction) - other.SupportFunction(-direction));

            if (!SupportMath.PointPassTheOrigin(simplex[0], simplex[1])) return false;


            direction = SupportMath.DirFromLineToCenter(simplex[0], simplex[1]);
            if (double.IsNaN(direction.X))
            {
                return SupportMath.CenterInLineSegment(simplex[0], simplex[1]);

            }

            simplex.Add(SupportFunction(direction) - other.SupportFunction(-direction));


            if (!SupportMath.PointPassTheOrigin(simplex[1], simplex[2]) || simplex[0] == simplex[2])
            {
                return false;
            }


            while (true)
            {
                direction = SupportMath.DirFromPlaneToCenter(simplex[^1], simplex[^2], simplex[^3]);
                simplex.Add(SupportFunction(direction) - other.SupportFunction(-direction));

                // first condition means that center on plane: (simplex[^4], simplex[^3], simplex[^2])
                if ((float.IsNaN(direction.X)))
                {
                    return false; 
                }
                if (!SupportMath.PointPassTheOrigin(simplex[^2], simplex[^1]) || SupportMath.PointsOnOnePlane(simplex.GetRange(simplex.Count - 4, 4)))
                {
                    return false;
                }


                    if (SupportMath.CenterInPyramid(simplex.GetRange(simplex.Count - 4, 4)))
                {
                    return true;
                }
            }
    
        }

    }


}
