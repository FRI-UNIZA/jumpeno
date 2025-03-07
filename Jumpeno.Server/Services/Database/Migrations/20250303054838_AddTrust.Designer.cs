﻿// <auto-generated />
using Jumpeno.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Jumpeno.Server.Services.Database.Migrations
{
    [DbContext(typeof(DB))]
    [Migration("20250303054838_AddTrust")]
    partial class AddTrust
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("Jumpeno.Shared.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActivationCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Skin")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Trust")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Persons", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 3,
                            ActivationCode = "5678",
                            Email = "Fero.Mrkva@gmail.com",
                            Name = "Fero",
                            Skin = "Red",
                            Trust = false
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
