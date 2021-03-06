﻿using DatingApp31.Helpers;
using DatingApp31.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp31.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            this._context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(v => v.Id != userParams.UserId);
            users = users.Where(v => v.Gender == userParams.Gender);

            if(userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(v => userLikers.Contains(v.Id));
            }
            if(userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(v => userLikees.Contains(v.Id));
            }

            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-(userParams.MaxAge + 1));
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(v => v.DateOfBirth >= minDob && v.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(v => v.Created);
                        break;
                    default:
                        users = users.OrderByDescending(v => v.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);

            if(likers)
            {
                return user.Likers.Where(v => v.LikeeId == id).Select(p => p.LikerId);
            }
            else
            {
                return user.Likees.Where(v => v.LikerId == id).Select(p => p.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(v => v.Id == id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(v => v.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(v => v.LikerId == userId && v.LikeeId == recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(v => v.Id == id); 
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(v => v.RecepientId == messageParams.UserId && v.RecepientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(v => v.SenderId == messageParams.UserId && v.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(v => v.RecepientId == messageParams.UserId && v.IsRead == false && v.RecepientDeleted == false);
                    break;
            }

            messages = messages.OrderByDescending(v => v.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Where(v => v.RecepientId == userId && v.RecepientDeleted == false && v.SenderId == recipientId
                || v.SenderId == userId && v.SenderDeleted == false && v.RecepientId == recipientId)
                .OrderByDescending(v => v.MessageSent)
                .ToListAsync();

            return messages;
        }
    }
}
