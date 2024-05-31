using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using PiranhaCMS.Validators.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Reflection;

namespace PiranhaCMS.Validators.Helpers;

public static class ValidatorHelpers
{
    public static IDictionary<string, IEnumerable<PageValidatorModel>> GetPageTypeValidators(IEnumerable<Type> types)
    {
        var coll = new Dictionary<string, IEnumerable<PageValidatorModel>>();

        foreach (var pageType in types)
        {
            IEnumerable<PropertyInfo> fields;
            var validationModels = new Collection<PageValidatorModel>();
            var regions = pageType.GetProperties()
                .Where(x => x.GetCustomAttributes().Any(y => y is RegionAttribute));

            foreach (var region in regions)
            {
                if (region.PropertyType.IsGenericType)
                {
                    var regionType = region.PropertyType.GetGenericArguments().Single();

                    fields = regionType.GetProperties()
                        .Where(x => x.PropertyType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IField)));
                }
                else
                {
                    fields = region.PropertyType.GetProperties()
                        .Where(x => x.PropertyType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IField)));
                }

                foreach (var field in fields)
                {
                    var validationAttributes = field.GetCustomAttributes()
                        .Where(x => x.GetType().IsSubclassOf(typeof(ValidationAttribute)))
                        .Cast<ValidationAttribute>();

                    if (!validationAttributes.Any()) continue;

                    validationModels.Add(
                        PageValidatorModel.Create(region.Name, field.Name, field.PropertyType, validationAttributes));
                }
            }

            if (validationModels.Any())
                coll.Add(pageType.Name, validationModels);
        }

        return coll;
    }

    public static void ValidateRegion(
        object content,
        string typeId,
        PageValidatorModel region,
        IDictionary<string, IEnumerable<PageValidatorModel>> validatorCollection)
    {
        if (content is IDynamicContent dynamicContent)
        {
            if ((dynamicContent.Regions as IDictionary<string, object>)[region.RegionName] is RegionList<ExpandoObject> regionList)
            {
                foreach (var fields in regionList)
                {
                    foreach (var field in fields.Where(x => x.Value is IField && x.Key.Equals(region.FieldName)))
                    {
                        var va = GetValidationAttributes(validatorCollection, typeId, region.RegionName, region.FieldName);
                        SetValidationContext(field.Value as IField, va);
                    }
                }
            }
            else
            {
                if (!(((dynamicContent.Regions as IDictionary<string, object>)[region.RegionName] as
                    IDictionary<string, object>)[region.FieldName] is IField field)) return;

                var va = GetValidationAttributes(validatorCollection, typeId, region.RegionName, region.FieldName);
                SetValidationContext(field, va);
            }
        }
        else
        {
            //TODO Check if object content has properties decorated with region attribute, validate those properties so unit tests check validation rules
        }
    }

    public static IEnumerable<ValidationAttribute> GetValidationAttributes(
        IDictionary<string, IEnumerable<PageValidatorModel>> validatorCollection,
        string typeId,
        string regionName,
        string fieldName)
    {
        return string.IsNullOrEmpty(regionName)
            ? validatorCollection[typeId]
                .Where(x => x.FieldName.Equals(fieldName))
                .Select(x => x.Validators)
                .Single()
            : validatorCollection[typeId]
                .Where(x => x.RegionName.Equals(regionName) &&
                            x.FieldName.Equals(fieldName))
                .Select(x => x.Validators)
                .Single();
    }

    public static void SetValidationContext(IField field, IEnumerable<ValidationAttribute> va)
    {
        var vc = new ValidationContext(field);

        switch (field)
        {
            case StringField stringField:
                Validator.ValidateValue(stringField.Value, vc, va);
                break;
            case TextField textField:
                Validator.ValidateValue(textField.Value, vc, va);
                break;
            case HtmlField htmlField:
                Validator.ValidateValue(htmlField.Value, vc, va);
                break;
            case ImageField imageField:
                Validator.ValidateValue(imageField.Id, vc, va);
                break;
            case DocumentField documentField:
                Validator.ValidateValue(documentField.Id, vc, va);
                break;
            case CheckBoxField checkboxField:
                Validator.ValidateValue(checkboxField.Value, vc, va);
                break;
            case DateField dateField:
                Validator.ValidateValue(dateField.Value, vc, va);
                break;
            case NumberField numberField:
                Validator.ValidateValue(numberField.Value, vc, va);
                break;
            case PageField pageField:
                Validator.ValidateValue(pageField.Id, vc, va);
                break;
            case PostField postField:
                Validator.ValidateValue(postField.Id, vc, va);
                break;
            case SelectFieldBase selectField:
                Validator.ValidateValue(selectField.EnumValue, vc, va);
                break;
            default:
                return;
        }
    }
}
