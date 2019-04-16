
using System;

namespace PickToLight.Models {
	public class Assembly {
		public int ID { get; set; }
		public string SerialNumber { get; set; }			
		public int AssembledPartID { get; set; }
		public int AssemblyOrder {get;set;}
		public DateTime? AssembledTime {get;set;}

        public virtual Part Part { get; set; }			
	}
}
