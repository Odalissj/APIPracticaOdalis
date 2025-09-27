using Microsoft.AspNetCore.Mvc;
using Dapper;
using APIPracticaExamen.Data;
using APIPracticaExamen.Dtos;

namespace APIPracticaExamen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly DbConnectionFactory _factory;
    public ProductosController(DbConnectionFactory factory) => _factory = factory;

    // GET /api/productosOdalis
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using var cn = _factory.Create();
        var sql = "SELECT Id, Nombre, Precio FROM dbo.ProductosOdalis ORDER BY Id;";
        var data = await cn.QueryAsync(sql); // devuelve dinámico
        return Ok(data);
    }

    // GET /api/productosOdalis/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var cn = _factory.Create();
        var sql = "SELECT Id, Nombre, Precio FROM dbo.ProductosOdalis WHERE Id=@id;";
        var item = await cn.QuerySingleOrDefaultAsync(sql, new { id });
        return item is null ? NotFound() : Ok(item);
    }

    // POST /api/productosOdalis
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        using var cn = _factory.Create();
        var sql = @"INSERT INTO dbo.ProductosOdalis (Nombre, Precio)
                    VALUES (@Nombre, @Precio);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        var id = await cn.ExecuteScalarAsync<int>(sql, dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id, dto.Nombre, dto.Precio });
    }

    // PUT /api/productosOdalis/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
    {
        using var cn = _factory.Create();
        var sql = "UPDATE dbo.ProductosOdalis SET Nombre=@Nombre, Precio=@Precio WHERE Id=@Id;";
        var rows = await cn.ExecuteAsync(sql, new { Id = id, dto.Nombre, dto.Precio });
        return rows == 0 ? NotFound() : NoContent();
    }

    // DELETE /api/productosOdalis/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var cn = _factory.Create();
        var sql = "DELETE FROM dbo.ProductosOdalis WHERE Id=@id;";
        var rows = await cn.ExecuteAsync(sql, new { id });
        return rows == 0 ? NotFound() : NoContent();
    }

    // (Opcional, solo dev) Crear tabla si no existe
    [HttpPost("admin/create-table")]
    public async Task<IActionResult> CreateTable()
    {
        using var cn = _factory.Create();
        var sql = """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='ProductosOdalis' AND schema_id = SCHEMA_ID('dbo'))
            CREATE TABLE dbo.ProductosOdalis(
              Id INT IDENTITY(1,1) PRIMARY KEY,
              Nombre NVARCHAR(100) NOT NULL,
              Precio DECIMAL(18,2) NOT NULL
            );
        """;
        await cn.ExecuteAsync(sql);
        return Ok("Tabla 'ProductosOdalis' lista.");
    }
}
