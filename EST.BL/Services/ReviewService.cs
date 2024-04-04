using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.BL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ExpensesContext _context;
        public ReviewService(ExpensesContext context)
        {
            _context = context;
        }
        public async Task<ReviewDTO> GetReveiwDTO(Guid id, CancellationToken token)
        {
            return await _context.Reviews.Where(r => r.Id == id).Select(i => new ReviewDTO()
            {
                Value = i.Value
            }).FirstOrDefaultAsync(token);
        }
        public async Task<double> GetItemsRating(Guid itemId, CancellationToken token)
        {
            var ratings = await _context.Reviews.Where(i => i.ItemId == itemId).ToListAsync(token);
            return ratings.Sum(l => l.Value) / ratings.Count;
        }
        public async Task<bool> Create(Guid itemId, Guid userId, ReviewDTO reviewDTO)
        {
            var review = new Review()
            {
                Value = reviewDTO.Value,
                ItemId = itemId,
                UserId = userId
            };
            await _context.Reviews.AddAsync(review);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid reviewId)
        {
            var review = await _context.Reviews.Where(r => r.Id == reviewId).FirstOrDefaultAsync();
            _context.Reviews.Remove(review);
            return await SaveAsync();
        }
        public async Task<bool> Exist(Guid reviewId)
        {
            return await _context.Reviews.Where(r => r.Id == reviewId).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var result = await _context.SaveChangesAsync();
            return result > 0 ? true : false;
        }
    }
}
