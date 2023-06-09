﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YouTubeV2.Application;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    [DbContext(typeof(YTContext))]
    [Migration("20230425172720_PlaylistVideoConfiguration")]
    partial class PlaylistVideoConfiguration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                            RoleId = "39cc2fe2-d00d-4f48-a49d-005d8e983c72"
                        },
                        new
                        {
                            UserId = "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                            RoleId = "63798117-72aa-4bc5-a1ef-4e771204d561"
                        },
                        new
                        {
                            UserId = "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                            RoleId = "b3a48a48-1a74-45da-a179-03b298bc53bc"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("PlaylistVideo", b =>
                {
                    b.Property<Guid>("PlaylistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VideoId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PlaylistId", "VideoId");

                    b.HasIndex("VideoId");

                    b.ToTable("PlaylistVideo");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuthorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("VideoId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("VideoId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.CommentResponse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuthorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("RespondOnId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("RespondOnId");

                    b.ToTable("CommentResponses");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Playlist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Visibility")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "39cc2fe2-d00d-4f48-a49d-005d8e983c72",
                            Name = "Simple",
                            NormalizedName = "SIMPLE"
                        },
                        new
                        {
                            Id = "63798117-72aa-4bc5-a1ef-4e771204d561",
                            Name = "Creator",
                            NormalizedName = "CREATOR"
                        },
                        new
                        {
                            Id = "b3a48a48-1a74-45da-a179-03b298bc53bc",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        });
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Subscription", b =>
                {
                    b.Property<string>("SubscriberId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SubscribeeId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("SubscriberId", "SubscribeeId");

                    b.HasIndex("SubscribeeId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("VideoId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VideoId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "08803007-ce53-4b86-99fc-a6fc4e4d095a",
                            Email = "simple@test.com",
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                            Name = "Simple",
                            NormalizedEmail = "SIMPLE@TEST.COM",
                            NormalizedUserName = "TESTSIMPLE",
                            PasswordHash = "AQAAAAIAAYagAAAAEA1OwJWOEzSG7SZcMz0hlLdaPpRy3Mx2oRJfXvQINgxe+zy3BCEwmw6nULReKbgowg==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "63061695-9037-4c29-aa6c-13cccda8f5b2",
                            Surname = "Test",
                            TwoFactorEnabled = false,
                            UserName = "TestSimple"
                        },
                        new
                        {
                            Id = "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "8d3ebc1d-7606-41a5-87e7-c3ab5592c9f2",
                            Email = "creator@test.com",
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                            Name = "Creator",
                            NormalizedEmail = "CREATOR@TEST.COM",
                            NormalizedUserName = "TESTCREATOR",
                            PasswordHash = "AQAAAAIAAYagAAAAEB5smZL6LRrGI5WTsjslHccf1GbxPmxj63Cv4jisSa7ISNAaWaGrJIAWX5ZSsrrb6A==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "11b86b1a-cbff-48ca-852e-867e1c182a80",
                            Surname = "Test",
                            TwoFactorEnabled = false,
                            UserName = "TestCreator"
                        },
                        new
                        {
                            Id = "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "a12ff120-4800-40ea-ab24-76f61dddf9ab",
                            Email = "admin@test.com",
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                            Name = "Admin",
                            NormalizedEmail = "ADMIN@TEST.COM",
                            NormalizedUserName = "TESTADMIN",
                            PasswordHash = "AQAAAAIAAYagAAAAELharXakR+FR6kfmqQauWwUBTL+kK1BI0W6sQjBBxmMzNNKFZ+3iJqYtyZ2DYBCoaQ==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "0b3316b0-21cc-43d4-80e6-facea709efb5",
                            Surname = "Test",
                            TwoFactorEnabled = false,
                            UserName = "TestAdmin"
                        });
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Video", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("EditDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("ProcessingProgress")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTimeOffset>("UploadDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.Property<int>("Visibility")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YouTubeV2.Application.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PlaylistVideo", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YouTubeV2.Application.Model.Video", null)
                        .WithMany()
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Comment", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("YouTubeV2.Application.Model.Video", "Video")
                        .WithMany("Comments")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.CommentResponse", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("YouTubeV2.Application.Model.Comment", "RespondOn")
                        .WithMany("Responses")
                        .HasForeignKey("RespondOnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("RespondOn");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Playlist", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", "Creator")
                        .WithMany("Playlists")
                        .HasForeignKey("CreatorId");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Subscription", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", "Subscribee")
                        .WithMany("Subscriptions")
                        .HasForeignKey("SubscribeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YouTubeV2.Application.Model.User", "Subscriber")
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Subscribee");

                    b.Navigation("Subscriber");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Tag", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.Video", "Video")
                        .WithMany("Tags")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Video");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Video", b =>
                {
                    b.HasOne("YouTubeV2.Application.Model.User", "Author")
                        .WithMany("Videos")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Comment", b =>
                {
                    b.Navigation("Responses");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.User", b =>
                {
                    b.Navigation("Playlists");

                    b.Navigation("Subscriptions");

                    b.Navigation("Videos");
                });

            modelBuilder.Entity("YouTubeV2.Application.Model.Video", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
