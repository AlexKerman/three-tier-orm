using Grpc.Core;
using GrpcContracts;
using Server.Data;

namespace Server.Services;

public partial class OrmService : Orm.OrmBase
{
	public override Task<SelectProductReply> SelectProduct(SelectRequest request, ServerCallContext context)
	{
		var reply = new SelectProductReply();
		try
		{
			reply.Objects.AddRange(Tables.Products.GetEntities(request.Request).Select(o => o.GetProto()));
		}
		catch (Exception ex)
		{
			reply.ErrorMessage = ex.Message + "\n" + ex.StackTrace;
		}
		return Task.FromResult(reply);
	}
}