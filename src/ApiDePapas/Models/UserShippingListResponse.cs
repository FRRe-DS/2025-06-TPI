using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class UserShippingListResponse
    {
        [Required]
        public List<UserShippingListResponseItem> shipping_list { get; set; }
    }

    public class UserShippingListResponseItem
    {
        [Required]
        public int shipping_id { get; set; }

        [Required]
        public List<ProductQty> products { get; set; }

        [Required]
        public ShippingStatus status { get; set; }
    }
}