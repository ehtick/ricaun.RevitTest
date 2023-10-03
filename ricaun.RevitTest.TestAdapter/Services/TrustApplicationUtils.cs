using ricaun.Security.WinTrust;

namespace ricaun.RevitTest.TestAdapter.Services
{
    internal static class TrustApplicationUtils
    {
        public static bool IsTrust(string filePath)
        {
            return WinTrust.VerifyEmbeddedSignature(filePath);
        }

        public static bool IsTrust(string filePath, out string signedMessage)
        {
            var isTrust = IsTrust(filePath);
            signedMessage = $"File is not Signed.";
            if (Certificate.IsSignedFile(filePath))
            {
                string communNameSubject = Certificate.GetSignedFileSubject(filePath, "cn");
                string organizationIssuer = Certificate.GetSignedFileIssuer(filePath, "o");
                signedMessage = $"File Signed by {communNameSubject} [{organizationIssuer}]";
            }
            return isTrust;
        }
    }
}