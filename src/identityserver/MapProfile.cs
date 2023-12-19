using AutoMapper;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.STS.Admin;

/// <summary>
/// automapper profile
/// </summary>
public class MapProfile  : Profile
{
    public MapProfile()
    {
        CreateMap<ClientInput, Client>();
    }
}