using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180323050701_CreateDatabase")]
    partial class CreateDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("ProviderKey");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("ProviderKey", "LoginProvider");

                    b.HasAlternateKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("UserId");

                    b.HasKey("RoleId", "UserId");

                    b.HasAlternateKey("UserId", "RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LoginProvider")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value");

                    b.HasKey("UserId");

                    b.HasAlternateKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("Birthdate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FullName")
                        .HasMaxLength(255);

                    b.Property<int>("Gender");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.BankTransactionInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BankTransactionId");

                    b.Property<int?>("PaybackId");

                    b.Property<decimal>("PaymentAmount");

                    b.Property<int>("PosId");

                    b.Property<int>("PosOperationId");

                    b.Property<DateTime?>("RefundDate");

                    b.Property<string>("RefundReason")
                        .HasMaxLength(3000);

                    b.HasKey("Id");

                    b.HasIndex("PosOperationId", "PosId");

                    b.ToTable("BankTransactionInfos");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CountryId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IsoCode")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("RUB")
                        .HasMaxLength(3);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasAlternateKey("IsoCode");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.DriverLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("PosId");

                    b.Property<string>("RelativePath")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("DriverLogs");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Good", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Article")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<int>("MakerId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<double?>("NetWeight");

                    b.Property<double?>("Volume");

                    b.HasKey("Id");

                    b.HasIndex("MakerId");

                    b.ToTable("Goods");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.GoodHistoryRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsFinal");

                    b.Property<int>("LabeledGoodId");

                    b.Property<int>("PosId");

                    b.Property<int>("PosOperationId");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("LabeledGoodId");

                    b.HasIndex("PosId");

                    b.HasIndex("PosOperationId", "PosId");

                    b.ToTable("GoodHistoryRecords");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.GoodImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GoodId");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("GoodId");

                    b.ToTable("GoodImages");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.GoodPriceInPos", b =>
                {
                    b.Property<int>("PosId");

                    b.Property<int>("GoodId");

                    b.Property<int>("CurrencyId");

                    b.Property<decimal>("Price");

                    b.HasKey("PosId", "GoodId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("GoodId");

                    b.ToTable("GoodPricesInPointsOfSale");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.LabeledGood", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ExpirationDate");

                    b.Property<int?>("GoodId");

                    b.Property<bool>("IsDisabled");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime?>("ManufactureDate");

                    b.Property<int?>("PosId");

                    b.Property<int?>("PosOperationId");

                    b.HasKey("Id");

                    b.HasIndex("GoodId");

                    b.HasIndex("Label")
                        .IsUnique();

                    b.HasIndex("PosId");

                    b.HasIndex("PosOperationId", "PosId");

                    b.ToTable("LabeledGoods");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Maker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.ToTable("Makers");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Pos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CityId");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("QrCode")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("8081c0fc-a73e-4397-a282-8e0a2c351330")
                        .HasMaxLength(100);

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("PointsOfSale");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.PosImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<int>("PosId");

                    b.HasKey("Id");

                    b.HasIndex("PosId");

                    b.ToTable("PosImages");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.PosOperation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PosId");

                    b.Property<int>("ApplicationUserId");

                    b.Property<DateTime?>("DateCompleted");

                    b.Property<DateTime?>("DateSentForVerification");

                    b.Property<DateTime>("DateStarted");

                    b.HasKey("Id", "PosId");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("PosId");

                    b.ToTable("PosOperations");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.UserBonus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Bonus");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UsersBonuses");
                });

            modelBuilder.Entity("NasladdinPlace.DAL.Entities.FeedbackEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AppVersion")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(3000);

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("DeviceName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("DeviceOperatingSystem")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("PosId");

                    b.Property<int>("Rating");

                    b.Property<int?>("UserId");

                    b.Property<double>("UserLatitude");

                    b.Property<double>("UserLongitude");

                    b.HasKey("Id");

                    b.HasIndex("PosId");

                    b.HasIndex("UserId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictApplication<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .IsRequired();

                    b.Property<string>("ClientSecret");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken();

                    b.Property<string>("ConsentType");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Permissions");

                    b.Property<string>("PostLogoutRedirectUris");

                    b.Property<string>("Properties");

                    b.Property<string>("RedirectUris");

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("OpenIddictApplications");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictAuthorization<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ApplicationId");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken();

                    b.Property<string>("Properties");

                    b.Property<string>("Scopes");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.ToTable("OpenIddictAuthorizations");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictScope<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken();

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Properties");

                    b.Property<string>("Resources");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("OpenIddictScopes");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictToken<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ApplicationId");

                    b.Property<int?>("AuthorizationId");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken();

                    b.Property<DateTimeOffset?>("CreationDate");

                    b.Property<DateTimeOffset?>("ExpirationDate");

                    b.Property<string>("Payload");

                    b.Property<string>("Properties");

                    b.Property<string>("ReferenceId");

                    b.Property<string>("Status");

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("AuthorizationId");

                    b.HasIndex("ReferenceId")
                        .IsUnique();

                    b.ToTable("OpenIddictTokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("NasladdinPlace.Core.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.BankTransactionInfo", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.PosOperation")
                        .WithMany("BankTransactionInfos")
                        .HasForeignKey("PosOperationId", "PosId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.City", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Good", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Maker", "Maker")
                        .WithMany("Goods")
                        .HasForeignKey("MakerId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.GoodHistoryRecord", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.LabeledGood", "LabeledGood")
                        .WithMany("GoodHistoryRecords")
                        .HasForeignKey("LabeledGoodId");

                    b.HasOne("NasladdinPlace.Core.Models.Pos", "Pos")
                        .WithMany()
                        .HasForeignKey("PosId");

                    b.HasOne("NasladdinPlace.Core.Models.PosOperation", "PosOperation")
                        .WithMany("GoodHistoryRecords")
                        .HasForeignKey("PosOperationId", "PosId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.GoodImage", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Good", "Good")
                        .WithMany("GoodImages")
                        .HasForeignKey("GoodId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.GoodPriceInPos", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Currency", "Currency")
                        .WithMany("GoodPriceInPlants")
                        .HasForeignKey("CurrencyId");

                    b.HasOne("NasladdinPlace.Core.Models.Good", "Good")
                        .WithMany("GoodPriceInPlants")
                        .HasForeignKey("GoodId");

                    b.HasOne("NasladdinPlace.Core.Models.Pos", "Pos")
                        .WithMany("GoodPricesInPointsOfSale")
                        .HasForeignKey("PosId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.LabeledGood", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Good", "Good")
                        .WithMany("LabeledGoods")
                        .HasForeignKey("GoodId");

                    b.HasOne("NasladdinPlace.Core.Models.Pos", "Pos")
                        .WithMany("LabeledGoods")
                        .HasForeignKey("PosId");

                    b.HasOne("NasladdinPlace.Core.Models.PosOperation", "PosOperation")
                        .WithMany("LabeledGoods")
                        .HasForeignKey("PosOperationId", "PosId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.Pos", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.PosImage", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Pos", "Pos")
                        .WithMany("Images")
                        .HasForeignKey("PosId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.PosOperation", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("PosOperations")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("NasladdinPlace.Core.Models.Pos", "Pos")
                        .WithMany("Operations")
                        .HasForeignKey("PosId");
                });

            modelBuilder.Entity("NasladdinPlace.Core.Models.UserBonus", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("NasladdinPlace.DAL.Entities.FeedbackEntity", b =>
                {
                    b.HasOne("NasladdinPlace.Core.Models.Pos", "Pos")
                        .WithMany()
                        .HasForeignKey("PosId");

                    b.HasOne("NasladdinPlace.Core.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictAuthorization<int>", b =>
                {
                    b.HasOne("OpenIddict.Models.OpenIddictApplication<int>", "Application")
                        .WithMany("Authorizations")
                        .HasForeignKey("ApplicationId");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictToken<int>", b =>
                {
                    b.HasOne("OpenIddict.Models.OpenIddictApplication<int>", "Application")
                        .WithMany("Tokens")
                        .HasForeignKey("ApplicationId");

                    b.HasOne("OpenIddict.Models.OpenIddictAuthorization<int>", "Authorization")
                        .WithMany("Tokens")
                        .HasForeignKey("AuthorizationId");
                });
        }
    }
}
