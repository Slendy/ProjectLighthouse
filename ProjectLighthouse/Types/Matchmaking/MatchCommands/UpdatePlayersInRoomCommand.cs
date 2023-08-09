#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

public class UpdatePlayersInRoomCommand : IMatchCommand
{
    public async Task<IActionResult> ProcessCommand(DatabaseContext database, IRoomService roomService, UserEntity user, RoomCommandData commandData)
    {
        if (commandData.Players == null) return new BadRequestResult();
        if (commandData.Players.Count is < 1 or > 4) return new BadRequestResult();

        NewRoom room = await roomService.GetOrCreateRoomForUser(user);
        if (room.Host.UserId != user.UserId) return new BadRequestResult();

        List<string> roomUsers = room.Users.Select(u => u.Username).ToList();

        foreach (string username in commandData.Players.Where(username => !roomUsers.Contains(username)))
        {
            int userId = await database.Users.Where(u => u.Username == username)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();
            RoomUser roomUser = new(username, userId);
            await roomService.AddPlayerToRoom(roomUser, room);
        }

        return IMatchCommand.CreateResponse(200, new EmptyMatchResponse());
    }
}