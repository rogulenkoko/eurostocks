using System;
using System.Collections.Generic;
using EuroStock.Domain.Models;
using EuroStock.Domain.Services.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EuroStocks;

public static class ProductApis
{
    public static Guid DefaultMerchant = new Guid("46570079-BEBC-4D54-BAE9-7A0EC41A9111");
    public static Guid DefaultUser = new Guid("DE35A902-1AEA-4101-B824-3462C5C4582F");
    
    public static void RegisterProductEndpoint(this IEndpointRouteBuilder routes)
    {
        var routeBuilder = routes.MapGroup("/product").DisableAntiforgery();

        routeBuilder.MapPost("", async (
            [FromBody]UpdateProductRequest request,
            [FromServices]IProductService productService) =>
        {
            try
            {
                request.MerchantId = DefaultMerchant;
                request.UserId = DefaultUser;
                var productId = await productService.CreateOrUpdateProduct(request);
                return Results.Ok(productId);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        
        routeBuilder.MapPost("{id}/image", async (
            Guid id,
            [FromBody]List<ProductImageModel> images,
            [FromServices]IProductService productService) =>
        {
            try
            {
                var request = new UpdateProductImagesRequest
                {
                    MerchantId = DefaultMerchant,
                    UserId = DefaultUser,
                    ProductId = id,
                    Images = images,
                };
                await productService.UpdateImages(request);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        
        routeBuilder.MapGet("{id}", async (
            Guid id,
            [FromServices]IProductService productService) =>
        {
            var product = await productService.GetProduct(id, DefaultMerchant);
            return Results.Ok(product);
        });
        
        routeBuilder.MapGet("image/{id}", async (
            Guid id,
            HttpContext context,
            [FromServices]IStorageService storageService) =>
        {
            try
            {
                var file = await storageService.GetFileAsync(DefaultMerchant, id, context.RequestAborted);
                return Results.File(file, "image/jpeg");
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message); 
            }
        });
    }
}