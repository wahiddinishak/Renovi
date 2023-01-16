﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using renovi.Models;

namespace renovi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211018041647_analisa")]
    partial class analisa
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("renovi.Models.ActD", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("header")
                        .HasColumnType("int");

                    b.Property<string>("idProyek")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("kegiatan")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("uom")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<double>("volume")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("actDs");
                });

            modelBuilder.Entity("renovi.Models.ActH", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Seq")
                        .HasColumnType("int");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("idProyek")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("kegiatan")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("actHs");
                });

            modelBuilder.Entity("renovi.Models.Material", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("harga")
                        .HasColumnType("double");

                    b.Property<string>("idProyek")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("item")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("jenis")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("uom")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("materials");
                });

            modelBuilder.Entity("renovi.Models.Proyek", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("alamat")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<double>("budget")
                        .HasColumnType("double");

                    b.Property<string>("desain")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("idProyek")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("isActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("judul")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("kontrak")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("mandor")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("pemilik")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("tglMulai")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("tglSelesai")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("proyeks");
                });

            modelBuilder.Entity("renovi.Models.Tagihan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Seq")
                        .HasColumnType("int");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("idProyek")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("isPaid")
                        .HasColumnType("tinyint(1)");

                    b.Property<double>("nominal")
                        .HasColumnType("double");

                    b.Property<DateTime>("tglBayar")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("tagihans");
                });

            modelBuilder.Entity("renovi.Models.accessRight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("menu")
                        .HasColumnType("int");

                    b.Property<string>("role")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("AccessRights");
                });

            modelBuilder.Entity("renovi.Models.analisa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("hargaItem")
                        .HasColumnType("double");

                    b.Property<int>("idActD")
                        .HasColumnType("int");

                    b.Property<string>("jenisItem")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<double>("koefisien")
                        .HasColumnType("double");

                    b.Property<string>("namaitem")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("uomItem")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("analisas");
                });

            modelBuilder.Entity("renovi.Models.item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("harga")
                        .HasColumnType("double");

                    b.Property<string>("nama")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("uom")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("renovi.Models.menu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("action")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("controller")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("parent")
                        .HasColumnType("int");

                    b.Property<string>("property")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("renovi.Models.role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("desc")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("roleName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("renovi.Models.tukang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("nama")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("namaBank")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("rekening")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("telepon")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex("UpdateBy");

                    b.ToTable("Tukangs");
                });

            modelBuilder.Entity("renovi.Models.user", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("fullname")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("isActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("role")
                        .HasColumnType("int");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("renovi.Models.ActD", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.ActH", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.Material", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.Proyek", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.Tagihan", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.accessRight", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.analisa", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.item", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.menu", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.role", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });

            modelBuilder.Entity("renovi.Models.tukang", b =>
                {
                    b.HasOne("renovi.Models.user", "AppUserCreatedBy")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("renovi.Models.user", "AppUserModifedBy")
                        .WithMany()
                        .HasForeignKey("UpdateBy");
                });
#pragma warning restore 612, 618
        }
    }
}
