// Variables Globals du projet
using System.Net;

public class Globals 
{
    public static bool IsServer = false; // Par défaut, on est pas le serveur
    public static string HostIP; // IP du serveur
    public static int HostPort; // Port du serveur
    public static string playerID; // Id du joueur

    public static string getLocalIPAddress() {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "Aucune adresse IPv4 trouvée.";
        }
        catch (System.Exception ex)
        {
            return $"Erreur : {ex.Message}";
        }
    }
}