using Microsoft.AspNetCore.Mvc;
using SMM_Azure_MVC.smm.Services;

namespace SMM_Azure_MVC.smm.Controllers
{
    public class AzureBaseController<TResult> : Controller
    {
        protected IAzureService<TResult> AzureService { get; }
        public AzureBaseController(IAzureService<TResult> azureService)
        {
            AzureService = azureService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await AzureService.GetResult();
            ProcesarResultadosAntesDeMostrar(result);
            return View(result);
        }

        protected virtual void ProcesarResultadosAntesDeMostrar(IEnumerable<TResult> result)
        {

        }
    }
}
