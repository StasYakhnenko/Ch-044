﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace task2
{
	[Serializable]
	public class Producer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Country { get; set; }
	}
}