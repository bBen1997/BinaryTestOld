using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryTestOld
{
    [Serializable]
    public class ComplexClass
    {
        public Mesh MeshGeo { get; set; } //= new Mesh();
        public Brep BrepGeo { get; set; } = new Sphere(Point3d.Origin, 10).ToBrep();

        public Plane PlaneGeo { get; set; } = new Plane(new Point3d(10,10,5), new Vector3d(5,5,5));

        public RobotHand Hand { get; set; } = new RobotHand();
    }

    [Serializable]
    public class RobotHand
    {
        public string HandName { get; set; }
    }
}
