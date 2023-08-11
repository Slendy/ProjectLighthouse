#nullable enable
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

public class UpdateRoomDataCommand : IMatchCommand
{
    public async Task<IActionResult> ProcessCommand(DatabaseContext database, IRoomService roomService, UserEntity user, RoomCommandData commandData)
    {
        NewRoom room = await roomService.GetOrCreateRoomForUser(user);
        // if (room.Host.UserId != user.UserId) return new BadRequestResult();

        if (commandData.RoomState == null) return new BadRequestResult();

        room.RoomState = commandData.RoomState.Value;

        if (commandData.Slot?.Count == 2)
        {
            room.RoomSlot = new RoomSlot
            {
                SlotType = (SlotType)commandData.Slot.ElementAt(0),
                SlotId = commandData.Slot.ElementAt(1),
            };
        }

        if (commandData.Mood != null) room.RoomMood = (RoomMood)commandData.Mood;

        await roomService.UpdateRoom(room);
        return IMatchCommand.CreateResponse(200, new EmptyMatchResponse());
    }
}