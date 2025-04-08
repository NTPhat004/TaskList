using System;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using static TaskManagement.Models.GroupInvitationModel;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Repositories.Implementations
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly TaskListDbContext _context;

        public InvitationRepository(TaskListDbContext context)
        {
            _context = context;
        }
        public async Task<GroupInvitationModel> CreateInvitationAsync(GroupInvitationModel invitation)
        {
            _context.GroupInvitation.Add(invitation);
            await _context.SaveChangesAsync();
            if(invitation != null)
            {
                invitation.Group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == invitation.GroupId);
            }         
            return invitation;
        }

        public async Task<List<GroupInvitationModel>> GetUserInvitationsAsync(Guid userId)
        {
            return await _context.GroupInvitation
                .Where(i => i.InviteeId == userId && i.Status == InvitationStatus.Pending)
                .Include(i => i.Group)
                .Include(i => i.Inviter)
                .ToListAsync();
        }

        public async Task UpdateInvitationStatusAsync(GroupInvitationModel invite)
        {
            if (invite != null)
            {
                _context.GroupInvitation.Update(invite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GroupInvitationModel?> GetInvitationByIdAsync(Guid invitationId)
        {
            return await _context.GroupInvitation
                .Include(i => i.Group)
                .Include(i => i.Inviter)
                .Include(i => i.Invitee)
                .FirstOrDefaultAsync(i => i.Id == invitationId);
        }

    }
}
