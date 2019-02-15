using Generator.Generators;
using Generator.Model;
using Newtonsoft.Json;
using System.IO;

namespace Generator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputPath = @"C:\Users\trich\source\repos\planner-ui\generator.json";
            var serializer = new JsonSerializer();
            var reader = new JsonTextReader(new StreamReader(File.OpenRead(inputPath)));

            var model = serializer.Deserialize<MainModel>(reader);

            var inputFolder = Path.GetDirectoryName(inputPath);
            var srcPath = Path.Join(inputFolder, "src");

            foreach (var form in model.Forms)
            {
                var outputFolder = Path.Join(srcPath, form.Path, form.KebabName);

                var indexFile = Path.Join(outputFolder, "index.tsx");
                var testFile = Path.Join(outputFolder, $"{form.KebabName}.generated.test.tsx");
                var dataInterface = Path.Join(srcPath, @"store\interfaces", $"{form.KebabName}.ts");

                FormGenerator.WriteForm(indexFile, form);
                FormTestsGenerator.WriteFormTests(testFile, form);
                StoreGenerator.WriteStoreInterface(dataInterface, form);
            }
        }
    }
}