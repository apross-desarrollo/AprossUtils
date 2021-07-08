using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprossUtils.GenericView
{

    public class GenericViewFilterForm
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GenericViewFilterAttribute : Attribute
    {

        // Private fields.
        private string pattern;
      

        // This constructor defines two required parameters: name and level.

        public GenericViewFilterAttribute(string pattern)
        {
            this.pattern = pattern;
        }

        // Define Name property.
        // This is a read-only attribute.

        public virtual string Pattern
        {
            get { return pattern; }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GenericViewSearchAttribute : Attribute
    {

        // Private fields.
        private string pattern;


        // This constructor defines two required parameters: name and level.

        public GenericViewSearchAttribute(string pattern)
        {
            this.pattern = pattern;
        }

        // Define Name property.
        // This is a read-only attribute.

        public virtual string Pattern
        {
            get { return pattern; }
        }
    }
}
