using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomDump2017
{
    class ValueCapsule
    {
        int num;
        byte[] capsule;
        int value;

        public ValueCapsule(byte[] capsule)
        {
            this.num = capsule.Length;
            this.capsule = capsule;
            this.value = 0;
        }

        public void CalcValue()
        {
            int weight = 1;

            for(int i=0;i<num;i++)
            {
                value += capsule[i] * weight;
                weight *= 2;
            }
        }

        public int GetValue()
        {
            return value;
        }
    }

    class TagElement
    {
        public byte tag_high;
        public byte tag_low;
        public byte element_high;
        public byte element_low;

        public bool discover = false;
    }

    class Dictionary
    {
        public List<TagElement> tag_element = new List<TagElement>();

        public Dictionary()
        {
            tag_element.Add(new TagElement { tag_high = 0x02, tag_low = 0x00, element_high = 0x00, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x02, tag_low = 0x00, element_high = 0x01, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x02, tag_low = 0x00, element_high = 0x02, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x02, tag_low = 0x00, element_high = 0x03, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x02, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x02, tag_low = 0x00, element_high = 0x12, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x02, tag_low = 0x00, element_high = 0x02, element_low = 0x00 });

            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x05, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x08, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x16, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x18, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x20, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x21, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x22, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x30, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x31, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x32, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x50, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x60, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x70, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x80, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x90, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x08, tag_low = 0x00, element_high = 0x90, element_low = 0x10 });

            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x12, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x13, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x10, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x15, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x40, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x41, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x10, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x12, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x13, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x14, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x26, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x27, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x16, element_low = 0x13 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x20, element_low = 0x13 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x30, element_low = 0x13 });
            tag_element.Add(new TagElement { tag_high = 0x09, tag_low = 0x00, element_high = 0x40, element_low = 0x13 });

            tag_element.Add(new TagElement { tag_high = 0x10, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x10, tag_low = 0x00, element_high = 0x20, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x10, tag_low = 0x00, element_high = 0x30, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x10, tag_low = 0x00, element_high = 0x40, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x10, tag_low = 0x00, element_high = 0x20, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x10, tag_low = 0x00, element_high = 0x30, element_low = 0x10 });

            tag_element.Add(new TagElement { tag_high = 0x11, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x11, tag_low = 0x00, element_high = 0x11, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x11, tag_low = 0x00, element_high = 0x10, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x11, tag_low = 0x00, element_high = 0x10, element_low = 0x11 });
            tag_element.Add(new TagElement { tag_high = 0x11, tag_low = 0x00, element_high = 0x23, element_low = 0x11 });

            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x00, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x10, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x12, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x20, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x22, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x31, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x35, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x42, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x44, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x13, tag_low = 0x00, element_high = 0x46, element_low = 0x10 });

            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x15, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x20, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x21, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x22, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x23, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x24, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x25, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x50, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x80, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x81, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x83, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x84, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x85, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x86, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x87, element_low = 0x00 });        
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x88, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x89, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x91, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x93, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x94, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x00, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x20, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x00, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x01, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x50, element_low = 0x12 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x10, element_low = 0x13 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x12, element_low = 0x13 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x14, element_low = 0x13 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x16, element_low = 0x13 });
            tag_element.Add(new TagElement { tag_high = 0x18, tag_low = 0x00, element_high = 0x00, element_low = 0x51 });

            //tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x00, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x0E, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x11, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x12, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x13, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x32, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x37, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x52, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x60, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x40, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x20, tag_low = 0x00, element_high = 0x41, element_low = 0x10 });

            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x02, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x04, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x10, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x11, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x30, element_low = 0x00 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x00, element_low = 0x01 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x01, element_low = 0x01 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x02, element_low = 0x01 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x01, element_low = 0x03 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x50, element_low = 0x10 });
            tag_element.Add(new TagElement { tag_high = 0x28, tag_low = 0x00, element_high = 0x51, element_low = 0x10 });

            tag_element.Add(new TagElement { tag_high = 0xE0, tag_low = 0x7F, element_high = 0x10, element_low = 0x00 });
        }
    }
}
