using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AplicacionU1_API.Models;
namespace AplicacionU1_API.Repositories
{
    public class LoginRepository : Repository<Usuarios>
    {
        public override itesrcne_181g0138Context Context { get; }

        public LoginRepository(itesrcne_181g0138Context context) : base(context)
        {
            Context = context;
        }

        public Usuarios Login(string username, string password)
        {
            var user = Context.Set<Usuarios>().FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user != null)
                return user;
            else
                return null;
        }
    }
}
