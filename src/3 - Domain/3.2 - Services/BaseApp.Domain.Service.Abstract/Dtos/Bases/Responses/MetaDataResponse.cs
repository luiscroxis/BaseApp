namespace BaseApp.Domain.Service.Abstract.Dtos.Bases.Responses;

public class MetaDataResponse
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public int PageSize { get; set; }
}
