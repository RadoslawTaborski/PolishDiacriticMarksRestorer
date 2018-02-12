using System;
using System.Linq;
using System.Windows.Markup;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// EnumBindingSourceExtension Class allows binding enum
    /// </summary>
    public class EnumBindingSourceExtension : MarkupExtension
    {
        #region FIELDS
        private Type _enumType;
        #endregion

        #region PROPERTIES
        public Type EnumType
        {
            get => _enumType;
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
        #endregion

        #region CONSTRUCTORS
        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        }
        #endregion

        #region PUBLIC

        #endregion

        #region PRIVATE

        #endregion

        #region OVERRIDES
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
        #endregion

        #region  NESTED CLASS
        public class EnumerationMember
        {
            public object Value { get; set; }
        }
        #endregion
    }
}
