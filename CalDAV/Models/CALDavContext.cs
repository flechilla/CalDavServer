﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CalDAV.Models
{
    public class CalDavContext :DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Caldav;Trusted_Connection=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarCollection>()
                .HasOne(u=>u.User)
                .WithMany(cr=>cr.CalendarCollections);
            modelBuilder.Entity<CalendarResource>()
                .HasOne(cl => cl.User)
                .WithMany(u => u.Resources);
        }
    }
}