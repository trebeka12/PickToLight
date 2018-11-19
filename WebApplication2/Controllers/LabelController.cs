using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication2.Controllers {

    public class LabelController : Controller {
		private readonly IHostingEnvironment env;

		public LabelController(IHostingEnvironment env) {
			this.env = env;
		}

        [HttpPost]
        public string Print([FromBody] dynamic param) {
			string labelPath = env.WebRootPath + "/label/SerialNumber.zpl";
			string lbt = System.IO.File.ReadAllText(labelPath);
			lbt = lbt.Replace("%SerialNumber%", (string)param.serialNumber);
			lbt = lbt.Replace("%Product%", (string)param.product);
			lbt = lbt.Replace("%Date%", DateTime.Now.ToString("yyyy.MM.dd"));
			return lbt;
		}
	}
}
