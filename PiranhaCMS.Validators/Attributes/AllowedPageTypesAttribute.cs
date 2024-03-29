﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PiranhaCMS.Validators.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AllowedPageTypesAttribute : ValidationAttribute
    {
        public Type[] Types { get; }
        public Availability Availability { get; }

        public AllowedPageTypesAttribute(Type[] types)
        {
            Types = types;
            Availability = Availability.Specific;
        }

        public AllowedPageTypesAttribute(Availability availability)
        {
            Availability = availability;
        }

        public override bool IsValid(object value)
        {
            return Availability switch
            {
                Availability.All => true,
                Availability.None => false,
                _ => Types.Any(x => x.Name.Equals((string)value))
            };
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} type is not allowed to be created!";
        }
    }

    public enum Availability
    {
        None,
        All,
        Specific
    }
}
