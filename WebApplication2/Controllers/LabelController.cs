using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PickToLight.Controllers {

    public class LabelController : Controller {
		private readonly IHostingEnvironment env;

		public LabelController(IHostingEnvironment env) {
			this.env = env;
		}

        [HttpPost]
        public string Print(string serial, string part) {
			string labelPath = env.WebRootPath + "/label/SerialNumber.zpl";
			string lbt = System.IO.File.ReadAllText(labelPath);
			lbt = lbt.Replace("%SerialNumber%", serial);
			lbt = lbt.Replace("%Product%", part);
			lbt = lbt.Replace("%Date%", DateTime.Now.ToString("yyyy.MM.dd"));
			return lbt;
		}
	}
}
