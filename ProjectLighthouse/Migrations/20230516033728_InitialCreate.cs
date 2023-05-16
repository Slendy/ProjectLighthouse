using System;
using LBPUnion.ProjectLighthouse.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LBPUnion.ProjectLighthouse.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230516033728_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Key = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompletedMigrations",
                columns: table => new
                {
                    MigrationName = table.Column<string>(type: "text", nullable: false),
                    RanAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedMigrations", x => x.MigrationName);
                });

            migrationBuilder.CreateTable(
                name: "CustomCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IconHash = table.Column<string>(type: "text", nullable: true),
                    Endpoint = table.Column<string>(type: "text", nullable: true),
                    SlotIdsCollection = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomCategories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ResetToken = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.TokenId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationTokens",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationTokens", x => x.TokenId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    EmailAddress = table.Column<string>(type: "text", nullable: true),
                    EmailAddressVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: true),
                    IconHash = table.Column<string>(type: "text", nullable: true),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    LocationPacked = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Pins = table.Column<string>(type: "text", nullable: true),
                    PlanetHashLBP2 = table.Column<string>(type: "text", nullable: true),
                    PlanetHashLBP2CC = table.Column<string>(type: "text", nullable: true),
                    PlanetHashLBP3 = table.Column<string>(type: "text", nullable: true),
                    PlanetHashLBPVita = table.Column<string>(type: "text", nullable: true),
                    PasswordResetRequired = table.Column<bool>(type: "boolean", nullable: false),
                    YayHash = table.Column<string>(type: "text", nullable: true),
                    BooHash = table.Column<string>(type: "text", nullable: true),
                    MehHash = table.Column<string>(type: "text", nullable: true),
                    LastLogin = table.Column<long>(type: "bigint", nullable: false),
                    LastLogout = table.Column<long>(type: "bigint", nullable: false),
                    PermissionLevel = table.Column<int>(type: "integer", nullable: false),
                    BannedReason = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    LevelVisibility = table.Column<int>(type: "integer", nullable: false),
                    ProfileVisibility = table.Column<int>(type: "integer", nullable: false),
                    TwoFactorSecret = table.Column<string>(type: "text", nullable: true),
                    TwoFactorBackup = table.Column<string>(type: "text", nullable: true),
                    LinkedRpcnId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LinkedPsnId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    AdminGrantedSlots = table.Column<int>(type: "integer", nullable: false),
                    CommentsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WebTokens",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserToken = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebTokens", x => x.TokenId);
                });

            migrationBuilder.CreateTable(
                name: "BlockedProfiles",
                columns: table => new
                {
                    BlockedProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BlockedUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedProfiles", x => x.BlockedProfileId);
                    table.ForeignKey(
                        name: "FK_BlockedProfiles_Users_BlockedUserId",
                        column: x => x.BlockedUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockedProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    CaseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    ModeratorNotes = table.Column<string>(type: "text", nullable: false),
                    Processed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DismissedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DismisserId = table.Column<int>(type: "integer", nullable: true),
                    DismisserUsername = table.Column<string>(type: "text", nullable: true),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    CreatorUsername = table.Column<string>(type: "text", nullable: false),
                    AffectedId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.CaseId);
                    table.ForeignKey(
                        name: "FK_Cases_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cases_Users_DismisserId",
                        column: x => x.DismisserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PosterUserId = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedType = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ThumbsUp = table.Column<int>(type: "integer", nullable: false),
                    ThumbsDown = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Users_PosterUserId",
                        column: x => x.PosterUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailSetTokens",
                columns: table => new
                {
                    EmailSetTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EmailToken = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSetTokens", x => x.EmailSetTokenId);
                    table.ForeignKey(
                        name: "FK_EmailSetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerificationTokens",
                columns: table => new
                {
                    EmailVerificationTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EmailToken = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationTokens", x => x.EmailVerificationTokenId);
                    table.ForeignKey(
                        name: "FK_EmailVerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameTokens",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserToken = table.Column<string>(type: "text", nullable: true),
                    UserLocation = table.Column<string>(type: "text", nullable: true),
                    GameVersion = table.Column<int>(type: "integer", nullable: false),
                    Platform = table.Column<int>(type: "integer", nullable: false),
                    TicketHash = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_GameTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeartedProfiles",
                columns: table => new
                {
                    HeartedProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    HeartedUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeartedProfiles", x => x.HeartedProfileId);
                    table.ForeignKey(
                        name: "FK_HeartedProfiles_Users_HeartedUserId",
                        column: x => x.HeartedUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeartedProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LastContacts",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    GameVersion = table.Column<int>(type: "integer", nullable: false),
                    Platform = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastContacts", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_LastContacts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformLinkAttempts",
                columns: table => new
                {
                    PlatformLinkAttemptId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PlatformId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Platform = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    IPAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformLinkAttempts", x => x.PlatformLinkAttemptId);
                    table.ForeignKey(
                        name: "FK_PlatformLinkAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    SlotCollection = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.PlaylistId);
                    table.ForeignKey(
                        name: "FK_Playlists_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    ReportingPlayerId = table.Column<int>(type: "integer", nullable: false),
                    Players = table.Column<string>(type: "text", nullable: true),
                    GriefStateHash = table.Column<string>(type: "text", nullable: true),
                    LevelOwner = table.Column<string>(type: "text", nullable: true),
                    InitialStateHash = table.Column<string>(type: "text", nullable: true),
                    JpegHash = table.Column<string>(type: "text", nullable: true),
                    LevelId = table.Column<int>(type: "integer", nullable: false),
                    LevelType = table.Column<string>(type: "text", nullable: true),
                    Bounds = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Reports_Users_ReportingPlayerId",
                        column: x => x.ReportingPlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    SlotId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternalSlotId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IconHash = table.Column<string>(type: "text", nullable: false),
                    IsAdventurePlanet = table.Column<bool>(type: "boolean", nullable: false),
                    RootLevel = table.Column<string>(type: "text", nullable: false),
                    ResourceCollection = table.Column<string>(type: "text", nullable: false),
                    LocationPacked = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    InitiallyLocked = table.Column<bool>(type: "boolean", nullable: false),
                    SubLevel = table.Column<bool>(type: "boolean", nullable: false),
                    Lbp1Only = table.Column<bool>(type: "boolean", nullable: false),
                    Shareable = table.Column<int>(type: "integer", nullable: false),
                    AuthorLabels = table.Column<string>(type: "text", nullable: false),
                    BackgroundHash = table.Column<string>(type: "text", nullable: false),
                    MinimumPlayers = table.Column<int>(type: "integer", nullable: false),
                    MaximumPlayers = table.Column<int>(type: "integer", nullable: false),
                    MoveRequired = table.Column<bool>(type: "boolean", nullable: false),
                    FirstUploaded = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdated = table.Column<long>(type: "bigint", nullable: false),
                    TeamPick = table.Column<bool>(type: "boolean", nullable: false),
                    GameVersion = table.Column<int>(type: "integer", nullable: false),
                    LevelType = table.Column<string>(type: "text", nullable: false),
                    CrossControllerRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Hidden = table.Column<bool>(type: "boolean", nullable: false),
                    HiddenReason = table.Column<string>(type: "text", nullable: false),
                    PlaysLBP1 = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP1Complete = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP1Unique = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP2 = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP2Complete = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP2Unique = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP3 = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP3Complete = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP3Unique = table.Column<int>(type: "integer", nullable: false),
                    CommentsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.SlotId);
                    table.ForeignKey(
                        name: "FK_Slots_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatedComments",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatedComments", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_RatedComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatedComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeartedPlaylists",
                columns: table => new
                {
                    HeartedPlaylistId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PlaylistId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeartedPlaylists", x => x.HeartedPlaylistId);
                    table.ForeignKey(
                        name: "FK_HeartedPlaylists_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "PlaylistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeartedPlaylists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeartedLevels",
                columns: table => new
                {
                    HeartedLevelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SlotId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeartedLevels", x => x.HeartedLevelId);
                    table.ForeignKey(
                        name: "FK_HeartedLevels_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeartedLevels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    SmallHash = table.Column<string>(type: "text", nullable: false),
                    MediumHash = table.Column<string>(type: "text", nullable: false),
                    LargeHash = table.Column<string>(type: "text", nullable: false),
                    PlanHash = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    SlotId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_Photos_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId");
                    table.ForeignKey(
                        name: "FK_Photos_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueuedLevels",
                columns: table => new
                {
                    QueuedLevelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SlotId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedLevels", x => x.QueuedLevelId);
                    table.ForeignKey(
                        name: "FK_QueuedLevels_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueuedLevels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatedLevels",
                columns: table => new
                {
                    RatedLevelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SlotId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    RatingLBP1 = table.Column<double>(type: "double precision", nullable: false),
                    TagLBP1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatedLevels", x => x.RatedLevelId);
                    table.ForeignKey(
                        name: "FK_RatedLevels_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatedLevels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewerId = table.Column<int>(type: "integer", nullable: false),
                    SlotId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    LabelCollection = table.Column<string>(type: "text", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Thumb = table.Column<int>(type: "integer", nullable: false),
                    ThumbsUp = table.Column<int>(type: "integer", nullable: false),
                    ThumbsDown = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    ScoreId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlotId = table.Column<int>(type: "integer", nullable: false),
                    ChildSlotId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    PlayerIdCollection = table.Column<string>(type: "text", nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.ScoreId);
                    table.ForeignKey(
                        name: "FK_Scores_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitedLevels",
                columns: table => new
                {
                    VisitedLevelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SlotId = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP1 = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP2 = table.Column<int>(type: "integer", nullable: false),
                    PlaysLBP3 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitedLevels", x => x.VisitedLevelId);
                    table.ForeignKey(
                        name: "FK_VisitedLevels_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitedLevels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotoSubjects",
                columns: table => new
                {
                    PhotoSubjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PhotoId = table.Column<int>(type: "integer", nullable: false),
                    Bounds = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoSubjects", x => x.PhotoSubjectId);
                    table.ForeignKey(
                        name: "FK_PhotoSubjects_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "PhotoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoSubjects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatedReviews",
                columns: table => new
                {
                    RatedReviewId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    Thumb = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatedReviews", x => x.RatedReviewId);
                    table.ForeignKey(
                        name: "FK_RatedReviews_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatedReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockedProfiles_BlockedUserId",
                table: "BlockedProfiles",
                column: "BlockedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedProfiles_UserId",
                table: "BlockedProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CreatorId",
                table: "Cases",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_DismisserId",
                table: "Cases",
                column: "DismisserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PosterUserId",
                table: "Comments",
                column: "PosterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSetTokens_UserId",
                table: "EmailSetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_UserId",
                table: "EmailVerificationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTokens_UserId",
                table: "GameTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedLevels_SlotId",
                table: "HeartedLevels",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedLevels_UserId",
                table: "HeartedLevels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedPlaylists_PlaylistId",
                table: "HeartedPlaylists",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedPlaylists_UserId",
                table: "HeartedPlaylists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedProfiles_HeartedUserId",
                table: "HeartedProfiles",
                column: "HeartedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedProfiles_UserId",
                table: "HeartedProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CreatorId",
                table: "Photos",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_SlotId",
                table: "Photos",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoSubjects_PhotoId",
                table: "PhotoSubjects",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoSubjects_UserId",
                table: "PhotoSubjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformLinkAttempts_UserId",
                table: "PlatformLinkAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_CreatorId",
                table: "Playlists",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuedLevels_SlotId",
                table: "QueuedLevels",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuedLevels_UserId",
                table: "QueuedLevels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RatedComments_CommentId",
                table: "RatedComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_RatedComments_UserId",
                table: "RatedComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RatedLevels_SlotId",
                table: "RatedLevels",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_RatedLevels_UserId",
                table: "RatedLevels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RatedReviews_ReviewId",
                table: "RatedReviews",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_RatedReviews_UserId",
                table: "RatedReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportingPlayerId",
                table: "Reports",
                column: "ReportingPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SlotId",
                table: "Reviews",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_SlotId",
                table: "Scores",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_CreatorId",
                table: "Slots",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedLevels_SlotId",
                table: "VisitedLevels",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedLevels_UserId",
                table: "VisitedLevels",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIKeys");

            migrationBuilder.DropTable(
                name: "BlockedProfiles");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "CompletedMigrations");

            migrationBuilder.DropTable(
                name: "CustomCategories");

            migrationBuilder.DropTable(
                name: "EmailSetTokens");

            migrationBuilder.DropTable(
                name: "EmailVerificationTokens");

            migrationBuilder.DropTable(
                name: "GameTokens");

            migrationBuilder.DropTable(
                name: "HeartedLevels");

            migrationBuilder.DropTable(
                name: "HeartedPlaylists");

            migrationBuilder.DropTable(
                name: "HeartedProfiles");

            migrationBuilder.DropTable(
                name: "LastContacts");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "PhotoSubjects");

            migrationBuilder.DropTable(
                name: "PlatformLinkAttempts");

            migrationBuilder.DropTable(
                name: "QueuedLevels");

            migrationBuilder.DropTable(
                name: "RatedComments");

            migrationBuilder.DropTable(
                name: "RatedLevels");

            migrationBuilder.DropTable(
                name: "RatedReviews");

            migrationBuilder.DropTable(
                name: "RegistrationTokens");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "VisitedLevels");

            migrationBuilder.DropTable(
                name: "WebTokens");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
