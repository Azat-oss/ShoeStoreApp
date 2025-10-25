using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStoreApp.Models
{
    public class Shoe
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // Наименование

        [Required]
        public string Group { get; set; } // Группа (мужская, женская и т.д.)

        [Required]
        public string Article { get; set; } // Артикул

        [Required]
        public string Manufacturer { get; set; } // Изготовитель

        [Required]
        public string Size { get; set; } // Размер

        [Required]
        public string Color { get; set; } // Цвет

        [Required]
        public string Material { get; set; } // Материал

        [Required]
        public string ModelName { get; set; } // Модель

        [Required]
        public DateTime ProductionDate { get; set; }  = DateTime.Today; // Дата производства

        [Required]
        public int WarrantyMonths { get; set; } // Гарантийный срок (в месяцах)

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Цена
    }
}
