﻿using PFSoftware.Extensions;
using System;

namespace PersonalTracker.Media.Models.MediaTypes
{
    /// <summary>Represents a film.</summary>
    internal class Film : BaseINPC
    {
        private string _name;
        private DateTime _released;
        private decimal _rating;

        #region Modifying Properties

        /// <summary>Name of the Film.</summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        /// <summary>Release date of the Film.</summary>
        public DateTime Released
        {
            get => _released;
            set { _released = value; NotifyPropertyChanged(nameof(Released)); }
        }

        /// <summary>Rating for the Film.</summary>
        public decimal Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                NotifyPropertyChanged(nameof(Rating));
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Release date of the Film, formatted to string.</summary>
        public string ReleasedToString => Released.ToString("yyyy/MM/dd");

        #endregion Helper Properties

        #region Override Operators

        private static bool Equals(Film left, Film right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase) && DateTime.Equals(left.Released, right.Released) && left.Rating == right.Rating;
        }

        public sealed override bool Equals(object obj) => Equals(this, obj as Film);

        public bool Equals(Film otherFilm) => Equals(this, otherFilm);

        public static bool operator ==(Film left, Film right) => Equals(left, right);

        public static bool operator !=(Film left, Film right) => !Equals(left, right);

        public sealed override int GetHashCode() => base.GetHashCode() ^ 17;

        public sealed override string ToString() => $"{Name} ({Released:yyyy})";

        #endregion Override Operators

        #region Constructors

        /// <summary>Initalizes a default instance of Film.</summary>
        public Film()
        {
        }

        /// <summary>Initializes an instance of Film by assigning values to Properties.</summary>
        /// <param name="name">Name of the Film</param>
        /// <param name="released">Release date of the Film</param>
        /// <param name="rating">Rating for the Film</param>
        public Film(string name, DateTime released, decimal rating)
        {
            Name = name;
            Released = released;
            Rating = rating;
        }

        /// <summary>Replaces this instance of Film with another instance.</summary>
        /// <param name="other">Instance of Film to replace this instance</param>
        public Film(Film other) : this(other.Name, other.Released, other.Rating)
        {
        }

        #endregion Constructors
    }
}