using Harmony;
using System;
using System.Reflection;

namespace Patches
{
	internal class Patch // it says its Obsolete but what it wants me to use doesnt work with what I'm trying to do!
	{

		public Patch(Type PatchClass, Type YourClass, string Method, string ReplaceMethod, BindingFlags stat = BindingFlags.Static, BindingFlags pub = BindingFlags.NonPublic)
		{

			Patch.HInstance.Patch(AccessTools.Method(PatchClass, Method, null, null), GetPatch(YourClass, ReplaceMethod, stat, pub), null, null);

		}

		public Patch(MethodInfo methodInfo, Type type, HarmonyLib.HarmonyMethod harmonyMethod)
		{
			MethodInfo = methodInfo;
			HarmonyMethod = harmonyMethod;
		}


		private HarmonyMethod GetPatch(Type YourClass, string MethodName, BindingFlags stat, BindingFlags pub)

		{

			return new HarmonyMethod(YourClass.GetMethod(MethodName, stat | pub));

		}

		private static readonly HarmonyInstance HInstance = HarmonyInstance.Create("Patches");

		public MethodInfo MethodInfo { get; }
		public HarmonyLib.HarmonyMethod HarmonyMethod { get; }
	}
}
