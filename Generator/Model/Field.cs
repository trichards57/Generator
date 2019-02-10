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
        Select
    }

    internal class Field
    {
        public string Disabled { get; set; }
        public bool HideZero { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string Items { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public bool NeedsStyles => Type == FieldTypes.Date || Type == FieldTypes.Time || Type == FieldTypes.LongText;
        public bool Required { get; set; }
        public FieldTypes Type { get; set; }
        public bool UsesProcessedValue => HideZero;
    }
}