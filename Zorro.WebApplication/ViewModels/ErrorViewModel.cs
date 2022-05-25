namespace Zorro.WebApplication.ViewModels
{
    //error view model with fields
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}