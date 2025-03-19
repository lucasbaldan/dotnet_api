namespace dotnet_api.Utilities;

public class Pagination
{
    const int _maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = _maxPageSize;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > _maxPageSize || value == 0) ? _maxPageSize : value;
    }
}
