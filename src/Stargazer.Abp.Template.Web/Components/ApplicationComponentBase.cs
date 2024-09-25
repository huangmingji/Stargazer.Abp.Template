using Microsoft.AspNetCore.Components;

public abstract class ApplicationComponentBase : ComponentBase, IDisposable
{
    private CancellationTokenSource? cancellationTokenSource;
    protected CancellationToken CancellationToken => (cancellationTokenSource ??= new()).Token;
    public virtual void Dispose()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }
}