using BlogPessoalTest.Factory;
using FluentAssertions;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net;
using System.Text;
using Xunit.Extensions.Ordering;
using blogpessoal.Model;
using System.Net.Http.Json;

namespace BlogPessoalTest.Controller
{
    public class UserControllerTest : IClassFixture<WebAppFactory>
    {
        protected readonly WebAppFactory _factory;
        protected HttpClient _client;

        private readonly dynamic token;
        
        private string Id { get; set; } = string.Empty;

        public UserControllerTest(WebAppFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            token = GetToken();
        }

        private static dynamic GetToken()
        {
            dynamic data = new ExpandoObject();
            data.sub = "root@root.com";
            return data;
        }

        [Fact, Order(1)]
        public async Task DeveCriarUmUsuario()
        {
            var novoUsuario = new Dictionary<string, string>
            {
                {"nome", "Ingrid" },
                {"usuario", "ingrid@email.com.br" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            resposta.EnsureSuccessStatusCode();

            resposta.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact, Order(2)]
        public async Task DeveDarErroEmail()
        {
            var novoUsuario = new Dictionary<string, string>
            {
                {"nome", "Ingrid" },
                {"usuario", "ingridemail.com" },
                {"senha", "123456789445" },
                {"foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            // resposta.EnsureSuccessStatusCode();

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact, Order(3)]
        public async Task NãoDeveCriarUsuarioDuplicado()
        {
            var novoUsuario = new Dictionary<string, string>
            {
                {"nome", "Karina" },
                {"usuario", "karina@email.com" },
                {"senha", "123456789" },
                {"foto", " " }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            // resposta.EnsureSuccessStatusCode();

            resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact, Order(4)]
        public async Task DeveListarTodosUsuarios()
        {
            _client.SetFakeBearerToken((object)token);

            var resposta = await _client.GetAsync("/usuarios/all");

            resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact, Order(5)]
        public async Task DeveAtualizarUmUsuario()
        {
            var novoUsuario = new Dictionary<string, string>()
            {
                {"nome", "João" },
                {"usuario", "Joao@email.com.br" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicaoPost = new StringContent(usuarioJson, Encoding.UTF8,
                "application/json");
            var respostaPost = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicaoPost);

            var corpoRespostaPost = await respostaPost.Content.ReadFromJsonAsync<User>();
            if (corpoRespostaPost != null)
            {
                Id = corpoRespostaPost.Id.ToString();
            }

            var atualizaUsuario = new Dictionary<string, string>()
            {
                {"id", Id },
                {"nome", "João Maia" },
                {"usuario", "Joao123@email.com.br" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            var usuarioJsonAtualizar = JsonConvert.SerializeObject(atualizaUsuario);

            var corpoRequisicaoAtualizar = new StringContent(usuarioJsonAtualizar, Encoding.UTF8, "application/json");

            _client.SetFakeBearerToken((object)token);

            var respostaPut = await _client.PutAsync("/usuarios/atualizar", corpoRequisicaoAtualizar);

            respostaPut.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Fact, Order(6)]
        public async Task DeveAutenticarUmUsuario()
        {
           var novoUsuario = new Dictionary<string, string>()
           {
              {"usuario", "ingrid@email.com.br" },
              {"senha", "123456789" },
           };

           var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
           var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

           var resposta = await _client.PostAsync("/usuarios/logar", corpoRequisicao);

           resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact, Order(7)]
        public async Task DeveListarUmUsuario()
        {
            var listaUsuario = new Dictionary<string, string>()
            {
                {"id", Id },
 
            };

            var usuarioJson = JsonConvert.SerializeObject(listaUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            _client.SetFakeBearerToken((object)token);

            var resposta = await _client.GetAsync("/usuarios/all");

            resposta.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}
