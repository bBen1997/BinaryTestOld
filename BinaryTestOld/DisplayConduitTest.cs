using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using System.Threading.Tasks;
using System.Threading;
using Rhino.Display;
using Rhino.Geometry;
using System.Diagnostics;

namespace BinaryTestOld
{
    public class DisplayConduitTest : DisplayConduit
    {
        public List<Brep> Geometries = new List<Brep>();
        public Brep BrepGeo { get; set; }
        public RotationalTransformationGenerator Generator { get; set; }
        public DisplayMaterial Material { get; set; } = new DisplayMaterial();
        public BoundingBox Box { get; internal set; }
        public List<Mesh> MeshGeometries { get; internal set; }

        private int frameCount;
        private Stopwatch stopwatch;
        private double fps;

        public DisplayConduitTest()
        {
            frameCount = 0;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            //var bb = BoundingBox.Empty;
            //for (int i = 0; i < Geometries.Count; i++)
            //{
            //    bb.Union(Geometries[i].GetBoundingBox(true));
            //};
            //bb.Inflate(10);
            e.IncludeBoundingBox(Box);
            base.CalculateBoundingBox(e);
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            Generator.transformations.TryDequeue(out var tr);

            e.Display.PushModelTransform(tr);
            for (int i = 0; i < Geometries.Count; i++)
            {
                e.Display.DrawBrepShaded(Geometries[i], material: Material);
                //e.Display.DrawMeshShaded(MeshGeometries[i], Material);

            }

            e.Display.PopModelTransform();

            frameCount++;
            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                fps = frameCount / (stopwatch.ElapsedMilliseconds / 1000.0);
                frameCount = 0;
                stopwatch.Restart();
            }

            e.Display.Draw2dText($"FPS: {fps:F2}", System.Drawing.Color.Black, new Point2d(30, 50), false, 30);

            base.PostDrawObjects(e);
        }
    }


    public class RotationalTransformationGenerator
    {
        public ConcurrentQueue<Transform> transformations;
        private CancellationTokenSource cancellationTokenSource;
        private Task rotationTask;

        public RotationalTransformationGenerator()
        {
            transformations = new ConcurrentQueue<Transform>();
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            rotationTask = Task.Run(() => GenerateTransformations(cancellationTokenSource.Token));
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            rotationTask.Wait();
        }
        public static List<Mesh> CreateMeshListFromBrepsParallel(List<Brep> breps)
        {
            ConcurrentBag<Mesh> meshes = new ConcurrentBag<Mesh>();

            Parallel.ForEach(breps, brep =>
            {
                var meshArray = Mesh.CreateFromBrep(brep, MeshingParameters.FastRenderMesh);
                if (meshArray != null && meshArray.Length > 0)
                {
                    meshes.Add(meshArray[0]);
                }
            });

            return new List<Mesh>(meshes);
        }
        private void GenerateTransformations(CancellationToken token)
        {
            double angle = 0;
            var f = new Stopwatch();
            //f.Start();
            while (!token.IsCancellationRequested)
            {

                Transform rotation = Transform.Rotation(RhinoMath.ToRadians(angle), new Vector3d(0, 1, 0), Point3d.Origin);
                transformations.Enqueue(rotation);

                angle += 1; // Increment by 1 degree
                if (angle >= 360)
                    angle = 0;

                //Thread.Sleep(20); // Adjust as needed for smoother or faster rotation
                //RhinoApp.WriteLine(f.Elapsed.Milliseconds.ToString());

                RhinoDoc.ActiveDoc.Views.Redraw();

                // f.Restart();

            }
        }

        public bool TryGetNextTransformation(out Transform transform)
        {
            return transformations.TryDequeue(out transform);
        }

        public static List<Brep> CreateSphereArray(int numberOfSpheres, double radius, double spacing)
        {
            List<Brep> spheres = new List<Brep>();

            for (int i = 0; i < numberOfSpheres; i++)
            {
                double x = i * spacing;

                // Create the center point for the sphere
                Point3d centerPoint = new Point3d(x, 0, 0);

                // Create the sphere
                Sphere sphere = new Sphere(centerPoint, radius);

                // Convert the sphere to a Brep
                Brep brep = sphere.ToBrep();

                // Add the brep to the list
                spheres.Add(brep);
            }

            return spheres;
        }

        public static List<Brep> CreateSphereBlock(int rows, int columns, double radius, double spacing)
        {
            List<Brep> spheres = new List<Brep>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    double x = j * spacing;
                    double y = i * spacing;
                    double z = 0;

                    // Create the center point for the sphere
                    Point3d centerPoint = new Point3d(x, y, z);

                    // Create the sphere
                    Sphere sphere = new Sphere(centerPoint, radius);

                    // Convert the sphere to a Brep
                    Brep brep = sphere.ToBrep();

                    // Add the brep to the list
                    spheres.Add(brep);
                }
            }

            return spheres;
        }

        public static List<Brep> CreateSphereGrid(int rows, int columns, int layers, double radius, double spacing)
        {
            List<Brep> spheres = new List<Brep>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    for (int k = 0; k < layers; k++)
                    {
                        double x = j * spacing;
                        double y = i * spacing;
                        double z = k * spacing;

                        // Create the center point for the sphere
                        Point3d centerPoint = new Point3d(x, y, z);

                        // Create the sphere
                        Sphere sphere = new Sphere(centerPoint, radius);

                        // Convert the sphere to a Brep
                        Brep brep = sphere.ToBrep();

                        // Add the brep to the list
                        spheres.Add(brep);
                    }
                }
            }

            return spheres;
        }

        public static List<Brep> CreateSphereGridParallel(int rows, int columns, int layers, double radius, double spacing)
        {
            ConcurrentBag<Brep> spheres = new ConcurrentBag<Brep>();

            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < columns; j++)
                {
                    for (int k = 0; k < layers; k++)
                    {
                        double x = j * spacing;
                        double y = i * spacing;
                        double z = k * spacing;

                        // Create the center point for the sphere
                        Point3d centerPoint = new Point3d(x, y, z);

                        // Create the sphere
                        Sphere sphere = new Sphere(centerPoint, radius);

                        // Convert the sphere to a Brep
                        Brep brep = sphere.ToBrep();

                        // Add the brep to the ConcurrentBag
                        spheres.Add(brep);
                    }
                }
            });

            return new List<Brep>(spheres);
        }
    }

}
