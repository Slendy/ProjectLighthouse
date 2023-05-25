using System;

namespace LBPUnion.ProjectLighthouse.Types.Roles;

[Flags]
public enum Entitlements : ulong
{
    None = 0,

    // General Permissions
    Banned = 1ul << 0,
    DisplayInUsers = 1ul << 1,
    Login = 1ul << 2,
    UploadLevels = 1ul << 3,
    UploadResources = 1ul << 4,
    PostComments = 1ul << 5,
    PostReviews = 1ul << 6,
    RateLevels = 1ul << 7,
    RateReviews = 1ul << 8,
    RateComments = 1ul << 9,
    UpdateBiography = 1ul << 10,
    UpdateProfilePicture = 1ul << 11,
    SubmitReport = 1ul << 12,
    CreatePlaylist = 1ul << 13,
    UpdatePlaylist = 1ul << 14,
    SubmitScore = 1ul << 15,
    UnpublishLevels = 1ul << 16,
    DeleteComment = 1ul << 17,
    DeletePhoto = 1ul << 18,

    // First 18 bits set to one except for the banned bit,
    // this is easier than manually OR'ing all the fields
    Default = 0b111111111111111110,

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