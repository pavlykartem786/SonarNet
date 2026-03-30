using System;
using System.Collections.Generic;
using LocalGuide.Models;
using LocalGuide.Services;
using Xunit;

namespace LocalGuide.Tests
{
    /// <summary>
    /// Unit tests for <see cref="LocationService"/> covering filter,
    /// sort, and CRUD behaviour.
    /// </summary>
    public class LocationServiceTests
    {
        // ?? Test data factory ???????????????????????????????????????????
        private static List<Location> BuildTestData() => new()
        {
            new Location { Id = 1, Name = "Central Park", Category = LocationCategory.Park,
                AverageRating = 4.8, IsActive = true,
                Description = "Large green area in the city centre.", Reviews = new() },
            new Location { Id = 2, Name = "Bistro Roma", Category = LocationCategory.Restaurant,
                AverageRating = 4.2, IsActive = true,
                Description = "Italian cuisine restaurant.", Reviews = new() },
            new Location { Id = 3, Name = "Old Castle", Category = LocationCategory.Landmark,
                AverageRating = 3.9, IsActive = true,
                Description = "Historical castle from the 14th century.", Reviews = new() },
            new Location { Id = 4, Name = "Closed Venue", Category = LocationCategory.Entertainment,
                AverageRating = 2.5, IsActive = false,
                Description = "Currently closed location.", Reviews = new() },
            new Location { Id = 5, Name = "Grand Hotel", Category = LocationCategory.Hotel,
                AverageRating = 4.5, IsActive = true,
                Description = "Five-star hotel in the city.", Reviews = new() },
        };

        // ?? FilterAndSort tests ?????????????????????????????????????????

        /// <summary>Verify that inactive locations are excluded when onlyActive is true.</summary>
        /// <summary>Verify that inactive locations are excluded when onlyActive is true.</summary>
        [Fact]
        public void FilterAndSortRefactored_OnlyActive_ExcludesInactiveLocations()
        {
            var service = new LocationService(BuildTestData());
            var filter = new LocationFilter(null, null, null, null, "rating", false, true, 0);
            var result = service.FilterAndSortRefactored(filter);

            Assert.All(result, l => Assert.True(l.IsActive));
        }

        /// <summary>Verify category filter returns only Park locations.</summary>
        [Fact]
        public void FilterAndSortRefactored_CategoryPark_ReturnsOnlyParks()
        {
            var service = new LocationService(BuildTestData());
            var filter = new LocationFilter(LocationCategory.Park, null, null, null, "rating", false, false, 0);
            var result = service.FilterAndSortRefactored(filter);

            Assert.All(result, l => Assert.Equal(LocationCategory.Park, l.Category));
        }

        /// <summary>Verify that minRating filter correctly excludes low-rated locations.</summary>
        [Fact]
        public void FilterAndSortRefactored_MinRating4_ExcludesLowRated()
        {
            var service = new LocationService(BuildTestData());
            var filter = new LocationFilter(null, 4.0, null, null, "rating", false, true, 0);
            var result = service.FilterAndSortRefactored(filter);

            Assert.All(result, l => Assert.True(l.AverageRating >= 4.0));
        }

        /// <summary>Verify text search matches Name field correctly.</summary>
        [Fact]
        public void FilterAndSortRefactored_SearchByName_ReturnsMatchingLocation()
        {
            var service = new LocationService(BuildTestData());
            var filter = new LocationFilter(null, null, null, "Castle", "rating", false, false, 0);
            var result = service.FilterAndSortRefactored(filter);

            Assert.Single(result);
            Assert.Equal("Old Castle", result[0].Name);
        }

        /// <summary>Verify maxResults limits the number of returned items.</summary>
        [Fact]
        public void FilterAndSortRefactored_MaxResults2_ReturnsTwoItems()
        {
            var service = new LocationService(BuildTestData());
            var filter = new LocationFilter(null, null, null, null, "rating", false, false, 2);
            var result = service.FilterAndSortRefactored(filter);

            Assert.Equal(2, result.Count);
        }

        /// <summary>Verify ascending name sort orders results alphabetically.</summary>
        [Fact]
        public void FilterAndSortRefactored_SortByNameAscending_ReturnsAlphabeticalOrder()
        {
            var service = new LocationService(BuildTestData());
            var filter = new LocationFilter(null, null, null, null, "name", true, false, 0);
            var result = service.FilterAndSortRefactored(filter);

            for (int i = 1; i < result.Count; i++)
                Assert.True(string.Compare(result[i - 1].Name, result[i].Name, StringComparison.Ordinal) <= 0);
        }

        // ?? CRUD tests ??????????????????????????????????????????????????

        /// <summary>Verify GetById returns the correct location.</summary>
        [Fact]
        public void GetById_ExistingId_ReturnsLocation()
        {
            var service = new LocationService(BuildTestData());
            var loc = service.GetById(1);
            Assert.NotNull(loc);
            Assert.Equal("Central Park", loc!.Name);
        }

        /// <summary>Verify GetById returns null for unknown ID.</summary>
        [Fact]
        public void GetById_UnknownId_ReturnsNull()
        {
            var service = new LocationService(BuildTestData());
            Assert.Null(service.GetById(999));
        }

        /// <summary>Verify Add increases the collection count and assigns an ID.</summary>
        [Fact]
        public void Add_NewLocation_IncreasesCount()
        {
            var service = new LocationService(BuildTestData());
            var before = service.GetAll().Count;
            service.Add(new Location { Name = "New Place", Category = LocationCategory.Shopping });
            Assert.Equal(before + 1, service.GetAll().Count);
        }

        /// <summary>Verify Remove returns true and decreases count for a known ID.</summary>
        [Fact]
        public void Remove_ExistingId_ReturnsTrueAndDecreasesCount()
        {
            var service = new LocationService(BuildTestData());
            var before = service.GetAll().Count;
            var removed = service.Remove(1);
            Assert.True(removed);
            Assert.Equal(before - 1, service.GetAll().Count);
        }

        /// <summary>Verify Remove returns false for an unknown ID.</summary>
        [Fact]
        public void Remove_UnknownId_ReturnsFalse()
        {
            var service = new LocationService(BuildTestData());
            Assert.False(service.Remove(999));
        }
    }

    /// <summary>
    /// Unit tests for <see cref="UserAuth"/> covering hashing and token generation.
    /// </summary>
    public class UserAuthTests
    {
        private readonly UserAuth _auth = new("super-secret-key-for-testing");

        /// <summary>Verify that hashing the same password twice produces different hashes (due to salt).</summary>
        [Fact]
        public void HashPassword_SamePasswordTwice_ProducesDifferentHashes()
        {
            var h1 = _auth.HashPassword("MyPassword123");
            var h2 = _auth.HashPassword("MyPassword123");
            Assert.NotEqual(h1, h2);
        }

        /// <summary>Verify that the correct password passes verification.</summary>
        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            var hash = _auth.HashPassword("SecurePass!1");
            Assert.True(_auth.VerifyPassword("SecurePass!1", hash));
        }

        /// <summary>Verify that a wrong password fails verification.</summary>
        [Fact]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            var hash = _auth.HashPassword("SecurePass!1");
            Assert.False(_auth.VerifyPassword("WrongPassword", hash));
        }

        /// <summary>Verify that a generated token passes validation and returns the correct user ID.</summary>
        [Fact]
        public void GenerateToken_ValidUser_TokenValidatesSuccessfully()
        {
            var user = new User { Id = 42, Email = "test@example.com", Role = UserRole.Visitor };
            var token = _auth.GenerateToken(user);
            var valid = _auth.ValidateToken(token, out int uid);
            Assert.True(valid);
            Assert.Equal(42, uid);
        }

        /// <summary>Verify that a tampered token fails validation.</summary>
        [Fact]
        public void ValidateToken_TamperedToken_ReturnsFalse()
        {
            var user = new User { Id = 1, Email = "a@b.com", Role = UserRole.Visitor };
            var token = _auth.GenerateToken(user) + "TAMPERED";
            Assert.False(_auth.ValidateToken(token, out _));
        }
    }
}