using System;
using System.ComponentModel.DataAnnotations;
using Abp.UI;

namespace AdministracionActivosSobrantes.CustomFields.Dto
{
    public class CustomFieldDto
    {
        public int Index { get; set; }

        public Guid Id { get; set; }

        public Guid AssetId { get; set; }

        [Required]
        [StringLength(CustomField.MaxNameLength)]
        public string Name { get; set; }

        public CustomFieldType CustomFieldType { get; set; }

        protected DateTime? ValueDate { get; private set; }

        public string Value { get; set; }// Todo: make this less accesible

        protected string ValueString { get; private set; }

        protected int? ValueInt { get; private set; }

        protected double? ValueDouble { get; private set; }

        public int Delete { get; set; }

        public int Saved { get; set; }

        public int Update { get; set; }

        public string ErrorDescription { get; set; }

        public string CompanyName { get; set; }

        public int ErrorCode { get; set; }

        public void SetValue(CustomFieldType customFieldType, object value)
        {
            CustomFieldType = customFieldType;
            if (customFieldType == CustomFieldType.DateTime)
            {
                ValueDate = DateTime.ParseExact(value.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (customFieldType == CustomFieldType.Double)
            {
                ValueDouble = Convert.ToDouble(value);
            }
            else if (customFieldType == CustomFieldType.Integer)
            {
                ValueInt = Convert.ToInt32(value);
            }
            else if (customFieldType == CustomFieldType.String)
            {
                ValueString = Convert.ToString(value);
            }

            Value = Convert.ToString(value);
        }

        public string GetStringValue()
        {
            if (CustomFieldType != CustomFieldType.String)
                return null;

            ValueString = Convert.ToString(Value);
            return ValueString;
        }

        public DateTime? GetDateValue()
        {
            if (CustomFieldType != CustomFieldType.DateTime)
                return null;

            ValueDate = DateTime.ParseExact(Value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            return ValueDate;
        }

        public int? GetIntValue()
        {
            if (CustomFieldType != CustomFieldType.Integer)
                return null;

            ValueInt = Convert.ToInt32(Value); 
            return ValueInt;
        }

        public double? GetDoubleValue()
        {
            if (CustomFieldType != CustomFieldType.Double)
                return null;

            ValueDouble = Convert.ToDouble(Value);
            return ValueDouble;
        }

        public CustomFieldDto()
        {
            Name = string.Empty;
        }
    }
}