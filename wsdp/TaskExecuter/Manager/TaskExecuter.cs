﻿using BAL;
using BAL.Interface;
using BAL.Manager;
using DAL;
using Common;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskExecuting.Interface;
using Common.Enum;
using SiteProcessor;
using HtmlAgilityPack;
using log4net;
using System.IO;
using DAL.Elastic;

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
			//elasticManager = new ElasticManager(elasticuOw);
			//goodwizardManager = new GoodDatabasesWizard(elasticManager,goodManager);
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

			HtmlDocument doc = null;
			string pageSource = "";
			try
			{
				SiteDownloader sw = new SiteDownloader();
				pageSource = sw.GetPageSouce(url);

				doc = new HtmlDocument();
				doc.LoadHtml(pageSource);

			}
			catch(Exception ex)
			{
				logger.Error(ex.Message);
				return null;
			}


			//gets configuration from parsertask id
			ParserTaskDTO parsertask = parsermanager.Get(parsertaskid);
			GrabberSettingsDTO grabbersettings = parsertask.GrabberSettings;

			GoodDTO resultGood = new GoodDTO();

			resultGood.WebShop_Id = parsertask.WebShopId;
			resultGood.Category_Id = parsertask.CategoryId;

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
					value = doc.DocumentNode.SelectSingleNode(propitem.Value);
					htmlvalue = value.InnerHtml;
				}
				catch (Exception ex)
				{
					logger.Error(ex.Message);
					return null;
				}

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
			
			resultGood.PropertyValues = propertyValues;
			goodManager.InsertGood(resultGood);
			return resultGood;
		}
	}
}