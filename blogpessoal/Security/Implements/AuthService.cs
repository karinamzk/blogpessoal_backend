using blogpessoal.Model;
using blogpessoal.Service;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace blogpessoal.Security.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService; 
        public AuthService(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<UserLogin?> Autenticar(UserLogin userLogin)
        {
            string FotoDefault = "https://i.imgur.com/I8MfmC8.png";

            if (userLogin is null || string.IsNullOrEmpty(userLogin.Usuario) || 
                string.IsNullOrEmpty(userLogin.Senha))
            return null;

            // usuarioLogin = email

            var BuscaUsuario = await _userService.GetByUsuario(userLogin.Usuario); 
            
            if(BuscaUsuario is null)
                return null;

            // usuarioLongin.Senha = 123456
            // buscausuario.Senha = aidhsusduaidhauihdauhaid

            if(!BCrypt.Net.BCrypt.Verify(userLogin.Senha, BuscaUsuario.Senha))
                return null;

            // Manipula o Token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Codifica a senha 
            var tokenKey = Encoding.UTF8.GetBytes(Settings.Secret);

            // Armazena as informaçoes necessarias 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userLogin.Usuario)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey
                (tokenKey), 
                SecurityAlgorithms.HmacSha256Signature ),
            };

            // Cria o Token com todas as informaçoes necessarias 
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 
            userLogin.Id = BuscaUsuario.Id;
            userLogin.Nome = BuscaUsuario.Nome;
            userLogin.Foto = BuscaUsuario.Foto is null ? FotoDefault : BuscaUsuario.Foto;
            userLogin.Token = "Bearer " + tokenHandler.WriteToken(token).ToString();
            userLogin.Senha = "";

            return userLogin;

        }
    }
}
