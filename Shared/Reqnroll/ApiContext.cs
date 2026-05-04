using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Reqnroll;

public class ApiContext
{
    public HttpClient Client { get; set; } = default!;
    public HttpResponseMessage Response { get; set; } = default!;
}
