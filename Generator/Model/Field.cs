using System;

namespace Generator.Model
{
    internal enum FieldTypes
    {
        Bool,
        Tel,
        Text,
        Number,
        Date,
        Time,
        LongText,
        Select,
        Email
    }

    internal class Field
    {
        public object ActualType
        {
            get
            {
                switch (Type)
                {
                    case FieldTypes.Bool:
                        return "boolean";

                    case FieldTypes.Date:
                    case FieldTypes.Email:
                    case FieldTypes.LongText:
                    case FieldTypes.Tel:
                    case FieldTypes.Text:
                    case FieldTypes.Time:
                        return "string";

                    case FieldTypes.Number:
                    case FieldTypes.Select:
                        return "number";

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public string Disabled { get; set; }
        public bool HideZero { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string Items { get; set; }
        public string ItemText { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public bool NeedsStyles => Type == FieldTypes.Date || Type == FieldTypes.Time || Type == FieldTypes.LongText;
        public bool Required { get; set; }
        public string StoreField => StoreParam ?? Name;
        public string StoreParam { get; set; }
        public string TbdText { get; set; }
        public FieldTypes Type { get; set; }
        public bool UsesProcessedValue => HideZero || WithTBD;
        public bool WithTBD { get; set; }
    }
}