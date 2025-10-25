using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoeStoreApp.Models;
using Bogus;

namespace ShoeStoreApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Shoe> Shoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Замените строку подключения на вашу
            optionsBuilder.UseSqlServer(@"Server = (localdb)\MSSQLLocalDB;Database=ShoeStoreDb;Trusted_Connection=True;Encrypt=False;");
        }

        //  уникальный индекс
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shoe>()
                .HasIndex(s => s.Article)
                .IsUnique(); 

            base.OnModelCreating(modelBuilder);
        }
        public void SeedData()
        {
            if (Shoes.Any()) return;

            var groups = new[] { "Мужская", "Женская", "Детская" };
            var sizes = new[] { "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45" };
            var materials = new[] { "Кожа", "Замша", "Текстиль", "Экокожа", "Полиуретан", "Нубук" };
            var colors = new[] { "Чёрный", "Белый", "Коричневый", "Серый", "Синий", "Красный", "Бежевый", "Зелёный" };
            var models = new[] { "Спортивная", "Классическая", "Повседневная", "Зимняя", "Летняя", "Туристическая" };
            var manufacturers = new[] { "Ecco", "Geox", "Ralf Ringer", "Westfalika", "Тэффи", "Belle", "Salamander", "Nike", "Adidas" };

            var random = new Random();

            var shoes = new Faker<Shoe>()
                .RuleFor(s => s.Name, f => $"{f.PickRandom(new[] { "Кроссовки", "Ботинки", "Туфли", "Сапоги", "Лоферы", "Мокасины", "Сандалии" })} {f.PickRandom(manufacturers)}")
                .RuleFor(s => s.Group, f => f.PickRandom(groups))
                .RuleFor(s => s.Article, f => f.Random.Replace("???-####")) // например: ABC-1234
                .RuleFor(s => s.Manufacturer, f => f.PickRandom(manufacturers))
                .RuleFor(s => s.Size, f => f.PickRandom(sizes))
                .RuleFor(s => s.Color, f => f.PickRandom(colors))
                .RuleFor(s => s.Material, f => f.PickRandom(materials))
                .RuleFor(s => s.ModelName, f => f.PickRandom(models))
                .RuleFor(s => s.ProductionDate, f => f.Date.Past(3))
                .RuleFor(s => s.WarrantyMonths, f => f.Random.Number(6, 36))
                .RuleFor(s => s.Price, f => Math.Round(f.Random.Decimal(1500, 25000), 2))
                .Generate(50);

            Shoes.AddRange(shoes);
            SaveChanges();
        }

    }
}
