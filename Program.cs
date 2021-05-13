using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace KeyVaultExample
{
	public static class IWebHostEnvironmentExtensions
	{
		public static bool IsAzureAppService(this IWebHostEnvironment env)
		{
			var websiteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
			return string.IsNullOrEmpty(websiteName) is not true;
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.UseStartup<Startup>()
						.ConfigureAppConfiguration((context, config) =>
						{
							// To run the "Production" app locally, modify your launchSettings.json file
							// -> set ASPNETCORE_ENVIRONMENT value as "Production"
							if (context.HostingEnvironment.IsProduction())
							{
								IConfigurationRoot builtConfig = config.Build();

								// ATTENTION:
								//
								// If running the app from your local dev machine (not in Azure AppService),
								// -> use the AzureCliCredential provider.
								// -> This means you have to log in locally via `az login` before running the app on your local machine.
								//
								// If running the app from Azure AppService
								// -> use the DefaultAzureCredential provider
								//
								TokenCredential cred = context.HostingEnvironment.IsAzureAppService() ?
									new DefaultAzureCredential(false) : new AzureCliCredential();

								var keyvaultUri = new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/");
								var secretClient = new SecretClient(keyvaultUri, cred);
								config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
							}
						});
				});
	}
}
