// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;

/// <summary>
///     Options to be fed to "puppeteer.launch()"
/// </summary>
public class PlaywrightLaunchOptions
{
    public bool Headless { get; set; } = true;

    public float SlowMo { get; set; } = 0;

    public float TimeOut { get; set; } = 0;

    /// <summary>
    ///     Extra parameter allowing to toggle user-specified executable path and args without constant addition/removal.
    ///     This allows for example easy switching between the user's Chrome installation plus extension(s) and
    ///     the Puppeteer version of Chromium.
    /// </summary>
    public bool IgnoreExecutablePathAndArgs { get; set; } = true;

    /// <summary>
    ///     Path to browser folder.
    /// </summary>
    public ExecutablePath ExecutablePath { get; set; } = new()
    {
        Windows = "C:/Program Files/Google/Chrome/Application/chrome.exe",
        MacOs = "~/Library/Application Support/Google/Chrome",
        Linux = "~/.config/google-chrome/default"
    };

    public string[] Args { get; set; } = Array.Empty<string>();

    public float? ProcessedSlowMo => SlowMo == 0 ? null : SlowMo;

    public string? ProcessedExecutablePath => IgnoreExecutablePathAndArgs ? null : GetExecutablePathForCurrentOs();

    public string[]? ProcessedArgs => IgnoreExecutablePathAndArgs ? null : Args;

    // Gets chrome directory for current OS platform
    public string? GetExecutablePathForCurrentOs() =>
        OsPlatformEnumHelper.GetCurrentOsPlatform() switch
        {
            OsPlatformEnum.MacOs => ExecutablePath.MacOs,
            OsPlatformEnum.Linux => ExecutablePath.Linux,
            OsPlatformEnum.Windows => ExecutablePath.Windows,
            _ => throw new ArgumentOutOfRangeException()
        };
}

public class EmailOptions
{
    public string SmtpServer { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 587;

    public string FromEmailAddress { get; set; } = string.Empty;

    public string FromEmailName { get; set; } = string.Empty;

    public string FromEmailPassword { get; set; } = string.Empty;

    public string ToEmailAddress { get; set; } = string.Empty;

    public string ToEmailName { get; set; } = string.Empty;
}

public class ConsentAuthoriserOptions
{
    /// <summary>
    ///     User-supplied settings which are processed in <see cref="GetProcessedPuppeteerLaunch" />.
    /// </summary>
    public PlaywrightLaunchOptions PlaywrightLaunch { get; set; } = new();

    public EmailOptions Email { get; set; } = new();
}

/// <summary>
///     Path to folder. This must be set to a valid
///     directory path for the current OS platform.
///     This path should not be in the public repo to ensure data is not committed there.
/// </summary>
public class OsSpecificDirectory
{
    // Path to folder when current OS is macOS
    public string MacOs { get; set; } = "";

    // Path to folder when current OS is Windows
    public string Windows { get; set; } = "";

    // Path to folder when current OS is Linux
    public string Linux { get; set; } = "";
}

/// <summary>
///     Path to Chrome folder.
///     This must be set to a valid
///     file path for the current OS platform.
/// </summary>
public class ExecutablePath
{
    // Path to chrome folder when current OS is macOS
    public string? MacOs { get; set; }

    // Path to chrome folder when current OS is Windows
    public string? Windows { get; set; }

    // Path to chrome folder when current OS is Linux
    public string? Linux { get; set; }
}

public class BankTestSettings : ISettings<BankTestSettings>
{
    public ConsentAuthoriserOptions Auth { get; set; } = new();

    /// <summary>
    ///     Path to data folder used for logging, "API overrides", and bank user information.
    /// </summary>
    public OsSpecificDirectory DataDirectory { get; set; } = new();

    /// <summary>
    ///     Log external API requests/responses. Off by default.
    /// </summary>
    public bool LogExternalApiData { get; set; } = false;

    public string SettingsGroupName => "OpenBankingConnector:BankTests";

    public BankTestSettings Validate()
    {
        if (!Directory.Exists(GetDataDirectoryForCurrentOs()))
        {
            throw new DirectoryNotFoundException(
                "Can't locate data path specified in bank test setting DataDirectory:" +
                $"{GetDataDirectoryForCurrentOs()}. Please update app settings.");
        }

        // Check executable path in the case where this is not ignored
        if (!Auth.PlaywrightLaunch.IgnoreExecutablePathAndArgs)
        {
            // Check executable path is not null
            if (Auth.PlaywrightLaunch.GetExecutablePathForCurrentOs() is null)
            {
                throw new ArgumentException("Please specify an executable path in app settings.");
            }

            // Check executable path exists
            if (!File.Exists(Auth.PlaywrightLaunch.GetExecutablePathForCurrentOs()))
            {
                throw new DirectoryNotFoundException(
                    "Can't locate executable path specified in bank test setting ExecutablePath:" +
                    $"{Auth.PlaywrightLaunch.GetExecutablePathForCurrentOs()}. Please update app settings.");
            }
        }

        return this;
    }

    // Gets data directory for current OS platform
    public string GetDataDirectoryForCurrentOs() =>
        OsPlatformEnumHelper.GetCurrentOsPlatform() switch
        {
            OsPlatformEnum.MacOs => DataDirectory.MacOs,
            OsPlatformEnum.Linux => DataDirectory.Linux,
            OsPlatformEnum.Windows => DataDirectory.Windows,
            _ => throw new ArgumentOutOfRangeException()
        };
}
