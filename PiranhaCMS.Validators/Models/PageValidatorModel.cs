﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PiranhaCMS.Validators.Models
{
    public struct PageValidatorModel
    {
        public string RegionName { get; private set; }
        public string FieldName { get; private set; }
        public Type FieldType { get; private set; }
        public IEnumerable<ValidationAttribute> Validators { get; private set; }

        public static PageValidatorModel Create(
            string regionName,
            string fieldName,
            Type fieldType,
            IEnumerable<ValidationAttribute> validators)
        {
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("", nameof(fieldName));
            if (fieldType == null) throw new ArgumentException("", nameof(fieldType));
            if (validators == null || !validators.Any()) throw new ArgumentException("", nameof(validators));

            return new PageValidatorModel
            {
                RegionName = regionName,
                FieldName = fieldName,
                FieldType = fieldType,
                Validators = validators
            };
        }
    }
}
