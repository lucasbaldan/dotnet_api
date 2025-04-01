using AutoMapper;
using dotnet_api.Database;
using dotnet_api.Repositories.UnitOfWork;
using dotnet_api.Shared.DTOs.AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_unit_test.UnitTests;

public class XunitUnitTests
{
    public readonly ITransaction repository;
    public readonly IMapper mapper;

    public static DbContextOptions<BDContext> BdContextOptions { get; }

    public const string connectionString = "Server=localhost;Database=dados_apidot;User Id=root;Password=;";

    static XunitUnitTests()
    {
        BdContextOptions = new DbContextOptionsBuilder<BDContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public XunitUnitTests()
    {
        var config = new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapperProfile());
        });

        mapper = config.CreateMapper();

        var context = new BDContext(BdContextOptions);

        repository = new Transaction(context);
    }
}