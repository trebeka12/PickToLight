using System;
using Dapper.Contrib.Extensions;

namespace WebApplication2.Models {
	[Table ("Part")]
	public class Part {
		[Key]
		public int ID { get; set; }
		public string PartName { get; set; }
        public int PartFamilyID { get; set; }	
		public string Code { get; set; }
        public int Qty { get; set; }
	}
}
