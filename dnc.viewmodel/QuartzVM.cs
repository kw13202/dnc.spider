using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.viewmodel
{
    public class QuartzVM
    {
        public int TypeId { get; set; }
        public int Id { get; set; }


        public bool verify(out string msg)
        {
            msg = string.Empty;
            bool result = true;
            StringBuilder sb = new StringBuilder();

            switch (this.TypeId)
            {
                case 0:
                    {
                        if (this.Id < 1)
                        {
                            result = false;
                            sb.AppendLine("Id必须大于0");
                        } 
                    }
                    break;
            }
            if (sb.Length > 0)
            {
                msg = sb.ToString();
            }

            return result;
        }
    }
}
