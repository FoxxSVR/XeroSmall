using System;

namespace ReMod.Core.Managers
{
	public class ConfigManager
	{
		public string CategoryName { get; }
		public static ConfigManager Instance { get; private set; }

		public ConfigManager(string categoryName)
		{
			if (ConfigManager.Instance != null)
			{
				throw new Exception("ConfigManager already exists.");
			}
			ConfigManager.Instance = this;
			this.CategoryName = categoryName;
		}
	}
}
