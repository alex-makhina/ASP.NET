using AwesomeAssertions;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;
using System.Linq.Expressions;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodeRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodeRepositoryMock;
    private readonly Mock<IRepository<Partner>> _partnerRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferenceRepositoryMock;
    private readonly PromoCodesController _sut;
    public CreateTests()
    {
        _promoCodeRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customerRepositoryMock = new Mock<IRepository<Customer>>();
        _customerPromoCodeRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _partnerRepositoryMock = new Mock<IRepository<Partner>>();
        _preferenceRepositoryMock = new Mock<IRepository<Preference>>();
        _sut = new PromoCodesController(_promoCodeRepositoryMock.Object, _customerRepositoryMock.Object,
            _customerPromoCodeRepositoryMock.Object, _partnerRepositoryMock.Object, _preferenceRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        //Arrange
        var request = CreatePromoCodeRequest();
        _partnerRepositoryMock.Setup(x => x.GetById(request.PartnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        //Act
        var response = await _sut.Create(request, CancellationToken.None);

        //Assert
        var notFoundObject = response.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFoundObject.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        //Arrange
        var request = CreatePromoCodeRequest();
        var partner = CreatePartner(request.PartnerId);

        _partnerRepositoryMock.Setup(x => x.GetById(request.PartnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(request.PreferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        //Act
        var response = await _sut.Create(request, CancellationToken.None);

        //Assert
        var notFoundObject = response.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFoundObject.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Preference not found");
    }

    [Theory]
    [InlineData("2025-01-01", null)]
    [InlineData("9999-01-01", "2025-01-01")]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity(string limitEndAt, string? limitCanceledAt)
    {
        //Arrange
        var request = CreatePromoCodeRequest();
        var limit = CreateLimit(DateTimeOffset.Parse(limitEndAt), limitCanceledAt != null ? DateTimeOffset.Parse(limitCanceledAt) : null);
        var partner = CreatePartner(request.PartnerId, limit);
        var preference = CreatePreference(request.PreferenceId);

        _partnerRepositoryMock.Setup(x => x.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(request.PreferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        //Act
        var response = await _sut.Create(request, CancellationToken.None);

        //Assert
        var result = response.Result.Should().BeOfType<ObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        var problemDetails = result.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("No active limit");
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(10, 5)]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity(int issuedCount, int limit)
    {
        //Arrange
        var request = CreatePromoCodeRequest();
        var promoCodeLimit = CreateLimit(DateTimeOffset.MaxValue, issuedCount: issuedCount, limit: limit);
        var partner = CreatePartner(request.PartnerId, promoCodeLimit);
        var preference = CreatePreference(request.PreferenceId);

        _partnerRepositoryMock.Setup(x => x.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(request.PreferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        //Act
        var response = await _sut.Create(request, CancellationToken.None);

        //Assert
        var result = response.Result.Should().BeOfType<ObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        var problemDetails = result.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Limit exceeded");
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        //Arrange
        var issuedCount = Randomizer.Seed.Next();
        var request = CreatePromoCodeRequest();        
        var promoCodeLimit = CreateLimit(DateTimeOffset.MaxValue, issuedCount: issuedCount, limit: issuedCount+1);
        var partner = CreatePartner(request.PartnerId, promoCodeLimit);
        var preference = CreatePreference(request.PreferenceId);
        var customer = CreateCustomer(preference);

        _partnerRepositoryMock.Setup(x => x.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _customerRepositoryMock.Setup(x => x.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer> { customer });

        //Act
        var response = await _sut.Create(request, CancellationToken.None);

        //Assert
        var createdAtObjectResult = response.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var responseModel = createdAtObjectResult.Value.Should().BeOfType<PromoCodeShortResponse>().Subject;
        createdAtObjectResult.RouteValues.Should().HaveCount(1);
        createdAtObjectResult.RouteValues["id"].Should().Be(responseModel.Id);
        createdAtObjectResult.ActionName.Should().Be(nameof(_sut.GetById));
        promoCodeLimit.IssuedCount.Should().Be(issuedCount+1);
    }

    private static Customer CreateCustomer(Preference preference)
    {
        var preferences = new List<Preference> { preference };

        return new AutoFaker<Customer>()
            .RuleFor(x => x.Preferences, preferences)
            .Generate();
    }
    private static PromoCodeCreateRequest CreatePromoCodeRequest()
    {
        return new AutoFaker<PromoCodeCreateRequest>();
    }
    private static Preference CreatePreference(Guid id)
    {
        var preference = new AutoFaker<Preference>()
            .RuleFor(x => x.Id, id)
            .Generate();

        return preference;
    }
    private static Partner CreatePartner(Guid id, PartnerPromoCodeLimit? limit = null)
    {
        var limits = new List<PartnerPromoCodeLimit>();
        var partner = new AutoFaker<Partner>()
            .RuleFor(x => x.Id, id)
            .RuleFor(x => x.PartnerLimits, limits)
            .Generate();

        if (limit != null)
        {
            limit.Partner = partner;
            partner.PartnerLimits.Add(limit);
        }

        return partner;
    }
    private static PartnerPromoCodeLimit CreateLimit(DateTimeOffset endAt, DateTimeOffset? canceledAt = null,
        int issuedCount = 1, int limit = 5)
    {
        var promoCodeLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(x => x.CanceledAt, canceledAt)
            .RuleFor(x => x.EndAt, endAt)
            .RuleFor(x => x.IssuedCount, issuedCount)
            .RuleFor(x => x.Limit, limit)
            .Generate();

        return promoCodeLimit;
    }
}
