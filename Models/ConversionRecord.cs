using System;

namespace JsonZipToolWPF.Models
{
    public class ConversionRecord
    {
        public int Id { get; set; }
        public string OperationType { get; set; } = string.Empty; // Compress, Decompress, Format
        public string InputText { get; set; } = string.Empty;
        public string OutputText { get; set; } = string.Empty;
        public DateTime ConversionTime { get; set; }
    }
} 