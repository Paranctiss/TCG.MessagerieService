using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCG.MessagerieService.Application.Consumer.Dto
{
    public class OfferDto
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public int BuyerId { get; set; }    
        public char OfferStatePostId { get; set; }
        public string MerchPostId { get; set; }
    
    }
}
