﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.WebApplication.Models
{
    public class BillPay
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, ForeignKey("Transaction"), Display(Name = "Transaction")]
        public Guid TransactionId { get; set; }
        [Required, ForeignKey("BpayBiller")]
        public int BpayBillerCode { get; set; }
        public decimal Amount { get; set; }

        public BpayBiller BpayBiller { get; set; }
        public Transaction Transaction { get; set; }
        
    }
}