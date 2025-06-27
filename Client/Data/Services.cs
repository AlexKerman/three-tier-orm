using Grpc.Core;
using GrpcContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace Client.Data;

internal class Services
{
	private static GrpcChannel channel;

	private static void CreateChannel()
	{
		channel = GrpcChannel.ForAddress(Connections.ConnectionAddress);
	}

	public static Orm.OrmClient GetOrmClient()
	{
		if (channel == null) CreateChannel();
		var client = new Orm.OrmClient(channel);
		return client;
	}
}