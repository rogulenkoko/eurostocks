using System;
using EuroStock.Domain.Services.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EuroStocks;

public static class ImagesEndpoint
{
    public static void RegisterImagesEndpoint(this IEndpointRouteBuilder routes)
    {
        var routeBuilder = routes.MapGroup("/file");

        routeBuilder.MapPost("", async (
            IFormFile file,
            HttpContext context,
            [FromServices]IStorageService storageService) =>
        {
            var userId = "user_id";

            await using var stream = file.OpenReadStream();
            Guid? fileId = await storageService.UploadFileAsync(userId, stream, context.RequestAborted);
            return Results.Ok(fileId);
        }).DisableAntiforgery();
        
        routeBuilder.MapGet("{id}", async (
            HttpContext context,
            [FromServices]IStorageService storageService,
            [FromQuery]Guid id) =>
        {
            var userId = "user_id";

            var file = await storageService.GetFileAsync(userId, id, context.RequestAborted);
            return Results.File(file, "image/jpeg");
        }).DisableAntiforgery();
    }
}