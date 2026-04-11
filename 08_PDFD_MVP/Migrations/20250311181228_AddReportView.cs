using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorLog_PDFD.Migrations
{
    /// <inheritdoc />
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
}
