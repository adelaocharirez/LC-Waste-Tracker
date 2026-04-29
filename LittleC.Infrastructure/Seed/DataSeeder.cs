using LittleC.Core.Models;
using LittleC.Infrastructure.Data;

namespace LittleC.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Name = "Khan N.", PIN = "1234", Role = "Manager", IsActive = true },
                    new User { Name = "Angel D.", PIN = "2345", Role = "AssistantManager", IsActive = true },
                    new User { Name = "Wal H.", PIN = "3456", Role = "ShiftLead", IsActive = true },
                    new User { Name = "Ana O.", PIN = "4567", Role = "ShiftLead", IsActive = true }
                );
            }

            if (!context.MenuItems.Any())
            {
                context.MenuItems.AddRange(
                    new MenuItem { Name = "Classic Pepperoni", CustomerPrice = 6.99m, IsActive = true },
                    new MenuItem { Name = "Classic Cheese", CustomerPrice = 6.99m, IsActive = true },
                    new MenuItem { Name = "3 Meat Treat", CustomerPrice = 12.49m, IsActive = true },
                    new MenuItem { Name = "5 Meat Feast", CustomerPrice = 13.99m, IsActive = true },
                    new MenuItem { Name = "Ultimate Supreme", CustomerPrice = 13.99m, IsActive = true },
                    new MenuItem { Name = "Hula Hawaiian", CustomerPrice = 11.99m, IsActive = true },
                    new MenuItem { Name = "Veggie", CustomerPrice = 13.49m, IsActive = true },
                    new MenuItem { Name = "Classic Italian Sausage", CustomerPrice = 7.99m, IsActive = true },
                    new MenuItem { Name = "Old World Fanceroni Pepperoni", CustomerPrice = 11.99m, IsActive = true },
                    new MenuItem { Name = "Slices-N-Stix", CustomerPrice = 9.49m, IsActive = true },
                    new MenuItem { Name = "EMB Cheese", CustomerPrice = 7.99m, IsActive = true },
                    new MenuItem { Name = "EMB Pepperoni", CustomerPrice = 7.99m, IsActive = true },
                    new MenuItem { Name = "EMB Sausage", CustomerPrice = 8.99m, IsActive = true },
                    new MenuItem { Name = "Crazy Bread", CustomerPrice = 4.49m, IsActive = true },
                    new MenuItem { Name = "Stuffed Crazy Bread", CustomerPrice = 5.49m, IsActive = true },
                    new MenuItem { Name = "Italian Cheese Bread", CustomerPrice = 6.99m, IsActive = true },
                    new MenuItem { Name = "Custom Italian Cheese Bread", CustomerPrice = 6.99m, IsActive = true },
                    new MenuItem { Name = "Pepperoni Cheese Bread", CustomerPrice = 7.49m, IsActive = true },
                    new MenuItem { Name = "4 Cheese Crazy Puffs", CustomerPrice = 3.99m, IsActive = true },
                    new MenuItem { Name = "Pepperoni Crazy Puffs", CustomerPrice = 3.99m, IsActive = true },
                    new MenuItem { Name = "Caesar Wings", CustomerPrice = 9.99m, IsActive = true },
                    new MenuItem { Name = "Caesar Dip", CustomerPrice = 1.45m, IsActive = true },
                    new MenuItem { Name = "Crazy Sauce", CustomerPrice = 1.49m, IsActive = true },
                    new MenuItem { Name = "Cookie Dough Brownie Twix", CustomerPrice = 4.89m, IsActive = true },
                    new MenuItem { Name = "Cookie Dough Brownie M&M", CustomerPrice = 4.89m, IsActive = true },
                    new MenuItem { Name = "Other (Custom)", CustomerPrice = 0.00m, IsActive = true, IsCustom = true }
                );
            }

            context.SaveChanges();
        }
    }
}