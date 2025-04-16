using System;
using System.Collections.Generic;
using System.Text;

namespace Nexia.models
{
    using System.Collections.Generic;

    public class DetectFaceResponse
    {
        public string request_id { get; set; }
        public int time_used { get; set; }
        public List<Face> faces { get; set; }
        public string image_id { get; set; }
        public int face_num { get; set; } // Number of faces detected
    }

    public class Face
    {
        public string face_token { get; set; }
        public FaceRectangle face_rectangle { get; set; }
        public Attributes attributes { get; set; }
    }

    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Attributes
    {
        public Gender gender { get; set; }
        public Age age { get; set; }
    }

    public class Gender
    {
        public string value { get; set; } // "Male" or "Female"
    }

    public class Age
    {
        public int value { get; set; } // Age value, e.g., 28
    }

}
