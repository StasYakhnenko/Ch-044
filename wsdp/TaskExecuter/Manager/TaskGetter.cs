﻿using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskExecuting.Interface;
using System.Web.Script.Serialization;
using WebApp.Models;
//using System.Web.Helpers;
using Newtonsoft.Json;

namespace TaskExecuting.Manager
{
	public class TaskGetter : ITaskGetter
	{
		public TaskExecuterModel GetTask()
		{
			ConnectionFactory connFactory = new ConnectionFactory();
			connFactory.uri = new Uri("amqp://bpmcftle:cxjupG82CztHJ_Nfkh2GUEyb0Z-2FyGY@chicken.rmq.cloudamqp.com/bpmcftle");
			using (var conn = connFactory.CreateConnection())
			using (var channel = conn.CreateModel())
			{

				// ensure that the queue exists before we access it
				//channel.QueueDeclare("json", false, false, false, null);
				// do a simple poll of the queue 
				var data = channel.BasicGet("f_test", false);
				// the message is null if the queue was empty 

				if (data == null)
				{
					Console.WriteLine("Queue is empty!There is no Tasks!");
				}

				// convert the message back from byte[] to a string
				var message = Encoding.UTF8.GetString(data.Body);
				// ack the message, ie. confirm that we have processed it
				// otherwise it will be requeued a bit later
				channel.BasicAck(data.DeliveryTag, false);
				var task = JsonConvert.DeserializeObject<TaskExecuterModel>(message);
				return task;
			}
		}
	}
}
