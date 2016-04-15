﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.Entities;

namespace DataLayer.Models.ACL
{
    /// <summary>
    ///     Entity for the properties of the principals.
    ///     A principal may be a user or a group that contains many users.
    /// </summary>
    public class Principal
    {
        /// <summary>
        ///     Identify uniquely a principal.
        ///     A principal may have many URLs, but there must be one "principal URL"
        ///     that clients can use to uniquely identify a principal.This protected
        ///     property contains the URL that MUST be used to identify this principal
        ///     in an ACL request.
        /// </summary>
        public string PrincipalURL { get; set; }

        /// <summary>
        ///     This property of a group principal identifies the principals that are
        ///     direct members of this group.
        /// </summary>
        public string GroupMemberSet { get; set; }

        /// <summary>
        ///     This protected property identifies the groups in which the principal
        ///     is directly a member.
        /// </summary>
        public string GroupMembership { get; set; }

        /// <summary>
        ///     The readable name of the principal.
        /// </summary>
        public string Displayname { get; set; }

        [ScaffoldColumn(false)]
        public int PrincipalId { get; set; }

        /// <summary>
        /// retrun the value of a property by the
        /// string representation of the name.
        /// This is done by reflection.
        /// </summary>
        /// <param name="propertyName">The string representation of the property.</param>
        /// <returns>The value of the desire property</returns>
        //public string this[string propertyName]
        //{
        //    get{

        //        try
        //        {
        //            return (string) this.GetType().GetProperty(propertyName).GetValue(this, null);
        //        }
        //        catch
        //        {
        //            return "";
        //        }
        //    }
        //}

        /// <summary>
        ///     Contains the properties of 
        ///     principal.
        /// </summary>            
        public List<Property> Properties { get; set; }
    }
}