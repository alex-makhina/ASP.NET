using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.Partners;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnerRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerPromoCodeLimitRepositoryMock;
    private readonly PartnersController _sut;
    public SetLimitTests()
    {
        _partnerRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerPromoCodeLimitRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();
        _sut = new PartnersController(_partnerRepositoryMock.Object, _partnerPromoCodeLimitRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var request = CreateLimitCreateRequest();
        _partnerRepositoryMock
            .Setup(x => x.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        //Act
        var response = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        //Assert
        var notFoundResult = response.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFoundResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        //Arrange
        var partner = CreatePartnerWithLimit(false);
        var request = CreateLimitCreateRequest();
        _partnerRepositoryMock
            .Setup(x => x.GetById(partner.Id, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        //Act
        var response = await _sut.CreateLimit(partner.Id, request, CancellationToken.None);

        //Assert
        var unprocessableEntityObjectResult = response.Result.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
        var problemDetails = unprocessableEntityObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Partner blocked");
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        //Arrange
        var partner = CreatePartnerWithLimit(true);
        var request = CreateLimitCreateRequest();
        _partnerRepositoryMock
            .Setup(x => x.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        //Act
        var response = await _sut.CreateLimit(partner.Id, request, CancellationToken.None);

        //Assert
        var createdAtObjectResult = response.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var responseModel = createdAtObjectResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>().Subject;
        createdAtObjectResult.RouteValues.Should().HaveCount(2);
        createdAtObjectResult.RouteValues["partnerId"].Should().Be(partner.Id);
        createdAtObjectResult.RouteValues["limitId"].Should().Be(responseModel.Id);
        createdAtObjectResult.ActionName.Should().Be(nameof(_sut.GetLimit));
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        //Arrange
        var partner = CreatePartnerWithLimit(true, null);
        var request = CreateLimitCreateRequest();
        _partnerRepositoryMock.Setup(x => x.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        //Act
        var response = await _sut.CreateLimit(partner.Id, request, CancellationToken.None);

        //Assert
        partner.PartnerLimits.First().CanceledAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        //Arrange
        var partner = CreatePartnerWithLimit(true);
        var request = CreateLimitCreateRequest();
        _partnerRepositoryMock.Setup(x => x.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _partnerRepositoryMock.Setup(x => x.Update(partner, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partner.Id));

        //Act
        var response = await _sut.CreateLimit(partner.Id, request, CancellationToken.None);

        //Assert
        response.Result.Should().BeOfType<NotFoundResult>();
    }

    private static PartnerPromoCodeLimitCreateRequest CreateLimitCreateRequest()
    {
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>();
        return request;
    }

    private static Partner CreatePartnerWithLimit(bool isActive, DateTimeOffset? limitCanceledAt = null)
    {
        var limits = new List<PartnerPromoCodeLimit>();
        var partner = new AutoFaker<Partner>()
            .RuleFor(x => x.IsActive, isActive)
            .RuleFor(x => x.PartnerLimits, limits)
            .Generate();

        var limit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(x => x.Partner, partner)
            .RuleFor(x => x.CanceledAt, limitCanceledAt)
            .Generate();

        partner.PartnerLimits.Add(limit);

        return partner;
    }
}
