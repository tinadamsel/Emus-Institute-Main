using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Config
{
    public interface IGeneralConfiguration
    {
        public string AdminEmail { get; set; }
        public string DeveloperEmail { get; set; }
        public string PayStakApiKey { get; set; }
        public string PayStackBase { get; set; }
        public string CallbackUrl { get; set; }
        public string StaffCallbackUrl { get; set; }
        public decimal StaffEvaluationAmountNgn { get; set; }
        public decimal StudentEvaluationAmountNgn { get; set; }
        public decimal CohortEvaluationAmountNgn { get; set; }
        public string SiteBaseUrl { get; set; }
        public string AssessmentCallbackUrl { get; set; }
        public decimal AssessmentAmountGbp { get; set; }
        public decimal AssessmentAmountNgn { get; set; }
    }
}
