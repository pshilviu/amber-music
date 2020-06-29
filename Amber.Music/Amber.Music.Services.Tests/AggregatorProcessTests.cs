using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amber.Music.Services.Tests
{
    [TestClass]
    public class AggregatorProcessTests
    {
        #region Setup Helpers

        /// <summary>
        /// Class used to help with creating a system under test object
        /// Helping with reusability of the setup but also to guide towards writing better unit tests by isolating resources
        /// </summary>
        private class SutContext
        {
            public Mock<ILyricsService> LyricsService { get; } = new Mock<ILyricsService>();

            public Mock<IArtistService> ArtistService { get; } = new Mock<IArtistService>();

            public Mock<IWordCounterService> WordCounterService { get; } = new Mock<IWordCounterService>();

            public Mock<ICacheService> CacheService { get; } = new Mock<ICacheService>();

            public IAggregatorProcess Process { get; }

            public SutContext()
            {
                var works = new Dictionary<Guid, Dictionary<Guid, ArtistWork>>();
                CacheService.Setup(x => x.Works).Returns(works);
                Process = new AggregatorProcess(
                    LyricsService.Object,
                    ArtistService.Object,
                    WordCounterService.Object,
                    CacheService.Object
                );
            }
        }

        #endregion Setup Helpers

        [TestMethod]
        public async Task AggregateDataAsync_ArtistNotFound_ThrowsException()
        {
            // Arrange
            var context = new SutContext();
            context.ArtistService
                .Setup(x => x.GetArtistAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException("abc"));

            // Act
            try
            {
                await context.Process.AggregateDataAsync(Guid.NewGuid());

                // Assert
                Assert.Fail("Expected method to throw InvalidOperationException");
            }
            catch (Exception ex)
            {
                // Assert
                var exceptionType = ex.GetType();
                Assert.AreEqual(typeof(InvalidOperationException), exceptionType, $"Expected Exception Type {nameof(InvalidOperationException)} but found {exceptionType}");
                Assert.AreEqual("abc", ex.Message, nameof(ex.Message));
            }
        }

        [TestMethod]
        public async Task AggregateDataAsync_ValidId_ServiceGetsCalledWithCorrectData()
        {
            // Arrange
            var context = new SutContext();
            var id = Guid.NewGuid();
            var artistSearch = new ArtistSearch
            {
                Id = id,
                Name = "abcdef",
                SortName = "abc",
                Type = "something",
                Area = "a"
            };

            context.ArtistService
                .Setup(x => x.GetArtistAsync(id))
                .ReturnsAsync(artistSearch);

            // Act
            var result = await context.Process.AggregateDataAsync(id);

            // Assert
            context.ArtistService.Verify(x => x.GetArtistAsync(id), Times.Once);
            Assert.AreEqual(id, result.ArtistId, nameof(result.ArtistId));
            Assert.AreEqual("abcdef", result.Name, nameof(result.Name));
        }

        [TestMethod]
        public async Task AggregateDataAsync_MixedData_ServiceReturnsCorrectData()
        {
            // Arrange
            var context = new SutContext();
            var id = Guid.NewGuid();
            var artistSearch = new ArtistSearch
            {
                Id = id,
                Name = "abcdef",
                SortName = "abc",
                Type = "something",
                Area = "a"
            };

            // TODO: could refactor and make it data driven
            var works = new[]
            {
                new ArtistWork { Id = new Guid("00000000-0000-0000-0000-000000000001"), Title = "1" },
                new ArtistWork { Id = new Guid("00000000-0000-0000-0000-000000000002"), Title = "2" },
                new ArtistWork { Id = new Guid("00000000-0000-0000-0000-000000000003"), Title = "3" },
                new ArtistWork { Id = new Guid("00000000-0000-0000-0000-000000000004"), Title = "4" },
                new ArtistWork { Id = new Guid("00000000-0000-0000-0000-000000000005"), Title = "5" },
                new ArtistWork { Id = new Guid("00000000-0000-0000-0000-000000000006"), Title = "6" }
            };

            var expectedResults = new Dictionary<string, (string, int)>()
            {
                { "1", ("a", 1) },
                { "2", ("a a", 2) },
                { "3", ("", 0) },
                { "4", ("a a a a", 4) },
                { "5", ("a a a a a", 5) },
                { "6", ("a a a a a a", 6) }
            };

            context.ArtistService
                .Setup(x => x.GetArtistAsync(id))
                .ReturnsAsync(artistSearch);

            context.ArtistService
                .Setup(x => x.GetArtistWorksAsync(id))
                .ReturnsAsync(works);

            foreach (var work in works)
            {
                var expectedSetupValue = expectedResults[work.Title];

                context.LyricsService
                    .Setup(x => x.SearchAsync(artistSearch.Name, work.Title))
                    .ReturnsAsync(expectedSetupValue.Item1);

                context.WordCounterService
                    .Setup(x => x.Count(expectedSetupValue.Item1))
                    .Returns(expectedSetupValue.Item2);
            }

            // Act
            var result = await context.Process.AggregateDataAsync(id);

            // Assert
            Assert.AreEqual(id, result.ArtistId, nameof(result.ArtistId));
            Assert.AreEqual("abcdef", result.Name, nameof(result.Name));
            Assert.AreEqual(6, result.TotalSongs, nameof(result.TotalSongs));
            Assert.AreEqual(5, result.SongsConsidered, nameof(result.SongsConsidered));
            Assert.AreEqual(3, result.AverageWords, nameof(result.AverageWords));
            Assert.AreEqual(3.8D, result.Variance, nameof(result.Variance));
            Assert.AreEqual(1.95D, result.StandardDeviation, nameof(result.StandardDeviation));
        }

        // TODO: There are more scenarios that can be tested, in a simillar method as above
        // Testing around works being found or not
        // Testing that it doesnt' crash the code when the lyrics are not found
        // Testing that the services get called with correct data
        // Testing for duplicates
    }
}
