using System.Text;

namespace Generator.Generators
{
    internal abstract class Generator
    {
        internal static string PathToSourceRoot(int pathLevel)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < pathLevel; i++)
            {
                builder.Append("../");
            }

            return builder.ToString();
        }
    }
}