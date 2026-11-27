using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Infrastructure.Migrations;

/// <inheritdoc />
public partial class _20261127165845_Inicio : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "dbo");

        migrationBuilder.CreateTable(
            name: "Tenancy",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Ativo = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                RazaoSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Fantasia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Cnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                TipoTenancy = table.Column<int>(type: "int", nullable: false),
                Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Celular = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Host = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Porta = table.Column<int>(type: "int", nullable: false),
                Cep = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                Uf = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Cidade = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Endereco = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Complemento = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenancy", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AuthUsers",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Ativo = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                TenancyAtiva = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                TenancyId = table.Column<long>(type: "bigint", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                PrimeiroAcesso = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Role = table.Column<int>(type: "int", nullable: false),
                DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuthUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_AuthUsers_Tenancy_TenancyId",
                    column: x => x.TenancyId,
                    principalSchema: "dbo",
                    principalTable: "Tenancy",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_AuthUsers_PrimeiroAcesso",
            schema: "dbo",
            table: "AuthUsers",
            column: "PrimeiroAcesso");

        migrationBuilder.CreateIndex(
            name: "IX_AuthUsers_TenancyId",
            schema: "dbo",
            table: "AuthUsers",
            column: "TenancyId");

        migrationBuilder.CreateIndex(
            name: "IX_AuthUsers_UserName",
            schema: "dbo",
            table: "AuthUsers",
            column: "UserName",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AuthUsers",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "Tenancy",
            schema: "dbo");
    }
}
