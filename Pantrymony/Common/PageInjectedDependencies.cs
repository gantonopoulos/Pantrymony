using Microsoft.JSInterop;

namespace Pantrymony.Common;

 

internal class PageInjectedDependencies
{
    public HttpClient? HttpClient { get;  set; }
    public IConfiguration? Configuration { get;  set; }
    public IJSRuntime? JScriptRuntime { get; set; }
    public ILogger? Logger { get;  set; }
}

