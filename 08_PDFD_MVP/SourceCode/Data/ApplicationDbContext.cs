using Microsoft.EntityFrameworkCore;
using VisitorLog_PDFD.Models;

namespace VisitorLog_PDFD.Data // Ensure this namespace matches your project's folder structure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Continent> Continents { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<SelectedContinent> SelectedContinents { get; set; }
        public DbSet<SelectedCountry> SelectedCountries { get; set; }
        public DbSet<SelectedState> SelectedStates { get; set; }
        public DbSet<SelectedCounty> SelectedCounties { get; set; }
        public DbSet<SelectedCity> SelectedCities { get; set; }
        public DbSet<NameType> NameTypes { get; set; }
        public DbSet<Report> Reports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasData(
                new Person { PersonId = 1, FirstName = "Test", MiddleName = "T", LastName = "Tester", Email = "tester@test.com" }
                );

            modelBuilder.Entity<Continent>().HasData(
                new Continent { ContinentId = 1, Name = "Asia", NameTypeId=1 },
                new Continent { ContinentId = 2, Name = "North America", NameTypeId = 1 }
            );

            modelBuilder.Entity<Country>().HasData(
                new Country { CountryId = 1, Name = "USA", ContinentId = 2, NameTypeId = 2 },
                new Country { CountryId = 2, Name = "Canada", ContinentId = 2, NameTypeId = 2 }
            );

            // Seeding States
            modelBuilder.Entity<State>().HasData(
                new State { StateId = 1, Name = "Maryland", CountryId = 1, NameTypeId = 3 },
                new State { StateId = 2, Name = "Virginia", CountryId = 1 , NameTypeId = 3 }
            );

            // Seeding Counties
            modelBuilder.Entity<County>().HasData(
                new County { CountyId = 1, Name = "Howard", StateId = 1 , NameTypeId = 4 },
                new County { CountyId = 2, Name = "Baltimore", StateId = 1 , NameTypeId = 4 }
            );

            // Seeding Cities
            modelBuilder.Entity<City>().HasData(
                new City { CityId = 1, Name = "Ellicott City", CountyId = 1 , NameTypeId = 5 },
                new City { CityId = 2, Name = "Columbia", CountyId = 1 , NameTypeId = 5 }
            );

            SeedNameType(modelBuilder);
            DisableNameTypeCascadeDelete(modelBuilder);

            // Configure the required constraint using Fluent API
            modelBuilder.Entity<State>()
                .HasOne(s => s.Country)
                .WithMany(c => c.States) // Explicit navigation property
                .HasForeignKey(s => s.CountryId)
                .IsRequired();
            // Prevent cascade delete
            modelBuilder.Entity<SelectedCountry>().HasOne(p => p.Country).WithMany().HasForeignKey(p => p.CountryId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SelectedState>().HasOne(p => p.SelectedCountry).WithMany().HasForeignKey(p => p.SelectedCountryId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SelectedState>().HasOne(p => p.State).WithMany().HasForeignKey(p => p.StateId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SelectedCity>().HasOne(p => p.SelectedCounty).WithMany().HasForeignKey(p => p.SelectedCountyId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SelectedCity>().HasOne(p => p.City).WithMany().HasForeignKey(p => p.CityId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasNoKey(); // Views don't have a primary key
                entity.ToView("vw_Report");
            });


            /* Execute custom SQL for the view
                1. run: dotnet ef migrations add AddReportView
                2. edit YYYYMMDDHHMMSS_AddReportView.cs in your Migrations folder
                    public partial class AddReportView : Migration
                    {
                        protected override void Up(MigrationBuilder migrationBuilder)
                                    {
                                migrationBuilder.Sql(@"
                                    CREATE VIEW vw_Report AS
                                        SELECT 
                                            p.FirstName + ' ' + MiddleName + ' ' + LastName AS PersonName,
                                            cp.ContinentName,
                                            ct.CountryName,
                                            sn.StateName,
                                            sc.CountyName,
                                            st.CityName
                                        FROM 
                                            Persons p
                                        LEFT JOIN 
                                            (select PersonId, c.ContinentId, c.Name + ' - ' + n.name as ContinentName from SelectedContinents sc inner join Continents c ON sc.ContinentId = c.ContinentId and isnull(sc.IsDeleted,0)=0
	                                        inner join NameTypes n on n.nametypeid=c.NameTypeId) cp on p.PersonId = cp.PersonId 
                                        LEFT JOIN 
                                            ( select ContinentId, c.countryid, c.Name +' - '+ n.name as countryname from SelectedCountries sc inner join Countries c ON sc.CountryId = c.CountryId and isnull(sc.IsDeleted,0)=0
	                                        inner join NameTypes n on n.nametypeid=c.NameTypeId) ct on ct.continentid = cp.continentid
                                        LEFT JOIN 
                                            ( select countryid, s.stateid, s.Name +' - '+ n.name as statename from SelectedStates ss inner join States s ON ss.StateId = s.StateId and isnull(ss.IsDeleted,0)=0
	                                        inner join NameTypes n on n.nametypeid=s.NameTypeId) sn on sn.countryid = ct.countryid
                                        LEFT JOIN 
                                            ( select stateid, c.countyid, c.Name +' - '+ n.name as CountyName from SelectedCounties sc inner join Counties c ON sc.CountyId = c.CountyId and isnull(sc.IsDeleted,0)=0
	                                        inner join NameTypes n on n.nametypeid=c.NameTypeId) sc on sn.stateid = sc.StateId
                                        LEFT JOIN 
                                            ( select countyid, c.cityid, c.Name +' - '+ n.name as CityName from SelectedCities sc inner join Cities c ON sc.CityId = c.CityId and isnull(sc.IsDeleted,0)=0
	                                        inner join NameTypes n on n.nametypeid=c.NameTypeId) st on st.CountyId = sc.CountyId
                                ");
                            }

                            protected override void Down(MigrationBuilder migrationBuilder)
                            {
                                migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Report;");
                            }
                        }
                3. apply the migration: dotnet ef database update
             */
        }

        private void SeedNameType(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NameType>().HasData(
                new NameType { NameTypeId = 1, Name = "Continent" }, 
                new NameType { NameTypeId = 2, Name = "Country" }, 
                new NameType { NameTypeId = 3, Name = "State" }, 
                new NameType { NameTypeId = 4, Name = "County" }, 
                new NameType { NameTypeId = 5, Name = "City" }, 
                new NameType { NameTypeId = 6, Name = "District" }, 
                new NameType { NameTypeId = 7, Name = "Province" }, 
                new NameType { NameTypeId = 8, Name = "Station" },
                new NameType { NameTypeId = 9, Name = "Special Administrative Region" },
                new NameType { NameTypeId = 10, Name = "Separate Political Entity" },
                new NameType { NameTypeId = 11, Name = "Region" }
            );
        }

        private void DisableNameTypeCascadeDelete(ModelBuilder modelBuilder)
        {
            // Continent configuration
            modelBuilder.Entity<Continent>() .HasOne(c => c.NameType) .WithMany() .HasForeignKey(c => c.NameTypeId) .OnDelete(DeleteBehavior.NoAction); 
            // Country configuration
            modelBuilder.Entity<Country>() .HasOne(c => c.NameType) .WithMany() .HasForeignKey(c => c.NameTypeId) .OnDelete(DeleteBehavior.NoAction); 
            // State configuration
            modelBuilder.Entity<State>() .HasOne(s => s.NameType) .WithMany() .HasForeignKey(s => s.NameTypeId) .OnDelete(DeleteBehavior.NoAction);
            // County configuration
            modelBuilder.Entity<County>() .HasOne(c => c.NameType) .WithMany() .HasForeignKey(c => c.NameTypeId) .OnDelete(DeleteBehavior.NoAction); 
            // City configuration
            modelBuilder.Entity<City>() .HasOne(c => c.NameType) .WithMany() .HasForeignKey(c => c.NameTypeId) .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
