namespace MoveMate.Repository.Repositories.UnitOfWork;

public class Pagination
{
    public Pagination()
    {
        pageIndex = 0;
        pageSize = 1;
        totalItemsCount = 1;
    }

    public Pagination(int pageIndex, int pageSize, int count)
    {
        this.pageIndex = pageIndex;
        this.pageSize = pageSize;
        totalItemsCount = count;
    }

    public int totalItemsCount { get; set; }
    public int pageSize { get; set; } = -1;
    public int pageIndex { get; set; } = 0;

    public int totalPagesCount
    {
        get
        {
            if (pageSize == -1)
            {
                return totalItemsCount;
            }

            var temp = totalItemsCount / pageSize;
            if (totalItemsCount % pageSize == 0)
            {
                return temp;
            }

            return temp + 1;
        }
    }
}