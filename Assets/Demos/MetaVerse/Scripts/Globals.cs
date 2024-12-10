using System.Net;
using UnityEngine;

public class Globals 
{
    public static bool IsServer = false;
    public static string HostIP;
    public static int HostPort;

    public static string GetLocalIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    // Debug.Log(ip.ToString());
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