namespace PortfolioMicroService.Definitions.Mongodb.ViewModels;

public class AsssetViewModel
{
    public string Id { get; set; }

    public  int VolumeActive { get; set; }

    public int VolumeFrozen { get; set; }
}