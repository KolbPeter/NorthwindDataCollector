namespace NorthwindDataCollector
{
    public class EntityProvider
    {
        public IEnumerable<TEntity> GetAll<TEntity>(DatabaseContext context)
            where TEntity : class
        {
            var entities = context.Set<TEntity>();
            return entities.ToArray();
        }
    }
}
