﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CalDAV.Models
{
    /// <summary>
    /// to store the main properties of a cal resource.
    /// </summary>
    public class CalendarResource
    {
        [ScaffoldColumn(false)]
        public int CalendarResourceId { get; set; }

        [Required]
        public string FileName { get; set; }

        public string Etag { get; set; }

        public DateTime DtStart { get; set; }

        public DateTime DtEnd { get; set; }

        public string Recurrence { get; set; }

        public string Uid { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public CalendarCollection Collection { get; set; }


    }
}