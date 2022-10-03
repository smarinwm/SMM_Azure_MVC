using SMM_Azure_MVC.smm.Models;
using SMM_Azure_MVC.smm.Services;

namespace SMM_Azure_MVC.smm.Controllers
{
    public class TextoEnImagenController : AzureBaseController<TextoEnImagen>
    {
        public TextoEnImagenController(TextoEnImagenService textoEnImagenService) : base(textoEnImagenService)
        {
        }
    }
}
