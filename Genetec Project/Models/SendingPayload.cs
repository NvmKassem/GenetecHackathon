using System;
using System.Collections.Generic;
using System.Text;

namespace Genetec_Project.Models
{
    class SendingPayload
    {
        public string LicensePlateCaptureTime {
            get;
            set;
        }
        public string LicensePlate {
            get;
            set;
        }
        public string Latitude {
            get;
            set;
        }
        public string Longitude {
            get;
            set;
        }

        public string ContextImageReference {
            get;
            set;
        }

    }
}
