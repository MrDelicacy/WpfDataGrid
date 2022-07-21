namespace WpfDataGrid.SQLiteOperation
{
    public class WorkProcessBook
    {
        public int Id { get; set; }
       // public int OrderId { get; set; }
        public int IterationId { get; set; }
        public int IterationEventCounter { get; set; }
        public int Field { get; set; }
        public float Amount { get; set; }
        public bool IsCancel { get; set; }

        public int CustomerOrder_Id { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
    }
}
