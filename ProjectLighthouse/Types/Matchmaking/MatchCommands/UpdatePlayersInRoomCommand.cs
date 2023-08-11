#nullable enable
using System;
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
        if (!commandData.Players.Contains(user.Username)) return new BadRequestResult();

        NewRoom room = await roomService.GetOrCreateRoomForUser(user);
        if (room.Host != user.Username) return new BadRequestResult();

        List<string> currentUsers = room.Users.ToList();

        List<string> removedUsers = currentUsers.Except(commandData.Players).ToList();
        List<string> newUsers = commandData.Players.Except(currentUsers).ToList();

        room.FailedJoin.RemoveAll(r => newUsers.Contains(r.Username));
        foreach (string username in removedUsers.Where(username => room.RecentlyLeft.All(u => u.Username != username)))
        {
            room.RecentlyLeft.Add(new UserExpiry(username, DateTime.UtcNow.AddMinutes(5)));
        }

        await roomService.UpdateRoom(room);

        foreach (string username in newUsers)
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