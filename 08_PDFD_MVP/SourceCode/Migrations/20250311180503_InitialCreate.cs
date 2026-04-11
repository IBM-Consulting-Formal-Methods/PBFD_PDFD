using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VisitorLog_PDFD.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NameTypes",
                columns: table => new
                {
                    NameTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameTypes", x => x.NameTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "Continents",
                columns: table => new
                {
                    ContinentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continents", x => x.ContinentId);
                    table.ForeignKey(
                        name: "FK_Continents_NameTypes_NameTypeId",
                        column: x => x.NameTypeId,
                        principalTable: "NameTypes",
                        principalColumn: "NameTypeId");
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContinentId = table.Column<int>(type: "int", nullable: false),
                    NameTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                    table.ForeignKey(
                        name: "FK_Countries_Continents_ContinentId",
                        column: x => x.ContinentId,
                        principalTable: "Continents",
                        principalColumn: "ContinentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Countries_NameTypes_NameTypeId",
                        column: x => x.NameTypeId,
                        principalTable: "NameTypes",
                        principalColumn: "NameTypeId");
                });

            migrationBuilder.CreateTable(
                name: "SelectedContinents",
                columns: table => new
                {
                    SelectedContinentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    ContinentId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedContinents", x => x.SelectedContinentId);
                    table.ForeignKey(
                        name: "FK_SelectedContinents_Continents_ContinentId",
                        column: x => x.ContinentId,
                        principalTable: "Continents",
                        principalColumn: "ContinentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedContinents_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    NameTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.StateId);
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_States_NameTypes_NameTypeId",
                        column: x => x.NameTypeId,
                        principalTable: "NameTypes",
                        principalColumn: "NameTypeId");
                });

            migrationBuilder.CreateTable(
                name: "SelectedCountries",
                columns: table => new
                {
                    SelectedCountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelectedContinentId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedCountries", x => x.SelectedCountryId);
                    table.ForeignKey(
                        name: "FK_SelectedCountries_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                    table.ForeignKey(
                        name: "FK_SelectedCountries_SelectedContinents_SelectedContinentId",
                        column: x => x.SelectedContinentId,
                        principalTable: "SelectedContinents",
                        principalColumn: "SelectedContinentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    CountyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    NameTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.CountyId);
                    table.ForeignKey(
                        name: "FK_Counties_NameTypes_NameTypeId",
                        column: x => x.NameTypeId,
                        principalTable: "NameTypes",
                        principalColumn: "NameTypeId");
                    table.ForeignKey(
                        name: "FK_Counties_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedStates",
                columns: table => new
                {
                    SelectedStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelectedCountryId = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedStates", x => x.SelectedStateId);
                    table.ForeignKey(
                        name: "FK_SelectedStates_SelectedCountries_SelectedCountryId",
                        column: x => x.SelectedCountryId,
                        principalTable: "SelectedCountries",
                        principalColumn: "SelectedCountryId");
                    table.ForeignKey(
                        name: "FK_SelectedStates_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "StateId");
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountyId = table.Column<int>(type: "int", nullable: false),
                    NameTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Cities_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "CountyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cities_NameTypes_NameTypeId",
                        column: x => x.NameTypeId,
                        principalTable: "NameTypes",
                        principalColumn: "NameTypeId");
                });

            migrationBuilder.CreateTable(
                name: "SelectedCounties",
                columns: table => new
                {
                    SelectedCountyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelectedStateId = table.Column<int>(type: "int", nullable: false),
                    CountyId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedCounties", x => x.SelectedCountyId);
                    table.ForeignKey(
                        name: "FK_SelectedCounties_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "CountyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedCounties_SelectedStates_SelectedStateId",
                        column: x => x.SelectedStateId,
                        principalTable: "SelectedStates",
                        principalColumn: "SelectedStateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedCities",
                columns: table => new
                {
                    SelectedCityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelectedCountyId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedCities", x => x.SelectedCityId);
                    table.ForeignKey(
                        name: "FK_SelectedCities_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedCities_SelectedCounties_SelectedCountyId",
                        column: x => x.SelectedCountyId,
                        principalTable: "SelectedCounties",
                        principalColumn: "SelectedCountyId");
                });

            migrationBuilder.InsertData(
                table: "NameTypes",
                columns: new[] { "NameTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Continent" },
                    { 2, "Country" },
                    { 3, "State" },
                    { 4, "County" },
                    { 5, "City" },
                    { 6, "District" },
                    { 7, "Province" },
                    { 8, "Station" },
                    { 9, "Special Administrative Region" },
                    { 10, "Separate Political Entity" },
                    { 11, "Region" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Email", "FirstName", "LastName", "MiddleName" },
                values: new object[] { 1, "tester@test.com", "Test", "Tester", "T" });

            migrationBuilder.InsertData(
                table: "Continents",
                columns: new[] { "ContinentId", "Name", "NameTypeId" },
                values: new object[,]
                {
                    { 1, "Asia", 1 },
                    { 2, "North America", 1 }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "ContinentId", "Name", "NameTypeId" },
                values: new object[,]
                {
                    { 1, 2, "USA", 2 },
                    { 2, 2, "Canada", 2 }
                });

            migrationBuilder.InsertData(
                table: "States",
                columns: new[] { "StateId", "CountryId", "Name", "NameTypeId" },
                values: new object[,]
                {
                    { 1, 1, "Maryland", 3 },
                    { 2, 1, "Viginia", 3 }
                });

            migrationBuilder.InsertData(
                table: "Counties",
                columns: new[] { "CountyId", "Name", "NameTypeId", "StateId" },
                values: new object[,]
                {
                    { 1, "Howard", 4, 1 },
                    { 2, "Boltimore", 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "CityId", "CountyId", "Name", "NameTypeId" },
                values: new object[,]
                {
                    { 1, 1, "Ellicott City", 5 },
                    { 2, 1, "Columbia", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountyId",
                table: "Cities",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_NameTypeId",
                table: "Cities",
                column: "NameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Continents_NameTypeId",
                table: "Continents",
                column: "NameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_NameTypeId",
                table: "Counties",
                column: "NameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_StateId",
                table: "Counties",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ContinentId",
                table: "Countries",
                column: "ContinentId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_NameTypeId",
                table: "Countries",
                column: "NameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedCities_CityId",
                table: "SelectedCities",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedCities_SelectedCountyId",
                table: "SelectedCities",
                column: "SelectedCountyId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedContinents_ContinentId",
                table: "SelectedContinents",
                column: "ContinentId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedContinents_PersonId",
                table: "SelectedContinents",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedCounties_CountyId",
                table: "SelectedCounties",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedCounties_SelectedStateId",
                table: "SelectedCounties",
                column: "SelectedStateId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedCountries_CountryId",
                table: "SelectedCountries",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedCountries_SelectedContinentId",
                table: "SelectedCountries",
                column: "SelectedContinentId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedStates_SelectedCountryId",
                table: "SelectedStates",
                column: "SelectedCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedStates_StateId",
                table: "SelectedStates",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                table: "States",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_States_NameTypeId",
                table: "States",
                column: "NameTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelectedCities");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "SelectedCounties");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "SelectedStates");

            migrationBuilder.DropTable(
                name: "SelectedCountries");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "SelectedContinents");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Continents");

            migrationBuilder.DropTable(
                name: "NameTypes");
        }
    }
}
