using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices.ComTypes;

namespace ConverterProject
{
    internal class Program
    {
        internal static BinaryTestOld.ComplexClass LoadFromBinaryFile(string path)
        {
            //using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //{
            //    var formatter = new BinaryFormatter();
            //    var data = formatter.Deserialize(stream); ;
            //    return (BinaryTestOld.ComplexClass)data;
            //}

            // Deserialize the object with a custom binder
            var formatter = new BinaryFormatter();
            //formatter.Binder = new CustomBinder();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var deserializedObject = (BinaryTestOld.ComplexClass)formatter.Deserialize(stream);
                return deserializedObject;
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0) // TO OPEN THE CONSOLE DIRECTLY
            {
                //Console.WriteLine("Usage: CompatibilityUtility <command> <arguments>");
                //return;
                string[] arguments = { "convert", "D:\\test.addr", "D:\\sd.txt" };
                args = arguments;

            }
            string command = args[0];
            Console.WriteLine(command);

            try
            {
                switch (command.ToLower())
                {
                    case "convert":
                        if (args.Length != 3)
                        {
                            foreach (var item in args)
                            {
                                Console.WriteLine(item);
                            }
                            return;
                        }

                        string binaryFilePath = args[1];
                        string jsonFilePath = args[2];

                        try
                        {
                            var data = LoadFromBinaryFile(binaryFilePath);
                            //RobotConverter.SaveToJsonFile(jsonFilePath, robot);
                            Console.WriteLine("Conversion successful.");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            File.WriteAllText("D:\\excep.txt", ex.Message);
                            File.WriteAllText("D:\\excep.txt", ex.InnerException.ToString());
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case "anothercommand":
                        // Handle another command with different arguments
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {command}");
                        break;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("D:\\excep.txt", ex.Message);
                File.WriteAllText("D:\\excep.txt", ex.InnerException.ToString());
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public class CustomBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Customize the type binding logic here
            if (assemblyName.StartsWith("OldAssemblyName"))
            {
                assemblyName = "NewAssemblyName, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null";
            }
            return Type.GetType($"{typeName}, {assemblyName}");
        }
    }
}
