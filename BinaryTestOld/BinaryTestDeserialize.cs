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

namespace BinaryTestOld
{
    public class BinaryTestDeserialize : Command
    {
        public BinaryTestDeserialize()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static BinaryTestDeserialize Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "BinaryTestDeserialize";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //DisplayConduitTest.Geometries = new List<GeometryBase>();
            //ConvertBinaryToJson("D:\\test.addr", "D:\\sd.txt"); // CHANGE FILE NAMES AS REQD.
            //DisplayConduitTest.Geometries.Add(new Sphere(Point3d.Origin, 50).ToBrep());
            var cond = BinaryTestOldPlugin.Instance.PluginConduit;

            if (cond.Enabled)
            {
                cond.Enabled = false;
                cond.Generator.Stop();
            }
            else
            {
                foreach (var item in cond.Geometries)
                {
                    RhinoDoc.ActiveDoc.Objects.AddBrep(item);
                }
            }

            return Result.Success;

        }

        private static void ConvertBinaryToJson(string binaryFilePath, string jsonFilePath)
        {
            string[] arguments = { "convert", binaryFilePath, jsonFilePath };
            StartProcess(arguments);
        }

        private static void StartProcess(params string[] arguments)
        {
            try
            {
                string argsString = string.Join(" ", arguments);
                ProcessStartInfo startInfo = new ProcessStartInfo // CHANGE PATH TO EXECUTABLE
                {
                    FileName = "C:\\Users\\bovas\\source\\repos\\BinaryTestOld\\ConverterProject\\bin\\Debug\\ConverterProject.exe",
                    Arguments = argsString,
                    //RedirectStandardOutput = true,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Console.WriteLine("Process completed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Process failed.");
                        string error = process.StandardOutput.ReadToEnd();
                        Console.WriteLine(error);
                    }
                }
                Console.WriteLine("Press any key to exit...");
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}
