﻿using BAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DB;
using DAL.Interface;
using Model.DTO;
using AutoMapper;

namespace BAL.Manager
{
	public class PriceManager : BaseManager,IPriceManager
	{
		public PriceManager(IUnitOfWork uOW) : base(uOW)
		{
		}

		public void Insert(PriceHistoryDTO price)
		{
			var priceDb = Mapper.Map<PriceHistory>(price);
			
			var GoodPrices = uOW.PriceRepo.All.Where(x => x.Url == priceDb.Url && x.Price==price.Price);
			if(GoodPrices==null)
			{
				uOW.PriceRepo.Insert(priceDb);
				uOW.Save();
			}
		}
	}
}
