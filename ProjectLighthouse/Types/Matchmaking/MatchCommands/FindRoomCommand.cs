#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

public class FindRoomCommand : IMatchCommand
{
    private readonly GameTokenEntity token;

    public FindRoomCommand(GameTokenEntity token)
    {
        this.token = token;
    }

    public async Task<IActionResult> ProcessCommand(DatabaseContext database, IRoomService roomService, UserEntity user, RoomCommandData commandData)
    {
        NewRoom? room = await roomService.GetRoomForUser(user.UserId);
        if (room == null) return new BadRequestResult();
        
        //TODO check room state and if the room has enough space for new players

        IList<NewRoom> rooms = await roomService.GetRooms(r =>
            r.RoomId != room.RoomId &&
            r.RoomPlatform == this.token.Platform &&
            r.RoomVersion == this.token.GameVersion);
        rooms = rooms.OrderByDescending(r => r.RoomMood).ToList();

        if (rooms.Count == 0) return IMatchCommand.CreateResponse(200, new EmptyMatchResponse());

        NewRoom targetRoom = rooms.First();

        targetRoom.Users = targetRoom.Users.Union(room.Users).ToList();
        FindRoomResponse roomResponse = new()
        {
            Players = targetRoom.Users.Select(u => new RoomPlayerResponse()
            {
                Username = u.Username,
                MatchingRes = Convert.ToByte(u.Username == user.Username),
            }).ToList(),
            Slots = new List<List<int>>
            {
                new(1)
                {
                    (int)targetRoom.RoomSlot.SlotType,
                    targetRoom.RoomSlot.SlotId,
                },
            },
            RoomState = (byte)targetRoom.RoomState,
        };

        // TODO: Don't join rooms we recently joined
        // TODO: implement recentlyLeft and failedJoin

        return IMatchCommand.CreateResponse(200, roomResponse);
    }
}