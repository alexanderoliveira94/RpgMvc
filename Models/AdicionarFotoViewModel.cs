using CamisasMvc.Models.Enuns;
using Microsoft.AspNetCore.Http; // Adicione esta referência

namespace CamisasMvc.Models
{
    public class AdicionarFotoViewModel
    {
        public int CamisaId  { get; set; }
        public string foto { get; set; }

        

    }
}
