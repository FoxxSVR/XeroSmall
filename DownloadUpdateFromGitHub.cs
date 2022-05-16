using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xero
{
    class DownloadUpdateFromGitHub
    {
        internal static class GitHubInfo
        {
            public const string Author = "FoxxSVR";
            public const string Repository = "XeroSmall";
            public const string Version = "latest";
        }
        public static void DownloadFromGitHub(string fileName, out Assembly loadedAssembly)
        {
            using var sha256 = SHA256.Create();

            byte[] bytes = null;
            if (File.Exists($"{fileName}.dll"))
            {
                bytes = File.ReadAllBytes($"{fileName}.dll");
            }

            using var wc = new WebClient
            {
                Headers =
                {
                 "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0"
                }
        };

            byte[] latestBytes = null;
            try
            {
                latestBytes = wc.DownloadData($"https://github.com/{GitHubInfo.Author}/{GitHubInfo.Repository}/releases/{GitHubInfo.Version}/download/{fileName}.dll");
            }

            catch (WebException e)
            {
                MelonLogger.Error($"Unable to download latest version of XeroSmall: {e}");
            }

            if (bytes == null)
            {
                if (latestBytes == null)
                {
                    MelonLogger.Error($"No local file exists and unable to download latest version from GitHub. {fileName} will not load!");
                    loadedAssembly = null;
                    return;
                }
                MelonLogger.Warning($"Couldn't find {fileName}.dll on disk. Saving latest version from GitHub.");
                bytes = latestBytes;
                try
                {
                    File.WriteAllBytes($"{fileName}.dll", bytes);
                }
                catch (IOException e)
                {
                    MelonLogger.Warning($"Failed writing {fileName} to disk. You may encounter errors while using Xero Small.");
                }
            }

            try
            {
                loadedAssembly = Assembly.Load(bytes);
            }

            catch (BadImageFormatException e)
            {
                MelonLogger.Error($"Couldn't load specified image: {e}");
                loadedAssembly = null;
            }
        }
    }
}
