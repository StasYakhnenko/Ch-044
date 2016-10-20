﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using log4net;
using Model.DB;
using BAL.Interface;

namespace BAL.Manager
{
    public class GoodManager : BaseManager, IGoodManager
    {
        static readonly ILog logger = LogManager.GetLogger("RollingLogFileAppender");
        public GoodManager(IUnitOfWork uOW) : base(uOW)
        {
            
        }

        public void InsertGood(Good good)
        {
            uOW.GoodRepo.Insert(good);
        }
    }
}
