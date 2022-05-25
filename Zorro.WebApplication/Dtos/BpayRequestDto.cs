namespace Zorro.WebApplication.Dtos
{
    //bpay request data transfer object
    public class BpayRequestDto
    {
        public int BillPayID { get; set; }
        public bool RememberBiller { get; set; }
        public int PayeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ScheduleTimeUtc { get; set; }
    }
}
