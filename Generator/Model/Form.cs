using Soltys.ChangeCase;
using System.Collections.Generic;
using System.Linq;

namespace Generator.Model
{
    internal class Form
    {
        private bool _customStoreGenerator;

        public string AddTitle { get; set; }

        public bool CustomStoreGenerator
        {
            get => _customStoreGenerator || !string.IsNullOrWhiteSpace(StoreType);
            set => _customStoreGenerator = value;
        }

        public string[] ExtraImports { get; set; }
        public IEnumerable<Field> Fields { get; set; }

        public string FormName => $"{Name.PascalCase()}Form";

        public string HumanName => $"{Name.TitleCase()} Form";

        public string KebabName
        {
            get
            {
                return Name.ParamCase();
            }
        }

        public string Name { get; set; }

        public bool NeedsStyles => Fields.Any(f => f.NeedsStyles);

        public string Path { get; set; }

        public int PathLevel
        {
            get
            {
                var parts = Path.Split("/");
                return parts.Length + 1;
            }
        }

        public string PropsInterface => $"I{Name.PascalCase()}Props";

        public string Store => $"{Name.LowerCaseFirst()}Store";

        public string StoreData => $"I{Name}Data";
        public string StoreType { get; set; }
        public string Title { get; set; }
        public string UpdateTitle { get; set; }
        public bool UsesUpdateMode => string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(AddTitle) && !string.IsNullOrWhiteSpace(UpdateTitle);
    }
}