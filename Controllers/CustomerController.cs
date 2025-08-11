using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyAssetsManagerBackend.data;
using MyAssetsManagerBackend.Entities;

namespace MyAssetsManagerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IMongoCollection<Customer>? _customers;
    
    public CustomerController(MongoDbService mongoDbService)
    {
        this._customers = mongoDbService.Database?.GetCollection<Customer>("customers");
    }

    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<Customer>> Get()
    {
        return await this._customers.Find(customer => true).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer?>> GetById(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(x => x.Id, id);
        var customer = _customers.Find(filter).FirstOrDefault();
        return customer is not null ? Ok(customer) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create(Customer customer)
    {
        await _customers.InsertOneAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut]
    public async Task<ActionResult<Customer>> Update(Customer customer)
    {
        var filter = Builders<Customer>.Filter.Eq(x => x.Id, customer.Id);
        await _customers.ReplaceOneAsync(filter, customer);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(x => x.Id, id);
        await _customers.DeleteOneAsync(filter);
        return Ok();
    }

}