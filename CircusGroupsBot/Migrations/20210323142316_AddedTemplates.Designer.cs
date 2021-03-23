﻿// <auto-generated />
using CircusGroupsBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CircusGroupsBot.Migrations
{
    [DbContext(typeof(CircusDbContext))]
    [Migration("20210323142316_AddedTemplates")]
    partial class AddedTemplates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CircusGroupsBot.Events.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DateAndTime")
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<ulong>("EventMessageId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("EventName")
                        .HasColumnType("longtext");

                    b.Property<ulong>("LeaderUserID")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("CircusGroupsBot.Events.Template", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("DDs")
                        .HasColumnType("int");

                    b.Property<int>("Healers")
                        .HasColumnType("int");

                    b.Property<int>("Runners")
                        .HasColumnType("int");

                    b.Property<int>("Tanks")
                        .HasColumnType("int");

                    b.Property<string>("TemplateName")
                        .HasColumnType("longtext");

                    b.HasKey("TemplateId");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("CircusGroupsBot.Events.Event", b =>
                {
                    b.OwnsMany("CircusGroupsBot.Events.Signup", "Signups", b1 =>
                        {
                            b1.Property<string>("SignupId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("varchar(255)");

                            b1.Property<int>("EventId")
                                .HasColumnType("int");

                            b1.Property<bool>("IsRequired")
                                .HasColumnType("tinyint(1)");

                            b1.Property<int>("Role")
                                .HasColumnType("int");

                            b1.Property<ulong>("UserId")
                                .HasColumnType("bigint unsigned");

                            b1.HasKey("SignupId");

                            b1.HasIndex("EventId");

                            b1.ToTable("Signup");

                            b1.WithOwner()
                                .HasForeignKey("EventId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
