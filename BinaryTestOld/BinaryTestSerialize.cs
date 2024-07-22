using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace BinaryTestOld
{
    public class BinaryTestSerialize : Command
    {
        #region BoilerPlate
        public BinaryTestSerialize()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static BinaryTestSerialize Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "BinaryTestSerialize";

        #endregion


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //DisplayConduitTest.Geometries = new List<GeometryBase>();
            //Task.Run(() => DisplayConduitTest.Geometries.Add(new Sphere(new Point3d(100,0,0), 30).ToBrep()));
            var brs = RotationalTransformationGenerator.CreateSphereGridParallel(10, 10, 10, 10, 20);
            //var ms = RotationalTransformationGenerator.CreateMeshListFromBrepsParallel(brs);

            BoundingBox bb = BoundingBox.Empty;
            foreach (var b in brs)
            {
                bb.Union(b.GetBoundingBox(false));
            }
            bb.Inflate(15);
            var trGen = new RotationalTransformationGenerator();
            trGen.Start();
            var cond = BinaryTestOldPlugin.Instance.PluginConduit;
            cond.Geometries = brs;
            //cond.MeshGeometries = ms;
            cond.Box = bb;
            cond.Generator = trGen;
            cond.Enabled = true;
            return Result.Success;

        }
    }
}
