namespace EventBooking.Core.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IEventRepository Event { get; }
        Task SaveAsync();
    }
}
