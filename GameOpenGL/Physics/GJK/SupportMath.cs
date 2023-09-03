
using OpenTK.Mathematics;


namespace GameOpenGL.Physics.GJK
{
    class SupportMath
    {
        public static bool PointPassTheOrigin(Vector3 point_a, Vector3 point_b)
        {
            return float.Sign(point_a.X) != float.Sign(point_b.X) || float.Sign(point_a.Y) != float.Sign(point_b.Y) || float.Sign(point_a.Z) != float.Sign(point_b.Z);
        }


        public static Vector3 DirFromLineToCenter(Vector3 point_a, Vector3 point_b)
        {

            Vector3 lineVector = point_b - point_a;
            float ah = (float)(Math.Pow(point_a.Length, 2) - Math.Pow(point_b.Length, 2) + Math.Pow(lineVector.Length, 2)) / (2*lineVector.Length); // distance from point a to point of intersection of a line and a perpendicular
            return Vector3.Normalize(new Vector3(-point_a - ah * Vector3.Normalize(lineVector)));
        }

        
        public static Vector3 DirFromPlaneToCenter(Vector3 point_a, Vector3 point_b, Vector3 point_c) 
        {

            return DirFromPlaneToPoint( point_a, point_b, point_c, Vector3.Zero);
        }

        public static Vector3 DirFromPlaneToPoint(Vector3 point_a, Vector3 point_b, Vector3 point_c, Vector3 point)
        {
            Vector3 direction = Vector3.Normalize(Vector3.Cross(point_b - point_a, point_c - point_a));

            if (Vector3.Dot(direction, point - point_a) > 0.0f) // check if vector directed to point
            {
                return direction;
            }
            else
            {
                return -direction;
            }
        }

        public static bool CenterInPyramid(List<Vector3> vertices)
        {
            Vector3 normal = DirFromPlaneToPoint(vertices[0], vertices[1], vertices[2], vertices[3]);
            if (Vector3.Dot(normal, - vertices[0]) < 0.0f) return false;

            normal = DirFromPlaneToPoint(vertices[0], vertices[3], vertices[1], vertices[2]);
            if (Vector3.Dot(normal, - vertices[0]) < 0.0f) return false;

            normal = DirFromPlaneToPoint(vertices[1], vertices[3], vertices[2], vertices[0]);
            if (Vector3.Dot(normal, - vertices[1]) < 0.0f) return false;

            normal = DirFromPlaneToPoint(vertices[0], vertices[3], vertices[2], vertices[1]);
            if (Vector3.Dot(normal,  - vertices[0]) < 0.0f) return false;

            return true;
        }

        public static bool CenterInLineSegment(Vector3 point_a, Vector3 point_b)
        {            
            return point_a.Length + point_b.Length == (point_a - point_b).Length;
        }

        private static bool IsCollinear(Vector3 a, Vector3 b)
        {
            return Vector3.Cross(a, b) == Vector3.Zero;
        }


        public static bool PointsOnOnePlane(List<Vector3> points)
        {
            if (points.Count < 4) return true;

            Vector3 plane_normal = Vector3.Cross(points[1] - points[0], points[2] - points[0]);

            for (int i = 3; i < points.Count; i++)
            {
                Vector3 normal = Vector3.Cross(points[1] - points[i], points[2] - points[i]);
                
                if (!IsCollinear(plane_normal, normal) ) return false;
            }

            return true;
        }
    }
}