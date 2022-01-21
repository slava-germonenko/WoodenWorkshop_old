using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Crm.Api.Models.Http;

namespace WoodenWorkshop.Crm.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly JsonSerializerOptions _errorSerializationOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
    };

    private readonly RequestDelegate _next;


    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(HttpContext context, IWebHostEnvironment environment)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (BadHttpRequestException e)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, new BaseApiResponse(e.Message));
        }
        catch (DuplicateException e)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, new BaseApiResponse(e.Message));
        }
        catch (ValidationException e)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, new BaseApiResponse(e.Message));
        }
        catch (NotFoundException e)
        {
            await WriteResponse(context, HttpStatusCode.NotFound, new BaseApiResponse(e.Message));
        }
        catch (UnauthorizedException e)
        {
            await WriteResponse(context, HttpStatusCode.Unauthorized, new BaseApiResponse(e.Message));
        }
        catch (Exception e)
        {
            var apiResponse = environment.IsDevelopment()
                ? new DeveloperErrorApiResponse(e.Message, e.StackTrace)
                : new BaseApiResponse("Произошла непредвиденная ошибка! Пожалуйста, свяжитесь с администрацией.");
            await WriteResponse(context, HttpStatusCode.InternalServerError, apiResponse);
        }
    }

    private async Task WriteResponse(HttpContext context, HttpStatusCode code, object? body)
    {
        context.Response.StatusCode = (int)code;
        if (body is not null)
        {
            var serializedBodyBytes = JsonSerializer.SerializeToUtf8Bytes(body, _errorSerializationOptions);
            await context.Response.Body.WriteAsync(serializedBodyBytes);
        }
    }
}