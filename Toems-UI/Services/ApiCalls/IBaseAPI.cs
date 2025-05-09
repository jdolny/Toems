using System.Collections.Generic;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    /// <summary>
    ///     Summary description for IGenericAPI
    /// </summary>
    public interface IBaseAPI<T>
    {
        Task<List<T>> Search(DtoSearchFilterCategories filter);
        Task<List<T>> Search(DtoSearchFilter filter);
        Task<List<T>> Get();
        Task<T> Get(int id);
        Task<DtoActionResult> Put(int id, T tObject);
        Task<DtoActionResult> Post(T tObject);
        Task<DtoActionResult> Delete(int id);
        Task<string> GetCount();
    }
}