
using OpenTK.Mathematics;

namespace GameOpenGL.Physics.GJK
{
    public class GJKPolyhedron : GJKCollider
    {
        List<Vector3> _vertieces = new List<Vector3>();

        public GJKPolyhedron(List<Vector3> vertieces)
        {
            _vertieces = vertieces.ConvertAll(v => new Vector3(v)); // deep copy
        }

        public override Vector3 SupportFunction(Vector3 direction)
        {
            Vector3 vertex = new Vector3(float.NaN, float.NaN, float.NaN);
            float last_dot_product = float.NegativeInfinity;

            foreach (Vector3 v in _vertieces)
            {
                float dot_product = Vector3.Dot(v, direction);
                if (dot_product > last_dot_product)
                {
                    last_dot_product = dot_product;
                    vertex = v;
                }
            }
            return vertex;
        }

        public override void Move(Vector3 traveledPath)
        {
            for (int i = 0;  i < _vertieces.Count; i++)
            {
                _vertieces[i] += traveledPath;
            }
        }
    }
}
