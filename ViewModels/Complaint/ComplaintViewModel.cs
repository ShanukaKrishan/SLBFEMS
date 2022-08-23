using SLBFEMS.Models;
using System;
using System.Collections.Generic;

namespace SLBFEMS.ViewModels.Complaint
{
    public class ComplaintViewModel
    {
        public ComplaintDataModel ComplaintStatus { get; set; }

        public List<ComplaintMessagesViewModel> MessageThred { get; set; }
    }

    public class ComplaintMessagesViewModel
    {
        public string Complaint { get; set; }

        public string Nic { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
