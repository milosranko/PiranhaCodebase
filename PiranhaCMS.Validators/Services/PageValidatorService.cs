using Microsoft.Extensions.Logging;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.Common.Extensions;
using PiranhaCMS.Validators.Attributes;
using PiranhaCMS.Validators.Helpers;
using PiranhaCMS.Validators.Models;
using PiranhaCMS.Validators.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PiranhaCMS.Validators.Services;

public class PageValidatorService : IPageValidatorService
{
    private IDictionary<string, AllowedPageTypesAttribute> pageTypeAllowedTypesCollection = new Dictionary<string, AllowedPageTypesAttribute>();
    private IDictionary<string, IEnumerable<PageValidatorModel>> pageValidatorCollection = new Dictionary<string, IEnumerable<PageValidatorModel>>();
    private readonly ILogger<PageValidatorService> _logger;

    public PageValidatorService(ILogger<PageValidatorService> logger)
    {
        _logger = logger;
    }

    public void Initialize(Assembly modelsAssembly)
    {
        var types = modelsAssembly.ExportedTypes;
        var blockTypes = types.Where(x =>
            x.GetTypeInfo().GetCustomAttributes().Any(y => y is BlockTypeAttribute));
        var pageTypes = types.Where(x =>
            x.GetTypeInfo().GetCustomAttributes().Any(y => y is PageTypeAttribute));
        var siteTypes = types.Where(x =>
            x.GetTypeInfo().GetCustomAttributes().Any(y => y is SiteTypeAttribute));

        GetPageTypeValidators(pageTypes);
        GetBlockTypeValidators(blockTypes);
        GetPageTypeAllowedTypes(pageTypes, siteTypes);
    }

    public void Validate(PageBase model)
    {
        if (model is not DynamicPage dynamicPage) return;

        try
        {
            ValidateAllowedPageType(dynamicPage);

            if (pageValidatorCollection.Any() && pageValidatorCollection.ContainsKey(dynamicPage.TypeId))
                foreach (var region in pageValidatorCollection[dynamicPage.TypeId])
                    ValidatorHelpers.ValidateRegion(dynamicPage, dynamicPage.TypeId, region, pageValidatorCollection);

            if (!dynamicPage.Blocks.Any()) return;

            foreach (var block in dynamicPage.Blocks)
            {
                ValidateBlock(block);

                if (block is BlockGroup blockGroup && blockGroup.Items.Any())
                {
                    foreach (var item in blockGroup.Items)
                    {
                        ValidateBlock(item);
                    }
                }
            }
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Unhandled exception occured while validating: {e.Message}");
            throw new ValidationException($"Unhandled exception occured while validating: {e.Message}");
        }
    }

    private void ValidateAllowedPageType(PageBase page)
    {
        if (page.ParentId.HasValue)
        {
            var parentPage = page.GetParentPage(true);

            if (pageTypeAllowedTypesCollection.ContainsKey(parentPage.TypeId))
                pageTypeAllowedTypesCollection[parentPage.TypeId].Validate(page.TypeId, page.TypeId);
        }
        else
        {
            var site = page.GetCurrentSite();

            if (pageTypeAllowedTypesCollection.ContainsKey(site.SiteTypeId))
                pageTypeAllowedTypesCollection[site.SiteTypeId].Validate(page.TypeId, page.TypeId);
        }
    }

    private void ValidateBlock(Block block)
    {
        var blockType = block.GetType();
        if (!pageValidatorCollection.ContainsKey(blockType.Name)) return;

        foreach (var pageValidatorModel in pageValidatorCollection[blockType.Name])
        {
            var field = Convert.ChangeType(
                blockType.GetProperty(pageValidatorModel.FieldName).GetValue(block, null),
                pageValidatorModel.FieldType) as IField;

            var va = ValidatorHelpers.GetValidationAttributes(pageValidatorCollection, blockType.Name, null, pageValidatorModel.FieldName);
            ValidatorHelpers.SetValidationContext(field, va);
        }
    }

    private void GetBlockTypeValidators(IEnumerable<Type> types)
    {
        foreach (var blockType in types)
        {
            var validationModels = new Collection<PageValidatorModel>();
            var fields = blockType.GetProperties()
                .Where(x => x.PropertyType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IField)));

            foreach (var field in fields)
            {
                var validationAttributes = field.GetCustomAttributes()
                    .Where(x => x.GetType().IsSubclassOf(typeof(ValidationAttribute)))
                    .Cast<ValidationAttribute>();

                if (!validationAttributes.Any()) continue;

                validationModels.Add(
                    PageValidatorModel.Create(null, field.Name, field.PropertyType, validationAttributes));
            }

            if (validationModels.Any())
                pageValidatorCollection.Add(blockType.Name, validationModels);
        }
    }

    private void GetPageTypeValidators(IEnumerable<Type> types)
    {
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
                pageValidatorCollection.Add(pageType.Name, validationModels);
        }
    }

    private void GetPageTypeAllowedTypes(IEnumerable<Type> pageTypes, IEnumerable<Type> siteTypes)
    {
        AllowedPageTypesAttribute attribute;

        foreach (var pageType in pageTypes)
        {
            attribute = pageType.GetCustomAttribute<AllowedPageTypesAttribute>();

            if (attribute != null)
            {
                pageTypeAllowedTypesCollection.Add(pageType.Name, attribute);
            }
        }

        foreach (var siteType in siteTypes)
        {
            attribute = siteType.GetCustomAttribute<AllowedPageTypesAttribute>();

            if (attribute != null)
            {
                pageTypeAllowedTypesCollection.Add(siteType.Name, attribute);
            }
        }
    }
}
