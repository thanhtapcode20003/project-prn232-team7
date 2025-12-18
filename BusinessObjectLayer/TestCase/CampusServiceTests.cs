//using BusinessObjectLayer.DTOs;
//using BusinessObjectLayer.DTOs.Campus;
//using BusinessObjectLayer.Enum;
//using BusinessObjectLayer.Exceptions;
//using BusinessObjectLayer.Services;
//using DataAccessLayer.Models;
//using FluentAssertions;
//using Moq;
//using Repository;
//using Xunit;

//namespace BusinessObjectLayer.Tests.Services
//{
//    public class CampusServiceTests
//    {
//        private readonly Mock<CampusRepository> _mockRepository;
//        private readonly CampusService _campusService;

//        public CampusServiceTests()
//        {
//            _mockRepository = new Mock<CampusRepository>();
//            _campusService = new CampusService(_mockRepository.Object);
//        }

//        #region CreateCampus

//        [Fact]
//        public async Task CreateCampus_WithValidData_ShouldReturnCampusResponse()
//        {
//            var request = new CampusRequest
//            {
//                Name = "CS1",
//                Address = "Q7",
//                Description = "Main campus"
//            };

//            _mockRepository
//                .Setup(x => x.CreateAsync(It.IsAny<Campus>()))
//                .Returns(Task.CompletedTask);

//            var result = await _campusService.CreateCampus(request);

//            result.Should().NotBeNull();
//            result.CampusName.Should().Be(request.Name);
//            result.Location.Should().Be(request.Address);
//            result.Status.Should().Be(StatusEnum.ACTIVE.ToString());

//            _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Campus>()), Times.Once);
//        }

//        #endregion

//        #region GetAllCampuses

//        [Fact]
//        public async Task GetAllCampuses_ShouldReturnOnlyActive()
//        {
//            var campuses = new List<Campus>
//            {
//                new Campus { Id = Guid.NewGuid(), Name = "A", Status = StatusEnum.ACTIVE.ToString() },
//                new Campus { Id = Guid.NewGuid(), Name = "B", Status = StatusEnum.INACTIVE.ToString() }
//            };

//            _mockRepository
//                .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Campus, bool>>>()))
//                .ReturnsAsync(campuses.Where(c => c.Status == StatusEnum.ACTIVE.ToString()).ToList());

//            var result = await _campusService.GetAllCampuses();

//            result.Should().HaveCount(1);
//            result.All(c => c.Status == StatusEnum.ACTIVE.ToString()).Should().BeTrue();
//        }

//        #endregion

//        #region GetCampusById

//        [Fact]
//        public async Task GetCampusById_ValidId_ShouldReturnCampus()
//        {
//            var id = Guid.NewGuid();
//            var campus = new Campus
//            {
//                Id = id,
//                Name = "Campus",
//                Address = "Addr",
//                Status = StatusEnum.ACTIVE.ToString()
//            };

//            _mockRepository
//                .Setup(x => x.GetByIdAsync(id))
//                .ReturnsAsync(campus);

//            var result = await _campusService.GetCampusById(id);

//            result.CampusId.Should().Be(id);
//        }

//        [Fact]
//        public async Task GetCampusById_NotFound_ShouldThrow()
//        {
//            var id = Guid.NewGuid();

//            _mockRepository
//                .Setup(x => x.GetByIdAsync(id))
//                .ReturnsAsync((Campus?)null);

//            Func<Task> act = () => _campusService.GetCampusById(id);

//            await act.Should().ThrowAsync<NotFoundException>();
//        }

//        #endregion

//        #region UpdateCampus

//        [Fact]
//        public async Task UpdateCampus_Valid_ShouldUpdate()
//        {
//            var id = Guid.NewGuid();
//            var campus = new Campus
//            {
//                Id = id,
//                Name = "Old",
//                Status = StatusEnum.ACTIVE.ToString()
//            };

//            _mockRepository
//                .Setup(x => x.GetByIdAsync(id))
//                .ReturnsAsync(campus);

//            _mockRepository
//                .Setup(x => x.UpdateAsync(It.IsAny<Campus>()))
//                .Returns(Task.CompletedTask);

//            var result = await _campusService.UpdateCampus(id, new CampusRequest
//            {
//                Name = "New",
//                Address = "Addr"
//            });

//            result.CampusName.Should().Be("New");
//        }

//        #endregion

//        #region DeleteCampus

//        [Fact]
//        public async Task DeleteCampus_Valid_ShouldSetInactive()
//        {
//            var id = Guid.NewGuid();
//            var campus = new Campus
//            {
//                Id = id,
//                Status = StatusEnum.ACTIVE.ToString()
//            };

//            _mockRepository
//                .Setup(x => x.GetByIdAsync(id))
//                .ReturnsAsync(campus);

//            _mockRepository
//                .Setup(x => x.UpdateAsync(It.IsAny<Campus>()))
//                .Returns(Task.CompletedTask);

//            var result = await _campusService.DeleteCampus(id);

//            result.Should().BeTrue();
//            campus.Status.Should().Be(StatusEnum.INACTIVE.ToString());
//        }

//        #endregion

//        #region SearchCampuses

//        [Fact]
//        public async Task SearchCampuses_ShouldReturnPagedResult()
//        {
//            var campuses = new List<Campus>
//            {
//                new Campus { Id = Guid.NewGuid(), Name = "CS1", Status = StatusEnum.ACTIVE.ToString() }
//            };

//            _mockRepository
//                .Setup(x => x.SearchCampusesAsync(
//                    It.IsAny<string>(),
//                    It.IsAny<string?>(),
//                    It.IsAny<int>(),
//                    It.IsAny<int>()))
//                .ReturnsAsync((campuses, campuses.Count));

//            var result = await _campusService.SearchCampuses(new CampusFilterDto
//            {
//                Page = 1,
//                PageSize = 10
//            });

//            result.Items.Should().HaveCount(1);
//            result.TotalItems.Should().Be(1);
//        }

//        [Fact]
//        public async Task SearchCampuses_Empty_ShouldReturnEmpty()
//        {
//            _mockRepository
//                .Setup(x => x.SearchCampusesAsync(
//                    It.IsAny<string>(),
//                    It.IsAny<string?>(),
//                    It.IsAny<int>(),
//                    It.IsAny<int>()))
//                .ReturnsAsync((new List<Campus>(), 0));

//            var result = await _campusService.SearchCampuses(new CampusFilterDto());

//            result.Items.Should().BeEmpty();
//        }

//        #endregion
//    }
//}
