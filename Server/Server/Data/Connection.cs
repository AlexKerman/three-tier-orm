namespace Server.Data;

public class Connection
{
#if DEBUG
	public static string OracleConnectionString = "User Id=kerman;Password=global;data source=192.168.1.34/XEPDB1";
#else
	public static string OracleConnectionString = "User Id=kerman;Password=global;data source=127.0.0.1/XEPDB1";
#endif
}