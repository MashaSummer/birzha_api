using AuthMicroservice.Domain.Base;

namespace AuthMicroservice.Web.Endpoints.EventItemsEndpoints.ViewModels;

public class EventItemUpdateViewModel : ViewModelBase
{
    public string Logger { get; set; } = null!;

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;
}