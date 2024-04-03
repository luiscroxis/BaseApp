namespace BaseApp.Domain.Service.Abstract.Mappers;

using AutoMapper;

public class BaseAutoMapper : Profile
{
    public BaseAutoMapper()
    {
        AllowNullDestinationValues = false;
        AllowNullCollections = false;
    }
}
