using System.Collections.Generic;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    /// <summary>
    ///     Summary description for IGenericAPI
    /// </summary>
    public interface IBaseAPI<T>
    {
        List<T> Search(DtoSearchFilterCategories filter);
        List<T> Search(DtoSearchFilter filter);
        List<T> Get();
        T Get(int id);
        DtoActionResult Put(int id, T tObject);
        DtoActionResult Post(T tObject);
        DtoActionResult Delete(int id);
        string GetCount();
    }
}