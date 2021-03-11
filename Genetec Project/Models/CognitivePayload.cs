using System;
using System.Collections.Generic;
using System.Text;

namespace Genetec_Project.Models
{
    class Lines
    {
        public string Text { get; set; }
    }

    class RecognitionResult
    {
        public Lines[] Lines { get; set; }
    }
    class CognitivePayload
    {
        public string Status {
            get;
            set;
        }
        public RecognitionResult RecognitionResult {
            get;
            set;
        }

    }
}
