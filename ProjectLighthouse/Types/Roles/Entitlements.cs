using System;

namespace LBPUnion.ProjectLighthouse.Types.Roles;

[Flags]
public enum Entitlements : ulong
{
    // General Permissions
    Banned = 1ul << 1,
    DisplayInUsers = 1ul << 2,
    Login = 1ul << 3,
    UploadLevels = 1ul << 4,
    UploadResources = 1ul << 5,
    PostComments = 1ul << 6,
    PostReviews = 1ul << 7,
    RateLevels = 1ul << 8,
    RateReviews = 1ul << 9,
    RateComments = 1ul << 10,
    UpdateBiography = 1ul << 11,
    UpdateProfilePicture = 1ul << 12,
    SubmitReport = 1ul << 13,
    CreatePlaylist = 1ul << 14,
    UpdatePlaylist = 1ul << 15,
    SubmitScore = 1ul << 16,
    UnpublishLevels = 1ul << 17,
    DeleteComment = 1ul << 18,
    DeletePhoto = 1ul << 19,

    // Management permissions
    RunCommands = 1ul << 32,
    RunMaintenanceJobs = 1ul << 33,
    GenerateApiKeys = 1ul << 34,
    ModifyUserSettings = 1ul << 35,
    ManagePhotos = 1ul << 36,
    ManageComments = 1ul << 37,
    ModifyLevelSettings = 1ul << 38,
    ManageLevels = 1ul << 39,

}