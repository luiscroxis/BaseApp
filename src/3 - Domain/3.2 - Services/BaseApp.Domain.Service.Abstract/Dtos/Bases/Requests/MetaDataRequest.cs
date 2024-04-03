namespace BaseApp.Domain.Service.Abstract.Dtos.Bases.Requests;

public class MetaDataRequest
{
    private int _pageSize = 10;
    private const int MaxPageSize = 100;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public MetaDataRequest() => PageSize = 10;

    public MetaDataRequest(int pageSize) => PageSize = pageSize;
}
