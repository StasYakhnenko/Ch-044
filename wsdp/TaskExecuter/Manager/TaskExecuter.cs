﻿using BAL;
using BAL.Manager;
using DAL;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskExecuting.Interface;
using Common.Enum;
using SiteProcessor;
using HtmlAgilityPack;
using log4net;
using DAL.Elastic;
using TaskExecuting.LogStorage;

namespace TaskExecuting.Manager
{
    public class TaskExecuter : ITaskExecuter
	{
		private UnitOfWork uOw = null;
		private ElasticUnitOfWork elasticuOw = null;
		private ParserTaskManager parsermanager = null;
		private GoodDatabasesWizard goodwizardManager = null;
		private PropertyManager propmanager = null;
		private ElasticManager elasticManager = null;
		private GoodManager goodManager = null;
		private URLManager urlManager = null;
		private HtmlValidator htmlValidator = null;
		private PriceManager priceManager = null;
        private ExecuteManager taskinfoManager = null;
		protected static readonly ILog logger = LogManager.GetLogger("RollingLogFileAppender");


		/// <summary>
		/// Initializating managers and uOw
		/// </summary>
		public TaskExecuter()
		{
			uOw = new UnitOfWork();
			//elasticuOw = new ElasticUnitOfWork();
			parsermanager = new ParserTaskManager(uOw);
			propmanager = new PropertyManager(uOw);
			goodManager = new GoodManager(uOw);
			urlManager = new URLManager(uOw);
			htmlValidator = new HtmlValidator();
			priceManager = new PriceManager(uOw);
			//elasticManager = new ElasticManager(elasticuOw);
			//goodwizardManager = new GoodDatabasesWizard(elasticuOw,uOw);
            taskinfoManager = new ExecuteManager(uOw);
			AutoMapperConfig.Configure();
		}

		/// <summary>
		/// Parses input url by configuration from parser task
		/// </summary>
		/// <param name="parsertaskid">id of parser task</param>
		/// <param name="url">url to parse</param>
		/// <returns>New parsed GoodDTO</returns>
		public GoodDTO ExecuteTask(int parsertaskid, string url)
		{
			//downloading page source using tor+phantomjs
			ParserTaskDTO parsertask = parsermanager.Get(parsertaskid);
			HtmlDocument doc = null;

            //adding to local log storage
            ExecutingInfoDTO taskinfo = new ExecutingInfoDTO()
            {
                GoodUrl = url,
                Status = ExecuteStatus.Executing,
                Date = DateTime.Now,
                ParserTaskId = parsertaskid
            };

            taskinfo.Id = taskinfoManager.Insert(taskinfo);

            //getting page souce due to method
			string pageSource = "";
			try
			{
				SiteDownloader sw = new SiteDownloader();

				switch (parsertask.IteratorSettings.DownloadMethod)
				{
					case DownloadMethod.Direct:
						pageSource = sw.GetPageSouceDirectly(url);
						break;
					case DownloadMethod.Tor:
						pageSource = sw.GetPageSouce(url);
						break;
					default:
						break;
				}

				pageSource = htmlValidator.CheckHtml(pageSource);

				doc = new HtmlDocument();
				doc.LoadHtml(pageSource);

			}
			catch(Exception ex)
			{
				logger.Error(ex.Message);
				return null;
			}


			//gets configuration from parsertask id
			
			GrabberSettingsDTO grabbersettings = parsertask.GrabberSettings;

			GoodDTO resultGood = new GoodDTO();

			resultGood.WebShop_Id = parsertask.WebShopId;
			resultGood.Category_Id = parsertask.CategoryId;
			///////////////////////////////////Parcing name by list of xpathes
			try
			{
				var name = "";
				foreach (var nameprop in grabbersettings.Name)
				{
					HtmlNode value = doc.DocumentNode.SelectSingleNode(nameprop);
					if (value != null)
					{
						name = value.InnerHtml;
						break;
					}
				}
				resultGood.Name = name;
			}
			catch(Exception ex)
			{

			}
			/////////////////////////////////////Parcing price by list of xpathes
			try
			{
				var price = "";
				foreach (var priceprop in grabbersettings.Price)
				{
					HtmlNode value = doc.DocumentNode.SelectSingleNode(priceprop);
					if (value != null)
					{
						price = value.InnerHtml;
						break;
					}
				}
				if (price!="")
				{
					resultGood.Price = Convert.ToDecimal(this.RemoveAllLetters(price));
				}
				
			}
			catch (Exception ex)
			{

			}
			//////////////////////////////////////Parcing old price by list of xpathes
			try
			{
				var oldPrice = "";
				foreach (var price in grabbersettings.OldPrice)
				{
					HtmlNode value = doc.DocumentNode.SelectNodes(price).FirstOrDefault();
					if (value != null)
					{
						oldPrice = value.InnerHtml;
						break;
					}
				}
				if (oldPrice != "")
				{
					resultGood.OldPrice = Convert.ToDecimal(this.RemoveAllLetters(oldPrice));
				}
				
			}
			catch (Exception ex)
			{

			}
			//////////////////////////////Parcing image link by list of xpathes
			try
			{
				var imagelink = "";
				foreach (var imglink in grabbersettings.ImgLink)
				{
					HtmlNode value = doc.DocumentNode.SelectSingleNode(imglink + "/@src");
					if (value != null)
					{
						imagelink = value.Attributes["src"].Value;
						break;
					}
                    if (imagelink == "")
                    {
                        resultGood.ImgLink = @"http://www.kalahandi.info/wp-content/uploads/2016/05/sorry-image-not-available.png";
                    }
                    else
                    {
                        resultGood.ImgLink = imagelink;
                    }
				}
				
			}
			catch (Exception ex)
			{
                resultGood.ImgLink = @"http://www.kalahandi.info/wp-content/uploads/2016/05/sorry-image-not-available.png";
			}

			resultGood.UrlLink = url;
			PropertyValuesDTO propertyValues = new PropertyValuesDTO();
			propertyValues.DictDoubleProperties = new Dictionary<int, double>();
			propertyValues.DictIntProperties = new Dictionary<int, int>();
			propertyValues.DictStringProperties = new Dictionary<int, string>();

			foreach (var propitem in grabbersettings.PropertyItems)
			{
				HtmlNode value = null;
				PropertyDTO property = propmanager.Get(propitem.Id);
				var htmlvalue = "";

				try
				{
					foreach (var item in propitem.Value)
					{
						value = doc.DocumentNode.SelectSingleNode(item);
						if (value != null)
						{
							htmlvalue = value.InnerHtml;
							break;
						}
					}
				}
				catch (Exception ex)
				{

				}

				try
				{
					switch (property.Type)
					{
						case PropertyType.Integer:
							propertyValues.DictIntProperties.Add(propitem.Id, Convert.ToInt32(htmlvalue));
							break;
						case PropertyType.Double:
							propertyValues.DictDoubleProperties.Add(propitem.Id, Convert.ToDouble(htmlvalue));
							break;
						case PropertyType.String:
							propertyValues.DictStringProperties.Add(propitem.Id, htmlvalue);
							break;
						default:
							break;
					}
				}
				catch(Exception ex)
				{
					logger.Error(ex);
				}

			}
			resultGood.Status = true;
			resultGood.PropertyValues = propertyValues;
			goodManager.Insert(resultGood);
			var newPrice = new PriceHistoryDTO();
			newPrice.Url = resultGood.UrlLink;
			newPrice.Price = resultGood.Price;
			newPrice.Date = DateTime.Now;
			newPrice.Name = resultGood.Name;
			priceManager.Insert(newPrice);

            //deleting from local log storage
            taskinfoManager.Delete(taskinfo);
            return resultGood;
		}


        /// <summary>
        /// Remove from strings all letters. Maken for correct parsing decimal values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		private string RemoveAllLetters(string value)
		{
			char[] arr = value.ToArray().Where(c => char.IsDigit(c) || c == '.').Select(c => c).ToArray();
			return new string(arr);
		}

	}
}
