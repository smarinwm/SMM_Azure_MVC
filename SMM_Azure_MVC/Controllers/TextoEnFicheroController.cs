using SMM_Azure_MVC.smm.Models;
using SMM_Azure_MVC.smm.Services;

namespace SMM_Azure_MVC.smm.Controllers
{
    public class TextoEnFicheroController : AzureBaseController<TextoEnFichero>
    {
        public TextoEnFicheroController(TextoEnFicheroService azureService) : base(azureService)
        {
        }

        TextoEnFicheroService TextoEnFicheroService => AzureService as TextoEnFicheroService;

        protected override void ProcesarResultadosAntesDeMostrar(IEnumerable<TextoEnFichero> result)
        {
            base.ProcesarResultadosAntesDeMostrar(result);
            ViewData["TotalTextosAnalizados"] = result.Count();
            ViewData["MediaPuntuacionNegativa"] = TextoEnFicheroService.ObtenerPromedio(result.Select(r => r.PuntuacionNegativa));
            ViewData["MediaPuntuacionNeutral"] = TextoEnFicheroService.ObtenerPromedio(result.Select(r => r.PuntuacionNeutral));
            ViewData["MediaPuntuacionPositiva"] = TextoEnFicheroService.ObtenerPromedio(result.Select(r => r.PuntuacionPositiva));
        }
    }
}
