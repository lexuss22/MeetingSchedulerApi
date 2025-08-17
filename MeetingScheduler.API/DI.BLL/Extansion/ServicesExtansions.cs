using DI.BLL.Services;
using DI.BLL.Services.Interface;
using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;
using DI.DAL.Context;
using DI.DAL.Repository;
using DI.DAL.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DI.BLL.Extansion
{
    public static class ServicesExtansions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton(new AppData());
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IMeetingServices, MeetingServices>();
            services.AddScoped<IMeetingRepository, MeetingRepository>();
            return services;
        }
    }
}
