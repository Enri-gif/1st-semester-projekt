using System;
using System.Collections.Generic;

namespace blazor.models
{
    public class CreateAssignmentSheetDto
    {
        public List<Guid> TaskIds { get; set; } = new List<Guid>();
        public string Type { get; set; } = ""; // "prøve" eller "lektier"
    }
}