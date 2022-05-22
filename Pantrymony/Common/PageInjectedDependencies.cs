using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Pantrymony.Common;

 

internal class PageInjectedDependencies
{
    [Inject]
    public HttpClient? HttpClient { get; private set; }
    [Inject]
    public IConfiguration? Configuration { get; private set; }
    [Inject]
    public IJSRuntime? JsRuntime { get; private set; }
    [Inject]
    public ILogger? Logger { get; private set; }
}

