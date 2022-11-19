using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class BillableAccountTaxInfo
{
	[JsonProperty("__typename")]
	public string Typename { get; set; }

	[JsonProperty("entity")]
	public string Entity { get; set; }

	[JsonProperty("business_country_code")]
	public string BusinessCountryCode { get; set; }

	[JsonProperty("predicated_business_country_code")]
	public string PredicatedBusinessCountryCode { get; set; }

	[JsonProperty("business_name")]
	public string BusinessName { get; set; }

	[JsonProperty("intl_address")]
	public IntlAddress IntlAddress { get; set; }

	[JsonProperty("tax_id")]
	public string TaxId { get; set; }

	[JsonProperty("second_tax_id")]
	public object SecondTaxId { get; set; }

	[JsonProperty("can_update_tax_id_fields")]
	public List<CanUpdateTaxIdField> CanUpdateTaxIdFields { get; set; }
}
