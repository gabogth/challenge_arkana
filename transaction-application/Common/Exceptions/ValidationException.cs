using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transaction_application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyCollection<ValidationError> Errors { get; }

        public ValidationException(IEnumerable<ValidationError> errors)
            : base("Validation failed")
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }
}
