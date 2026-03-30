using System;
using System.Collections.Generic;
using System.Linq;
using LocalGuide.Models;

namespace LocalGuide.Services
{
    /// <summary>
    /// </summary>
    public class LocationService
    {
        /// <summary>In-in production).</summary>
        private readonly List<Location> _locations;

        /// <summary>
        /// Initialises a new instance of <see cref="LocationService"/>.
        /// </summary>
        /// <param name="locations">Initial collection of locations to manage.</param>
        public LocationService(List<Location> locations)
        {
            _locations = locations ?? throw new ArgumentNullException(nameof(locations));
        }

        /// <summary>
        /// </summary>
        /// <param name="filter">Encapsulates all filter criteria.</param>
        /// <returns>Filtered and sorted list of locations.</returns>
        public List<Location> FilterAndSortRefactored(LocationFilter filter)
        {
            var result = ApplyActiveFilter(_locations, filter.OnlyActive);
            result = ApplyCategoryFilter(result, filter.Category);
            result = ApplyRatingFilter(result, filter.MinRating, filter.MaxRating);
            result = ApplyTextSearch(result, filter.SearchText);
            result = ApplySort(result, filter.SortBy, filter.Ascending);
            return ApplyLimit(result, filter.MaxResults);
        }

        /// <summary>Filters locations by their active status.</summary>
        private static List<Location> ApplyActiveFilter(IEnumerable<Location> src, bool onlyActive)
            => onlyActive ? src.Where(l => l.IsActive).ToList() : src.ToList();

        /// <summary>Filters locations by category. Returns all if category is null.</summary>
        private static List<Location> ApplyCategoryFilter(List<Location> src, LocationCategory? category)
            => category == null ? src : src.Where(l => l.Category == category).ToList();

        /// <summary>
        /// </summary>
        private static List<Location> ApplyRatingFilter(List<Location> src, double? min, double? max)
        {
            var safeMin = min.HasValue ? Math.Max(1.0, min.Value) : (double?)null;
            var safeMax = max.HasValue ? Math.Min(5.0, max.Value) : (double?)null;
            return src
                .Where(l => (safeMin == null || l.AverageRating >= safeMin)
                         && (safeMax == null || l.AverageRating <= safeMax))
                .ToList();
        }

        /// <summary>Applies free-text search against Name and Description fields.</summary>
        private static List<Location> ApplyTextSearch(List<Location> src, string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return src;
            var q = text.ToLowerInvariant();
            return src.Where(l =>
                l.Name.ToLowerInvariant().Contains(q) ||
                l.Description.ToLowerInvariant().Contains(q)).ToList();
        }

        /// <summary>Sorts the list according to the specified field and direction.</summary>
        private static List<Location> ApplySort(List<Location> src, string sortBy, bool ascending)
            => sortBy?.ToLowerInvariant() switch
            {
                "name" => ascending ? src.OrderBy(l => l.Name).ToList() : src.OrderByDescending(l => l.Name).ToList(),
                "date" => ascending ? src.OrderBy(l => l.CreatedAt).ToList() : src.OrderByDescending(l => l.CreatedAt).ToList(),
                "reviews" => ascending ? src.OrderBy(l => l.Reviews.Count).ToList() : src.OrderByDescending(l => l.Reviews.Count).ToList(),
                _ => src.OrderByDescending(l => l.AverageRating).ToList()
            };

        /// <summary>Truncates the list to a maximum number of results. 0 means no limit.</summary>
        private static List<Location> ApplyLimit(List<Location> src, int max)
            => max > 0 ? src.Take(max).ToList() : src;

        // ──────────────────────────────────────────────────────────────────
        // Additional CRUD methods
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a location by its identifier, or null if not found.
        /// </summary>
        /// <param name="id">The identifier of the location.</param>
        /// <returns>The matching <see cref="Location"/> or null.</returns>
        public Location? GetById(int id)
            => _locations.FirstOrDefault(l => l.Id == id);

        /// <summary>
        /// Adds a new location to the collection.
        /// </summary>
        /// <param name="location">The location to add. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when location is null.</exception>
        public void Add(Location location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            location.Id = _locations.Count > 0 ? _locations.Max(l => l.Id) + 1 : 1;
            _locations.Add(location);
        }

        /// <summary>
        /// Removes a location by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the location to remove.</param>
        /// <returns>True if the location was removed; false if not found.</returns>
        public bool Remove(int id)
        {
            var location = GetById(id);
            if (location == null) return false;
            _locations.Remove(location);
            return true;
        }

        /// <summary>
        /// Returns all locations in the collection regardless of filters.
        /// </summary>
        /// <returns>Read-only list of all locations.</returns>
        public IReadOnlyList<Location> GetAll() => _locations.AsReadOnly();
    }

    /// <summary>
    /// Encapsulates filter and sort parameters for <see cref="LocationService.FilterAndSortRefactored"/>.
    /// </summary>
    public record LocationFilter(
        LocationCategory? Category,
        double? MinRating,
        double? MaxRating,
        string? SearchText,
        string SortBy,
        bool Ascending,
        bool OnlyActive,
        int MaxResults
    );
}