using BlocNotasAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlocNotasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly Context _dbContext;
        private IConfiguration _config;

        public UsuariosController(Context dbContext, IConfiguration configuration)
        {
            try
            {
                _dbContext = dbContext;
                _config = configuration;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("Registro")]
        public async Task<ActionResult<Usuarios>> RegistroUsuarios(Usuarios usuario)
        {
            bool usuarioRegistrado = _dbContext.Usuarios.Any(u => u.NombreUsuario == usuario.NombreUsuario || u.CorreoElectronico == usuario.CorreoElectronico || u.Contraseña == usuario.Contraseña);

            if (usuarioRegistrado)
            {
                return BadRequest(new { Message = "El usuario ya está registrado" });
            }

            _dbContext.Usuarios.Add(usuario);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Nombre = usuario.NombreUsuario, Email = usuario.CorreoElectronico });
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult<Usuarios> LoginUsuarios(Usuarios usuario)
        {
            Usuarios usuarioEncontrado = _dbContext.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuario.NombreUsuario && u.CorreoElectronico == usuario.CorreoElectronico && u.Contraseña == usuario.Contraseña);  
            ActionResult<Usuarios> response;
  
            if (usuarioEncontrado == null)
            {
                return response = StatusCode(StatusCodes.Status404NotFound, new { Message = "Usuario debe registrarse" });
            }

            var token = GenerateToken(usuario);
            response = Ok(new {Token = token, IdUsuario = usuarioEncontrado.UsuarioId, Nombre = usuarioEncontrado.NombreUsuario, Email = usuarioEncontrado.CorreoElectronico });
            return response;

        }


        [HttpPost]
        [Authorize]
        [Route("Notas")]
        public async Task<ActionResult<Notas>> AgregarNotas(Notas nota)
        {
            _dbContext.Notas.Add(nota);
            await _dbContext.SaveChangesAsync();
            nota.NroNota = nota.NotaId;
            return Ok(new { UsuarioId= nota.UsuarioId,Titulo= nota.Titulo, Contenido = nota.Contenido, Etiquetas = nota.Etiquetas });
        }

        [HttpGet]
        [Authorize]
        [Route("ListarNotas")]
        public async Task<ActionResult<IEnumerable<Notas>>> GetNotas([FromQuery] int IdUsuario)
        {
            var NotasUsuario = await _dbContext.Notas.Where(n => n.UsuarioId == IdUsuario).ToListAsync();

            if (NotasUsuario == null)
            {
                return NotFound();
            }
            return NotasUsuario;
        }

        [HttpDelete]
        [Authorize]
        [Route("BorrarNota")]
        public async Task<ActionResult<Notas>> BorrarNota([FromQuery] int idnotas, [FromQuery] int UsuarioId)
        {
            var NotasUsuario = await _dbContext.Notas.FirstOrDefaultAsync(n => n.NotaId == idnotas && n.UsuarioId == UsuarioId);

            if(NotasUsuario == null){return NotFound();}

            _dbContext.Notas.Remove(NotasUsuario);  
            await _dbContext.SaveChangesAsync();  

            return Ok();
            
        }


        //FUNCIONES RELACIONADAS A LA GENERACION Y FIRMA DEL TOKEN


        private string GenerateToken(Usuarios users)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, users.NombreUsuario),
                new Claim(ClaimTypes.Email, users.CorreoElectronico)
                // Agregar más claims según necesites
            };

            var token = new JwtSecurityToken(
            null,
            null,
            claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: credentials
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    

}
}
