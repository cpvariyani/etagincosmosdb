﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos_CRUD.Models
{
    public class UserInfo
    {
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UniqueNumber { get; set; }
        public string _etag { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Product
    {
        public string id { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Order
    {
        public string id { get; set; }
        public Product Product { get; set; }
        public UserInfo User { get; set; }

        public bool IsPaymentDone { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
