﻿using System.ComponentModel.DataAnnotations;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     This entity represents the properties
    ///     of any other entity.
    /// </summary>
    public class Property
    {
        public Property(string name, string nameSpace)
        {
            Name = name;
            Namespace = nameSpace;
            IsMutable = true;
            IsDestroyable = true;
            IsVisible = true;
        }

        public Property()
        {
        }

        [ScaffoldColumn(false)]
        public int PropertyId { get; set; }

        /// <summary>
        ///     The name of the property.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     The ns of the property in the xml.
        /// </summary>
        [Required]
        public string Namespace { get; set; }

        /// <summary>
        ///     The string representation of the
        ///     xml that contains the values of the
        ///     property.
        ///     Should contains the full ns of the node.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     True if the property can be deleted.
        ///     False otherwise.
        /// </summary>
        public bool IsDestroyable { get; set; }

        /// <summary>
        ///     True if the property can be modified.
        ///     False otherwise.
        /// </summary>
        public bool IsMutable { get; set; }

        /// <summary>
        ///     True if the property can be send in a
        ///     PROFIND request with allprop.
        /// </summary>
        public bool IsVisible { get; set; }

        #region Foreign keys and navigators

        public CalendarResource CalendarResource { get; set; }

        public int? CalendarResourceId { get; set; }

        public CalendarCollection CalendarCollection { get; set; }

        public int? CalendarCollectionId { get; set; }
        public CalendarHome CalendarHome{ get; set; }

        public int? CalendarHomeId { get; set; }

        public int? PricipalId { get; set; }

        public Principal Principal { get; set; }

        #endregion
    }
}