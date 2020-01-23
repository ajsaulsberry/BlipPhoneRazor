using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhoneNumbers;
using BlipPhoneRazor.Models;

namespace BlipPhoneRazor
{
    public class PhoneCheckModel : PageModel
    {
        private static PhoneNumberUtil _phoneUtil;
        
        [BindProperty(SupportsGet = true)]
        public PhoneNumberCheck PhoneNumberCheck { get; set; }

        public PhoneCheckModel()
        {
            _phoneUtil = PhoneNumberUtil.GetInstance();
        }

        public IActionResult OnGet()
        {
            PhoneNumberCheck.CountryCodeSelected = $"US";
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                PhoneNumber phoneNumber = _phoneUtil.Parse(PhoneNumberCheck.PhoneNumberRaw, PhoneNumberCheck.CountryCodeSelected);

                var valid = _phoneUtil.IsValidNumberForRegion(phoneNumber, PhoneNumberCheck.CountryCodeSelected);
                Debug.Print($"{phoneNumber} is {valid} for region {PhoneNumberCheck.CountryCodeSelected}");

                ModelState.FirstOrDefault(x => x.Key == $"{nameof(PhoneNumberCheck)}.{nameof(PhoneNumberCheck.CountryCodeSelected)}").Value.RawValue =
                    PhoneNumberCheck.CountryCodeSelected;

                ModelState.FirstOrDefault(x => x.Key == $"{nameof(PhoneNumberCheck)}.{nameof(PhoneNumberCheck.PhoneNumberRaw)}").Value.RawValue =
                    PhoneNumberCheck.PhoneNumberRaw;

                ModelState.FirstOrDefault(x => x.Key == $"{nameof(PhoneNumberCheck)}.{nameof(PhoneNumberCheck.Valid)}").Value.RawValue =
                    _phoneUtil.IsValidNumberForRegion(phoneNumber, PhoneNumberCheck.CountryCodeSelected);

                ModelState.FirstOrDefault(x => x.Key == $"{nameof(PhoneNumberCheck)}.{nameof(PhoneNumberCheck.PhoneNumberType)}").Value.RawValue =
                    _phoneUtil.GetNumberType(phoneNumber);

                ModelState.FirstOrDefault(x => x.Key == $"{nameof(PhoneNumberCheck)}.{nameof(PhoneNumberCheck.RegionCode)}").Value.RawValue =
                    _phoneUtil.GetRegionCodeForNumber(phoneNumber);

                ModelState.FirstOrDefault(x => x.Key == $"{nameof(PhoneNumberCheck)}.{nameof(PhoneNumberCheck.PhoneNumberFormatted)}").Value.RawValue =
                    _phoneUtil.FormatOutOfCountryCallingNumber(phoneNumber, PhoneNumberCheck.CountryCodeSelected);

                ModelState.FirstOrDefault(x => x.Key == $"{nameof(PhoneNumberCheck)}.{nameof(PhoneNumberCheck.PhoneNumberMobileDialing)}").Value.RawValue =
                    _phoneUtil.FormatNumberForMobileDialing(phoneNumber, PhoneNumberCheck.CountryCodeSelected, true);

                return Page();
            }
            catch (NumberParseException npex)
            {
                ModelState.AddModelError(npex.ErrorType.ToString(), npex.Message);
            }
            return Page();
        }
    }
}
