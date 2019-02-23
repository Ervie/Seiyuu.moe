
namespace SeiyuuMoe.Data.Context
{
	public interface ISeiyuuMoeContext
	{
		void SetAttached<TEntity>(TEntity entity) where TEntity : class;

		void SetModified<TEntity>(TEntity entity) where TEntity : class;

		void SetDeleted<TEntity>(TEntity entity) where TEntity : class;
	}
}
