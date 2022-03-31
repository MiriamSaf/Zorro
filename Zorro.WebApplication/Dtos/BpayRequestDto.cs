namespace Zorro.WebApplication.Dtos
{
    public class BpayRequestDto
    {
        public string BillPayID { get; set; }
        public bool RememberBiller { get; set; }
        public int PayeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ScheduleTimeUtc { get; set; }
    }
}
