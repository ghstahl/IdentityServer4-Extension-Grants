using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;

namespace InternalizeIdentityServerApp.Services
{
    
    public class MyIdentityServerRequestTrackerEvaluator : IIdentityServerRequestTrackerEvaluator
    {
        public MyIdentityServerRequestTrackerEvaluator(IAllowRequestTrackerResult allowRequestTrackerResult)
        {
            _allowRequestTrackerResult = allowRequestTrackerResult;
        }
        private string _name;
        private IAllowRequestTrackerResult _allowRequestTrackerResult;

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_name))
                {
                    _name = nameof(MyIdentityServerRequestTrackerEvaluator);
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public async Task<IRequestTrackerResult> PreEvaluateAsync(IdentityServerRequestRecord requestRecord)
        {
            return _allowRequestTrackerResult;
        }
        public async Task<IRequestTrackerResult> PostEvaluateAsync(IdentityServerRequestRecord requestRecord, bool error)
        {
            return _allowRequestTrackerResult;
        }
    }
}
