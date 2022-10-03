namespace SMM_Azure_MVC.smm.Services
{
    public interface IAzureService<TResult>
    {
        public IConfiguration Configuration { get; set; }
        public Task<IEnumerable<TResult>> GetResult();
    }
}
