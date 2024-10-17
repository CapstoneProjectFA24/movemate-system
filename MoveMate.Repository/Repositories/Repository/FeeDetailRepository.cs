﻿using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class FeeDetailRepository : GenericRepository<FeeDetail>, IFeeDetailRepository
    {
        public FeeDetailRepository(MoveMateDbContext context) : base(context)
        {
        }
    }
}