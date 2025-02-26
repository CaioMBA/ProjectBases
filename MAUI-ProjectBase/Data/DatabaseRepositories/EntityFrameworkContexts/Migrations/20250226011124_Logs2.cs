using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.DatabaseRepositories.EntityFrameworkContexts.Migrations
{
    /// <inheritdoc />
    public partial class Logs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAt",
                table: "Logs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAt",
                table: "Logs");
        }
    }
}
