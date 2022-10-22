﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sample.Repository;

#nullable disable

namespace AzureDb.Passwordless.MySqlConnectorTests.Migrations
{
    [DbContext(typeof(ChecklistContext))]
    [Migration("20221022215911_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Sample.Repository.Model.CheckItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ChecklistID")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.HasIndex("ChecklistID");

                    b.ToTable("CheckItems");
                });

            modelBuilder.Entity("Sample.Repository.Model.Checklist", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("Checklists");
                });

            modelBuilder.Entity("Sample.Repository.Model.CheckItem", b =>
                {
                    b.HasOne("Sample.Repository.Model.Checklist", "Checklist")
                        .WithMany("CheckItems")
                        .HasForeignKey("ChecklistID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Checklist");
                });

            modelBuilder.Entity("Sample.Repository.Model.Checklist", b =>
                {
                    b.Navigation("CheckItems");
                });
#pragma warning restore 612, 618
        }
    }
}
