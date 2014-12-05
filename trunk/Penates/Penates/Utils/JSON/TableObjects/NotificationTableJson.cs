using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects
{
    public class NotificationTableJson
    {

        public long NotificationID { get; set; }

        public string Date { get; set; }

        public string Message { get; set; }

        public string Address { get; set; }
    }


}