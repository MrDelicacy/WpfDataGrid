using System.ComponentModel.DataAnnotations;

namespace WpfDataGrid.SQLiteOperation
{
    public class CompletedWork
    {
        public int Id { get; set; }
        public int CustomerOrderId { get; set; }
        [Required]
        public string TypeWork { get; set; }
        public bool SpectroTinting { get; set; }
        public bool CarTinting { get; set; }
        public bool Tinting { get; set; }
        public bool SpectroPreparation { get; set; }
        public int RecipeRaiting { get; set; }
        public CustomerOrder Order { get; set; }
    }
}
