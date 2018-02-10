using System;
using System.Linq;
using System.Windows.Markup;

namespace PolishDiacriticMarksRestorer
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;


        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            EnumType = enumType;
        }

        public Type EnumType
        {
            get { return _enumType; }
            private set
            {
                if (_enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return (
                from object enumValue in enumValues
                select new EnumerationMember
                {
                    Value = enumValue,
                }).ToArray();
        }

        public class EnumerationMember
        {
            public object Value { get; set; }
        }
    }
}
