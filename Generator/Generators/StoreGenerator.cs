using Generator.Model;
using System.IO;
using System.Linq;

namespace Generator.Generators
{
    internal class StoreGenerator
    {
        public static void WriteStoreInterface(string path, Form form)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var file = new StreamWriter(path))
            {
                file.WriteLine($"export interface {form.StoreData} {{");

                foreach (var field in form.Fields.OrderBy(f => f.Name))
                {
                    file.WriteLine($"  {field.Name}: {field.ActualType};");
                }

                file.WriteLine("}");
            }
        }
    }
}