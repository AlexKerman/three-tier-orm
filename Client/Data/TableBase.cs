using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Grpc.Core;
using GrpcContracts;

namespace Client.Data;

public interface ITable<T>
{
	IEnumerable<T> Select(SelectRequest request, Orm.OrmClient client);
	//IAsyncProtoFetcher<T> SelectAsync(SelectRequest request);
	string SchemaName { get; }
	string TableName { get; }
}

public interface IReceiveNotification
{
	string TableName { get; }
	void Notify(int version);
}

public interface IIncludableQueryable<TEntity>
{
	LinqProvider<TEntity> Include(Expression<Func<TEntity, object>> expr);
}

public abstract class TableBase<TEntity> : IQueryable<TEntity>, IOrderedQueryable<TEntity>
{
	//public TableBase()
	//{
	//	NotificationReceiver.Receivers.Add(this);
	//}

	//public abstract IEnumerable<TEntity> Select(SelectRequest request, Orm.OrmClient client);
	//public abstract AsyncServerStreamingCall<TProto> GetStreamingCall(SelectRequest request, CancellationToken cts);

	//public abstract TEntity ProtoToEntity(TProto proto);

	//public abstract string SchemaName { get; }
	//public abstract string TableName { get; }

	public Expression Expression => Expression.Constant(this);

	public Type ElementType => typeof(TEntity);

	public IQueryProvider Provider => new LinqProvider<TEntity>(this);

	public IEnumerator<TEntity> GetEnumerator()
	{
		return new LinqProvider<TEntity>(this).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public EventHandler TableChanged;

	public void Notify(int version)
	{
		TableChanged?.Invoke(this, EventArgs.Empty);
	}

	//public IAsyncProtoFetcher<TEntity> SelectAsync(SelectRequest request)
	//{
	//	return new AsyncProtoFetcher<TEntity, TProto>(request, this);
	//}
}