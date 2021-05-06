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
							if (context.HostingEnvironment.IsProduction())
							{
								IConfigurationRoot builtConfig = config.Build();

								// If running as "Production" from our local environment (not in Azure), then use the Azure CLI Credential provider. This means you
								// have to log in via `az login` before running the local app as Production.
								TokenCredential cred = context.HostingEnvironment.IsProduction() ? new DefaultAzureCredential(false) : new AzureCliCredential();

								var keyvaultUri = new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/");
								var secretClient = new SecretClient(keyvaultUri, cred);
								config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
							}
						});
				});
	}
}
