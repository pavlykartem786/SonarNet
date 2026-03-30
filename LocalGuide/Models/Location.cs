using System;
using System.Collections.Generic;

namespace LocalGuide.Models
{
    /// <summary>
    /// Represents a geographic location or point of interest in the city.
    /// Contains all details displayed to users on the guide platform.
    /// </summary>
    public class Location
    {
        /// <summary>Gets or sets the unique identifier of the location.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the display name of the location.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets a detailed description of the location.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the street address.</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Gets or sets the category (e.g., Restaurant, Park, Landmark).</summary>
        public LocationCategory Category { get; set; }

        /// <summary>Gets or sets the latitude coordinate.</summary>
        public double Latitude { get; set; }

        /// <summary>Gets or sets the longitude coordinate.</summary>
        public double Longitude { get; set; }

        /// <summary>Gets or sets the average rating (1-5).</summary>
        public double AverageRating { get; set; }

        /// <summary>Gets or sets the date when the location was added.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets whether the location is currently active/visible.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Gets or sets the collection of user reviews for this location.</summary>
        public List<Review> Reviews { get; set; } = new();
    }

    /// <summary>Defines the category types for locations.</summary>
    public enum LocationCategory
    {
        /// <summary>Restaurant or cafe.</summary>
        Restaurant,
        /// <summary>Public park or green area.</summary>
        Park,
        /// <summary>Historical or cultural landmark.</summary>
        Landmark,
        /// <summary>Entertainment venue.</summary>
        Entertainment,
        /// <summary>Hotel or accommodation.</summary>
        Hotel,
        /// <summary>Shopping center or store.</summary>
        Shopping
    }
}