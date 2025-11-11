public interface IBusinessManager
{
    Task<IEnumerable<BusinessListDto>> GetAllAsync();
    Task<BusinessDetailDto?> GetByIdAsync(Guid id);
    Task<BusinessListDto> CreateAsync(BusinessCreateDto dto);
}
