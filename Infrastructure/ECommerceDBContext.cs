﻿using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ECommerceDBContext:IdentityDbContext<User,IdentityRole<int>,int>
    {
        public ECommerceDBContext(DbContextOptions<ECommerceDBContext> options):base(options) { }
       
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Product { get; set; } // get error when name it products

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Compiste PK
            // Composite PK for Table ProductIdentity
            modelBuilder.Entity<ProductIdentity>()
                .HasKey(PID => new { PID.SerialNumber, PID.ProductID });

            // Composite PK for Phone
            modelBuilder.Entity<Phone>()
                .HasKey(Ph => new { Ph.PhoneNumber, Ph.UserID });
            #endregion


            #region User Entity To make Delete Behavior Cascade
            modelBuilder.Entity<User>()
                    .HasMany(U => U.Favourites)
                    .WithOne(F => F.User)
                    .HasForeignKey(F => F.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(U => U.Carts)
                .WithOne(C => C.User)
                .HasForeignKey(C => C.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(U => U.Wishlists)
                .WithOne(W => W.User)
                .HasForeignKey(W => W.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(U => U.Orders)
                .WithOne(O => O.User)
                .HasForeignKey(O => O.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(U => U.Reviews)
                .WithOne(RV => RV.User)
                .HasForeignKey(RV => RV.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(U => U.Addresses)
                .WithOne(A => A.User)
                .HasForeignKey(A => A.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(U => U.Phones)
                .WithOne(Ph => Ph.User)
                .HasForeignKey(Ph => Ph.UserID)
                .OnDelete(DeleteBehavior.Cascade); 
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
