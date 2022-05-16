using System;
using System.Linq;
using MelonLoader;
using ReMod.Core.Managers;

namespace ReMod.Core
{
	// Token: 0x02000004 RID: 4
	public class ConfigValue<T>
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000006 RID: 6 RVA: 0x00002134 File Offset: 0x00000334
		// (remove) Token: 0x06000007 RID: 7 RVA: 0x0000216C File Offset: 0x0000036C
		public event Action OnValueChanged;

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000021A1 File Offset: 0x000003A1
		public T Value
		{
			get
			{
				return this._entry.Value;
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021B0 File Offset: 0x000003B0
		public ConfigValue(string name, T defaultValue, string displayName = null, string description = null, bool isHidden = false)
		{
			MelonPreferences_Category melonPreferences_Category = MelonPreferences.CreateCategory(ConfigManager.Instance.CategoryName);
			string text = string.Concat<char>(from c in name
											  where char.IsLetter(c) || char.IsNumber(c)
											  select c);
			this._entry = (melonPreferences_Category.GetEntry<T>(text) ?? melonPreferences_Category.CreateEntry<T>(text, defaultValue, displayName, description, isHidden, false, null, null));
			this._entry.OnValueChangedUntyped += delegate ()
			{
				Action onValueChanged = this.OnValueChanged;
				if (onValueChanged == null)
				{
					return;
				}
				onValueChanged();
			};
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002236 File Offset: 0x00000436
		public static implicit operator T(ConfigValue<T> conf)
		{
			return conf._entry.Value;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002243 File Offset: 0x00000443
		public void SetValue(T value)
		{
			this._entry.Value = value;
			MelonPreferences.Save();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002258 File Offset: 0x00000458
		public override string ToString()
		{
			T value = this._entry.Value;
			return value.ToString();
		}

		// Token: 0x04000002 RID: 2
		private readonly MelonPreferences_Entry<T> _entry;
	}
}
