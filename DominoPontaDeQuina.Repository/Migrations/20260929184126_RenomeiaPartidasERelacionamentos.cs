using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominoPontaDeQuina.Repository.Migrations;

public partial class RenomeiaPartidasERelacionamentos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(
            name: "Jogos",
            newName: "Partidas");

        migrationBuilder.RenameTable(
            name: "ParticipacoesJogo",
            newName: "ParticipacoesPartida");

        migrationBuilder.RenameColumn(
            name: "JogoId",
            table: "ParticipacoesPartida",
            newName: "PartidaId");

        migrationBuilder.DropIndex(
            name: "IX_ParticipacoesJogo_JogoId",
            table: "ParticipacoesPartida");

        migrationBuilder.DropIndex(
            name: "IX_ParticipacoesJogo_JogadorId",
            table: "ParticipacoesPartida");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipacoesPartida_JogadorId",
            table: "ParticipacoesPartida",
            column: "JogadorId");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipacoesPartida_PartidaId_JogadorId",
            table: "ParticipacoesPartida",
            columns: new[] { "PartidaId", "JogadorId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_ParticipacoesPartida_PartidaId_JogadorId",
            table: "ParticipacoesPartida");

        migrationBuilder.DropIndex(
            name: "IX_ParticipacoesPartida_JogadorId",
            table: "ParticipacoesPartida");

        migrationBuilder.RenameColumn(
            name: "PartidaId",
            table: "ParticipacoesPartida",
            newName: "JogoId");

        migrationBuilder.RenameTable(
            name: "ParticipacoesPartida",
            newName: "ParticipacoesJogo");

        migrationBuilder.RenameTable(
            name: "Partidas",
            newName: "Jogos");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipacoesJogo_JogadorId",
            table: "ParticipacoesJogo",
            column: "JogadorId");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipacoesJogo_JogoId",
            table: "ParticipacoesJogo",
            column: "JogoId");
    }
}
