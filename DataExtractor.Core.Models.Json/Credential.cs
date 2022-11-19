using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Credential
{
	[JsonProperty("__typename")]
	public string Typename { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("credential_id")]
	public string CredentialId { get; set; }

	[JsonProperty("needs_verification")]
	public bool NeedsVerification { get; set; }

	[JsonProperty("card_association")]
	public string CardAssociation { get; set; }

	[JsonProperty("card_association_name")]
	public string CardAssociationName { get; set; }

	[JsonProperty("last_four_digits")]
	public string LastFourDigits { get; set; }

	[JsonProperty("expires")]
	public string Expires { get; set; }

	[JsonProperty("campaign_info")]
	public List<object> CampaignInfo { get; set; }

	[JsonProperty("campaign_group_edit_permission")]
	public string CampaignGroupEditPermission { get; set; }

	[JsonProperty("current_balance")]
	public CurrentBalance CurrentBalance { get; set; }

	[JsonProperty("original_balance")]
	public OriginalBalance OriginalBalance { get; set; }

	[JsonProperty("enabled_for_postpay")]
	public bool EnabledForPostpay { get; set; }

	[JsonProperty("expiry_month")]
	public string ExpiryMonth { get; set; }

	[JsonProperty("expiry_year")]
	public string ExpiryYear { get; set; }

	[JsonProperty("is_expired")]
	public bool IsExpired { get; set; }

	[JsonProperty("network_tokenization_eligibility")]
	public string NetworkTokenizationEligibility { get; set; }

	[JsonProperty("fbin_tokenization_consent_received")]
	public bool FbinTokenizationConsentReceived { get; set; }
}
