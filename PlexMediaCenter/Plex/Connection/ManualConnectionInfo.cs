using System;
using System.Net;
using PlexMediaCenter.Util;

namespace PlexMediaCenter.Plex.Connection {
    [Serializable]
    public class ManualConnectionInfo : BaseConnectionInfo {

        public string UserName { get; set; }
        public string UserPass { get; set; }

        public ManualConnectionInfo() {
        }

        public ManualConnectionInfo(string hostName, string hostAdress, int plexPort, string userName, string userPass) 
            : base(hostName, hostAdress, plexPort){
            UserName = userName;
            UserPass = EncryptPassword(userName, userPass);
        }

        internal override void AddAuthHeaders(WebClient webClient) {
            webClient.Headers["X-Plex-User"] = UserName;
            webClient.Headers["X-Plex-Pass"] = UserPass;
        }

        internal override string GetAuthUrlParameters() {
            return String.Format("X-Plex-User={0}&X-Plex-Pass={1}", UserName, UserPass);
        }

        public static string EncryptPassword(string userName, string userPass) {
            return Encryption.GetSHA1Hash(userName.ToLower() + Encryption.GetSHA1Hash(userPass));
        }
    }
}
