using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class InitialFinalSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProfilFotografiYolu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Durumlar",
                columns: table => new
                {
                    DurumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Durumlar", x => x.DurumID);
                });

            migrationBuilder.CreateTable(
                name: "Etiketler",
                columns: table => new
                {
                    EtiketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EtiketAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiketler", x => x.EtiketID);
                });

            migrationBuilder.CreateTable(
                name: "Rozetler",
                columns: table => new
                {
                    RozetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RozetAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ikon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rozetler", x => x.RozetID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AktiviteKayitlari",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Islem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IcerikID = table.Column<int>(type: "int", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AktiviteKayitlari", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_AktiviteKayitlari_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    BildirimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GorulduMu = table.Column<bool>(type: "bit", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.BildirimID);
                    table.ForeignKey(
                        name: "FK_Bildirimler_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mesajlar",
                columns: table => new
                {
                    MesajID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GonderimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GonderenID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AliciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesajlar", x => x.MesajID);
                    table.ForeignKey(
                        name: "FK_Mesajlar_AspNetUsers_AliciID",
                        column: x => x.AliciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mesajlar_AspNetUsers_GonderenID",
                        column: x => x.GonderenID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Oneriler",
                columns: table => new
                {
                    OneriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IcerikTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OnerilenIcerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Neden = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oneriler", x => x.OneriID);
                    table.ForeignKey(
                        name: "FK_Oneriler_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Paylasimlar",
                columns: table => new
                {
                    PaylasimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaylasimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BegeniSayisi = table.Column<int>(type: "int", nullable: false),
                    YorumSayisi = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KaynakTipi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaynakID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paylasimlar", x => x.PaylasimID);
                    table.ForeignKey(
                        name: "FK_Paylasimlar_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Filmler",
                columns: table => new
                {
                    FilmID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yonetmen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yil = table.Column<int>(type: "int", nullable: true),
                    PosterYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciPuani = table.Column<int>(type: "int", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurumID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filmler", x => x.FilmID);
                    table.ForeignKey(
                        name: "FK_Filmler_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Filmler_Durumlar_DurumID",
                        column: x => x.DurumID,
                        principalTable: "Durumlar",
                        principalColumn: "DurumID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kitaplar",
                columns: table => new
                {
                    KitapID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yazar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KapakResmiYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciPuani = table.Column<int>(type: "int", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurumID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitaplar", x => x.KitapID);
                    table.ForeignKey(
                        name: "FK_Kitaplar_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kitaplar_Durumlar_DurumID",
                        column: x => x.DurumID,
                        principalTable: "Durumlar",
                        principalColumn: "DurumID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Muzikler",
                columns: table => new
                {
                    MuzikID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sanatci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Album = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yil = table.Column<int>(type: "int", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurumID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muzikler", x => x.MuzikID);
                    table.ForeignKey(
                        name: "FK_Muzikler_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Muzikler_Durumlar_DurumID",
                        column: x => x.DurumID,
                        principalTable: "Durumlar",
                        principalColumn: "DurumID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seyahatler",
                columns: table => new
                {
                    SeyahatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sehir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ulke = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeziNotu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZiyaretTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Enlem = table.Column<double>(type: "float", nullable: true),
                    Boylam = table.Column<double>(type: "float", nullable: true),
                    DurumID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seyahatler", x => x.SeyahatID);
                    table.ForeignKey(
                        name: "FK_Seyahatler_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seyahatler_Durumlar_DurumID",
                        column: x => x.DurumID,
                        principalTable: "Durumlar",
                        principalColumn: "DurumID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EtiketliIcerikler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IcerikTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IcerikID = table.Column<int>(type: "int", nullable: false),
                    EtiketID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtiketliIcerikler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EtiketliIcerikler_Etiketler_EtiketID",
                        column: x => x.EtiketID,
                        principalTable: "Etiketler",
                        principalColumn: "EtiketID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciRozetleri",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VerilmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RozetID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciRozetleri", x => x.ID);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_Rozetler_RozetID",
                        column: x => x.RozetID,
                        principalTable: "Rozetler",
                        principalColumn: "RozetID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Begeniler",
                columns: table => new
                {
                    BegeniID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BegeniTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaylasimID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Begeniler", x => x.BegeniID);
                    table.ForeignKey(
                        name: "FK_Begeniler_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Begeniler_Paylasimlar_PaylasimID",
                        column: x => x.PaylasimID,
                        principalTable: "Paylasimlar",
                        principalColumn: "PaylasimID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Yorumlar",
                columns: table => new
                {
                    YorumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metin = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaylasimID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yorumlar", x => x.YorumID);
                    table.ForeignKey(
                        name: "FK_Yorumlar_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yorumlar_Paylasimlar_PaylasimID",
                        column: x => x.PaylasimID,
                        principalTable: "Paylasimlar",
                        principalColumn: "PaylasimID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GezilecekYerler",
                columns: table => new
                {
                    YerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MekanAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotografYolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KonumKoordinati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeyahatID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GezilecekYerler", x => x.YerID);
                    table.ForeignKey(
                        name: "FK_GezilecekYerler_Seyahatler_SeyahatID",
                        column: x => x.SeyahatID,
                        principalTable: "Seyahatler",
                        principalColumn: "SeyahatID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AktiviteKayitlari_KullaniciID",
                table: "AktiviteKayitlari",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Begeniler_KullaniciID",
                table: "Begeniler",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Begeniler_PaylasimID",
                table: "Begeniler",
                column: "PaylasimID");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_KullaniciID",
                table: "Bildirimler",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_EtiketliIcerikler_EtiketID",
                table: "EtiketliIcerikler",
                column: "EtiketID");

            migrationBuilder.CreateIndex(
                name: "IX_Filmler_DurumID",
                table: "Filmler",
                column: "DurumID");

            migrationBuilder.CreateIndex(
                name: "IX_Filmler_KullaniciID",
                table: "Filmler",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_GezilecekYerler_SeyahatID",
                table: "GezilecekYerler",
                column: "SeyahatID");

            migrationBuilder.CreateIndex(
                name: "IX_Kitaplar_DurumID",
                table: "Kitaplar",
                column: "DurumID");

            migrationBuilder.CreateIndex(
                name: "IX_Kitaplar_KullaniciID",
                table: "Kitaplar",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_KullaniciID",
                table: "KullaniciRozetleri",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_RozetID",
                table: "KullaniciRozetleri",
                column: "RozetID");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_AliciID",
                table: "Mesajlar",
                column: "AliciID");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_GonderenID",
                table: "Mesajlar",
                column: "GonderenID");

            migrationBuilder.CreateIndex(
                name: "IX_Muzikler_DurumID",
                table: "Muzikler",
                column: "DurumID");

            migrationBuilder.CreateIndex(
                name: "IX_Muzikler_KullaniciID",
                table: "Muzikler",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Oneriler_KullaniciID",
                table: "Oneriler",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Paylasimlar_KullaniciID",
                table: "Paylasimlar",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Seyahatler_DurumID",
                table: "Seyahatler",
                column: "DurumID");

            migrationBuilder.CreateIndex(
                name: "IX_Seyahatler_KullaniciID",
                table: "Seyahatler",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_KullaniciID",
                table: "Yorumlar",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_PaylasimID",
                table: "Yorumlar",
                column: "PaylasimID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AktiviteKayitlari");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Begeniler");

            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "EtiketliIcerikler");

            migrationBuilder.DropTable(
                name: "Filmler");

            migrationBuilder.DropTable(
                name: "GezilecekYerler");

            migrationBuilder.DropTable(
                name: "Kitaplar");

            migrationBuilder.DropTable(
                name: "KullaniciRozetleri");

            migrationBuilder.DropTable(
                name: "Mesajlar");

            migrationBuilder.DropTable(
                name: "Muzikler");

            migrationBuilder.DropTable(
                name: "Oneriler");

            migrationBuilder.DropTable(
                name: "Yorumlar");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Etiketler");

            migrationBuilder.DropTable(
                name: "Seyahatler");

            migrationBuilder.DropTable(
                name: "Rozetler");

            migrationBuilder.DropTable(
                name: "Paylasimlar");

            migrationBuilder.DropTable(
                name: "Durumlar");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
