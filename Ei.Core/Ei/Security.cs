using System;
using System.Collections.Generic;
using System.Linq;

namespace Ei
{
  using Ei.Ontology;

  public struct AuthorisationInfo
  {
    internal readonly string Organisation;
    internal readonly string User;
    internal readonly string Password;
    internal Group[] Groups;

    public AuthorisationInfo(Institution ei, string user, string password, string organisation, Group[] groups) {
      User = user;
      Password = password;
      Organisation = organisation;
      Groups = groups;
    }

    public bool IsEmpty { get { return string.IsNullOrEmpty(this.User) && string.IsNullOrEmpty(this.Organisation); } }

    public override string ToString() {
      return string.Format("[{0}] {1}: {2}", this.Organisation, this.User, this.Password);
    }
  }

  public class Security
  {
    private List<AuthorisationInfo> authorisations;

    public void Init(List<AuthorisationInfo> auth, Institution ei) {
      if (auth != null) {
        this.authorisations = new List<AuthorisationInfo>(auth.Count);
        foreach (var authorisation in auth) {
          // check existence of roles and organisations
          foreach (var group in authorisation.Groups) {
            if (ei.RoleById(group.Role.Id) == null) {
              throw new ApplicationException("Authorisation contains non existing role: " + group.Role.Id);
            }
            if (group.Organisation != null && group.Organisation.Id != "0" && ei.OrganisationById(group.Organisation.Id) == null) {
              throw new ApplicationException("Authorisation contains non existing organisation: " + group.Organisation.Id);
            }
          }
          this.authorisations.Add(authorisation);
        }
      }
    }

    public AuthorisationInfo Authenticate(string organisation, string user, string password) {
      if (this.authorisations == null) {
        throw new Exception("You need to define authorisations!");
      }
      if (!string.IsNullOrEmpty(user)) {
        var authuser = this.authorisations.Find(w => w.User == user && w.Password == password);
        if (!authuser.IsEmpty) {
          return authuser;
        }
      }

      if (!string.IsNullOrEmpty(organisation)) {
        var authuser = this.authorisations.Find(w => w.Organisation == organisation && w.Password == password);
        if (!authuser.IsEmpty) {
          return authuser;
        }
      }

      return new AuthorisationInfo();
    }

    public static bool Authorise(AuthorisationInfo user, ICollection<Group> roles) {
      return roles.All(role => Access.IsInGroup(role, user.Groups));
    }


  }
}
