using System;

namespace LBPUnion.ProjectLighthouse.Types.Roles;

[Flags]
public enum Entitlements : ulong
{
    None = 0,

    // General Permissions
    Banned = 1ul << 0,
    ShowInUsers = 1ul << 1,
    Login = 1ul << 2,
    PublishLevel = 1ul << 3,
    UploadResource = 1ul << 4,
    PostComment = 1ul << 5,
    PostReview = 1ul << 6,
    PostPhoto = 1ul << 7,
    RateLevel = 1ul << 8,
    RateReview = 1ul << 9,
    RateComment = 1ul << 10,
    UpdateBiography = 1ul << 11,
    UpdateProfilePicture = 1ul << 12,
    SubmitReport = 1ul << 13,
    CreatePlaylist = 1ul << 14,
    UpdatePlaylist = 1ul << 15,
    SubmitScore = 1ul << 16,
    UnpublishLevel = 1ul << 17,
    DeleteComment = 1ul << 18,
    DeletePhoto = 1ul << 19,
    UpdatePlanetDecoration = 1ul << 20,
    MatchMake = 1ul << 21,

    // First 18 bits set to one except for the banned bit,
    // this is easier than manually OR'ing all the fields
    Default = 0b111111111111111110,

    RequireTwoFactor = 1ul << 31,

    // Management permissions
    RunCommands = 1ul << 32,
    RunMaintenanceJobs = 1ul << 33,
    GenerateApiKeys = 1ul << 34,
    ModifyUserSettings = 1ul << 35,
    ManagePhotos = 1ul << 36,
    ManageComments = 1ul << 37,
    ModifyLevelSettings = 1ul << 38,
    ManageLevels = 1ul << 39,
    ManageUsers = 1ul << 40,

    Moderator = RunCommands |
                RunMaintenanceJobs |
                GenerateApiKeys |
                ModifyUserSettings |
                ManagePhotos |
                ManageComments |
                ModifyLevelSettings |
                ManageLevels |
                ManageUsers,

    // Admin has every permission except for Banned
    Admin = ulong.MaxValue & ~Banned,

}