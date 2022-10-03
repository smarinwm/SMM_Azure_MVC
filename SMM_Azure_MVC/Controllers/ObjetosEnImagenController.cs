using SMM_Azure_MVC.smm.Models;
using SMM_Azure_MVC.smm.Services;

namespace SMM_Azure_MVC.smm.Controllers
{
    public class ObjetosEnImagenController : AzureBaseController<ObjetosEnImagen>
    {
        public ObjetosEnImagenController(ObjetosEnImagenService objetosEnImagen) : base(objetosEnImagen)
        {
        }
    }
}
