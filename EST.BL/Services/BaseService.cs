using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.BL.Services
{
    public abstract class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        private readonly ExpensesContext _expensesContext;
        public BaseService(ExpensesContext expensesContext)
        {
            _expensesContext = expensesContext;
        }
        public async Task<TEntity> GetById(Guid Id)
        {
            return await _expensesContext.Set<TEntity>().Where(u => u.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<bool> Create(TEntity entity)
        {
            await _expensesContext.Set<TEntity>().AddAsync(entity);
            return await SaveAsync();
        }
        public async Task<bool> Update(TEntity entity)
        {
            _expensesContext.Set<TEntity>().Update(entity);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid Id)
        {
            if (!await Exist(Id))
            {
                return false;
            }
            var entity = await GetById(Id);
            _expensesContext.Set<TEntity>().Remove(entity);
            return await SaveAsync();
        }
        public async Task<bool> Exist(Guid Id)
        {
            return await _expensesContext.Set<TEntity>().Where(u => u.Id == Id).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _expensesContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
