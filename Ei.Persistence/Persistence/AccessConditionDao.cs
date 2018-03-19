using System;
using Newtonsoft.Json;

namespace Ei.Persistence
{
    public struct PostconditionDao
    {
        private string _condition;

        public string Condition {
            get => AccessConditionDao.FixCondition(_condition);
            set => _condition = value;
        }

        public string Action { get; set; }
    }

    public struct AccessConditionDao
    {
        public static string FixCondition(string condition) {
            if (!string.IsNullOrEmpty(condition)) {
                // correct bad conditions
                if (condition.IndexOf("return", StringComparison.Ordinal) == -1) {
                    condition = "return " + condition;
                }

                if (!condition.EndsWith(";")) {
                    condition += ";";
                }
            }

            return condition;
        }
        private string _precondition;

        [JsonIgnore]
        public string InstitutionStore => InstitutionDao.Instance.ClassName + ".Store";

        [JsonIgnore]
        public string WorkflowStore => InstitutionDao.Instance.CurrentWorkflowClass + ".Store";

        [JsonIgnore]
        public string ParameterStore {
            get {
                // we eliminate the value after first read as we only care about this once
                var action = InstitutionDao.Instance.CurrentAction ?? "ParameterState";
                InstitutionDao.Instance.CurrentAction = null;
                return action;
            }
        }

        [JsonIgnore]
        public string OrganisationStore {
            get {
                if (this.Organisation == null && InstitutionDao.Instance.Organisations.Count > 0) {
                    this.Organisation = InstitutionDao.Instance.Organisations[0].Id;
                }

                var organisation = InstitutionDao.Instance.OrganisationById(this.Organisation);
                if (organisation == null) {
                    throw new Exception($"Cannot find organisation with id: '{this.Organisation}'");
                }

                return organisation.ClassName + ".Store";
            }
        }

        [JsonIgnore]
        public string RoleStore {
            get {
                if (this.Role == null && InstitutionDao.Instance.Roles.Count > 0) {
                    this.Role = InstitutionDao.Instance.Roles[0].Id;
                }

                var role = InstitutionDao.Instance.RoleById(this.Role);
                if (role == null) {
                    throw new Exception($"Cannot find role with id '{this.Role}'");
                }

                return role.ClassName + ".Store";
            }
        }

        public static string GenerateCode(AccessConditionDao[] conditions) {
            return "null";
        }

        public string Role { get; set; }
        public string Organisation { get; set; }

        public string Precondition {
            get  => AccessConditionDao.FixCondition(_precondition);
            set { _precondition = value; }
        }

        public PostconditionDao[] Postconditions { get; set; }
    }
}