using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using AdministracionActivosSobrantes.Assets;
using AdministracionActivosSobrantes.Common;
using AdministracionActivosSobrantes.Filters;

namespace AdministracionActivosSobrantes.CustomFields
{
    public class CustomField : Entity<Guid>, IFullAuditedCustom,ITenantCompanyName
    {
        public const int MaxNameLength = 350;
        public const int MaxValueLength = 256;

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(MaxValueLength)]
        public string Value { get; private set; }

        public DateTime? ValueDate { get; private set; }

        [StringLength(MaxValueLength)]
        public string ValueString { get; private set; }

        public int? ValueInt { get; private set; }

        public double? ValueDouble { get; private set; }

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public CustomFieldType CustomFieldType { get; private set; }

        //--Auditing
        public Guid? DeleterUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatorUserId { get; set; }
        /// <summary>
        /// We don't make constructor public and forcing to create clients using <see cref="Create"/> method.
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        protected CustomField()
        {

        }

        public static CustomField Create(string name, CustomFieldType customFieldType, Guid assetId, string value, string valueString, DateTime? valueDate, int? valueInteger, double? valueDouble, Guid creatorid, DateTime createDateTime,string companyName)
        {
            var @entity = new CustomField
            {
                Id = Guid.NewGuid(),
                Name = name,
                CustomFieldType = customFieldType,
                AssetId = assetId,
                Value = value,
                ValueDate = valueDate,
                ValueInt = valueInteger,
                ValueDouble = valueDouble,
                ValueString = valueString,
                CreatorUserId = creatorid,
                CreationTime = createDateTime,
                CompanyName = companyName,
                IsDeleted = false
            };
            return @entity;
        }

        public void SetValue(CustomFieldType customFieldType, object value)
        {
            CustomFieldType = customFieldType;
            if (customFieldType == CustomFieldType.DateTime)
            {
                ValueDate = DateTime.ParseExact(value.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                ValueDouble = null;
                ValueInt = null;
                ValueString = null;
            }
            else if (customFieldType == CustomFieldType.Double)
            {
                ValueDouble = Convert.ToDouble(value);
                ValueDate = null;
                ValueInt = null;
                ValueString = null;
            }
            else if (customFieldType == CustomFieldType.Integer)
            {
                ValueInt = Convert.ToInt32(value);
                ValueDate = null;
                ValueDouble = null;
                ValueString = null;
            }
            else if (customFieldType == CustomFieldType.String)
            {
                ValueString = Convert.ToString(value);
                ValueDate = null;
                ValueDouble = null;
                ValueInt = null;
            }
            Value = Convert.ToString(value);
        }

        [StringLength(250)]
        public string CompanyName { get; set; }
    }

    public enum CustomFieldType
    {
        String,
        DateTime,
        Double,
        Integer
    }
}
