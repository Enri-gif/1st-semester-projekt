using System;

namespace blazor.models
{
    public class AssignmentSheetItem
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = "";
        public string Topic { get; set; } = "";
        public int Number { get; set; } = 1;
    }
}