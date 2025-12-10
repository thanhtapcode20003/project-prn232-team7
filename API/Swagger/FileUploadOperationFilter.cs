using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace API.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if any parameter is IFormFile or DTO containing IFormFile
            var hasFileParameter = context.MethodInfo.GetParameters()
                .Any(p => p.ParameterType == typeof(IFormFile) || 
                         p.ParameterType.GetProperties().Any(prop => prop.PropertyType == typeof(IFormFile)) ||
                         (p.GetCustomAttributes(typeof(FromFormAttribute), false).Any() && 
                          p.ParameterType.GetProperties().Any(prop => prop.PropertyType == typeof(IFormFile))));

            if (hasFileParameter)
            {
                // Find the parameter with IFormFile
                var fileParam = context.MethodInfo.GetParameters()
                    .FirstOrDefault(p => p.GetCustomAttributes(typeof(FromFormAttribute), false).Any() &&
                                       (p.ParameterType == typeof(IFormFile) || 
                                        p.ParameterType.GetProperties().Any(prop => prop.PropertyType == typeof(IFormFile))));

                if (fileParam != null)
                {
                    var paramType = fileParam.ParameterType;
                    
                    // If it's a DTO class, get properties from it
                    if (paramType != typeof(IFormFile) && paramType.IsClass)
                    {
                        var properties = paramType.GetProperties();
                        var schemaProperties = new Dictionary<string, OpenApiSchema>();
                        var required = new HashSet<string>();

                        foreach (var prop in properties)
                        {
                            var propType = prop.PropertyType;
                            var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;

                            if (propType == typeof(IFormFile))
                            {
                                schemaProperties[prop.Name] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                };
                                required.Add(prop.Name);
                            }
                            else
                            {
                                schemaProperties[prop.Name] = new OpenApiSchema
                                {
                                    Type = GetOpenApiType(propType),
                                    Format = GetOpenApiFormat(propType)
                                };
                                
                                // Add to required if not nullable and no default value
                                if (!IsNullable(propType))
                                {
                                    required.Add(prop.Name);
                                }
                            }
                        }

                        operation.RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["multipart/form-data"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = schemaProperties,
                                        Required = required
                                    }
                                }
                            }
                        };
                    }
                    else
                    {
                        // Single IFormFile parameter
                        operation.RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["multipart/form-data"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            [fileParam.Name!] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Format = "binary"
                                            }
                                        },
                                        Required = new HashSet<string> { fileParam.Name! }
                                    }
                                }
                            }
                        };
                    }

                    // Remove FromForm parameters from parameters list
                    operation.Parameters = operation.Parameters?
                        .Where(p => !context.MethodInfo.GetParameters()
                            .Any(param => param.GetCustomAttributes(typeof(FromFormAttribute), false).Any() && 
                                         param.Name == p.Name))
                        .ToList();
                }
            }
        }

        private bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null || 
                   (type.IsClass && type != typeof(string) && type != typeof(IFormFile));
        }

        private string GetOpenApiType(Type type)
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            
            if (underlyingType == typeof(string))
                return "string";
            if (underlyingType == typeof(int) || underlyingType == typeof(Guid))
                return "string";
            if (underlyingType == typeof(bool))
                return "boolean";
            if (underlyingType == typeof(DateTime))
                return "string";
            return "string";
        }

        private string? GetOpenApiFormat(Type type)
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            
            if (underlyingType == typeof(Guid))
                return "uuid";
            if (underlyingType == typeof(DateTime))
                return "date-time";
            return null;
        }
    }
}

