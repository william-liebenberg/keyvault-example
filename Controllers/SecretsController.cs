using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text;

namespace KeyVaultExample.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class SecretsController : ControllerBase
    {
		private readonly IConfiguration _config;
		private readonly ApplicationSecrets _secrets;

		public SecretsController(IConfiguration config, IOptions<ApplicationSecrets> secrets)
        {
			_config = config;
			_secrets = secrets.Value;
		}

		[HttpGet]
		[Route("/RevealYourConfig")]
		public IActionResult RevealYourConfig()
		{
			var licenseKey = _config["ApplicationSecrets:LicenseKey"];
			var connectionString = _config["ApplicationSecrets:SqlConnectionString"];

			var sb = new StringBuilder();
			sb.AppendLine($"LicenseKey: {licenseKey}");
			sb.AppendLine($"SqlConnectionString: {connectionString}");			
			return new OkObjectResult(sb.ToString());
		}

		[HttpGet]
		[Route("/RevealYourSecrets")]
		public IActionResult RevealYourSecrets()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"LicenseKey: {_secrets.LicenseKey}");
			sb.AppendLine($"SqlConnectionString: {_secrets.SqlConnectionString}");
			return new OkObjectResult(sb.ToString());
		}
	}
}
