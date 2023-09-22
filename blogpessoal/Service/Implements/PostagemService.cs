using blogpessoal.Model;
using blogpessoal.Data;
using Microsoft.EntityFrameworkCore;

namespace blogpessoal.Service.Implements
{
    public class PostagemService : IPostagemService
    {
        private readonly AppDbContext _context;
        public PostagemService(AppDbContext context)
        {
            _context = context; 
        }
        public Task<Postagem?> Create(Postagem postagem)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Postagem postagem)
        {
            throw new NotImplementedException();
        }

        public Task<Postagem?> GetById(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Postagem>> GettAll()
        {
            return await _context.Postagens.ToListAsync();
        }

        public Task<IEnumerable<Postagem>> GettByTitulo(string titulo)
        {
            throw new NotImplementedException();
        }

        public Task<Postagem?> UpDate(Postagem postagem)
        {
            throw new NotImplementedException();
        }
    }
}
