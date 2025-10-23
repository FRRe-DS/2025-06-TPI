[ApiController]
[Route("api/shipments")]
public class ShippingController : ControllerBase
{
    private readonly IShippingRepository _repo;

    public ShippingController(IShippingRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IEnumerable<Shipping>> GetAll()
    {
        return await _repo.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Shipping>> GetById(string id)
    {
        var shipment = await _repo.GetByIdAsync(id);
        if (shipment == null) return NotFound();
        return shipment;
    }
}