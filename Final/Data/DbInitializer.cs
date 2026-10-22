using Final.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Final.Data
{
    public static class DbInitializer
    {
        public const string DefaultUserEmail = "developer@yzu.edu.tw";

        public static void Initialize(CmsContext context)
        {
            context.Database.Migrate();

            if (!context.Category.Any())
            {
                var categories = new Category[]
                {
                    new Category { CateName = "課業" },
                    new Category { CateName = "工作" },
                    new Category { CateName = "娛樂" },
                    new Category { CateName = "休息" }
                };
                context.Category.AddRange(categories);
                context.SaveChanges();
            }

            var defaultUser = context.Users.FirstOrDefault(u => u.Email == DefaultUserEmail);
            if (defaultUser == null)
            {
                defaultUser = new Users
                {
                    Name = "開發者測試",
                    Email = DefaultUserEmail,
                    Password = "password123"
                };
                context.Users.Add(defaultUser);
                context.SaveChanges();
            }

        }
    }
}
