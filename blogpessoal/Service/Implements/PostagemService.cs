using blogpessoal.Model;
using blogpessoal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace blogpessoal.Service.Implements
{
    public class PostagemService : IPostagemService
    {
        private readonly AppDbContext _context;
        public PostagemService(AppDbContext context)
        {
            _context = context; 
        }

        public async Task<IEnumerable<Postagem>> GettAll()
        {
            return await _context.Postagens
                .AsNoTracking()
                .Include(p => p.Tema)
                .Include(p => p.Usuario)
                .ToListAsync();
        }
        public async Task<Postagem?> GetById(long id)
        {
            try
            {
                var Postagem = await _context.Postagens
                    .AsNoTracking()
                    .Include(p => p.Tema)
                    .Include(p => p.Usuario)
                    .FirstAsync(i => i.Id == id);

                return Postagem;
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<Postagem>> GetByTitulo(string titulo)
        {
            var Postagem = await _context.Postagens
                                .Include(p => p.Tema)
                                .Include(p => p.Usuario)
                                .Where(p => p.Titulo.ToUpper()
                                  .Contains(titulo.ToUpper())
        )
                                
                                .ToListAsync();
            return Postagem;
        }

        public async Task<Postagem?> Create(Postagem postagem)
        {
            if (postagem.Tema is not null)
            {
                var BuscarTema = await _context.Temas.FindAsync(postagem.Tema.Id);

                if (BuscarTema is null)
                    return null;
            }

            postagem.Tema = postagem.Tema is not null ? _context.Temas
                .FirstOrDefault(t => t.Id == postagem.Tema.Id) : null;

            postagem.Usuario = postagem.Usuario is not null ?
                await _context.Users.FirstOrDefaultAsync
                (u => u.Id == postagem.Usuario.Id) : null;

            await _context.Postagens.AddAsync(postagem);
            await _context.SaveChangesAsync();

            return postagem;
        }

        public async Task<Postagem?> UpDate(Postagem postagem)
        {
            var PostagemUpdate = await _context.Postagens.FindAsync(postagem.Id);

            if (PostagemUpdate is null)
                return null;

            if (postagem.Tema is not null)
            {
                var BuscarTema = await _context.Temas.FindAsync(postagem.Tema.Id);

                if (BuscarTema is null)
                    return null;

            }

            postagem.Tema = postagem.Tema is not null ? _context.Temas.FirstOrDefault
                (t => t.Id == postagem.Tema.Id) : null;

            _context.Entry(PostagemUpdate).State = EntityState.Detached;
            _context.Entry(postagem).State = EntityState.Modified;
            await _context.SaveChangesAsync(); 

            return postagem;
        }
        
        public async Task Delete(Postagem postagem)
        {
            _context.Postagens.Remove(postagem);

            await _context.SaveChangesAsync();
        }
    }
}
