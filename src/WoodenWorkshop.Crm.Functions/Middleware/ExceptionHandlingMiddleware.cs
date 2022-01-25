using System;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace WoodenWorkshop.Crm.Functions.Middleware;

public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (AggregateException e)
        {
            await HandleExceptionAsync(e.InnerException ?? e);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(e);
        }
    }

    private Task HandleExceptionAsync(Exception e)
    {
        // Add logging here.
        return Task.CompletedTask;
    }
}