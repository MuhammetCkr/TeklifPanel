﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Concrete
{
    public class OfferManager : IOfferService
    {
        public void Create(Offer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(Offer entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Offer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Offer entity)
        {
            throw new NotImplementedException();
        }

        public Offer GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Offer> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Offer>> GetManyAsync(Expression<Func<Offer, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void Update(Offer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Offer entity)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<Offer>> IService<Offer>.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<Offer> IService<Offer>.GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}