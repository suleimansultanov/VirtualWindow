using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Constants;

namespace NasladdinPlace.Api.Extensions
{
    public static class DbContextAppBuilderExtensions
    {
        public static void EnsureMigrationOfContext<T>(this IApplicationBuilder app) where T : DbContext
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<T>();
                context.Database.Migrate();
            }
        }

        public static void CreateRoles(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                var roleNames = Roles.All;
                
                foreach (var role in roleNames)
                {
                    if (roleManager.FindByNameAsync(role).Result == null)
                        roleManager.CreateAsync(Role.FromName(role)).Wait();
                }
            }
        }

        public static void CreateUserAccount(
            this IApplicationBuilder app,
            string userName,
            string email,
            string role,
            string password)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()
                .CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                if (userManager.FindByNameAsync(userName).Result != null)
                    return;

                if (roleManager.FindByNameAsync(role).Result == null)
                    roleManager.CreateAsync(Role.FromName(role)).Wait();

                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email                    
                };

                var result = userManager.CreateAsync(user, password).Result;
                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, role).Wait();
            }
        }
    }
}