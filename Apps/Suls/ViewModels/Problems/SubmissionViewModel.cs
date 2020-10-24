using System;
using System.Collections.Generic;
using System.Text;

namespace Suls.ViewModels.Problems
{
    public class SubmissionViewModel
    {
        public string Username { get; set; }

        public string SubmissionId { get; set; }

        public DateTime CreatedOn { get; set; }

        public int AchivedResult { get; set; }

        public int MaxPoints { get; set; }

    }
}
