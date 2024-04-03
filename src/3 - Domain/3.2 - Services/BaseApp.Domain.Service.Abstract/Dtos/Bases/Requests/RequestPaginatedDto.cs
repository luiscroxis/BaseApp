namespace BaseApp.Domain.Service.Abstract.Dtos.Bases.Requests;

public class RequestPaginatedDto : RequestDto
{
    public string ColunmSort { get; set; }
    public MetaDataRequest MetaData { get; set; }
}
