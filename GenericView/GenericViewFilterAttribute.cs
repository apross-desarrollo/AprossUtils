using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AprossUtils.GenericView
{

    public class GenericViewFilterForm
    {
        public virtual void PreProcess()
        {
            return;
        }

        public virtual void PostProcess()
        {
            return;
        }
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

        public GenericViewSearchAttribute(string pattern = null)
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
    public class GenericViewOrderAttribute : Attribute
    {

        // Private fields.
        private string pattern;


        // This constructor defines two required parameters: name and level.

        public GenericViewOrderAttribute(string pattern)
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
