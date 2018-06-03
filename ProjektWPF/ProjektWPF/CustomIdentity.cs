using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Principal;

namespace ProjektWPF
{ 
    public class CustomIdentity : IIdentity //Identyfikator każdego użytkownika
    {
        public CustomIdentity(string name, string email, string[] roles)    //KOnstruktor dla użytkownika
        {
            Name = name;
            Email = email;
            Roles = roles;  //Rola uzytkownika w porgramie
        }

        public string Name { get; private set; }
        public string Email { get; private set; }
        public string[] Roles { get; private set; }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }
}