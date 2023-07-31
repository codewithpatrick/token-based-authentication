using Microsoft.AspNetCore.Identity;
using Server.Data.Helpers;

namespace Server.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedRolesToDb(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Manager))
                {
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = UserRoles.Manager,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }

                if (!await roleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = UserRoles.Student,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }
            }
        }
    }
}
