using System.ComponentModel.DataAnnotations;

namespace WpfDataGrid.SQLiteOperation
{
    public class Manufacturer
    {
        public int Id { get; set; }

        /// <summary>
        /// категория
        /// </summary>
        [Required]
        public string Category{ get; set; }
        /// <summary>
        /// название производителя
        /// </summary>
        [Required]
        public string ManufacturerName { get; set; }

        public override string ToString()
        {
            return ManufacturerName;
        }
    }
}
