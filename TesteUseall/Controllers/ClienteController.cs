using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TesteUseall.Models;
using System.Linq;
using TesteUseall.Data;

namespace TesteUseall.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly Context _context;

        public ClientesController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> ObterTodosClientes()
        {
            return Ok(await _context.Cliente.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> ObterClientePorId(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound(new { message = "Não foi encontrado o cliente com o id: " +id+ "." });
            }

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CriarCliente(Cliente cliente)
        {
            if (VerificarCnpjExistente(cliente.Cnpj))
            {
                return BadRequest(new { message = "Já existe um cliente com o CNPJ: " +cliente.Cnpj+ "." });
            }

            cliente.DataCadastro = DateTime.Now;
            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterClientePorId), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCliente(int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest(new { message = "O id da requisição não corresponde ao enviado." });
            }

            var clienteExistente = await _context.Cliente.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (clienteExistente == null)
            {
                return NotFound(new { message = "Não foi encontrado o cliente com o id: " + id + "." });
            }

            cliente.DataCadastro = clienteExistente.DataCadastro;

            try
            {
                _context.Cliente.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Erro ao atualizar o cliente." });
            }

            return Ok(new { message = "Cliente atualizado com sucesso.", cliente });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverCliente(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound(new { message = "Não foi encontrado o cliente com o id: " + id + "." });
            }

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cliente deletado com sucesso.", cliente });
        }

        private bool VerificarCnpjExistente(string cnpj)
        {
            return _context.Cliente.Any(e => e.Cnpj == cnpj);
        }
    }
}