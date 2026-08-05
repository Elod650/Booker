global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using AutoMapper;
global using Booker.Models.DTOs;
global using Booker.Models.Enums;
global using Booker.Models.Requests;
global using Booker.Repository.Entities;
global using Booker.Repository.Repositories.Interfaces;
global using Booker.Services.Helpers;
global using Booker.Services.Interfaces;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
// Aliased instead of importing System.Globalization wholesale: that namespace also
// contains a Calendar type, which would clash with Booker.Repository.Entities.Calendar.
global using CultureInfo = System.Globalization.CultureInfo;
