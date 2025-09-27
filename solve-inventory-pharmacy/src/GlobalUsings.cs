// Global using directives for common namespaces used across the solution
// Following coding conventions: Global using statements in GlobalUsings.cs

// System namespaces
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.ComponentModel.DataAnnotations;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;

// ASP.NET Core - Only include basic ones that are universally available
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;

// FluentValidation
global using FluentValidation;

 //Entity Framework - Removed to avoid compilation errors
 global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

//Common project namespaces - Removed to avoid circular references
 //global using SolveStation.Common.Models;
 //global using SolveStation.Common.Exceptions;
 //global using SolveStation.Common.Interfaces;
 //global using SolveStation.UserManagement.Models;
 //global using SolveStation.Data.Models;
 //global using SolveStation.Data.Repositories;
