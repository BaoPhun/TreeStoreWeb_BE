using Microsoft.EntityFrameworkCore;
using TreeStore.Models.Entities;

namespace TreeStore.Services
{
    public abstract class BaseServices
    {
        public readonly TreeStoreDBContext _db;
        public readonly ITreeStoreDBContextProcedures _sp;
        private TreeStoreDBContext db;

        public BaseServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp)
        {
            _db = db;
            _sp = sp;
        }

       
    }
}
