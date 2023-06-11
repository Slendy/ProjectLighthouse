using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Misc;
using LBPUnion.ProjectLighthouse.Types.Roles;
using LBPUnion.ProjectLighthouse.Types.Users;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Profile;

public class UserEntity
{

    #nullable enable
    [Key]
    public int UserId { get; set; }

    public string Username { get; set; } = "";

    #nullable enable
    public string? EmailAddress { get; set; }
    #nullable disable

    public bool EmailAddressVerified { get; set; }

    public string Password { get; set; }

    public string IconHash { get; set; }

    /// <summary>
    ///     A user-customizable biography shown on the profile card
    /// </summary>
    public string Biography { get; set; }

    [NotMapped]
    public string WebsiteAvatarHash {
        get {
            string avatarHash = this.IconHash;

            if (string.IsNullOrWhiteSpace(avatarHash) || this.IconHash.StartsWith('g')) avatarHash = this.YayHash;
            if (string.IsNullOrWhiteSpace(avatarHash)) avatarHash = this.MehHash;
            if (string.IsNullOrWhiteSpace(avatarHash)) avatarHash = this.BooHash;
            if (string.IsNullOrWhiteSpace(avatarHash)) avatarHash = ServerConfiguration.Instance.WebsiteConfiguration.MissingIconHash;

            return avatarHash;
        }
    }

    public UserStatus GetStatus(DatabaseContext database) => new(database, this.UserId);
    public int GetHeartCount(DatabaseContext database) => database.HeartedProfiles.Count(hp => hp.HeartedUserId == this.UserId);
    public int GetCommentCount(DatabaseContext database) => database.Comments.Count(c => c.TargetId == this.UserId && c.Type == CommentType.Profile);
    public int GetUsedSlotCount(DatabaseContext database) => database.Slots.Count(s => s.CreatorId == this.UserId);
    public int GetUploadedPhotoCount(DatabaseContext database) => database.Photos.Count(p => p.CreatorId == this.UserId);

    /// <summary>
    ///     The location of the profile card on the user's earth
    ///     Stored as a single 64 bit unsigned integer but split into
    ///     2 unsigned 32 bit integers
    /// </summary>
    public ulong LocationPacked { get; set; }

    [NotMapped]
    public Location Location
    {
        get =>
            new()
            {
                X = (int)(this.LocationPacked >> 32),
                Y = (int)this.LocationPacked,
            };
        set => this.LocationPacked = (ulong)value.X << 32 | (uint)value.Y;
    }

    public string Pins { get; set; } = "";

    public string PlanetHashLBP2 { get; set; } = "";

    public string PlanetHashLBP2CC { get; set; } = "";

    public string PlanetHashLBP3 { get; set; } = "";

    public string PlanetHashLBPVita { get; set; } = "";

    public bool PasswordResetRequired { get; set; }

    public string YayHash { get; set; } = "";
    public string BooHash { get; set; } = "";
    public string MehHash { get; set; } = "";

    public long LastLogin { get; set; }
    public long LastLogout { get; set; }

    public ICollection<UserRoleEntity> Roles { get; set; } = new HashSet<UserRoleEntity>();

    /// <summary>
    /// This field by default is a a cache for a user's entitlements given by their roles
    /// <p>
    ///     It can also be used to override a singular users permissions, i.e. if someone is abusing a certain feature
    ///     then their access can be revoked
    /// </p>
    /// </summary>
    public Entitlements Permissions { get; set; } = Entitlements.Default;

    public bool IsModerator => (this.Permissions & Entitlements.Admin) != 0;

    public bool IsAdmin => this.Permissions == Entitlements.Admin;

    public bool IsBanned => this.HasPermission(Entitlements.Banned);

    public bool HasPermission(Entitlements entitlements) => (this.Permissions & entitlements) == entitlements;

    #nullable enable
    public string? BannedReason { get; set; }
    #nullable disable

    public string Language { get; set; } = "en";

    public string TimeZone { get; set; } = TimeZoneInfo.Local.Id;

    public PrivacyType LevelVisibility { get; set; } = PrivacyType.All;

    public PrivacyType ProfileVisibility { get; set; } = PrivacyType.All;

    public bool TwoFactorRequired => ServerConfiguration.Instance.TwoFactorConfiguration.RequireTwoFactor && this.HasPermission(Entitlements.RequireTwoFactor);

    public bool IsTwoFactorSetup => this.TwoFactorBackup?.Length > 0 && this.TwoFactorSecret?.Length > 0;

    public string TwoFactorSecret { get; set; } = "";

    public string TwoFactorBackup { get; set; } = "";

    public ulong LinkedRpcnId { get; set; }

    public ulong LinkedPsnId { get; set; }

    public int AdminGrantedSlots { get; set; }

    public int EntitledSlots => ServerConfiguration.Instance.UserGeneratedContentLimits.EntitledSlots + this.AdminGrantedSlots;

    // should not be adjustable by user
    public bool CommentsEnabled { get; set; } = true;

    #nullable enable
    public override bool Equals(object? obj)
    {
        if (obj is UserEntity user) return user.UserId == this.UserId;

        return false;
    }

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public static bool operator ==(UserEntity? user1, UserEntity? user2)
    {
        if (ReferenceEquals(user1, user2)) return true;
        if ((object?)user1 == null || (object?)user2 == null) return false;

        return user1.UserId == user2.UserId;
    }
    public static bool operator !=(UserEntity? user1, UserEntity? user2) => !(user1 == user2);

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => this.UserId;
    #nullable disable
}