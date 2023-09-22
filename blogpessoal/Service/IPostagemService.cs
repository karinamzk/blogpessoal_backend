using blogpessoal.Model;

namespace blogpessoal.Service
{
    public interface IPostagemService
    {
        Task<IEnumerable<Postagem>> GettAll();

        Task<Postagem?> GetById(long id);

        Task<IEnumerable<Postagem>> GettByTitulo(string titulo);

        Task<Postagem?> Create(Postagem postagem);

        Task<Postagem?> UpDate(Postagem postagem);

        Task Delete(Postagem postagem);
    }
}
