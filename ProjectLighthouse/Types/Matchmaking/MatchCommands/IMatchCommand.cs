#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

public interface IMatchCommand
{
    public Task<IActionResult> ProcessCommand(DatabaseContext database, IRoomService roomService, UserEntity user, RoomCommandData commandData);

    public static IActionResult CreateResponse(int statusCode, MatchResponse response)
    {
        if (statusCode == 200)
        {
            return new OkObjectResult(new List<object>
            {
                new StatusCodeResponse
                {
                    StatusCode = 200,
                },
                response,
            });
        }
        return new StatusCodeResult(statusCode);
    }
}