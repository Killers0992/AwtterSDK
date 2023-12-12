using System;

namespace AwtterSDK.Editor.Enums
{
    [Flags]
    public enum SdkStatus
    {
        None,
        NotLoggedIn,
        PatreonItems,
        TosNotAccepted,
        InstallInProgress,
        BaseInstalled,
        ViewAdditionalPackages,
        BaseNotInstalled,
        ViewScenes,
        ViewSettings,
        ManagePackages,
        ResetPage,
    }
}
