using CamisasMvc.Models.Enuns;
using Microsoft.AspNetCore.Http; // Adicione esta referência

namespace CamisasMvc.Models
{
    public class CamisasViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Tamanho { get; set; }
        public ClasseEnum Classe { get; set; } //referencia das condições dentro da PASTA ENUNS

        public string ClasseSelecionada { get; set; }
        public List<string> ListaClasses { get; set; }

        public IFormFile Foto { get; set; }

    }
}
