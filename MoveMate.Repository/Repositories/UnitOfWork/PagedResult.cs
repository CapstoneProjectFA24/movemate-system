namespace MoveMate.Repository.Repositories.UnitOfWork;

public class PagedResult<TEntity>
{
    public List<TEntity> Results { get; set; }
    public Pagination? Pagination { get; set; }
}
